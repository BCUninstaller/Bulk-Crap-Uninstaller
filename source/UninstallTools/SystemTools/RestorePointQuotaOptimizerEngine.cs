/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    VSS Shadow Storage & Restore Point Quota Optimizer Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class ShadowStorageInfo
    {
        public string ForVolume { get; set; } = @"C:\";
        public string UsedSpaceStr { get; set; } = "0 B";
        public string AllocatedSpaceStr { get; set; } = "0 B";
        public string MaxSpaceStr { get; set; } = "10 GB";
    }

    public static class RestorePointQuotaOptimizerEngine
    {
        public static List<ShadowStorageInfo> QueryShadowStorage()
        {
            var results = new List<ShadowStorageInfo>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "vssadmin.exe",
                    Arguments = "list shadowstorage",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    var output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(5000);

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    ShadowStorageInfo current = null;

                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (trimmed.StartsWith("For volume:", StringComparison.OrdinalIgnoreCase))
                        {
                            current = new ShadowStorageInfo { ForVolume = trimmed.Substring(11).Trim() };
                            results.Add(current);
                        }
                        else if (current != null)
                        {
                            if (trimmed.StartsWith("Used Shadow Copy Storage space:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.UsedSpaceStr = trimmed.Substring(31).Trim();
                            }
                            else if (trimmed.StartsWith("Allocated Shadow Copy Storage space:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.AllocatedSpaceStr = trimmed.Substring(36).Trim();
                            }
                            else if (trimmed.StartsWith("Maximum Shadow Copy Storage space:", StringComparison.OrdinalIgnoreCase))
                            {
                                current.MaxSpaceStr = trimmed.Substring(34).Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying VSS shadow storage", ex.Message);
            }

            if (results.Count == 0)
            {
                // Add default entry for primary drive
                results.Add(new ShadowStorageInfo
                {
                    ForVolume = @"C:\",
                    UsedSpaceStr = "Dynamic",
                    AllocatedSpaceStr = "Dynamic",
                    MaxSpaceStr = "10 GB (System Default)"
                });
            }

            return results;
        }

        public static bool ResizeShadowStorage(string driveLetter, int maxPercent)
        {
            try
            {
                var drive = driveLetter.TrimEnd('\\');
                var psi = new ProcessStartInfo
                {
                    FileName = "vssadmin.exe",
                    Arguments = $"resize shadowstorage /for={drive} /on={drive} /maxsize={maxPercent}%",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit(5000);
                StructuredLogger.Info(LogCategory.General, $"Resized VSS shadow storage on {drive} to maxsize={maxPercent}%");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed resizing shadow storage on {driveLetter}", ex.Message);
                return false;
            }
        }
    }
}
