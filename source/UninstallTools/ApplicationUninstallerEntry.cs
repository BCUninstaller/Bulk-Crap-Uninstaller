/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Localising;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Factory;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Lists;
using UninstallTools.Properties;
using UninstallTools.Startup;

namespace UninstallTools
{
    public class ApplicationUninstallerEntry
    {
        /// <summary>
        /// List of properties that migh have changed by updating the key property
        /// IMPORTANT: Keep up to date!
        /// </summary>
        internal static readonly ILookup<string, string> PropertyRelationships = new Dictionary<string, List<string>>
        {
            {
                nameof(UninstallString),
                new List<string>
                {
                    nameof(UninstallerLocation),
                    nameof(UninstallerFullFilename)
                }
            },
            {
                nameof(UninstallerFullFilename),
                new List<string> {nameof(UninstallerLocation)}
            },
            {
                nameof(RawDisplayName),
                new List<string> {nameof(DisplayName)}
            },
            {
                nameof(RegistryKeyName),
                new List<string> {nameof(RatingId)}
            },
        }.SelectMany(x => x.Value.Select(y => new { x.Key, Value = y })).ToLookup(x => x.Key, x => x.Value);

        internal static readonly IEnumerable<string> CompanyNameEndTrimmers =
            new[] { "corp", "corporation", "limited", "inc", "incorporated" };

        private X509Certificate2 _certificate;
        private bool _certificateGotten;
        private bool? _certificateValid;
        private string _ratingId;
        private string _installLocation;
        private string _installSource;
        private string _modifyPath;
        private string _displayIcon;
        private string _uninstallString;
        private string _uninstallerFullFilename;

