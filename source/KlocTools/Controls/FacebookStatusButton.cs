/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Klocman.Forms.Tools;

namespace Klocman.Controls
{
    public sealed class FacebookStatusButton : ToolStripStatusLabel
    {
        public FacebookStatusButton()
        {
            Image = Properties.Resources.facebookButton;
            DisplayStyle = ToolStripItemDisplayStyle.Image;
            Click += FacebookStatusButton_Click;
            IsLink = true;

            Padding = new Padding(2, 0, 2, 0);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TargetSite { get; set; }

        private void FacebookStatusButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TargetSite))
            {
                PremadeDialogs.StartProcessSafely(@"https://www.facebook.com/sharer/sharer.php?u=" + TargetSite);
            }
            else
            {
                throw new InvalidOperationException("TargetSite is null or empty");
            }
        }
    }
}