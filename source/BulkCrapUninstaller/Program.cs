/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Xml.Linq;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Functions.Ratings;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools;
using UninstallTools.Factory;

namespace BulkCrapUninstaller
{
    //TODO This is a leftover class, extract the self installed detection logic and get rid of it
    public static class Program
    {
        private static DirectoryInfo _assemblyLocation;
        private static string _installedRegistryKeyName;
        private static bool? _isInstalled;

        public static DirectoryInfo AssemblyLocation
        {
            get
            {
                if (_assemblyLocation == null)
                {
                    var location = Assembly.GetAssembly(typeof(Program))?.Location;
                    if (location == null) throw new InvalidOperationException("Failed to get entry assembly location");
                    if (Path.HasExtension(location))
                        location = PathTools.GetDirectory(location);
                    _assemblyLocation = new DirectoryInfo(location);
                }
                return _assemblyLocation;
            }
        }

        public static Version AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        ///     Do not call before CheckForOldSettings() completes
        /// </summary>
        public static bool EnableDebug => Debugger.IsAttached || Settings.Default.Debug;

        /// <summary>
        /// Don't use settings
        /// </summary>
        public static Uri ConnectionString { get; } = Debugger.IsAttached ? new Uri(@"http://localhost:7721") : new Uri(@"http://bugsklocman.ddns.net:7721");

        public static string InstalledRegistryKeyName
        {
            get
            {
                if (_installedRegistryKeyName == null)
                    GetInstalledRegKey();
                return _installedRegistryKeyName;
            }
        }

        public static bool IsAfterUpgrade { get; private set; }

