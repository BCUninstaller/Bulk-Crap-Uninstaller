/*
    EBUninstaller Pro - Unit Test Suite
    Software Environment Variables Residuals Cleaner Tests
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
    public class EnvVarResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanOrphanedEnvironmentVariablesSafety()
        {
            var results = EnvVarResidualsCleanerEngine.ScanOrphanedEnvironmentVariables();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestOrphanedEnvVarItemModel()
        {
            var item = new OrphanedEnvVarItem
            {
                VariableName = "JAVA_HOME",
                VariableValue = @"C:\Program Files\Java\jdk-11.0.1",
                Scope = "System",
                MissingPath = @"C:\Program Files\Java\jdk-11.0.1",
                IsOrphaned = true
            };

            Assert.AreEqual("JAVA_HOME", item.VariableName);
            Assert.AreEqual("System", item.Scope);
            Assert.IsTrue(item.IsOrphaned);
        }
    }
}
