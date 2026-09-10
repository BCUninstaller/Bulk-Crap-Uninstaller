/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Firewall Orphaned Rules & Broken Bindings Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class FirewallOrphanRuleItem
    {
        public string RuleName { get; set; } = string.Empty;
        public string Direction { get; set; } = "In"; // In or Out
        public string Action { get; set; } = "Allow"; // Allow or Block
        public string Protocol { get; set; } = "Any";
        public string LocalPorts { get; set; } = "Any";
        public string TargetApplicationPath { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; } = true;
        public string RawRuleString { get; set; } = string.Empty;
        public string RegistryValueName { get; set; } = string.Empty;
    }

    public static class FirewallOrphanCleanerEngine
    {
        private static readonly string FirewallRulesKey = @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules";

        public static List<FirewallOrphanRuleItem> ScanOrphanedRules()
        {
            var results = new List<FirewallOrphanRuleItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(FirewallRulesKey);
                if (key != null)
                {
                    foreach (var valName in key.GetValueNames())
                    {
                        var valData = key.GetValue(valName) as string;
                        if (!string.IsNullOrWhiteSpace(valData))
                        {
                            var item = ParseFirewallRule(valName, valData);
                            if (item != null && item.IsOrphaned)
                            {
                                results.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed reading firewall rules", ex.Message);
            }

            return results;
        }

        public static FirewallOrphanRuleItem ParseFirewallRule(string valueName, string ruleString)
        {
            if (string.IsNullOrWhiteSpace(ruleString)) return null;

            // Rule format: v2.30|Action=Allow|Active=TRUE|Dir=In|Protocol=6|LPort=80|App=C:\path\app.exe|Name=MyRule|...
            var appMatch = Regex.Match(ruleString, @"\|App=([^\|]+)", RegexOptions.IgnoreCase);
            if (!appMatch.Success) return null;

            var appPath = appMatch.Groups[1].Value.Trim();
            var expandedPath = Environment.ExpandEnvironmentVariables(appPath);

            // Check if file exists
            var isMissing = !File.Exists(expandedPath) && !expandedPath.StartsWith("System", StringComparison.OrdinalIgnoreCase);

            var dirMatch = Regex.Match(ruleString, @"\|Dir=([^\|]+)", RegexOptions.IgnoreCase);
            var actionMatch = Regex.Match(ruleString, @"\|Action=([^\|]+)", RegexOptions.IgnoreCase);
            var protoMatch = Regex.Match(ruleString, @"\|Protocol=([^\|]+)", RegexOptions.IgnoreCase);
            var portMatch = Regex.Match(ruleString, @"\|LPort=([^\|]+)", RegexOptions.IgnoreCase);
            var nameMatch = Regex.Match(ruleString, @"\|Name=([^\|]+)", RegexOptions.IgnoreCase);

            return new FirewallOrphanRuleItem
            {
                RegistryValueName = valueName,
                RuleName = nameMatch.Success ? nameMatch.Groups[1].Value : valueName,
                Direction = dirMatch.Success ? dirMatch.Groups[1].Value : "In",
                Action = actionMatch.Success ? actionMatch.Groups[1].Value : "Allow",
                Protocol = protoMatch.Success ? protoMatch.Groups[1].Value : "Any",
                LocalPorts = portMatch.Success ? portMatch.Groups[1].Value : "Any",
                TargetApplicationPath = appPath,
                IsOrphaned = isMissing,
                RawRuleString = ruleString
            };
        }

        public static int CleanOrphanedRules(IEnumerable<FirewallOrphanRuleItem> rules)
        {
            int count = 0;
            if (rules == null) return 0;

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(FirewallRulesKey, true);
                if (key != null)
                {
                    foreach (var r in rules)
                    {
                        try
                        {
                            key.DeleteValue(r.RegistryValueName, false);
                            count++;
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed cleaning firewall rules", ex.Message);
            }

            return count;
        }
    }
}
