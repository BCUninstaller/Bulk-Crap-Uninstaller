/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Factory;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller
{
    public static class UninstallManager
    {
        private const int SimulationDelay = 2500;

        /// <summary>
        ///     Rename the uninstaller entry by changing registry data. The entry is not refreshed in the process.
        /// </summary>
        public static bool Rename(this ApplicationUninstallerEntry entry, string newName)
        {
            if (string.IsNullOrEmpty(newName) || newName.ContainsAny(StringTools.InvalidPathChars))
                return false;

            using (var key = entry.OpenRegKey(true))
            {
                key.SetValue(RegistryFactory.RegistryNameDisplayName, newName);
            }
            return true;
        }

        /// <summary>
        ///     Uninstall multiple items in sequence. Items are uninstalled in order specified by the configuration.
        ///     This is a non-blocking method, a controller object is returned for monitoring of the task.
        ///     The task waits until an uninstaller fully exits before running the next one.
        /// </summary>
        /// <param name="targets">Uninstallers to run.</param>
        /// <param name="configuration">How the uninstallers should be ran.</param>
        public static BulkUninstallTask CreateBulkUninstallTask(IReadOnlyList<BulkUninstallEntry> targets,
            BulkUninstallConfiguration configuration)
        {
            return new BulkUninstallTask(targets, configuration);
        }

        /// <summary>
        ///     Start the default uninstaller with normal UI
        /// </summary>
        /// <exception cref="IOException">Uninstaller returned error code.</exception>
        /// <exception cref="InvalidOperationException">There are no usable ways of uninstalling this entry </exception>
        /// <exception cref="FormatException">Exception while decoding or attempting to run the uninstaller command. </exception>
        public static Process RunUninstaller(this ApplicationUninstallerEntry entry)
        {
            return RunUninstaller(entry, false, false);
        }

        /// <summary>
        ///     Start selected uninstaller type. If selected type is not available, fall back to the default.
        /// </summary>
        /// <param name="entry">Application to uninstall</param>
        /// <param name="silentIfAvailable">Choose quiet uninstaller if it's available.</param>
        /// <param name="simulate">If true, nothing will actually be uninstalled</param>
        /// <param name="safeMode">Don't modify the uninstall command to try avoid problems. Use when normal run fails.</param>
        /// <exception cref="IOException">Uninstaller returned error code.</exception>
        /// <exception cref="InvalidOperationException">There are no usable ways of uninstalling this entry </exception>
        /// <exception cref="FormatException">Exception while decoding or attempting to run the uninstaller command. </exception>
        public static Process RunUninstaller(this ApplicationUninstallerEntry entry, bool silentIfAvailable, bool simulate, bool safeMode = false)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            try
            {
                ProcessStartInfo startInfo = null;
                string fallBack = null;

                if (silentIfAvailable && entry.QuietUninstallPossible)
                {
                    // Use supplied quiet uninstaller if any
                    try
                    {
                        startInfo = ProcessTools.SeparateArgsFromCommand(entry.QuietUninstallString).ToProcessStartInfo();
                        Debug.Assert(!startInfo.FileName.Contains(' ') || File.Exists(startInfo.FileName));
                        if (QuietUninstallerIsCLI(entry))
                        {
                            // Safe to minimize quiet command windows
                            startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        }
                    }
                    catch (FormatException)
                    {
                        fallBack = entry.QuietUninstallString;
                    }
                }
                else if (entry.UninstallPossible)
                {
                    // Fall back to the non-quiet uninstaller
                    try
                    {
                        startInfo = ProcessTools.SeparateArgsFromCommand(entry.UninstallString).ToProcessStartInfo();
                        Debug.Assert(!startInfo.FileName.Contains(' ') || File.Exists(startInfo.FileName));

                        if (entry.UninstallerKind == UninstallerType.Nsis && !safeMode)
                            UpdateNsisStartInfo(startInfo, entry.DisplayName);
                    }
                    catch (FormatException)
                    {
                        fallBack = entry.UninstallString;
                    }
                }
                else
                {
                    // Cant do shit, capt'n
                    throw new InvalidOperationException(Localisation.UninstallError_Nowaytouninstall);
                }

                if (simulate)
                {
                    Thread.Sleep(SimulationDelay);
                    if (Debugger.IsAttached && new Random().Next(0, 2) == 0)
                        throw new IOException("Random failure for debugging");
                    return null;
                }

                if (fallBack != null)
                    return Process.Start(new ProcessStartInfo(fallBack) { UseShellExecute = true });

                if (startInfo != null)
                {
                    startInfo.UseShellExecute = true;
                    return Process.Start(startInfo);
                }

                // Cant do shit, capt'n
                throw new InvalidOperationException(Localisation.UninstallError_Nowaytouninstall);
            }
            catch (IOException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        public static bool QuietUninstallerIsCLI(this ApplicationUninstallerEntry entry)
        {
            if (!entry.QuietUninstallPossible) return false;
            switch (entry.UninstallerKind)
            {
                case UninstallerType.PowerShell:
                case UninstallerType.Steam:
                case UninstallerType.WindowsFeature:
                case UninstallerType.WindowsUpdate:
                case UninstallerType.StoreApp:
                case UninstallerType.Oculus:
                    return true;

                default:
                    return entry.QuietUninstallString.StartsWith("cmd ", StringComparison.OrdinalIgnoreCase) ||
                        entry.QuietUninstallString.Contains("cmd.exe", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        ///     Check if NSIS needs to be executed directly to get the return code. If yes, update the ProcessStartInfo
        ///     http://nsis.sourceforge.net/Docs/AppendixD.html#errorlevels
        /// </summary>
        private static void UpdateNsisStartInfo(ProcessStartInfo startInfo, string entryName)
        {
            var dirName = Path.GetFileName(Path.GetDirectoryName(startInfo.FileName));
            if (!string.IsNullOrEmpty(startInfo.Arguments) // Only works reliably if uninstaller doesn't use any Arguments already.
                                                           // Filter out non-standard uninstallers that might pose problems
                || !Path.GetFileNameWithoutExtension(startInfo.FileName).Contains("uninst", StringComparison.InvariantCultureIgnoreCase)
                || (dirName != null && dirName.Equals("uninstall", StringComparison.InvariantCultureIgnoreCase)))
                return;

            var newName = PathTools.SanitizeFileName(entryName);
            if (newName.Length > 8) newName = newName.Substring(0, 8);
            newName += "_" + Path.GetFileName(startInfo.FileName);

            var originalDirectory = Path.GetDirectoryName(startInfo.FileName)?.TrimEnd('\\');
            Debug.Assert(originalDirectory != null);
            startInfo.Arguments = "_?=" + originalDirectory;

            var tempPath = Path.Combine(Path.GetTempPath(), newName);
            File.Copy(startInfo.FileName, tempPath, true);
            startInfo.FileName = tempPath;
        }

        /// <summary>
        ///     Uninstall using msiexec in selected mode. If no guid is present nothing is done and -1 is returned.
        /// </summary>
        /// <param name="entry">Application to uninstall</param>
        /// <param name="mode">Mode of the MsiExec run.</param>
        /// <param name="simulate">If true, nothing will be actually uninstalled</param>
        /// <exception cref="IOException">Uninstaller returned error code.</exception>
        /// <exception cref="InvalidOperationException">There are no usable ways of uninstalling this entry </exception>
        /// <exception cref="FormatException">Exception while decoding or attempting to run the uninstaller command. </exception>
        public static int UninstallUsingMsi(this ApplicationUninstallerEntry entry, MsiUninstallModes mode,
            bool simulate)
        {
            try
            {
                var uninstallPath = GetMsiString(entry.BundleProviderKey, mode);
                if (string.IsNullOrEmpty(uninstallPath))
                    return -1;

                var startInfo = ProcessTools.SeparateArgsFromCommand(uninstallPath).ToProcessStartInfo();
                startInfo.UseShellExecute = false;
                if (simulate)
                {
                    Thread.Sleep(SimulationDelay);
                    return 0;
                }
                return startInfo.StartAndWait();
            }
            catch (IOException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }

        internal static string GetMsiString(Guid bundleProviderKey, MsiUninstallModes mode)
        {
            if (bundleProviderKey == Guid.Empty) return string.Empty;

            switch (mode)
            {
                case MsiUninstallModes.InstallModify:
                    return $@"MsiExec.exe /I{bundleProviderKey:B}";

                case MsiUninstallModes.QuietUninstall:
                    return $@"MsiExec.exe /qb /X{bundleProviderKey:B} REBOOT=ReallySuppress /norestart";

                case MsiUninstallModes.Uninstall:
                    return $@"MsiExec.exe /X{bundleProviderKey:B}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, @"Unknown mode");
            }
        }

        public static int Modify(this ApplicationUninstallerEntry entry, bool simulate)
        {
            try
            {
                if (string.IsNullOrEmpty(entry.ModifyPath)) return -1;

                var startInfo = ProcessTools.SeparateArgsFromCommand(entry.ModifyPath).ToProcessStartInfo();
                startInfo.UseShellExecute = false;
                if (simulate)
                {
                    Thread.Sleep(SimulationDelay);
                    return 0;
                }
                return startInfo.StartAndWait();
            }
            catch (IOException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }
        }
    }
}