/*
    EBUninstaller Pro - Unit Test Suite
    DCOM Permissions & Orphaned Registration Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class DcomPermissionsOrphanCleanerTests
    {
        [TestMethod]
        public void TestScanOrphanedDcomSafety()
        {
            var results = DcomPermissionsOrphanCleanerEngine.ScanOrphanedDcomRegistrations();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestDcomOrphanItemModel()
        {
            var item = new DcomOrphanItem
            {
                AppId = "{12345678-ABCD-1234-ABCD-123456789ABC}",
                DisplayName = "OldMediaServer",
                LocalServer32Path = @"C:\Program Files\OldMedia\server.exe",
                IsOrphaned = true,
                IssueReason = "Binary not found"
            };

            Assert.AreEqual("{12345678-ABCD-1234-ABCD-123456789ABC}", item.AppId);
            Assert.AreEqual("OldMediaServer", item.DisplayName);
            Assert.IsTrue(item.IsOrphaned);
        }
    }
}
