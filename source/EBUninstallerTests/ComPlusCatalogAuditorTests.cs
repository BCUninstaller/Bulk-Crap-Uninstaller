/*
    EBUninstaller Pro - Unit Test Suite
    COM+ Applications Catalog Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class ComPlusCatalogAuditorTests
    {
        [TestMethod]
        public void TestScanComPlusAppsSafety()
        {
            var apps = ComPlusCatalogAuditorEngine.ScanComPlusApps();
            Assert.IsNotNull(apps);
        }

        [TestMethod]
        public void TestComPlusAppItemModel()
        {
            var app = new ComPlusAppItem
            {
                AppId = "{12345678-ABCD-1234-ABCD-123456789ABC}",
                ApplicationName = "Enterprise ERP Data Layer",
                Identity = "NT AUTHORITY\\NetworkService",
                IsSystemApp = false
            };

            Assert.AreEqual("Enterprise ERP Data Layer", app.ApplicationName);
            Assert.IsFalse(app.IsSystemApp);
            Assert.AreEqual("NT AUTHORITY\\NetworkService", app.Identity);
        }
    }
}
