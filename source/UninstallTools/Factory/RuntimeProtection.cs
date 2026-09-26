/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Runtime.InteropServices;

namespace UninstallTools.Factory
{
    internal static class RuntimeProtection
    {
        // Framework-dependent apps list loaded shared frameworks here; self-contained apps do not.
        private const string AppContextDepsFiles = "APP_CONTEXT_DEPS_FILES";

        internal static bool IsLoadedRuntime(string displayName)
        {
            return IsLoadedRuntime(displayName, AppContext.GetData(AppContextDepsFiles) as string,
                RuntimeInformation.ProcessArchitecture);
        }

        internal static bool IsLoadedRuntime(string displayName, string depsFiles, Architecture processArchitecture)
        {
            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(depsFiles))
                return false;

            var architecture = GetArchitectureName(processArchitecture);
            if (architecture == null)
                return false;

            foreach (var depsFile in depsFiles.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var normalizedPath = depsFile.Replace('\\', '/');
                const string sharedMarker = "/shared/";
                var sharedIndex = normalizedPath.LastIndexOf(sharedMarker, StringComparison.OrdinalIgnoreCase);
                if (sharedIndex < 0)
                    continue;

                var frameworkPath = normalizedPath.Substring(sharedIndex + sharedMarker.Length);
                var parts = frameworkPath.Split('/');
                if (parts.Length != 3 || !parts[2].Equals(
                        $"{parts[0]}.deps.json", StringComparison.OrdinalIgnoreCase))
                    continue;

                var runtimeName = parts[0] switch
                {
                    "Microsoft.WindowsDesktop.App" => "Microsoft Windows Desktop Runtime",
                    "Microsoft.NETCore.App" => "Microsoft .NET Runtime",
                    _ => null
                };

                if (runtimeName != null && displayName.Equals(
                        $"{runtimeName} - {parts[1]} ({architecture})", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static string GetArchitectureName(Architecture architecture)
        {
            return architecture switch
            {
                Architecture.X86 => "x86",
                Architecture.X64 => "x64",
                Architecture.Arm64 => "arm64",
                _ => null
            };
        }
    }
}
