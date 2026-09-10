/*
    EBUninstaller Pro - Unit Test Suite
    Environment PATH Auditor & Optimizer Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class PathEnvironmentAuditorTests
    {
        [TestMethod]
        public void TestAuditPathStringDuplicatesAndEmpty()
        {
            var testPath = "C:\\Windows\\System32;;C:\\Program Files\\NodeJS;C:\\Windows\\System32;C:\\NonExistentDirectory_12345\\";
            var issues = new List<PathAuditItem>();

            PathEnvironmentAuditorEngine.AuditPathString(testPath, "System", issues);

            Assert.IsTrue(issues.Any(i => i.IssueType == PathIssueType.EmptyEntry), "Should detect empty semicolon entry.");
            Assert.IsTrue(issues.Any(i => i.IssueType == PathIssueType.DuplicateEntry), "Should detect duplicate System32 entry.");
            Assert.IsTrue(issues.Any(i => i.IssueType == PathIssueType.MissingDirectory), "Should detect missing test directory.");
            Assert.IsTrue(issues.Any(i => i.IssueType == PathIssueType.TrailingBackslash), "Should detect trailing backslash.");
        }

        [TestMethod]
        public void TestCleanAndOptimizePath()
        {
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var sys32 = Path.Combine(winDir, "System32");

            var testPath = $"{sys32};;{sys32}\\;{winDir};C:\\TotallyFakeDirectory_99999\\";
            var cleaned = PathEnvironmentAuditorEngine.CleanAndOptimizePath(testPath, removeNonExistent: true);

            Assert.IsFalse(cleaned.Contains(";;"), "Cleaned PATH should not contain double semicolons.");
            Assert.IsFalse(cleaned.Contains("TotallyFakeDirectory_99999"), "Non-existent directory should be removed.");

            var segments = cleaned.Split(';');
            Assert.AreEqual(segments.Distinct(StringComparer.OrdinalIgnoreCase).Count(), segments.Length, "Cleaned PATH should have no duplicates.");
        }
    }
}
