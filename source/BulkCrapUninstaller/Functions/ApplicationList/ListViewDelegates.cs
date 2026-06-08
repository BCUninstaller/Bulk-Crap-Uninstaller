/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Resources;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal static class ListViewDelegates
    {
        internal static string AspectToStringConverter(object x)
        {
            return x is long l ? new FileSize(l).ToString() : string.Empty;
        }

        internal static string BoolToYesNoAspectConverter(object rowObject)
        {
            var result = rowObject as bool?;
            return result.ToYesNo();
        }

        internal static object ColumnGuidAspectGetter(object rowObj)
        {
            if (rowObj is ApplicationUninstallerEntry entry)
            {
                var result = entry.BundleProviderKey;
                if (!result.IsEmpty())
                    return $"{result:B}".ToUpperInvariant();
            }
            return string.Empty;
        }

        internal static object ColumnGuidGroupKeyGetter(object rowObj)
        {
            if (rowObj is ApplicationUninstallerEntry entry)
            {
                var result = entry.BundleProviderKey;
                if (result.Equals(Guid.Empty))
                    return Localisable.GuidFound;
            }
            return Localisable.GuidMissing;
        }

        internal static object ColumnInstallLocationGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.InstallLocation);
        }

        internal static object ColumnInstallSourceGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.InstallSource);
        }

        internal static object ColumnPublisherGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return string.IsNullOrEmpty(entry?.PublisherTrimmed) ? CommonStrings.Unknown : entry.PublisherTrimmed;
        }

        internal static object ColumnQuietUninstallStringGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.QuietUninstallString);
        }

        internal static object ColumnSizeAspectGetter(object x)
        {
            if (x is ApplicationUninstallerEntry applicationUninstallerEntry)
                return applicationUninstallerEntry.EstimatedSize.GetKbSize();
            return (long)0;
        }

        internal static object ColumnUninstallStringGroupKeyGetter(object rowObj)
        {
            var entry = rowObj as ApplicationUninstallerEntry;
            return GetFuzzyDirectory(entry?.UninstallString);
        }

        /// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
        internal static object GetFirstCharGroupKeyGetter(object rowobject)
        {
            var entry = rowobject as ApplicationUninstallerEntry;
            if (entry?.DisplayName == null)
                return Localisable.Empty;

            var character = entry.DisplayName.StripAccents().FirstOrDefault(x => !char.IsWhiteSpace(x));

            return character.IsDefault() ? Localisable.Empty : char.ToUpperInvariant(character).ToString();
        }

        internal static object DisplayVersionGroupKeyGetter(object rowObject)
        {
            var entry = rowObject as ApplicationUninstallerEntry;
            if (string.IsNullOrEmpty(entry?.DisplayVersion))
                return CommonStrings.Unknown;

            var dotIndex = entry.DisplayVersion.IndexOf('.');
            return dotIndex > 0 ? entry.DisplayVersion.Substring(0, dotIndex) + ".x" : entry.DisplayVersion;
        }

        internal static object ColumnSizeGroupKeyGetter(object rowObject)
        {
            return rowObject is not ApplicationUninstallerEntry entry ? 0L : entry.EstimatedSize.GetRoundedKbSize();
        }

        /// <summary>
        ///  Convert path to a directory string usable for grouping
        /// </summary>
        private static string GetFuzzyDirectory(string fullCommand)
        {
            if (string.IsNullOrEmpty(fullCommand)) return Localisable.Empty;

            if (fullCommand.StartsWith("msiexec", StringComparison.OrdinalIgnoreCase)
                || fullCommand.Contains("msiexec.exe", StringComparison.OrdinalIgnoreCase))
                return "MsiExec";

            try
            {
                if (fullCommand.Contains('\\'))
                {
                    string strOut;
                    try
                    {
                        strOut = ProcessTools.SeparateArgsFromCommand(fullCommand).FileName;
                    }
                    catch
                    {
                        strOut = fullCommand;
                    }

                    strOut = Path.GetDirectoryName(strOut);

                    strOut = PathTools.GetPathUpToLevel(strOut, 1, false);
                    if (strOut.IsNotEmpty())
                    {
                        return PathTools.PathToNormalCase(strOut); //Path.GetFullPath(strOut);
                    }
                }
            }
            catch
            {
                // Assume path is invalid
            }
            return Localisable.Empty;
        }

        private static readonly string UninstallStringTrimString = '"' + Program.AssemblyLocation.FullName + '\\';

        private static object CleanupUninstallString(string uninstallString)
        {
            if (uninstallString == null)
                return string.Empty;

            if (uninstallString.StartsWith(UninstallStringTrimString, StringComparison.OrdinalIgnoreCase))
            {
                var trimmed = uninstallString.Substring(UninstallStringTrimString.Length);
                var closingQuote = trimmed.IndexOf('"');
                if (closingQuote >= 0)
                {
                    trimmed = trimmed.Remove(closingQuote, 1);
                    return trimmed;
                }
            }
            return uninstallString;
        }

        public static object ColumnUninstallStringGetter(object rowobject)
        {
            if (rowobject is ApplicationUninstallerEntry entry)
                return CleanupUninstallString(entry.UninstallString);
            return string.Empty;
        }

        public static object ColumnQuietUninstallStringGetter(object rowobject)
        {
            if (rowobject is ApplicationUninstallerEntry entry)
                return CleanupUninstallString(entry.QuietUninstallString);
            return string.Empty;
        }
    }
}