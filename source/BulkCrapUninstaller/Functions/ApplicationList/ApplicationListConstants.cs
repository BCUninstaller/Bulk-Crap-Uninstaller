/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using BulkCrapUninstaller.Properties;
using Klocman.Resources;
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
                return CommonStrings.Unknown;

            var result = entry.IsCertificateValid(true);
            if (!result.HasValue)
                return Localisable.CertificateColumn_NotFound;
            
            return result.Value ? Localisable.CertificateColumn_Verified : Localisable.CertificateColumn_Unverified;
        }

        public static object GetApplicationIntegrityText(ApplicationUninstallerEntry entry)
        {
            if (entry == null) return Localisable.Empty;

            var missingRegistry = entry.IsOrphaned || !entry.IsRegistered;
            var missingUninstaller = !entry.IsValid;

            if (missingRegistry && missingUninstaller)
                return new[] { Localisable.IntegrityColumn_Invalid, Localisable.IntegrityColumn_Unregistered };

            if (missingRegistry)
                return Localisable.IntegrityColumn_Unregistered;

            if (missingUninstaller)
                return Localisable.IntegrityColumn_Invalid;

            return Localisable.IntegrityColumn_Good;
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
