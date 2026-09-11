/*
    EBUninstaller Pro - Windows Driver & Kernel Module Manager
    Enumeration, health verification, and orphan detection for 3rd-party and kernel drivers.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public enum DriverStartupType
    {
        Boot = 0,
        System = 1,
        Automatic = 2,
        Manual = 3,
        Disabled = 4,
        Unknown = -1
    }

    public enum DriverState
    {
        Running,
        Stopped,
        Unknown
    }

    public class DriverInfoItem
    {
        public string DriverName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DriverPath { get; set; } = string.Empty;
        public DriverStartupType StartupType { get; set; } = DriverStartupType.Unknown;
        public DriverState State { get; set; } = DriverState.Unknown;
        public string Provider { get; set; } = "Unknown";
        public bool IsMicrosoftDriver { get; set; }
        public bool IsOrphaned { get; set; }
        public long FileSizeBytes { get; set; }
    }

    public static class WindowsDriverManager
    {
        private const string ServicesKeyPath = @"SYSTEM\CurrentControlSet\Services";

        public static List<DriverInfoItem> GetInstalledDrivers(bool thirdPartyOnly = false)
        {
            var results = new List<DriverInfoItem>();

            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(ServicesKeyPath);
                if (servicesKey == null) return results;

                foreach (var subKeyName in servicesKey.GetSubKeyNames())
                {
                    try
                    {
                        using var key = servicesKey.OpenSubKey(subKeyName);
                        if (key == null) continue;

                        object? typeObj = key.GetValue("Type");
                        if (!(typeObj is int typeVal)) continue;

                        // Driver types: 1 (Kernel driver), 2 (File system driver), 8 (Recognizer), 512 (Package)
                        if ((typeVal & 0x01) == 0 && (typeVal & 0x02) == 0 && (typeVal & 0x08) == 0)
                            continue;

                        string imagePath = key.GetValue("ImagePath") as string ?? string.Empty;
                        if (string.IsNullOrEmpty(imagePath))
                        {
                            imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", $"{subKeyName}.sys");
                        }
                        else
                        {
                            imagePath = Environment.ExpandEnvironmentVariables(imagePath);
                            if (imagePath.StartsWith(@"\??\"))
                                imagePath = imagePath.Substring(4);
                            else if (imagePath.StartsWith(@"System32\", StringComparison.OrdinalIgnoreCase))
                                imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), imagePath);
                        }

                        string displayName = key.GetValue("DisplayName") as string ?? subKeyName;
                        string description = key.GetValue("Description") as string ?? string.Empty;
                        int startVal = key.GetValue("Start") as int? ?? -1;

                        var startupType = startVal switch
                        {
                            0 => DriverStartupType.Boot,
                            1 => DriverStartupType.System,
                            2 => DriverStartupType.Automatic,
                            3 => DriverStartupType.Manual,
                            4 => DriverStartupType.Disabled,
                            _ => DriverStartupType.Unknown
                        };

                        bool fileExists = File.Exists(imagePath);
                        bool isOrphaned = !fileExists;
                        string provider = "Unknown";
                        long fileSizeBytes = 0;
                        bool isMicrosoft = false;

                        if (fileExists)
                        {
                            try
                            {
                                var fi = new FileInfo(imagePath);
                                fileSizeBytes = fi.Length;

                                var vi = FileVersionInfo.GetVersionInfo(imagePath);
                                provider = vi.CompanyName ?? "Unknown";
                                if (provider.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) ||
                                    (vi.FileDescription != null && vi.FileDescription.Contains("Microsoft", StringComparison.OrdinalIgnoreCase)))
                                {
                                    isMicrosoft = true;
                                }
                            }
                            catch
                            {
                                // Ignore version query errors
                            }
                        }

                        if (thirdPartyOnly && isMicrosoft)
                            continue;

                        results.Add(new DriverInfoItem
                        {
                            DriverName = subKeyName,
                            DisplayName = displayName,
                            Description = description,
                            DriverPath = imagePath,
                            StartupType = startupType,
                            State = DriverState.Unknown,
                            Provider = provider,
                            IsMicrosoftDriver = isMicrosoft,
                            IsOrphaned = isOrphaned,
                            FileSizeBytes = fileSizeBytes
                        });
                    }
                    catch
                    {
                        // Ignore inaccessible subkey
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "WindowsDriverManager", $"Failed to enumerate drivers: {ex.Message}");
            }

            return results.OrderBy(d => d.DisplayName).ToList();
        }

        public static bool SetDriverStartupType(string driverName, DriverStartupType newType)
        {
            if (string.IsNullOrWhiteSpace(driverName)) return false;

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey($@"{ServicesKeyPath}\{driverName}", true);
                if (key == null) return false;

                int val = (int)newType;
                key.SetValue("Start", val, RegistryValueKind.DWord);
                StructuredLogger.Log(LogLevel.Info, "WindowsDriverManager", $"Changed driver '{driverName}' startup type to {newType}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsDriverManager", $"Failed to set startup type for '{driverName}': {ex.Message}");
                return false;
            }
        }

        public static bool RemoveOrphanedDriver(string driverName)
        {
            if (string.IsNullOrWhiteSpace(driverName)) return false;

            try
            {
                using var parentKey = Registry.LocalMachine.OpenSubKey(ServicesKeyPath, true);
                if (parentKey == null) return false;

                using var targetKey = parentKey.OpenSubKey(driverName);
                if (targetKey == null) return false;

                string imagePath = targetKey.GetValue("ImagePath") as string ?? string.Empty;
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // Driver binary still exists; do not delete active driver service key
                    return false;
                }

                parentKey.DeleteSubKeyTree(driverName, false);
                StructuredLogger.Log(LogLevel.Info, "WindowsDriverManager", $"Removed orphaned driver registry entry: {driverName}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsDriverManager", $"Failed to delete orphaned driver '{driverName}': {ex.Message}");
                return false;
            }
        }
    }
}
