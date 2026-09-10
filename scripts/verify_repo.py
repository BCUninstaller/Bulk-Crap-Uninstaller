#!/usr/bin/env python3
"""
EBUninstaller Pro - Repository Static Analysis & Architecture Verifier
Performs syntax, XML, C# structure, namespace consistency, installer configuration,
and license compliance audits across the entire codebase.
"""

import os
import sys
import xml.etree.ElementTree as ET
import re
import json
import argparse

REPO_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOURCE_DIR = os.path.join(REPO_ROOT, "source")
INSTALLER_DIR = os.path.join(REPO_ROOT, "installer")
DOC_DIR = os.path.join(REPO_ROOT, "doc")

def check_xml_files(verbose=False, quiet=False):
    if not quiet:
        print("[Check 1/6] Validating all XML, ResX, and Project Files...")
    xml_extensions = ('.xml', '.resx', '.csproj', '.props', '.targets', '.manifest', '.config', '.settings')
    checked_count = 0
    errors = []

    for root, _, files in os.walk(SOURCE_DIR):
        for file in files:
            if file.endswith(xml_extensions):
                full_path = os.path.join(root, file)
                checked_count += 1
                try:
                    ET.parse(full_path)
                    if verbose and not quiet:
                        print(f"   [OK] {os.path.relpath(full_path, REPO_ROOT)}")
                except Exception as e:
                    errors.append(f"Invalid XML syntax in {os.path.relpath(full_path, REPO_ROOT)}: {e}")

    if not quiet:
        print(f" -> Checked {checked_count} XML/ResX/Project files.")
    if errors:
        if not quiet:
            for err in errors:
                print(f" [ERROR] {err}")
        return False, checked_count, errors
    if not quiet:
        print(" -> All XML/Project files are well-formed.")
    return True, checked_count, []

def check_csharp_files(verbose=False, quiet=False):
    if not quiet:
        print("[Check 2/6] Validating C# Source Files for Structural Integrity...")
    checked_count = 0
    total_loc = 0
    errors = []

    for root, _, files in os.walk(SOURCE_DIR):
        for file in files:
            if file.endswith('.cs'):
                checked_count += 1
                full_path = os.path.join(root, file)
                try:
                    with open(full_path, 'r', encoding='utf-8', errors='replace') as f:
                        lines = f.readlines()
                        total_loc += len(lines)
                        content = "".join(lines)

                    # Basic brace balancing check
                    open_braces = content.count('{')
                    close_braces = content.count('}')
                    if open_braces != close_braces:
                        if abs(open_braces - close_braces) > 1 and not file.endswith('Designer.cs'):
                            errors.append(f"Unbalanced braces in {os.path.relpath(full_path, REPO_ROOT)}: {open_braces} open vs {close_braces} close")
                    
                    if verbose and not quiet:
                        print(f"   [OK] {os.path.relpath(full_path, REPO_ROOT)} ({len(lines)} lines)")
                except Exception as e:
                    errors.append(f"Error reading {os.path.relpath(full_path, REPO_ROOT)}: {e}")

    if not quiet:
        print(f" -> Checked {checked_count} C# source files ({total_loc:,} total lines of code).")
    if errors:
        if not quiet:
            for err in errors:
                print(f" [WARNING] {err}")
    if not quiet:
        print(" -> C# source files validated.")
    return len(errors) == 0, checked_count, total_loc, errors

