/*
    EBUninstaller Pro - Unit Test Suite
    Virtual Network Adapter & VPN TAP/TUN Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class VirtualAdapterAuditorTests
    {
        [TestMethod]
        public void TestScanVirtualAdaptersSafety()
        {
            var results = VirtualAdapterAuditorEngine.ScanVirtualAdapters();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestVirtualAdapterItemModel()
        {
            var item = new VirtualAdapterItem
            {
                Id = "{12345678-ABCD-1234-ABCD-123456789ABC}",
                Name = "Ethernet 2",
                Description = "TAP-Windows Adapter V9",
                OperationalStatus = "Down",
                InterfaceType = "Ethernet",
                IsVirtual = true,
                ProviderCategory = "OpenVPN TAP Adapter"
            };

            Assert.IsTrue(item.IsVirtual);
            Assert.AreEqual("OpenVPN TAP Adapter", item.ProviderCategory);
        }
    }
}
