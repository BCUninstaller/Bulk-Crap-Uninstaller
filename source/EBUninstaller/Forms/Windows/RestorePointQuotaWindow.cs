/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    VSS Shadow Storage & Restore Point Quota Window
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
    public sealed class RestorePointQuotaWindow : Form
    {
        private FastObjectListView _folvVss;
        private Label _lblSummary;
        private NumericUpDown _nudPercent;
        private Button _btnApply;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<ShadowStorageInfo> _items = new();

        public RestorePointQuotaWindow()
        {
            InitializeComponent();
            ScanVss();
        }

        private void InitializeComponent()
        {
            Text = "VSS Restore Point Storage Quota & Allocation Optimizer - EBUninstaller Pro";
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

            _folvVss = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colVol = new OLVColumn("Drive Volume", nameof(ShadowStorageInfo.ForVolume)) { Width = 180 };
            var colUsed = new OLVColumn("Used VSS Space", nameof(ShadowStorageInfo.UsedSpaceStr)) { Width = 180 };
            var colAlloc = new OLVColumn("Allocated Space", nameof(ShadowStorageInfo.AllocatedSpaceStr)) { Width = 180 };
            var colMax = new OLVColumn("Maximum Storage Limit", nameof(ShadowStorageInfo.MaxSpaceStr)) { Width = 230, FillsFreeSpace = true };

            _folvVss.AllColumns.AddRange(new[] { colVol, colUsed, colAlloc, colMax });
            _folvVss.RebuildColumns();

            mainLayout.Controls.Add(_folvVss, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Volume Shadow Copy (VSS) restore point allocation per volume...",
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
            _btnRefresh.Click += (s, e) => ScanVss();

            _btnApply = new Button
            {
                Text = "Set Max Quota (%)",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnApply.Click += OnApplyClick;

            _nudPercent = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 50,
                Value = 10,
                Width = 60
            };

            var lblNud = new Label { Text = "Max Drive %:", AutoSize = true, Padding = new Padding(0, 6, 4, 0) };

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnApply);
            btnPanel.Controls.Add(_nudPercent);
            btnPanel.Controls.Add(lblNud);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanVss()
        {
            _items = RestorePointQuotaOptimizerEngine.QueryShadowStorage();
            _folvVss.SetObjects(_items);

            _lblSummary.Text = $"Detected {_items.Count} VSS storage volume allocation(s). Adjusting quota prevents restore points from filling SSD storage.";
            _lblSummary.ForeColor = Color.DarkSlateBlue;
        }

        private void OnApplyClick(object sender, EventArgs e)
        {
            var selected = _folvVss.SelectedObject as ShadowStorageInfo ?? _items.FirstOrDefault();
            if (selected == null) return;

            var val = (int)_nudPercent.Value;
            var success = RestorePointQuotaOptimizerEngine.ResizeShadowStorage(selected.ForVolume, val);
            if (success)
            {
                MessageBox.Show($"VSS Shadow Storage on {selected.ForVolume} resized to max {val}%.", "Quota Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanVss();
            }
            else
            {
                MessageBox.Show("Failed updating VSS quota. Ensure Administrator privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
