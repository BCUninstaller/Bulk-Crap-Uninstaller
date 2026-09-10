/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Environment PATH Health Auditor & Optimizer Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class PathEnvironmentAuditorWindow : Form
    {
        private FastObjectListView _folvIssues;
        private Label _lblSummary;
        private Button _btnOptimize;
        private Button _btnRefresh;
        private Button _btnClose;
        private PathAuditReport _currentReport;

        public PathEnvironmentAuditorWindow()
        {
            InitializeComponent();
            AuditEnvironment();
        }

        private void InitializeComponent()
        {
            Text = "Environment PATH Health Auditor & Optimizer - EBUninstaller Pro";
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

            _folvIssues = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colScope = new OLVColumn("Scope", nameof(PathAuditItem.Scope)) { Width = 90 };
            var colIssue = new OLVColumn("Issue Type", nameof(PathAuditItem.IssueType)) { Width = 170 };
            var colPath = new OLVColumn("Path Entry", nameof(PathAuditItem.PathSegment)) { Width = 320 };
            var colDesc = new OLVColumn("Description", nameof(PathAuditItem.Description)) { Width = 300, FillsFreeSpace = true };

            _folvIssues.AllColumns.AddRange(new[] { colScope, colIssue, colPath, colDesc });
            _folvIssues.RebuildColumns();

            mainLayout.Controls.Add(_folvIssues, 0, 0);

            // Summary Label
            _lblSummary = new Label
            {
                Text = "Auditing System and User PATH environment variables...",
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
            _btnRefresh.Click += (s, e) => AuditEnvironment();

            _btnOptimize = new Button
            {
                Text = "Clean & Optimize PATH",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnOptimize.Click += OnOptimizeClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnOptimize);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void AuditEnvironment()
        {
            _currentReport = PathEnvironmentAuditorEngine.AuditEnvironmentPaths();
            _folvIssues.SetObjects(_currentReport.Issues);

            var missing = _currentReport.MissingDirectoryCount;
            var dupes = _currentReport.DuplicateCount;
            var risks = _currentReport.SecurityRiskCount;
            var total = _currentReport.Issues.Count;

            _lblSummary.Text = $"System PATH: {_currentReport.SystemPathLength} chars | User PATH: {_currentReport.UserPathLength} chars | Detected {total} issue(s) ({missing} dead paths, {dupes} duplicates, {risks} security flags).";
            _lblSummary.ForeColor = total > 0 ? Color.DarkOrange : Color.DarkGreen;
            _btnOptimize.Enabled = missing > 0 || dupes > 0;
        }

        private void OnOptimizeClick(object sender, EventArgs e)
        {
            if (_currentReport == null) return;

            var cleanedUser = PathEnvironmentAuditorEngine.CleanAndOptimizePath(_currentReport.RawUserPath, true);
            var confirm = MessageBox.Show(
                "Do you want to apply the cleaned and deduplicated PATH configuration to User environment variables?",
                "Optimize PATH",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    Environment.SetEnvironmentVariable("PATH", cleanedUser, EnvironmentVariableTarget.User);
                    MessageBox.Show("User PATH successfully cleaned and updated!", "Optimization Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AuditEnvironment();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to update PATH: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
