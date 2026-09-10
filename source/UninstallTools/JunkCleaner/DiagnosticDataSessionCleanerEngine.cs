/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Diagnostic Data, Telemetry Logs & Session History Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class DiagnosticLogItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string LogType { get; set; } = "ETL Trace"; // ETL, WER, Watson, CEIP
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    public static class DiagnosticDataSessionCleanerEngine
    {
        public static List<DiagnosticLogItem> ScanDiagnosticLogs()
        {
            var results = new List<DiagnosticLogItem>();

            var targets = new (string Path, string Type, string Pattern)[]
            {
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Diagnosis"), "DiagTrack Cache", "*.*"),
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "WER", "ReportQueue"), "WER Queue", "*.*"),
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "WER", "ReportArchive"), "WER Archive", "*.*"),
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs", "CBS"), "CBS Setup Logs", "*.log"),
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs", "DISM"), "DISM Service Logs", "*.log"),
                (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "tracing"), "Windows Tracing", "*.*")
            };

            foreach (var (folder, logType, pattern) in targets)
            {
                try
                {
                    if (Directory.Exists(folder))
                    {
                        foreach (var file in Directory.EnumerateFiles(folder, pattern, SearchOption.AllDirectories))
                        {
                            try
                            {
                                var fi = new FileInfo(file);
                                results.Add(new DiagnosticLogItem
                                {
                                    FilePath = file,
                                    LogType = logType,
                                    SizeBytes = fi.Length,
                                    LastModified = fi.LastWriteTime
                                });
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            return results;
        }

        public static (int CleanedCount, long FreedBytes) CleanDiagnosticLogs(IEnumerable<DiagnosticLogItem> items)
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
                        var size = item.SizeBytes;
                        File.Delete(item.FilePath);
                        count++;
                        freed += size;
                    }
                }
                catch { }
            }

            StructuredLogger.Info(LogCategory.General, $"Cleaned {count} diagnostic logs, freed {freed} bytes.");
            return (count, freed);
        }
    }
}
