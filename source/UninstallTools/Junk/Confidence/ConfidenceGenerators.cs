/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;
using UninstallTools.Junk.Containers;

namespace UninstallTools.Junk.Confidence
{
    public static class ConfidenceGenerators
    {
        public static IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, ApplicationUninstallerEntry applicationUninstallerEntry)
        {
            return GenerateConfidence(itemName, null, 0, applicationUninstallerEntry);
        }

        internal static IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, string itemParentPath, int level, ApplicationUninstallerEntry applicationUninstallerEntry)
        {
            var matchResult = MatchStringToProductName(applicationUninstallerEntry, itemName);

            return GenerateConfidence(itemName, matchResult, itemParentPath, level, applicationUninstallerEntry);
        }

        internal static IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, int similarityToEntry, string itemParentPath, int level,
            ApplicationUninstallerEntry applicationUninstallerEntry)
        {
            if (similarityToEntry < 0)
                yield break;

            yield return similarityToEntry < 2
                ? ConfidenceRecords.ProductNamePerfectMatch
                : ConfidenceRecords.ProductNameDodgyMatch;

            // Base rating according to path depth. 0 is best
            yield return new ConfidenceRecord(2 - Math.Abs(level) * 2);

            if (ItemNameEqualsCompanyName(applicationUninstallerEntry, itemName))
                yield return ConfidenceRecords.ItemNameEqualsCompanyName;

            if (level > 0)
            {
                if (applicationUninstallerEntry.PublisherTrimmed.ToLowerInvariant()
                    .Contains(PathTools.GetName(itemParentPath).Replace('_', ' ').ToLowerInvariant()))
                    yield return ConfidenceRecords.CompanyNameMatch;
            }

            if (UninstallToolsGlobalConfig.IsKnownFolder(itemParentPath))
                yield return ConfidenceRecords.DirectlyInsideKnownFolder;
        }

        /// <summary>
        /// -1 if match failed, 0 if string matched perfectly, higher if match was worse
        /// </summary>
        internal static int MatchStringToProductName(ApplicationUninstallerEntry applicationUninstallerEntry, string str)
        {
            var productName = applicationUninstallerEntry.DisplayNameTrimmed.ToLowerInvariant();
            str = str.Replace('_', ' ').ToLowerInvariant().Trim();
            var lowestLength = Math.Min(productName.Length, str.Length);

            // Don't match short strings
            if (lowestLength <= 4)
                return -1;

            var result = Sift4.SimplestDistance(productName, str, 1);

            // Strings match perfectly
            if (result <= 1)
                return result;

            // If the product name contains company name, try trimming it and testing again
            var publisher = applicationUninstallerEntry.PublisherTrimmed.ToLower();
            if (publisher.Length > 4 && productName.Contains(publisher))
            {
                var trimmedProductName = productName.Replace(publisher, "").Trim();
                if (trimmedProductName.Length <= 4)
                    return -1;

                var trimmedResult = Sift4.SimplestDistance(trimmedProductName, str, 1);

                if (trimmedResult <= 1)
                    return trimmedResult;
            }

            var dirToName = str.Contains(productName);
            var nameToDir = productName.Contains(str);

            if (dirToName || nameToDir)
                return 2;

            // Hard cut-off if the difference is more than a third of the checked name
            if (result < lowestLength / 3)
                return result;

            return -1;
        }

        // Check if name is the same as publisher, could be "Adobe AIR" getting matched to a folder "Adobe"
        internal static bool ItemNameEqualsCompanyName(ApplicationUninstallerEntry applicationUninstallerEntry, string itemName)
        {
            var publisher = applicationUninstallerEntry.PublisherTrimmed.ToLowerInvariant();
            itemName = itemName.ToLowerInvariant();
            return !publisher.Equals(applicationUninstallerEntry.DisplayNameTrimmed.ToLowerInvariant()) && publisher.Contains(itemName);
        }

        /// <summary>
        /// Check if there are any similar names that match DisplayNameTrimmed, and if there are then add negative confidence to names other than the best match.
        /// This is to avoid e.g. `AppX Extended` matching junk entries from `AppX`
        /// </summary>
        internal static void TestForSimilarNames(ApplicationUninstallerEntry thisUninstaller, IEnumerable<ApplicationUninstallerEntry> otherUninstallers,
            ICollection<KeyValuePair<JunkResultBase, string>> createdJunk)
        {
            if (createdJunk.Count == 0) return;

            var thisDisplayName = thisUninstaller.DisplayNameTrimmed;
            var otherEntries = otherUninstallers.Where(x => x != thisUninstaller).ToList();
            var sameProductEntries = otherEntries
                .Where(x => string.Equals(x.DisplayNameTrimmed, thisDisplayName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Same-name versions can share product- or publisher-named settings and registry keys.
            // Keep those heuristic matches below the default Good-or-better selection threshold.
            if (sameProductEntries.Count > 0)
            {
                foreach (var sharedJunk in createdJunk.Where(x => sameProductEntries.Any(y =>
                             string.Equals(x.Value, y.DisplayNameTrimmed, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.Value, y.PublisherTrimmed, StringComparison.OrdinalIgnoreCase))))
                {
                    var adjustment = Math.Min(ConfidenceRecords.SameProductNameStillInstalled.Change,
                        1 - sharedJunk.Key.Confidence.GetRawConfidence());
                    sharedJunk.Key.Confidence.Add(new ConfidenceRecord(adjustment,
                        ConfidenceRecords.SameProductNameStillInstalled.Reason));
                }
            }

            // Check if any of the other apps match any of the entries, as long as the app names don't contain this app's name
            var otherFiltered = otherEntries.Where(x => !x.DisplayNameTrimmed.Contains(thisDisplayName)).ToList();
            var matchingWithOther = createdJunk.Where(x => otherFiltered.Any(y => y.DisplayNameTrimmed.Contains(x.Value)));

            if (createdJunk.Count >= 2)
            {
                // Check for folders with similar names like `AppX Extended` and `AppX` and give confidence penalty to every one other than the best match
                matchingWithOther = matchingWithOther
                    .Concat(createdJunk
                        .Where(x => x.Value.Contains(thisDisplayName) || thisDisplayName.Contains(x.Value))
                        .OrderBy(x => Sift4.SimplestDistance(x.Value, thisDisplayName, 100))
                        .Skip(1))
                    .Distinct();
            }

            foreach (var sketchyJunk in matchingWithOther)
                sketchyJunk.Key.Confidence.Add(ConfidenceRecords.UsedBySimilarNamedApp);
        }
    }
}