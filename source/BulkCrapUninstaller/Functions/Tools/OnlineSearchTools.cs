/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.Tools
{
    public static class OnlineSearchTools
    {
        public static void SearchGoogle(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"https://www.google.com/search?q=", GetFullName, SearchSeparatorType.Plus);
        }
        public static void SearchFilehippo(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"http://filehippo.com/search?q=", GetTrimmedName, SearchSeparatorType.Plus);
        }
        public static void SearchSourceforge(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"https://sourceforge.net/directory/?q=", GetTrimmedName, SearchSeparatorType.Escaped);
        }

        public static void SearchFosshub(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"https://www.fosshub.com/search/", GetTrimmedName, SearchSeparatorType.Escaped);
        }

        public static void SearchAlternativeTo(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"https://alternativeto.net/browse/search/?q=", GetTrimmedName, SearchSeparatorType.Plus);
        }

        public static void SearchGithub(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"https://github.com/search?q=", GetTrimmedName, SearchSeparatorType.Plus);
        }

        public static void SearchSlantCo(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            SearchOnline(selectedUninstallers, @"https://www.slant.co/search?query=", GetTrimmedName, SearchSeparatorType.Escaped);
        }

        private static string GetTrimmedName(ApplicationUninstallerEntry entry)
        {
            var displayNameTrimmed = entry.DisplayNameTrimmed;
            return displayNameTrimmed.Length > 3 ? displayNameTrimmed : entry.DisplayName;
        }

        private static string GetFullName(ApplicationUninstallerEntry entry)
        {
            return entry.DisplayName;
        }

        public static void SearchOnline(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers, string searchString, Func<ApplicationUninstallerEntry, string> searchStringGetter, SearchSeparatorType spaceReplacement)
        {
            if (WindowsTools.IsNetworkAvailable())
            {
                var items = selectedUninstallers
                    .Select(searchStringGetter)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(str =>
                    {
                        switch (spaceReplacement)
                        {
                            case SearchSeparatorType.Plus:
                                return HttpUtility.UrlEncode(str);
                                //return str.Replace(' ', '+');
                            case SearchSeparatorType.Escaped:
                                return HttpUtility.UrlEncode(str).Replace("+", "%20");
                            default:
                                throw new ArgumentOutOfRangeException(nameof(spaceReplacement), spaceReplacement, null);
                        }
                    }).Select(y => string.Concat(searchString, y)).ToList();

                if (MessageBoxes.SearchOnlineMessageBox(items.Count) == MessageBoxes.PressedButton.Yes)
                {
                    try
                    {
                        items.ForEach(x => Process.Start(new ProcessStartInfo(x) { UseShellExecute = true }));
                    }
                    catch (Exception ex)
                    {
                        MessageBoxes.SearchOnlineError(ex);
                    }
                }
            }
            else
                MessageBoxes.NoNetworkConnected();
        }

        public enum SearchSeparatorType
        {
            Plus,
            Escaped
        }
    }
}