/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    USB & Peripheral Device Driver Residuals Cleaner Window
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
    public sealed class UsbDriverCleanerWindow : Form
    {
        private FastObjectListView _folvDevices;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<UsbDeviceResidualItem> _items = new();

        public UsbDriverCleanerWindow()
        {
            InitializeComponent();
            ScanUsb();
        }

        private void InitializeComponent()
        {
            Text = "USB & Peripheral Device Driver Residuals Auditor - EBUninstaller Pro";
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

            _folvDevices = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colDesc = new OLVColumn("Device Description", nameof(UsbDeviceResidualItem.DeviceDesc)) { Width = 280 };
            var colMfg = new OLVColumn("Manufacturer", nameof(UsbDeviceResidualItem.Manufacturer)) { Width = 160 };
            var colSvc = new OLVColumn("Driver Service", nameof(UsbDeviceResidualItem.ServiceDriver)) { Width = 140 };
            var colHwid = new OLVColumn("Hardware Instance ID", nameof(UsbDeviceResidualItem.HardwareId)) { Width = 380, FillsFreeSpace = true };

            _folvDevices.AllColumns.AddRange(new[] { colDesc, colMfg, colSvc, colHwid });
            _folvDevices.RebuildColumns();

            mainLayout.Controls.Add(_folvDevices, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows USB and storage device registry enumerations...",
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
            _btnRefresh.Click += (s, e) => ScanUsb();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanUsb()
        {
            _items = UsbDriverResidualsCleanerEngine.ScanUsbResiduals();
            _folvDevices.SetObjects(_items);

            _lblSummary.Text = $"Detected {_items.Count} registered USB device and peripheral driver instance(s).";
            _lblSummary.ForeColor = Color.DarkSlateBlue;
        }
    }
}
