/*
    EBUninstaller Pro - Unit Test Suite
    Active Network Socket & Port Health Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class SocketHealthAuditorTests
    {
        [TestMethod]
        public void TestQueryActiveSocketsSafety()
        {
            var sockets = SocketHealthAuditorEngine.QueryActiveSockets();
            Assert.IsNotNull(sockets);
        }

        [TestMethod]
        public void TestActiveSocketInfoModel()
        {
            var sockPublic = new ActiveSocketInfo
            {
                Protocol = "TCP",
                LocalAddress = "0.0.0.0",
                LocalPort = 8080,
                KnownServiceName = "HTTP Alternate"
            };
            Assert.IsTrue(sockPublic.IsPubliclyExposed);
            Assert.IsTrue(sockPublic.SecurityAssessment.Contains("Public Listener"));

            var sockLocal = new ActiveSocketInfo
            {
                Protocol = "TCP",
                LocalAddress = "127.0.0.1",
                LocalPort = 5000,
                KnownServiceName = "Local Dev Service"
            };
            Assert.IsFalse(sockLocal.IsPubliclyExposed);
            Assert.IsTrue(sockLocal.SecurityAssessment.Contains("Loopback Only"));
        }
    }
}
