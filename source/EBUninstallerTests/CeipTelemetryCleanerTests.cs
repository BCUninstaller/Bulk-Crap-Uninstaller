/*
    EBUninstaller Pro - Unit Test Suite
    CEIP & SQM Telemetry Cleaner Tests
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
    public class CeipTelemetryCleanerTests
    {
        [TestMethod]
        public void TestScanCeipTelemetrySafety()
        {
            var stats = CeipTelemetryCleanerEngine.ScanCeipTelemetry();
            Assert.IsNotNull(stats);
        }

        [TestMethod]
        public void TestCeipStoreStatsModel()
        {
            var stats = new CeipStoreStats
            {
                TotalSqmFiles = 120,
                TotalSizeBytes = 15728640
            };

            Assert.AreEqual(120, stats.TotalSqmFiles);
            Assert.AreEqual(15728640, stats.TotalSizeBytes);
        }
    }
}
