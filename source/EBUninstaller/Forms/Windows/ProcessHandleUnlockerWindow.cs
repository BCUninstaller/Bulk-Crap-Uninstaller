/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Process Handle & File Lock Resolution Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class ProcessHandleUnlockerWindow : Form
    {
        private FastObjectListView _folvLockers;
        private TextBox _txtTargetPath;
        private Label _lblSummary;
        private Button _btnFind;
        private Button _btnUnlockKill;
        private Button _btnClose;
        private List<LockingProcessInfo> _lockingProcesses = new();

        public ProcessHandleUnlockerWindow(string targetPath = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(targetPath))
            {
                _txtTargetPath.Text = targetPath;
                FindLocks();
            }
        }

        private void InitializeComponent()
        {
            Text = "Process Handle & File Lock Resolver - EBUninstaller Pro";
            Size = new Size(920, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 380);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Search Target
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Top Path Selector
            var topPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, Margin = new Padding(0, 0, 0, 8) };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblTarget = new Label { Text = "Locked File/Folder:", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
            _txtTargetPath = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 2, 8, 0) };
            var btnBrowse = new Button { Text = "Browse...", AutoSize = true };
            btnBrowse.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Title = "Select Locked File", CheckFileExists = false };
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    _txtTargetPath.Text = ofd.FileName;
                    FindLocks();
                }
            };

            topPanel.Controls.Add(lblTarget, 0, 0);
            topPanel.Controls.Add(_txtTargetPath, 1, 0);
            topPanel.Controls.Add(btnBrowse, 2, 0);
            mainLayout.Controls.Add(topPanel, 0, 0);

            // FastObjectListView
            _folvLockers = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colPid = new OLVColumn("PID", nameof(LockingProcessInfo.ProcessId)) { Width = 80 };
            var colName = new OLVColumn("Process Name", nameof(LockingProcessInfo.ProcessName)) { Width = 180 };
            var colDesc = new OLVColumn("Application Title", nameof(LockingProcessInfo.ApplicationDescription)) { Width = 200 };
            var colPath = new OLVColumn("Executable Path", nameof(LockingProcessInfo.ExecutablePath)) { Width = 400, FillsFreeSpace = true };

            _folvLockers.AllColumns.AddRange(new[] { colPid, colName, colDesc, colPath });
            _folvLockers.RebuildColumns();
            _folvLockers.SelectionChanged += (s, e) => _btnUnlockKill.Enabled = _folvLockers.SelectedObject != null;

            mainLayout.Controls.Add(_folvLockers, 0, 1);

            // Summary
            _lblSummary = new Label
            {
                Text = "Enter or browse a file or folder path and click 'Find Locking Processes'.",
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 8),
                Font = new Font(Font, FontStyle.Bold)
            };
            mainLayout.Controls.Add(_lblSummary, 0, 2);

            // Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK, AutoSize = true };
            _btnFind = new Button { Text = "Find Locks", AutoSize = true };
            _btnFind.Click += (s, e) => FindLocks();

            _btnUnlockKill = new Button
            {
                Text = "Terminate Locking Process",
                AutoSize = true,
                Enabled = false,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            _btnUnlockKill.Click += OnKillProcessClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnFind);
            btnPanel.Controls.Add(_btnUnlockKill);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void FindLocks()
        {
            var target = _txtTargetPath.Text?.Trim();
            if (string.IsNullOrEmpty(target)) return;

            _lockingProcesses = ProcessHandleUnlockerEngine.FindLockingProcesses(target);
            _folvLockers.SetObjects(_lockingProcesses);

            if (_lockingProcesses.Count == 0)
            {
                _lblSummary.Text = "No active process locks found on the specified target.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Found {_lockingProcesses.Count} process(es) locking the specified target.";
                _lblSummary.ForeColor = Color.DarkRed;
            }
        }

        private void OnKillProcessClick(object sender, EventArgs e)
        {
            var selected = _folvLockers.SelectedObject as LockingProcessInfo;
            if (selected == null) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to terminate process '{selected.ProcessName}' (PID: {selected.ProcessId}) to release the file lock?",
                "Terminate Locking Process",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (ProcessHandleUnlockerEngine.TerminateLockingProcess(selected.ProcessId))
                {
                    MessageBox.Show($"Terminated process '{selected.ProcessName}' successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FindLocks();
                }
                else
                {
                    MessageBox.Show($"Could not terminate process '{selected.ProcessName}'. Administrator privileges may be required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
