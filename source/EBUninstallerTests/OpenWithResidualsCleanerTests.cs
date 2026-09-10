/*
    EBUninstaller Pro - Unit Test Suite
    Shell OpenWith & File Association Orphan Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class OpenWithResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanOpenWithOrphansSafety()
        {
            var results = OpenWithResidualsCleanerEngine.ScanOpenWithOrphans();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestOpenWithOrphanItemModel()
        {
            var item = new OpenWithOrphanItem
            {
                ApplicationExe = "photoviewer.exe",
                MissingPath = @"C:\Program Files\OldViewer\photoviewer.exe",
                RegistryLocation = @"HKEY_CLASSES_ROOT\Applications\photoviewer.exe",
                IsOrphaned = true
            };

            Assert.AreEqual("photoviewer.exe", item.ApplicationExe);
            Assert.IsTrue(item.IsOrphaned);
        }
    }
}