        /// <summary>
        /// Junk specified during creation of the entry that would not be detected afterwards. It's added to the results during junk scan.
        /// </summary>
        [XmlIgnore]
        internal readonly List<Junk.Containers.IJunkResult> AdditionalJunk = new();

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.DisplayName))]
        public string DisplayName
        {
            get { return (string.IsNullOrEmpty(RawDisplayName) ? RegistryKeyName : RawDisplayName) ?? string.Empty; }
            set { RawDisplayName = value; }
        }

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), nameof(Localisation.DisplayNameTrimmed))]
        public string DisplayNameTrimmed => StringTools.StripStringFromVersionNumber(DisplayName);

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), nameof(Localisation.PublisherTrimmed))]
        public string PublisherTrimmed => string.IsNullOrEmpty(Publisher)
            ? string.Empty
            : Publisher.Replace("(R)", string.Empty)
                .ExtendedTrimEndAny(CompanyNameEndTrimmers, StringComparison.CurrentCultureIgnoreCase);

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), nameof(Localisation.QuietUninstallPossible))]
        public bool QuietUninstallPossible => !string.IsNullOrEmpty(QuietUninstallString) ||
                                              (UninstallerKind == UninstallerType.Msiexec &&
                                               BundleProviderKey != Guid.Empty);

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), nameof(Localisation.UninstallPossible))]
        public bool UninstallPossible => !string.IsNullOrEmpty(UninstallString);

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.AboutUrl))]
        public string AboutUrl { get; set; }

        /// <summary>
        ///     Product code used by msiexec. If it wasn't found, returns Guid.Empty.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.BundleProviderKey))]
        public Guid BundleProviderKey { get; set; }

        [LocalisedName(typeof(Localisation), nameof(Localisation.Comment))]
        public string Comment { get; set; }

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), nameof(Localisation.CustomNote))]
        public string CustomNote
        {
            get => CustomNotesManager.GetNote(GetCacheId());
            set => CustomNotesManager.SetNote(GetCacheId(), value);
        }

        [LocalisedName(typeof(Localisation), nameof(Localisation.DisplayIcon))]
        public string DisplayIcon
        {
            get { return _displayIcon; }
            set { _displayIcon = CleanupPath(value, true); }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.DisplayVersion))]
        public string DisplayVersion { get; set; }

        [LocalisedName(typeof(Localisation), nameof(Localisation.EstimatedSize))]
        public FileSize EstimatedSize { get; set; }

        [LocalisedName(typeof(Localisation), nameof(Localisation.InstallDate))]
        public DateTime InstallDate { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.InstallLocation))]
        public string InstallLocation
        {
            get { return _installLocation; }
            set { _installLocation = CleanupPath(value); }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.InstallSource))]
        public string InstallSource
        {
            get { return _installSource; }
            set { _installSource = CleanupPath(value); }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.Is64Bit))]
        public MachineType Is64Bit { get; set; }

        /// <summary>
        ///     Protection from uninstalling.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.IsProtected))]
        public bool IsProtected { get; set; }

        /// <summary>
        ///     The application's uniunstaller is mentioned in the registry (if it's not normal uninstallers will not see it)
        /// </summary>
        [LocalisedName(typeof(Localisation), nameof(Localisation.IsRegistered))]
        public bool IsRegistered { get; set; }

        /// <summary>
        ///     The application is present on the drive, but not in any of the application listings
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.IsOrphaned))]
        public bool IsOrphaned { get; set; }

        /// <summary>
        ///     True if this is an update for another product
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.IsUpdate))]
        public bool IsUpdate { get; set; }

        [XmlIgnore]
        [ComparisonTarget]
        public bool IsScriptTweak => !string.IsNullOrEmpty(RatingId) &&
                                     RatingId.StartsWith("Tweak-", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     True if the application can be uninstalled. False if the uninstaller is missing or is otherwise invalid.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.IsValid))]
        public bool IsValid { get; set; }

        /// <summary>
        ///     True if the application is listed as a web browser.
        /// </summary>
        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.IsWebBrowser))]
        public bool IsWebBrowser { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.ModifyPath))]
        public string ModifyPath
        {
            get { return _modifyPath; }
            set { _modifyPath = CleanupPath(value); }
        }

        [LocalisedName(typeof(Localisation), nameof(Localisation.ParentKeyName))]
        public string ParentKeyName { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.Publisher))]
        public string Publisher { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.QuietUninstallString))]
        public string QuietUninstallString { get; set; }

        /// <summary>
        /// Get a unique cache ID of this item. 
        /// Returns null if there isn't enough information to get a reasonably unique key.
        /// </summary>
        public string GetCacheId()
        {
            if (!string.IsNullOrEmpty(CacheIdOverride))
                return CacheIdOverride;

            var rid = RatingId;
            if (!string.IsNullOrEmpty(rid))
                return rid;

            if (!string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(InstallLocation))
                return DisplayName + InstallLocation;// + DisplayVersion + InstallDate + EstimatedSize;

            return null;
        }

        public string CacheIdOverride;

        public string RatingId
        {
            get
            {
                if (!string.IsNullOrEmpty(_ratingId))
                    return _ratingId;
                if (!string.IsNullOrEmpty(RegistryKeyName))
                    return RegistryKeyName;
                //if (BundleProviderKey != Guid.Empty)
                //    return BundleProviderKey.ToString();
                return null;
            }
            set { _ratingId = value; }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.RegistryKeyName))]
        public string RegistryKeyName { get; set; }

        /// <summary>
        ///     Full registry path of this entry
        /// </summary>
        [LocalisedName(typeof(Localisation), nameof(Localisation.RegistryPath))]
        public string RegistryPath { get; set; }

        [XmlIgnore]
        [LocalisedName(typeof(Localisation), nameof(Localisation.StartupEntries))]
        public IEnumerable<StartupEntryBase> StartupEntries { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.HasStartupEntries))]
        public bool HasStartups => StartupEntries != null && StartupEntries.Any();

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.SystemComponent))]
        public bool SystemComponent { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.UninstallerFullFilename))]
        public string UninstallerFullFilename
        {
            get { return _uninstallerFullFilename; }
            set
            {
                _uninstallerFullFilename = value;

                UninstallerLocation = ApplicationEntryTools.ExtractDirectoryName(UninstallerFullFilename)
                                      ?? UninstallerLocation ?? string.Empty;
            }
        }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.UninstallerKind))]
        public UninstallerType UninstallerKind { get; set; }

        //[LocalisedName(typeof(Localisation), nameof(Localisation.IsInstalled))]
        //public bool IsInstalled { get; internal set; }

        [LocalisedName(typeof(Localisation), nameof(Localisation.UninstallerLocation))]
        public string UninstallerLocation { get; set; }

        [ComparisonTarget]
        [LocalisedName(typeof(Localisation), nameof(Localisation.UninstallString))]
        public string UninstallString
        {
            get { return _uninstallString; }
            set
            {
                _uninstallString = value;

                UninstallerFullFilename = ApplicationEntryTools.ExtractFullFilename(value)
                    ?? UninstallerFullFilename ?? string.Empty;
            }
        }

        internal string RawDisplayName { get; set; }

        internal Icon IconBitmap { get; set; }

        /// <summary>
        ///     Check if the install location is not empty and is not a system directory
        /// </summary>
        public bool IsInstallLocationValid()
        {
            if (string.IsNullOrEmpty(InstallLocation?.Trim()))
                return false;
            return !UninstallToolsGlobalConfig.GetAllProgramFiles().Any(x => PathTools.PathsEqual(x, InstallLocation));
        }

        /*/// <summary>
        ///     Get the certificate associated to the uninstaller or application.
        /// </summary>
        /// <param name="onlyStored">If true only return the stored value, otherwise generate it if needed.</param>
        public X509Certificate2 GetCertificate(bool onlyStored)
        {
            return onlyStored ? _certificate : GetCertificate();
        }*/

        /// <summary>
        ///     Get the certificate associated to the uninstaller or application.
        /// </summary>
        public X509Certificate2 GetCertificate()
        {
            if (!_certificateGotten)
            {
                _certificateGotten = true;
                _certificate = CertificateGetter.TryGetCertificate(this);

                if (_certificate != null)
                    _certificateValid = _certificate.Verify();
            }
            return _certificate;
        }

        public void SetCertificate(X509Certificate2 c)
        {
            _certificateGotten = true;
            _certificate = c;

            if (_certificate != null)
                _certificateValid = _certificate.Verify();
        }

        public void SetCertificate(X509Certificate2 c, bool v)
        {
            _certificateGotten = true;
            _certificate = c;
            _certificateValid = v;
        }

        public Icon GetIcon()
        {
            return IconBitmap;
        }

        /// <summary>
        ///     Ordered collection of filenames that could be the main executable of the application.
        ///     The most likely files are first, the least likely are last.
        /// </summary>
        internal string[] SortedExecutables { get; set; }

        public IEnumerable<string> GetSortedExecutables()
        {
            if (SortedExecutables == null)
                return Enumerable.Empty<string>();
            var output = SortedExecutables.AsEnumerable();
            if (!string.IsNullOrEmpty(UninstallerFullFilename))
                output = output.OrderBy(x => x.Equals(UninstallerFullFilename, StringComparison.InvariantCultureIgnoreCase));
            return output;
        }

        public Uri GetAboutUri()
        {
            var temp = AboutUrl;
            if (!temp.IsNotEmpty()) return null;

            temp = temp.ToLowerInvariant().Replace("www.",
                temp.StartsWith("www.", StringComparison.InvariantCulture) ? @"http://www." : string.Empty);

            try
            {
                return new Uri(temp, UriKind.Absolute);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Check if certificate is valid. It returns null if the certificate is missing or GetCertificate has not
        ///     been ran yet and onlyStored is set to true.
        /// </summary>
        public bool? IsCertificateValid(bool onlyStored)
        {
            if (!onlyStored && !_certificateGotten)
                GetCertificate();

            return _certificateValid;
        }

        /// <summary>
        ///     Opens a new read-only instance of registry key used by this uninstaller. Remember to close it!
        /// </summary>
        public RegistryKey OpenRegKey()
        {
            return RegistryPath != null ? RegistryTools.OpenRegistryKey(RegistryPath) : null;
        }

        /// <summary>
        ///     Opens a new instance of registry key used by this uninstaller. Remember to close it!
        /// </summary>
        /// <exception cref="System.Security.SecurityException">
        ///     The user does not have the permissions required to access the registry key in the
        ///     specified mode.
        /// </exception>
        public RegistryKey OpenRegKey(bool writable)
        {
            return RegistryTools.OpenRegistryKey(RegistryPath, writable);
        }

        /// <summary>
        ///     Check if entry has not been uninstalled already (check registry key)
        /// </summary>
        /// <returns></returns>
        public bool RegKeyStillExists()
        {
            if (string.IsNullOrEmpty(RegistryPath))
                return false;
            try
            {
                using (var key = OpenRegKey())
                    return key != null;
            }
            catch
            {
                return false;
            }
        }

        public string ToLongString()
        {
            var sb = new StringBuilder();
            sb.Append(DisplayName);
            sb.AppendFormat(" | {0}", Publisher);
            sb.AppendFormat(" | {0}", DisplayVersion);
            sb.AppendFormat(" | {0}", GetInstallDateString());
            sb.AppendFormat(" | {0}", EstimatedSize);
            sb.AppendFormat(" | {0}", RegistryPath);
            sb.AppendFormat(" | {0}", UninstallerKind);
            sb.AppendFormat(" | {0}", UninstallString);
            sb.AppendFormat(" | {0}", QuietUninstallString);
            sb.AppendFormat(" | {0}", Comment);

            return sb.ToString();
        }

        private string GetInstallDateString()
        {
            try
            {
                return DateTime.MinValue.Equals(InstallDate) ? string.Empty : InstallDate.ToShortDateString();
            }
            catch (SystemException)
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(DisplayName);
            sb.AppendFormat(" | {0}", Publisher);
            sb.AppendFormat(" | {0}", DisplayVersion);
            sb.AppendFormat(" | {0}", UninstallString);
            sb.AppendFormat(" | {0}", Comment);

            return sb.ToString();
        }

        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static string CleanupPath(string path, bool isFilename = false)
        {
            if (string.IsNullOrEmpty(path)) return null;

            if (!isFilename)
            {
                // Try the fast method first for directories
                var trimmed = path.Trim('"', ' ', '\'', '\\', '/');

                if (!trimmed.ContainsAny(InvalidPathChars))
                    return trimmed;
            }

            try
            {
                path = ProcessTools.SeparateArgsFromCommand(path).FileName;
                if (!isFilename && path.Contains('.') && !Directory.Exists(path))
                    return Path.GetDirectoryName(path);
            }
            catch
            {
                // If sanitization failed just leave it be, it will be handled afterwards
            }
            return path.TrimEnd('\\');
        }
    }
}
