/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Bluetooth & Wireless Device Pairing Residuals Window
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
    public sealed class BluetoothPairingCleanerWindow : Form
    {
        private FastObjectListView _folvDevices;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<BluetoothDeviceResidualItem> _items = new();

        public BluetoothPairingCleanerWindow()
        {
            InitializeComponent();
            ScanBluetooth();
        }

        private void InitializeComponent()
        {
            Text = "Bluetooth & Wireless Device Pairing Residuals Auditor - EBUninstaller Pro";
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

            _folvDevices = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("Device Name", nameof(BluetoothDeviceResidualItem.DeviceName)) { Width = 260 };
            var colClass = new OLVColumn("Device Class", nameof(BluetoothDeviceResidualItem.DeviceClass)) { Width = 180 };
            var colMac = new OLVColumn("MAC Address (BD_ADDR)", nameof(BluetoothDeviceResidualItem.MacAddress)) { Width = 180 };
            var colReg = new OLVColumn("Registry Key", nameof(BluetoothDeviceResidualItem.RegistryPath)) { Width = 300, FillsFreeSpace = true };

            _folvDevices.AllColumns.AddRange(new[] { colName, colClass, colMac, colReg });
            _folvDevices.RebuildColumns();

            mainLayout.Controls.Add(_folvDevices, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Bluetooth stack pairing entries and link profiles...",
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
            _btnRefresh = new Button { Text = "Refresh Devices", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanBluetooth();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanBluetooth()
        {
            _items = BluetoothPairingResidualsCleanerEngine.ScanBluetoothResiduals();
            _folvDevices.SetObjects(_items);

            _lblSummary.Text = $"Detected {_items.Count} paired Bluetooth / wireless device instance(s) in the Windows registry.";
            _lblSummary.ForeColor = Color.DarkSlateBlue;
        }
    }
}
