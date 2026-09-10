/*
    EBUninstaller Pro - Unit Test Suite
    Firewall Port Matrix & Security Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class FirewallPortMatrixAuditorTests
    {
        [TestMethod]
        public void TestScanFirewallPortRulesSafety()
        {
            var rules = FirewallPortMatrixAuditorEngine.ScanFirewallPortRules();
            Assert.IsNotNull(rules);
        }

        [TestMethod]
        public void TestFirewallPortRuleItemModel()
        {
            var rule = new FirewallPortRuleItem
            {
                RuleName = "Node.js Server Inbound",
                Direction = "Inbound",
                Action = "Allow",
                Protocol = "TCP",
                LocalPort = "3000",
                Profile = "Public",
                ApplicationPath = @"C:\Program Files\nodejs\node.exe"
            };

            Assert.IsTrue(rule.IsPublicInboundAllow);
            Assert.AreEqual("Node.js Server Inbound", rule.RuleName);
            Assert.AreEqual("3000", rule.LocalPort);
        }
    }
}
