/*
    EBUninstaller Pro - Unit Test Suite
    Kernel LiveDump & Crash Cleaner Tests
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
    public class KernelLiveDumpCleanerTests
    {
        [TestMethod]
        public void TestScanCrashDumpsSafety()
        {
            var dumps = KernelLiveDumpCleanerEngine.ScanCrashDumps();
            Assert.IsNotNull(dumps);
        }

        [TestMethod]
        public void TestCrashDumpResidualItemModel()
        {
            var item = new CrashDumpResidualItem
            {
                FilePath = @"C:\Windows\Minidump\091026-1234-01.dmp",
                DumpType = "Kernel MiniDump (.dmp)",
                SizeBytes = 524288,
                CreationTime = DateTime.UtcNow
            };

            Assert.AreEqual("Kernel MiniDump (.dmp)", item.DumpType);
            Assert.AreEqual(524288, item.SizeBytes);
        }
    }
}
