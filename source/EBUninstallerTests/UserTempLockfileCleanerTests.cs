/*
    EBUninstaller Pro - Unit Test Suite
    User Temp Lockfiles Cleaner Tests
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
    public class UserTempLockfileCleanerTests
    {
        [TestMethod]
        public void TestScanStaleLockfilesSafety()
        {
            var locks = UserTempLockfileCleanerEngine.ScanStaleLockfiles();
            Assert.IsNotNull(locks);
        }

        [TestMethod]
        public void TestStaleLockfileItemModel()
        {
            var item = new StaleLockfileItem
            {
                FilePath = @"C:\Users\TestUser\AppData\Local\Temp\vscode-update.lock",
                LockType = "Application Lockfile",
                SizeBytes = 0,
                LastModified = DateTime.UtcNow
            };

            Assert.AreEqual("vscode-update.lock", item.FileName);
            Assert.AreEqual("Application Lockfile", item.LockType);
        }
    }
}
