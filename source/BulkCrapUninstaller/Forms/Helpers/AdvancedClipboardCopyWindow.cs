/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using Klocman.Forms.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Forms
{
    public partial class AdvancedClipboardCopyWindow : Form
    {
        private AdvancedClipboardCopyWindow()
        {
            InitializeComponent();
            ModernStyleController.ApplyOnLoad(this);
        }

        public static void ShowDialog(Form parent, IEnumerable<ApplicationUninstallerEntry> targets)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            using (var window = new AdvancedClipboardCopyWindow())
            {
                window.advancedClipboardCopy1.Targets = targets;
                window.Icon = parent.Icon;
                window.ShowDialog(parent);
            }
        }

        private void AdvancedClipboardCopyWindow_Shown(object sender, EventArgs e)
        {
            advancedClipboardCopy1.PatternText = "{" + nameof(ApplicationUninstallerEntry.DisplayName) + "} - {" +
                                                 nameof(ApplicationUninstallerEntry.UninstallString) + "}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(advancedClipboardCopy1.Result))
            {
                MessageBoxes.NothingToCopy();
            }
            else
            {
                try
                {
                    Clipboard.SetText(advancedClipboardCopy1.Result);
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}