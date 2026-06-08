/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Localising;

namespace BulkCrapUninstaller.Functions.Tools
{
    internal class LocalizedX509Certificate2
    {
        public LocalizedX509Certificate2(X509Certificate2 baseCert)
        {
            BaseCert = baseCert;
        }

        private X509Certificate2 BaseCert { get; }

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_Archived))]
        public bool Archived => BaseCert.Archived;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_Extensions))]
        public string Extensions
            => string.Join(", ", BaseCert.Extensions.Cast<X509Extension>().Where(x => x.Oid != null).Select(x => x.Oid.FriendlyName).ToArray());

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_FriendlyName))]
        public string FriendlyName => BaseCert.FriendlyName;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_HasPrivateKey))]
        public bool HasPrivateKey => BaseCert.HasPrivateKey;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_Issuer))]
        public string Issuer => BaseCert.Issuer;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_IssuerName))]
        public string IssuerName => BaseCert.IssuerName.Format(false);

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_NotAfter))]
        public DateTime NotAfter => BaseCert.NotAfter;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_NotBefore))]
        public DateTime NotBefore => BaseCert.NotBefore;

#pragma warning disable SYSLIB0027
#pragma warning disable SYSLIB0028
        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_PrivateKey))]
        public string PrivateKey => BaseCert.PrivateKey?.ToString();

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_PublicKey))]
        public string PublicKey => BaseCert.PublicKey.Key.SignatureAlgorithm;
#pragma warning restore SYSLIB0028
#pragma warning restore SYSLIB0027

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_RawData))]
        public string RawData => BaseCert.RawData.ToHexString();

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_SerialNumber))]
        public string SerialNumber => BaseCert.SerialNumber;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_SignatureAlgorithm))]
        public string SignatureAlgorithm => BaseCert.SignatureAlgorithm.FriendlyName;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_Subject))]
        public string Subject => BaseCert.Subject;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_SubjectName))]
        public string SubjectName => BaseCert.SubjectName.Format(false);

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_Thumbprint))]
        public string Thumbprint => BaseCert.Thumbprint;

        [LocalisedName(typeof(Localisable), nameof(Localisable.LocalizedX509Certificate2_Version))]
        public int Version => BaseCert.Version;
    }
}