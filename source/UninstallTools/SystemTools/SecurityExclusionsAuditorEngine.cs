/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Defender & Security Exclusions Auditor Subsystem
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
    public enum ExclusionRiskLevel
    {
        Low,
        Medium,
        High,
        Critical
    }

    public sealed class SecurityExclusionItem
    {
        public string ExclusionType { get; set; } = "Path"; // Path, Process, Extension, IP
        public string Target { get; set; } = string.Empty;
        public ExclusionRiskLevel RiskLevel { get; set; } = ExclusionRiskLevel.Low;
        public bool IsOrphaned { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string RegistrySubKey { get; set; } = string.Empty;
    }

    public static class SecurityExclusionsAuditorEngine
    {
        private static readonly string DefenderExclusionsBasePath = @"SOFTWARE\Microsoft\Windows Defender\Exclusions";

        private static readonly string[] DangerousPathPatterns =
        {
            @"C:\",
            @"C:\Windows",
            @"C:\Windows\System32",
            @"C:\Users",
            @"C:\Users\Public",
            @"%TEMP%",
            @"%TMP%",
            @"%APPDATA%",
            @"%LOCALAPPDATA%"
        };

        private static readonly string[] DangerousProcesses =
        {
            "cmd.exe",
            "powershell.exe",
            "pwsh.exe",
            "rundll32.exe",
            "regsvr32.exe",
            "mshta.exe",
            "cscript.exe",
            "wscript.exe",
            "certutil.exe",
            "bitsadmin.exe"
        };

        public static List<SecurityExclusionItem> ScanDefenderExclusions()
        {
            var results = new List<SecurityExclusionItem>();

            try
            {
                using var baseKey = Registry.LocalMachine.OpenSubKey(DefenderExclusionsBasePath);
                if (baseKey == null)
                    return results;

                // 1. Path Exclusions
                using (var pathsKey = baseKey.OpenSubKey("Paths"))
                {
                    if (pathsKey != null)
                    {
                        foreach (var name in pathsKey.GetValueNames())
                        {
                            var item = AnalyzePathExclusion(name, "Paths");
                            results.Add(item);
                        }
                    }
                }

                // 2. Process Exclusions
                using (var procKey = baseKey.OpenSubKey("Processes"))
                {
                    if (procKey != null)
                    {
                        foreach (var name in procKey.GetValueNames())
                        {
                            var item = AnalyzeProcessExclusion(name, "Processes");
                            results.Add(item);
                        }
                    }
                }

                // 3. Extension Exclusions
                using (var extKey = baseKey.OpenSubKey("Extensions"))
                {
                    if (extKey != null)
                    {
                        foreach (var name in extKey.GetValueNames())
                        {
                            var item = AnalyzeExtensionExclusion(name, "Extensions");
                            results.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Defender exclusions read failed or restricted", ex.Message);
            }

            return results;
        }

        public static SecurityExclusionItem AnalyzePathExclusion(string pathTarget, string subKeyName = "Paths")
        {
            if (string.IsNullOrWhiteSpace(pathTarget))
                return new SecurityExclusionItem { Target = pathTarget ?? string.Empty, RegistrySubKey = subKeyName };

            var expanded = Environment.ExpandEnvironmentVariables(pathTarget).TrimEnd('\\');
            var isOrphaned = !Directory.Exists(expanded) && !File.Exists(expanded);

            var risk = ExclusionRiskLevel.Low;
            var reason = "Standard folder/file exclusion.";

            // Check dangerous broad exclusions
            foreach (var d in DangerousPathPatterns)
            {
                var dExpanded = Environment.ExpandEnvironmentVariables(d).TrimEnd('\\');
                if (expanded.Equals(dExpanded, StringComparison.OrdinalIgnoreCase) ||
                    expanded.Equals(d, StringComparison.OrdinalIgnoreCase))
                {
                    risk = ExclusionRiskLevel.Critical;
                    reason = $"Extremely broad root/system exclusion ({d}), disables antivirus protection for major system locations.";
                    break;
                }
            }

            if (risk == ExclusionRiskLevel.Low && isOrphaned)
            {
                risk = ExclusionRiskLevel.Medium;
                reason = "Target path does not exist on disk (orphaned software leftover).";
            }

            return new SecurityExclusionItem
            {
                ExclusionType = "Path",
                Target = pathTarget,
                IsOrphaned = isOrphaned,
                RiskLevel = risk,
                Reason = reason,
                RegistrySubKey = subKeyName
            };
        }

        public static SecurityExclusionItem AnalyzeProcessExclusion(string processTarget, string subKeyName = "Processes")
        {
            if (string.IsNullOrWhiteSpace(processTarget))
                return new SecurityExclusionItem { Target = processTarget ?? string.Empty, RegistrySubKey = subKeyName };

            var exeName = Path.GetFileName(processTarget).ToLowerInvariant();
            var risk = ExclusionRiskLevel.Low;
            var reason = "Application process exclusion.";

            if (DangerousProcesses.Contains(exeName))
            {
                risk = ExclusionRiskLevel.Critical;
                reason = $"Exclusion for system interpreter/utility ({exeName}) allows malicious script execution without scanning.";
            }

            return new SecurityExclusionItem
            {
                ExclusionType = "Process",
                Target = processTarget,
                RiskLevel = risk,
                Reason = reason,
                RegistrySubKey = subKeyName
            };
        }

        public static SecurityExclusionItem AnalyzeExtensionExclusion(string extTarget, string subKeyName = "Extensions")
        {
            var ext = extTarget.Trim().ToLowerInvariant().TrimStart('.');
            var risk = ExclusionRiskLevel.Low;
            var reason = "File type extension exclusion.";

            var riskyExtensions = new[] { "exe", "dll", "bat", "cmd", "vbs", "js", "ps1", "scr", "hta", "pif" };
            if (riskyExtensions.Contains(ext))
            {
                risk = ExclusionRiskLevel.High;
                reason = $"Excluding executable extension (.{ext}) allows malware in this format to bypass real-time protection.";
            }

            return new SecurityExclusionItem
            {
                ExclusionType = "Extension",
                Target = extTarget,
                RiskLevel = risk,
                Reason = reason,
                RegistrySubKey = subKeyName
            };
        }

        public static bool RemoveExclusion(SecurityExclusionItem item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Target))
                return false;

            try
            {
                var fullKeyPath = $@"{DefenderExclusionsBasePath}\{item.RegistrySubKey}";
                using var key = Registry.LocalMachine.OpenSubKey(fullKeyPath, true);
                if (key != null)
                {
                    key.DeleteValue(item.Target, false);
                    StructuredLogger.Info(LogCategory.General, $"Removed Defender exclusion: {item.Target} ({item.ExclusionType})");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed removing Defender exclusion {item.Target}", ex.Message);
            }

            return false;
        }
    }
}
