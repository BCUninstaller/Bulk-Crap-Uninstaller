using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ConfidenceGeneratorsTests
    {
        [TestMethod]
        public void SameProductVersionStillInstalled_DemotesSharedJunkBelowGood()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            var stillInstalled = CreateBlenderEntry("Blender 4.2");
            var junk = CreateBlenderSettingsJunk(target);

            ConfidenceGenerators.TestForSimilarNames(
                target,
                new[] { stillInstalled },
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender") });

            Assert.IsTrue(
                junk.Confidence.GetConfidence() < ConfidenceLevel.Good,
                $"Shared settings remained {junk.Confidence.GetConfidence()} (raw {junk.Confidence.GetRawConfidence()})");
        }

        [TestMethod]
        public void SameProductVersionStillInstalled_DemotesExplicitRegistryConnectionBelowGood()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            target.InstallLocation = @"C:\Program Files\Blender Foundation\Blender 4.4";
            var stillInstalled = CreateBlenderEntry("Blender 4.2");
            var junk = new TestJunkResult(target);
            junk.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(
                "Blender",
                @"HKEY_CURRENT_USER\SOFTWARE\Blender Foundation",
                1,
                target));
            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);

            ConfidenceGenerators.TestForSimilarNames(
                target,
                new[] { stillInstalled },
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender") });

            Assert.IsTrue(
                junk.Confidence.GetConfidence() < ConfidenceLevel.Good,
                $"Explicit shared registry key remained {junk.Confidence.GetConfidence()} (raw {junk.Confidence.GetRawConfidence()})");
        }

        [TestMethod]
        public void SameProductVersionStillInstalled_DemotesExplicitRegistryConnectionWithDifferentPublisherBelowGood()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            target.Publisher = "Open Source Studio";
            target.InstallLocation = @"C:\Program Files\Blender Foundation\Blender 4.4";
            var stillInstalled = CreateBlenderEntry("Blender 4.2");
            stillInstalled.Publisher = "Open Source Studio";
            var junk = new TestJunkResult(target);
            junk.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(
                "Blender",
                @"C:\Users\test\AppData\Roaming\Open Source Studio",
                1,
                target));
            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);

            ConfidenceGenerators.TestForSimilarNames(
                target,
                new[] { stillInstalled },
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender") });

            Assert.IsTrue(
                junk.Confidence.GetConfidence() < ConfidenceLevel.Good,
                $"Explicit shared registry key remained {junk.Confidence.GetConfidence()} (raw {junk.Confidence.GetRawConfidence()})");
        }

        [TestMethod]
        public void SameProductVersionStillInstalled_DemotesSharedPublisherJunkBelowGood()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            var stillInstalled = CreateBlenderEntry("Blender 4.2");
            var junk = CreateBlenderPublisherJunk(target);

            ConfidenceGenerators.TestForSimilarNames(
                target,
                new[] { stillInstalled },
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender Foundation") });

            Assert.IsTrue(
                junk.Confidence.GetConfidence() < ConfidenceLevel.Good,
                $"Shared publisher settings remained {junk.Confidence.GetConfidence()} (raw {junk.Confidence.GetRawConfidence()})");
        }

        [TestMethod]
        public void SameProductVersionStillInstalled_DoesNotDemoteVersionSpecificJunk()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            var stillInstalled = CreateBlenderEntry("Blender 4.2");
            var junk = CreateBlenderVersionJunk(target);
            var initialConfidence = junk.Confidence.GetConfidence();

            ConfidenceGenerators.TestForSimilarNames(
                target,
                new[] { stillInstalled },
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender 4.4") });

            Assert.AreEqual(initialConfidence, junk.Confidence.GetConfidence());
            Assert.AreEqual(ConfidenceLevel.Good, junk.Confidence.GetConfidence());
        }

        [TestMethod]
        public void NoSameProductRemains_DoesNotDemoteOwnedJunk()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            var junk = CreateBlenderSettingsJunk(target);
            var initialConfidence = junk.Confidence.GetConfidence();

            ConfidenceGenerators.TestForSimilarNames(
                target,
                Array.Empty<ApplicationUninstallerEntry>(),
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender") });

            Assert.AreEqual(initialConfidence, junk.Confidence.GetConfidence());
            Assert.AreEqual(ConfidenceLevel.Good, junk.Confidence.GetConfidence());
        }

        [TestMethod]
        public void UnrelatedProductInstalled_DoesNotDemoteOwnedJunk()
        {
            var target = CreateBlenderEntry("Blender 4.4");
            var unrelated = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Krita 5.2",
                Publisher = "KDE"
            };
            var junk = CreateBlenderSettingsJunk(target);
            var initialConfidence = junk.Confidence.GetConfidence();

            ConfidenceGenerators.TestForSimilarNames(
                target,
                new[] { unrelated },
                new List<KeyValuePair<JunkResultBase, string>> { new(junk, "Blender") });

            Assert.AreEqual(initialConfidence, junk.Confidence.GetConfidence());
            Assert.AreEqual(ConfidenceLevel.Good, junk.Confidence.GetConfidence());
        }

        private static ApplicationUninstallerEntry CreateBlenderEntry(string displayName)
        {
            return new ApplicationUninstallerEntry
            {
                RawDisplayName = displayName,
                Publisher = "Blender Foundation"
            };
        }

        private static TestJunkResult CreateBlenderSettingsJunk(ApplicationUninstallerEntry target)
        {
            var junk = new TestJunkResult(target);
            junk.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(
                "Blender",
                @"C:\Users\test\AppData\Roaming\Blender Foundation",
                1,
                target));
            return junk;
        }

        private static TestJunkResult CreateBlenderPublisherJunk(ApplicationUninstallerEntry target)
        {
            var junk = new TestJunkResult(target);
            junk.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(
                "Blender Foundation",
                @"C:\Users\test\AppData\Roaming",
                0,
                target));
            junk.Confidence.Add(ConfidenceRecords.AllSubdirsMatched);
            return junk;
        }

        private static TestJunkResult CreateBlenderVersionJunk(ApplicationUninstallerEntry target)
        {
            var junk = new TestJunkResult(target);
            junk.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(
                "Blender 4.4",
                @"C:\Users\test\AppData\Roaming\Blender Foundation",
                1,
                target));
            return junk;
        }

        private sealed class TestJunkResult : JunkResultBase
        {
            public TestJunkResult(ApplicationUninstallerEntry application) : base(application, null)
            {
            }

            public override void Backup(string backupDirectory) { }
            public override void Delete() { }
            public override void Open() { }
            public override string GetDisplayName() => "Blender";
        }
    }
}