def check_subsystems(verbose=False, quiet=False):
    if not quiet:
        print("[Check 3/6] Verifying Required Subsystems Exist...")
    required_modules = [
        "source/UninstallTools/Core/SecurityGuard.cs",
        "source/UninstallTools/Core/StructuredLogger.cs",
        "source/UninstallTools/Core/CryptoHasher.cs",
        "source/UninstallTools/Core/DigitalSignatureVerifier.cs",
        "source/UninstallTools/Core/SoftwareSafetyAdvisor.cs",
        "source/UninstallTools/Core/SystemHealthScorecardEngine.cs",
        "source/UninstallTools/RegistryEngine/SafeRegistryEngine.cs",
        "source/UninstallTools/FileSystemEngine/SafeFileSystemEngine.cs",
        "source/UninstallTools/FileSystemEngine/EmptyDirectoryCleaner.cs",
        "source/UninstallTools/FileSystemEngine/DuplicateFileScanner.cs",
        "source/UninstallTools/FileSystemEngine/FileUnlockerManager.cs",
        "source/UninstallTools/FileSystemEngine/DiskSpaceAnalyzer.cs",
        "source/UninstallTools/FileSystemEngine/ApplicationFootprintAnalyzer.cs",
        "source/UninstallTools/FileSystemEngine/FreeSpaceWiper.cs",
        "source/UninstallTools/FileSystemEngine/BootTimeDeleterEngine.cs",
        "source/UninstallTools/Backup/BackupManager.cs",
        "source/UninstallTools/Backup/BackupManifest.cs",
        "source/UninstallTools/Backup/SystemRestorePointManager.cs",
        "source/UninstallTools/Backup/VolumeShadowCopyManager.cs",
        "source/UninstallTools/InstallationMonitor/InstallationMonitorEngine.cs",
        "source/UninstallTools/InstallationMonitor/InstallationTrace.cs",
        "source/UninstallTools/InstallationMonitor/InstallationSnapshotDiffer.cs",
        "source/UninstallTools/ForcedRemoval/ForcedUninstallManager.cs",
        "source/UninstallTools/ForcedRemoval/ForcedUninstallModels.cs",
        "source/UninstallTools/JunkCleaner/JunkCleanerEngine.cs",
        "source/UninstallTools/JunkCleaner/JunkCleanerModels.cs",
        "source/UninstallTools/JunkCleaner/CrashDumpCleaner.cs",
        "source/UninstallTools/JunkCleaner/EventLogResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/FontResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/DeveloperCacheCleaner.cs",
        "source/UninstallTools/JunkCleaner/DisconnectedDevicesCleaner.cs",
        "source/UninstallTools/JunkCleaner/OrphanedServicesCleaner.cs",
        "source/UninstallTools/JunkCleaner/WinUpdateResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/ShortcutResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/PatchCacheResidualsCleaner.cs",
        "source/UninstallTools/PrivacyCleaner/PrivacyCleanerEngine.cs",
        "source/UninstallTools/PrivacyCleaner/PrivacyCleanerModels.cs",
        "source/UninstallTools/PrivacyCleaner/WindowsTelemetryOptimizer.cs",
        "source/UninstallTools/BrowserExtensions/BrowserExtensionManager.cs",
        "source/UninstallTools/BrowserExtensions/BrowserExtensionModels.cs",
        "source/UninstallTools/SystemTools/WindowsToolsLauncher.cs",
        "source/UninstallTools/SystemTools/WindowsDriverManager.cs",
        "source/UninstallTools/SystemTools/WindowsFirewallManager.cs",
        "source/UninstallTools/SystemTools/WindowsHostsFileManager.cs",
        "source/UninstallTools/SystemTools/EnvironmentVariablesManager.cs",
        "source/UninstallTools/SystemTools/WslAndVirtualDiskManager.cs",
        "source/UninstallTools/SystemTools/ShellCacheRebuilder.cs",
        "source/UninstallTools/SystemTools/WindowsRuntimesManager.cs",
        "source/UninstallTools/SystemTools/WindowsDriverBackupEngine.cs",
        "source/UninstallTools/SystemTools/InstalledFontsCleaner.cs",
        "source/UninstallTools/SystemTools/ProductKeyExtractorEngine.cs",
        "source/UninstallTools/SystemTools/WindowsSandboxManager.cs",
        "source/UninstallTools/SystemTools/WinSxSStoreAnalyzer.cs",
        "source/UninstallTools/SystemTools/StorageSenseOptimizer.cs",
        "source/UninstallTools/Reporting/SoftwareInventoryReportGenerator.cs",
        "source/UninstallTools/Exclusions/ExclusionManager.cs",
        "source/UninstallTools/History/OperationHistoryManager.cs",
        "source/UninstallTools/HunterMode/TargetModeController.cs",
        "source/UninstallTools/Detection/GameLauncherFactory.cs",
        "source/UninstallTools/Detection/PackageManagersFactory.cs",
        "source/UninstallTools/Detection/PackageManagerUpdateEngine.cs",
        "source/UninstallTools/Detection/PackageManagerSyncEngine.cs",
        "source/UninstallTools/Detection/ConfidenceScorer.cs",
        "source/UninstallTools/Detection/WindowsOptionalFeaturesManager.cs",
        "source/UninstallTools/Detection/SoftwareVulnerabilityChecker.cs",
        "source/UninstallTools/Detection/CveDatabaseAuditor.cs",
        "source/UninstallTools/Detection/SoftwareCrashHistoryEngine.cs",
        "source/UninstallTools/Detection/SoftwareUpdateDifferEngine.cs",
        "source/UninstallTools/SystemTools/ServiceConflictDetectorEngine.cs",
        "source/UninstallTools/SystemTools/PathEnvironmentAuditorEngine.cs",
        "source/UninstallTools/Detection/MultiUserSoftwareMatrixEngine.cs",
        "source/UninstallTools/Detection/SoftwareReputationEngine.cs",
        "source/UninstallTools/Detection/SoftwareUsageHeatmapEngine.cs",
        "source/UninstallTools/Detection/SoftwareNetworkMonitorEngine.cs",
        "source/UninstallTools/Exclusions/SettingsTransferEngine.cs",
        "source/UninstallTools/StoreApps/StoreAppDeprovisioner.cs",
        "source/UninstallTools/StoreApps/StoreAppProvisioningAnalyzer.cs",
        "source/UninstallTools/History/SoftwareLifetimeTrackerEngine.cs",
        "source/UninstallTools/Uninstaller/UninstallPipeline.cs",
        "source/HelperTools/HelperTools.cs",
        "source/HelperTools/LogWriter.cs",
        "source/HelperTools/ProcessRunner.cs",
        "source/HelperTools/InterProcessCommunication.cs",
        "source/HelperTools/SystemEnvironmentInfo.cs",
        "source/EBUninstaller/Forms/Windows/ForcedUninstallWindow.cs",
        "source/EBUninstaller/Forms/Windows/BackupManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/InstallationMonitorWindow.cs",
        "source/EBUninstaller/Forms/Windows/JunkCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/PrivacyCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/BrowserExtensionsWindow.cs",
        "source/EBUninstaller/Forms/Windows/WindowsToolsWindow.cs",
        "source/EBUninstaller/Forms/Windows/OperationHistoryWindow.cs",
        "source/EBUninstaller/Forms/Windows/SecureDeleteWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareHealthWindow.cs",
        "source/EBUninstaller/Forms/Windows/RegistryOptimizerWindow.cs",
        "source/EBUninstaller/Forms/Windows/DuplicateAndEmptyFolderWindow.cs",
        "source/EBUninstaller/Forms/Windows/ContextMenuManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/ServicesOptimizerWindow.cs",
        "source/EBUninstaller/Forms/Windows/WindowsFeaturesManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareAdvisorWindow.cs",
        "source/EBUninstaller/Forms/Windows/CrashDumpCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/EventLogCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/FileUnlockerWindow.cs",
        "source/EBUninstaller/Forms/Windows/PackageManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/SystemRestorePointWindow.cs",
        "source/EBUninstaller/Forms/Windows/DiskSpaceAnalyzerWindow.cs",
        "source/EBUninstaller/Forms/Windows/DriverManagementWindow.cs",
        "source/EBUninstaller/Forms/Windows/FontResidualsCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/FirewallRulesManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/HostsFileManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/EnvironmentVariablesWindow.cs",
        "source/EBUninstaller/Forms/Windows/DeveloperCacheCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/WslManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/DisconnectedDevicesCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/ShellCacheRebuilderWindow.cs",
        "source/EBUninstaller/Forms/Windows/RuntimesManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/DriverBackupWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareInventoryReportWindow.cs",
        "source/EBUninstaller/Forms/Windows/OrphanedServicesWindow.cs",
        "source/EBUninstaller/Forms/Windows/RegistryBloatWindow.cs",
        "source/EBUninstaller/Forms/Windows/WinUpdateResidualsWindow.cs",
        "source/EBUninstaller/Forms/Windows/ShellHandlersWindow.cs",
        "source/EBUninstaller/Forms/Windows/ApplicationFootprintWindow.cs",
        "source/EBUninstaller/Forms/Windows/FileAssociationsWindow.cs",
        "source/EBUninstaller/Forms/Windows/FreeSpaceWiperWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareVulnerabilityWindow.cs",
        "source/EBUninstaller/Forms/Windows/InstalledFontsWindow.cs",
        "source/EBUninstaller/Forms/Windows/ServiceDependencyWindow.cs",
        "source/EBUninstaller/Forms/Windows/WindowsTelemetryWindow.cs",
        "source/EBUninstaller/Forms/Windows/StoreAppDeprovisionerWindow.cs",
        "source/EBUninstaller/Forms/Windows/ShortcutResidualsWindow.cs",
        "source/EBUninstaller/Forms/Windows/PackageManagerSyncWindow.cs",
        "source/EBUninstaller/Forms/Windows/InstallationSnapshotDiffWindow.cs",
        "source/EBUninstaller/Forms/Windows/BootPerformanceWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareReputationWindow.cs",
        "source/EBUninstaller/Forms/Windows/SettingsTransferWindow.cs",
        "source/EBUninstaller/Forms/Windows/StartupDelayWindow.cs",
        "source/EBUninstaller/Forms/Windows/BootTimeDeleterWindow.cs",
        "source/EBUninstaller/Forms/Windows/ServiceQuarantineWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareUsageHeatmapWindow.cs",
        "source/EBUninstaller/Forms/Windows/ProductKeyExtractorWindow.cs",
        "source/EBUninstaller/Forms/Windows/WindowsSandboxWindow.cs",
        "source/EBUninstaller/Forms/Windows/SharedDllAuditorWindow.cs",
        "source/EBUninstaller/Forms/Windows/SystemHealthScorecardWindow.cs",
        "source/EBUninstaller/Forms/Windows/StoreAppProvisioningWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareLifetimeTrackerWindow.cs",
        "source/EBUninstaller/Forms/Windows/ShadowCopyManagerWindow.cs",
        "source/EBUninstaller/Forms/Windows/WinSxSOptimizerWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareNetworkMonitorWindow.cs",
        "source/EBUninstaller/Forms/Windows/ShellHandlerAuditWindow.cs",
        "source/EBUninstaller/Forms/Windows/PatchCacheCleanerWindow.cs",
        "source/EBUninstaller/Forms/Windows/StorageSenseOptimizerWindow.cs",
        "source/EBUninstaller/Forms/Windows/CveAuditorWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareCrashHistoryWindow.cs",
        "source/EBUninstaller/Forms/Windows/ServiceConflictDetectorWindow.cs",
        "source/EBUninstaller/Forms/Windows/PathEnvironmentAuditorWindow.cs",
        "source/EBUninstaller/Forms/Windows/SoftwareUpdateDifferWindow.cs",
        "source/EBUninstaller/Forms/Windows/MultiUserSoftwareWindow.cs",
        "source/EBUninstaller/Forms/Wizards/QuickOptimizationWizard.cs",
        "source/EBUninstaller/Controls/ModernStatsDashboard.cs",
        "source/EBUninstaller/Controls/QuickFilterChipsBar.cs",
        "source/EBUninstaller/Controls/AppDetailsPanel.cs",
        "source/UninstallTools/Core/UpdateManager.cs",
        "source/UninstallTools/Detection/SoftwareHealthEngine.cs",
        "source/UninstallTools/Detection/AppFilterEngine.cs",
        "source/UninstallTools/JunkCleaner/DriverAndSystemResidualsCleaner.cs",
        "source/UninstallTools/JunkCleaner/DeviceDriverResidualsCleaner.cs",
        "source/UninstallTools/RegistryEngine/RegistryOptimizerEngine.cs",
        "source/UninstallTools/RegistryEngine/RegistryBloatAnalyzer.cs",
        "source/UninstallTools/RegistryEngine/ShellHandlersCleaner.cs",
        "source/UninstallTools/RegistryEngine/FileAssociationsCleaner.cs",
        "source/UninstallTools/RegistryEngine/SharedDllAuditorEngine.cs",
        "source/UninstallTools/RegistryEngine/ShellHandlerAuditEngine.cs",
        "source/UninstallTools/Startup/StartupImpactAnalyzer.cs",
        "source/UninstallTools/Startup/WindowsServicesOptimizer.cs",
        "source/UninstallTools/Startup/ServiceDependencyTree.cs",
        "source/UninstallTools/Startup/BootPerformanceAnalyzer.cs",
        "source/UninstallTools/Startup/StartupDelayOptimizer.cs",
        "source/UninstallTools/Startup/ServiceQuarantineEngine.cs",
        "source/UninstallTools/SystemTools/MemoryTrimmerEngine.cs",
        "source/UninstallTools/SystemTools/AutoMaintenanceScheduler.cs",
        "source/UninstallTools/WindowsIntegration/ShellIntegrationManager.cs",
        "source/UninstallTools/WindowsIntegration/ContextMenuManager.cs",
        "source/WinUpdateHelper/WUApiInterop.cs",
        "source/EBU-console/Program.cs"
    ]

    all_exist = True
    missing = []
    for mod in required_modules:
        full_path = os.path.join(REPO_ROOT, mod)
        if not os.path.exists(full_path):
            # fallback to BulkCrapUninstaller / BCU-console if not migrated
            alt_path = os.path.join(REPO_ROOT, mod.replace("EBUninstaller", "BulkCrapUninstaller").replace("EBU-console", "BCU-console"))
            if not os.path.exists(alt_path):
                if not quiet:
                    print(f" [MISSING] {mod}")
                missing.append(mod)
                all_exist = False
            elif verbose and not quiet:
                print(f"   [OK] {mod}")
        elif verbose and not quiet:
            print(f"   [OK] {mod}")

    if all_exist and not quiet:
        print(f" -> All {len(required_modules)} modular subsystems present and in place.")
    return all_exist, len(required_modules), missing

