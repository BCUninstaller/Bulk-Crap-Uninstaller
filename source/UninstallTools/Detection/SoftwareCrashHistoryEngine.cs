/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Stability, Hang & Crash History Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public sealed class SoftwareCrashRecord
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string ApplicationVersion { get; set; } = string.Empty;
        public string FaultingModule { get; set; } = string.Empty;
        public string ExceptionCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } = "Crash"; // Crash, Hang, Error
        public string RawMessage { get; set; } = string.Empty;
    }

    public sealed class AppStabilitySummary
    {
        public string ApplicationName { get; set; } = string.Empty;
        public int TotalCrashes { get; set; }
        public int TotalHangs { get; set; }
        public DateTime LastCrashDate { get; set; }
        public string PrimaryFaultingModule { get; set; } = string.Empty;
        public double StabilityScore { get; set; } = 100.0; // 0 to 100
        public string HealthStatus => StabilityScore >= 95 ? "Stable" : (StabilityScore >= 75 ? "Moderate" : "Crash Prone");
    }

    public static class SoftwareCrashHistoryEngine
    {
        private static readonly Regex AppNameRegex = new(@"Faulting application name:\s*([^\r\n,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex AppVerRegex = new(@"Faulting application version:\s*([^\r\n,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ModuleRegex = new(@"Faulting module name:\s*([^\r\n,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ExceptionCodeRegex = new(@"Exception code:\s*([^\r\n,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static List<SoftwareCrashRecord> ScanCrashHistory(int daysBack = 90)
        {
            var records = new List<SoftwareCrashRecord>();
            var cutoff = DateTime.Now.AddDays(-daysBack);

            try
            {
                if (EventLog.Exists("Application"))
                {
                    using var appLog = new EventLog("Application");
                    var entries = appLog.Entries;
                    var count = entries.Count;

                    // Scan in reverse (newest first)
                    for (var i = count - 1; i >= 0 && i >= count - 2000; i--)
                    {
                        try
                        {
                            var entry = entries[i];
                            if (entry.TimeGenerated < cutoff)
                                break;

                            // Event ID 1000 = Application Error, 1002 = Application Hang
                            if (entry.InstanceId == 1000 || entry.InstanceId == 1002 ||
                                (entry.Source != null && (entry.Source.Equals("Application Error", StringComparison.OrdinalIgnoreCase) ||
                                                         entry.Source.Equals("Application Hang", StringComparison.OrdinalIgnoreCase))))
                            {
                                var rec = ParseEventLogEntry(entry.Message, entry.TimeGenerated, entry.InstanceId == 1002 ? "Hang" : "Crash");
                                if (rec != null)
                                {
                                    records.Add(rec);
                                }
                            }
                        }
                        catch
                        {
                            // Ignore single entry read errors
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Event log scan limited or unavailable", ex.Message);
            }

            return records;
        }

        public static SoftwareCrashRecord ParseEventLogEntry(string message, DateTime timestamp, string defaultType = "Crash")
        {
            if (string.IsNullOrWhiteSpace(message)) return null;

            var rec = new SoftwareCrashRecord
            {
                Timestamp = timestamp,
                EventType = defaultType,
                RawMessage = message
            };

            var appMatch = AppNameRegex.Match(message);
            if (appMatch.Success)
            {
                rec.ApplicationName = appMatch.Groups[1].Value.Trim();
            }
            else
            {
                // Fallback: extract first word or line
                var firstLine = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                rec.ApplicationName = firstLine != null ? firstLine.Trim() : "Unknown Application";
            }

            var verMatch = AppVerRegex.Match(message);
            if (verMatch.Success)
            {
                rec.ApplicationVersion = verMatch.Groups[1].Value.Trim();
            }

            var modMatch = ModuleRegex.Match(message);
            if (modMatch.Success)
            {
                rec.FaultingModule = modMatch.Groups[1].Value.Trim();
            }

            var codeMatch = ExceptionCodeRegex.Match(message);
            if (codeMatch.Success)
            {
                rec.ExceptionCode = codeMatch.Groups[1].Value.Trim();
            }

            return rec;
        }

        public static List<AppStabilitySummary> GenerateStabilitySummaries(IEnumerable<SoftwareCrashRecord> records)
        {
            var summaries = new List<AppStabilitySummary>();
            if (records == null) return summaries;

            var grouped = records.GroupBy(r => r.ApplicationName, StringComparer.OrdinalIgnoreCase);

            foreach (var group in grouped)
            {
                var crashes = group.Count(x => x.EventType.Equals("Crash", StringComparison.OrdinalIgnoreCase));
                var hangs = group.Count(x => x.EventType.Equals("Hang", StringComparison.OrdinalIgnoreCase));
                var latest = group.Max(x => x.Timestamp);

                var primaryModule = group
                    .Where(x => !string.IsNullOrEmpty(x.FaultingModule))
                    .GroupBy(x => x.FaultingModule, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "Unknown Module";

                var totalIncidents = crashes + hangs;
                // Stability penalty: each crash is -5%, each hang is -2.5%
                var score = Math.Max(0.0, 100.0 - (crashes * 5.0) - (hangs * 2.5));

                summaries.Add(new AppStabilitySummary
                {
                    ApplicationName = group.Key,
                    TotalCrashes = crashes,
                    TotalHangs = hangs,
                    LastCrashDate = latest,
                    PrimaryFaultingModule = primaryModule,
                    StabilityScore = Math.Round(score, 1)
                });
            }

            return summaries.OrderBy(s => s.StabilityScore).ToList();
        }
    }
}
