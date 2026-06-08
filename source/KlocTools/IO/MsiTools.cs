/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;
using Microsoft.Win32.Interop;

namespace Klocman.IO
{
    public static class MsiTools
    {
        private static readonly int[] GuidRegistryFormatPattern = { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 };

        public static IEnumerable<Guid> MsiEnumProducts()
        {
            var sbProductCode = new StringBuilder(39);
            var iIdx = 0;

            while (MsiWrapper.MsiEnumProducts(iIdx++, sbProductCode) == 0)
            {
                var guidString = sbProductCode.ToString();
                if (GuidTools.GuidTryParse(guidString, out var guid))
                    yield return guid;
                else
                    Console.WriteLine($@"Invalid MSI guid in MsiEnumProducts: {guidString}");
            }
        }

        public static string MsiGetProductInfo(Guid productCode, MsiWrapper.INSTALLPROPERTY property)
        {
            var propertyLen = 512;
            var sbProperty = new StringBuilder(propertyLen);

            var code = MsiWrapper.MsiGetProductInfo(productCode.ToString("B"), property.PropertyName, sbProperty,
                ref propertyLen);

            //if (code != 0)
            //    throw new System.IO.IOException("MsiGetProductInfo returned error code " + code);

            //If code is 0 prevent returning junk
            return code != 0 ? null : sbProperty.ToString();
        }

        public static Guid ConvertBetweenUpgradeAndProductCode(Guid from)
        {
            return new Guid(from.ToString("N").Reverse(GuidRegistryFormatPattern));
        }

        public static X509Certificate2 GetCertificate(Guid productCode)
        {
            var localPackage = MsiGetProductInfo(productCode, MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE);

            if (localPackage == null || !Path.IsPathRooted(localPackage) || !File.Exists(localPackage))
                return null;

            IntPtr certData;
            uint pcb = 0;
            var result = MsiWrapper.MsiGetFileSignatureInformation(localPackage, 0, out certData, null, ref pcb);

            if (result == 0)
                return new X509Certificate2(certData);

            return null;
        }

        public static IEnumerable<string> GetAllComponents()
        {
            var lpComponentBuf = new StringBuilder(40);
            for (var i = 0; ; i++)
            {
                var ret = (ResultWin32)MsiWrapper.MsiEnumComponents(i, lpComponentBuf);
                if (ret == ResultWin32.ERROR_NO_MORE_ITEMS) break;
                if (ret != 0) throw ret.ToException();
                yield return lpComponentBuf.ToString();
            }
        }

        /// <summary>
        /// A list of files, folders, registry keys, and registry values associated with an MSI product.
        /// </summary>
        /// <param name="Filenames">A list of full paths of files and folders.</param>
        /// <param name="RegistryKeys">A list of full paths of registry keys.</param>
        /// <param name="RegistryValues">A list of full paths of registry keys together with value names.</param>
        public record MsiComponentPaths(IReadOnlyCollection<string> Filenames, IReadOnlyCollection<string> RegistryKeys, IReadOnlyCollection<KeyValuePair<string, string>> RegistryValues);

        /// <summary>
        /// Retrieves the raw installation path of a specified component, if available.
        /// </summary>
        /// <remarks>
        /// The path can be either a file path or a registry path. If it's a reg path it starts with a number:
        /// HKEY_CLASSES_ROOT 00
        /// HKEY_CURRENT_USER 01
        /// HKEY_LOCAL_MACHINE 02
        /// HKEY_USERS 03
        /// On 64-bit operating systems, a value of 20 is added to distinguish native 64bit keys from 32-bit keys that are placed in the Wow6432Node (02 -> 22).
        /// If a registry path ends with \ it is a registry key, otherwise it's a registry value.</remarks>
        /// <param name="component">The component identifier to look up.</param>
        /// <returns>The installation path of the component if found; otherwise, null.</returns>
        public static string GetInstalledComponentPathRaw(string component)
        {
            if (ComponentPathLookup.TryGetValue(component, out var value))
                return value;
            
            InitLookups();

            _reverseComponentLookup.TryGetValue(component, out var product);
            if (!string.IsNullOrEmpty(product))
            {
                var lpPathBuf = new StringBuilder(512);
                var pcchPathBuf = lpPathBuf.Capacity;
                var state = MsiWrapper.MsiGetComponentPath(product, component, lpPathBuf, ref pcchPathBuf);
                if (state == MsiWrapper.INSTALLSTATE.INSTALLSTATE_LOCAL) // TODO also include source?
                {
                    value = lpPathBuf.ToString();
                    ComponentPathLookup[component] = value;
                    return value;
                }
            }

            ComponentPathLookup[component] = null;
            return null;
        }

