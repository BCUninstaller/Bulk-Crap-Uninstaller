/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    BranchCache & Peer Distribution Cache Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class BranchCacheStats
    {
        public int FileCount { get; set; }
        public long TotalSizeBytes { get; set; }
        public List<string> CacheDirectories { get; set; } = new();
    }

    public static class BranchCacheCleanerEngine
    {
        public static BranchCacheStats ScanBranchCache()
        {
            var stats = new BranchCacheStats();
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            var targets = new[]
            {
                Path.Combine(winDir, @"ServiceProfiles\NetworkService\AppData\Local\PeerDistRepub"),
                Path.Combine(winDir, @"System32\PeerDistRepub"),
                Path.Combine(winDir, @"ServiceProfiles\NetworkService\AppData\Local\PeerDistPub")
            };

            foreach (var path in targets)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        stats.CacheDirectories.Add(path);
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        stats.FileCount += files.Length;
                        stats.TotalSizeBytes += files.Sum(f => new FileInfo(f).Length);
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning BranchCache {path}", ex.Message);
                }
            }

            return stats;
        }

        public static (int DeletedFiles, long FreedBytes) FlushBranchCache()
        {
            int files = 0;
            long bytes = 0;

            try
            {
                // Run netsh branchcache flush
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh.exe",
                    Arguments = "branchcache flush",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(5000);
            }
            catch { }

            var stats = ScanBranchCache();
            foreach (var dir in stats.CacheDirectories)
            {
                try
                {
                    if (Directory.Exists(dir))
                    {
                        foreach (var f in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                var len = new FileInfo(f).Length;
                                File.Delete(f);
                                files++;
                                bytes += len;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            return (files, bytes);
        }
    }
}
