/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using UninstallTools;

namespace BulkCrapUninstaller.Controls
{
    public partial class AdvancedClipboardCopy : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ApplicationUninstallerEntry> Targets { get; set; }

        public AdvancedClipboardCopy()
        {
            InitializeComponent();

            comboBoxInsert.Items.AddRange(ClipboardCopyItem.Items.Cast<object>().ToArray());
            comboBoxInsert.SelectedIndex = 0;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PatternText
        {
            get { return textBoxPatternInput.Text; }
            set { textBoxPatternInput.Text = value; }
        }

        public string Result => textBoxResults.Text;

        private void RefreshResult(object sender, EventArgs e)
        {
            try
            {
                var pattern = checkBoxUnescape.Checked
                    ? Regex.Unescape(textBoxPatternInput.Text)
                    : textBoxPatternInput.Text;
                textBoxResults.Text = string.Join(Environment.NewLine,
                    Targets.Select(x => ClipboardCopyItem.GetStringFromPattern(pattern, x))
                        .ToArray());
            }
            catch (Exception ex)
            {
                textBoxResults.Text = ex.Message;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxInsert.SelectedIndex == 0) return;
            if (comboBoxInsert.SelectedItem is ClipboardCopyItem x)
                textBoxPatternInput.SelectedText = x.Name;
            comboBoxInsert.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RefreshResult(sender, e);
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxes.DisplayHelp();
        }
    }
}