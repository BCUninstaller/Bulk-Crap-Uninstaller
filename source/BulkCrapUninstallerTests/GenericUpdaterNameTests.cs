using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using UninstallTools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Junk.Finders.Registry;
using UninstallTools.Startup;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class GenericUpdaterNameTests
    {
        [TestMethod]
        [DataRow("Update", "Update")]
        [DataRow("Update", "Updates")]
        [DataRow("Update", "Updater")]
        [DataRow("Update", "Microsoft Update Health Tools")]
        [DataRow("UPDATER", "updater")]
        [DataRow("Updates", "Update")]
        [DataRow("Update 1.2.3", "Update")]
        public void GenerateConfidence_GenericUpdaterName_DoesNotMatchUnrelatedItems(string applicationName, string itemName)
        {
            var entry = new ApplicationUninstallerEntry { DisplayName = applicationName };

            var result = ConfidenceGenerators.GenerateConfidence(itemName, entry).ToList();

            Assert.IsEmpty(result);
        }

        [TestMethod]
        [DataRow("Update")]
        [DataRow("Updates")]
        [DataRow("Updater")]
        public void MatchStringToProductName_GenericName_CanBeUsedWithAnIndependentPathCheck(string name)
        {
            var entry = new ApplicationUninstallerEntry { DisplayName = name };

            Assert.AreEqual(-1, ConfidenceGenerators.MatchStringToProductName(entry, name));
            Assert.AreEqual(0, ConfidenceGenerators.MatchStringToProductName(entry, name, allowGenericName: true));
        }

        [TestMethod]
        [DataRow("Update")]
        [DataRow("UPDATER")]
        [DataRow("Updates")]
        [DataRow("Update 1.2.3")]
        public void AssignStartupEntries_GenericUpdaterName_DoesNotMatchAnotherApplication(string applicationName)
        {
            var entry = new ApplicationUninstallerEntry
            {
                DisplayName = applicationName,
                InstallLocation = @"C:\Apps\MongoDBCompass"
            };
            var startup = new TestStartupEntry(entry.DisplayNameTrimmed, @"C:\Apps\DiscordPTB\Update.exe");

            StartupManager.AssignStartupEntries(new[] { entry }, new[] { startup });

            Assert.IsNull(entry.StartupEntries);
        }

        [TestMethod]
        [DataRow("MongoDB Compass")]
        [DataRow("Acme Updater")]
        public void GenerateConfidence_ProductName_StillMatches(string name)
        {
            var entry = new ApplicationUninstallerEntry { DisplayName = name };
            var confidence = new ConfidenceCollection();

            confidence.AddRange(ConfidenceGenerators.GenerateConfidence(name, entry));

            Assert.AreEqual(ConfidenceLevel.Good, confidence.GetConfidence());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void AssignStartupEntries_GenericUpdaterName_StillMatchesByDirectory(bool useUninstallerLocation)
        {
            var entry = new ApplicationUninstallerEntry { DisplayName = "Update" };
            if (useUninstallerLocation)
                entry.UninstallerLocation = @"C:\Apps\MongoDBCompass";
            else
                entry.InstallLocation = @"C:\Apps\MongoDBCompass";
            var startup = new TestStartupEntry("Update", @"C:\Apps\MongoDBCompass\Update.exe");

            StartupManager.AssignStartupEntries(new[] { entry }, new[] { startup });

            Assert.AreSame(startup, entry.StartupEntries.Single());
        }

        [TestMethod]
        public void AssignStartupEntries_ProductName_StillMatchesByName()
        {
            var entry = new ApplicationUninstallerEntry { DisplayName = "Acme Updater" };
            var startup = new TestStartupEntry("Acme Updater", null);

            StartupManager.AssignStartupEntries(new[] { entry }, new[] { startup });

            Assert.AreSame(startup, entry.StartupEntries.Single());
        }

        [TestMethod]
        public void FindJunk_GenericUpdaterName_RequiresExplicitRegistryConnection()
        {
            var keyName = @"Software\BCU_JunkTest_" + Guid.NewGuid().ToString("N");
            var fullKeyName = @"HKEY_CURRENT_USER\" + keyName;
            var entry = new ApplicationUninstallerEntry
            {
                DisplayName = "Update",
                InstallLocation = @"C:\Apps\MongoDBCompass"
            };
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(keyName))
                {
                    using (key.CreateSubKey("Update")) { }
                    using (var connectedKey = key.CreateSubKey("Connected"))
                        connectedKey.SetValue("InstallDir", entry.InstallLocation);
                }
                var scanner = new SoftwareRegKeyScanner();
                scanner.Setup(new[] { entry });

                var results = scanner.FindJunk(entry).OfType<RegistryKeyJunk>()
                    .Where(x => x.FullRegKeyPath.StartsWith(fullKeyName + "\\", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Assert.HasCount(1, results);
                Assert.IsTrue(string.Equals(fullKeyName + @"\Connected", results[0].FullRegKeyPath, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConfidenceLevel.Good, results[0].Confidence.GetConfidence());
            }
            finally
            {
                Registry.CurrentUser.DeleteSubKeyTree(keyName, false);
            }
        }

        private sealed class TestStartupEntry : StartupEntryBase
        {
            public TestStartupEntry(string name, string commandFilePath)
            {
                ProgramNameTrimmed = name;
                CommandFilePath = commandFilePath;
            }

            public override bool Disabled { get; set; }
            public override bool StillExists() => true;
            public override void Delete() => throw new NotSupportedException();
            public override void CreateBackup(string backupPath) => throw new NotSupportedException();
        }
    }
}
