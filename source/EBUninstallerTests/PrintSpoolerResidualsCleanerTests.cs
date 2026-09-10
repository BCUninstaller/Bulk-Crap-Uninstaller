/*
    EBUninstaller Pro - Unit Test Suite
    Print Spooler Residuals Cleaner Tests
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
    public class PrintSpoolerResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanSpoolerResidualsSafety()
        {
            var residuals = PrintSpoolerResidualsCleanerEngine.ScanSpoolerResiduals();
            Assert.IsNotNull(residuals);
        }

        [TestMethod]
        public void TestSpoolerResidualItemModel()
        {
            var item = new SpoolerResidualItem
            {
                FilePath = @"C:\Windows\System32\spool\PRINTERS\FP00001.SPL",
                ItemType = "Print Data Job (.SPL)",
                SizeBytes = 2048576,
                CreationTime = DateTime.Now
            };

            Assert.AreEqual("Print Data Job (.SPL)", item.ItemType);
            Assert.AreEqual(2048576, item.SizeBytes);
        }
    }
}
