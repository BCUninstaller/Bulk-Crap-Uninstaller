/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Error Reporting (WER) Crash Dump Policy Optimizer Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class WerPolicySettings
    {
        public bool IsWerDisabled { get; set; }
        public bool DontSendAdditionalData { get; set; } = true;
        public int DumpCount { get; set; } = 10;
        public int DumpType { get; set; } = 1; // 1 = MiniDump, 2 = FullDump
        public string LocalDumpsPath { get; set; } = @"%LOCALAPPDATA%\CrashDumps";
    }

    public static class WerCrashPolicyOptimizerEngine
    {
        private static readonly string WerRegKey = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting";
        private static readonly string LocalDumpsRegKey = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps";

        public static WerPolicySettings QueryCurrentPolicy()
        {
            var policy = new WerPolicySettings();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(WerRegKey);
                if (key != null)
                {
                    policy.IsWerDisabled = ((int?)key.GetValue("Disabled") ?? 0) == 1;
                    policy.DontSendAdditionalData = ((int?)key.GetValue("DontSendAdditionalData") ?? 0) == 1;
                }

                using var dumpKey = Registry.LocalMachine.OpenSubKey(LocalDumpsRegKey);
                if (dumpKey != null)
                {
                    policy.DumpCount = (int?)dumpKey.GetValue("DumpCount") ?? 10;
                    policy.DumpType = (int?)dumpKey.GetValue("DumpType") ?? 1;
                    policy.LocalDumpsPath = dumpKey.GetValue("DumpFolder") as string ?? @"%LOCALAPPDATA%\CrashDumps";
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying WER policy", ex.Message);
            }

            return policy;
        }

        public static bool ApplyOptimizedPolicy(bool limitDumpStorage, bool preventTelemetryUpload)
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(WerRegKey, true))
                {
                    if (preventTelemetryUpload)
                    {
                        key.SetValue("DontSendAdditionalData", 1, RegistryValueKind.DWord);
                        key.SetValue("LoggingDisabled", 1, RegistryValueKind.DWord);
                    }
                }

                using (var dumpKey = Registry.LocalMachine.CreateSubKey(LocalDumpsRegKey, true))
                {
                    if (limitDumpStorage)
                    {
                        dumpKey.SetValue("DumpCount", 3, RegistryValueKind.DWord);
                        dumpKey.SetValue("DumpType", 1, RegistryValueKind.DWord); // MiniDump only (avoids multi-GB full dumps)
                    }
                }

                StructuredLogger.Info(LogCategory.Registry, "Applied optimized Windows Error Reporting & Crash Dump policies");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, "Failed applying WER policy", ex.Message);
                return false;
            }
        }
    }
}
