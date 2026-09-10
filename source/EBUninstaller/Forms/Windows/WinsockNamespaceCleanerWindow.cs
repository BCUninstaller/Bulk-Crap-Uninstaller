/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Winsock NameSpace & LSP Provider Residuals Cleaner Window
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
    public sealed class WinsockNamespaceCleanerWindow : Form
    {
        private FastObjectListView _folvProviders;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<WinsockProviderItem> _items = new();

        public WinsockNamespaceCleanerWindow()
        {
            InitializeComponent();
            ScanWinsock();
        }

        private void InitializeComponent()
        {
            Text = "Winsock NameSpace & LSP Providers Auditor - EBUninstaller Pro";
            Size = new Size(1000, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 400);
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

            _folvProviders = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("NameSpace Provider Name", nameof(WinsockProviderItem.ProviderName)) { Width = 280 };
            var colStatus = new OLVColumn("Status", nameof(WinsockProviderItem.HealthStatus)) { Width = 190 };
            var colGuid = new OLVColumn("Provider GUID", nameof(WinsockProviderItem.ProviderId)) { Width = 200 };
            var colDll = new OLVColumn("Provider DLL Path", nameof(WinsockProviderItem.LibraryPath)) { Width = 290, FillsFreeSpace = true };

            _folvProviders.AllColumns.AddRange(new[] { colName, colStatus, colGuid, colDll });
            _folvProviders.RebuildColumns();

            mainLayout.Controls.Add(_folvProviders, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Sockets (Winsock2) Name Space providers and network stack bindings...",
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
            _btnRefresh.Click += (s, e) => ScanWinsock();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanWinsock()
        {
            _items = WinsockNamespaceResidualsCleanerEngine.ScanWinsockProviders();
            _folvProviders.SetObjects(_items);

            var missingCount = _items.Count(p => p.IsLibraryMissing);
            var validCount = _items.Count(p => !p.IsLibraryMissing);

            if (missingCount == 0)
            {
                _lblSummary.Text = $"All {validCount} registered Winsock Name Space provider(s) have verified DLLs.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {missingCount} orphaned Winsock provider(s) with missing DLLs (Potential network resolution issues).";
                _lblSummary.ForeColor = Color.DarkOrange;
            }
        }
    }
}
