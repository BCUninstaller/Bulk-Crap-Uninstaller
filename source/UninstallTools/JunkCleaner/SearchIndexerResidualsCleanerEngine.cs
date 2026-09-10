/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Search Indexer Residuals & Catalog Rebuilder Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class SearchIndexStats
    {
        public string CatalogPath { get; set; } = string.Empty;
        public long CatalogSizeBytes { get; set; }
        public bool IsServiceRunning { get; set; }
        public int IndexedLocationCount { get; set; }
    }

    public static class SearchIndexerResidualsCleanerEngine
    {
        public static SearchIndexStats QuerySearchIndexStats()
        {
            var stats = new SearchIndexStats();
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var edbPath = Path.Combine(progData, @"Microsoft\Search\Data\Applications\Windows\Windows.edb");

            try
            {
                if (File.Exists(edbPath))
                {
                    var fi = new FileInfo(edbPath);
                    stats.CatalogPath = edbPath;
                    stats.CatalogSizeBytes = fi.Length;
                }

                // Check CrawlScopeManager indexed locations in registry
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Search\CrawlScopeManager\Windows\Gather\Windows\Sites");
                if (key != null)
                {
                    stats.IndexedLocationCount = key.GetSubKeyNames().Length;
                }

                // Check WSearch service status
                var p = Process.GetProcessesByName("SearchIndexer");
                stats.IsServiceRunning = p.Length > 0;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying Windows Search Indexer stats", ex.Message);
            }

            return stats;
        }

        public static bool ResetAndRebuildIndex()
        {
            try
            {
                StructuredLogger.Info(LogCategory.General, "Stopping Windows Search Service (WSearch)...");
                RunNetCommand("stop WSearch");

                // Set SetupCompletedSuccessfully to 0 in registry to trigger native rebuild
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Search", true))
                {
                    key?.SetValue("SetupCompletedSuccessfully", 0, RegistryValueKind.DWord);
                }

                StructuredLogger.Info(LogCategory.General, "Restarting Windows Search Service...");
                RunNetCommand("start WSearch");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed resetting Windows Search index", ex.Message);
                try { RunNetCommand("start WSearch"); } catch { }
                return false;
            }
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
