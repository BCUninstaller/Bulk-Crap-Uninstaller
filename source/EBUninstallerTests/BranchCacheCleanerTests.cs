/*
    EBUninstaller Pro - Unit Test Suite
    BranchCache Cleaner Tests
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
    public class BranchCacheCleanerTests
    {
        [TestMethod]
        public void TestScanBranchCacheSafety()
        {
            var stats = BranchCacheCleanerEngine.ScanBranchCache();
            Assert.IsNotNull(stats);
        }

        [TestMethod]
        public void TestBranchCacheStatsModel()
        {
            var stats = new BranchCacheStats
            {
                FileCount = 350,
                TotalSizeBytes = 536870912
            };

            Assert.AreEqual(350, stats.FileCount);
            Assert.AreEqual(536870912, stats.TotalSizeBytes);
        }
    }
}
