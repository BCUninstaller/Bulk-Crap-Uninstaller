/*
    EBUninstaller Pro - Unit Test Suite
    Windows Firewall Orphaned Rules Cleaner Tests
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
    public class FirewallOrphanCleanerTests
    {
        [TestMethod]
        public void TestParseFirewallRuleString()
        {
            var ruleStr = "v2.30|Action=Allow|Active=TRUE|Dir=In|Protocol=6|LPort=8080|App=C:\\NonExistentAppFolder_9999\\app.exe|Name=DemoRule|";
            var item = FirewallOrphanCleanerEngine.ParseFirewallRule("DemoRuleKey", ruleStr);

            Assert.IsNotNull(item);
            Assert.AreEqual("DemoRule", item.RuleName);
            Assert.AreEqual("In", item.Direction);
            Assert.AreEqual("Allow", item.Action);
            Assert.AreEqual("6", item.Protocol);
            Assert.AreEqual("8080", item.LocalPorts);
            Assert.IsTrue(item.IsOrphaned);
            Assert.AreEqual(@"C:\NonExistentAppFolder_9999\app.exe", item.TargetApplicationPath);
        }

        [TestMethod]
        public void TestParseInvalidFirewallRuleString()
        {
            var invalid = "v2.30|Action=Allow|Active=TRUE|Dir=In|";
            var item = FirewallOrphanCleanerEngine.ParseFirewallRule("InvalidKey", invalid);
            Assert.IsNull(item);
        }
    }
}
