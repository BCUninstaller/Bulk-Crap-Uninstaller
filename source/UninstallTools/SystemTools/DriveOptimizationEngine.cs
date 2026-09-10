/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Storage Drive TRIM, Media Detection & Health Optimization Engine
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public enum DriveMediaType
    {
        Unknown,
        HDD,
        SSD,
        NVMe,
        Removable
    }

    public sealed class StorageDriveInfo
    {
        public string DriveLetter { get; set; } = string.Empty;
        public string VolumeLabel { get; set; } = string.Empty;
        public string FileSystem { get; set; } = "NTFS";
        public DriveMediaType MediaType { get; set; } = DriveMediaType.SSD;
        public long TotalSizeBytes { get; set; }
        public long FreeSizeBytes { get; set; }
        public double FreePercent => TotalSizeBytes > 0 ? (FreeSizeBytes * 100.0) / TotalSizeBytes : 0.0;
        public bool IsTrimSupported { get; set; } = true;
        public string RecommendedAction => MediaType switch
        {
            DriveMediaType.SSD => "Execute SSD TRIM Optimization (Retrim)",
            DriveMediaType.NVMe => "Execute SSD TRIM Optimization (Retrim)",
            DriveMediaType.HDD => "Execute Defragmentation & Space Consolidation",
            _ => "Standard File System Maintenance"
        };
    }

    public static class DriveOptimizationEngine
    {
        public static List<StorageDriveInfo> GetDrives()
        {
            var results = new List<StorageDriveInfo>();

            try
            {
                var drives = DriveInfo.GetDrives().Where(d => d.IsReady && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable));

                foreach (var d in drives)
                {
                    var driveLetter = d.Name.TrimEnd('\\');
                    var mediaType = DetectMediaType(driveLetter);

                    results.Add(new StorageDriveInfo
                    {
                        DriveLetter = driveLetter,
                        VolumeLabel = string.IsNullOrWhiteSpace(d.VolumeLabel) ? "Local Disk" : d.VolumeLabel,
                        FileSystem = d.DriveFormat,
                        MediaType = mediaType,
                        TotalSizeBytes = d.TotalSize,
                        FreeSizeBytes = d.AvailableFreeSpace,
                        IsTrimSupported = mediaType == DriveMediaType.SSD || mediaType == DriveMediaType.NVMe
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying drive information", ex.Message);
            }

            return results;
        }

        public static DriveMediaType DetectMediaType(string driveLetter)
        {
            try
            {
                // In a Windows environment, WMI MSFT_PhysicalDisk or Win32_DiskDrive provides MediaType
                using var searcher = new ManagementObjectSearcher("SELECT MediaType, Model FROM Win32_DiskDrive");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var model = obj["Model"]?.ToString() ?? string.Empty;
                    if (model.IndexOf("NVMe", StringComparison.OrdinalIgnoreCase) >= 0)
                        return DriveMediaType.NVMe;
                    if (model.IndexOf("SSD", StringComparison.OrdinalIgnoreCase) >= 0)
                        return DriveMediaType.SSD;
                }
            }
            catch
            {
                // Fallback default
            }

            return DriveMediaType.SSD;
        }

        public static bool OptimizeDrive(string driveLetter, bool retrim = true)
        {
            if (string.IsNullOrWhiteSpace(driveLetter)) return false;

            StructuredLogger.Info(LogCategory.General, $"Starting drive optimization for {driveLetter} (Retrim={retrim})");

            try
            {
                // Run defrag.exe /O (Optimize automatically picks TRIM for SSD, defrag for HDD)
                var args = $"{driveLetter} /O";
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "defrag.exe"),
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.WaitForExit(120000); // 2 minutes timeout
                    return proc.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed running drive optimization on {driveLetter}", ex.Message);
            }

            return false;
        }
    }
}
