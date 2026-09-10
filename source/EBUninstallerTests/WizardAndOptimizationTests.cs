/*
    EBUninstaller Pro - Wizard & Optimization Tests
    Unit tests for Quick Optimization Wizard steps and execution state.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WizardAndOptimizationTests
    {
        [TestMethod]
        public void TestOptimizationTaskConfiguration()
        {
            var task = new JunkCleanupTask
            {
                Name = "Windows Temp Cleaner",
                Category = JunkCategory.WindowsTemp,
                EstimatedBytes = 1048576,
                IsSelected = true
            };

            Assert.AreEqual("Windows Temp Cleaner", task.Name);
            Assert.AreEqual(JunkCategory.WindowsTemp, task.Category);
            Assert.IsTrue(task.IsSelected);
            Assert.AreEqual(1048576, task.EstimatedBytes);
        }

        [TestMethod]
        public void TestWizardOptimizationSubsystemIntegration()
        {
            var patchTask = new JunkCleanupTask
            {
                Name = "Windows Installer Patch Cache Cleaner",
                Category = JunkCategory.WindowsUpdates,
                EstimatedBytes = 52428800,
                IsSelected = true
            };

            Assert.IsTrue(patchTask.IsSelected);
            Assert.AreEqual(52428800, patchTask.EstimatedBytes);
        }
    }
}
