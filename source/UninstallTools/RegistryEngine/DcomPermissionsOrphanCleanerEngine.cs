/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DCOM & Component Services Orphaned Registration Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.RegistryEngine
{
    public sealed class DcomOrphanItem
    {
        public string AppId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string LocalServer32Path { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; } = true;
        public string IssueReason { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
    }

    public static class DcomPermissionsOrphanCleanerEngine
    {
        private static readonly string AppIdBasePath = @"SOFTWARE\Classes\AppID";

        public static List<DcomOrphanItem> ScanOrphanedDcomRegistrations()
        {
            var results = new List<DcomOrphanItem>();

            try
            {
                using var appIdKey = Registry.LocalMachine.OpenSubKey(AppIdBasePath);
                if (appIdKey != null)
                {
                    foreach (var subName in appIdKey.GetSubKeyNames())
                    {
                        try
                        {
                            using var sub = appIdKey.OpenSubKey(subName);
                            if (sub == null) continue;

                            var defaultVal = sub.GetValue("") as string ?? subName;
                            var localService = sub.GetValue("LocalService") as string;
                            var dllSurrogate = sub.GetValue("DllSurrogate") as string;

                            // Check corresponding CLSID if GUID
                            if (subName.StartsWith("{") && subName.EndsWith("}"))
                            {
                                var clsidPath = $@"SOFTWARE\Classes\CLSID\{subName}";
                                using var clsidKey = Registry.LocalMachine.OpenSubKey(clsidPath);
                                if (clsidKey != null)
                                {
                                    using var serverKey = clsidKey.OpenSubKey("LocalServer32") ?? clsidKey.OpenSubKey("InprocServer32");
                                    if (serverKey != null)
                                    {
                                        var rawServer = serverKey.GetValue("") as string;
                                        if (!string.IsNullOrWhiteSpace(rawServer))
                                        {
                                            var cleanServer = ExtractPath(rawServer);
                                            if (!string.IsNullOrEmpty(cleanServer) &&
                                                !cleanServer.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                                                !cleanServer.StartsWith(@"\SystemRoot\", StringComparison.OrdinalIgnoreCase) &&
                                                !File.Exists(cleanServer))
                                            {
                                                results.Add(new DcomOrphanItem
                                                {
                                                    AppId = subName,
                                                    DisplayName = defaultVal,
                                                    LocalServer32Path = cleanServer,
                                                    IsOrphaned = true,
                                                    IssueReason = $"COM server binary missing on disk: {cleanServer}",
                                                    RegistryPath = $@"{AppIdBasePath}\{subName}"
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying DCOM AppIDs", ex.Message);
            }

            return results;
        }

        public static int CleanOrphanedDcomItems(IEnumerable<DcomOrphanItem> items)
        {
            int count = 0;
            if (items == null) return 0;

            try
            {
                using var appIdKey = Registry.LocalMachine.OpenSubKey(AppIdBasePath, true);
                if (appIdKey != null)
                {
                    foreach (var item in items)
                    {
                        try
                        {
                            appIdKey.DeleteSubKeyTree(item.AppId, false);
                            count++;
                            StructuredLogger.Info(LogCategory.Registry, $"Cleaned orphaned DCOM AppID: {item.AppId} ({item.DisplayName})");
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, "Failed cleaning DCOM items", ex.Message);
            }

            return count;
        }

        private static string ExtractPath(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var trimmed = raw.Trim();
            if (trimmed.StartsWith("\""))
            {
                var end = trimmed.IndexOf('\"', 1);
                if (end > 1) return Environment.ExpandEnvironmentVariables(trimmed.Substring(1, end - 1));
            }

            var exeIdx = trimmed.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (exeIdx > 0) return Environment.ExpandEnvironmentVariables(trimmed.Substring(0, exeIdx + 4));

            var dllIdx = trimmed.IndexOf(".dll", StringComparison.OrdinalIgnoreCase);
            if (dllIdx > 0) return Environment.ExpandEnvironmentVariables(trimmed.Substring(0, dllIdx + 4));

            return Environment.ExpandEnvironmentVariables(trimmed);
        }
    }
}
