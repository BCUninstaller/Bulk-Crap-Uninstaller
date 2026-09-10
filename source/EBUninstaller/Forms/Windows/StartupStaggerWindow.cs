/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Startup Staggering & Delayed Execution Window
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
    public sealed class StartupStaggerWindow : Form
    {
        private FastObjectListView _folvStartup;
        private ComboBox _cboDelay;
        private Label _lblSummary;
        private Button _btnStagger;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<StaggeredStartupItem> _items = new();

        public StartupStaggerWindow()
        {
            InitializeComponent();
            LoadStartup();
        }

        private void InitializeComponent()
        {
            Text = "Startup Staggering & Delayed Execution Manager - EBUninstaller Pro";
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

            _folvStartup = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("Startup Entry", nameof(StaggeredStartupItem.Name)) { Width = 200 };
            var colLoc = new OLVColumn("Location", nameof(StaggeredStartupItem.SourceLocation)) { Width = 110 };
            var colDelay = new OLVColumn("Delay Status", nameof(StaggeredStartupItem.DelaySeconds))
            {
                Width = 110,
                AspectToStringConverter = v => (int)v == 0 ? "Instant (0s)" : $"Delayed ({(int)v}s)"
            };
            var colCmd = new OLVColumn("Executable Command", nameof(StaggeredStartupItem.Command)) { Width = 450, FillsFreeSpace = true };

            _folvStartup.AllColumns.AddRange(new[] { colName, colLoc, colDelay, colCmd });
            _folvStartup.RebuildColumns();
            _folvStartup.SelectionChanged += (s, e) => _btnStagger.Enabled = _folvStartup.SelectedObject != null;

            mainLayout.Controls.Add(_folvStartup, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Stagger non-essential background applications to speed up initial Windows desktop logon...",
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
            _btnRefresh = new Button { Text = "Refresh", AutoSize = true };
            _btnRefresh.Click += (s, e) => LoadStartup();

            _btnStagger = new Button
            {
                Text = "Set Delayed Startup",
                AutoSize = true,
                Enabled = false,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnStagger.Click += OnStaggerClick;

            _cboDelay = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 130,
                Margin = new Padding(8, 3, 8, 3)
            };
            _cboDelay.Items.AddRange(new object[] { "15 Seconds", "30 Seconds", "60 Seconds", "120 Seconds", "300 Seconds" });
            _cboDelay.SelectedIndex = 1; // 30s default

            var lblDelayChoice = new Label { Text = "Delay Interval:", AutoSize = true, Margin = new Padding(0, 6, 4, 0) };

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnStagger);
            btnPanel.Controls.Add(_cboDelay);
            btnPanel.Controls.Add(lblDelayChoice);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void LoadStartup()
        {
            _items = StartupStaggerEngine.GetStartupItems();
            _folvStartup.SetObjects(_items);

            _lblSummary.Text = $"Detected {_items.Count} registered startup applications. Select an entry to stagger its logon execution.";
            _lblSummary.ForeColor = Color.DarkGreen;
        }

        private void OnStaggerClick(object sender, EventArgs e)
        {
            var selected = _folvStartup.SelectedObject as StaggeredStartupItem;
            if (selected == null) return;

            var delaySecs = _cboDelay.SelectedIndex switch
            {
                0 => 15,
                1 => 30,
                2 => 60,
                3 => 120,
                4 => 300,
                _ => 30
            };

            if (StartupStaggerEngine.StaggerStartupItem(selected, delaySecs))
            {
                MessageBox.Show($"Startup item '{selected.Name}' will now launch {delaySecs} seconds after Windows logon.", "Stagger Configured", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStartup();
            }
            else
            {
                MessageBox.Show($"Failed to configure delayed startup for '{selected.Name}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
