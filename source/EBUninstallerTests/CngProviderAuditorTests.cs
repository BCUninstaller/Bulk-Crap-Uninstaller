/*
    EBUninstaller Pro - Unit Test Suite
    CNG & CryptoAPI Provider Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SecurityHardening;

namespace EBUninstallerTests
{
    [TestClass]
    public class CngProviderAuditorTests
    {
        [TestMethod]
        public void TestScanCryptoProvidersSafety()
        {
            var providers = CngProviderAuditorEngine.ScanCryptoProviders();
            Assert.IsNotNull(providers);
        }

        [TestMethod]
        public void TestCryptoProviderItemModel()
        {
            var item = new CryptoProviderItem
            {
                ProviderName = "Microsoft Enhanced Cryptographic Provider v1.0",
                ProviderType = "CryptoAPI (CAPI)",
                ImagePath = "rsaenh.dll",
                IsBinaryMissing = false
            };

            Assert.AreEqual("Microsoft Enhanced Cryptographic Provider v1.0", item.ProviderName);
            Assert.AreEqual("CryptoAPI (CAPI)", item.ProviderType);
            Assert.IsFalse(item.IsBinaryMissing);
        }
    }
}
