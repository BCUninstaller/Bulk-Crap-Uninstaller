/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Forms
{
    public partial class FeedbackBox : Form
    {
        public static void ShowFeedbackBox(Form parent, bool showDisableCheckbox)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            using (var f = new FeedbackBox())
            {
                f.checkBoxNeverShow.Visible = showDisableCheckbox;
                f.checkBoxNeverShow.Enabled = showDisableCheckbox;

                f.Icon = parent.Icon;
                f.Owner = parent;
                f.StartPosition = FormStartPosition.CenterParent;

                f.ShowDialog(parent);
            }
        }

        private FeedbackBox()
        {
            InitializeComponent();
            ModernStyleController.ApplyOnLoad(this);

            if (DesignMode) return;

            Settings.Default.SettingBinder.BindControl(checkBoxNeverShow, x => x.MiscFeedbackNagNeverShow, this);
            Settings.Default.SettingBinder.SendUpdates(this);
        }

        private void buttonSendFeedback2_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.SubmitFeedbackLink) });
        }

        private void buttonRate_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.ReviewLink) });
        }

        private void buttonTwitter_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.TwitterLink) });
        }

        private void buttonSubmitGithub_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.GithubNewIssueLink) });
        }

        private void buttonIssues_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.GithubIssuesLink) });
        }

        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.TranslateLink) });
        }

        private void buttonDonate_Click(object sender, EventArgs e)
        {
            MainWindow.OpenUrls(new[] { new Uri(Resources.DonateLink) });
        }
    }
}
