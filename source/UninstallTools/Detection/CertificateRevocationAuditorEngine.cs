/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Code Signing Certificate & Revocation Auditor Engine
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum CertValidityStatus
    {
        Valid,
        Expired,
        SelfSigned,
        UntrustedRoot,
        Unsigned,
        RevokedOrInvalid
    }

    public sealed class SoftwareCertificateAuditResult
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string BinaryPath { get; set; } = string.Empty;
        public CertValidityStatus Status { get; set; } = CertValidityStatus.Unsigned;
        public string SubjectName { get; set; } = string.Empty;
        public string IssuerName { get; set; } = string.Empty;
        public string SignatureAlgorithm { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public bool IsWeakAlgorithm => SignatureAlgorithm.Contains("sha1", StringComparison.OrdinalIgnoreCase) ||
                                       SignatureAlgorithm.Contains("md5", StringComparison.OrdinalIgnoreCase);
        public string SecurityAssessment => Status switch
        {
            CertValidityStatus.Valid when !IsWeakAlgorithm => "Secure (Trusted & Valid Authenticode Signature)",
            CertValidityStatus.Valid when IsWeakAlgorithm => "Warning (Deprecated weak signature algorithm)",
            CertValidityStatus.Expired => "Expired (Digital signature is past validity expiration date)",
            CertValidityStatus.UntrustedRoot => "Untrusted (Issued by unknown or untrusted certificate authority)",
            CertValidityStatus.SelfSigned => "Self-Signed (Untrusted self-generated developer certificate)",
            CertValidityStatus.Unsigned => "Unsigned (No Authenticode digital signature embedded)",
            _ => "Unknown / Invalid Signature"
        };
    }

    public static class CertificateRevocationAuditorEngine
    {
        public static List<SoftwareCertificateAuditResult> AuditApplications(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            var results = new List<SoftwareCertificateAuditResult>();
            if (apps == null) return results;

            foreach (var app in apps)
            {
                if (string.IsNullOrWhiteSpace(app.DisplayName)) continue;

                var exePath = FindPrimaryExecutable(app);
                if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
                {
                    results.Add(new SoftwareCertificateAuditResult
                    {
                        ApplicationName = app.DisplayName,
                        BinaryPath = app.InstallLocation ?? string.Empty,
                        Status = CertValidityStatus.Unsigned,
                        SubjectName = app.Publisher ?? "Unknown Publisher"
                    });
                    continue;
                }

                results.Add(AuditBinaryCertificate(app.DisplayName, exePath));
            }

            return results;
        }

        public static SoftwareCertificateAuditResult AuditBinaryCertificate(string appName, string exePath)
        {
            var result = new SoftwareCertificateAuditResult
            {
                ApplicationName = appName,
                BinaryPath = exePath
            };

            try
            {
                var cert = X509Certificate.CreateFromSignedFile(exePath);
                var cert2 = new X509Certificate2(cert);

                result.SubjectName = cert2.GetNameInfo(X509NameType.SimpleName, false) ?? cert2.Subject;
                result.IssuerName = cert2.GetNameInfo(X509NameType.SimpleName, true) ?? cert2.Issuer;
                result.SignatureAlgorithm = cert2.SignatureAlgorithm.FriendlyName ?? cert2.SignatureAlgorithm.Value ?? "SHA-256";
                result.ExpirationDate = cert2.NotAfter;

                if (DateTime.Now > cert2.NotAfter)
                {
                    result.Status = CertValidityStatus.Expired;
                }
                else if (cert2.Subject.Equals(cert2.Issuer, StringComparison.OrdinalIgnoreCase))
                {
                    result.Status = CertValidityStatus.SelfSigned;
                }
                else
                {
                    result.Status = CertValidityStatus.Valid;
                }
            }
            catch
            {
                result.Status = CertValidityStatus.Unsigned;
                result.SubjectName = "Unsigned Binary";
            }

            return result;
        }

        private static string FindPrimaryExecutable(ApplicationUninstallerEntry app)
        {
            if (!string.IsNullOrEmpty(app.InstallLocation) && Directory.Exists(app.InstallLocation))
            {
                try
                {
                    var exeFiles = Directory.GetFiles(app.InstallLocation, "*.exe", SearchOption.TopDirectoryOnly);
                    if (exeFiles.Length > 0)
                    {
                        return exeFiles[0];
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
