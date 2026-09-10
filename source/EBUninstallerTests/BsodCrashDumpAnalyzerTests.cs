/*
    EBUninstaller Pro - Unit Test Suite
    BSOD Crash Dump Analyzer Engine Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class BsodCrashDumpAnalyzerTests
    {
        [TestMethod]
        public void TestParseBugcheckEventLogMessage()
        {
            var logMsg = "The computer has rebooted from a bugcheck. A bugcheck was: 0x0000003B (0x00000000c0000005, 0xfffff80123456789, 0xffffd00123456789, 0x0000000000000000). A dump was saved in: C:\\Windows\\MEMORY.DMP. Faulting driver nvlddmkm.sys.";
            var record = BsodCrashDumpAnalyzerEngine.ParseBugcheckMessage(logMsg, DateTime.Now);

            Assert.IsNotNull(record);
            Assert.AreEqual("0x0000003B", record.BugcheckCode);
            Assert.AreEqual("SYSTEM_SERVICE_EXCEPTION", record.BugcheckName);
            Assert.AreEqual("nvlddmkm.sys", record.FaultingDriver);
            Assert.AreEqual("NVIDIA Graphics", record.AssociatedVendor);
            Assert.IsTrue(record.Recommendation.Contains("GPU"));
        }

        [TestMethod]
        public void TestScanCrashDumpsSafety()
        {
            var dumps = BsodCrashDumpAnalyzerEngine.ScanCrashDumps();
            Assert.IsNotNull(dumps);
        }
    }
}
