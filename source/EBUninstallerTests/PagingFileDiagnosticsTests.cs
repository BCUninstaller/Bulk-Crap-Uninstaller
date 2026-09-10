/*
    EBUninstaller Pro - Unit Test Suite
    Paging File Diagnostics & Security Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class PagingFileDiagnosticsTests
    {
        [TestMethod]
        public void TestQueryPagefileConfigSafety()
        {
            var config = PagingFileDiagnosticsEngine.QueryPagefileConfig();
            Assert.IsNotNull(config);
        }

        [TestMethod]
        public void TestPagefileConfigInfoModel()
        {
            var config = new PagefileConfigInfo
            {
                PagingFilesConfig = "C:\\pagefile.sys 4096 8192",
                ClearPagefileOnShutdown = true,
                LargeSystemCache = false,
                MainDrivePagefileSizeBytes = 4294967296,
                PagefileExistsOnDisk = true
            };

            Assert.IsTrue(config.ClearPagefileOnShutdown);
            Assert.IsTrue(config.PagefileExistsOnDisk);
            Assert.AreEqual(4294967296, config.MainDrivePagefileSizeBytes);
        }
    }
}
