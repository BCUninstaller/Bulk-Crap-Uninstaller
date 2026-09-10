/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DirectX & GPU Shader Cache Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class ShaderCacheStoreItem
    {
        public string Architecture { get; set; } = "DirectX D3D";
        public string DirectoryPath { get; set; } = string.Empty;
        public int FileCount { get; set; }
        public long TotalSizeBytes { get; set; }
    }

    public static class DirectXShaderCacheCleanerEngine
    {
        public static List<ShaderCacheStoreItem> ScanShaderCaches()
        {
            var results = new List<ShaderCacheStoreItem>();
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var targets = new (string Arch, string Path)[]
            {
                ("DirectX D3D Shader Cache", Path.Combine(localAppData, "D3DSCache")),
                ("NVIDIA DXCache", Path.Combine(localAppData, @"NVIDIA\DXCache")),
                ("NVIDIA GLCache (OpenGL/Vulkan)", Path.Combine(localAppData, @"NVIDIA\GLCache")),
                ("AMD Radeon DxCache", Path.Combine(localAppData, @"AMD\DxCache")),
                ("AMD Radeon GLCache", Path.Combine(localAppData, @"AMD\GLCache")),
                ("Intel Graphics Shader Cache", Path.Combine(localAppData, @"Intel\ShaderCache")),
                ("Steam Shader Pre-Cache", Path.Combine(userProfile, @"AppData\Local\Steam\htmlcache"))
            };

            foreach (var (arch, path) in targets)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        var (count, size) = GetDirStats(path);
                        if (count > 0)
                        {
                            results.Add(new ShaderCacheStoreItem
                            {
                                Architecture = arch,
                                DirectoryPath = path,
                                FileCount = count,
                                TotalSizeBytes = size
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning shader cache {path}", ex.Message);
                }
            }

            return results;
        }

        public static (int DeletedFiles, long FreedBytes) CleanShaderCaches(IEnumerable<ShaderCacheStoreItem> stores)
        {
            int files = 0;
            long bytes = 0;
            if (stores == null) return (0, 0);

            foreach (var s in stores)
            {
                try
                {
                    if (Directory.Exists(s.DirectoryPath))
                    {
                        var di = new DirectoryInfo(s.DirectoryPath);
                        foreach (var f in di.GetFiles("*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                var len = f.Length;
                                f.Delete();
                                files++;
                                bytes += len;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            return (files, bytes);
        }

        private static (int count, long size) GetDirStats(string dir)
        {
            int count = 0;
            long size = 0;
            try
            {
                var di = new DirectoryInfo(dir);
                foreach (var f in di.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    count++;
                    size += f.Length;
                }
            }
            catch { }
            return (count, size);
        }
    }
}
