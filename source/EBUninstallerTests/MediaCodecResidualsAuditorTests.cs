/*
    EBUninstaller Pro - Unit Test Suite
    Media Codec Residuals Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class MediaCodecResidualsAuditorTests
    {
        [TestMethod]
        public void TestScanMediaCodecsSafety()
        {
            var codecs = MediaCodecResidualsAuditorEngine.ScanMediaCodecs();
            Assert.IsNotNull(codecs);
        }

        [TestMethod]
        public void TestMediaCodecResidualItemModel()
        {
            var item = new MediaCodecResidualItem
            {
                CodecName = "LAV Video Decoder",
                ClsidGuid = "{EE30215D-164F-4A92-A4EB-9D4C13390F9F}",
                FrameworkType = "DirectShow Filter",
                ServerBinaryPath = @"C:\Program Files\LAV Filters\x64\LAVVideo.ax",
                IsBinaryMissing = false
            };

            Assert.AreEqual("LAV Video Decoder", item.CodecName);
            Assert.IsFalse(item.IsBinaryMissing);
            Assert.AreEqual("Valid Codec Registration", item.HealthStatus);
        }
    }
}
