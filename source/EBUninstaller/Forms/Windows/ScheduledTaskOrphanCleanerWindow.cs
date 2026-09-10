/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Scheduled Tasks Orphan Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class ScheduledTaskOrphanCleanerWindow : Form
    {
        private FastObjectListView _folvTasks;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<OrphanScheduledTaskItem> _items = new();

        public ScheduledTaskOrphanCleanerWindow()
        {
            InitializeComponent();
            ScanTasks();
        }

        private void InitializeComponent()
        {
            Text = "Scheduled Tasks Orphan & Residuals Cleaner - EBUninstaller Pro";
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

            _folvTasks = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colName = new OLVColumn("Task Name", nameof(OrphanScheduledTaskItem.TaskName)) { Width = 220 };
            var colTarget = new OLVColumn("Missing Target Executable", nameof(OrphanScheduledTaskItem.TargetBinary)) { Width = 320 };
            var colReason = new OLVColumn("Issue Reason", nameof(OrphanScheduledTaskItem.IssueReason)) { Width = 280, FillsFreeSpace = true };

            _folvTasks.AllColumns.AddRange(new[] { colName, colTarget, colReason });
            _folvTasks.RebuildColumns();

            mainLayout.Controls.Add(_folvTasks, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Task Scheduler for orphaned third-party tasks...",
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
            _btnRefresh.Click += (s, e) => ScanTasks();

            _btnClean = new Button
            {
                Text = "Clean Selected Orphaned Tasks",
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

        private void ScanTasks()
        {
            _items = ScheduledTaskOrphanCleanerEngine.ScanOrphanedTasks();
            _folvTasks.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No orphaned scheduled tasks detected. All tasks point to valid executables.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} orphaned scheduled task(s) attempting to launch non-existent programs.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvTasks.SelectedObjects.Count > 0 ? _folvTasks.SelectedObjects.Cast<OrphanScheduledTaskItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Delete {selected.Count} orphaned scheduled task(s)?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = ScheduledTaskOrphanCleanerEngine.CleanOrphanedTasks(selected);
                MessageBox.Show($"Cleaned {cleaned} orphaned scheduled task(s) successfully.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanTasks();
            }
        }
    }
}
