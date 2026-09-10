/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DCOM & Component Services Orphaned Registration Cleaner Window
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
    public sealed class DcomOrphanCleanerWindow : Form
    {
        private FastObjectListView _folvDcom;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<DcomOrphanItem> _items = new();

        public DcomOrphanCleanerWindow()
        {
            InitializeComponent();
            ScanDcom();
        }

        private void InitializeComponent()
        {
            Text = "DCOM & Component Services Orphaned Registration Cleaner - EBUninstaller Pro";
            Size = new Size(980, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(750, 420);
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

            _folvDcom = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colAppId = new OLVColumn("AppID (GUID)", nameof(DcomOrphanItem.AppId)) { Width = 230 };
            var colName = new OLVColumn("COM Application Name", nameof(DcomOrphanItem.DisplayName)) { Width = 200 };
            var colServer = new OLVColumn("Missing Server Executable / DLL", nameof(DcomOrphanItem.LocalServer32Path)) { Width = 300 };
            var colReason = new OLVColumn("Issue Reason", nameof(DcomOrphanItem.IssueReason)) { Width = 220, FillsFreeSpace = true };

            _folvDcom.AllColumns.AddRange(new[] { colAppId, colName, colServer, colReason });
            _folvDcom.RebuildColumns();

            mainLayout.Controls.Add(_folvDcom, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Component Services / DCOM AppID registrations...",
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
            _btnRefresh.Click += (s, e) => ScanDcom();

            _btnClean = new Button
            {
                Text = "Clean Selected DCOM Orphans",
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

        private void ScanDcom()
        {
            _items = DcomPermissionsOrphanCleanerEngine.ScanOrphanedDcomRegistrations();
            _folvDcom.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No orphaned DCOM or Component Services registrations detected. COM subsystem is clean.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} orphaned DCOM AppID registration(s) pointing to missing binary files.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvDcom.SelectedObjects.Count > 0 ? _folvDcom.SelectedObjects.Cast<DcomOrphanItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to clean {selected.Count} orphaned DCOM AppID entries?",
                "Confirm DCOM Cleanup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = DcomPermissionsOrphanCleanerEngine.CleanOrphanedDcomItems(selected);
                MessageBox.Show($"Cleaned {cleaned} orphaned DCOM registration(s) successfully.", "DCOM Cleaned", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanDcom();
            }
        }
    }
}
