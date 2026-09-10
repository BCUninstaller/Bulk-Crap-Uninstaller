/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Kernel LiveDump, MiniDump & WER Crash Dump Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class CrashDumpResidualItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string DumpType { get; set; } = "Crash Dump";
        public long SizeBytes { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public static class KernelLiveDumpCleanerEngine
    {
        public static List<CrashDumpResidualItem> ScanCrashDumps()
        {
            var results = new List<CrashDumpResidualItem>();

            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            var dumpPaths = new (string Path, string Type, string Pattern)[]
            {
                (Path.Combine(winDir, "Minidump"), "Kernel MiniDump (.dmp)", "*.dmp"),
                (Path.Combine(winDir, "LiveKernelReports"), "Kernel LiveDump (.dmp)", "*.dmp"),
                (winDir, "Full Kernel Memory Dump (MEMORY.DMP)", "MEMORY.DMP"),
                (Path.Combine(localAppData, "CrashDumps"), "User-Mode Crash Dump (.dmp)", "*.dmp"),
                (Path.Combine(progData, @"Microsoft\Windows\WER\ReportArchive"), "WER Crash Report Archive", "*.*"),
                (Path.Combine(progData, @"Microsoft\Windows\WER\ReportQueue"), "WER Crash Report Queue", "*.*")
            };

            foreach (var (p, type, pattern) in dumpPaths)
            {
                try
                {
                    if (Directory.Exists(p))
                    {
                        var files = Directory.GetFiles(p, pattern, SearchOption.AllDirectories);
                        foreach (var f in files)
                        {
                            try
                            {
                                var fi = new FileInfo(f);
                                results.Add(new CrashDumpResidualItem
                                {
                                    FilePath = f,
                                    DumpType = type,
                                    SizeBytes = fi.Length,
                                    CreationTime = fi.CreationTime
                                });
                            }
                            catch { }
                        }
                    }
                    else if (File.Exists(p))
                    {
                        var fi = new FileInfo(p);
                        results.Add(new CrashDumpResidualItem
                        {
                            FilePath = p,
                            DumpType = type,
                            SizeBytes = fi.Length,
                            CreationTime = fi.CreationTime
                        });
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning crash dump location {p}", ex.Message);
                }
            }

            return results.OrderByDescending(d => d.SizeBytes).ToList();
        }

        public static (int CleanedCount, long FreedBytes) CleanCrashDumps(IEnumerable<CrashDumpResidualItem> items)
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

            return (count, freed);
        }
    }
}
