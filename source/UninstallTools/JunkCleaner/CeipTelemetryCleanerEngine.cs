/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    CEIP (Customer Experience) & SQM Software Telemetry Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class CeipStoreStats
    {
        public int TotalSqmFiles { get; set; }
        public long TotalSizeBytes { get; set; }
        public List<string> ScannedLocations { get; set; } = new();
    }

    public static class CeipTelemetryCleanerEngine
    {
        public static CeipStoreStats ScanCeipTelemetry()
        {
            var stats = new CeipStoreStats();
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var targets = new[]
            {
                Path.Combine(progData, @"Microsoft\SQM"),
                Path.Combine(localAppData, @"Microsoft\SQM"),
                Path.Combine(progData, @"Microsoft\Windows\Customer Experience Improvement Program"),
                Path.Combine(userProfile, @"AppData\LocalLow\Microsoft\CryptnetUrlCache")
            };

            foreach (var path in targets)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        stats.ScannedLocations.Add(path);
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        stats.TotalSqmFiles += files.Length;
                        stats.TotalSizeBytes += files.Sum(f => new FileInfo(f).Length);
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning CEIP location {path}", ex.Message);
                }
            }

            return stats;
        }

        public static (int DeletedFiles, long FreedBytes) PurgeCeipTelemetry()
        {
            int files = 0;
            long bytes = 0;
            var stats = ScanCeipTelemetry();

            foreach (var loc in stats.ScannedLocations)
            {
                try
                {
                    if (Directory.Exists(loc))
                    {
                        foreach (var f in Directory.GetFiles(loc, "*.*", SearchOption.AllDirectories))
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
