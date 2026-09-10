/*
    EBUninstaller Pro - Unit Test Suite
    Service Conflict & Port Collision Detector Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class ServiceConflictDetectorTests
    {
        [TestMethod]
        public void TestExtractCleanExecutablePath()
        {
            var quoted = "\"C:\\Program Files\\Apache Group\\Apache2\\bin\\httpd.exe\" -k runservice";
            var cleanQuoted = ServiceConflictDetectorEngine.ExtractCleanExecutablePath(quoted);
            Assert.AreEqual("C:\\Program Files\\Apache Group\\Apache2\\bin\\httpd.exe", cleanQuoted);

            var unquoted = "C:\\Windows\\System32\\myservice.exe /run";
            var cleanUnquoted = ServiceConflictDetectorEngine.ExtractCleanExecutablePath(unquoted);
            Assert.AreEqual("C:\\Windows\\System32\\myservice.exe", cleanUnquoted);
        }

        [TestMethod]
        public void TestDetectPortCollisions()
        {
            var sampleServices = new List<ServiceSnapshotInfo>
            {
                new()
                {
                    ServiceName = "W3SVC",
                    DisplayName = "World Wide Web Publishing Service",
                    BinaryPath = "C:\\Windows\\System32\\svchost.exe -k iissvcs",
                    CleanBinaryPath = "C:\\Windows\\System32\\svchost.exe",
                    StartType = ServiceStartMode.Automatic
                },
                new()
                {
                    ServiceName = "Apache2.4",
                    DisplayName = "Apache HTTP Server",
                    BinaryPath = "C:\\Apache24\\bin\\httpd.exe",
                    CleanBinaryPath = "C:\\Apache24\\bin\\httpd.exe",
                    StartType = ServiceStartMode.Automatic
                }
            };

            var conflicts = ServiceConflictDetectorEngine.AnalyzeConflicts(sampleServices);

            var portConflict = conflicts.FirstOrDefault(c => c.ConflictType == "PortCollision" && c.PortNumber == 80);
            Assert.IsNotNull(portConflict, "Should detect port 80 collision between IIS and Apache.");
            Assert.AreEqual(ServiceConflictSeverity.Critical, portConflict.Severity);
        }

        [TestMethod]
        public void TestDetectSharedBinaryRegistration()
        {
            var sampleServices = new List<ServiceSnapshotInfo>
            {
                new()
                {
                    ServiceName = "AgentService1",
                    DisplayName = "Monitoring Agent Alpha",
                    BinaryPath = "C:\\Agent\\agent.exe -a",
                    CleanBinaryPath = "C:\\Agent\\agent.exe",
                    StartType = ServiceStartMode.Automatic
                },
                new()
                {
                    ServiceName = "AgentService2",
                    DisplayName = "Monitoring Agent Beta",
                    BinaryPath = "C:\\Agent\\agent.exe -b",
                    CleanBinaryPath = "C:\\Agent\\agent.exe",
                    StartType = ServiceStartMode.Manual
                }
            };

            var conflicts = ServiceConflictDetectorEngine.AnalyzeConflicts(sampleServices);

            var sharedConflict = conflicts.FirstOrDefault(c => c.ConflictType == "SharedBinaryRegistration");
            Assert.IsNotNull(sharedConflict, "Should detect shared binary between AgentService1 and AgentService2.");
        }
    }
}
