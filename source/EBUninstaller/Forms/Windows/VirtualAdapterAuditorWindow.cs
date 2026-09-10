/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Virtual Network Adapter & VPN TAP/TUN Residuals Auditor Window
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
    public sealed class VirtualAdapterAuditorWindow : Form
    {
        private FastObjectListView _folvNics;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<VirtualAdapterItem> _items = new();

        public VirtualAdapterAuditorWindow()
        {
            InitializeComponent();
            ScanAdapters();
        }

        private void InitializeComponent()
        {
            Text = "Virtual Network Adapter & VPN TAP/TUN Residuals Auditor - EBUninstaller Pro";
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

            _folvNics = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colProvider = new OLVColumn("Adapter Category / Provider", nameof(VirtualAdapterItem.ProviderCategory)) { Width = 230 };
            var colName = new OLVColumn("Interface Name", nameof(VirtualAdapterItem.Name)) { Width = 170 };
            var colStatus = new OLVColumn("Status", nameof(VirtualAdapterItem.OperationalStatus)) { Width = 90 };
            var colType = new OLVColumn("Type", nameof(VirtualAdapterItem.InterfaceType)) { Width = 90 };
            var colDesc = new OLVColumn("Hardware / Driver Description", nameof(VirtualAdapterItem.Description)) { Width = 380, FillsFreeSpace = true };

            _folvNics.AllColumns.AddRange(new[] { colProvider, colName, colStatus, colType, colDesc });
            _folvNics.RebuildColumns();

            mainLayout.Controls.Add(_folvNics, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing network adapters and virtual TAP/TUN interfaces...",
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
            _btnRefresh = new Button { Text = "Refresh Adapters", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanAdapters();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanAdapters()
        {
            _items = VirtualAdapterAuditorEngine.ScanVirtualAdapters();
            _folvNics.SetObjects(_items);

            var virtualCount = _items.Count(i => i.IsVirtual);
            var disconnectedVirtual = _items.Count(i => i.IsVirtual && i.OperationalStatus != "Up");

            _lblSummary.Text = $"Detected {_items.Count} total network adapter(s) ({virtualCount} virtual/VPN adapters, {disconnectedVirtual} disconnected or dormant).";
            _lblSummary.ForeColor = disconnectedVirtual > 0 ? Color.DarkOrange : Color.DarkGreen;
        }
    }
}
