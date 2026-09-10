/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    ETW (Event Tracing for Windows) AutoLogger Residuals Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class EtwSessionResidualItem
    {
        public string SessionName { get; set; } = string.Empty;
        public string LogFileName { get; set; } = string.Empty;
        public long LogSizeBytes { get; set; }
        public bool IsThirdParty { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
    }

    public static class EtwSessionResidualsCleanerEngine
    {
        private static readonly string AutoLoggerPath = @"SYSTEM\CurrentControlSet\Control\WMI\Autologger";

        private static readonly string[] SystemLoggers =
        {
            "Circular Kernel Context Logger", "EventLog-Application", "EventLog-System",
            "EventLog-Security", "DiagLog", "Diagtrack-Listener", "WiFiSession",
            "WdiContextLog", "LwtNetLog", "UBPM", "SQMLogger", "Audio", "ReadyBoot"
        };

        public static List<EtwSessionResidualItem> ScanAutoLoggers()
        {
            var results = new List<EtwSessionResidualItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(AutoLoggerPath);
                if (key != null)
                {
                    foreach (var name in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var sub = key.OpenSubKey(name);
                            if (sub == null) continue;

                            var file = sub.GetValue("FileName") as string ?? sub.GetValue("LogFile") as string ?? string.Empty;
                            var isSystem = SystemLoggers.Any(s => s.Equals(name, StringComparison.OrdinalIgnoreCase)) ||
                                           name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);

                            long size = 0;
                            if (!string.IsNullOrEmpty(file))
                            {
                                var expanded = Environment.ExpandEnvironmentVariables(file);
                                if (File.Exists(expanded))
                                {
                                    try { size = new FileInfo(expanded).Length; } catch { }
                                }
                            }

                            results.Add(new EtwSessionResidualItem
                            {
                                SessionName = name,
                                LogFileName = file,
                                LogSizeBytes = size,
                                IsThirdParty = !isSystem,
                                RegistryPath = $@"{AutoLoggerPath}\{name}"
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning ETW AutoLoggers", ex.Message);
            }

            return results.OrderByDescending(e => e.IsThirdParty).ThenBy(e => e.SessionName).ToList();
        }

        public static int CleanThirdPartyLoggers(IEnumerable<EtwSessionResidualItem> loggers)
        {
            int count = 0;
            if (loggers == null) return 0;

            try
            {
                using var baseKey = Registry.LocalMachine.OpenSubKey(AutoLoggerPath, true);
                if (baseKey != null)
                {
                    foreach (var l in loggers.Where(l => l.IsThirdParty))
                    {
                        try
                        {
                            baseKey.DeleteSubKeyTree(l.SessionName, false);
                            count++;

                            if (!string.IsNullOrEmpty(l.LogFileName))
                            {
                                var exp = Environment.ExpandEnvironmentVariables(l.LogFileName);
                                if (File.Exists(exp))
                                {
                                    try { File.Delete(exp); } catch { }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, "Failed deleting ETW loggers", ex.Message);
            }

            return count;
        }
    }
}
