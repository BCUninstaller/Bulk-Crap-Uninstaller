/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Storage Drive TRIM & Media Optimization Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class DriveOptimizationWindow : Form
    {
        private FastObjectListView _folvDrives;
        private Label _lblSummary;
        private Button _btnOptimize;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<StorageDriveInfo> _drives = new();

        public DriveOptimizationWindow()
        {
            InitializeComponent();
            LoadDrives();
        }

        private void InitializeComponent()
        {
            Text = "Storage Drive TRIM & Media Optimizer - EBUninstaller Pro";
            Size = new Size(950, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 380);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            _folvDrives = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colDrive = new OLVColumn("Drive", nameof(StorageDriveInfo.DriveLetter)) { Width = 70 };
            var colLabel = new OLVColumn("Volume Label", nameof(StorageDriveInfo.VolumeLabel)) { Width = 140 };
            var colMedia = new OLVColumn("Media Type", nameof(StorageDriveInfo.MediaType)) { Width = 100 };
            var colFS = new OLVColumn("File System", nameof(StorageDriveInfo.FileSystem)) { Width = 90 };
            var colFree = new OLVColumn("Free Space", nameof(StorageDriveInfo.FreeSizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colTotal = new OLVColumn("Total Size", nameof(StorageDriveInfo.TotalSizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colFreePct = new OLVColumn("Free %", nameof(StorageDriveInfo.FreePercent))
            {
                Width = 80,
                AspectToStringConverter = v => $"{v:F1}%"
            };
            var colAction = new OLVColumn("Recommended Action", nameof(StorageDriveInfo.RecommendedAction)) { Width = 230, FillsFreeSpace = true };

            _folvDrives.AllColumns.AddRange(new[] { colDrive, colLabel, colMedia, colFS, colFree, colTotal, colFreePct, colAction });
            _folvDrives.RebuildColumns();
            _folvDrives.SelectionChanged += (s, e) => _btnOptimize.Enabled = _folvDrives.SelectedObject != null;

            mainLayout.Controls.Add(_folvDrives, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing storage drive media types, free space, and TRIM capability...",
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 8),
                Font = new Font(Font, FontStyle.Bold)
            };
            mainLayout.Controls.Add(_lblSummary, 0, 1);

            // Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK, AutoSize = true };
            _btnRefresh = new Button { Text = "Refresh Drives", AutoSize = true };
            _btnRefresh.Click += (s, e) => LoadDrives();

            _btnOptimize = new Button
            {
                Text = "Optimize Selected Drive",
                AutoSize = true,
                Enabled = false,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnOptimize.Click += OnOptimizeClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnOptimize);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void LoadDrives()
        {
            _drives = DriveOptimizationEngine.GetDrives();
            _folvDrives.SetObjects(_drives);

            var totalDrives = _drives.Count;
            var ssdCount = _drives.Count(d => d.MediaType is DriveMediaType.SSD or DriveMediaType.NVMe);
            var totalFree = _drives.Sum(d => d.FreeSizeBytes);

            _lblSummary.Text = $"Detected {totalDrives} active volume(s) ({ssdCount} SSD/NVMe). Total available free storage: {FormatBytes(totalFree)}.";
            _lblSummary.ForeColor = Color.DarkGreen;

            if (_drives.Count > 0)
            {
                _folvDrives.SelectObject(_drives[0]);
            }
        }

        private void OnOptimizeClick(object sender, EventArgs e)
        {
            var selected = _folvDrives.SelectedObject as StorageDriveInfo;
            if (selected == null) return;

            var confirm = MessageBox.Show(
                $"Do you want to run storage optimization ({selected.RecommendedAction}) on {selected.DriveLetter} ({selected.VolumeLabel})?",
                "Optimize Drive",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                Cursor = Cursors.WaitCursor;
                _btnOptimize.Enabled = false;

                try
                {
                    var success = DriveOptimizationEngine.OptimizeDrive(selected.DriveLetter);
                    if (success)
                    {
                        MessageBox.Show($"Optimization for drive {selected.DriveLetter} completed successfully!", "Optimization Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Drive optimization completed with exit status or notifications.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                    _btnOptimize.Enabled = true;
                    LoadDrives();
                }
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }
    }
}
