using System.Linq;
using BulkCrapUninstaller.Functions.ApplicationList;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class UninstallerListViewUpdaterTests
    {
        [TestMethod]
        public void ReconcileCheckedObjects_VisibleCheckWinsWhenMappedStateIsMissing()
        {
            var app = CreateEntry("Visible checked");

            var result = UninstallerListViewUpdater.ReconcileCheckedObjects(
                Enumerable.Empty<ApplicationUninstallerEntry>(),
                new[] { app },
                new[] { app }).ToArray();

            CollectionAssert.AreEqual(new[] { app }, result);
        }

        [TestMethod]
        public void ReconcileCheckedObjects_VisibleUncheckWinsOverStaleMappedState()
        {
            var app = CreateEntry("Visible unchecked");

            var result = UninstallerListViewUpdater.ReconcileCheckedObjects(
                new[] { app },
                new[] { app },
                Enumerable.Empty<ApplicationUninstallerEntry>()).ToArray();

            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ReconcileCheckedObjects_PreservesMappedChecksHiddenByFiltering()
        {
            var hiddenApp = CreateEntry("Hidden checked");
            var visibleApp = CreateEntry("Visible unchecked");

            var result = UninstallerListViewUpdater.ReconcileCheckedObjects(
                new[] { hiddenApp },
                new[] { visibleApp },
                Enumerable.Empty<ApplicationUninstallerEntry>()).ToArray();

            CollectionAssert.AreEqual(new[] { hiddenApp }, result);
        }

        [TestMethod]
        public void ReconcileCheckedObjects_DoesNotDuplicateSyncedVisibleChecks()
        {
            var app = CreateEntry("Visible synced");

            var result = UninstallerListViewUpdater.ReconcileCheckedObjects(
                new[] { app },
                new[] { app },
                new[] { app }).ToArray();

            CollectionAssert.AreEqual(new[] { app }, result);
        }

        private static ApplicationUninstallerEntry CreateEntry(string name)
        {
            return new ApplicationUninstallerEntry { RawDisplayName = name };
        }
    }
}
