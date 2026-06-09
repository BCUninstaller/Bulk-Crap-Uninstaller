/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using BulkCrapUninstaller.Controls;
using BulkCrapUninstaller.Properties;
using UninstallTools;
using UninstallToolsLocalisation = UninstallTools.Properties.Localisation;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal static class ApplicationListConstants
    {
        private static readonly ComponentResourceManager ListLegendResources = new(typeof(ListLegend));

        public static ApplicationListColors Colors => Settings.Default.MiscColorblind ? ApplicationListColors.ColorBlind : ApplicationListColors.Normal;

        public static string GetApplicationCertificateText(ApplicationUninstallerEntry entry)
        {
            if (entry == null) return Localisable.Empty;

            if (!Settings.Default.AdvancedTestCertificates)
                return Localisable.Empty;

            var result = entry.IsCertificateValid(true);
            if (!result.HasValue)
                return "None";

            return result.Value
                ? GetListLegendText("labelVerified.Text", "Verified certificate")
                : GetListLegendText("labelUnverified.Text", "Unverified certificate");
        }

        public static string GetApplicationIntegrityText(ApplicationUninstallerEntry entry)
        {
            if (entry == null) return Localisable.Empty;

            var missingRegistry = entry.IsOrphaned || !entry.IsRegistered;
            var missingUninstaller = !entry.IsValid;

            if (missingRegistry && missingUninstaller)
                return "Missing uninstaller and registry";

            if (missingRegistry)
                return "Missing registry";

            if (missingUninstaller)
                return GetListLegendText("labelInvalid.Text", "Missing uninstaller");

            return UninstallToolsLocalisation.Confidence_Good;
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

        private static string GetListLegendText(string resourceName, string fallback)
        {
            return ListLegendResources.GetString(resourceName) ?? fallback;
        }
    }
}
