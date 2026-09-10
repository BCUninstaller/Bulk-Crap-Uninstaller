/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Firewall Orphaned Rules Cleaner Window
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
    public sealed class FirewallOrphanCleanerWindow : Form
    {
        private FastObjectListView _folvRules;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<FirewallOrphanRuleItem> _items = new();

        public FirewallOrphanCleanerWindow()
        {
            InitializeComponent();
            ScanRules();
        }

        private void InitializeComponent()
        {
            Text = "Windows Firewall Orphaned Rules & Broken Bindings Cleaner - EBUninstaller Pro";
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

            _folvRules = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colName = new OLVColumn("Rule Name", nameof(FirewallOrphanRuleItem.RuleName)) { Width = 220 };
            var colDir = new OLVColumn("Direction", nameof(FirewallOrphanRuleItem.Direction)) { Width = 70 };
            var colAct = new OLVColumn("Action", nameof(FirewallOrphanRuleItem.Action)) { Width = 70 };
            var colPorts = new OLVColumn("Ports", nameof(FirewallOrphanRuleItem.LocalPorts)) { Width = 80 };
            var colApp = new OLVColumn("Orphaned Application Executable Path", nameof(FirewallOrphanRuleItem.TargetApplicationPath)) { Width = 450, FillsFreeSpace = true };

            _folvRules.AllColumns.AddRange(new[] { colName, colDir, colAct, colPorts, colApp });
            _folvRules.RebuildColumns();

            mainLayout.Controls.Add(_folvRules, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Advanced Firewall for rules of uninstalled software...",
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
            _btnRefresh.Click += (s, e) => ScanRules();

            _btnClean = new Button
            {
                Text = "Clean Selected Orphan Rules",
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

        private void ScanRules()
        {
            _items = FirewallOrphanCleanerEngine.ScanOrphanedRules();
            _folvRules.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No orphaned Windows Firewall rules detected. All active rules point to valid executables.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Found {_items.Count} orphaned firewall rule(s) pointing to deleted or uninstalled applications.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvRules.SelectedObjects.Count > 0 ? _folvRules.SelectedObjects.Cast<FirewallOrphanRuleItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete {selected.Count} orphaned firewall rule(s)?",
                "Clean Orphaned Rules",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = FirewallOrphanCleanerEngine.CleanOrphanedRules(selected);
                MessageBox.Show($"Removed {cleaned} orphaned firewall rule(s) successfully.", "Firewall Cleaned", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanRules();
            }
        }
    }
}