        public static MsiComponentPaths GetInstalledComponentPaths(Guid productCodeGuid)
        {
            InitLookups();

            var productCode = productCodeGuid.ToString("B");
            if (!_componentLookup.Contains(productCode)) return null;

            var filePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var regKeyPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var regValPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var components = _componentLookup[productCode];
            foreach (var component in components)
            {
                var path = GetInstalledComponentPathRaw(component);
                if (!string.IsNullOrEmpty(path) && path.Length > 3) // Length of at least 4 will also exclude disk roots whenever they are included for some reason
                {
                    if (char.IsDigit(path[0]) && char.IsDigit(path[1]) && path[2] == ':')
                    {
                        // Convert to a valid registry path
                        var hiveId = path[1] - '0';
                        var hiveName = hiveId switch
                        {
                            0 => "HKEY_CLASSES_ROOT",
                            1 => "HKEY_CURRENT_USER",
                            2 => "HKEY_LOCAL_MACHINE",
                            3 => "HKEY_USERS",
#if RELEASE
                            _ => hiveId.ToString()
#else
                                _ => throw new ArgumentOutOfRangeException(nameof(hiveId), hiveId, @"Invalid hive")
#endif
                        };
                        path = hiveName + path[3..];

                        // Deal with Wow6432Node redirection
                        var isWow64 = ProcessTools.Is64BitProcess && path[0] == '0';
                        if (isWow64 && !path.Contains(@"\Wow6432Node\", StringComparison.OrdinalIgnoreCase))
                        {
                            path = path.Replace(@"HKEY_LOCAL_MACHINE\Software\", @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\", StringComparison.OrdinalIgnoreCase)
                                       // TODO: Might not be required? HKCR is virtualized and shows both 64 and 32 keys. Some 32bit apps might manage to put their keys in root too?
                                       .Replace(@"HKEY_CLASSES_ROOT\", @"HKEY_CLASSES_ROOT\Wow6432Node\", StringComparison.OrdinalIgnoreCase);
                        }

                        if (path.EndsWith('\\'))
                            regKeyPaths.Add(path.TrimEnd('\\'));
                        else
                            regValPaths.Add(path);
                    }
                    else
                    {
                        filePaths.Add(path);
                    }
                }
            }

            var splitKeyValueNames = regValPaths.Select(x => new KeyValuePair<string, string>(Path.GetDirectoryName(x), Path.GetFileName(x))).ToArray();
            return new MsiComponentPaths(filePaths, regKeyPaths, splitKeyValueNames);
        }

        private static ILookup<string, string> _componentLookup;
        private static Dictionary<string, string> _reverseComponentLookup;
        private static readonly ConcurrentDictionary<string, string> ComponentPathLookup = new();
        public static void InitLookups()
        {
            lock (GuidRegistryFormatPattern)
            {
                if (_componentLookup != null) return;

                var sw = Stopwatch.StartNew();

                // TODO This is slow, on the order of ~8 seconds (4 with AsParallel) on an SSD. It could use caching of GetProductCode and _componentPathLookup
                // 20% of time is spent in GetAllComponents, 80% in GetProductCode
                _componentLookup = GetAllComponents()
                                   .ToList().AsParallel() // Cuts total time by about 45%, ToList is critical
                                   .ToLookup(GetProductCode, StringComparer.OrdinalIgnoreCase);

                _reverseComponentLookup = _componentLookup
                                          .Where(x => !string.IsNullOrEmpty(x.Key))
                                          .SelectMany(x => x.Select(y => new { product = x.Key, component = y, }))
                                          .ToDictionary(x => x.component, x => x.product);

                Trace.WriteLine($"[Performance] Built MSI component lookup in {sw.Elapsed.TotalSeconds:F2} seconds");
            }
            return;

            static string GetProductCode(string component)
            {
                var lpBuf39 = new StringBuilder(40);
                var ret = MsiWrapper.MsiGetProductCode(component, lpBuf39);
                return ret != 0 ? null : lpBuf39.ToString();
            }
        }

        // TODO Use the lookups instead? Some products don't have any components though.
        public static bool IsInstalled(Guid productCodeGuid)
        {
            var productCode = productCodeGuid.ToString("B");
            var state = MsiWrapper.MsiQueryProductState(productCode);
            // https://learn.microsoft.com/en-us/windows/win32/api/msi/nf-msi-msiqueryproductstatea
            // Default - installed for current user, Absent - installed for another user
            return state is MsiWrapper.INSTALLSTATE.INSTALLSTATE_DEFAULT or MsiWrapper.INSTALLSTATE.INSTALLSTATE_ABSENT;
        }
    }
}