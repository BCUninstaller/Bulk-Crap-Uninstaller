/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Shell OpenWith & File Association Orphan Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class OpenWithCleanerWindow : Form
    {
        private FastObjectListView _folvApps;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<OpenWithOrphanItem> _items = new();

        public OpenWithCleanerWindow()
        {
            InitializeComponent();
            ScanOpenWith();
        }

        private void InitializeComponent()
        {
            Text = "Shell OpenWith & File Association Orphan Cleaner - EBUninstaller Pro";
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

            _folvApps = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colExe = new OLVColumn("Application Target", nameof(OpenWithOrphanItem.ApplicationExe)) { Width = 180 };
            var colMissing = new OLVColumn("Non-Existent Target Path", nameof(OpenWithOrphanItem.MissingPath)) { Width = 320 };
            var colReg = new OLVColumn("Registry Key", nameof(OpenWithOrphanItem.RegistryLocation)) { Width = 230 };
            var colReason = new OLVColumn("Issue Reason", nameof(OpenWithOrphanItem.IssueReason)) { Width = 200, FillsFreeSpace = true };

            _folvApps.AllColumns.AddRange(new[] { colExe, colMissing, colReg, colReason });
            _folvApps.RebuildColumns();

            mainLayout.Controls.Add(_folvApps, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows OpenWith handlers and file associations...",
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
            _btnRefresh.Click += (s, e) => ScanOpenWith();

            _btnClean = new Button
            {
                Text = "Clean Selected OpenWith Orphans",
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

        private void ScanOpenWith()
        {
            _items = OpenWithResidualsCleanerEngine.ScanOpenWithOrphans();
            _folvApps.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No orphaned OpenWith handlers found. All file associations point to existing applications.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} orphaned OpenWith handler(s) pointing to deleted application executables.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvApps.SelectedObjects.Count > 0 ? _folvApps.SelectedObjects.Cast<OpenWithOrphanItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Remove {selected.Count} orphaned OpenWith registration(s)?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = OpenWithResidualsCleanerEngine.CleanOpenWithItems(selected);
                MessageBox.Show($"Removed {cleaned} orphaned OpenWith registration(s) successfully.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanOpenWith();
            }
        }
    }
}
