/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    UWP / AppContainer Sandbox Isolation & Permissions Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.StoreApps
{
    public enum CapabilityRiskLevel
    {
        Normal,
        Elevated,
        HighRisk
    }

    public sealed class AppContainerAuditItem
    {
        public string PackageName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string ManifestPath { get; set; } = string.Empty;
        public List<string> DeclaredCapabilities { get; set; } = new();
        public List<string> HighRiskCapabilities { get; set; } = new();
        public CapabilityRiskLevel OverallRisk { get; set; } = CapabilityRiskLevel.Normal;
        public bool HasFullTrust { get; set; }
        public bool HasBroadFileSystemAccess { get; set; }
        public string RiskSummary => OverallRisk switch
        {
            CapabilityRiskLevel.HighRisk => "High Risk (Unrestricted full filesystem / full trust execution)",
            CapabilityRiskLevel.Elevated => "Elevated (Access to microphone, webcam, or enterprise credentials)",
            _ => "Standard (Isolated inside AppContainer sandbox)"
        };
    }

    public static class AppContainerIsolationAuditorEngine
    {
        private static readonly string[] DangerousCapabilities =
        {
            "broadFileSystemAccess",
            "runFullTrust",
            "enterpriseAuthentication",
            "sharedUserCertificates",
            "localSystemServices",
            "systemManagement",
            "packageQuery",
            "confirmAppClose"
        };

        private static readonly string[] ElevatedCapabilities =
        {
            "webcam",
            "microphone",
            "location",
            "userAccountInformation",
            "picturesLibrary",
            "videosLibrary",
            "documentsLibrary"
        };

        public static List<AppContainerAuditItem> AuditInstalledStorePackages()
        {
            var results = new List<AppContainerAuditItem>();

            try
            {
                var appxPackagesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WindowsApps");
                if (Directory.Exists(appxPackagesDir))
                {
                    var packageDirs = Directory.GetDirectories(appxPackagesDir);
                    foreach (var pDir in packageDirs)
                    {
                        var manifestFile = Path.Combine(pDir, "AppxManifest.xml");
                        if (File.Exists(manifestFile))
                        {
                            var audit = AuditManifest(manifestFile);
                            if (audit != null)
                            {
                                results.Add(audit);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Store packages directory audit restricted", ex.Message);
            }

            return results.OrderByDescending(r => r.OverallRisk).ThenBy(r => r.DisplayName).ToList();
        }

        public static AppContainerAuditItem AuditManifest(string manifestFilePath)
        {
            if (string.IsNullOrWhiteSpace(manifestFilePath) || !File.Exists(manifestFilePath))
                return null;

            try
            {
                var doc = XDocument.Load(manifestFilePath);
                var root = doc.Root;
                if (root == null) return null;

                var ns = root.GetDefaultNamespace();
                var identity = root.Element(ns + "Identity");
                var packageName = identity?.Attribute("Name")?.Value ?? Path.GetFileName(Path.GetDirectoryName(manifestFilePath));
                var publisher = identity?.Attribute("Publisher")?.Value ?? "Microsoft Corporation";

                var properties = root.Element(ns + "Properties");
                var displayName = properties?.Element(ns + "DisplayName")?.Value ?? packageName;

                var capList = new List<string>();
                var highRiskCaps = new List<string>();

                var capContainer = root.Element(ns + "Capabilities");
                if (capContainer != null)
                {
                    foreach (var elem in capContainer.Elements())
                    {
                        var capName = elem.Attribute("Name")?.Value ?? elem.Name.LocalName;
                        if (!string.IsNullOrEmpty(capName))
                        {
                            capList.Add(capName);

                            if (DangerousCapabilities.Any(d => d.Equals(capName, StringComparison.OrdinalIgnoreCase)))
                            {
                                highRiskCaps.Add(capName);
                            }
                        }
                    }
                }

                var hasFullTrust = capList.Any(c => c.Equals("runFullTrust", StringComparison.OrdinalIgnoreCase));
                var hasBroadFs = capList.Any(c => c.Equals("broadFileSystemAccess", StringComparison.OrdinalIgnoreCase));

                var risk = CapabilityRiskLevel.Normal;
                if (highRiskCaps.Count > 0 || hasFullTrust || hasBroadFs)
                {
                    risk = CapabilityRiskLevel.HighRisk;
                }
                else if (capList.Any(c => ElevatedCapabilities.Contains(c.ToLowerInvariant())))
                {
                    risk = CapabilityRiskLevel.Elevated;
                }

                return new AppContainerAuditItem
                {
                    PackageName = packageName,
                    DisplayName = displayName,
                    Publisher = publisher,
                    ManifestPath = manifestFilePath,
                    DeclaredCapabilities = capList,
                    HighRiskCapabilities = highRiskCaps,
                    OverallRisk = risk,
                    HasFullTrust = hasFullTrust,
                    HasBroadFileSystemAccess = hasBroadFs
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
