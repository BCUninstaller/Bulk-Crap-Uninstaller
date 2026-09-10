/*
    EBUninstaller Pro - Unit Test Suite
    UWP / AppContainer Isolation Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.StoreApps;

namespace EBUninstallerTests
{
    [TestClass]
    public class AppContainerIsolationAuditorTests
    {
        [TestMethod]
        public void TestAuditManifestWithFullTrust()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "AppxTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var manifestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Package xmlns=""http://schemas.microsoft.com/appx/manifest/foundation/windows10""
         xmlns:rescap=""http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"">
  <Identity Name=""TestAppContainer"" Publisher=""CN=Test"" Version=""1.0.0.0"" />
  <Properties>
    <DisplayName>Sample FullTrust Store App</DisplayName>
  </Properties>
  <Capabilities>
    <Capability Name=""internetClient"" />
    <rescap:Capability Name=""runFullTrust"" />
    <rescap:Capability Name=""broadFileSystemAccess"" />
  </Capabilities>
</Package>";

                var manifestPath = Path.Combine(tempDir, "AppxManifest.xml");
                File.WriteAllText(manifestPath, manifestXml);

                var audit = AppContainerIsolationAuditorEngine.AuditManifest(manifestPath);

                Assert.IsNotNull(audit);
                Assert.AreEqual("TestAppContainer", audit.PackageName);
                Assert.AreEqual("Sample FullTrust Store App", audit.DisplayName);
                Assert.IsTrue(audit.HasFullTrust);
                Assert.IsTrue(audit.HasBroadFileSystemAccess);
                Assert.AreEqual(CapabilityRiskLevel.HighRisk, audit.OverallRisk);
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }
    }
}