        /// <summary>
        ///     Use setter to override the value
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                if (!_isInstalled.HasValue)
                    _isInstalled = InstalledRegistryKeyName.IsNotEmpty();
                return _isInstalled.Value;
            }
            internal set { _isInstalled = value; }
        }

        internal static string ConfigFileFullname { get; private set; }

        /// <summary>
        ///     Remove old or invalid setting files and make sure settings are ready to be used.
        ///     Run before the settings are used, best at the very start of the application.
        /// </summary>
        internal static void PrepareSettings()
        {
            const string exeName = "BCUninstaller";

            IsAfterUpgrade = false;
            try
            {
                var dir = AssemblyLocation;
                // Check if we are bundled with a launcher and place settings in the same folder as the launcher, so they are shared between different builds
                if (dir.Name.StartsWith("win-") && dir.Parent != null &&
                    File.Exists(Path.Combine(dir.Parent.FullName, exeName + ".exe"))) dir = dir.Parent;

                var settingsDir = dir.FullName;
                ConfigFileFullname = Path.Combine(settingsDir, exeName + ".settings");

                PortableSettingsProvider.PortableSettingsProvider.AppSettingsPathOverride = settingsDir;
                PortableSettingsProvider.PortableSettingsProvider.ApplicationNameOverride = exeName;

                var settingsXmlDocument = XDocument.Parse(File.ReadAllText(ConfigFileFullname));
                if (settingsXmlDocument.Root == null) throw new FormatException("Missing root element");
                
                var result = settingsXmlDocument.Root.Element("MiscVersion");
                if (result == null) throw new FormatException("Invalid version number");
                if (result.Value.Equals("Reset")) throw new OperationCanceledException("Settings reset was requested");

                if (!string.IsNullOrWhiteSpace(result.Value) && new Version(result.Value) < AssemblyVersion)
                    IsAfterUpgrade = true;

                // One extra check to make sure loading and using the settings doesn't throw
                // Initializes the Default settings object (unless it has been accessed before, which it shouldn't have)
                Settings.Default.Reload();
                Settings.Default.AdvancedSimulate = Settings.Default.AdvancedSimulate;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                    Console.WriteLine(@"Settings file not found, creating new one.");
                else if (ex is not OperationCanceledException)
                    Console.WriteLine(@"Failed to load settings from the config file: " + ex);

                File.Delete(ConfigFileFullname);
                Settings.Default.Reload();
            }

            // Ensure the user ID is valid
            if (Settings.Default.MiscUserId == 0)
                Settings.Default.MiscUserId = GetUniqueUserId();

            if (IsAfterUpgrade)
                ClearCaches(false);
        }

        /// <summary>
        /// Get an ID that is unlikely to be duplicate but that should always return the same on current pc
        /// </summary>
        private static ulong GetUniqueUserId()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();

            string networkIdentity;
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                networkIdentity = string.Join("", networkInterfaces.Select(x => x.GetPhysicalAddress().ToString()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                networkIdentity = e.ToString();
            }

            var idStr = windowsIdentity.User?.Value + string.Join("", windowsIdentity.Claims.Select(x => x.Value)) + networkIdentity;
            return UninstallerRatingManager.Utils.StableHash(idStr);
        }

        /// <summary>
        /// Check if this application is installed by looking for the registry key created by the installer.
        /// If the key is not found it means this is most likely a portable version.
        /// </summary>
        private static void GetInstalledRegKey()
        {
            // This GUID is the AppID from the installer. It can end with an optional identifier if the installer had to create a new key because of a conflict.
            const string appId = "f4fef76c-1aa9-441c-af7e-d27f58d898d1";
            const string regUninstallersKeyDirect = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            try
            {
                using var regKey = Registry.LocalMachine.OpenSubKey(regUninstallersKeyDirect);

                if (regKey == null)
                    throw new ArgumentException("Could not open Software registry key");

                var keyNames = regKey.GetSubKeyNames().Where(x => x.Contains(appId, StringComparison.InvariantCultureIgnoreCase));

                foreach (var keyName in keyNames)
                {
                    using var subKey = regKey.OpenSubKey(keyName, true);

                    var installLocation = subKey?.GetStringSafe(RegistryFactory.RegistryNameInstallLocation);
                    if (string.IsNullOrEmpty(installLocation)) continue;

                    if (PathTools.SubPathIsInsideBasePath(installLocation, AssemblyLocation.FullName, true, true))
                    {
                        // We are installed!
                        _installedRegistryKeyName = keyName;

                        // Update the version number in case it changed, so the user can see it in the list of installed programs
                        subKey.SetValue("DisplayVersion", AssemblyVersion.ToString(), RegistryValueKind.String);
                    }
                }
            }
            catch
            {
                _installedRegistryKeyName = String.Empty;
            }
        }

        public static void StartLogCleaner()
        {
            try
            {
                const string cleanerName = "CleanLogs.bat";
                var cleanerPath = Path.Combine(AssemblyLocation.FullName, cleanerName);

                if (!File.Exists(cleanerPath))
                {
                    Console.WriteLine(@"WARNING: CleanLogs.bat doesn't exist, can't clean logs.");
                    return;
                }

                var cleanerUri = PathToUri(cleanerPath);
                if (cleanerUri.IsUnc)
                {
                    // 'cmd.exe /c start' doesn't work with UNC paths, script has to run in foreground.
                    Process.Start(new ProcessStartInfo(cleanerPath) { UseShellExecute = true });
                }
                else
                {
                    // Run cleanup script in minimized cmd window
                    var ps = new ProcessStartInfo
                    {
                        WorkingDirectory = AssemblyLocation.FullName,
                        FileName = "cmd.exe",
                        Arguments = "/c start /min " + cleanerName,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                    Process.Start(ps);
                }
            }
            catch (Exception ex)
            {
                // Ignore errors, not critical
                Console.WriteLine(ex);
            }
        }

        private static Uri PathToUri(string filePath)
        {
            try
            {
                return new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                return new Uri(filePath);
            }
        }

        public static void ClearCaches(bool showErrors)
        {
            try
            {
                MainWindow.CertificateCache.ClearChache();
                UninstallToolsGlobalConfig.ClearChache();
            }
            catch (SystemException systemException)
            {
                if (showErrors)
                    PremadeDialogs.GenericError(systemException);
                else
                    Console.WriteLine(systemException);
            }
        }

        public static HttpClient HomeServerClient
        {
            get
            {
                var cl = new HttpClient();
                cl.BaseAddress = ConnectionString;
                return cl;
            }
        }
    }
}