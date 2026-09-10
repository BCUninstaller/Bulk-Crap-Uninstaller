/*
    EBUninstaller Pro - Unit Test Suite
    Software Certificate Revocation & Expiration Auditor Tests
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
    public class CertificateRevocationAuditorTests
    {
        [TestMethod]
        public void TestSoftwareCertificateAuditResultModel()
        {
            var result = new SoftwareCertificateAuditResult
            {
                ApplicationName = "SafeApp",
                BinaryPath = @"C:\Program Files\SafeApp\app.exe",
                Status = CertValidityStatus.Valid,
                SignatureAlgorithm = "sha256RSA",
                SubjectName = "Safe Corp",
                IssuerName = "DigiCert Trusted G4"
            };

            Assert.IsFalse(result.IsWeakAlgorithm);
            Assert.IsTrue(result.SecurityAssessment.Contains("Secure"));
        }

        [TestMethod]
        public void TestWeakAlgorithmDetection()
        {
            var weakResult = new SoftwareCertificateAuditResult
            {
                ApplicationName = "LegacyApp",
                BinaryPath = @"C:\Program Files\Legacy\old.exe",
                Status = CertValidityStatus.Valid,
                SignatureAlgorithm = "sha1RSA"
            };

            Assert.IsTrue(weakResult.IsWeakAlgorithm);
            Assert.IsTrue(weakResult.SecurityAssessment.Contains("Warning"));
        }

        [TestMethod]
        public void TestExpiredCertificateDetection()
        {
            var expired = new SoftwareCertificateAuditResult
            {
                ApplicationName = "ExpiredApp",
                BinaryPath = @"C:\Program Files\Expired\tool.exe",
                Status = CertValidityStatus.Expired,
                ExpirationDate = DateTime.Now.AddDays(-10)
            };

            Assert.AreEqual(CertValidityStatus.Expired, expired.Status);
            Assert.IsTrue(expired.SecurityAssessment.Contains("Expired"));
        }
    }
}
