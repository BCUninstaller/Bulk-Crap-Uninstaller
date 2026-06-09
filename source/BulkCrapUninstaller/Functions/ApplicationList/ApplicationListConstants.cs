/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using BulkCrapUninstaller.Properties;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal static class ApplicationListConstants
    {
        public static ApplicationListColors Colors => Settings.Default.MiscColorblind ? ApplicationListColors.ColorBlind : ApplicationListColors.Normal;

        public static string GetApplicationCertificateText(ApplicationUninstallerEntry entry)
        {
            if (entry == null) return Localisable.Empty;

            if (!Settings.Default.AdvancedTestCertificates)
                return "Disabled";

            var result = entry.IsCertificateValid(true);
            if (!result.HasValue)
                return "None";

            return result.Value ? "Verified" : "Unverified";
        }

        public static object GetApplicationIntegrityText(ApplicationUninstallerEntry entry)
        {
            if (entry == null) return Localisable.Empty;

            var missingRegistry = entry.IsOrphaned || !entry.IsRegistered;
            var missingUninstaller = !entry.IsValid;

            if (missingRegistry && missingUninstaller)
                return new[] { "No uninstaller", "No registry" };

            if (missingRegistry)
                return "No registry";

            if (missingUninstaller)
                return "No uninstaller";

            return "Good";
        }

        public static Color GetApplicationBackColor(ApplicationUninstallerEntry entry)
        {
            if (Settings.Default.AdvancedHighlightSpecial)
            {
                if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                    return Colors.WindowsFeatureColor;

                if (entry.UninstallerKind == UninstallerType.StoreApp)
                    return Colors.WindowsStoreAppColor;

                if (entry.IsOrphaned)
                    return Colors.UnregisteredColor;
            }

            if (!entry.IsValid && Settings.Default.AdvancedTestInvalid)
                return Colors.InvalidColor;

            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value
                        ? Colors.VerifiedColor
                        : Colors.UnverifiedColor;
            }

            return Color.Empty;
        }

        public static Color GetApplicationTreemapColor(ApplicationUninstallerEntry entry)
        {
            if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                return Colors.WindowsFeatureColor;

            if (entry.UninstallerKind == UninstallerType.StoreApp)
                return Colors.WindowsStoreAppColor;

            if (entry.IsOrphaned)
                return Colors.UnregisteredColor;

            if (!entry.IsValid)
                return Colors.InvalidColor;

            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value
                        ? Colors.VerifiedColor
                        : Colors.UnverifiedColor;
            }

            return Color.White;
        }
    }
}
