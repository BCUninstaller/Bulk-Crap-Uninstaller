/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class UninstallerTypeAdder : IMissingInfoAdder
    {
        private static readonly Regex InnoSetupFilenameRegex = new(@"unins\d\d\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.Unknown)
                return;

            var uninstallString = !string.IsNullOrEmpty(target.UninstallString) 
                ? target.UninstallString 
                : target.QuietUninstallString;

            if (!string.IsNullOrEmpty(uninstallString))
            {
                target.UninstallerKind = GetUninstallerType(uninstallString);
            }
            else if (!string.IsNullOrEmpty(target.InstallLocation))
            {
                // We don't have a valid uninstaller, so tell simple delete adder to do its job and make our own
                target.UninstallerKind = UninstallerType.SimpleDelete;
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.QuietUninstallString),
            nameof(ApplicationUninstallerEntry.InstallLocation)
        };
        public bool RequiresAllValues { get; } = false;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind)
        };

        // Let other adders run first in case they add uninstall strings
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;

        public static UninstallerType GetUninstallerType(string uninstallString)
        {
            // Detect MSI / Windows installer based on the uninstall string
            // e.g. "C:\ProgramData\Package Cache\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}\vcredist_x86.exe"  /uninstall
            if (ApplicationEntryTools.PathPointsToMsiExec(uninstallString) || uninstallString.ContainsAll(
                new[] { @"\Package Cache\{", @"}\", ".exe" }, StringComparison.OrdinalIgnoreCase))
                return UninstallerType.Msiexec;

            // Detect Sdbinst
            if (uninstallString.Contains("sdbinst", StringComparison.OrdinalIgnoreCase)
                && uninstallString.Contains(".sdb", StringComparison.OrdinalIgnoreCase))
                return UninstallerType.SdbInst;

            if (uninstallString.Contains(@"InstallShield Installation Information\{", StringComparison.OrdinalIgnoreCase))
                return UninstallerType.InstallShield;

            if (uninstallString.Contains("powershell.exe", StringComparison.OrdinalIgnoreCase) ||
                uninstallString.Contains(".ps1", StringComparison.OrdinalIgnoreCase))
                return UninstallerType.PowerShell;

            // Detect Squirrel / Electron auto-updater installs
            // e.g. "C:\Users\<u>\AppData\Local\<Vendor>\<App>\Update.exe --uninstall -s"
            // Discord, GitHub Desktop, Slack (classic), Teams (classic), many Electron apps
            if (uninstallString.Contains("Update.exe", StringComparison.OrdinalIgnoreCase)
                && uninstallString.Contains("--uninstall", StringComparison.OrdinalIgnoreCase)
                && uninstallString.Contains(@"\AppData\Local\", StringComparison.OrdinalIgnoreCase))
                return UninstallerType.Squirrel;

            if (ProcessStartCommand.TryParse(uninstallString, out var ps) 
                && Path.IsPathRooted(ps.FileName) 
                && File.Exists(ps.FileName))
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(ps.FileName);
                    // Detect Inno Setup
                    if (fileName != null && InnoSetupFilenameRegex.IsMatch(fileName))
                    {
                        // Check if Inno Setup Uninstall Log exists
                        if (File.Exists(ps.FileName.Substring(0, ps.FileName.Length - 3) + "dat"))
                            return UninstallerType.InnoSetup;
                    }

                    // Detect NSIS Nullsoft.NSIS. Slow, but there's no other way than to scan the file
                    using (var reader = new StreamReader(ps.FileName!, Encoding.ASCII))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("Nullsoft", StringComparison.Ordinal))
                                return UninstallerType.Nsis;
                        }
                    }

                    /* Unused/unnecessary
                    if (result.Contains("InstallShield"))
                        return UninstallerType.InstallShield;
                    if (result.Contains("Inno.Setup") || result.Contains("Inno Setup"))
                        return UninstallerType.InnoSetup;
                    if(result.Contains(@"<description>Adobe Systems Incorporated Setup</description>"))
                        return UninstallerType.AdobeSetup;
                    */
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (SecurityException) { }
            }
            return UninstallerType.Unknown;
        }
    }
}