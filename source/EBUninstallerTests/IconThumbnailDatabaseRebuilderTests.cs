/*
    EBUninstaller Pro - Unit Test Suite
    Shell Icon & Thumbnail Cache Rebuilder Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class IconThumbnailDatabaseRebuilderTests
    {
        [TestMethod]
        public void TestScanCacheDatabasesSafety()
        {
            var caches = IconThumbnailDatabaseRebuilderEngine.ScanCacheDatabases();
            Assert.IsNotNull(caches);
        }

        [TestMethod]
        public void TestShellCacheDatabaseItemModel()
        {
            var item = new ShellCacheDatabaseItem
            {
                FileName = "thumbcache_256.db",
                FullPath = @"C:\Users\User\AppData\Local\Microsoft\Windows\Explorer\thumbcache_256.db",
                SizeBytes = 10485760,
                CacheType = "Thumbnail Database"
            };

            Assert.AreEqual("thumbcache_256.db", item.FileName);
            Assert.AreEqual(10485760, item.SizeBytes);
            Assert.AreEqual("Thumbnail Database", item.CacheType);
        }
    }
}
