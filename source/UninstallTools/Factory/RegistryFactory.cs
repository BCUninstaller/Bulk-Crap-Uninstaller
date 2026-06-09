/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class RegistryFactory : IUninstallerFactory
    {
        public static readonly string RegistryNameBundleProviderKey = "BundleProviderKey";
        public static readonly string RegistryNameComment = "Comment";
        public static readonly string RegistryNameDisplayIcon = "DisplayIcon";
        public static readonly string RegistryNameDisplayName = "DisplayName";
        public static readonly string RegistryNameDisplayVersion = "DisplayVersion";
        public static readonly string RegistryNameEstimatedSize = "EstimatedSize";
        public static readonly string RegistryNameInstallDate = "InstallDate";
        public static readonly string RegistryNameInstallLocation = "InstallLocation";
        public static readonly string RegistryNameInstallSource = "InstallSource";
        public static readonly string RegistryNameModifyPath = "ModifyPath";
        public static readonly string RegistryNameParentKeyName = "ParentKeyName";
        public static readonly string RegistryNamePublisher = "Publisher";
        public static readonly string RegistryNameQuietUninstallString = "QuietUninstallString";
        public static readonly IEnumerable<string> RegistryNamesOfUrlSources = new[]
            {"URLInfoAbout", "URLUpdateInfo", "HelpLink"};
        public static readonly string RegistryNameSystemComponent = "SystemComponent";
        public static readonly string RegistryNameUninstallString = "UninstallString";
        public static readonly string RegistryNameWindowsInstaller = "WindowsInstaller";

        private static readonly string RegUninstallersKeyDirect =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private static readonly string RegUninstallersKeyWow =
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private readonly IEnumerable<Guid> _windowsInstallerValidGuids;

        public RegistryFactory(IEnumerable<Guid> windowsInstallerValidGuids)
        {
            _windowsInstallerValidGuids = windowsInstallerValidGuids;
        }

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var uninstallerRegistryKeys = new List<KeyValuePair<RegistryKey, bool>>();

            progressCallback(new ListGenerationProgress(0, -1, Localisation.Progress_Registry_Gathering));

            foreach (var kvp in GetParentRegistryKeys())
            {
                try
                {
                    uninstallerRegistryKeys.AddRange(
                        kvp.Key.GetSubKeyNames()
                            .Select(subkeyName => OpenSubKeySafe(kvp.Key, subkeyName))
                            .Where(subkey => subkey != null)
                            .Select(subkey => new KeyValuePair<RegistryKey, bool>(subkey, kvp.Value)));
                }
                catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException or IOException)
                {
                    Trace.WriteLine($@"Failed to enumerate registry key {kvp.Key.Name} - {ex}");
                }
                finally
                {
                    kvp.Key.Close();
                }
            }

            void WorkLogic(KeyValuePair<RegistryKey, bool> data, List<ApplicationUninstallerEntry> state)
            {
                try
                {
                    var entry = TryCreateFromRegistry(data.Key, data.Value);
                    if (entry != null)
                        state.Add(entry);
                }
                catch (Exception ex)
                {
                    //Uninstaller is invalid or there is no uninstaller in the first place. Skip it to avoid problems.
                    Trace.WriteLine($@"Failed to extract reg entry {data.Key.Name} - {ex}");
                }
                finally
                {
                    data.Key.Close();
                }
            }

            var workSpreader = new ThreadedWorkSpreader<KeyValuePair<RegistryKey, bool>, List<ApplicationUninstallerEntry>>(
                FactoryThreadedHelpers.MaxThreadsPerDrive, WorkLogic,
                list => new List<ApplicationUninstallerEntry>(list.Count),
                pair =>
                {
                    try
                    {
                        return string.Format(Localisation.Progress_Registry_Processing, Path.GetFileName(pair.Key.Name));
                    }
                    catch
                    {
                        return string.Empty;
                    }
                });

            // We are mostly reading from registry, so treat everything as on a single drive
            var dataBuckets = new List<IList<KeyValuePair<RegistryKey, bool>>> { uninstallerRegistryKeys };

            workSpreader.Start(dataBuckets, progressCallback);

            return workSpreader.Join().SelectMany(x => x).ToList();
        }

        private static string GetAboutUrl(RegistryKey uninstallerKey)
        {
            return RegistryNamesOfUrlSources.Select(uninstallerKey.GetStringSafe)
                .FirstOrDefault(tempSource => !string.IsNullOrEmpty(tempSource) && tempSource.Contains('.'));
        }

        private static ApplicationUninstallerEntry GetBasicInformation(RegistryKey uninstallerKey)
        {
            return new ApplicationUninstallerEntry
            {
                RegistryPath = uninstallerKey.Name,
                RegistryKeyName = uninstallerKey.GetKeyName(),
                Comment = uninstallerKey.GetStringSafe(RegistryNameComment),
                RawDisplayName = uninstallerKey.GetStringSafe(RegistryNameDisplayName),
                DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(uninstallerKey.GetStringSafe(RegistryNameDisplayVersion)),
                ParentKeyName = uninstallerKey.GetStringSafe(RegistryNameParentKeyName),
                Publisher = uninstallerKey.GetStringSafe(RegistryNamePublisher),
                UninstallString = GetUninstallString(uninstallerKey),
                QuietUninstallString = GetQuietUninstallString(uninstallerKey),
                ModifyPath = uninstallerKey.GetStringSafe(RegistryNameModifyPath),
                InstallLocation = uninstallerKey.GetStringSafe(RegistryNameInstallLocation),
                InstallSource = uninstallerKey.GetStringSafe(RegistryNameInstallSource),
                SystemComponent = Convert.ToInt32(uninstallerKey.GetValue(RegistryNameSystemComponent, 0)) != 0,
                DisplayIcon = uninstallerKey.GetStringSafe(RegistryNameDisplayIcon)
            };
        }

        private static FileSize GetEstimatedSize(RegistryKey uninstallerKey)
        {
            try
            {
                // Use Convert.ToInt64 because some applications put a string in here instead of a number
                var tempSize = Convert.ToInt64(uninstallerKey.GetValue(RegistryNameEstimatedSize, 0));
                return FileSize.FromKilobytes(tempSize);
            }
            catch (SystemException e) when (e is FormatException or InvalidCastException)
            {
                return FileSize.Empty;
            }
        }

        private static Guid GetGuid(RegistryKey uninstallerKey)
        {
            // Look for a GUID registry entry
            var tempGuidString = uninstallerKey.GetStringSafe(RegistryNameBundleProviderKey);

            if (GuidTools.GuidTryParse(tempGuidString, out var resultGuid))
                return resultGuid;

            if (GuidTools.TryExtractGuid(uninstallerKey.GetKeyName(), out resultGuid))
                return resultGuid;
            string uninstallString = GetUninstallString(uninstallerKey);
            // Look for a valid GUID in the path
            return GuidTools.TryExtractGuid(uninstallString, out resultGuid) ? resultGuid : Guid.Empty;
        }

        private static string GetUninstallString(RegistryKey uninstallerKey)
        {
            return GetKeyFuzzy(uninstallerKey, RegistryNameUninstallString) ?? GetQuietUninstallString(uninstallerKey);
        }

        private static string GetQuietUninstallString(RegistryKey uninstallerKey)
        {
            return GetKeyFuzzy(uninstallerKey, RegistryNameQuietUninstallString);
        }

        private static string GetKeyFuzzy(RegistryKey uninstallerKey, string keyName)
        {
            var uninstallString = uninstallerKey.GetStringSafe(keyName);
            if (uninstallString == null)
            {
                // Handle hidden uninstall strings like UninstallString_hidden
                uninstallString = uninstallerKey.GetValueNames()
                    .Where(x => x.StartsWith(keyName, StringComparison.OrdinalIgnoreCase))
                    .Select(uninstallerKey.GetStringSafe)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x));
            }

            return uninstallString;
        }

        private static DateTime GetInstallDate(RegistryKey uninstallerKey)
        {
            var dateString = uninstallerKey.GetStringSafe(RegistryNameInstallDate);
            if (dateString != null && dateString.Length == 8)
            {
                try
                {
                    // Likely to be in YYYYMMDD format
                    return new DateTime(int.Parse(dateString.Substring(0, 4)),
                                    int.Parse(dateString.Substring(4, 2)),
                                    int.Parse(dateString.Substring(6, 2)));
                }
                catch (ArgumentOutOfRangeException)
                {
                    try
                    {
                        // YYYYDDMM format instead of standard YYYYMMDD?
                        return new DateTime(int.Parse(dateString.Substring(0, 4)),
                                            int.Parse(dateString.Substring(6, 2)),
                                            int.Parse(dateString.Substring(4, 2)));
                    }
                    catch (SystemException)
                    {
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return DateTime.MinValue;
        }

        private static bool GetIsUpdate(RegistryKey uninstallerKey)
        {
            var parentKeyName = uninstallerKey.GetStringSafe("ParentKeyName");
            if (!string.IsNullOrEmpty(parentKeyName))
                return true;

            var releaseType = uninstallerKey.GetStringSafe("ReleaseType");
            if (!string.IsNullOrEmpty(releaseType) &&
                releaseType.ContainsAny(new[] { "Update", "Hotfix" }, StringComparison.OrdinalIgnoreCase))
                return true;

            var defaultValue = uninstallerKey.GetStringSafe(null);
            if (string.IsNullOrEmpty(defaultValue))
                return false;

            //Regex WindowsUpdateRegEx = new Regex(@"KB[0-9]{6}$"); //Doesnt work for all cases
            return defaultValue.Length > 6 && defaultValue.StartsWith("KB", StringComparison.Ordinal)
                   && char.IsNumber(defaultValue[2]) && char.IsNumber(defaultValue.Last());
        }

        private static bool GetProtectedFlag(RegistryKey uninstallerKey)
        {
            return Convert.ToInt32(uninstallerKey.GetValue("NoRemove", 0)) != 0;
        }

        private static RegistryKey OpenSubKeySafe(RegistryKey baseKey, string name, bool writable = false)
        {
            try
            {
                return baseKey.OpenSubKey(name, writable);
            }
            catch (SecurityException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static IEnumerable<KeyValuePair<RegistryKey, bool>> GetParentRegistryKeys()
        {
            var keysToCheck = new List<KeyValuePair<RegistryKey, bool>>();

            var hklm = Registry.LocalMachine;
            var hkcu = Registry.CurrentUser;

            if (ProcessTools.Is64BitProcess)
            {
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(OpenSubKeySafe(hklm, RegUninstallersKeyDirect), true));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(OpenSubKeySafe(hkcu, RegUninstallersKeyDirect), true));

                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(OpenSubKeySafe(hklm, RegUninstallersKeyWow), false));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(OpenSubKeySafe(hkcu, RegUninstallersKeyWow), false));
            }
            else
            {
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(OpenSubKeySafe(hklm, RegUninstallersKeyDirect), false));
                keysToCheck.Add(new KeyValuePair<RegistryKey, bool>(OpenSubKeySafe(hkcu, RegUninstallersKeyDirect), false));
            }
            return keysToCheck.Where(x => x.Key != null);
        }

        private static UninstallerType GetUninstallerType(RegistryKey uninstallerKey)
        {
            // Detect MSI installer based on registry entry (the proper way)
            if (Convert.ToInt32(uninstallerKey.GetValue(RegistryNameWindowsInstaller, 0)) != 0)
            {
                return UninstallerType.Msiexec;
            }

            // Detect InnoSetup
            if (uninstallerKey.GetValueNames().Any(x => x.Contains("Inno Setup:")))
            {
                return UninstallerType.InnoSetup;
            }

            // Detect Steam
            if (uninstallerKey.GetKeyName().StartsWith("Steam App ", StringComparison.Ordinal))
            {
                return UninstallerType.Steam;
            }

            var uninstallString = GetUninstallString(uninstallerKey);

            return string.IsNullOrEmpty(uninstallString)
                ? UninstallerType.Unknown
                : UninstallerTypeAdder.GetUninstallerType(uninstallString);
        }

        /// <summary>
        ///     Tries to create a new uninstaller entry. If the registry key doesn't contain valid uninstaller
        ///     information, null is returned. It will throw ArgumentNullException if passed uninstallerKey is null.
        ///     If there are any problems while reading the registry an exception will be thrown as well.
        /// </summary>
        /// <param name="uninstallerKey">Registry key which contains the uninstaller information.</param>
        /// <param name="is64Bit">Is the registry key pointing to a 64 bit subkey?</param>
        private ApplicationUninstallerEntry TryCreateFromRegistry(RegistryKey uninstallerKey, bool is64Bit)
        {
            if (uninstallerKey == null)
                throw new ArgumentNullException(nameof(uninstallerKey));

            var tempEntry = GetBasicInformation(uninstallerKey);
            tempEntry.IsRegistered = true;

            // Check for invalid registry key
            if (tempEntry.RawDisplayName == null)
            {
                if (tempEntry.Publisher == null && !tempEntry.UninstallPossible && !tempEntry.QuietUninstallPossible)
                {
                    //throw new ArgumentException("Supplied key doesn't contain any useful information");
                    return null;
                }
                tempEntry.RawDisplayName = string.Empty;
            }

            // Get rest of the information from registry
            tempEntry.IsProtected = GetProtectedFlag(uninstallerKey);
            tempEntry.InstallDate = GetInstallDate(uninstallerKey);
            tempEntry.EstimatedSize = GetEstimatedSize(uninstallerKey);
            tempEntry.AboutUrl = GetAboutUrl(uninstallerKey);

            tempEntry.Is64Bit = is64Bit ? MachineType.X64 : MachineType.X86;
            tempEntry.IsUpdate = GetIsUpdate(uninstallerKey);

            tempEntry.BundleProviderKey = GetGuid(uninstallerKey);

            // Figure out what we are dealing with
            tempEntry.UninstallerKind = GetUninstallerType(uninstallerKey);

            // Corner case with some microsoft application installations.
            // They will sometimes create a naked registry key (product code as reg name) with only the display name value.
            if (tempEntry.UninstallerKind != UninstallerType.Msiexec && tempEntry.BundleProviderKey != Guid.Empty
                && !tempEntry.UninstallPossible && !tempEntry.QuietUninstallPossible)
            {
                if (_windowsInstallerValidGuids.Contains(tempEntry.BundleProviderKey))
                    tempEntry.UninstallerKind = UninstallerType.Msiexec;
            }

            return tempEntry;
        }
    }
}