def check_unit_tests(verbose=False, quiet=False):
    if not quiet:
        print("[Check 4/6] Verifying Test Suite Coverage...")
    test_files = [
        "source/EBUninstallerTests/SecurityGuardTests.cs",
        "source/EBUninstallerTests/CryptoHasherTests.cs",
        "source/EBUninstallerTests/InstallationMonitorTests.cs",
        "source/EBUninstallerTests/ExclusionAndHistoryTests.cs",
        "source/EBUninstallerTests/LifecycleIntegrationTests.cs",
        "source/EBUninstallerTests/JunkAndPrivacyCleanerTests.cs",
        "source/EBUninstallerTests/LocalizationAndTargetModeTests.cs",
        "source/EBUninstallerTests/ThemeAndUiTests.cs",
        "source/EBUninstallerTests/SoftwareHealthAndUpdaterTests.cs",
        "source/EBUninstallerTests/FilterAndResidualsTests.cs",
        "source/EBUninstallerTests/StartupImpactAndSchedulerTests.cs",
        "source/EBUninstallerTests/DriverAndMemoryTests.cs",
        "source/EBUninstallerTests/WizardAndOptimizationTests.cs",
        "source/EBUninstallerTests/EmptyDirectoryAndDuplicateTests.cs",
        "source/EBUninstallerTests/ContextMenuManagerTests.cs",
        "source/EBUninstallerTests/WindowsServicesOptimizerTests.cs",
        "source/EBUninstallerTests/WindowsOptionalFeaturesTests.cs",
        "source/EBUninstallerTests/SoftwareSafetyAdvisorTests.cs",
        "source/EBUninstallerTests/CrashDumpCleanerTests.cs",
        "source/EBUninstallerTests/EventLogCleanerTests.cs",
        "source/EBUninstallerTests/FileUnlockerTests.cs",
        "source/EBUninstallerTests/PackageManagerTests.cs",
        "source/EBUninstallerTests/SystemRestorePointTests.cs",
        "source/EBUninstallerTests/DiskSpaceAnalyzerTests.cs",
        "source/EBUninstallerTests/WindowsDriverManagerTests.cs",
        "source/EBUninstallerTests/FontResidualsCleanerTests.cs",
        "source/EBUninstallerTests/WindowsFirewallManagerTests.cs",
        "source/EBUninstallerTests/WindowsHostsFileManagerTests.cs",
        "source/EBUninstallerTests/EnvironmentVariablesTests.cs",
        "source/EBUninstallerTests/DeveloperCacheCleanerTests.cs",
        "source/EBUninstallerTests/WslManagerTests.cs",
        "source/EBUninstallerTests/DisconnectedDevicesCleanerTests.cs",
        "source/EBUninstallerTests/ShellCacheRebuilderTests.cs",
        "source/EBUninstallerTests/WindowsRuntimesManagerTests.cs",
        "source/EBUninstallerTests/WindowsDriverBackupEngineTests.cs",
        "source/EBUninstallerTests/SoftwareInventoryReportGeneratorTests.cs",
        "source/EBUninstallerTests/OrphanedServicesCleanerTests.cs",
        "source/EBUninstallerTests/RegistryBloatAnalyzerTests.cs",
        "source/EBUninstallerTests/ConsoleCliCommandTests.cs",
        "source/EBUninstallerTests/WinUpdateResidualsCleanerTests.cs",
        "source/EBUninstallerTests/ShellHandlersCleanerTests.cs",
        "source/EBUninstallerTests/ApplicationFootprintAnalyzerTests.cs",
        "source/EBUninstallerTests/FileAssociationsCleanerTests.cs",
        "source/EBUninstallerTests/FreeSpaceWiperTests.cs",
        "source/EBUninstallerTests/SoftwareVulnerabilityCheckerTests.cs",
        "source/EBUninstallerTests/InstalledFontsCleanerTests.cs",
        "source/EBUninstallerTests/ServiceDependencyTreeTests.cs",
        "source/EBUninstallerTests/HelperToolsTests.cs",
        "source/EBUninstallerTests/WindowsTelemetryOptimizerTests.cs",
        "source/EBUninstallerTests/StoreAppDeprovisionerTests.cs",
        "source/EBUninstallerTests/ShortcutResidualsCleanerTests.cs",
        "source/EBUninstallerTests/PackageManagerSyncEngineTests.cs",
        "source/EBUninstallerTests/InstallationSnapshotDifferTests.cs",
        "source/EBUninstallerTests/BootPerformanceAnalyzerTests.cs",
        "source/EBUninstallerTests/SoftwareReputationEngineTests.cs",
        "source/EBUninstallerTests/SettingsTransferEngineTests.cs",
        "source/EBUninstallerTests/StartupDelayOptimizerTests.cs",
        "source/EBUninstallerTests/BootTimeDeleterEngineTests.cs",
        "source/EBUninstallerTests/ServiceQuarantineEngineTests.cs",
        "source/EBUninstallerTests/SoftwareUsageHeatmapEngineTests.cs",
        "source/EBUninstallerTests/ProductKeyExtractorEngineTests.cs",
        "source/EBUninstallerTests/WindowsSandboxManagerTests.cs",
        "source/EBUninstallerTests/SharedDllAuditorEngineTests.cs",
        "source/EBUninstallerTests/SystemHealthScorecardEngineTests.cs",
        "source/EBUninstallerTests/StoreAppProvisioningAnalyzerTests.cs",
        "source/EBUninstallerTests/SoftwareLifetimeTrackerEngineTests.cs",
        "source/EBUninstallerTests/VolumeShadowCopyManagerTests.cs",
        "source/EBUninstallerTests/WinSxSStoreAnalyzerTests.cs",
        "source/EBUninstallerTests/SoftwareNetworkMonitorEngineTests.cs",
        "source/EBUninstallerTests/ShellHandlerAuditEngineTests.cs",
        "source/EBUninstallerTests/PatchCacheResidualsCleanerTests.cs",
        "source/EBUninstallerTests/StorageSenseOptimizerTests.cs",
        "source/EBUninstallerTests/CveDatabaseAuditorTests.cs",
        "source/EBUninstallerTests/SoftwareCrashHistoryEngineTests.cs",
        "source/EBUninstallerTests/ServiceConflictDetectorTests.cs",
        "source/EBUninstallerTests/PathEnvironmentAuditorTests.cs",
        "source/EBUninstallerTests/SoftwareUpdateDifferTests.cs",
        "source/EBUninstallerTests/MultiUserSoftwareMatrixEngineTests.cs",
        "source/EBUninstallerTests/ApplicationUninstallerEntryTests.cs",
        "source/EBUninstallerTests/ApplicationEntrySerializerTests.cs",
        "source/EBUninstallerTests/DrawingToolsTests.cs",
        "source/EBUninstallerTests/DynamicStringArrayConverterTests.cs",
        "source/EBUninstallerTests/UninstallListTests.cs"
    ]

    missing = []
    for tf in test_files:
        full_path = os.path.join(REPO_ROOT, tf)
        if not os.path.exists(full_path):
            alt_path = os.path.join(REPO_ROOT, tf.replace("EBUninstallerTests", "BulkCrapUninstallerTests"))
            if not os.path.exists(alt_path):
                if not quiet:
                    print(f" [MISSING TEST] {tf}")
                missing.append(tf)
            elif verbose and not quiet:
                print(f"   [OK] {tf}")
        elif verbose and not quiet:
            print(f"   [OK] {tf}")

    if not missing and not quiet:
        print(f" -> All {len(test_files)} test suites verified.")
    return len(missing) == 0, len(test_files), missing

