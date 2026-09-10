/*
    EBUninstaller Pro - Unit Test Suite
    DirectX & GPU Shader Cache Cleaner Tests
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
    public class DirectXShaderCacheCleanerTests
    {
        [TestMethod]
        public void TestScanShaderCachesSafety()
        {
            var stores = DirectXShaderCacheCleanerEngine.ScanShaderCaches();
            Assert.IsNotNull(stores);
        }

        [TestMethod]
        public void TestShaderCacheStoreItemModel()
        {
            var item = new ShaderCacheStoreItem
            {
                Architecture = "NVIDIA DXCache",
                DirectoryPath = @"C:\Users\TestUser\AppData\Local\NVIDIA\DXCache",
                FileCount = 420,
                TotalSizeBytes = 524288000
            };

            Assert.AreEqual("NVIDIA DXCache", item.Architecture);
            Assert.AreEqual(420, item.FileCount);
            Assert.AreEqual(524288000, item.TotalSizeBytes);
        }
    }
}
