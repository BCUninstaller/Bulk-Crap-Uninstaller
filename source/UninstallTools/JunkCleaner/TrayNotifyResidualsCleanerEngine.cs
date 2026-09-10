/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    System Tray Notification Area (TrayNotify) Cache Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class TrayNotifyStatus
    {
        public bool HasIconStreams { get; set; }
        public bool HasPastIconsStream { get; set; }
        public int TotalStreamSizeBytes { get; set; }
    }

    public static class TrayNotifyResidualsCleanerEngine
    {
        private static readonly string TrayNotifyKeyPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\TrayNotify";

        public static TrayNotifyStatus QueryTrayNotifyStatus()
        {
            var status = new TrayNotifyStatus();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(TrayNotifyKeyPath);
                if (key != null)
                {
                    var iconStreams = key.GetValue("IconStreams") as byte[];
                    var pastIcons = key.GetValue("PastIconsStream") as byte[];

                    if (iconStreams != null)
                    {
                        status.HasIconStreams = true;
                        status.TotalStreamSizeBytes += iconStreams.Length;
                    }

                    if (pastIcons != null)
                    {
                        status.HasPastIconsStream = true;
                        status.TotalStreamSizeBytes += pastIcons.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying TrayNotify registry key", ex.Message);
            }

            return status;
        }

        public static bool ResetTrayNotifyCache()
        {
            try
            {
                StructuredLogger.Info(LogCategory.General, "Resetting Windows System Tray Notification Cache...");

                // 1. Delete values from registry
                using (var key = Registry.CurrentUser.OpenSubKey(TrayNotifyKeyPath, true))
                {
                    if (key != null)
                    {
                        try { key.DeleteValue("IconStreams", false); } catch { }
                        try { key.DeleteValue("PastIconsStream", false); } catch { }
                    }
                }

                // 2. Restart explorer.exe safely
                var explorers = Process.GetProcessesByName("explorer");
                foreach (var exp in explorers)
                {
                    try { exp.Kill(); } catch { }
                }

                Process.Start("explorer.exe");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed resetting TrayNotify cache", ex.Message);
                try { Process.Start("explorer.exe"); } catch { }
                return false;
            }
        }
    }
}
