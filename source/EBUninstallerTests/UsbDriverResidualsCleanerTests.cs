/*
    EBUninstaller Pro - Unit Test Suite
    USB Device Driver Residuals Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class UsbDriverResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanUsbResidualsSafety()
        {
            var residuals = UsbDriverResidualsCleanerEngine.ScanUsbResiduals();
            Assert.IsNotNull(residuals);
        }

        [TestMethod]
        public void TestUsbDeviceResidualItemModel()
        {
            var item = new UsbDeviceResidualItem
            {
                HardwareId = @"VID_046D&PID_C52B\6&28C2E60F&0&1",
                DeviceDesc = "Logitech USB Receiver",
                Manufacturer = "Logitech",
                ServiceDriver = "HidUsb",
                IsNonPresent = true
            };

            Assert.AreEqual("Logitech USB Receiver", item.DeviceDesc);
            Assert.AreEqual("Logitech", item.Manufacturer);
            Assert.IsTrue(item.IsNonPresent);
        }
    }
}
