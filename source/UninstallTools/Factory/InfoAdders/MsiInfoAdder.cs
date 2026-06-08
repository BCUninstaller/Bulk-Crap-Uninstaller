/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;

namespace UninstallTools.Factory.InfoAdders
{
    public class MsiInfoAdder : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            //Debug.Assert(target.UninstallerKind != UninstallerType.Msiexec);
            if (target.UninstallerKind != UninstallerType.Msiexec)
                return;

            ApplyMsiInfo(target, target.BundleProviderKey);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.BundleProviderKey)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;
        public string[] CanProduceValueNames { get; } =  {
            nameof(ApplicationUninstallerEntry.RawDisplayName),
            nameof(ApplicationUninstallerEntry.DisplayVersion),
            nameof(ApplicationUninstallerEntry.Publisher),
            nameof(ApplicationUninstallerEntry.InstallLocation),
            nameof(ApplicationUninstallerEntry.InstallSource),
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.DisplayIcon),
            nameof(ApplicationUninstallerEntry.AboutUrl),
            nameof(ApplicationUninstallerEntry.InstallDate),
            //nameof(ApplicationUninstallerEntry.SortedExecutables) // TODO: This works but is much too slow to run for every entry
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;

        /// <summary>
        ///     A valid guid is REQUIRED. It doesn't have to be set on the entry, but should be.
        /// </summary>
        private static void ApplyMsiInfo(ApplicationUninstallerEntry entry, Guid guid)
        {
            // Make sure this is a real msiexec product ID
            if (!MsiTools.IsInstalled(guid))
                return;

            FillInMissingInfoMsiHelper(() => entry.RawDisplayName, x => entry.RawDisplayName = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLEDPRODUCTNAME, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME);

            FillInMissingInfoMsiHelper(() => entry.DisplayVersion, x => entry.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(x), guid,
                MsiWrapper.INSTALLPROPERTY.VERSIONSTRING, MsiWrapper.INSTALLPROPERTY.VERSION);

            FillInMissingInfoMsiHelper(() => entry.Publisher, x => entry.Publisher = x, guid,
                MsiWrapper.INSTALLPROPERTY.PUBLISHER);

            FillInMissingInfoMsiHelper(() => entry.InstallLocation, x => entry.InstallLocation = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLLOCATION);

            FillInMissingInfoMsiHelper(() => entry.InstallSource, x => entry.InstallSource = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLSOURCE);

            FillInMissingInfoMsiHelper(() => entry.UninstallerFullFilename, x => entry.UninstallerFullFilename = x, guid,
                MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE);

            FillInMissingInfoMsiHelper(() => entry.DisplayIcon, x => entry.DisplayIcon = x, guid,
                MsiWrapper.INSTALLPROPERTY.PRODUCTICON);

            FillInMissingInfoMsiHelper(() => entry.AboutUrl, x => entry.AboutUrl = x, guid,
                MsiWrapper.INSTALLPROPERTY.HELPLINK, MsiWrapper.INSTALLPROPERTY.URLUPDATEINFO,
                MsiWrapper.INSTALLPROPERTY.URLINFOABOUT);

            if (entry.InstallDate.IsDefault())
            {
                var temp = MsiTools.MsiGetProductInfo(guid, MsiWrapper.INSTALLPROPERTY.INSTALLDATE);
                if (temp.IsNotEmpty())
                {
                    try
                    {
                        entry.InstallDate = new DateTime(int.Parse(temp.Substring(0, 4)),
                                                         int.Parse(temp.Substring(4, 2)),
                                                         int.Parse(temp.Substring(6, 2)));
                    }
                    catch
                    {
                        // Date had invalid format, default to nothing
                    }
                }
            }


            if (string.IsNullOrWhiteSpace(entry.InstallLocation) || entry.SortedExecutables == null)
            {
                var paths = MsiTools.GetInstalledComponentPaths(guid);
                if (paths == null) return;

                // Checking .exe paths seems to be pretty reliable
                var executables = paths.Filenames.Where(x => x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (executables.Length > 0)
                {
                    var addedInstallLocation = false;
                    if (string.IsNullOrWhiteSpace(entry.InstallLocation))
                    {
                        var bestGuess = executables.GroupBy(Path.GetDirectoryName, StringComparer.OrdinalIgnoreCase)
                                                   .Where(x => !UninstallToolsGlobalConfig.IsSystemDirectory(x.Key)) // Deal with apps installing executables to the Windows directory
                                                   .OrderByDescending(x => UninstallToolsGlobalConfig.IsPathInsideProgramFiles(x.Key)) // Always prefer PF
                                                   .ThenBy(x => x.Key.Length) // Shortest path is most likely to be the application root
                                                   .FirstOrDefault();

                        if (!string.IsNullOrEmpty(bestGuess?.Key))
                        {
                            entry.InstallLocation = bestGuess.Key;
                            addedInstallLocation = true;
                        }
                    }

                    // Update executable list based on new information. Keep existing executables if they exist, as they may be more accurate than the MSI component paths
                    var executablesToSort = executables.Select(x => new FileInfo(x));

                    if (entry.SortedExecutables != null) executablesToSort = executablesToSort.Concat(entry.SortedExecutables.Select(x => new FileInfo(x)));

                    if (addedInstallLocation) executablesToSort = executablesToSort.Concat(AppExecutablesSearcher.ScanDirectory(new DirectoryInfo(entry.InstallLocation)).ExecutableFiles);
                    
                    entry.SortedExecutables = AppExecutablesSearcher.SortListExecutables(executablesToSort.DistinctBy(x => x.FullName, StringComparer.OrdinalIgnoreCase), entry.DisplayName)
                                                                    .Select(x => x.FullName).ToArray();
                }

                // If the exe search failed, pick the folder with the most files in it instead (needed with e.g. Net Framework reference assemblies)
                if (string.IsNullOrWhiteSpace(entry.InstallLocation))
                {
                    var bestGuess = paths.Filenames
                                         .Where(Path.HasExtension)
                                         .GroupBy(Path.GetDirectoryName, StringComparer.OrdinalIgnoreCase)
                                         .Where(x => !UninstallToolsGlobalConfig.IsSystemDirectory(x.Key)) // Deal with apps installing executables to the Windows directory
                                         .OrderByDescending(x => UninstallToolsGlobalConfig.IsPathInsideProgramFiles(x.Key)) // Always prefer PF
                                         .ThenByDescending(x => x.Count()) // Directory with the highest amount of files is the safest bet
                                         .ThenBy(x => x.Key.Length)
                                         .FirstOrDefault();

                    if (!string.IsNullOrEmpty(bestGuess?.Key))
                        entry.InstallLocation = bestGuess.Key;
                }
            }
        }

        private static void FillInMissingInfoMsiHelper(Func<string> get, Action<string> set, Guid guid,
            params MsiWrapper.INSTALLPROPERTY[] properties)
        {
            if (string.IsNullOrEmpty(get()))
            {
                foreach (var item in properties)
                {
                    var temp = MsiTools.MsiGetProductInfo(guid, item);

                    //IMPORTANT: Do not assign empty strings, they will mess up automatic property creation later on.
                    if (temp.IsNotEmpty())
                        set(temp);
                }
            }
        }
    }
}