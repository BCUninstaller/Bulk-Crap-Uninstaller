/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Firewall Port Matrix & Security Auditor Window
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
    public sealed class FirewallPortMatrixWindow : Form
    {
        private FastObjectListView _folvRules;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<FirewallPortRuleItem> _items = new();

        public FirewallPortMatrixWindow()
        {
            InitializeComponent();
            ScanRules();
        }

        private void InitializeComponent()
        {
            Text = "Windows Firewall Port Matrix & Public Exposure Auditor - EBUninstaller Pro";
            Size = new Size(1000, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 420);
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

            _folvRules = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("Firewall Rule Name", nameof(FirewallPortRuleItem.RuleName)) { Width = 230 };
            var colDir = new OLVColumn("Direction", nameof(FirewallPortRuleItem.Direction)) { Width = 80 };
            var colAct = new OLVColumn("Action", nameof(FirewallPortRuleItem.Action)) { Width = 70 };
            var colProto = new OLVColumn("Protocol", nameof(FirewallPortRuleItem.Protocol)) { Width = 70 };
            var colPort = new OLVColumn("Local Port", nameof(FirewallPortRuleItem.LocalPort)) { Width = 90 };
            var colProf = new OLVColumn("Network Profile", nameof(FirewallPortRuleItem.Profile)) { Width = 110 };
            var colApp = new OLVColumn("Associated Executable", nameof(FirewallPortRuleItem.ApplicationPath)) { Width = 300, FillsFreeSpace = true };

            _folvRules.AllColumns.AddRange(new[] { colName, colDir, colAct, colProto, colPort, colProf, colApp });
            _folvRules.RebuildColumns();

            mainLayout.Controls.Add(_folvRules, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Defender Firewall port matrices and public exceptions...",
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
            _btnRefresh = new Button { Text = "Refresh Matrix", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanRules();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanRules()
        {
            _items = FirewallPortMatrixAuditorEngine.ScanFirewallPortRules();
            _folvRules.SetObjects(_items);

            var publicInbound = _items.Count(r => r.IsPublicInboundAllow);

            _lblSummary.Text = $"Audited {_items.Count} active firewall rules ({publicInbound} inbound rules exposing ports on Public/Untrusted networks).";
            _lblSummary.ForeColor = publicInbound > 5 ? Color.DarkOrange : Color.DarkGreen;
        }
    }
}
