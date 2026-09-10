/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Version Differ & Package Changelog Engine
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum DiffItemChangeType
    {
        Added,
        Removed,
        Modified,
        Unchanged
    }

    public sealed class SoftwareDiffItem
    {
        public string RelativePath { get; set; } = string.Empty;
        public DiffItemChangeType ChangeType { get; set; }
        public long OldSizeBytes { get; set; }
        public long NewSizeBytes { get; set; }
        public long SizeDelta => NewSizeBytes - OldSizeBytes;
        public string FileType { get; set; } = "File"; // Binary, DLL, Config, Asset
    }

    public sealed class SoftwareVersionDiffReport
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string OldVersion { get; set; } = string.Empty;
        public string NewVersion { get; set; } = string.Empty;
        public List<SoftwareDiffItem> Items { get; set; } = new();
        public int AddedCount => Items.Count(i => i.ChangeType == DiffItemChangeType.Added);
        public int RemovedCount => Items.Count(i => i.ChangeType == DiffItemChangeType.Removed);
        public int ModifiedCount => Items.Count(i => i.ChangeType == DiffItemChangeType.Modified);
        public long NetSizeDeltaBytes => Items.Sum(i => i.SizeDelta);
    }

    public static class SoftwareUpdateDifferEngine
    {
        public static SoftwareVersionDiffReport CompareDirectories(string oldDir, string newDir, string appName = "", string oldVer = "", string newVer = "")
        {
            var report = new SoftwareVersionDiffReport
            {
                ApplicationName = appName,
                OldVersion = oldVer,
                NewVersion = newVer
            };

            var oldFiles = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            var newFiles = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(oldDir) && Directory.Exists(oldDir))
            {
                foreach (var f in Directory.EnumerateFiles(oldDir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var rel = Path.GetRelativePath(oldDir, f);
                        var size = new FileInfo(f).Length;
                        oldFiles[rel] = size;
                    }
                    catch { }
                }
            }

            if (!string.IsNullOrWhiteSpace(newDir) && Directory.Exists(newDir))
            {
                foreach (var f in Directory.EnumerateFiles(newDir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var rel = Path.GetRelativePath(newDir, f);
                        var size = new FileInfo(f).Length;
                        newFiles[rel] = size;
                    }
                    catch { }
                }
            }

            // Detect Added & Modified
            foreach (var (rel, newSize) in newFiles)
            {
                var ext = Path.GetExtension(rel).ToLowerInvariant();
                var fileType = ext switch
                {
                    ".exe" => "Binary (Executable)",
                    ".dll" => "Dynamic Library (DLL)",
                    ".json" or ".xml" or ".config" or ".ini" => "Configuration",
                    _ => "Asset / Data"
                };

                if (!oldFiles.TryGetValue(rel, out var oldSize))
                {
                    report.Items.Add(new SoftwareDiffItem
                    {
                        RelativePath = rel,
                        ChangeType = DiffItemChangeType.Added,
                        OldSizeBytes = 0,
                        NewSizeBytes = newSize,
                        FileType = fileType
                    });
                }
                else if (oldSize != newSize)
                {
                    report.Items.Add(new SoftwareDiffItem
                    {
                        RelativePath = rel,
                        ChangeType = DiffItemChangeType.Modified,
                        OldSizeBytes = oldSize,
                        NewSizeBytes = newSize,
                        FileType = fileType
                    });
                }
            }

            // Detect Removed
            foreach (var (rel, oldSize) in oldFiles)
            {
                if (!newFiles.ContainsKey(rel))
                {
                    var ext = Path.GetExtension(rel).ToLowerInvariant();
                    var fileType = ext switch
                    {
                        ".exe" => "Binary (Executable)",
                        ".dll" => "Dynamic Library (DLL)",
                        ".json" or ".xml" or ".config" or ".ini" => "Configuration",
                        _ => "Asset / Data"
                    };

                    report.Items.Add(new SoftwareDiffItem
                    {
                        RelativePath = rel,
                        ChangeType = DiffItemChangeType.Removed,
                        OldSizeBytes = oldSize,
                        NewSizeBytes = 0,
                        FileType = fileType
                    });
                }
            }

            return report;
        }
    }
}
