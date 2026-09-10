/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Environment Variables Registry Residuals Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class OrphanedEnvVarItem
    {
        public string VariableName { get; set; } = string.Empty;
        public string VariableValue { get; set; } = string.Empty;
        public string Scope { get; set; } = "System"; // System or User
        public string MissingPath { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; } = true;
    }

    public static class EnvVarResidualsCleanerEngine
    {
        private static readonly string[] KnownSoftwareVars =
        {
            "JAVA_HOME", "JDK_HOME", "JRE_HOME",
            "PYTHONHOME", "PYTHONPATH",
            "CUDA_PATH", "CUDA_PATH_V", "VULKAN_SDK",
            "ANDROID_HOME", "ANDROID_SDK_ROOT",
            "GOPATH", "GOROOT",
            "RUSTUP_HOME", "CARGO_HOME",
            "MAVEN_HOME", "M2_HOME", "GRADLE_HOME",
            "ANT_HOME", "CATALINA_HOME",
            "NODE_PATH", "NVM_HOME", "NVM_SYMLINK",
            "ORACLE_HOME", "TNS_ADMIN",
            "FLUTTER_ROOT", "DART_SDK",
            "DOTNET_ROOT"
        };

        public static List<OrphanedEnvVarItem> ScanOrphanedEnvironmentVariables()
        {
            var results = new List<OrphanedEnvVarItem>();

            // 1. Scan System Environment Variables
            try
            {
                using var sysKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                if (sysKey != null)
                {
                    AuditKey(sysKey, "System", results);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning system environment variables", ex.Message);
            }

            // 2. Scan User Environment Variables
            try
            {
                using var userKey = Registry.CurrentUser.OpenSubKey(@"Environment");
                if (userKey != null)
                {
                    AuditKey(userKey, "User", results);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning user environment variables", ex.Message);
            }

            return results;
        }

        private static void AuditKey(RegistryKey key, string scope, List<OrphanedEnvVarItem> results)
        {
            foreach (var valName in key.GetValueNames())
            {
                if (valName.Equals("PATH", StringComparison.OrdinalIgnoreCase) ||
                    valName.Equals("PATHEXT", StringComparison.OrdinalIgnoreCase) ||
                    valName.Equals("PSModulePath", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Checked by PathEnvironmentAuditor
                }

                var valData = key.GetValue(valName, "", RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
                if (string.IsNullOrWhiteSpace(valData)) continue;

                var isKnownVar = KnownSoftwareVars.Any(k => valName.StartsWith(k, StringComparison.OrdinalIgnoreCase));
                var looksLikePath = valData.Contains(":\\") || valData.StartsWith("%") || valData.Contains("\\");

                if (isKnownVar || looksLikePath)
                {
                    var expanded = Environment.ExpandEnvironmentVariables(valData);
                    if (!expanded.Contains("%") && !Directory.Exists(expanded) && !File.Exists(expanded))
                    {
                        results.Add(new OrphanedEnvVarItem
                        {
                            VariableName = valName,
                            VariableValue = valData,
                            Scope = scope,
                            MissingPath = expanded,
                            IsOrphaned = true
                        });
                    }
                }
            }
        }

        public static int CleanOrphanedVariables(IEnumerable<OrphanedEnvVarItem> items)
        {
            int cleaned = 0;
            if (items == null) return 0;

            foreach (var item in items)
            {
                try
                {
                    if (item.Scope == "System")
                    {
                        using var sysKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", true);
                        sysKey?.DeleteValue(item.VariableName, false);
                        cleaned++;
                    }
                    else
                    {
                        using var userKey = Registry.CurrentUser.OpenSubKey(@"Environment", true);
                        userKey?.DeleteValue(item.VariableName, false);
                        cleaned++;
                    }
                }
                catch { }
            }

            return cleaned;
        }
    }
}
