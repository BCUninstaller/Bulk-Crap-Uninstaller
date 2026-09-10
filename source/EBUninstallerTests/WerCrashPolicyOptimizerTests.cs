/*
    EBUninstaller Pro - Unit Test Suite
    WER Crash Policy Optimizer Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WerCrashPolicyOptimizerTests
    {
        [TestMethod]
        public void TestQueryCurrentPolicySafety()
        {
            var policy = WerCrashPolicyOptimizerEngine.QueryCurrentPolicy();
            Assert.IsNotNull(policy);
        }

        [TestMethod]
        public void TestWerPolicySettingsModel()
        {
            var policy = new WerPolicySettings
            {
                DumpCount = 3,
                DumpType = 1,
                DontSendAdditionalData = true,
                IsWerDisabled = false
            };

            Assert.AreEqual(3, policy.DumpCount);
            Assert.AreEqual(1, policy.DumpType);
            Assert.IsTrue(policy.DontSendAdditionalData);
        }
    }
}
