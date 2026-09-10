/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    CNG (Cryptography Next Generation) & CryptoAPI Provider Residuals Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SecurityHardening
{
    public sealed class CryptoProviderItem
    {
        public string ProviderName { get; set; } = string.Empty;
        public string ProviderType { get; set; } = "CNG";
        public string ImagePath { get; set; } = string.Empty;
        public bool IsBinaryMissing { get; set; }
        public string RegistryLocation { get; set; } = string.Empty;
        public string HealthStatus => IsBinaryMissing ? "Orphaned Provider (DLL Missing)" : "Valid / Registered Binary";
    }

    public static class CngProviderAuditorEngine
    {
        private static readonly (string RegPath, string Type)[] ProviderKeys =
        {
            (@"SOFTWARE\Microsoft\Cryptography\Defaults\Provider", "CryptoAPI (CAPI)"),
            (@"SOFTWARE\Microsoft\Cryptography\Defaults\Provider Types", "CryptoAPI Types"),
            (@"SOFTWARE\Microsoft\Cryptography\Configuration\Providers", "CNG Provider")
        };

        public static List<CryptoProviderItem> ScanCryptoProviders()
        {
            var results = new List<CryptoProviderItem>();
            var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            foreach (var (path, type) in ProviderKeys)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(path);
                    if (key != null)
                    {
                        foreach (var subName in key.GetSubKeyNames())
                        {
                            try
                            {
                                using var sub = key.OpenSubKey(subName);
                                if (sub == null) continue;

                                var img = sub.GetValue("Image Path") as string ?? sub.GetValue("Image") as string ?? string.Empty;
                                var isMissing = false;

                                if (!string.IsNullOrEmpty(img))
                                {
                                    var full = Path.IsPathRooted(img) ? img : Path.Combine(sysDir, img);
                                    if (!File.Exists(full))
                                    {
                                        isMissing = true;
                                    }
                                }

                                results.Add(new CryptoProviderItem
                                {
                                    ProviderName = subName,
                                    ProviderType = type,
                                    ImagePath = img,
                                    IsBinaryMissing = isMissing,
                                    RegistryLocation = $@"{path}\{subName}"
                                });
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.Security, $"Failed scanning crypto providers in {path}", ex.Message);
                }
            }

            return results.OrderByDescending(p => p.IsBinaryMissing).ThenBy(p => p.ProviderName).ToList();
        }
    }
}
