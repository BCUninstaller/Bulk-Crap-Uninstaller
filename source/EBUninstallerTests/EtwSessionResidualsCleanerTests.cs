/*
    EBUninstaller Pro - Unit Test Suite
    ETW AutoLogger Trace Sessions Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class EtwSessionResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanAutoLoggersSafety()
        {
            var loggers = EtwSessionResidualsCleanerEngine.ScanAutoLoggers();
            Assert.IsNotNull(loggers);
        }

        [TestMethod]
        public void TestEtwSessionResidualItemModel()
        {
            var item = new EtwSessionResidualItem
            {
                SessionName = "NvidiaTelemetryLogger",
                LogFileName = @"C:\ProgramData\Nvidia\telemetry.etl",
                LogSizeBytes = 10485760,
                IsThirdParty = true
            };

            Assert.AreEqual("NvidiaTelemetryLogger", item.SessionName);
            Assert.IsTrue(item.IsThirdParty);
            Assert.AreEqual(10485760, item.LogSizeBytes);
        }
    }
}
