/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Power, Wake Lock & Battery Impact Profiler Window
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
    public sealed class SoftwarePowerImpactWindow : Form
    {
        private FastObjectListView _folvPower;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<SoftwarePowerRequestItem> _powerRequests = new();

        public SoftwarePowerImpactWindow()
        {
            InitializeComponent();
            LoadPowerProfile();
        }

        private void InitializeComponent()
        {
            Text = "Software Power & Battery Drain Profiler - EBUninstaller Pro";
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

            _folvPower = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colType = new OLVColumn("Request Type", nameof(SoftwarePowerRequestItem.RequestType)) { Width = 120 };
            var colCaller = new OLVColumn("Caller Subsystem", nameof(SoftwarePowerRequestItem.CallerName)) { Width = 150 };
            var colPrev = new OLVColumn("Prevents Sleep", nameof(SoftwarePowerRequestItem.PreventsSleep))
            {
                Width = 100,
                AspectToStringConverter = v => "Yes (WakeLock)"
            };
            var colDesc = new OLVColumn("Application / Process Details", nameof(SoftwarePowerRequestItem.Description)) { Width = 450, FillsFreeSpace = true };

            _folvPower.AllColumns.AddRange(new[] { colType, colCaller, colPrev, colDesc });
            _folvPower.RebuildColumns();

            mainLayout.Controls.Add(_folvPower, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing active Windows power requests and sleep-blocking applications...",
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
            _btnRefresh = new Button { Text = "Refresh Requests", AutoSize = true };
            _btnRefresh.Click += (s, e) => LoadPowerProfile();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void LoadPowerProfile()
        {
            _powerRequests = SoftwarePowerImpactEngine.QueryPowerRequests();
            _folvPower.SetObjects(_powerRequests);

            if (_powerRequests.Count == 0)
            {
                _lblSummary.Text = "No active power execution requests or sleep-preventing wake locks detected. Standby/Sleep will function normally.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {_powerRequests.Count} active power request(s) preventing Windows from sleeping.";
                _lblSummary.ForeColor = Color.DarkOrange;
            }
        }
    }
}
