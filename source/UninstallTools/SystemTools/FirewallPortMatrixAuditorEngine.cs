/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Firewall Port Matrix & Public Exception Security Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class FirewallPortRuleItem
    {
        public string RuleName { get; set; } = string.Empty;
        public string Direction { get; set; } = "Inbound";
        public string Action { get; set; } = "Allow";
        public string Protocol { get; set; } = "TCP";
        public string LocalPort { get; set; } = "Any";
        public string Profile { get; set; } = "Public";
        public string ApplicationPath { get; set; } = string.Empty;
        public bool IsPublicInboundAllow => Action.Equals("Allow", StringComparison.OrdinalIgnoreCase) &&
                                            Direction.Equals("Inbound", StringComparison.OrdinalIgnoreCase) &&
                                            (Profile.Contains("Public") || Profile.Contains("All"));
    }

    public static class FirewallPortMatrixAuditorEngine
    {
        private static readonly string RulesKey = @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules";

        public static List<FirewallPortRuleItem> ScanFirewallPortRules()
        {
            var results = new List<FirewallPortRuleItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(RulesKey);
                if (key != null)
                {
                    foreach (var valName in key.GetValueNames())
                    {
                        var data = key.GetValue(valName) as string;
                        if (string.IsNullOrWhiteSpace(data)) continue;

                        var name = ExtractField(data, "Name=") ?? valName;
                        var dir = ExtractField(data, "Dir=") ?? "Inbound";
                        if (dir.Equals("In", StringComparison.OrdinalIgnoreCase)) dir = "Inbound";
                        if (dir.Equals("Out", StringComparison.OrdinalIgnoreCase)) dir = "Outbound";

                        var act = ExtractField(data, "Action=") ?? "Allow";
                        var proto = ExtractField(data, "Protocol=") ?? "Any";
                        var port = ExtractField(data, "LPort=") ?? "Any";
                        var profile = ExtractField(data, "Profile=") ?? "All";
                        var app = ExtractField(data, "App=") ?? string.Empty;

                        results.Add(new FirewallPortRuleItem
                        {
                            RuleName = name,
                            Direction = dir,
                            Action = act,
                            Protocol = proto == "6" ? "TCP" : (proto == "17" ? "UDP" : proto),
                            LocalPort = port,
                            Profile = profile,
                            ApplicationPath = app
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning firewall port rules", ex.Message);
            }

            return results.OrderByDescending(r => r.IsPublicInboundAllow).ThenBy(r => r.RuleName).ToList();
        }

        private static string ExtractField(string ruleData, string fieldPrefix)
        {
            var parts = ruleData.Split('|');
            foreach (var p in parts)
            {
                if (p.StartsWith(fieldPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return p.Substring(fieldPrefix.Length);
                }
            }
            return null;
        }
    }
}
