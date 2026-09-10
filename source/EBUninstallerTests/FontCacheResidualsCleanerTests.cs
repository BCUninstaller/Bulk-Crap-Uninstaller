/*
    EBUninstaller Pro - Unit Test Suite
    Font Cache Residuals Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class FontCacheResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanStaleFontsSafety()
        {
            var fonts = FontCacheResidualsCleanerEngine.ScanStaleFonts();
            Assert.IsNotNull(fonts);
        }

        [TestMethod]
        public void TestStaleFontItemModel()
        {
            var item = new StaleFontItem
            {
                FontName = "Obsolete Font Bold (TrueType)",
                FontFileName = "obsolete_bold.ttf",
                IssueReason = "Font file missing"
            };

            Assert.AreEqual("Obsolete Font Bold (TrueType)", item.FontName);
            Assert.AreEqual("obsolete_bold.ttf", item.FontFileName);
        }
    }
}
