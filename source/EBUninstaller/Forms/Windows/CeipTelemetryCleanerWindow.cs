/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    CEIP & SQM Telemetry Residuals Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class CeipTelemetryCleanerWindow : Form
    {
        private Label _lblFiles;
        private Label _lblSize;
        private Label _lblStatus;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private CeipStoreStats _stats;

        public CeipTelemetryCleanerWindow()
        {
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            Text = "Customer Experience (CEIP) & SQM Telemetry Cleaner - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows SQM & Customer Experience Improvement Program Residuals",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Windows and third-party software write Software Quality Metrics (SQM) telemetry files\nand Customer Experience reports tracking application sessions, crashes, and performance.\nPurging eliminates lingering telemetry traces and cleans local SQM caches.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Telemetry Cache Statistics",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblFiles = new Label { Location = new Point(16, 30), AutoSize = true, Text = "Telemetry Files Found: -" };
            _lblSize = new Label { Location = new Point(16, 62), AutoSize = true, Text = "Total Cache Size: -" };

            group.Controls.AddRange(new Control[] { _lblFiles, _lblSize });

            _lblStatus = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Status: Ready"
            };

            _btnClean = new Button
            {
                Text = "Purge CEIP & SQM Telemetry",
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
            _stats = CeipTelemetryCleanerEngine.ScanCeipTelemetry();
            _lblFiles.Text = $"Telemetry Files: {_stats.TotalSqmFiles} file(s) across {_stats.ScannedLocations.Count} location(s)";
            _lblSize.Text = $"Cache Space Used: {FormatBytes(_stats.TotalSizeBytes)}";
            _lblStatus.Text = _stats.TotalSqmFiles > 0 ? "Telemetry traces detected on disk." : "CEIP/SQM telemetry folders are clean.";
            _lblStatus.ForeColor = _stats.TotalSqmFiles > 0 ? Color.DarkOrange : Color.DarkGreen;
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var (files, bytes) = CeipTelemetryCleanerEngine.PurgeCeipTelemetry();
            MessageBox.Show($"Purged {files} telemetry file(s) (Freed {FormatBytes(bytes)}).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
