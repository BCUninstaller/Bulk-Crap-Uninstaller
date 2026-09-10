/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Reliability & Crash History Window
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
    public sealed class SoftwareCrashHistoryWindow : Form
    {
        private FastObjectListView _folvStability;
        private FastObjectListView _folvEvents;
        private SplitContainer _splitDetails;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<SoftwareCrashRecord> _allRecords = new();
        private List<AppStabilitySummary> _stabilitySummaries = new();

        public SoftwareCrashHistoryWindow()
        {
            InitializeComponent();
            LoadCrashHistory();
        }

        private void InitializeComponent()
        {
            Text = "Software Reliability & Crash History Monitor - EBUninstaller Pro";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 480);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // SplitContainer
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary Label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons Panel

            _splitDetails = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 260
            };

            // Top List: App Stability Overview
            _folvStability = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colAppName = new OLVColumn("Application", nameof(AppStabilitySummary.ApplicationName)) { Width = 250 };
            var colCrashes = new OLVColumn("Crashes", nameof(AppStabilitySummary.TotalCrashes)) { Width = 80 };
            var colHangs = new OLVColumn("Hangs", nameof(AppStabilitySummary.TotalHangs)) { Width = 80 };
            var colScore = new OLVColumn("Stability Score", nameof(AppStabilitySummary.StabilityScore))
            {
                Width = 110,
                AspectToStringConverter = v => $"{v}%"
            };
            var colStatus = new OLVColumn("Health Status", nameof(AppStabilitySummary.HealthStatus)) { Width = 110 };
            var colModule = new OLVColumn("Faulting Module", nameof(AppStabilitySummary.PrimaryFaultingModule)) { Width = 180 };
            var colLastCrash = new OLVColumn("Last Incident", nameof(AppStabilitySummary.LastCrashDate))
            {
                Width = 140,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };

            _folvStability.AllColumns.AddRange(new[] { colAppName, colScore, colStatus, colCrashes, colHangs, colModule, colLastCrash });
            _folvStability.RebuildColumns();
            _folvStability.SelectionChanged += OnStabilitySelectionChanged;

            _splitDetails.Panel1.Controls.Add(_folvStability);

            // Bottom List: Detailed Crash Events for Selected App
            _folvEvents = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colEvTime = new OLVColumn("Timestamp", nameof(SoftwareCrashRecord.Timestamp))
            {
                Width = 140,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss")
            };
            var colEvType = new OLVColumn("Incident Type", nameof(SoftwareCrashRecord.EventType)) { Width = 100 };
            var colEvModule = new OLVColumn("Faulting Module", nameof(SoftwareCrashRecord.FaultingModule)) { Width = 180 };
            var colEvCode = new OLVColumn("Exception Code", nameof(SoftwareCrashRecord.ExceptionCode)) { Width = 120 };
            var colEvVer = new OLVColumn("App Version", nameof(SoftwareCrashRecord.ApplicationVersion)) { Width = 110 };
            var colEvRaw = new OLVColumn("Details / Log Message", nameof(SoftwareCrashRecord.RawMessage)) { Width = 300, FillsFreeSpace = true };

            _folvEvents.AllColumns.AddRange(new[] { colEvTime, colEvType, colEvModule, colEvCode, colEvVer, colEvRaw });
            _folvEvents.RebuildColumns();

            _splitDetails.Panel2.Controls.Add(_folvEvents);
            mainLayout.Controls.Add(_splitDetails, 0, 0);

            // Summary Label
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Event Logs for application crashes and hangs...",
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 8),
                Font = new Font(Font, FontStyle.Bold)
            };
            mainLayout.Controls.Add(_lblSummary, 0, 1);

            // Buttons Panel
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK, AutoSize = true };
            _btnRefresh = new Button { Text = "Refresh Scan", AutoSize = true };
            _btnRefresh.Click += (s, e) => LoadCrashHistory();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void LoadCrashHistory()
        {
            _allRecords = SoftwareCrashHistoryEngine.ScanCrashHistory(90);
            _stabilitySummaries = SoftwareCrashHistoryEngine.GenerateStabilitySummaries(_allRecords);

            _folvStability.SetObjects(_stabilitySummaries);

            var totalIncidents = _allRecords.Count;
            var crashCount = _allRecords.Count(x => x.EventType.Equals("Crash", StringComparison.OrdinalIgnoreCase));
            var hangCount = _allRecords.Count(x => x.EventType.Equals("Hang", StringComparison.OrdinalIgnoreCase));
            var unstableApps = _stabilitySummaries.Count(x => x.StabilityScore < 80);

            _lblSummary.Text = $"Found {totalIncidents} application incidents ({crashCount} crashes, {hangCount} hangs) across {_stabilitySummaries.Count} applications. Unstable apps: {unstableApps}";
            _lblSummary.ForeColor = unstableApps > 0 ? Color.DarkOrange : Color.DarkGreen;

            if (_stabilitySummaries.Count > 0)
            {
                _folvStability.SelectObject(_stabilitySummaries[0]);
            }
            else
            {
                _folvEvents.SetObjects(new List<SoftwareCrashRecord>());
            }
        }

        private void OnStabilitySelectionChanged(object sender, EventArgs e)
        {
            var selected = _folvStability.SelectedObject as AppStabilitySummary;
            if (selected == null)
            {
                _folvEvents.SetObjects(new List<SoftwareCrashRecord>());
                return;
            }

            var matchingEvents = _allRecords
                .Where(r => r.ApplicationName.Equals(selected.ApplicationName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.Timestamp)
                .ToList();

            _folvEvents.SetObjects(matchingEvents);
        }
    }
}
