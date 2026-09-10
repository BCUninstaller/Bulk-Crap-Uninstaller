/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Services Conflict & Port Collision Detector Window
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
    public sealed class ServiceConflictDetectorWindow : Form
    {
        private FastObjectListView _folvConflicts;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<ServiceConflictItem> _conflicts = new();

        public ServiceConflictDetectorWindow()
        {
            InitializeComponent();
            AnalyzeServices();
        }

        private void InitializeComponent()
        {
            Text = "Windows Services Conflict & Port Collision Detector - EBUninstaller Pro";
            Size = new Size(950, 520);
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

            _folvConflicts = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colSev = new OLVColumn("Severity", nameof(ServiceConflictItem.Severity)) { Width = 90 };
            var colType = new OLVColumn("Issue Type", nameof(ServiceConflictItem.ConflictType)) { Width = 140 };
            var colPrim = new OLVColumn("Primary Service", nameof(ServiceConflictItem.PrimaryService)) { Width = 150 };
            var colConf = new OLVColumn("Conflicting Service", nameof(ServiceConflictItem.ConflictingService)) { Width = 150 };
            var colPort = new OLVColumn("Port", nameof(ServiceConflictItem.PortNumber))
            {
                Width = 70,
                AspectToStringConverter = v => v != null ? v.ToString() : "-"
            };
            var colDesc = new OLVColumn("Description", nameof(ServiceConflictItem.Description)) { Width = 280 };
            var colRec = new OLVColumn("Recommended Resolution", nameof(ServiceConflictItem.Recommendation)) { Width = 220, FillsFreeSpace = true };

            _folvConflicts.AllColumns.AddRange(new[] { colSev, colType, colPrim, colConf, colPort, colDesc, colRec });
            _folvConflicts.RebuildColumns();

            mainLayout.Controls.Add(_folvConflicts, 0, 0);

            // Summary Label
            _lblSummary = new Label
            {
                Text = "Analyzing Windows service dependencies, port bindings, and executable targets...",
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
            _btnRefresh = new Button { Text = "Scan Conflicts", AutoSize = true };
            _btnRefresh.Click += (s, e) => AnalyzeServices();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void AnalyzeServices()
        {
            _conflicts = ServiceConflictDetectorEngine.AnalyzeConflicts();
            _folvConflicts.SetObjects(_conflicts);

            var criticalCount = _conflicts.Count(c => c.Severity == ServiceConflictSeverity.Critical);
            var warningCount = _conflicts.Count(c => c.Severity == ServiceConflictSeverity.Warning);

            if (_conflicts.Count == 0)
            {
                _lblSummary.Text = "No service conflicts or port binding collisions detected on this system.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {_conflicts.Count} issue(s): {criticalCount} critical port collisions, {warningCount} service warnings.";
                _lblSummary.ForeColor = criticalCount > 0 ? Color.DarkRed : Color.DarkOrange;
            }
        }
    }
}
