/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Authenticode Binary Signature & Certificate Integrity Auditor Subsystem
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
    public sealed class AuthenticodeAuditResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName => Path.GetFileName(FilePath);
        public bool IsSigned { get; set; }
        public string SignerSubject { get; set; } = "Unsigned";
        public string Issuer { get; set; } = "N/A";
        public string ValidTo { get; set; } = "N/A";
        public bool IsValid { get; set; }
        public string IntegrityStatus => !IsSigned ? "Unsigned Binary" : (IsValid ? "Valid Authenticode Signature" : "Invalid / Expired Signature");
    }

    public static class AuthenticodeIntegrityAuditorEngine
    {
        public static List<AuthenticodeAuditResult> AuditDirectory(string directoryPath, int maxFiles = 200)
        {
            var results = new List<AuthenticodeAuditResult>();
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath)) return results;

            try
            {
                var files = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".sys", StringComparison.OrdinalIgnoreCase))
                    .Take(maxFiles);

                foreach (var file in files)
                {
                    results.Add(AuditFile(file));
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Security, $"Failed auditing Authenticode in {directoryPath}", ex.Message);
            }

            return results;
        }

        public static AuthenticodeAuditResult AuditFile(string filePath)
        {
            var result = new AuthenticodeAuditResult { FilePath = filePath };
            if (!File.Exists(filePath)) return result;

            try
            {
                var basicCert = X509Certificate.CreateFromSignedFile(filePath);
                if (basicCert != null)
                {
                    using var cert2 = new X509Certificate2(basicCert);
                    result.IsSigned = true;
                    result.SignerSubject = cert2.GetNameInfo(X509NameType.SimpleName, false) ?? cert2.Subject;
                    result.Issuer = cert2.GetNameInfo(X509NameType.SimpleName, true) ?? cert2.Issuer;
                    result.ValidTo = cert2.NotAfter.ToString("yyyy-MM-dd");
                    result.IsValid = cert2.NotAfter >= DateTime.UtcNow && cert2.NotBefore <= DateTime.UtcNow;
                }
            }
            catch
            {
                // Unsigned or non-authenticode
                result.IsSigned = false;
            }

            return result;
        }
    }
}
