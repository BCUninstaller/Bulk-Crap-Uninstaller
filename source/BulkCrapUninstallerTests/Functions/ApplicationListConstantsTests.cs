using BulkCrapUninstaller.Functions.ApplicationList;
using BulkCrapUninstaller.Properties;
using Klocman.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;

namespace BulkCrapUninstallerTests.Functions
{
    [TestClass]
    public class ApplicationListConstantsTests
    {
        private bool _advancedTestCertificates;

        [TestInitialize]
        public void TestInitialize()
        {
            _advancedTestCertificates = Settings.Default.AdvancedTestCertificates;
            Settings.Default.AdvancedTestCertificates = true;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Settings.Default.AdvancedTestCertificates = _advancedTestCertificates;
        }

        [TestMethod]
        public void GetApplicationCertificateText_ReturnsNoneWhenCertificateIsMissing()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationCertificateText(entry);

            Assert.AreEqual(Localisable.CertificateColumn_NotFound, result);
        }

        [TestMethod]
        public void GetApplicationCertificateText_ReturnsVerifiedCertificateStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };
            entry.SetCertificate(null, true);

            var result = ApplicationListConstants.GetApplicationCertificateText(entry);

            Assert.AreEqual(Localisable.CertificateColumn_Verified, result);
        }

        [TestMethod]
        public void GetApplicationCertificateText_ReturnsUnverifiedCertificateStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };
            entry.SetCertificate(null, false);

            var result = ApplicationListConstants.GetApplicationCertificateText(entry);

            Assert.AreEqual(Localisable.CertificateColumn_Unverified, result);
        }

        [TestMethod]
        public void GetApplicationCertificateText_ReturnsEmptyWhenCertificateChecksAreDisabled()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };
            Settings.Default.AdvancedTestCertificates = false;

            var result = ApplicationListConstants.GetApplicationCertificateText(entry);

            Assert.AreEqual(CommonStrings.Unknown, result);
        }

        [TestMethod]
        public void GetApplicationIntegrityText_ReturnsGoodWhenEntryIsRegisteredAndValid()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsRegistered = true,
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationIntegrityText(entry);

            Assert.AreEqual(UninstallTools.Properties.Localisation.Confidence_Good, result);
        }

        [TestMethod]
        public void GetApplicationIntegrityText_ReturnsMissingUninstallerWhenEntryIsInvalid()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsRegistered = true,
                IsValid = false
            };

            var result = ApplicationListConstants.GetApplicationIntegrityText(entry);

            Assert.AreEqual(Localisable.IntegrityColumn_Invalid, result);
        }

        [TestMethod]
        public void GetApplicationIntegrityText_ReturnsMissingRegistryWhenEntryIsUnregistered()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsRegistered = false,
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationIntegrityText(entry);

            Assert.AreEqual(Localisable.IntegrityColumn_Unregistered, result);
        }

        [TestMethod]
        public void GetApplicationIntegrityText_ReturnsMissingRegistryWhenEntryIsOrphaned()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsRegistered = true,
                IsOrphaned = true,
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationIntegrityText(entry);

            Assert.AreEqual(Localisable.IntegrityColumn_Unregistered, result);
        }

        [TestMethod]
        public void GetApplicationIntegrityText_ReturnsMissingRegistryAndUninstallerWhenBothAreMissing()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsRegistered = false,
                IsValid = false
            };

            var result = ApplicationListConstants.GetApplicationIntegrityText(entry);

            CollectionAssert.AreEqual(new[] { Localisable.IntegrityColumn_Invalid, Localisable.IntegrityColumn_Unregistered }, (object[])result);
        }
    }
}
