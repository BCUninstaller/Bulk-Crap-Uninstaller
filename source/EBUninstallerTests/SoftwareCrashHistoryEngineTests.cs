/*
    EBUninstaller Pro - Unit Test Suite
    Software Crash History & Stability Engine Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwareCrashHistoryEngineTests
    {
        [TestMethod]
        public void TestParseApplicationErrorLog()
        {
            var logMessage = "Faulting application name: badapp.exe, version: 1.2.3.4, time stamp: 0x5f123456\r\n" +
                             "Faulting module name: ntdll.dll, version: 10.0.19041.1, time stamp: 0x5f654321\r\n" +
                             "Exception code: 0xc0000005\r\n" +
                             "Fault offset: 0x0000000000012345";

            var now = DateTime.Now;
            var record = SoftwareCrashHistoryEngine.ParseEventLogEntry(logMessage, now, "Crash");

            Assert.IsNotNull(record);
            Assert.AreEqual("badapp.exe", record.ApplicationName);
            Assert.AreEqual("1.2.3.4", record.ApplicationVersion);
            Assert.AreEqual("ntdll.dll", record.FaultingModule);
            Assert.AreEqual("0xc0000005", record.ExceptionCode);
            Assert.AreEqual("Crash", record.EventType);
        }

        [TestMethod]
        public void TestGenerateStabilitySummaries()
        {
            var records = new List<SoftwareCrashRecord>
            {
                new() { ApplicationName = "AppA.exe", FaultingModule = "kernel32.dll", EventType = "Crash", Timestamp = DateTime.Now },
                new() { ApplicationName = "AppA.exe", FaultingModule = "kernel32.dll", EventType = "Crash", Timestamp = DateTime.Now },
                new() { ApplicationName = "AppA.exe", FaultingModule = "user32.dll", EventType = "Hang", Timestamp = DateTime.Now },
                new() { ApplicationName = "AppB.exe", FaultingModule = "clr.dll", EventType = "Crash", Timestamp = DateTime.Now }
            };

            var summaries = SoftwareCrashHistoryEngine.GenerateStabilitySummaries(records);

            Assert.AreEqual(2, summaries.Count);

            var summaryA = summaries.FirstOrDefault(s => s.ApplicationName == "AppA.exe");
            Assert.IsNotNull(summaryA);
            Assert.AreEqual(2, summaryA.TotalCrashes);
            Assert.AreEqual(1, summaryA.TotalHangs);
            Assert.AreEqual("kernel32.dll", summaryA.PrimaryFaultingModule);
            // 100 - (2*5) - (1*2.5) = 87.5
            Assert.AreEqual(87.5, summaryA.StabilityScore);
            Assert.AreEqual("Moderate", summaryA.HealthStatus);

            var summaryB = summaries.FirstOrDefault(s => s.ApplicationName == "AppB.exe");
            Assert.IsNotNull(summaryB);
            Assert.AreEqual(1, summaryB.TotalCrashes);
            Assert.AreEqual(0, summaryB.TotalHangs);
            // 100 - (1*5) = 95.0
            Assert.AreEqual(95.0, summaryB.StabilityScore);
            Assert.AreEqual("Stable", summaryB.HealthStatus);
        }

        [TestMethod]
        public void TestEmptyOrNullRecordsHandling()
        {
            var empty = SoftwareCrashHistoryEngine.GenerateStabilitySummaries(null);
            Assert.IsNotNull(empty);
            Assert.AreEqual(0, empty.Count);

            var emptyLog = SoftwareCrashHistoryEngine.ParseEventLogEntry("", DateTime.Now);
            Assert.IsNull(emptyLog);
        }
    }
}
