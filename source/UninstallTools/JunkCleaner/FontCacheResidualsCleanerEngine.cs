/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Font Cache & Stale Font Registrations Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class StaleFontItem
    {
        public string FontName { get; set; } = string.Empty;
        public string FontFileName { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public string IssueReason { get; set; } = "Font file missing from Windows Fonts directory";
    }

    public static class FontCacheResidualsCleanerEngine
    {
        private static readonly string FontsRegKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

        public static List<StaleFontItem> ScanStaleFonts()
        {
            var results = new List<StaleFontItem>();
            var winFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(FontsRegKey);
                if (key != null)
                {
                    foreach (var valName in key.GetValueNames())
                    {
                        var fontFile = key.GetValue(valName) as string;
                        if (string.IsNullOrWhiteSpace(fontFile)) continue;

                        var fullPath = Path.IsPathRooted(fontFile) ? fontFile : Path.Combine(winFontsDir, fontFile);
                        if (!File.Exists(fullPath))
                        {
                            results.Add(new StaleFontItem
                            {
                                FontName = valName,
                                FontFileName = fontFile,
                                RegistryPath = FontsRegKey,
                                IssueReason = $"Font file not found on disk: {fullPath}"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning font registry entries", ex.Message);
            }

            return results;
        }

        public static (int CleanedFonts, int PurgedCacheFiles) PurgeFontCacheAndClean(IEnumerable<StaleFontItem> staleFonts)
        {
            int cleanedFonts = 0;
            int purgedCaches = 0;

            // 1. Clean registry entries
            if (staleFonts != null)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(FontsRegKey, true);
                    if (key != null)
                    {
                        foreach (var f in staleFonts)
                        {
                            try
                            {
                                key.DeleteValue(f.FontName, false);
                                cleanedFonts++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            // 2. Clean FontCache files
            var cacheDirs = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "ServiceProfiles", "LocalService", "AppData", "Local", "FontCache")
            };

            foreach (var d in cacheDirs)
            {
                try
                {
                    if (Directory.Exists(d))
                    {
                        foreach (var f in Directory.GetFiles(d, "FontCache*.dat", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(f);
                                purgedCaches++;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            return (cleanedFonts, purgedCaches);
        }
    }
}
