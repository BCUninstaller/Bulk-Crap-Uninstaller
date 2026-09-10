/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Certificate Store Orphan & Residuals Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UninstallTools.Core;

namespace UninstallTools.SecurityHardening
{
    public sealed class OrphanCertItem
    {
        public string Subject { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Thumbprint { get; set; } = string.Empty;
        public string StoreLocationName { get; set; } = "CurrentUser";
        public string StoreNameLabel { get; set; } = "My";
        public DateTime NotAfter { get; set; }
        public bool IsExpired => NotAfter < DateTime.UtcNow;
        public bool IsSelfSigned => Subject.Equals(Issuer, StringComparison.OrdinalIgnoreCase);
        public string RiskCategory => IsExpired ? "Expired Certificate" : (IsSelfSigned ? "Self-Signed Root / Proxy Cert" : "Untrusted Publisher Residual");
    }

    public static class CertStoreOrphanCleanerEngine
    {
        public static List<OrphanCertItem> ScanResidualCertificates()
        {
            var results = new List<OrphanCertItem>();

            var storesToScan = new (StoreLocation Location, StoreName Name)[]
            {
                (StoreLocation.CurrentUser, StoreName.My),
                (StoreLocation.CurrentUser, StoreName.TrustedPublisher),
                (StoreLocation.LocalMachine, StoreName.My),
                (StoreLocation.LocalMachine, StoreName.TrustedPublisher),
                (StoreLocation.CurrentUser, StoreName.CertificateAuthority)
            };

            foreach (var (loc, sName) in storesToScan)
            {
                try
                {
                    using var store = new X509Store(sName, loc);
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    foreach (var cert in store.Certificates)
                    {
                        var isExpired = cert.NotAfter < DateTime.UtcNow;
                        var isSelfSigned = cert.Subject.Equals(cert.Issuer, StringComparison.OrdinalIgnoreCase);

                        // Only surface certificates that are expired or self-signed non-system certs
                        if (isExpired || isSelfSigned)
                        {
                            var simpleSubject = cert.GetNameInfo(X509NameType.SimpleName, false) ?? cert.Subject;
                            // Skip core Microsoft root certs if valid
                            if (simpleSubject.Contains("Microsoft Root") && !isExpired) continue;

                            results.Add(new OrphanCertItem
                            {
                                Subject = simpleSubject,
                                Issuer = cert.GetNameInfo(X509NameType.SimpleName, true) ?? cert.Issuer,
                                Thumbprint = cert.Thumbprint,
                                StoreLocationName = loc.ToString(),
                                StoreNameLabel = sName.ToString(),
                                NotAfter = cert.NotAfter
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.Security, $"Failed querying certificate store {sName} in {loc}", ex.Message);
                }
            }

            return results.OrderByDescending(c => c.IsExpired).ThenBy(c => c.Subject).ToList();
        }

        public static int CleanCertificates(IEnumerable<OrphanCertItem> certs)
        {
            int cleaned = 0;
            if (certs == null) return 0;

            foreach (var item in certs)
            {
                try
                {
                    var loc = Enum.TryParse<StoreLocation>(item.StoreLocationName, out var l) ? l : StoreLocation.CurrentUser;
                    var name = Enum.TryParse<StoreName>(item.StoreNameLabel, out var n) ? n : StoreName.My;

                    using var store = new X509Store(name, loc);
                    store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);

                    var matches = store.Certificates.Find(X509FindType.FindByThumbprint, item.Thumbprint, false);
                    foreach (var match in matches)
                    {
                        store.Remove(match);
                        cleaned++;
                        StructuredLogger.Info(LogCategory.Security, $"Removed certificate {item.Subject} ({item.Thumbprint}) from {name}");
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Error(LogCategory.Security, $"Failed removing certificate {item.Subject}", ex.Message);
                }
            }

            return cleaned;
        }
    }
}
