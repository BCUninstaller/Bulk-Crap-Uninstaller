/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Shell Icon & Thumbnail Cache Database Rebuilder Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class ShellCacheDatabaseItem
    {
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string CacheType { get; set; } = "Thumbnail Database"; // Icon Database, Thumbnail Database
    }

    public static class IconThumbnailDatabaseRebuilderEngine
    {
        public static List<ShellCacheDatabaseItem> ScanCacheDatabases()
        {
            var results = new List<ShellCacheDatabaseItem>();

            var localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var explorerCacheDir = Path.Combine(localApp, "Microsoft", "Windows", "Explorer");

            // 1. IconCache.db in %LOCALAPPDATA%
            var legacyIconCache = Path.Combine(localApp, "IconCache.db");
            if (File.Exists(legacyIconCache))
            {
                var fi = new FileInfo(legacyIconCache);
                results.Add(new ShellCacheDatabaseItem
                {
                    FileName = fi.Name,
                    FullPath = fi.FullName,
                    SizeBytes = fi.Length,
                    CacheType = "Shell Icon Cache"
                });
            }

            // 2. thumbcache_*.db and iconcache_*.db in Explorer directory
            if (Directory.Exists(explorerCacheDir))
            {
                try
                {
                    var files = Directory.GetFiles(explorerCacheDir, "*cache_*.db");
                    foreach (var f in files)
                    {
                        var fi = new FileInfo(f);
                        results.Add(new ShellCacheDatabaseItem
                        {
                            FileName = fi.Name,
                            FullPath = fi.FullName,
                            SizeBytes = fi.Length,
                            CacheType = fi.Name.StartsWith("icon", StringComparison.OrdinalIgnoreCase) ? "Icon Database" : "Thumbnail Database"
                        });
                    }
                }
                catch { }
            }

            return results;
        }

        public static (int CleanedCount, long FreedBytes) PurgeAndRebuildCaches(IEnumerable<ShellCacheDatabaseItem> items)
        {
            int cleaned = 0;
            long freed = 0;
            if (items == null) return (0, 0);

            StructuredLogger.Info(LogCategory.General, "Restarting Explorer to purge and rebuild Shell Icon/Thumbnail databases...");

            try
            {
                // 1. Stop Explorer processes
                foreach (var exp in Process.GetProcessesByName("explorer"))
                {
                    try { exp.Kill(); exp.WaitForExit(3000); } catch { }
                }

                // 2. Delete cache files
                foreach (var item in items)
                {
                    try
                    {
                        if (File.Exists(item.FullPath))
                        {
                            var s = item.SizeBytes;
                            File.Delete(item.FullPath);
                            cleaned++;
                            freed += s;
                        }
                    }
                    catch { }
                }

                // 3. Restart Explorer
                Process.Start("explorer.exe");
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed rebuilding icon/thumbnail cache", ex.Message);
                try { Process.Start("explorer.exe"); } catch { }
            }

            return (cleaned, freed);
        }
    }
}
