/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Power, Wake Lock & Battery Impact Profiler Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public sealed class SoftwarePowerRequestItem
    {
        public string RequestType { get; set; } = "SYSTEM"; // DISPLAY, SYSTEM, AWAYMODE, EXECUTION, PERFBOOST
        public string CallerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool PreventsSleep => true;
    }

    public sealed class SoftwarePowerProfile
    {
        public string ApplicationName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public string ExecutablePath { get; set; } = string.Empty;
        public double CpuUsagePercent { get; set; }
        public int ActiveWakeLocks { get; set; }
        public string EfficiencyRating => ActiveWakeLocks > 0 ? "F (Prevents Sleep)" : (CpuUsagePercent > 5.0 ? "D (High CPU Drain)" : (CpuUsagePercent > 1.0 ? "C (Moderate)" : "A (Energy Efficient)"));
        public string EnergyStatus => EfficiencyRating.StartsWith("A") ? "Optimal" : "High Drain";
    }

    public static class SoftwarePowerImpactEngine
    {
        public static List<SoftwarePowerRequestItem> QueryPowerRequests()
        {
            var results = new List<SoftwarePowerRequestItem>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "powercfg.exe"),
                    Arguments = "/requests",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    var output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(5000);

                    ParsePowerCfgOutput(output, results);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "powercfg query failed", ex.Message);
            }

            return results;
        }

        public static void ParsePowerCfgOutput(string output, List<SoftwarePowerRequestItem> results)
        {
            if (string.IsNullOrWhiteSpace(output)) return;

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string currentCategory = "SYSTEM";

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.EndsWith(":"))
                {
                    currentCategory = trimmed.TrimEnd(':').ToUpperInvariant();
                    continue;
                }

                if (trimmed.Equals("None.", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("None", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (trimmed.StartsWith("[") && trimmed.Contains("]"))
                {
                    var closeIdx = trimmed.IndexOf(']');
                    var caller = trimmed.Substring(1, closeIdx - 1);
                    var desc = trimmed.Substring(closeIdx + 1).Trim();

                    results.Add(new SoftwarePowerRequestItem
                    {
                        RequestType = currentCategory,
                        CallerName = caller,
                        Description = desc
                    });
                }
            }
        }

        public static List<SoftwarePowerProfile> ProfileRunningSoftware()
        {
            var profiles = new List<SoftwarePowerProfile>();
            var activeRequests = QueryPowerRequests();

            try
            {
                var processes = Process.GetProcesses();
                foreach (var p in processes)
                {
                    try
                    {
                        if (p.Id == 0 || p.Id == 4) continue; // Idle & System

                        var name = p.ProcessName;
                        var wakeLocks = activeRequests.Count(r => r.Description.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                                 r.CallerName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

                        profiles.Add(new SoftwarePowerProfile
                        {
                            ApplicationName = name,
                            ProcessId = p.Id,
                            ActiveWakeLocks = wakeLocks,
                            CpuUsagePercent = 0.0 // Instant snapshot
                        });
                    }
                    catch { }
                }
            }
            catch { }

            return profiles.OrderByDescending(p => p.ActiveWakeLocks).ThenBy(p => p.ApplicationName).ToList();
        }
    }
}
