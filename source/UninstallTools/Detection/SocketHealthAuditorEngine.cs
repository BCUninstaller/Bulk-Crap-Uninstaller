/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Active Local Network Socket & Port Health Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public sealed class ActiveSocketInfo
    {
        public string Protocol { get; set; } = "TCP";
        public string LocalAddress { get; set; } = "0.0.0.0";
        public int LocalPort { get; set; }
        public string State { get; set; } = "Listen";
        public bool IsPubliclyExposed => LocalAddress == "0.0.0.0" || LocalAddress == "::";
        public string KnownServiceName { get; set; } = "Unknown Application";
        public string SecurityAssessment => IsPubliclyExposed ? "Public Listener (Exposed to all local/external networks)" : "Loopback Only (Local process IPC)";
    }

    public static class SocketHealthAuditorEngine
    {
        private static readonly Dictionary<int, string> WellKnownPorts = new()
        {
            { 80, "HTTP Web Server" },
            { 443, "HTTPS Web Server" },
            { 21, "FTP File Server" },
            { 22, "SSH Remote Shell" },
            { 25, "SMTP Mail" },
            { 53, "DNS Resolver" },
            { 3389, "RDP Remote Desktop" },
            { 3306, "MySQL Database" },
            { 5432, "PostgreSQL Database" },
            { 1433, "Microsoft SQL Server" },
            { 27017, "MongoDB Server" },
            { 8080, "HTTP Alternate / Proxy" },
            { 8443, "HTTPS Alternate" },
            { 9000, "PHP-FPM / SonarQube" }
        };

        public static List<ActiveSocketInfo> QueryActiveSockets()
        {
            var results = new List<ActiveSocketInfo>();

            try
            {
                var ipGlobal = IPGlobalProperties.GetIPGlobalProperties();

                // 1. TCP Listeners
                var tcpEndpoints = ipGlobal.GetActiveTcpListeners();
                foreach (var ep in tcpEndpoints)
                {
                    var port = ep.Port;
                    var name = WellKnownPorts.TryGetValue(port, out var desc) ? desc : $"TCP Service ({port})";

                    results.Add(new ActiveSocketInfo
                    {
                        Protocol = "TCP",
                        LocalAddress = ep.Address.ToString(),
                        LocalPort = port,
                        State = "Listening",
                        KnownServiceName = name
                    });
                }

                // 2. UDP Listeners
                var udpEndpoints = ipGlobal.GetActiveUdpListeners();
                foreach (var ep in udpEndpoints)
                {
                    var port = ep.Port;
                    var name = WellKnownPorts.TryGetValue(port, out var desc) ? desc : $"UDP Service ({port})";

                    results.Add(new ActiveSocketInfo
                    {
                        Protocol = "UDP",
                        LocalAddress = ep.Address.ToString(),
                        LocalPort = port,
                        State = "Active",
                        KnownServiceName = name
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying network sockets", ex.Message);
            }

            return results.OrderByDescending(s => s.IsPubliclyExposed).ThenBy(s => s.LocalPort).ToList();
        }
    }
}
