/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Bluetooth & Wireless Device Pairing Residuals Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class BluetoothDeviceResidualItem
    {
        public string MacAddress { get; set; } = string.Empty;
        public string DeviceName { get; set; } = "Unknown Bluetooth Device";
        public string DeviceClass { get; set; } = "Audio/Peripheral";
        public string RegistryPath { get; set; } = string.Empty;
        public bool IsPaired { get; set; } = true;
    }

    public static class BluetoothPairingResidualsCleanerEngine
    {
        private static readonly string BthDevicesPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";

        public static List<BluetoothDeviceResidualItem> ScanBluetoothResiduals()
        {
            var results = new List<BluetoothDeviceResidualItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(BthDevicesPath);
                if (key != null)
                {
                    foreach (var mac in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var sub = key.OpenSubKey(mac);
                            if (sub == null) continue;

                            var rawName = sub.GetValue("Name") as byte[];
                            var name = rawName != null ? Encoding.UTF8.GetString(rawName).Trim('\0', ' ') : mac;
                            if (string.IsNullOrWhiteSpace(name)) name = $"Bluetooth Device ({mac})";

                            var cod = (int?)sub.GetValue("COD") ?? 0;
                            var classLabel = GetClassOfDeviceLabel(cod);

                            results.Add(new BluetoothDeviceResidualItem
                            {
                                MacAddress = FormatMac(mac),
                                DeviceName = name,
                                DeviceClass = classLabel,
                                RegistryPath = $@"{BthDevicesPath}\{mac}",
                                IsPaired = true
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying Bluetooth registry devices", ex.Message);
            }

            return results.OrderBy(b => b.DeviceName).ToList();
        }

        private static string FormatMac(string raw)
        {
            if (raw.Length == 12)
            {
                return string.Join(":", Enumerable.Range(0, 6).Select(i => raw.Substring(i * 2, 2)));
            }
            return raw;
        }

        private static string GetClassOfDeviceLabel(int cod)
        {
            var major = (cod >> 8) & 0x1F;
            return major switch
            {
                1 => "Computer / Laptop",
                2 => "Phone / Smartphone",
                4 => "Audio / Headset / Speaker",
                5 => "Peripheral (Keyboard / Mouse / Gamepad)",
                6 => "Imaging (Printer / Scanner)",
                7 => "Wearable (Smartwatch)",
                _ => "Wireless Peripheral"
            };
        }
    }
}
