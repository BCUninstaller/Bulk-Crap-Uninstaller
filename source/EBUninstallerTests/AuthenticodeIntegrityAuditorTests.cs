/*
    EBUninstaller Pro - Unit Test Suite
    Authenticode Signature & Integrity Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class AuthenticodeIntegrityAuditorTests
    {
        [TestMethod]
        public void TestAuditDirectorySafety()
        {
            var results = AuthenticodeIntegrityAuditorEngine.AuditDirectory(Environment.GetFolderPath(Environment.SpecialFolder.System), 10);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestAuthenticodeAuditResultModel()
        {
            var res = new AuthenticodeAuditResult
            {
                FilePath = @"C:\Program Files\TestApp\app.exe",
                IsSigned = true,
                IsValid = true,
                SignerSubject = "Test Publisher LLC",
                Issuer = "DigiCert Trusted G4 Code Signing",
                ValidTo = "2027-12-31"
            };

            Assert.AreEqual("app.exe", res.FileName);
            Assert.IsTrue(res.IsSigned);
            Assert.IsTrue(res.IsValid);
            Assert.AreEqual("Valid Authenticode Signature", res.IntegrityStatus);
        }
    }
}
