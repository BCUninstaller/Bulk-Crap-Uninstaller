/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Services Conflict, Dependency & Port Collision Engine
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public enum ServiceConflictSeverity
    {
        Info,
        Warning,
        Critical
    }

    public sealed class ServiceConflictItem
    {
        public string ConflictType { get; set; } = string.Empty; // PortCollision, DuplicateBinary, MissingExecutable, CircularDependency
        public ServiceConflictSeverity Severity { get; set; } = ServiceConflictSeverity.Warning;
        public string PrimaryService { get; set; } = string.Empty;
        public string ConflictingService { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public int? PortNumber { get; set; }
    }

    public sealed class ServiceSnapshotInfo
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string BinaryPath { get; set; } = string.Empty;
        public string CleanBinaryPath { get; set; } = string.Empty;
        public ServiceStartMode StartType { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public List<string> Dependencies { get; set; } = new();
    }

    public static class ServiceConflictDetectorEngine
    {
        private static readonly Dictionary<int, string> CommonServicePorts = new()
        {
            { 80, "HTTP Web Server (IIS, Apache, Nginx)" },
            { 443, "HTTPS Web Server (IIS, Apache, Nginx)" },
            { 8080, "Alternative Web Server (Tomcat, Jenkins, Proxy)" },
            { 8443, "Alternative HTTPS (Tomcat, Unifi)" },
            { 1433, "Microsoft SQL Server Database" },
            { 3306, "MySQL / MariaDB Database Server" },
            { 5432, "PostgreSQL Database Server" },
            { 27017, "MongoDB Database Server" },
            { 6379, "Redis In-Memory Data Store" },
            { 53, "DNS Server / Resolver" },
            { 21, "FTP Server (IIS, FileZilla Server)" },
            { 25, "SMTP Mail Server" },
            { 3389, "Remote Desktop Protocol (RDP)" }
        };

        public static List<ServiceSnapshotInfo> QuerySystemServices()
        {
            var results = new List<ServiceSnapshotInfo>();

            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");
                if (servicesKey != null)
                {
                    var subKeyNames = servicesKey.GetSubKeyNames();
                    foreach (var sName in subKeyNames)
                    {
                        try
                        {
                            using var sKey = servicesKey.OpenSubKey(sName);
                            if (sKey == null) continue;

                            var imagePath = sKey.GetValue("ImagePath") as string;
                            if (string.IsNullOrWhiteSpace(imagePath)) continue;

                            var displayName = sKey.GetValue("DisplayName") as string ?? sName;
                            var startTypeVal = sKey.GetValue("Start") as int? ?? 3;
                            var startMode = startTypeVal switch
                            {
                                2 => ServiceStartMode.Automatic,
                                3 => ServiceStartMode.Manual,
                                4 => ServiceStartMode.Disabled,
                                _ => ServiceStartMode.Manual
                            };

                            var cleanPath = ExtractCleanExecutablePath(imagePath);
                            var dependsOn = sKey.GetValue("DependOnService") as string[];

                            results.Add(new ServiceSnapshotInfo
                            {
                                ServiceName = sName,
                                DisplayName = displayName,
                                BinaryPath = imagePath,
                                CleanBinaryPath = cleanPath,
                                StartType = startMode,
                                Dependencies = dependsOn != null ? dependsOn.ToList() : new List<string>()
                            });
                        }
                        catch
                        {
                            // Skip inaccessible service keys
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying services registry", ex.Message);
            }

            return results;
        }

        public static List<ServiceConflictItem> AnalyzeConflicts(IEnumerable<ServiceSnapshotInfo> services = null)
        {
            var conflicts = new List<ServiceConflictItem>();
            var serviceList = (services ?? QuerySystemServices()).ToList();

            // 1. Missing Binary Targets (Services pointing to deleted executables)
            foreach (var s in serviceList)
            {
                if (!string.IsNullOrWhiteSpace(s.CleanBinaryPath) &&
                    !s.CleanBinaryPath.StartsWith(@"\SystemRoot\", StringComparison.OrdinalIgnoreCase) &&
                    !s.CleanBinaryPath.StartsWith(@"System32\", StringComparison.OrdinalIgnoreCase) &&
                    !File.Exists(s.CleanBinaryPath))
                {
                    conflicts.Add(new ServiceConflictItem
                    {
                        ConflictType = "MissingExecutable",
                        Severity = ServiceConflictSeverity.Warning,
                        PrimaryService = s.ServiceName,
                        ConflictingService = string.Empty,
                        Description = $"Service '{s.DisplayName}' ({s.ServiceName}) points to non-existent executable: {s.CleanBinaryPath}",
                        Recommendation = "Remove the orphaned service entry or restore missing binaries."
                    });
                }
            }

            // 2. Duplicate Binary Paths across different services
            var groupedByBinary = serviceList
                .Where(s => !string.IsNullOrEmpty(s.CleanBinaryPath) && !s.CleanBinaryPath.EndsWith("svchost.exe", StringComparison.OrdinalIgnoreCase))
                .GroupBy(s => s.CleanBinaryPath, StringComparer.OrdinalIgnoreCase);

            foreach (var g in groupedByBinary)
            {
                var groupList = g.ToList();
                if (groupList.Count > 1)
                {
                    for (int i = 0; i < groupList.Count; i++)
                    {
                        for (int j = i + 1; j < groupList.Count; j++)
                        {
                            conflicts.Add(new ServiceConflictItem
                            {
                                ConflictType = "SharedBinaryRegistration",
                                Severity = ServiceConflictSeverity.Info,
                                PrimaryService = groupList[i].ServiceName,
                                ConflictingService = groupList[j].ServiceName,
                                Description = $"Services '{groupList[i].ServiceName}' and '{groupList[j].ServiceName}' both execute the identical binary: {g.Key}",
                                Recommendation = "Verify whether both service registrations are needed or if one is redundant."
                            });
                        }
                    }
                }
            }

            // 3. Known Port Collisions among Web/Database Servers
            var webAndDbKeywords = new[]
            {
                new { Keyword = "Apache", Port = 80, Alt = 443 },
                new { Keyword = "nginx", Port = 80, Alt = 443 },
                new { Keyword = "W3SVC", Port = 80, Alt = 443 }, // IIS
                new { Keyword = "MSSQL", Port = 1433, Alt = 1433 },
                new { Keyword = "MySQL", Port = 3306, Alt = 3306 },
                new { Keyword = "MariaDB", Port = 3306, Alt = 3306 },
                new { Keyword = "postgresql", Port = 5432, Alt = 5432 },
                new { Keyword = "MongoDB", Port = 27017, Alt = 27017 }
            };

            var matchingServices = new List<(string Name, int Port, string Display)>();
            foreach (var s in serviceList)
            {
                foreach (var kw in webAndDbKeywords)
                {
                    if (s.ServiceName.IndexOf(kw.Keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.DisplayName.IndexOf(kw.Keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.CleanBinaryPath.IndexOf(kw.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matchingServices.Add((s.ServiceName, kw.Port, s.DisplayName));
                        break;
                    }
                }
            }

            var portGroups = matchingServices.GroupBy(x => x.Port);
            foreach (var pg in portGroups)
            {
                var list = pg.ToList();
                if (list.Count > 1)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = i + 1; j < list.Count; j++)
                        {
                            var portDesc = CommonServicePorts.ContainsKey(pg.Key) ? CommonServicePorts[pg.Key] : $"Port {pg.Key}";
                            conflicts.Add(new ServiceConflictItem
                            {
                                ConflictType = "PortCollision",
                                Severity = ServiceConflictSeverity.Critical,
                                PrimaryService = list[i].Name,
                                ConflictingService = list[j].Name,
                                PortNumber = pg.Key,
                                Description = $"Potential port {pg.Key} collision between '{list[i].Display}' and '{list[j].Display}' ({portDesc}).",
                                Recommendation = $"Configure one of the services to use an alternate port or set its startup type to Manual."
                            });
                        }
                    }
                }
            }

            return conflicts;
        }

        public static string ExtractCleanExecutablePath(string rawImagePath)
        {
            if (string.IsNullOrWhiteSpace(rawImagePath)) return string.Empty;

            var trimmed = rawImagePath.Trim();
            if (trimmed.StartsWith("\""))
            {
                var endQuote = trimmed.IndexOf('\"', 1);
                if (endQuote > 1)
                {
                    return trimmed.Substring(1, endQuote - 1).Trim();
                }
            }

            // Unquoted with parameters
            var exeIdx = trimmed.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (exeIdx > 0)
            {
                return trimmed.Substring(0, exeIdx + 4).Trim();
            }

            return trimmed;
        }
    }
}
