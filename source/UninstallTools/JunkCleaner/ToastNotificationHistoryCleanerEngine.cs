/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Toast Notification History & Action Center Database Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class NotificationStoreStats
    {
        public string DatabasePath { get; set; } = string.Empty;
        public long DatabaseSizeBytes { get; set; }
        public int CachedLogoCount { get; set; }
        public long CachedLogosSizeBytes { get; set; }
        public bool Exists { get; set; }
    }

    public static class ToastNotificationHistoryCleanerEngine
    {
        public static NotificationStoreStats QueryNotificationStore()
        {
            var stats = new NotificationStoreStats();
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var notifDir = Path.Combine(localAppData, @"Microsoft\Windows\Notifications");

            try
            {
                if (Directory.Exists(notifDir))
                {
                    stats.Exists = true;
                    var dbPath = Path.Combine(notifDir, "wpndatabase.db");
                    if (File.Exists(dbPath))
                    {
                        stats.DatabasePath = dbPath;
                        stats.DatabaseSizeBytes = new FileInfo(dbPath).Length;

                        var walPath = Path.Combine(notifDir, "wpndatabase.db-wal");
                        if (File.Exists(walPath))
                        {
                            stats.DatabaseSizeBytes += new FileInfo(walPath).Length;
                        }
                    }

                    var logosDir = Path.Combine(notifDir, "applogos");
                    if (Directory.Exists(logosDir))
                    {
                        var files = Directory.GetFiles(logosDir, "*.*", SearchOption.AllDirectories);
                        stats.CachedLogoCount = files.Length;
                        stats.CachedLogosSizeBytes = files.Sum(f => new FileInfo(f).Length);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying notification store", ex.Message);
            }

            return stats;
        }

        public static (int CleanedFiles, long FreedBytes) PurgeNotificationHistory()
        {
            int count = 0;
            long bytes = 0;

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var notifDir = Path.Combine(localAppData, @"Microsoft\Windows\Notifications");

            try
            {
                if (Directory.Exists(notifDir))
                {
                    // Purge applogos
                    var logosDir = Path.Combine(notifDir, "applogos");
                    if (Directory.Exists(logosDir))
                    {
                        foreach (var f in Directory.GetFiles(logosDir, "*.*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                var len = new FileInfo(f).Length;
                                File.Delete(f);
                                count++;
                                bytes += len;
                            }
                            catch { }
                        }
                    }

                    // Purge journal/wal files
                    var walFiles = Directory.GetFiles(notifDir, "*.db-wal").Concat(Directory.GetFiles(notifDir, "*.db-shm"));
                    foreach (var f in walFiles)
                    {
                        try
                        {
                            var len = new FileInfo(f).Length;
                            File.Delete(f);
                            count++;
                            bytes += len;
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed purging notification history", ex.Message);
            }

            return (count, bytes);
        }
    }
}
