/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Virtual Memory Pagefile Diagnostics Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class PagingFileDiagnosticsWindow : Form
    {
        private Label _lblConfig;
        private Label _lblDiskSize;
        private Label _lblStatus;
        private CheckBox _cbClearShutdown;
        private Button _btnApply;
        private Button _btnRefresh;
        private Button _btnClose;
        private PagefileConfigInfo _info;

        public PagingFileDiagnosticsWindow()
        {
            InitializeComponent();
            RefreshData();
        }

        private void InitializeComponent()
        {
            Text = "Virtual Memory Pagefile Diagnostics & Hardening - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Virtual Memory (pagefile.sys) Security & Storage Management",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "The Windows paging file holds memory swapped from physical RAM. Sensitive credentials,\napplication keys, and uncompressed data may remain stored in pagefile.sys across restarts.\nEnabling pagefile wiping clears virtual memory on shutdown to safeguard privacy.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Paging File State",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblConfig = new Label { Location = new Point(16, 26), AutoSize = true, Text = "Paging Files: -" };
            _lblDiskSize = new Label { Location = new Point(16, 52), AutoSize = true, Text = "Pagefile on Disk: -" };
            _cbClearShutdown = new CheckBox
            {
                Text = "Securely wipe pagefile virtual memory on system shutdown (ClearPageFileAtShutdown)",
                Location = new Point(16, 78),
                AutoSize = true
            };

            group.Controls.AddRange(new Control[] { _lblConfig, _lblDiskSize, _cbClearShutdown });

            _lblStatus = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Status: Ready"
            };

            _btnApply = new Button
            {
                Text = "Apply Setting",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(480, 265),
                Size = new Size(130, 32)
            };
            _btnApply.Click += OnApplyClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(620, 265),
                Size = new Size(80, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshData();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(710, 265),
                Size = new Size(75, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblStatus, _btnApply, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshData()
        {
            _info = PagingFileDiagnosticsEngine.QueryPagefileConfig();
            _lblConfig.Text = $"Configured Paging Location: {_info.PagingFilesConfig}";
            _lblDiskSize.Text = $"Pagefile on Boot Drive: {(_info.PagefileExistsOnDisk ? FormatBytes(_info.MainDrivePagefileSizeBytes) : "None / Dynamic Allocation")}";
            _cbClearShutdown.Checked = _info.ClearPagefileOnShutdown;
            _lblStatus.Text = $"Shutdown Wipe: {(_info.ClearPagefileOnShutdown ? "Enabled (Hardened)" : "Disabled (Standard)")}";
            _lblStatus.ForeColor = _info.ClearPagefileOnShutdown ? Color.DarkGreen : Color.DarkSlateBlue;
        }

        private void OnApplyClick(object sender, EventArgs e)
        {
            var success = PagingFileDiagnosticsEngine.SetClearPagefileOnShutdown(_cbClearShutdown.Checked);
            if (success)
            {
                MessageBox.Show("Pagefile shutdown security policy updated successfully.", "Setting Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
            }
            else
            {
                MessageBox.Show("Failed updating Pagefile policy. Administrator privilege required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F2} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }
    }
}
