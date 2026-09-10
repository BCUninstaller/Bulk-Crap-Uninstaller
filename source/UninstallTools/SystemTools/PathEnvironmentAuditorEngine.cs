/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Environment PATH & System Variable Health Auditor Subsystem
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
    public enum PathIssueType
    {
        MissingDirectory,
        DuplicateEntry,
        UnquotedSpaceSecurityRisk,
        TrailingBackslash,
        EmptyEntry,
        LengthLimitWarning
    }

    public sealed class PathAuditItem
    {
        public string PathSegment { get; set; } = string.Empty;
        public string Scope { get; set; } = "System"; // System or User
        public PathIssueType IssueType { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsResolvable { get; set; } = true;
    }

    public sealed class PathAuditReport
    {
        public string RawSystemPath { get; set; } = string.Empty;
        public string RawUserPath { get; set; } = string.Empty;
        public int SystemPathLength => RawSystemPath?.Length ?? 0;
        public int UserPathLength => RawUserPath?.Length ?? 0;
        public List<PathAuditItem> Issues { get; set; } = new();
        public int MissingDirectoryCount => Issues.Count(i => i.IssueType == PathIssueType.MissingDirectory);
        public int DuplicateCount => Issues.Count(i => i.IssueType == PathIssueType.DuplicateEntry);
        public int SecurityRiskCount => Issues.Count(i => i.IssueType == PathIssueType.UnquotedSpaceSecurityRisk);
    }

    public static class PathEnvironmentAuditorEngine
    {
        public const int SafePathLengthLimit = 2048;

        public static PathAuditReport AuditEnvironmentPaths()
        {
            var report = new PathAuditReport();

            try
            {
                // Read System PATH from Registry
                using var sysKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                report.RawSystemPath = sysKey?.GetValue("PATH", "", RegistryValueOptions.DoNotExpandEnvironmentNames) as string ?? string.Empty;

                // Read User PATH from Registry
                using var userKey = Registry.CurrentUser.OpenSubKey(@"Environment");
                report.RawUserPath = userKey?.GetValue("PATH", "", RegistryValueOptions.DoNotExpandEnvironmentNames) as string ?? string.Empty;

                // Audit System PATH
                AuditPathString(report.RawSystemPath, "System", report.Issues);

                // Audit User PATH
                AuditPathString(report.RawUserPath, "User", report.Issues);

                // Check overall length limits
                if (report.SystemPathLength > SafePathLengthLimit)
                {
                    report.Issues.Add(new PathAuditItem
                    {
                        PathSegment = "System PATH Variable",
                        Scope = "System",
                        IssueType = PathIssueType.LengthLimitWarning,
                        Description = $"System PATH length ({report.SystemPathLength} chars) exceeds recommended limit ({SafePathLengthLimit} chars).",
                        IsResolvable = false
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed reading PATH environment variables", ex.Message);
            }

            return report;
        }

        public static void AuditPathString(string rawPath, string scope, List<PathAuditItem> issues)
        {
            if (string.IsNullOrWhiteSpace(rawPath)) return;

            var rawSegments = rawPath.Split(';');
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var seg in rawSegments)
            {
                var trimmed = seg.Trim();

                if (string.IsNullOrEmpty(trimmed))
                {
                    issues.Add(new PathAuditItem
                    {
                        PathSegment = ";;",
                        Scope = scope,
                        IssueType = PathIssueType.EmptyEntry,
                        Description = "Empty PATH segment (redundant semicolon).",
                        IsResolvable = true
                    });
                    continue;
                }

                // Check Duplicates
                if (seen.Contains(trimmed))
                {
                    issues.Add(new PathAuditItem
                    {
                        PathSegment = trimmed,
                        Scope = scope,
                        IssueType = PathIssueType.DuplicateEntry,
                        Description = "Duplicate directory entry in PATH.",
                        IsResolvable = true
                    });
                }
                else
                {
                    seen.Add(trimmed);
                }

                // Check Trailing Backslash (minor style issue)
                if (trimmed.Length > 3 && trimmed.EndsWith("\\"))
                {
                    issues.Add(new PathAuditItem
                    {
                        PathSegment = trimmed,
                        Scope = scope,
                        IssueType = PathIssueType.TrailingBackslash,
                        Description = "Trailing backslash in PATH entry.",
                        IsResolvable = true
                    });
                }

                // Check Missing Directory on Disk
                var expanded = Environment.ExpandEnvironmentVariables(trimmed);
                if (!expanded.Contains("%") && !Directory.Exists(expanded))
                {
                    issues.Add(new PathAuditItem
                    {
                        PathSegment = trimmed,
                        Scope = scope,
                        IssueType = PathIssueType.MissingDirectory,
                        Description = $"Directory does not exist on disk: {expanded}",
                        IsResolvable = true
                    });
                }

                // Check Unquoted spaces risk (if not wrapped in quotes and contains spaces before executable)
                if (trimmed.Contains(" ") && !trimmed.StartsWith("\"") && !trimmed.EndsWith("\""))
                {
                    issues.Add(new PathAuditItem
                    {
                        PathSegment = trimmed,
                        Scope = scope,
                        IssueType = PathIssueType.UnquotedSpaceSecurityRisk,
                        Description = $"Path contains whitespace without quotes, potential DLL search order vulnerability.",
                        IsResolvable = false
                    });
                }
            }
        }

        public static string CleanAndOptimizePath(string rawPath, bool removeNonExistent = true)
        {
            if (string.IsNullOrWhiteSpace(rawPath)) return string.Empty;

            var rawSegments = rawPath.Split(';');
            var cleanedList = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var seg in rawSegments)
            {
                var trimmed = seg.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.Length > 3 && trimmed.EndsWith("\\"))
                {
                    trimmed = trimmed.TrimEnd('\\');
                }

                if (seen.Contains(trimmed)) continue;

                if (removeNonExistent)
                {
                    var expanded = Environment.ExpandEnvironmentVariables(trimmed);
                    if (!expanded.Contains("%") && !Directory.Exists(expanded))
                    {
                        continue; // skip dead paths
                    }
                }

                seen.Add(trimmed);
                cleanedList.Add(trimmed);
            }

            return string.Join(";", cleanedList);
        }
    }
}
