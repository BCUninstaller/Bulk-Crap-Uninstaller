/*
    EBUninstaller Pro - Unit Test Suite
    Startup Staggering & Delayed Execution Engine Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Startup;

namespace EBUninstallerTests
{
    [TestClass]
    public class StartupStaggerEngineTests
    {
        [TestMethod]
        public void TestStaggeredStartupItemModel()
        {
            var item = new StaggeredStartupItem
            {
                Name = "CloudSync",
                Command = "\"C:\\Program Files\\CloudSync\\sync.exe\" --minimized",
                DelaySeconds = 45,
                IsStaggered = true
            };

            Assert.AreEqual("CloudSync", item.Name);
            Assert.AreEqual("EBUninstaller_Staggered_CloudSync", item.TaskName);
            Assert.AreEqual(45, item.DelaySeconds);
            Assert.IsTrue(item.IsStaggered);
        }

        [TestMethod]
        public void TestGetStartupItemsSafeExecution()
        {
            var items = StartupStaggerEngine.GetStartupItems();
            Assert.IsNotNull(items);
        }
    }
}
