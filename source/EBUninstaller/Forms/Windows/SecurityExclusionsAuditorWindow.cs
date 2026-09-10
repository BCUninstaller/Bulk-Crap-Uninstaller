/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Defender & Security Exclusions Auditor Window
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
    public sealed class SecurityExclusionsAuditorWindow : Form
    {
        private FastObjectListView _folvExclusions;
        private Label _lblSummary;
        private Button _btnRemoveSelected;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<SecurityExclusionItem> _items = new();

        public SecurityExclusionsAuditorWindow()
        {
            InitializeComponent();
            ScanExclusions();
        }

        private void InitializeComponent()
        {
            Text = "Windows Security & Defender Exclusions Auditor - EBUninstaller Pro";
            Size = new Size(960, 520);
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

            _folvExclusions = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colRisk = new OLVColumn("Risk Level", nameof(SecurityExclusionItem.RiskLevel)) { Width = 90 };
            var colType = new OLVColumn("Type", nameof(SecurityExclusionItem.ExclusionType)) { Width = 90 };
            var colTarget = new OLVColumn("Exclusion Target", nameof(SecurityExclusionItem.Target)) { Width = 300 };
            var colOrphan = new OLVColumn("Orphaned", nameof(SecurityExclusionItem.IsOrphaned))
            {
                Width = 80,
                AspectToStringConverter = v => (bool)v ? "Yes" : "No"
            };
            var colReason = new OLVColumn("Security Assessment / Reason", nameof(SecurityExclusionItem.Reason)) { Width = 350, FillsFreeSpace = true };

            _folvExclusions.AllColumns.AddRange(new[] { colRisk, colType, colTarget, colOrphan, colReason });
            _folvExclusions.RebuildColumns();
            _folvExclusions.SelectionChanged += (s, e) => _btnRemoveSelected.Enabled = _folvExclusions.SelectedObjects.Count > 0;

            mainLayout.Controls.Add(_folvExclusions, 0, 0);

            // Summary Label
            _lblSummary = new Label
            {
                Text = "Auditing Windows Defender active antivirus exclusions...",
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
            _btnRefresh.Click += (s, e) => ScanExclusions();

            _btnRemoveSelected = new Button
            {
                Text = "Remove Selected Exclusion(s)",
                AutoSize = true,
                Enabled = false,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            _btnRemoveSelected.Click += OnRemoveClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnRemoveSelected);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanExclusions()
        {
            _items = SecurityExclusionsAuditorEngine.ScanDefenderExclusions();
            _folvExclusions.SetObjects(_items);

            var critical = _items.Count(i => i.RiskLevel == ExclusionRiskLevel.Critical);
            var high = _items.Count(i => i.RiskLevel == ExclusionRiskLevel.High);
            var orphaned = _items.Count(i => i.IsOrphaned);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No Windows Defender exclusions detected. Antivirus shield is operating at full scope.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Found {_items.Count} exclusion(s) ({critical} critical risks, {high} high risks, {orphaned} orphaned paths).";
                _lblSummary.ForeColor = (critical > 0 || high > 0) ? Color.DarkRed : Color.DarkOrange;
            }
        }

        private void OnRemoveClick(object sender, EventArgs e)
        {
            var selected = _folvExclusions.SelectedObjects.Cast<SecurityExclusionItem>().ToList();
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete {selected.Count} selected security exclusion(s)? Real-time protection will be restored for these targets.",
                "Confirm Exclusion Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                int removed = 0;
                foreach (var item in selected)
                {
                    if (SecurityExclusionsAuditorEngine.RemoveExclusion(item))
                        removed++;
                }

                MessageBox.Show($"Removed {removed} of {selected.Count} exclusion(s) successfully.", "Exclusions Cleaned", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanExclusions();
            }
        }
    }
}
