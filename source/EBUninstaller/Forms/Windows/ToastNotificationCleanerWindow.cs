/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Toast Notification History & Action Center Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class ToastNotificationCleanerWindow : Form
    {
        private Label _lblDbSize;
        private Label _lblLogos;
        private Label _lblStatus;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private NotificationStoreStats _stats;

        public ToastNotificationCleanerWindow()
        {
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            Text = "Windows Action Center & Toast Notification Cache Cleaner - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Action Center (wpndatabase.db) Residuals & Logo Cache",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Windows stores toast notifications, push token banners, and application logos in an SQLite database.\nEven after applications are uninstalled, residual notification history and cached app banners\nremain in the Action Center. Purging cleans obsolete banners and releases database space.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Notification Repository Details",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblDbSize = new Label { Location = new Point(16, 30), AutoSize = true, Text = "Notification Database Size: -" };
            _lblLogos = new Label { Location = new Point(16, 62), AutoSize = true, Text = "Cached Application Logos: -" };

            group.Controls.AddRange(new Control[] { _lblDbSize, _lblLogos });

            _lblStatus = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Status: Ready"
            };

            _btnClean = new Button
            {
                Text = "Purge Notification Cache",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(440, 265),
                Size = new Size(180, 32)
            };
            _btnClean.Click += OnCleanClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(630, 265),
                Size = new Size(70, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshStats();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(710, 265),
                Size = new Size(75, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblStatus, _btnClean, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshStats()
        {
            _stats = ToastNotificationHistoryCleanerEngine.QueryNotificationStore();
            _lblDbSize.Text = $"Notification Database: {FormatBytes(_stats.DatabaseSizeBytes)} ({_stats.DatabasePath})";
            _lblLogos.Text = $"Cached App Logos: {_stats.CachedLogoCount} files ({FormatBytes(_stats.CachedLogosSizeBytes)})";
            _lblStatus.Text = $"Total Notification Storage: {FormatBytes(_stats.DatabaseSizeBytes + _stats.CachedLogosSizeBytes)}";
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var (files, bytes) = ToastNotificationHistoryCleanerEngine.PurgeNotificationHistory();
            MessageBox.Show($"Purged {files} notification logo and journal file(s) (Freed {FormatBytes(bytes)}).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshStats();
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }
    }
}
