/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Security Center Providers Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SecurityHardening;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class WscProviderAuditorWindow : Form
    {
        private FastObjectListView _folvProviders;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<WscProviderItem> _items = new();

        public WscProviderAuditorWindow()
        {
            InitializeComponent();
            ScanProviders();
        }

        private void InitializeComponent()
        {
            Text = "Windows Security Center (WSC) Security Providers Auditor - EBUninstaller Pro";
            Size = new Size(1000, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 400);
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

            _folvProviders = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("Security Provider Name", nameof(WscProviderItem.DisplayName)) { Width = 230 };
            var colCat = new OLVColumn("Category", nameof(WscProviderItem.ProviderCategory)) { Width = 110 };
            var colStatus = new OLVColumn("Health Status", nameof(WscProviderItem.HealthStatus)) { Width = 200 };
            var colPath = new OLVColumn("Executable / Reporting Path", nameof(WscProviderItem.ExecutablePath)) { Width = 380, FillsFreeSpace = true };

            _folvProviders.AllColumns.AddRange(new[] { colName, colCat, colStatus, colPath });
            _folvProviders.RebuildColumns();

            mainLayout.Controls.Add(_folvProviders, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Security Center (WSC) registered Antivirus, Firewall & Security suites...",
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
            _btnRefresh.Click += (s, e) => ScanProviders();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanProviders()
        {
            _items = WscProviderAuditorEngine.ScanSecurityProviders();
            _folvProviders.SetObjects(_items);

            var orphanCount = _items.Count(p => p.IsOrphaned);
            var activeCount = _items.Count(p => !p.IsOrphaned);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "Windows Security Center has no registered third-party security providers. Windows Defender is standard.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else if (orphanCount > 0)
            {
                _lblSummary.Text = $"Detected {_items.Count} provider(s) ({orphanCount} orphaned provider(s) pointing to deleted security software).";
                _lblSummary.ForeColor = Color.DarkOrange;
            }
            else
            {
                _lblSummary.Text = $"All {activeCount} registered Security Center provider(s) have verified binaries.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
        }
    }
}
