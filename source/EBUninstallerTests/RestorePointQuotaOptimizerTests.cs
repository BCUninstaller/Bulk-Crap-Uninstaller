/*
    EBUninstaller Pro - Unit Test Suite
    VSS Shadow Storage & Quota Optimizer Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class RestorePointQuotaOptimizerTests
    {
        [TestMethod]
        public void TestQueryShadowStorageSafety()
        {
            var results = RestorePointQuotaOptimizerEngine.QueryShadowStorage();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestShadowStorageInfoModel()
        {
            var info = new ShadowStorageInfo
            {
                ForVolume = @"C:\",
                UsedSpaceStr = "2.4 GB",
                AllocatedSpaceStr = "5.0 GB",
                MaxSpaceStr = "15.0 GB (10%)"
            };

            Assert.AreEqual(@"C:\", info.ForVolume);
            Assert.AreEqual("2.4 GB", info.UsedSpaceStr);
        }
    }
}
