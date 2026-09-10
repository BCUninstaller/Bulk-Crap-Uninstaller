/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Shell OpenWith & File Association Orphan Cleaner Subsystem
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
    public sealed class OpenWithOrphanItem
    {
        public string ApplicationExe { get; set; } = string.Empty;
        public string MissingPath { get; set; } = string.Empty;
        public string RegistryLocation { get; set; } = string.Empty;
        public string IssueReason { get; set; } = "Referenced executable missing on disk";
        public bool IsOrphaned { get; set; } = true;
    }

    public static class OpenWithResidualsCleanerEngine
    {
        public static List<OpenWithOrphanItem> ScanOpenWithOrphans()
        {
            var results = new List<OpenWithOrphanItem>();

            var keysToScan = new (RegistryKey BaseKey, string Path)[]
            {
                (Registry.ClassesRoot, "Applications"),
                (Registry.CurrentUser, @"Software\Classes\Applications")
            };

            foreach (var (root, subPath) in keysToScan)
            {
                try
                {
                    using var appKey = root.OpenSubKey(subPath);
                    if (appKey != null)
                    {
                        foreach (var exeName in appKey.GetSubKeyNames())
                        {
                            try
                            {
                                using var cmdKey = appKey.OpenSubKey($@"{exeName}\shell\open\command");
                                if (cmdKey != null)
                                {
                                    var cmd = cmdKey.GetValue("") as string;
                                    if (!string.IsNullOrWhiteSpace(cmd))
                                    {
                                        var cleanPath = ExtractPath(cmd);
                                        if (!string.IsNullOrEmpty(cleanPath) &&
                                            !cleanPath.StartsWith("%SystemRoot%", StringComparison.OrdinalIgnoreCase) &&
                                            !cleanPath.StartsWith(@"C:\Windows", StringComparison.OrdinalIgnoreCase) &&
                                            !File.Exists(cleanPath))
                                        {
                                            results.Add(new OpenWithOrphanItem
                                            {
                                                ApplicationExe = exeName,
                                                MissingPath = cleanPath,
                                                RegistryLocation = $@"{root.Name}\{subPath}\{exeName}",
                                                IssueReason = $"Binary does not exist: {cleanPath}"
                                            });
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
                    StructuredLogger.Warning(LogCategory.Registry, $"Failed querying OpenWith in {subPath}", ex.Message);
                }
            }

            return results;
        }

        public static int CleanOpenWithItems(IEnumerable<OpenWithOrphanItem> items)
        {
            int count = 0;
            if (items == null) return 0;

            foreach (var item in items)
            {
                try
                {
                    if (item.RegistryLocation.StartsWith("HKEY_CURRENT_USER", StringComparison.OrdinalIgnoreCase))
                    {
                        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Applications", true);
                        key?.DeleteSubKeyTree(item.ApplicationExe, false);
                        count++;
                    }
                    else
                    {
                        using var key = Registry.ClassesRoot.OpenSubKey("Applications", true);
                        key?.DeleteSubKeyTree(item.ApplicationExe, false);
                        count++;
                    }
                }
                catch { }
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
            var spaceIdx = trimmed.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (spaceIdx > 0) return Environment.ExpandEnvironmentVariables(trimmed.Substring(0, spaceIdx + 4));
            return Environment.ExpandEnvironmentVariables(trimmed);
        }
    }
}
