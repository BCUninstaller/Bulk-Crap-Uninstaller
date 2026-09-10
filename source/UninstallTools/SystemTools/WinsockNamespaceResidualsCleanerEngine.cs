/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Winsock NameSpace & LSP Provider Residuals Cleaner Subsystem
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
    public sealed class WinsockProviderItem
    {
        public string ProviderName { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;
        public string LibraryPath { get; set; } = string.Empty;
        public bool IsLibraryMissing { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
        public string HealthStatus => IsLibraryMissing ? "Orphaned Winsock Provider (DLL Missing)" : "Valid Winsock Provider";
    }

    public static class WinsockNamespaceResidualsCleanerEngine
    {
        private static readonly string NameSpaceCatalogPath = @"SYSTEM\CurrentControlSet\Services\WinSock2\Parameters\NameSpace_Catalog5\Catalog_Entries";

        public static List<WinsockProviderItem> ScanWinsockProviders()
        {
            var results = new List<WinsockProviderItem>();
            var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            try
            {
                using var catKey = Registry.LocalMachine.OpenSubKey(NameSpaceCatalogPath);
                if (catKey != null)
                {
                    foreach (var entryName in catKey.GetSubKeyNames())
                    {
                        try
                        {
                            using var entry = catKey.OpenSubKey(entryName);
                            if (entry == null) continue;

                            var name = entry.GetValue("DisplayString") as string ?? entryName;
                            var dll = entry.GetValue("LibraryPath") as string ?? string.Empty;
                            var id = entry.GetValue("ProviderId") as byte[];
                            var guidStr = id != null && id.Length == 16 ? new Guid(id).ToString("B") : entryName;

                            var isMissing = false;
                            if (!string.IsNullOrEmpty(dll))
                            {
                                var expanded = Environment.ExpandEnvironmentVariables(dll);
                                var full = Path.IsPathRooted(expanded) ? expanded : Path.Combine(sysDir, expanded);
                                if (!File.Exists(full))
                                {
                                    isMissing = true;
                                }
                            }

                            results.Add(new WinsockProviderItem
                            {
                                ProviderName = name,
                                ProviderId = guidStr,
                                LibraryPath = dll,
                                IsLibraryMissing = isMissing,
                                RegistryPath = $@"{NameSpaceCatalogPath}\{entryName}"
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying Winsock catalog", ex.Message);
            }

            return results.OrderByDescending(p => p.IsLibraryMissing).ThenBy(p => p.ProviderName).ToList();
        }
    }
}
