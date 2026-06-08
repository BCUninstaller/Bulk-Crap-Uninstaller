/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32;

namespace UninstallTools.Startup.Normal
{
    internal sealed class NewStartupDisable : IStartupDisable
    {
        /// <summary>
        ///     Only the first byte matters, following bytes contain data that there is no documentation for (at least I didn't
        ///     find any)
        /// </summary>
        private static readonly byte[] DisabledBytes =
        {
            0x03, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        /// <summary>
        ///     So far I've only seen enabled values filled with 0's except for the first byte
        /// </summary>
        private static readonly byte[] EnabledBytes =
        {
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public IEnumerable<StartupEntry> AddDisableInfo(IList<StartupEntry> existingEntries)
        {
            return existingEntries.DoForEach(x =>
            {
                if (GetDisabled(x))
                    x.DisabledStore = true;
            });
        }

        public void Disable(StartupEntry startupEntry)
        {
            SetDisabled(startupEntry, true);
        }

        public void Enable(StartupEntry startupEntry)
        {
            SetDisabled(startupEntry, false);
        }

        public string GetDisabledEntryPath(StartupEntry startupEntry)
        {
            return startupEntry.FullLongName;
        }

        public bool StillExists(StartupEntry startupEntry)
        {
            if (startupEntry.IsRegKey)
            {
                using (var key = RegistryTools.OpenRegistryKey(startupEntry.ParentLongName))
                    return key != null && !string.IsNullOrEmpty(key.GetStringSafe(startupEntry.EntryLongName));
            }

            return File.Exists(startupEntry.FullLongName);
        }

        private static bool GetDisabled(StartupEntry startupEntry)
        {
            try
            {
                using (var key = RegistryTools.CreateSubKeyRecursively(GetStartupApprovedKey(startupEntry)))
                {
                    return key.GetValue(startupEntry.EntryLongName) is byte[] bytes 
                        && bytes.Length > 0 
                        && !bytes[0].Equals(0x02);
                }
            }
            catch (SystemException ex)
            {
                Trace.WriteLine($"Failed to get Disabled start-up state for {startupEntry.ProgramName}: {ex}");
                return false;
            }
        }
        
        private static string GetStartupApprovedKey(StartupEntry startupEntry)
        {
            if (!startupEntry.IsRegKey)
                return startupEntry.AllUsers
                    ? @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder"
                    : @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";

            if (startupEntry.IsRunOnce)
            {
                if (startupEntry.AllUsers)
                {
                    return startupEntry.ParentLongName.Contains("Wow6432Node")
                        ? @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\RunOnce32"
                        : @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\RunOnce";
                }
                return @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\RunOnce";
            }

            if (startupEntry.AllUsers)
            {
                return startupEntry.ParentLongName.Contains("Wow6432Node")
                    ? @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run32"
                    : @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
            }
            return @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        }

        private static void SetDisabled(StartupEntry startupEntry, bool disabled)
        {
            using (var key = RegistryTools.CreateSubKeyRecursively(GetStartupApprovedKey(startupEntry)))
            {
                key.SetValue(startupEntry.EntryLongName, disabled ? DisabledBytes : EnabledBytes,
                    RegistryValueKind.Binary);
            }
        }
    }
}