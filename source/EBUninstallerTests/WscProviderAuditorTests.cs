/*
    EBUninstaller Pro - Unit Test Suite
    Windows Security Center Providers Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SecurityHardening;

namespace EBUninstallerTests
{
    [TestClass]
    public class WscProviderAuditorTests
    {
        [TestMethod]
        public void TestScanSecurityProvidersSafety()
        {
            var providers = WscProviderAuditorEngine.ScanSecurityProviders();
            Assert.IsNotNull(providers);
        }

        [TestMethod]
        public void TestWscProviderItemModel()
        {
            var item = new WscProviderItem
            {
                InstanceGuid = "{12345678-ABCD-1234-ABCD-123456789ABC}",
                DisplayName = "Old Antivirus Suite",
                ProviderCategory = "AntiVirus",
                ExecutablePath = @"C:\Program Files\OldAV\avcenter.exe"
            };

            Assert.AreEqual("Old Antivirus Suite", item.DisplayName);
            Assert.AreEqual("AntiVirus", item.ProviderCategory);
        }
    }
}
