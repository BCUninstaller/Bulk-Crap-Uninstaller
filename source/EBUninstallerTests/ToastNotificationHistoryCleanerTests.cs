/*
    EBUninstaller Pro - Unit Test Suite
    Toast Notification History Cleaner Tests
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
    public class ToastNotificationHistoryCleanerTests
    {
        [TestMethod]
        public void TestQueryNotificationStoreSafety()
        {
            var stats = ToastNotificationHistoryCleanerEngine.QueryNotificationStore();
            Assert.IsNotNull(stats);
        }

        [TestMethod]
        public void TestNotificationStoreStatsModel()
        {
            var stats = new NotificationStoreStats
            {
                DatabasePath = @"C:\Users\TestUser\AppData\Local\Microsoft\Windows\Notifications\wpndatabase.db",
                DatabaseSizeBytes = 2097152,
                CachedLogoCount = 45,
                CachedLogosSizeBytes = 524288,
                Exists = true
            };

            Assert.IsTrue(stats.Exists);
            Assert.AreEqual(2097152, stats.DatabaseSizeBytes);
            Assert.AreEqual(45, stats.CachedLogoCount);
        }
    }
}