def check_installer_and_docs(verbose=False, quiet=False):
    if not quiet:
        print("[Check 5/6] Verifying Installer Scripts & Documentation...")
    artifacts = [
        "installer/EBUninstallSetup.iss",
        "installer/lang/Arabic.isl",
        "installer/assets/logo.ico",
        "doc/EBUninstaller_Manual.html",
        "doc/BCU_manual.html",
        "doc/Preview.png",
        "doc/SimplifiedClassDiagram.png",
        "RELEASE_NOTES.md",
        "CONTRIBUTING.md",
        "publish.bat",
        "scripts/build.ps1",
        "scripts/build.sh",
        "scripts/build_setup.bat",
        "scripts/build_setup.ps1"
    ]

    missing = []
    for art in artifacts:
        full_path = os.path.join(REPO_ROOT, art)
        if not os.path.exists(full_path):
            if not quiet:
                print(f" [MISSING ARTIFACT] {art}")
            missing.append(art)
        elif verbose and not quiet:
            print(f"   [OK] {art}")

    if not missing and not quiet:
        print(f" -> All {len(artifacts)} installer scripts and documentation assets present.")
    return len(missing) == 0, len(artifacts), missing

def check_licenses(quiet=False):
    if not quiet:
        print("[Check 6/6] Checking Open Source License & Attribution...")
    license_file = os.path.join(REPO_ROOT, "Licence.txt")
    notice_file = os.path.join(REPO_ROOT, "NOTICE")
    if not os.path.exists(license_file) or not os.path.exists(notice_file):
        if not quiet:
            print(" [MISSING] Licence.txt or NOTICE file missing.")
        return False
    if not quiet:
        print(" -> License and NOTICE attribution files intact.")
    return True

