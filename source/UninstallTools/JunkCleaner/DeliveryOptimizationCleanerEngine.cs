/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Delivery Optimization & Developer Package Cache Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class PackageCacheGroup
    {
        public string Category { get; set; } = "Delivery Optimization";
        public string DirectoryPath { get; set; } = string.Empty;
        public long TotalSizeBytes { get; set; }
        public int FileCount { get; set; }
    }

    public static class DeliveryOptimizationCleanerEngine
    {
        public static List<PackageCacheGroup> ScanPackageCaches()
        {
            var results = new List<PackageCacheGroup>();

            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var targets = new (string Category, string Path)[]
            {
                ("Windows Delivery Optimization Cache", Path.Combine(winDir, "SoftwareDistribution", "DeliveryOptimization")),
                ("ProgramData Delivery Optimization Store", Path.Combine(progData, @"Microsoft\Windows\DeliveryOptimization\Cache")),
                ("NuGet Global Package Cache", Path.Combine(userProfile, @".nuget\packages")),
                ("NPM Cache Directory", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm-cache")),
                ("Pip Download Cache", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"pip\cache")),
                ("Cargo / Rust Package Cache", Path.Combine(userProfile, @".cargo\registry\cache")),
                ("Gradle Wrapper & Artifact Cache", Path.Combine(userProfile, @".gradle\caches"))
            };

            foreach (var (cat, path) in targets)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        var (count, size) = GetDirStats(path);
                        if (count > 0)
                        {
                            results.Add(new PackageCacheGroup
                            {
                                Category = cat,
                                DirectoryPath = path,
                                FileCount = count,
                                TotalSizeBytes = size
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed analyzing cache {path}", ex.Message);
                }
            }

            return results;
        }

        public static (int DeletedFiles, long FreedBytes) CleanCaches(IEnumerable<PackageCacheGroup> groups)
        {
            int totalFiles = 0;
            long totalBytes = 0;
            if (groups == null) return (0, 0);

            foreach (var g in groups)
            {
                try
                {
                    if (Directory.Exists(g.DirectoryPath))
                    {
                        var di = new DirectoryInfo(g.DirectoryPath);
                        foreach (var f in di.GetFiles("*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                var len = f.Length;
                                f.Delete();
                                totalFiles++;
                                totalBytes += len;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            return (totalFiles, totalBytes);
        }

        private static (int count, long size) GetDirStats(string dir)
        {
            int count = 0;
            long size = 0;
            try
            {
                var di = new DirectoryInfo(dir);
                foreach (var f in di.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    count++;
                    size += f.Length;
                }
            }
            catch { }
            return (count, size);
        }
    }
}
