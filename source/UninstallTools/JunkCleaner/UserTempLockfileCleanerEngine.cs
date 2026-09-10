/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Stale User Temp Lockfiles, PIDs & Crash Artifacts Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class StaleLockfileItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName => Path.GetFileName(FilePath);
        public string LockType { get; set; } = "Process Lockfile (.lock)";
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    public static class UserTempLockfileCleanerEngine
    {
        private static readonly string[] LockExtensions = { ".lock", ".lck", ".pid", ".sock", ".semaphore", ".hprof", ".dmp.tmp" };

        public static List<StaleLockfileItem> ScanStaleLockfiles()
        {
            var results = new List<StaleLockfileItem>();
            var tempDirs = new[]
            {
                Path.GetTempPath(),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp"
            }.Distinct();

            foreach (var dir in tempDirs)
            {
                try
                {
                    if (Directory.Exists(dir))
                    {
                        var files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (var f in files)
                        {
                            var ext = Path.GetExtension(f).ToLowerInvariant();
                            if (LockExtensions.Contains(ext) || Path.GetFileName(f).StartsWith("lock-", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    var fi = new FileInfo(f);
                                    // Check if file can be opened (not locked by active process)
                                    if (IsFileNotLocked(f))
                                    {
                                        results.Add(new StaleLockfileItem
                                        {
                                            FilePath = f,
                                            LockType = GetLockType(ext),
                                            SizeBytes = fi.Length,
                                            LastModified = fi.LastWriteTime
                                        });
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning temp dir {dir}", ex.Message);
                }
            }

            return results.OrderByDescending(l => l.LastModified).ToList();
        }

        public static (int DeletedCount, long FreedBytes) PurgeLockfiles(IEnumerable<StaleLockfileItem> items)
        {
            int count = 0;
            long freed = 0;
            if (items == null) return (0, 0);

            foreach (var item in items)
            {
                try
                {
                    if (File.Exists(item.FilePath))
                    {
                        var s = item.SizeBytes;
                        File.Delete(item.FilePath);
                        count++;
                        freed += s;
                    }
                }
                catch { }
            }

            return (count, freed);
        }

        private static bool IsFileNotLocked(string path)
        {
            try
            {
                using var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return true;
            }
            catch (IOException)
            {
                return false; // Still locked by running process
            }
            catch
            {
                return false;
            }
        }

        private static string GetLockType(string ext)
        {
            return ext switch
            {
                ".lock" => "Application Lockfile",
                ".lck" => "Application Lockfile",
                ".pid" => "Process PID Artifact",
                ".sock" => "Abandoned IPC Socket",
                ".semaphore" => "Abandoned Semaphore File",
                ".hprof" => "JVM Heap Dump Residual",
                _ => "Stale Temporary Lock Artifact"
            };
        }
    }
}
