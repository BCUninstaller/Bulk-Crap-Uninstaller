/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Print Spooler Residuals & Stale Print Job Cleaner Subsystem
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
    public sealed class SpoolerResidualItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string ItemType { get; set; } = "Stale Print Job (.SPL)";
        public long SizeBytes { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public static class PrintSpoolerResidualsCleanerEngine
    {
        public static List<SpoolerResidualItem> ScanSpoolerResiduals()
        {
            var results = new List<SpoolerResidualItem>();
            var spoolerDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "spool", "PRINTERS");

            try
            {
                if (Directory.Exists(spoolerDir))
                {
                    var files = Directory.GetFiles(spoolerDir, "*.*");
                    foreach (var f in files)
                    {
                        var ext = Path.GetExtension(f).ToLowerInvariant();
                        if (ext == ".spl" || ext == ".shd" || ext == ".tmp")
                        {
                            var fi = new FileInfo(f);
                            results.Add(new SpoolerResidualItem
                            {
                                FilePath = f,
                                ItemType = ext == ".spl" ? "Print Data Job (.SPL)" : (ext == ".shd" ? "Print Shadow Header (.SHD)" : "Spooler Temporary File"),
                                SizeBytes = fi.Length,
                                CreationTime = fi.CreationTime
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed scanning spooler directory", ex.Message);
            }

            return results;
        }

        public static (int CleanedCount, long FreedBytes) CleanSpoolerResiduals(IEnumerable<SpoolerResidualItem> items)
        {
            int cleaned = 0;
            long freed = 0;
            if (items == null) return (0, 0);

            StructuredLogger.Info(LogCategory.General, "Stopping Spooler service to purge stale printer jobs...");

            try
            {
                // Stop spooler
                RunNetCommand("stop spooler");

                foreach (var item in items)
                {
                    try
                    {
                        if (File.Exists(item.FilePath))
                        {
                            var s = item.SizeBytes;
                            File.Delete(item.FilePath);
                            cleaned++;
                            freed += s;
                        }
                    }
                    catch { }
                }

                // Restart spooler
                RunNetCommand("start spooler");
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed cleaning spooler residuals", ex.Message);
                try { RunNetCommand("start spooler"); } catch { }
            }

            return (cleaned, freed);
        }

        private static void RunNetCommand(string arg)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "net.exe",
                    Arguments = arg,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                p?.WaitForExit(5000);
            }
            catch { }
        }
    }
}
