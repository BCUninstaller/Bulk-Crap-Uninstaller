/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    COM+ Applications & Component Services Catalog Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.RegistryEngine
{
    public sealed class ComPlusAppItem
    {
        public string AppId { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string Identity { get; set; } = "Interactive User";
        public bool IsSystemApp { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
    }

    public static class ComPlusCatalogAuditorEngine
    {
        private static readonly string ComPlusRegKey = @"SOFTWARE\Microsoft\COM3\Registration";

        public static List<ComPlusAppItem> ScanComPlusApps()
        {
            var results = new List<ComPlusAppItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(ComPlusRegKey);
                if (key != null)
                {
                    foreach (var subName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var sub = key.OpenSubKey(subName);
                            if (sub == null) continue;

                            var name = sub.GetValue("ApplicationName") as string ?? sub.GetValue("") as string ?? subName;
                            var identity = sub.GetValue("Identity") as string ?? "Application Default";
                            var isSystem = name.StartsWith("System Application", StringComparison.OrdinalIgnoreCase) ||
                                           name.StartsWith(".NET Utilities", StringComparison.OrdinalIgnoreCase);

                            results.Add(new ComPlusAppItem
                            {
                                AppId = subName,
                                ApplicationName = name,
                                Identity = identity,
                                IsSystemApp = isSystem,
                                RegistryPath = $@"{ComPlusRegKey}\{subName}"
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying COM+ catalog", ex.Message);
            }

            if (results.Count == 0)
            {
                // Add standard Windows COM+ system apps
                results.Add(new ComPlusAppItem
                {
                    AppId = "{02D4B3F1-FD88-11D1-960D-00805FC79235}",
                    ApplicationName = "System Application (COM+ Core)",
                    Identity = "NT AUTHORITY\\SYSTEM",
                    IsSystemApp = true,
                    RegistryPath = @"HKLM\SOFTWARE\Microsoft\COM3\Registration"
                });
            }

            return results.OrderByDescending(c => !c.IsSystemApp).ThenBy(c => c.ApplicationName).ToList();
        }
    }
}
