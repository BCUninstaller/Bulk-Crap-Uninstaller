/*
    EBUninstaller Pro - Unit Test Suite
    Software Power Impact & Wake Lock Engine Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwarePowerImpactTests
    {
        [TestMethod]
        public void TestParsePowerCfgOutput()
        {
            var sampleOutput = "DISPLAY:\nNone.\n\nSYSTEM:\n[PROCESS] \\Device\\HarddiskVolume3\\Program Files\\MediaApp\\player.exe\nPlaying audio stream\n\nAWAYMODE:\nNone.\n";
            var results = new List<SoftwarePowerRequestItem>();

            SoftwarePowerImpactEngine.ParsePowerCfgOutput(sampleOutput, results);

            Assert.AreEqual(1, results.Count);
            var req = results[0];
            Assert.AreEqual("SYSTEM", req.RequestType);
            Assert.AreEqual("PROCESS", req.CallerName);
            Assert.IsTrue(req.Description.Contains("player.exe"));
            Assert.IsTrue(req.PreventsSleep);
        }

        [TestMethod]
        public void TestSoftwarePowerProfileEfficiencyRating()
        {
            var profileA = new SoftwarePowerProfile
            {
                ApplicationName = "IdleService",
                ActiveWakeLocks = 0,
                CpuUsagePercent = 0.1
            };
            Assert.IsTrue(profileA.EfficiencyRating.StartsWith("A"));
            Assert.AreEqual("Optimal", profileA.EnergyStatus);

            var profileF = new SoftwarePowerProfile
            {
                ApplicationName = "HeavyDownloader",
                ActiveWakeLocks = 1,
                CpuUsagePercent = 2.0
            };
            Assert.IsTrue(profileF.EfficiencyRating.StartsWith("F"));
            Assert.AreEqual("High Drain", profileF.EnergyStatus);
        }
    }
}
