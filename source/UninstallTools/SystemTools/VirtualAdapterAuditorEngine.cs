/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Virtual Network Adapter & VPN TAP/TUN Residuals Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class VirtualAdapterItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OperationalStatus { get; set; } = "Unknown";
        public string InterfaceType { get; set; } = "Ethernet";
        public bool IsVirtual { get; set; }
        public string ProviderCategory { get; set; } = "Unknown Provider";
    }

    public static class VirtualAdapterAuditorEngine
    {
        private static readonly (string Pattern, string Category)[] KnownVirtualSignatures =
        {
            ("TAP-Windows", "OpenVPN TAP Adapter"),
            ("Wintun", "WireGuard Wintun Adapter"),
            ("VirtualBox", "Oracle VM VirtualBox Host-Only Adapter"),
            ("VMware", "VMware Virtual Ethernet Adapter"),
            ("Hyper-V", "Microsoft Hyper-V Virtual Switch"),
            ("Hamachi", "LogMeIn Hamachi Virtual Adapter"),
            ("Tailscale", "Tailscale Tunnel Interface"),
            ("ZeroTier", "ZeroTier Virtual Network Port"),
            ("Cisco AnyConnect", "Cisco AnyConnect VPN Virtual Adapter"),
            ("NordLynx", "NordVPN NordLynx Adapter"),
            ("ProtonVPN", "ProtonVPN TUN Adapter"),
            ("PANGP", "Palo Alto GlobalProtect Adapter"),
            ("Npcap", "Npcap Loopback Adapter")
        };

        public static List<VirtualAdapterItem> ScanVirtualAdapters()
        {
            var results = new List<VirtualAdapterItem>();

            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var nic in interfaces)
                {
                    var desc = nic.Description;
                    var name = nic.Name;

                    var isVirtual = false;
                    var provider = "Physical Network Interface";

                    foreach (var (sig, cat) in KnownVirtualSignatures)
                    {
                        if (desc.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            isVirtual = true;
                            provider = cat;
                            break;
                        }
                    }

                    if (!isVirtual &&
                        (desc.Contains("Virtual", StringComparison.OrdinalIgnoreCase) ||
                         desc.Contains("VPN", StringComparison.OrdinalIgnoreCase) ||
                         desc.Contains("TAP", StringComparison.OrdinalIgnoreCase) ||
                         desc.Contains("TUN", StringComparison.OrdinalIgnoreCase)))
                    {
                        isVirtual = true;
                        provider = "Generic Virtual / VPN Adapter";
                    }

                    results.Add(new VirtualAdapterItem
                    {
                        Id = nic.Id,
                        Name = name,
                        Description = desc,
                        OperationalStatus = nic.OperationalStatus.ToString(),
                        InterfaceType = nic.NetworkInterfaceType.ToString(),
                        IsVirtual = isVirtual,
                        ProviderCategory = provider
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed scanning network adapters", ex.Message);
            }

            return results.OrderByDescending(a => a.IsVirtual).ThenBy(a => a.Name).ToList();
        }
    }
}
