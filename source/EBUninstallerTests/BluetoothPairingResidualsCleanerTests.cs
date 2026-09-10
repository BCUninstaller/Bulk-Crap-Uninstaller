/*
    EBUninstaller Pro - Unit Test Suite
    Bluetooth Device Pairing Residuals Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class BluetoothPairingResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanBluetoothResidualsSafety()
        {
            var results = BluetoothPairingResidualsCleanerEngine.ScanBluetoothResiduals();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestBluetoothDeviceResidualItemModel()
        {
            var item = new BluetoothDeviceResidualItem
            {
                MacAddress = "00:1A:7D:DA:71:13",
                DeviceName = "Sony WH-1000XM4",
                DeviceClass = "Audio / Headset / Speaker",
                IsPaired = true
            };

            Assert.AreEqual("Sony WH-1000XM4", item.DeviceName);
            Assert.AreEqual("00:1A:7D:DA:71:13", item.MacAddress);
            Assert.IsTrue(item.IsPaired);
        }
    }
}
