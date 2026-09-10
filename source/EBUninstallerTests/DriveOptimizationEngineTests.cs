/*
    EBUninstaller Pro - Unit Test Suite
    Storage Drive TRIM & Media Optimization Tests
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
    public class DriveOptimizationEngineTests
    {
        [TestMethod]
        public void TestGetDrivesEnumeration()
        {
            var drives = DriveOptimizationEngine.GetDrives();
            Assert.IsNotNull(drives);

            // If running on a system with at least one drive
            if (drives.Count > 0)
            {
                var first = drives[0];
                Assert.IsFalse(string.IsNullOrEmpty(first.DriveLetter));
                Assert.IsTrue(first.TotalSizeBytes > 0);
                Assert.IsNotNull(first.RecommendedAction);
            }
        }

        [TestMethod]
        public void TestDriveRecommendedAction()
        {
            var ssdDrive = new StorageDriveInfo
            {
                DriveLetter = "C:",
                MediaType = DriveMediaType.SSD
            };
            Assert.IsTrue(ssdDrive.RecommendedAction.Contains("TRIM"));

            var hddDrive = new StorageDriveInfo
            {
                DriveLetter = "D:",
                MediaType = DriveMediaType.HDD
            };
            Assert.IsTrue(hddDrive.RecommendedAction.Contains("Defragmentation"));
        }
    }
}
