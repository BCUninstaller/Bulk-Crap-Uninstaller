/*
    EBUninstaller Pro - Unit Test Suite
    Process Handle & File Lock Resolver Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.FileSystemEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class ProcessHandleUnlockerTests
    {
        [TestMethod]
        public void TestFindLockingProcessesNullTarget()
        {
            var results = ProcessHandleUnlockerEngine.FindLockingProcesses(null);
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);

            var resultsEmpty = ProcessHandleUnlockerEngine.FindLockingProcesses("");
            Assert.IsNotNull(resultsEmpty);
            Assert.AreEqual(0, resultsEmpty.Count);
        }

        [TestMethod]
        public void TestLockingProcessInfoModel()
        {
            var info = new LockingProcessInfo
            {
                ProcessId = 1234,
                ProcessName = "chrome",
                ApplicationDescription = "Google Chrome",
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                LockedFilePath = @"C:\Program Files\Google\Chrome\Application\lock.dat"
            };

            Assert.AreEqual(1234, info.ProcessId);
            Assert.AreEqual("chrome", info.ProcessName);
            Assert.AreEqual("Google Chrome", info.ApplicationDescription);
        }
    }
}