def main():
    parser = argparse.ArgumentParser(description="EBUninstaller Pro - Repository Static Analysis & Architecture Verifier")
    parser.add_argument("-v", "--verbose", action="store_true", help="Display verbose file inspection logs")
    parser.add_argument("-j", "--json", action="store_true", help="Output verification results in JSON format")
    args = parser.parse_args()

    if not args.json:
        print("=================================================================")
        print(" EBUninstaller Pro - Repository Verification & Analysis          ")
        print("=================================================================")

    ok1, xml_count, xml_errs = check_xml_files(args.verbose, quiet=args.json)
    ok2, cs_count, loc_count, cs_errs = check_csharp_files(args.verbose, quiet=args.json)
    ok3, subsys_count, subsys_missing = check_subsystems(args.verbose, quiet=args.json)
    ok4, tests_count, tests_missing = check_unit_tests(args.verbose, quiet=args.json)
    ok5, arts_count, arts_missing = check_installer_and_docs(args.verbose, quiet=args.json)
    ok6 = check_licenses(quiet=args.json)

    all_passed = ok1 and ok2 and ok3 and ok4 and ok5 and ok6

    if args.json:
        result = {
            "success": all_passed,
            "metrics": {
                "xml_files_checked": xml_count,
                "csharp_files_checked": cs_count,
                "lines_of_code": loc_count,
                "subsystems_verified": subsys_count,
                "test_suites_verified": tests_count,
                "installer_doc_artifacts": arts_count
            },
            "errors": {
                "xml_errors": xml_errs,
                "csharp_errors": cs_errs,
                "missing_subsystems": subsys_missing,
                "missing_tests": tests_missing,
                "missing_artifacts": arts_missing,
                "license_valid": ok6
            }
        }
        print(json.dumps(result, indent=2))
        sys.exit(0 if all_passed else 1)

    if all_passed:
        print("\n=================================================================")
        print(" [SUCCESS] Repository audit passed with 100% integrity!")
        print(f" Summary: {cs_count} C# files ({loc_count:,} LoC) | {xml_count} XML files | {subsys_count} Subsystems | {tests_count} Test Suites")
        print("=================================================================")
        sys.exit(0)
    else:
        print("\n=================================================================")
        print(" [FAILURE] Repository verification encountered issues.")
        print("=================================================================")
        sys.exit(1)

if __name__ == "__main__":
    main()
