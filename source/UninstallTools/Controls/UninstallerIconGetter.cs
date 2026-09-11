/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Klocman.Extensions;

namespace UninstallTools.Controls
{
    public class UninstallerIconGetter : Component
    {
        private static readonly string ApplicationIconKey = "applicationIcon";
        private static readonly string InvalidIconKey = "invalidIcon";
        private static readonly string MsiexecIconKey = "msiexecIcon";
        private static readonly string UpdateIconKey = "updateIcon";
        // The name "OperatingSystem" is important
        private static readonly string WindowsIconKey = "OperatingSystem";

        public UninstallerIconGetter()
        {
            Disposed += DisposeHandler;
        }

        public ImageList IconList { get; private set; }

        public object ColumnImageGetter(object rowObj)
        {
            if (!(rowObj is ApplicationUninstallerEntry entry) || IconList == null)
                return null;

            if (IconListContainsKey(entry.DisplayName))
                return entry.DisplayName;

            if (entry.ParentKeyName.IsNotEmpty())
            {
                if (entry.ParentKeyName.Equals(WindowsIconKey))
                    return WindowsIconKey;

                if (IconListContainsKey(entry.ParentKeyName))
                    return entry.ParentKeyName;
            }

            if (entry.IsUpdate || entry.UninstallerKind == UninstallerType.WindowsFeature)
                return UpdateIconKey;

            if (entry.UninstallerKind == UninstallerType.Msiexec || entry.UninstallerKind == UninstallerType.SdbInst)
                return MsiexecIconKey;

            if (entry.IsValid)
                return ApplicationIconKey;

            return InvalidIconKey;
        }

        /// <exception cref="PlatformNotSupportedException">The current platform is not supported.</exception>
        public void UpdateIconList(IEnumerable<ApplicationUninstallerEntry> objList)
        {
            IconList = new ImageList();
            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            IconList.Images.Add(ApplicationIconKey, SystemIcons.Application);
            IconList.Images.Add(InvalidIconKey, SystemIcons.Exclamation);
            IconList.Images.Add(WindowsIconKey, SystemIcons.Shield); //SystemIcons.WinLogo); winlogo not working on xp?
            IconList.Images.Add(UpdateIconKey, SystemIcons.Shield);
            var msiIcon = UninstallToolsGlobalConfig.TryExtractAssociatedIcon(windowsPath + @"\msiexec.exe");
            IconList.Images.Add(MsiexecIconKey, msiIcon ?? SystemIcons.Application);

            foreach (var obj in objList)
            {
                if (IconListContainsKey(obj.DisplayName))
                    continue;

                try
                {
                    var image = obj.GetIcon();
                    if (image != null)
                    {
                        IconList.Images.Add(obj.DisplayName, image);
                    }
                }
                catch
                {
                    // Revert to default icon
                }
            }
        }

        private void DisposeHandler(object x, EventArgs y)
        {
            if (IconList != null)
            {
                IconList.Dispose();
                IconList = null;
            }
        }

        private bool IconListContainsKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (IconList == null)
                throw new InvalidOperationException("IconListContainsKey called when IconList is null");

            return IconList.Images.Keys.Cast<string>().Any(x => x.Equals(key));
        }
    }
}