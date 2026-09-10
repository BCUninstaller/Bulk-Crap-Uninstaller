/*
    EBUninstaller Pro - Unit Test Suite
    Winsock NameSpace & LSP Residuals Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WinsockNamespaceResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanWinsockProvidersSafety()
        {
            var providers = WinsockNamespaceResidualsCleanerEngine.ScanWinsockProviders();
            Assert.IsNotNull(providers);
        }

        [TestMethod]
        public void TestWinsockProviderItemModel()
        {
            var item = new WinsockProviderItem
            {
                ProviderName = "Tcpip",
                ProviderId = "{6642243A-3BA8-4AA6-BAA5-CA71791C66E4}",
                LibraryPath = "%SystemRoot%\\System32\\mswsock.dll",
                IsLibraryMissing = false
            };

            Assert.AreEqual("Tcpip", item.ProviderName);
            Assert.IsFalse(item.IsLibraryMissing);
            Assert.AreEqual("Valid Winsock Provider", item.HealthStatus);
        }
    }
}
