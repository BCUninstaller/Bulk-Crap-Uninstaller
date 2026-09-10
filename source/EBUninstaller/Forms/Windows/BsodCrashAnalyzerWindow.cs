/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    BSOD BugCheck & Kernel Crash Dump Analyzer Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class BsodCrashAnalyzerWindow : Form
    {
        private FastObjectListView _folvBsod;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<BsodCrashRecord> _records = new();

        public BsodCrashAnalyzerWindow()
        {
            InitializeComponent();
            LoadDumps();
        }

        private void InitializeComponent()
        {
            Text = "BSOD BugCheck & Kernel Crash Dump Analyzer - EBUninstaller Pro";
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

            _folvBsod = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colTime = new OLVColumn("Crash Timestamp", nameof(BsodCrashRecord.CrashTime))
            {
                Width = 140,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss")
            };
            var colCode = new OLVColumn("Bugcheck Code", nameof(BsodCrashRecord.BugcheckCode)) { Width = 110 };
            var colName = new OLVColumn("Bugcheck Name", nameof(BsodCrashRecord.BugcheckName)) { Width = 190 };
            var colDriver = new OLVColumn("Faulting Driver", nameof(BsodCrashRecord.FaultingDriver)) { Width = 130 };
            var colVendor = new OLVColumn("Vendor / Subsystem", nameof(BsodCrashRecord.AssociatedVendor)) { Width = 150 };
            var colRec = new OLVColumn("Diagnostic Advice", nameof(BsodCrashRecord.Recommendation)) { Width = 250, FillsFreeSpace = true };

            _folvBsod.AllColumns.AddRange(new[] { colTime, colCode, colName, colDriver, colVendor, colRec });
            _folvBsod.RebuildColumns();

            mainLayout.Controls.Add(_folvBsod, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Minidump directory and System BugCheck event logs...",
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
            _btnRefresh.Click += (s, e) => LoadDumps();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void LoadDumps()
        {
            _records = BsodCrashDumpAnalyzerEngine.ScanCrashDumps();
            _folvBsod.SetObjects(_records);

            if (_records.Count == 0)
            {
                _lblSummary.Text = "No recent Blue Screen (BSOD) minidump crash records detected. System kernel is operating stably.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {_records.Count} kernel crash dump / bugcheck event(s). Review faulting drivers above.";
                _lblSummary.ForeColor = Color.DarkRed;
            }
        }
    }
}
