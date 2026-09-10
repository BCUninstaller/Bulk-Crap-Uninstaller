/*
    EBUninstaller Pro - Unit Test Suite
    BITS Queue Residuals Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class BitsQueueResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanBitsJobsSafety()
        {
            var jobs = BitsQueueResidualsCleanerEngine.ScanBitsJobs();
            Assert.IsNotNull(jobs);
        }

        [TestMethod]
        public void TestBitsJobItemModel()
        {
            var item = new BitsJobItem
            {
                JobId = "{12345678-ABCD-1234-ABCD-123456789ABC}",
                DisplayName = "EdgeUpdate_BackgroundDownload",
                State = "SUSPENDED",
                Owner = "SYSTEM",
                TargetFile = "https://msedge.sf.dl.delivery.mp.microsoft.com/update.msi"
            };

            Assert.AreEqual("{12345678-ABCD-1234-ABCD-123456789ABC}", item.JobId);
            Assert.AreEqual("EdgeUpdate_BackgroundDownload", item.DisplayName);
            Assert.AreEqual("SUSPENDED", item.State);
        }
    }
}
