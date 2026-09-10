/*
    EBUninstaller Pro - Unit Test Suite
    Diagnostic Data & Telemetry Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class DiagnosticDataSessionCleanerTests
    {
        [TestMethod]
        public void TestScanDiagnosticLogsSafety()
        {
            var logs = DiagnosticDataSessionCleanerEngine.ScanDiagnosticLogs();
            Assert.IsNotNull(logs);
        }

        [TestMethod]
        public void TestCleanDiagnosticLogsExecution()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "EBDiagTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var file1 = Path.Combine(tempDir, "test.etl");
                File.WriteAllText(file1, "dummy etl data");

                var items = new List<DiagnosticLogItem>
                {
                    new() { FilePath = file1, LogType = "ETL Trace", SizeBytes = 14 }
                };

                var (cleaned, freed) = DiagnosticDataSessionCleanerEngine.CleanDiagnosticLogs(items);
                Assert.AreEqual(1, cleaned);
                Assert.AreEqual(14, freed);
                Assert.IsFalse(File.Exists(file1));
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }
    }
}
