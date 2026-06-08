/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class AppExecutablesSearcher : IMissingInfoAdder
    {
        internal static readonly string[] BinaryDirectoryNames;

        static AppExecutablesSearcher()
        {
            var postfixes = new List<string> {"arm64", "aarch64", "32", "64", "x32", "x64", "x86", "x86-64", "ia32", "ia64", "ia-32", "ia-64" };
            var connectors = new[] { "-", "_", " ", "." };
            postfixes.AddRange(connectors.SelectMany(c => postfixes.Select(p => c + p)).ToList());
            var prefixes = new[] { "bin", "binaries", "program", "client", "app", "application", "win", "win7", "win8", "win81", "win10" };

            var names = new List<string>();
            names.AddRange(prefixes);
            names.AddRange(postfixes);
            names.AddRange(prefixes.SelectMany(pr => postfixes.Select(po => pr + po)));

            BinaryDirectoryNames = names.ToArray();
        }

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.SortedExecutables)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.InstallLocation)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            /*var trimmedDispName = target.DisplayNameTrimmed;
            if (string.IsNullOrEmpty(trimmedDispName) || trimmedDispName.Length < 5)
            {
                trimmedDispName = target.DisplayName;
                if (string.IsNullOrEmpty(trimmedDispName))
                    // Impossible to search for the executable without knowing the app name
                    return;
            }*/

            if (!Directory.Exists(target.InstallLocation))
                return;

            var trimmedDispName = target.DisplayNameTrimmed;

            try
            {
                var results = ScanDirectory(new DirectoryInfo(target.InstallLocation));

                target.SortedExecutables = SortListExecutables(results.ExecutableFiles, trimmedDispName)
                    .Select(x => x.FullName).ToArray();
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        internal static ScanDirectoryResult ScanDirectory(DirectoryInfo directory)
        {
            var results = new List<FileInfo>(directory.GetFiles("*.exe", SearchOption.TopDirectoryOnly));
            var binSubdirs = new List<DirectoryInfo>();
            var otherSubdirs = new List<DirectoryInfo>();
            var maybeSubdirs = new List<DirectoryInfo>();
            foreach (var subdir in directory.GetDirectories())
            {
                try
                {
                    var subName = subdir.Name;
                    if (subName.StartsWithAny(BinaryDirectoryNames, StringComparison.OrdinalIgnoreCase))
                    {
                        binSubdirs.Add(subdir);
                        results.AddRange(subdir.GetFiles("*.exe", SearchOption.TopDirectoryOnly));
                    }
                    else
                    {
                        // This skips ISO language codes, much faster than a more specific compare
                        if (subName.Length == 5 && subName[2].Equals('-')) continue;

                        // Directories with very short names likely contain program files
                        if (subName.Length > 3)
                            otherSubdirs.Add(subdir);
                        else 
                            maybeSubdirs.Add(subdir);
                    }
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            if (results.Count == 0 && binSubdirs.Count == 0)
                otherSubdirs.AddRange(maybeSubdirs);

            // 7-Zip console application. Sometimes causes bad display data if it's picked as the most likely executable. No effect on real 7-Zip entries.
            results.RemoveAll(x => string.Equals(x.Name, "7z.exe", StringComparison.OrdinalIgnoreCase));

            return new ScanDirectoryResult(results, binSubdirs, otherSubdirs);
        }

        internal static IEnumerable<FileInfo> SortListExecutables(IEnumerable<FileInfo> targets, string targetString)
        {
            var reportInName = targetString.Contains("report", StringComparison.OrdinalIgnoreCase);
            var crashInName = targetString.Contains("crash", StringComparison.OrdinalIgnoreCase);
            int GetPenaltyPoints(FileInfo fileInfo)
            {
                var fileName = fileInfo.Name;
                if (fileName.Equals("uninstaller.exe", StringComparison.OrdinalIgnoreCase)
                    || fileName.Equals("uninstall.exe", StringComparison.OrdinalIgnoreCase)
                    || fileName.Contains("unins00", StringComparison.OrdinalIgnoreCase))
                    return 20;
                if(!reportInName && fileName.Contains("report", StringComparison.OrdinalIgnoreCase))
                    return 10;
                if(!crashInName && fileName.Contains("crash", StringComparison.OrdinalIgnoreCase))
                    return 10;
                if (fileName.Contains("uninsta", StringComparison.OrdinalIgnoreCase))
                    return 4;
                if (fileName.Contains("unins", StringComparison.OrdinalIgnoreCase))
                    return 2;
                return 0;
            }

            return targets.Select(x => new { x, p = GetPenaltyPoints(x) })
                .OrderBy(x => Sift4.SimplestDistance(x.x.Name, targetString, 3) + x.p)
                .Select(x => x.x);
        }

        internal sealed class ScanDirectoryResult
        {
            public ScanDirectoryResult(ICollection<FileInfo> executableFiles,
                ICollection<DirectoryInfo> binSubdirs, ICollection<DirectoryInfo> otherSubdirs)
            {
                OtherSubdirs = otherSubdirs;
                ExecutableFiles = executableFiles;
                BinSubdirs = binSubdirs;
            }

            public ICollection<DirectoryInfo> BinSubdirs { get; }
            public ICollection<FileInfo> ExecutableFiles { get; }
            public ICollection<DirectoryInfo> OtherSubdirs { get; }
        }
    }
}