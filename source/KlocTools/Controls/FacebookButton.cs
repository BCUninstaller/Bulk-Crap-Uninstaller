/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Forms.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public sealed class FacebookButton : Button
    {
        public FacebookButton()
        {
            Text = string.Empty;
            BackgroundImage = Properties.Resources.facebookButton;
            BackgroundImageLayout = ImageLayout.Stretch;

            Size = new Size(23, 23);
            base.FlatStyle = FlatStyle.Standard;

            TabStop = false;

            Click += FacebookButton_Click;
        }

        public new FlatStyle FlatStyle => base.FlatStyle;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TargetSite { get; set; }

        private void FacebookButton_Click(object sender, EventArgs e)
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