/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Scheduled Tasks Orphan & Residuals Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    public sealed class OrphanScheduledTaskItem
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskPath { get; set; } = string.Empty;
        public string TargetBinary { get; set; } = string.Empty;
        public string IssueReason { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; } = true;
    }

    public static class ScheduledTaskOrphanCleanerEngine
    {
        public static List<OrphanScheduledTaskItem> ScanOrphanedTasks()
        {
            var results = new List<OrphanScheduledTaskItem>();
            var tasksDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "Tasks");

            if (!Directory.Exists(tasksDir)) return results;

            try
            {
                var files = Directory.GetFiles(tasksDir, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        var relName = file.Substring(tasksDir.Length).TrimStart('\\', '/');
                        // Skip Microsoft core tasks
                        if (relName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)) continue;

                        var xml = File.ReadAllText(file);
                        var match = Regex.Match(xml, @"<Command>([^<]+)</Command>", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var command = match.Groups[1].Value.Trim();
                            var cleanPath = ExtractPath(command);

                            if (!string.IsNullOrEmpty(cleanPath) &&
                                !cleanPath.StartsWith("%SystemRoot%", StringComparison.OrdinalIgnoreCase) &&
                                !cleanPath.StartsWith(@"C:\Windows\System32", StringComparison.OrdinalIgnoreCase) &&
                                !File.Exists(cleanPath))
                            {
                                results.Add(new OrphanScheduledTaskItem
                                {
                                    TaskName = relName,
                                    TaskPath = file,
                                    TargetBinary = cleanPath,
                                    IssueReason = $"Task target binary does not exist: {cleanPath}",
                                    IsOrphaned = true
                                });
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Startup, "Failed scanning scheduled tasks", ex.Message);
            }

            return results;
        }

        public static int CleanOrphanedTasks(IEnumerable<OrphanScheduledTaskItem> items)
        {
            int count = 0;
            if (items == null) return 0;

            foreach (var item in items)
            {
                try
                {
                    // Run schtasks /delete /tn "task" /f
                    var psi = new ProcessStartInfo
                    {
                        FileName = "schtasks.exe",
                        Arguments = $"/delete /tn \"{item.TaskName}\" /f",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var p = Process.Start(psi);
                    p?.WaitForExit(5000);

                    if (File.Exists(item.TaskPath))
                    {
                        File.Delete(item.TaskPath);
                    }
                    count++;
                }
                catch { }
            }

            return count;
        }

        private static string ExtractPath(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var trimmed = raw.Trim().Trim('\"');
            return Environment.ExpandEnvironmentVariables(trimmed);
        }
    }
}
