/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    BranchCache & Peer Distribution Cache Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class BranchCacheCleanerWindow : Form
    {
        private Label _lblFiles;
        private Label _lblSize;
        private Label _lblStatus;
        private Button _btnFlush;
        private Button _btnRefresh;
        private Button _btnClose;
        private BranchCacheStats _stats;

        public BranchCacheCleanerWindow()
        {
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            Text = "BranchCache & Peer Distribution Cache Cleaner - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows BranchCache Local Network Peer Cache Management",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "BranchCache stores downloaded corporate packages, software distributions, and peer-to-peer\npublication caches on disk. Over time, obsolete deployment packages remain stored in the cache.\nFlushing purges stale peer distributions and reclaims local drive space.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "BranchCache Storage Details",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblFiles = new Label { Location = new Point(16, 30), AutoSize = true, Text = "Cached Publication Files: -" };
            _lblSize = new Label { Location = new Point(16, 62), AutoSize = true, Text = "Total Cache Size: -" };

            group.Controls.AddRange(new Control[] { _lblFiles, _lblSize });

            _lblStatus = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Status: Ready"
            };

            _btnFlush = new Button
            {
                Text = "Flush BranchCache",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(460, 265),
                Size = new Size(160, 32)
            };
            _btnFlush.Click += OnFlushClick;

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

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblStatus, _btnFlush, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshStats()
        {
            _stats = BranchCacheCleanerEngine.ScanBranchCache();
            _lblFiles.Text = $"Cached Distribution Files: {_stats.FileCount} files across {_stats.CacheDirectories.Count} store(s)";
            _lblSize.Text = $"Total Storage Consumed: {FormatBytes(_stats.TotalSizeBytes)}";
            _lblStatus.Text = _stats.FileCount > 0 ? "BranchCache publication data detected." : "BranchCache is clean.";
            _lblStatus.ForeColor = _stats.FileCount > 0 ? Color.DarkOrange : Color.DarkGreen;
        }

        private void OnFlushClick(object sender, EventArgs e)
        {
            var (files, bytes) = BranchCacheCleanerEngine.FlushBranchCache();
            MessageBox.Show($"Flushed BranchCache ({files} files purged, Freed {FormatBytes(bytes)}).", "BranchCache Flushed", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
