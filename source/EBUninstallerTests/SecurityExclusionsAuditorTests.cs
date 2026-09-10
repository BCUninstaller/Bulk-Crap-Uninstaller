/*
    EBUninstaller Pro - Unit Test Suite
    Windows Defender & Security Exclusions Auditor Tests
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
    public class SecurityExclusionsAuditorTests
    {
        [TestMethod]
        public void TestAnalyzeDangerousPathExclusions()
        {
            var rootExclusion = SecurityExclusionsAuditorEngine.AnalyzePathExclusion(@"C:\", "Paths");
            Assert.AreEqual(ExclusionRiskLevel.Critical, rootExclusion.RiskLevel);
            Assert.IsTrue(rootExclusion.Reason.Contains("broad root/system exclusion"));

            var tempExclusion = SecurityExclusionsAuditorEngine.AnalyzePathExclusion(@"%TEMP%", "Paths");
            Assert.AreEqual(ExclusionRiskLevel.Critical, tempExclusion.RiskLevel);
        }

        [TestMethod]
        public void TestAnalyzeDangerousProcessExclusions()
        {
            var cmdExclusion = SecurityExclusionsAuditorEngine.AnalyzeProcessExclusion(@"C:\Windows\System32\cmd.exe", "Processes");
            Assert.AreEqual(ExclusionRiskLevel.Critical, cmdExclusion.RiskLevel);
            Assert.IsTrue(cmdExclusion.Reason.Contains("interpreter/utility"));

            var pwshExclusion = SecurityExclusionsAuditorEngine.AnalyzeProcessExclusion(@"powershell.exe", "Processes");
            Assert.AreEqual(ExclusionRiskLevel.Critical, pwshExclusion.RiskLevel);

            var safeApp = SecurityExclusionsAuditorEngine.AnalyzeProcessExclusion(@"C:\Program Files\MyPhotoApp\editor.exe", "Processes");
            Assert.AreEqual(ExclusionRiskLevel.Low, safeApp.RiskLevel);
        }

        [TestMethod]
        public void TestAnalyzeExtensionExclusions()
        {
            var exeExt = SecurityExclusionsAuditorEngine.AnalyzeExtensionExclusion(".exe", "Extensions");
            Assert.AreEqual(ExclusionRiskLevel.High, exeExt.RiskLevel);

            var dllExt = SecurityExclusionsAuditorEngine.AnalyzeExtensionExclusion("dll", "Extensions");
            Assert.AreEqual(ExclusionRiskLevel.High, dllExt.RiskLevel);

            var txtExt = SecurityExclusionsAuditorEngine.AnalyzeExtensionExclusion(".txt", "Extensions");
            Assert.AreEqual(ExclusionRiskLevel.Low, txtExt.RiskLevel);
        }

        [TestMethod]
        public void TestOrphanedPathExclusion()
        {
            var nonExistent = @"C:\Program Files\CompletelyUninstalledSoftware_12345\app.exe";
            var result = SecurityExclusionsAuditorEngine.AnalyzePathExclusion(nonExistent, "Paths");

            Assert.IsTrue(result.IsOrphaned);
            Assert.AreEqual(ExclusionRiskLevel.Medium, result.RiskLevel);
            Assert.IsTrue(result.Reason.Contains("does not exist on disk"));
        }
    }
}
