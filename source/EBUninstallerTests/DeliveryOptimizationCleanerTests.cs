/*
    EBUninstaller Pro - Unit Test Suite
    Delivery Optimization & Package Cache Cleaner Tests
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
    public class DeliveryOptimizationCleanerTests
    {
        [TestMethod]
        public void TestScanPackageCachesSafety()
        {
            var results = DeliveryOptimizationCleanerEngine.ScanPackageCaches();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestPackageCacheGroupModel()
        {
            var group = new PackageCacheGroup
            {
                Category = "NuGet Global Package Cache",
                DirectoryPath = @"C:\Users\TestUser\.nuget\packages",
                FileCount = 1520,
                TotalSizeBytes = 536870912
            };

            Assert.AreEqual("NuGet Global Package Cache", group.Category);
            Assert.AreEqual(1520, group.FileCount);
            Assert.AreEqual(536870912, group.TotalSizeBytes);
        }
    }
}
