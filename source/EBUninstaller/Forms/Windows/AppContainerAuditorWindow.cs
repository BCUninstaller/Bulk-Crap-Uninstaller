/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    UWP / AppContainer Permissions & Sandbox Isolation Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.StoreApps;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class AppContainerAuditorWindow : Form
    {
        private FastObjectListView _folvApps;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<AppContainerAuditItem> _items = new();

        public AppContainerAuditorWindow()
        {
            InitializeComponent();
            AuditApps();
        }

        private void InitializeComponent()
        {
            Text = "UWP / AppContainer Sandbox Isolation & Permissions Auditor - EBUninstaller Pro";
            Size = new Size(1000, 520);
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

            _folvApps = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colRisk = new OLVColumn("Isolation Risk", nameof(AppContainerAuditItem.OverallRisk)) { Width = 110 };
            var colName = new OLVColumn("Application / Package Name", nameof(AppContainerAuditItem.DisplayName)) { Width = 230 };
            var colPub = new OLVColumn("Publisher", nameof(AppContainerAuditItem.Publisher)) { Width = 160 };
            var colFullTrust = new OLVColumn("Full Trust", nameof(AppContainerAuditItem.HasFullTrust))
            {
                Width = 80,
                AspectToStringConverter = v => (bool)v ? "Yes" : "No"
            };
            var colBroadFs = new OLVColumn("Broad FS Access", nameof(AppContainerAuditItem.HasBroadFileSystemAccess))
            {
                Width = 110,
                AspectToStringConverter = v => (bool)v ? "Yes" : "No"
            };
            var colSummary = new OLVColumn("Declared Capabilities & Permissions", nameof(AppContainerAuditItem.RiskSummary)) { Width = 300, FillsFreeSpace = true };

            _folvApps.AllColumns.AddRange(new[] { colRisk, colName, colPub, colFullTrust, colBroadFs, colSummary });
            _folvApps.RebuildColumns();

            mainLayout.Controls.Add(_folvApps, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Auditing Windows Store / AppContainer application capability manifests...",
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
            _btnRefresh = new Button { Text = "Refresh Audit", AutoSize = true };
            _btnRefresh.Click += (s, e) => AuditApps();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void AuditApps()
        {
            _items = AppContainerIsolationAuditorEngine.AuditInstalledStorePackages();
            _folvApps.SetObjects(_items);

            var highRisk = _items.Count(i => i.OverallRisk == CapabilityRiskLevel.HighRisk);
            var elevated = _items.Count(i => i.OverallRisk == CapabilityRiskLevel.Elevated);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No AppContainer/UWP applications detected or WindowsApps directory is default-isolated.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Audited {_items.Count} package(s): {highRisk} high-privilege apps (FullTrust/BroadFS), {elevated} with elevated sensor permissions.";
                _lblSummary.ForeColor = highRisk > 0 ? Color.DarkOrange : Color.DarkGreen;
            }
        }
    }
}
