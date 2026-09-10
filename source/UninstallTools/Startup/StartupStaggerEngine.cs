/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Startup Staggering & Delayed Execution Engine
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    public sealed class StaggeredStartupItem
    {
        public string Name { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public int DelaySeconds { get; set; } = 30; // 15, 30, 60, 120, 300
        public bool IsStaggered { get; set; }
        public string TaskName => $"EBUninstaller_Staggered_{Name.Replace(" ", "_")}";
        public string SourceLocation { get; set; } = "HKCU_Run";
    }

    public static class StartupStaggerEngine
    {
        private static readonly string HkcuRun = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string HklmRun = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static List<StaggeredStartupItem> GetStartupItems()
        {
            var results = new List<StaggeredStartupItem>();

            // 1. Read HKCU Run
            try
            {
                using var hkcu = Registry.CurrentUser.OpenSubKey(HkcuRun);
                if (hkcu != null)
                {
                    foreach (var val in hkcu.GetValueNames())
                    {
                        var cmd = hkcu.GetValue(val) as string;
                        if (!string.IsNullOrWhiteSpace(cmd))
                        {
                            results.Add(new StaggeredStartupItem
                            {
                                Name = val,
                                Command = cmd,
                                DelaySeconds = 0,
                                IsStaggered = false,
                                SourceLocation = "HKCU: Run"
                            });
                        }
                    }
                }
            }
            catch { }

            // 2. Read HKLM Run
            try
            {
                using var hklm = Registry.LocalMachine.OpenSubKey(HklmRun);
                if (hklm != null)
                {
                    foreach (var val in hklm.GetValueNames())
                    {
                        var cmd = hklm.GetValue(val) as string;
                        if (!string.IsNullOrWhiteSpace(cmd))
                        {
                            results.Add(new StaggeredStartupItem
                            {
                                Name = val,
                                Command = cmd,
                                DelaySeconds = 0,
                                IsStaggered = false,
                                SourceLocation = "HKLM: Run"
                            });
                        }
                    }
                }
            }
            catch { }

            return results;
        }

        public static bool StaggerStartupItem(StaggeredStartupItem item, int delaySeconds)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Command)) return false;

            try
            {
                // Create delayed scheduled task using schtasks
                var taskName = item.TaskName;
                var delaySpan = $"PT{delaySeconds}S";

                // Format schtasks create command
                var psi = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = $"/create /tn \"{taskName}\" /tr \"{item.Command}\" /sc onlogon /delay {delaySeconds:D2}:00 /f /rl limited",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(10000);

                // Remove from original registry Run key
                if (item.SourceLocation.Contains("HKCU"))
                {
                    using var key = Registry.CurrentUser.OpenSubKey(HkcuRun, true);
                    key?.DeleteValue(item.Name, false);
                }
                else if (item.SourceLocation.Contains("HKLM"))
                {
                    using var key = Registry.LocalMachine.OpenSubKey(HklmRun, true);
                    key?.DeleteValue(item.Name, false);
                }

                item.DelaySeconds = delaySeconds;
                item.IsStaggered = true;
                StructuredLogger.Info(LogCategory.General, $"Staggered startup item {item.Name} with {delaySeconds}s delay.");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed staggering startup item {item.Name}", ex.Message);
                return false;
            }
        }
    }
}
