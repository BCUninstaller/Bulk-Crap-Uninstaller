/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    BITS Queue Residuals & Stuck Downloads Cleaner Window
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
    public sealed class BitsQueueCleanerWindow : Form
    {
        private FastObjectListView _folvJobs;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<BitsJobItem> _items = new();

        public BitsQueueCleanerWindow()
        {
            InitializeComponent();
            ScanBits();
        }

        private void InitializeComponent()
        {
            Text = "Background Intelligent Transfer (BITS) Queue Cleaner - EBUninstaller Pro";
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

            _folvJobs = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colName = new OLVColumn("Job Display Name", nameof(BitsJobItem.DisplayName)) { Width = 230 };
            var colState = new OLVColumn("State", nameof(BitsJobItem.State)) { Width = 110 };
            var colOwner = new OLVColumn("Owner / User", nameof(BitsJobItem.Owner)) { Width = 140 };
            var colGuid = new OLVColumn("Job GUID", nameof(BitsJobItem.JobId)) { Width = 200 };
            var colTarget = new OLVColumn("Target / URL", nameof(BitsJobItem.TargetFile)) { Width = 230, FillsFreeSpace = true };

            _folvJobs.AllColumns.AddRange(new[] { colName, colState, colOwner, colGuid, colTarget });
            _folvJobs.RebuildColumns();

            mainLayout.Controls.Add(_folvJobs, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Background Intelligent Transfer Service (BITS) download queue...",
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
            _btnRefresh = new Button { Text = "Refresh Queue", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanBits();

            _btnClean = new Button
            {
                Text = "Cancel & Purge Stuck BITS Jobs",
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

        private void ScanBits()
        {
            _items = BitsQueueResidualsCleanerEngine.ScanBitsJobs();
            _folvJobs.SetObjects(_items);

            var suspendedCount = _items.Count(j => j.State.Contains("SUSPENDED", StringComparison.OrdinalIgnoreCase) || j.State.Contains("ERROR", StringComparison.OrdinalIgnoreCase));

            if (_items.Count == 0)
            {
                _lblSummary.Text = "BITS download queue is clear. No active or stuck background transfer jobs.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Found {_items.Count} BITS job(s) in queue ({suspendedCount} suspended/failed).";
                _lblSummary.ForeColor = suspendedCount > 0 ? Color.DarkOrange : Color.DarkGreen;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvJobs.SelectedObjects.Count > 0 ? _folvJobs.SelectedObjects.Cast<BitsJobItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Cancel {selected.Count} background transfer job(s) and reset BITS cache?",
                "Confirm BITS Queue Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cancelled, reset) = BitsQueueResidualsCleanerEngine.PurgeBitsQueue(selected);
                MessageBox.Show($"Cancelled {cancelled} BITS job(s). Queue state refreshed.", "BITS Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanBits();
            }
        }
    }
}
