/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Environment Variables Residuals Cleaner Window
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
    public sealed class EnvVarResidualsCleanerWindow : Form
    {
        private FastObjectListView _folvVars;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<OrphanedEnvVarItem> _items = new();

        public EnvVarResidualsCleanerWindow()
        {
            InitializeComponent();
            ScanVars();
        }

        private void InitializeComponent()
        {
            Text = "Software Environment Variables Residuals Cleaner - EBUninstaller Pro";
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

            _folvVars = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colScope = new OLVColumn("Scope", nameof(OrphanedEnvVarItem.Scope)) { Width = 80 };
            var colName = new OLVColumn("Variable Name", nameof(OrphanedEnvVarItem.VariableName)) { Width = 180 };
            var colVal = new OLVColumn("Configured Value", nameof(OrphanedEnvVarItem.VariableValue)) { Width = 280 };
            var colMissing = new OLVColumn("Non-Existent Target Path", nameof(OrphanedEnvVarItem.MissingPath)) { Width = 350, FillsFreeSpace = true };

            _folvVars.AllColumns.AddRange(new[] { colScope, colName, colVal, colMissing });
            _folvVars.RebuildColumns();

            mainLayout.Controls.Add(_folvVars, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning System and User environment variables for dead SDK/runtime paths...",
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
            _btnRefresh.Click += (s, e) => ScanVars();

            _btnClean = new Button
            {
                Text = "Clean Selected Orphan Variables",
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

        private void ScanVars()
        {
            _items = EnvVarResidualsCleanerEngine.ScanOrphanedEnvironmentVariables();
            _folvVars.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No orphaned environment variables detected. All configured SDK/runtime variables point to valid paths.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Found {_items.Count} orphaned environment variable(s) pointing to deleted directories or SDKs.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvVars.SelectedObjects.Count > 0 ? _folvVars.SelectedObjects.Cast<OrphanedEnvVarItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Delete {selected.Count} orphaned environment variable(s)?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = EnvVarResidualsCleanerEngine.CleanOrphanedVariables(selected);
                MessageBox.Show($"Cleaned {cleaned} orphaned variable(s) successfully.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanVars();
            }
        }
    }
}
