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
    public sealed class TwitterStatusButton : ToolStripStatusLabel
    {
        public TwitterStatusButton()
        {
            Image = Properties.Resources.twitterButton;
            DisplayStyle = ToolStripItemDisplayStyle.Image;
            Click += FacebookStatusButton_Click;
            IsLink = true;

            Padding = new Padding(2, 0, 2, 0);
        }

        /// <summary>
        ///     Overwrite default message text. If set ignores TargetSite.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string MessageText { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TargetSite { get; set; }

        private void FacebookStatusButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageText))
            {
                PremadeDialogs.StartProcessSafely(@"https://twitter.com/intent/tweet?text=" +
                                                MessageText.Replace(' ', '+'));
            }
            else if (!string.IsNullOrEmpty(TargetSite))
            {
                PremadeDialogs.StartProcessSafely(@"https://twitter.com/intent/tweet?text=" +
                                                "Check+out+this+cool+app+I+found!+" + TargetSite);
            }
            else
            {
                throw new InvalidOperationException("TargetSite and MessageText are both null or empty");
            }
        }
    }
}