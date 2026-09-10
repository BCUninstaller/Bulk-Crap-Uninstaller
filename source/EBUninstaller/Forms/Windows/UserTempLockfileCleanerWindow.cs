/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Stale User Temp Lockfiles Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class UserTempLockfileCleanerWindow : Form
    {
        private FastObjectListView _folvLocks;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<StaleLockfileItem> _items = new();

        public UserTempLockfileCleanerWindow()
        {
            InitializeComponent();
            ScanLocks();
        }

        private void InitializeComponent()
        {
            Text = "Stale Temp Lockfiles & Abandoned IPC Sockets Cleaner - EBUninstaller Pro";
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

            _folvLocks = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colFile = new OLVColumn("Artifact File", nameof(StaleLockfileItem.FileName)) { Width = 220 };
            var colType = new OLVColumn("Artifact Type", nameof(StaleLockfileItem.LockType)) { Width = 180 };
            var colSize = new OLVColumn("Size", nameof(StaleLockfileItem.SizeBytes))
            {
                Width = 100,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colDate = new OLVColumn("Modified", nameof(StaleLockfileItem.LastModified))
            {
                Width = 130,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };
            var colPath = new OLVColumn("Temp Directory Location", nameof(StaleLockfileItem.FilePath)) { Width = 300, FillsFreeSpace = true };

            _folvLocks.AllColumns.AddRange(new[] { colFile, colType, colSize, colDate, colPath });
            _folvLocks.RebuildColumns();

            mainLayout.Controls.Add(_folvLocks, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning user temporary directories for dead lockfiles and stale IPC artifacts...",
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
            _btnRefresh = new Button { Text = "Refresh Scan", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanLocks();

            _btnClean = new Button
            {
                Text = "Purge Dead Lockfiles",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnClean.Click += OnCleanClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnClean);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanLocks()
        {
            _items = UserTempLockfileCleanerEngine.ScanStaleLockfiles();
            _folvLocks.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No stale process lockfiles or dead IPC sockets detected. Temp folder is clean.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} abandoned lockfile(s) and dead IPC artifact(s) from terminated applications.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvLocks.SelectedObjects.Count > 0 ? _folvLocks.SelectedObjects.Cast<StaleLockfileItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Delete {selected.Count} dead lockfile artifact(s)?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cleaned, freed) = UserTempLockfileCleanerEngine.PurgeLockfiles(selected);
                MessageBox.Show($"Purged {cleaned} dead lockfile(s) (Freed {FormatBytes(freed)}).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanLocks();
            }
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
