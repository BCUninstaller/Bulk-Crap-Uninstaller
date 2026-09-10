/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    USB & Peripheral Device Driver Residuals Cleaner Subsystem
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
    public sealed class UsbDeviceResidualItem
    {
        public string HardwareId { get; set; } = string.Empty;
        public string DeviceDesc { get; set; } = "Unknown USB Device";
        public string Manufacturer { get; set; } = "Unknown";
        public string ServiceDriver { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public bool IsNonPresent { get; set; } = true;
    }

    public static class UsbDriverResidualsCleanerEngine
    {
        private static readonly string[] EnumRoots = { @"SYSTEM\CurrentControlSet\Enum\USB", @"SYSTEM\CurrentControlSet\Enum\USBSTOR" };

        public static List<UsbDeviceResidualItem> ScanUsbResiduals()
        {
            var results = new List<UsbDeviceResidualItem>();

            foreach (var root in EnumRoots)
            {
                try
                {
                    using var baseKey = Registry.LocalMachine.OpenSubKey(root);
                    if (baseKey != null)
                    {
                        foreach (var vidPidKey in baseKey.GetSubKeyNames())
                        {
                            try
                            {
                                using var sub = baseKey.OpenSubKey(vidPidKey);
                                if (sub == null) continue;

                                foreach (var instance in sub.GetSubKeyNames())
                                {
                                    try
                                    {
                                        using var instKey = sub.OpenSubKey(instance);
                                        if (instKey == null) continue;

                                        var desc = instKey.GetValue("DeviceDesc") as string;
                                        if (!string.IsNullOrEmpty(desc))
                                        {
                                            var semi = desc.IndexOf(';');
                                            if (semi >= 0 && semi < desc.Length - 1) desc = desc.Substring(semi + 1);
                                        }
                                        else
                                        {
                                            desc = instKey.GetValue("FriendlyName") as string ?? vidPidKey;
                                        }

                                        var mfg = instKey.GetValue("Mfg") as string ?? "Generic";
                                        var semiMfg = mfg.IndexOf(';');
                                        if (semiMfg >= 0 && semiMfg < mfg.Length - 1) mfg = mfg.Substring(semiMfg + 1);

                                        var svc = instKey.GetValue("Service") as string ?? string.Empty;

                                        results.Add(new UsbDeviceResidualItem
                                        {
                                            HardwareId = $"{vidPidKey}\\{instance}",
                                            DeviceDesc = desc,
                                            Manufacturer = mfg,
                                            ServiceDriver = svc,
                                            RegistryPath = $@"{root}\{vidPidKey}\{instance}",
                                            IsNonPresent = true
                                        });
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.Registry, $"Failed scanning USB registry {root}", ex.Message);
                }
            }

            return results.OrderBy(u => u.DeviceDesc).ToList();
        }
    }
}
