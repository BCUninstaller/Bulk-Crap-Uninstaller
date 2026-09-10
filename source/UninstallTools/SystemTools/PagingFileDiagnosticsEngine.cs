/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Virtual Memory Pagefile Diagnostics & Security Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class PagefileConfigInfo
    {
        public string PagingFilesConfig { get; set; } = string.Empty;
        public bool ClearPagefileOnShutdown { get; set; }
        public bool LargeSystemCache { get; set; }
        public long MainDrivePagefileSizeBytes { get; set; }
        public bool PagefileExistsOnDisk { get; set; }
    }

    public static class PagingFileDiagnosticsEngine
    {
        private static readonly string MemManagementKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management";

        public static PagefileConfigInfo QueryPagefileConfig()
        {
            var info = new PagefileConfigInfo();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(MemManagementKey);
                if (key != null)
                {
                    var pf = key.GetValue("PagingFiles") as string[];
                    info.PagingFilesConfig = pf != null && pf.Length > 0 ? string.Join("; ", pf) : "System Managed (Automatic)";

                    info.ClearPagefileOnShutdown = ((int?)key.GetValue("ClearPageFileAtShutdown") ?? 0) == 1;
                    info.LargeSystemCache = ((int?)key.GetValue("LargeSystemCache") ?? 0) == 1;
                }

                var sysDrive = Path.GetPathRoot(Environment.SystemDirectory);
                var pagefilePath = Path.Combine(sysDrive, "pagefile.sys");
                if (File.Exists(pagefilePath))
                {
                    info.PagefileExistsOnDisk = true;
                    try
                    {
                        info.MainDrivePagefileSizeBytes = new FileInfo(pagefilePath).Length;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying Pagefile configuration", ex.Message);
            }

            return info;
        }

        public static bool SetClearPagefileOnShutdown(bool enable)
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(MemManagementKey, true);
                key?.SetValue("ClearPageFileAtShutdown", enable ? 1 : 0, RegistryValueKind.DWord);
                StructuredLogger.Info(LogCategory.Registry, $"Set ClearPageFileAtShutdown to {enable}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, "Failed updating ClearPageFileAtShutdown", ex.Message);
                return false;
            }
        }
    }
}
