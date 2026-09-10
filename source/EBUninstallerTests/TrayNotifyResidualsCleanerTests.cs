/*
    EBUninstaller Pro - Unit Test Suite
    System Tray Notification Area Cache Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class TrayNotifyResidualsCleanerTests
    {
        [TestMethod]
        public void TestQueryTrayNotifyStatusSafety()
        {
            var status = TrayNotifyResidualsCleanerEngine.QueryTrayNotifyStatus();
            Assert.IsNotNull(status);
        }

        [TestMethod]
        public void TestTrayNotifyStatusModel()
        {
            var status = new TrayNotifyStatus
            {
                HasIconStreams = true,
                HasPastIconsStream = true,
                TotalStreamSizeBytes = 40960
            };

            Assert.IsTrue(status.HasIconStreams);
            Assert.IsTrue(status.HasPastIconsStream);
            Assert.AreEqual(40960, status.TotalStreamSizeBytes);
        }
    }
}
