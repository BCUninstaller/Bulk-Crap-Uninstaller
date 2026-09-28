/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class EventLogScanner : JunkCreatorBase
    {
        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            if (string.IsNullOrEmpty(target.InstallLocation)) yield break;

            var otherUninstallers = GetOtherUninstallers(target).ToList();

            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SYSTEM\CurrentControlSet\Services\EventLog\Application"))
            {
                if (key == null) yield break;

                var query = from name in key.GetSubKeyNames()
                    let m = ConfidenceGenerators.MatchStringToProductName(target, name, allowGenericName: true)
                    where m >= 0 && m < 3
                    //orderby m ascending
                    select name;

                foreach (var result in query)
                {
                    using (var subkey = key.OpenSubKey(result))
                    {
                        var exePath = subkey?.GetStringSafe("EventMessageFile");
                        if (string.IsNullOrEmpty(exePath) || !PathTools.SubPathIsInsideBasePath(target.InstallLocation, Path.GetDirectoryName(exePath), true, false)) continue;

                        var node = new RegistryKeyJunk(subkey.Name, target, this);
                        // Already matched names above
                        node.Confidence.Add(ConfidenceRecords.ProductNamePerfectMatch);

                        if (otherUninstallers.Any(x => PathTools.SubPathIsInsideBasePath(x.InstallLocation, Path.GetDirectoryName(exePath), true, false)))
                            node.Confidence.Add(ConfidenceRecords.DirectoryStillUsed);

                        yield return node;
                    }
                }
            }
        }

        public override string CategoryName => Localisation.Junk_EventLog_GroupName;
    }
}