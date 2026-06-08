/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class ComScanner : JunkCreatorBase
    {
        private static readonly string[] _classesKeys =
        {
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\WOW6432Node",
            @"HKEY_CURRENT_USER\SOFTWARE\Classes",
            @"HKEY_CURRENT_USER\SOFTWARE\Classes\WOW6432Node"
        };

        private List<ComEntry> _comEntries;
        private Dictionary<string, string[]> _extensionKeyNames;

        public override string CategoryName => Localisation.Junk_Clsid_GroupName; // "COM Objects";

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            if (string.IsNullOrEmpty(target.InstallLocation))
                yield break;

            if (UninstallToolsGlobalConfig.IsSystemDirectory(target.InstallLocation))
                yield break;

            foreach (var comEntry in _comEntries.Where(x => PathTools.SubPathIsInsideBasePath(target.InstallLocation, x.FullFilename, true, true)))
            {
                foreach (var interfacePath in comEntry.InterfaceNames)
                {
                    using (var interfaceKey = RegistryTools.OpenRegistryKey(interfacePath, false, true))
                    {
                        if (interfaceKey != null)
                            yield return JunkFromKey(target, interfaceKey);
                    }
                }

                foreach (var classesKeyPath in _classesKeys)
                {
                    using (var classesKey = RegistryTools.OpenRegistryKey(classesKeyPath, false, true))
                    {
                        if (classesKey == null) continue;

                        foreach (var targetSubKeyPath in new[]
                        {
                            Path.Combine("CLSID", comEntry.Guid),
                            Path.Combine("TypeLib", comEntry.Guid),
                            comEntry.ProgId,
                            comEntry.VersionIndependentProgId
                        })
                        {
                            if (targetSubKeyPath != null)
                            {
                                var result = TryGetFromPath(target, classesKey, targetSubKeyPath);
                                if (result != null) yield return result;
                            }
                        }

                        foreach (var extensionKeyName in GetExtensionNames(classesKeyPath))
                        {
                            using (var extensionKey = classesKey.OpenSubKey(extensionKeyName))
                            {
                                if (extensionKey == null) continue;

                                // Contains subkeys with default values containing class guids of the extensions
                                using (var shellExKey = extensionKey.OpenSubKey("ShellEx"))
                                {
                                    if (shellExKey != null)
                                    {
                                        foreach (var shellSubKeyName in shellExKey.GetSubKeyNames())
                                        {
                                            using (var shellSubKey = shellExKey.OpenSubKey(shellSubKeyName))
                                            {
                                                if (string.Equals(shellSubKey?.GetValue(null, null) as string, comEntry.Guid, StringComparison.OrdinalIgnoreCase))
                                                    yield return JunkFromKey(target, shellSubKey);
                                            }
                                        }
                                    }
                                }

                                // Contains default value pointing to a class guid
                                using (var persistentHandlerKey = extensionKey.OpenSubKey("PersistentHandler"))
                                {
                                    if (string.Equals(persistentHandlerKey?.GetValue(null, null) as string, comEntry.Guid, StringComparison.OrdinalIgnoreCase))
                                        yield return JunkFromKey(target, persistentHandlerKey);
                                }

                                if (comEntry.ProgId != null || comEntry.VersionIndependentProgId != null)
                                {
                                    // Contains values with names corresponding to ProgIDs
                                    using (var openWithProgidsKey = extensionKey.OpenSubKey("OpenWithProgIDs"))
                                    {
                                        if (openWithProgidsKey != null)
                                        {
                                            foreach (var progIdName in openWithProgidsKey.GetValueNames())
                                            {
                                                if (string.Equals(progIdName, comEntry.ProgId, StringComparison.OrdinalIgnoreCase) ||
                                                    string.Equals(progIdName, comEntry.VersionIndependentProgId, StringComparison.OrdinalIgnoreCase))
                                                    yield return JunkFromValue(target, openWithProgidsKey.Name, progIdName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            base.Setup(allUninstallers);

            _comEntries = new List<ComEntry>();

            _extensionKeyNames = new Dictionary<string, string[]>();

            foreach (var classesKeyPath in _classesKeys)
            {
                using (var classesKey = RegistryTools.OpenRegistryKey(classesKeyPath, false, true))
                {
                    if (classesKey == null) continue;

                    _extensionKeyNames.Add(classesKeyPath, classesKey.GetSubKeyNames().Where(x => x.Length > 0 && x[0] == '.').ToArray());

                    try
                    {
                        GetClsidEntries(_comEntries, classesKey);
                        GetTypeLibEntries(_comEntries, classesKey);
                    }
                    catch (SystemException ex)
                    {
                        Trace.WriteLine(@"Unexpected error while scanning COM entries, the registry might be corrupted. COM junk detection will not work. Error: " + ex);
                    }
                }
            }

            // Gather com interface info
            // https://docs.microsoft.com/en-us/windows/desktop/com/interface-key
            foreach (var classesKeyPath in _classesKeys)
            {
                using (var interfacesKey = RegistryTools.OpenRegistryKey(Path.Combine(classesKeyPath, "Interface"), false, true))
                {
                    if (interfacesKey == null) continue;

                    foreach (var singleInterfaceKey in interfacesKey.GetSubKeyNames())
                    {
                        using (var proxyKey = interfacesKey.OpenSubKey(Path.Combine(singleInterfaceKey, "ProxyStubClsid32")))
                        {
                            var proxyGuid = proxyKey?.GetValue(null, null) as string;
                            if (proxyGuid == null) continue;

                            var matchClass = _comEntries.FirstOrDefault(x => string.Equals(x.Guid, proxyGuid, StringComparison.OrdinalIgnoreCase));
                            matchClass?.InterfaceNames.Add(Path.Combine(interfacesKey.Name, singleInterfaceKey));
                        }
                    }
                }
            }
        }

        private static void GetClsidEntries(ICollection<ComEntry> results, RegistryKey classes)
        {
            // https://docs.microsoft.com/en-us/windows/desktop/com/clsid-key-hklm
            using (var clsid = RegistryTools.OpenRegistryKey(Path.Combine(classes.Name, "CLSID"), false, true))
            {
                if (clsid == null) return;

                foreach (var clsidGuid in clsid.GetSubKeyNames())
                {
                    // This catches most system classes, rest is caught by IsSystemDirectory check later
                    if (IsSystemGuid(clsidGuid)) continue;

                    RegistryKey guidKey = null;
                    try
                    {
                        guidKey = clsid.OpenSubKey(clsidGuid);
                        if (guidKey == null) continue;

                        var result = results.FirstOrDefault(x => string.Equals(x.Guid, clsidGuid, StringComparison.OrdinalIgnoreCase)) ?? new ComEntry(clsidGuid);

                        using (var inprocKey = guidKey.OpenSubKey("InprocServer32"))
                        {
                            var path = inprocKey?.GetValue(null, null) as string;
                            if (string.IsNullOrEmpty(path)) continue;

                            path = PathTools.NormalizePath(path);
                            if (UninstallToolsGlobalConfig.IsSystemDirectory(path)) continue;

                            result.FullFilename = PathTools.NormalizePath(Environment.ExpandEnvironmentVariables(path));
                        }

                        using (var progIdKey = guidKey.OpenSubKey("ProgID"))
                        {
                            if (progIdKey != null)
                                result.ProgId = progIdKey.GetValue(null, null) as string;
                        }

                        using (var indepProgIdKey = guidKey.OpenSubKey("VersionIndependentProgID"))
                        {
                            if (indepProgIdKey != null)
                                result.VersionIndependentProgId = indepProgIdKey.GetValue(null, null) as string;
                        }

                        results.Add(result);
                    }
                    catch (SystemException ex)
                    {
                        Trace.WriteLine($@"Crash while processing COM GUID: {clsidGuid} - {ex}");
                    }
                    finally
                    {
                        guidKey?.Close();
                    }
                }
            }
        }

        private static void GetTypeLibEntries(ICollection<ComEntry> results, RegistryKey classes)
        {
            using (var typeLibKey = RegistryTools.OpenRegistryKey(Path.Combine(classes.Name, "TypeLib"), false, true))
            {
                if (typeLibKey == null) return;

                foreach (var typeLibKeyGuid in typeLibKey.GetSubKeyNames())
                {
                    if (IsSystemGuid(typeLibKeyGuid)) continue;

                    using (var guidKey = typeLibKey.OpenSubKey(typeLibKeyGuid))
                    {
                        var versionKeyName = guidKey?.GetSubKeyNames().FirstOrDefault();
                        if (versionKeyName == null) continue;

                        var result = results.FirstOrDefault(x => string.Equals(x.Guid, typeLibKeyGuid, StringComparison.OrdinalIgnoreCase)) ?? new ComEntry(typeLibKeyGuid);

                        foreach (var fileKeyPath in new[] { Path.Combine(versionKeyName, "0\\win32"), Path.Combine(versionKeyName, "0\\win64") })
                        {
                            using (var fileKey = guidKey.OpenSubKey(fileKeyPath))
                            {
                                var path = fileKey?.GetValue(null, null) as string;
                                if (string.IsNullOrEmpty(path)) continue;

                                path = PathTools.NormalizePath(path);
                                if (UninstallToolsGlobalConfig.IsSystemDirectory(path)) continue;

                                result.FullFilename = PathTools.NormalizePath(Environment.ExpandEnvironmentVariables(path));
                                results.Add(result);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static bool IsSystemGuid(string guid)
        {
            // Treat missing/invalid GUID text as non-target so callers can safely skip it.
            if (string.IsNullOrEmpty(guid)) return true;
            return guid.Contains("-0000-") || guid[0] != '{';
        }

        private RegistryKeyJunk JunkFromKey(ApplicationUninstallerEntry target, RegistryKey targetKey)
        {
            var junk = new RegistryKeyJunk(targetKey.Name, target, this);
            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
            return junk;
        }

        private IJunkResult JunkFromValue(ApplicationUninstallerEntry target, string key, string valueName)
        {
            var junk = new RegistryValueJunk(key, valueName, target, this);
            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
            return junk;
        }

        private RegistryKeyJunk TryGetFromPath(ApplicationUninstallerEntry target, RegistryKey classesKey, string targetSubKeyPath)
        {
            using (var targetKey = classesKey.OpenSubKey(targetSubKeyPath))
            {
                if (targetKey == null) return null;
                return JunkFromKey(target, targetKey);
            }
        }

        private IEnumerable<string> GetExtensionNames(string classesKey)
        {
            _extensionKeyNames.TryGetValue(classesKey, out var result);
            return result ?? Enumerable.Empty<string>();
        }

        private sealed class ComEntry
        {
            public readonly string Guid;
            public readonly List<string> InterfaceNames = new();

            public string FullFilename;
            //https://docs.microsoft.com/en-us/windows/desktop/com/-progid--key
            public string ProgId;
            public string VersionIndependentProgId;

            public ComEntry(string guid)
            {
                Guid = guid;
            }
        }
    }
}
