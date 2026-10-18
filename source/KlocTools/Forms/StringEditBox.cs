/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using Klocman.Tools;

namespace Klocman.Forms
{
    public partial class StringEditBox : Form
    {
        public StringEditBox()
        {
            InitializeComponent();
            ModernStyleController.ApplyOnLoad(this);
        }

        public static bool ShowDialog(string description, string title, string defaultText,
            string acceptButton, string cancelButton, out string result)
        {
            using (var instance = new StringEditBox())
            {
                instance.button1.Text = acceptButton;
                instance.button2.Text = cancelButton;
                instance.label1.Text = description;
                instance.Text = title;
                instance.textBox1.Text = defaultText;

                instance.textBox1.Focus();
                var outVar = instance.ShowDialog();
                result = instance.textBox1.Text;
                return outVar != DialogResult.Cancel;
            }
        }

        private void StringEditBox_Shown(object sender, EventArgs e)
        {
            if (ParentForm != null && ParentForm.Icon != null)
                Icon = ParentForm.Icon;
            else
            {
                try
                {
                    Icon = ProcessTools.GetIconFromEntryExe();
                }
                catch
                {
                    /* Fall back to the default icon */
                }
            }
        }
    }
}