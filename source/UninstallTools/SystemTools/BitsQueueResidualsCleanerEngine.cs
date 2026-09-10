/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    BITS (Background Intelligent Transfer Service) Queue Residuals Cleaner Subsystem
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
    public sealed class BitsJobItem
    {
        public string JobId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string State { get; set; } = "Suspended";
        public string Owner { get; set; } = "SYSTEM";
        public int FileCount { get; set; } = 1;
        public string TargetFile { get; set; } = string.Empty;
    }

    public static class BitsQueueResidualsCleanerEngine
    {
        public static List<BitsJobItem> ScanBitsJobs()
        {
            var results = new List<BitsJobItem>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "bitsadmin.exe",
                    Arguments = "/list /allusers /verbose",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var p = Process.Start(psi);
                if (p != null)
                {
                    var output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(5000);

                    // Parse BITSAdmin output blocks: {GUID} 'Job Name' STATE
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    BitsJobItem current = null;

                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (trimmed.StartsWith("GUID:", StringComparison.OrdinalIgnoreCase))
                        {
                            current = new BitsJobItem { JobId = trimmed.Substring(5).Trim() };
                            results.Add(current);
                        }
                        else if (current != null)
                        {
                            if (trimmed.StartsWith("DISPLAY:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.DisplayName = trimmed.Substring(8).Trim();
                            }
                            else if (trimmed.StartsWith("STATE:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.State = trimmed.Substring(6).Trim();
                            }
                            else if (trimmed.StartsWith("OWNER:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.Owner = trimmed.Substring(6).Trim();
                            }
                            else if (trimmed.StartsWith("URL:", StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith("FILE:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.TargetFile = trimmed;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed scanning BITS jobs", ex.Message);
            }

            return results;
        }

        public static (int CancelledJobs, bool QueueReset) PurgeBitsQueue(IEnumerable<BitsJobItem> jobs)
        {
            int cancelled = 0;
            bool reset = false;

            if (jobs != null)
            {
                foreach (var j in jobs)
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "bitsadmin.exe",
                            Arguments = $"/cancel \"{j.JobId}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using var p = Process.Start(psi);
                        p?.WaitForExit(3000);
                        cancelled++;
                    }
                    catch { }
                }
            }

            // Also purge qmgr*.dat files in ProgramData if stopped
            try
            {
                var downloaderDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\Network\Downloader");
                if (Directory.Exists(downloaderDir))
                {
                    var files = Directory.GetFiles(downloaderDir, "qmgr*.dat");
                    foreach (var f in files)
                    {
                        try { File.Delete(f); reset = true; } catch { }
                    }
                }
            }
            catch { }

            return (cancelled, reset);
        }
    }
}
