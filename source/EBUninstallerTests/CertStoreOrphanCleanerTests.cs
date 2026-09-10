/*
    EBUninstaller Pro - Unit Test Suite
    Certificate Store Orphan & Residuals Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SecurityHardening;

namespace EBUninstallerTests
{
    [TestClass]
    public class CertStoreOrphanCleanerTests
    {
        [TestMethod]
        public void TestScanResidualCertificatesSafety()
        {
            var certs = CertStoreOrphanCleanerEngine.ScanResidualCertificates();
            Assert.IsNotNull(certs);
        }

        [TestMethod]
        public void TestOrphanCertItemModel()
        {
            var item = new OrphanCertItem
            {
                Subject = "FiddlerRoot",
                Issuer = "FiddlerRoot",
                Thumbprint = "1234567890ABCDEF1234567890ABCDEF12345678",
                StoreLocationName = "CurrentUser",
                StoreNameLabel = "Root",
                NotAfter = DateTime.UtcNow.AddYears(-1)
            };

            Assert.AreEqual("FiddlerRoot", item.Subject);
            Assert.IsTrue(item.IsExpired);
            Assert.IsTrue(item.IsSelfSigned);
            Assert.AreEqual("Expired Certificate", item.RiskCategory);
        }
    }
}
