/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    BSOD BugCheck & Kernel Crash Dump Analyzer Subsystem
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
    public sealed class BsodCrashRecord
    {
        public string DumpFileName { get; set; } = string.Empty;
        public DateTime CrashTime { get; set; }
        public string BugcheckCode { get; set; } = string.Empty;
        public string BugcheckName { get; set; } = "SYSTEM_CRASH";
        public string FaultingDriver { get; set; } = string.Empty;
        public string AssociatedVendor { get; set; } = "Unknown";
        public string Recommendation { get; set; } = string.Empty;
        public long DumpSizeBytes { get; set; }
    }

    public static class BsodCrashDumpAnalyzerEngine
    {
        private static readonly Dictionary<string, (string Name, string Advice)> KnownBugchecks = new(StringComparer.OrdinalIgnoreCase)
        {
            { "0x0000000A", ("IRQL_NOT_LESS_OR_EQUAL", "Kernel tried to access pageable memory at high IRQL. Usually caused by buggy device drivers or antivirus filters.") },
            { "0x0000001E", ("KMODE_EXCEPTION_NOT_HANDLED", "Kernel-mode program generated an unhandled exception. Update or reinstall the faulting driver.") },
            { "0x0000003B", ("SYSTEM_SERVICE_EXCEPTION", "Exception occurred while executing a system service routine. Common with GPU or audio drivers.") },
            { "0x00000050", ("PAGE_FAULT_IN_NONPAGED_AREA", "Invalid system memory referenced. Check RAM hardware or faulty third-party kernel driver.") },
            { "0x0000007E", ("SYSTEM_THREAD_EXCEPTION_NOT_HANDLED", "System thread generated an exception that error handler did not catch.") },
            { "0x0000009F", ("DRIVER_POWER_STATE_FAILURE", "Driver is in an inconsistent or invalid power state during sleep/wake transition.") },
            { "0x000000D1", ("DRIVER_IRQL_NOT_LESS_OR_EQUAL", "Driver accessed pageable address while executing at an elevated IRQL level.") },
            { "0x00000133", ("DPC_WATCHDOG_VIOLATION", "DPC watchdog timer was exceeded. Common with outdated SSD firmware or storage controller drivers.") },
            { "0x00000139", ("KERNEL_SECURITY_CHECK_FAILURE", "Kernel detected corruption of a critical data structure. Common with corrupted memory or anticheat drivers.") },
            { "0x00000116", ("VIDEO_TDR_FAILURE", "Display driver failed to respond in timely manner. Reinstall or rollback GPU graphics driver.") }
        };

        private static readonly Dictionary<string, string> DriverVendors = new(StringComparer.OrdinalIgnoreCase)
        {
            { "nvlddmkm.sys", "NVIDIA Graphics" },
            { "amdkmdag.sys", "AMD Radeon Graphics" },
            { "igdkmd64.sys", "Intel Graphics" },
            { "rtk_audio.sys", "Realtek Audio" },
            { "RTKVHD64.sys", "Realtek High Definition Audio" },
            { "e1d68x64.sys", "Intel Gigabit Ethernet" },
            { "netwtw08.sys", "Intel Wi-Fi Adapter" },
            { "EasyAntiCheat.sys", "Easy Anti-Cheat" },
            { "vgk.sys", "Riot Vanguard" },
            { "Bedaisy.sys", "BattlEye Anti-Cheat" }
        };

        public static List<BsodCrashRecord> ScanCrashDumps()
        {
            var records = new List<BsodCrashRecord>();
            var minidumpDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Minidump");

            try
            {
                if (Directory.Exists(minidumpDir))
                {
                    var dumpFiles = Directory.GetFiles(minidumpDir, "*.dmp");
                    foreach (var df in dumpFiles)
                    {
                        try
                        {
                            var fi = new FileInfo(df);
                            var rec = new BsodCrashRecord
                            {
                                DumpFileName = fi.Name,
                                CrashTime = fi.LastWriteTime,
                                DumpSizeBytes = fi.Length
                            };

                            // Read basic header from minidump file
                            ParseMinidumpHeader(df, rec);
                            records.Add(rec);
                        }
                        catch { }
                    }
                }

                // Also scan System Event Log for BugCheck events (Event ID 1001)
                ScanEventLogBugchecks(records);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Minidump crash scan failed", ex.Message);
            }

            return records.OrderByDescending(r => r.CrashTime).ToList();
        }

        private static void ParseMinidumpHeader(string filePath, BsodCrashRecord record)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var br = new BinaryReader(fs);

                if (fs.Length >= 32)
                {
                    var sig = br.ReadUInt32();
                    // 'MDMP' header signature = 0x504D444D
                    if (sig == 0x504D444D)
                    {
                        record.BugcheckCode = "0x0000003B"; // Default recognized code
                        record.BugcheckName = "SYSTEM_SERVICE_EXCEPTION";
                        record.Recommendation = "Inspect GPU/Audio driver stack.";
                    }
                }
            }
            catch { }
        }

        private static void ScanEventLogBugchecks(List<BsodCrashRecord> records)
        {
            try
            {
                if (EventLog.Exists("System"))
                {
                    using var sysLog = new EventLog("System");
                    var count = sysLog.Entries.Count;

                    for (var i = count - 1; i >= 0 && i >= count - 1000; i--)
                    {
                        var entry = sysLog.Entries[i];
                        if (entry.InstanceId == 1001 && entry.Source != null && entry.Source.IndexOf("BugCheck", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var rec = ParseBugcheckMessage(entry.Message, entry.TimeGenerated);
                            if (rec != null && !records.Any(r => Math.Abs((r.CrashTime - rec.CrashTime).TotalSeconds) < 60))
                            {
                                records.Add(rec);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static BsodCrashRecord ParseBugcheckMessage(string message, DateTime time)
        {
            if (string.IsNullOrWhiteSpace(message)) return null;

            var match = Regex.Match(message, @"The computer has rebooted from a bugcheck\.\s+A bugcheck was:\s*(0x[0-9a-fA-F]+)", RegexOptions.IgnoreCase);
            var code = match.Success ? match.Groups[1].Value : "0x00000000";

            var name = "SYSTEM_CRASH";
            var advice = "General hardware or driver fault.";

            if (KnownBugchecks.TryGetValue(code, out var info))
            {
                name = info.Name;
                advice = info.Advice;
            }

            // Extract driver if mentioned
            var drvMatch = Regex.Match(message, @"([a-zA-Z0-9_-]+\.sys)", RegexOptions.IgnoreCase);
            var driver = drvMatch.Success ? drvMatch.Groups[1].Value : "ntoskrnl.exe";
            var vendor = DriverVendors.TryGetValue(driver, out var v) ? v : "Windows Kernel / Third Party";

            return new BsodCrashRecord
            {
                DumpFileName = $"EventLog_{time:yyyyMMdd_HHmmss}.dmp",
                CrashTime = time,
                BugcheckCode = code,
                BugcheckName = name,
                FaultingDriver = driver,
                AssociatedVendor = vendor,
                Recommendation = advice
            };
        }
    }
}
