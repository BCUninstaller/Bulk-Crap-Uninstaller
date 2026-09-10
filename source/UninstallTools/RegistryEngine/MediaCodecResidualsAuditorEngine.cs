/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DirectShow & Media Foundation Codec Residuals Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.RegistryEngine
{
    public sealed class MediaCodecResidualItem
    {
        public string CodecName { get; set; } = string.Empty;
        public string ClsidGuid { get; set; } = string.Empty;
        public string FrameworkType { get; set; } = "DirectShow Filter";
        public string ServerBinaryPath { get; set; } = string.Empty;
        public bool IsBinaryMissing { get; set; }
        public string HealthStatus => IsBinaryMissing ? "Orphaned Codec (DLL Missing)" : "Valid Codec Registration";
    }

    public static class MediaCodecResidualsAuditorEngine
    {
        private static readonly string DirectShowCategoryPath = @"CLSID\{083863F1-70DE-11D0-BD40-00A0C911CE86}\Instance";
        private static readonly string MediaFoundationPath = @"SOFTWARE\Classes\MediaFoundation\Transforms";

        public static List<MediaCodecResidualItem> ScanMediaCodecs()
        {
            var results = new List<MediaCodecResidualItem>();
            var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            // 1. DirectShow Filters
            try
            {
                using var dsKey = Registry.ClassesRoot.OpenSubKey(DirectShowCategoryPath);
                if (dsKey != null)
                {
                    foreach (var filterGuid in dsKey.GetSubKeyNames())
                    {
                        try
                        {
                            using var sub = dsKey.OpenSubKey(filterGuid);
                            if (sub == null) continue;

                            var name = sub.GetValue("FriendlyName") as string ?? sub.GetValue("") as string ?? filterGuid;
                            var clsid = sub.GetValue("CLSID") as string ?? filterGuid;

                            // Resolve CLSID in InprocServer32
                            var dllPath = string.Empty;
                            var isMissing = false;

                            using var clsidKey = Registry.ClassesRoot.OpenSubKey($@"CLSID\{clsid}\InprocServer32");
                            if (clsidKey != null)
                            {
                                dllPath = clsidKey.GetValue("") as string ?? string.Empty;
                                if (!string.IsNullOrEmpty(dllPath))
                                {
                                    var full = Path.IsPathRooted(dllPath) ? dllPath : Path.Combine(sysDir, dllPath);
                                    if (!File.Exists(full))
                                    {
                                        isMissing = true;
                                    }
                                }
                            }

                            results.Add(new MediaCodecResidualItem
                            {
                                CodecName = name,
                                ClsidGuid = clsid,
                                FrameworkType = "DirectShow Filter",
                                ServerBinaryPath = dllPath,
                                IsBinaryMissing = isMissing
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning DirectShow filters", ex.Message);
            }

            return results.OrderByDescending(c => c.IsBinaryMissing).ThenBy(c => c.CodecName).ToList();
        }
    }
}
