/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Security Center (WSC) Security Provider Residuals Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using UninstallTools.Core;

namespace UninstallTools.SecurityHardening
{
    public sealed class WscProviderItem
    {
        public string InstanceGuid { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ProviderCategory { get; set; } = "AntiVirus";
        public string ExecutablePath { get; set; } = string.Empty;
        public bool IsBinaryPresent => !string.IsNullOrEmpty(ExecutablePath) && File.Exists(ExecutablePath);
        public bool IsOrphaned => !IsBinaryPresent && !string.IsNullOrEmpty(ExecutablePath);
        public string HealthStatus => IsBinaryPresent ? "Operational / Binary Verified" : "Orphaned Provider (Binary Missing)";
    }

    public static class WscProviderAuditorEngine
    {
        public static List<WscProviderItem> ScanSecurityProviders()
        {
            var results = new List<WscProviderItem>();

            var categories = new (string WmiClass, string Label)[]
            {
                ("AntiVirusProduct", "AntiVirus"),
                ("FirewallProduct", "Firewall"),
                ("AntiSpywareProduct", "AntiSpyware")
            };

            foreach (var (wmiClass, label) in categories)
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", $"SELECT * FROM {wmiClass}");
                    foreach (var obj in searcher.Get())
                    {
                        var name = obj["displayName"]?.ToString() ?? "Unknown Security Product";
                        var instanceGuid = obj["instanceGuid"]?.ToString() ?? Guid.NewGuid().ToString();
                        var exePath = obj["pathToSignedProductExe"]?.ToString() ?? obj["pathToSignedReportingExe"]?.ToString() ?? string.Empty;

                        results.Add(new WscProviderItem
                        {
                            InstanceGuid = instanceGuid,
                            DisplayName = name,
                            ProviderCategory = label,
                            ExecutablePath = exePath
                        });
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.Security, $"Failed querying WSC class {wmiClass}", ex.Message);
                }
            }

            return results.OrderByDescending(p => p.IsOrphaned).ThenBy(p => p.DisplayName).ToList();
        }
    }
}
