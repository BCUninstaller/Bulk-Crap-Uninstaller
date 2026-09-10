/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    System Tray Notification Area (TrayNotify) Cache Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class TrayNotifyCleanerWindow : Form
    {
        private Label _lblStatus;
        private Label _lblSize;
        private Button _btnReset;
        private Button _btnRefresh;
        private Button _btnClose;
        private TrayNotifyStatus _status;

        public TrayNotifyCleanerWindow()
        {
            InitializeComponent();
            RefreshStatus();
        }

        private void InitializeComponent()
        {
            Text = "Taskbar Notification Area (TrayNotify) Icon Cache Cleaner - EBUninstaller Pro";
            Size = new Size(800, 340);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Notification Area (System Tray) Icon History Reset",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Windows Explorer retains past system tray icons for years even after the associated programs have been\ndeleted. This can cause obsolete application icons to appear in Taskbar settings and slow down tray loading.\nResetting clears phantom icon history and restarts Windows Explorer cleanly.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Tray Notification Cache Details",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblStatus = new Label { Location = new Point(16, 30), AutoSize = true, Text = "Cache Streams: -" };
            _lblSize = new Label { Location = new Point(16, 62), AutoSize = true, Text = "Cache Size: -" };

            group.Controls.AddRange(new Control[] { _lblStatus, _lblSize });

            _btnReset = new Button
            {
                Text = "Purge Tray Icon History & Restart Explorer",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(270, 240),
                Size = new Size(280, 32)
            };
            _btnReset.Click += OnResetClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(560, 240),
                Size = new Size(100, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshStatus();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(670, 240),
                Size = new Size(86, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _btnReset, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshStatus()
        {
            _status = TrayNotifyResidualsCleanerEngine.QueryTrayNotifyStatus();

            _lblStatus.Text = $"Active Icon Streams: {(_status.HasIconStreams ? "Present" : "None")}, Past Icon Streams: {(_status.HasPastIconsStream ? "Present" : "None")}";
            _lblSize.Text = $"Total Stream Data: {FormatBytes(_status.TotalStreamSizeBytes)}";
        }

        private void OnResetClick(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Windows Explorer will momentarily restart to clear all phantom and residual system tray icons.\n\nDo you want to continue?",
                "Confirm Tray Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var success = TrayNotifyResidualsCleanerEngine.ResetTrayNotifyCache();
                if (success)
                {
                    MessageBox.Show("Notification area icon cache reset successfully. Windows Explorer restarted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshStatus();
                }
                else
                {
                    MessageBox.Show("Failed resetting notification cache.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            return $"{bytes / 1024.0:F1} KB";
        }
    }
}
