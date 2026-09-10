/*
    EBUninstaller Pro - Unit Test Suite
    Scheduled Tasks Orphan Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Startup;

namespace EBUninstallerTests
{
    [TestClass]
    public class ScheduledTaskOrphanCleanerTests
    {
        [TestMethod]
        public void TestScanOrphanedTasksSafety()
        {
            var results = ScheduledTaskOrphanCleanerEngine.ScanOrphanedTasks();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestOrphanScheduledTaskItemModel()
        {
            var item = new OrphanScheduledTaskItem
            {
                TaskName = "AdobeGCInvoker-1.0",
                TaskPath = @"C:\Windows\System32\Tasks\AdobeGCInvoker-1.0",
                TargetBinary = @"C:\Program Files (x86)\Common Files\Adobe\AdobeGCClient\AdobeGCClient.exe",
                IssueReason = "Binary not found",
                IsOrphaned = true
            };

            Assert.AreEqual("AdobeGCInvoker-1.0", item.TaskName);
            Assert.IsTrue(item.IsOrphaned);
        }
    }
}
