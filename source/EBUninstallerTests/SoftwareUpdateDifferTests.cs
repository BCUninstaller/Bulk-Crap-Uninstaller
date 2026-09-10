/*
    EBUninstaller Pro - Unit Test Suite
    Software Update Version Differ Engine Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwareUpdateDifferTests
    {
        [TestMethod]
        public void TestCompareDirectoriesDiff()
        {
            var tempBase = Path.Combine(Path.GetTempPath(), "EBDiffTest_Base_" + Guid.NewGuid().ToString("N"));
            var tempNew = Path.Combine(Path.GetTempPath(), "EBDiffTest_New_" + Guid.NewGuid().ToString("N"));

            try
            {
                Directory.CreateDirectory(tempBase);
                Directory.CreateDirectory(tempNew);

                // File 1: Unchanged
                File.WriteAllText(Path.Combine(tempBase, "app.exe"), "12345");
                File.WriteAllText(Path.Combine(tempNew, "app.exe"), "12345");

                // File 2: Modified
                File.WriteAllText(Path.Combine(tempBase, "config.json"), "version 1");
                File.WriteAllText(Path.Combine(tempNew, "config.json"), "version 2 with longer text");

                // File 3: Removed
                File.WriteAllText(Path.Combine(tempBase, "legacy.dll"), "old dll");

                // File 4: Added
                File.WriteAllText(Path.Combine(tempNew, "feature.dll"), "new feature dll");

                var diff = SoftwareUpdateDifferEngine.CompareDirectories(tempBase, tempNew, "TestApp", "1.0", "2.0");

                Assert.AreEqual(1, diff.AddedCount);
                Assert.AreEqual(1, diff.RemovedCount);
                Assert.AreEqual(1, diff.ModifiedCount);

                var addedItem = diff.Items.FirstOrDefault(i => i.RelativePath == "feature.dll");
                Assert.IsNotNull(addedItem);
                Assert.AreEqual(DiffItemChangeType.Added, addedItem.ChangeType);
                Assert.AreEqual("Dynamic Library (DLL)", addedItem.FileType);

                var removedItem = diff.Items.FirstOrDefault(i => i.RelativePath == "legacy.dll");
                Assert.IsNotNull(removedItem);
                Assert.AreEqual(DiffItemChangeType.Removed, removedItem.ChangeType);

                var modItem = diff.Items.FirstOrDefault(i => i.RelativePath == "config.json");
                Assert.IsNotNull(modItem);
                Assert.AreEqual(DiffItemChangeType.Modified, modItem.ChangeType);
                Assert.AreEqual("Configuration", modItem.FileType);
            }
            finally
            {
                if (Directory.Exists(tempBase)) Directory.Delete(tempBase, true);
                if (Directory.Exists(tempNew)) Directory.Delete(tempNew, true);
            }
        }
    }
}
