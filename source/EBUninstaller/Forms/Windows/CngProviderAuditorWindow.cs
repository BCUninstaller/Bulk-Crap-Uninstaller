/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    CNG & CryptoAPI Cryptographic Provider Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SecurityHardening;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class CngProviderAuditorWindow : Form
    {
        private FastObjectListView _folvProviders;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<CryptoProviderItem> _items = new();

        public CngProviderAuditorWindow()
        {
            InitializeComponent();
            ScanProviders();
        }

        private void InitializeComponent()
        {
            Text = "CNG & CryptoAPI Cryptographic Providers Auditor - EBUninstaller Pro";
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

            var colName = new OLVColumn("Provider Name", nameof(CryptoProviderItem.ProviderName)) { Width = 260 };
            var colType = new OLVColumn("Architecture / Type", nameof(CryptoProviderItem.ProviderType)) { Width = 140 };
            var colStatus = new OLVColumn("Health Status", nameof(CryptoProviderItem.HealthStatus)) { Width = 180 };
            var colImage = new OLVColumn("Driver / DLL Binary", nameof(CryptoProviderItem.ImagePath)) { Width = 380, FillsFreeSpace = true };

            _folvProviders.AllColumns.AddRange(new[] { colName, colType, colStatus, colImage });
            _folvProviders.RebuildColumns();

            mainLayout.Controls.Add(_folvProviders, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows CNG and CAPI cryptographic subsystem providers...",
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
            _btnRefresh = new Button { Text = "Refresh Providers", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanProviders();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanProviders()
        {
            _items = CngProviderAuditorEngine.ScanCryptoProviders();
            _folvProviders.SetObjects(_items);

            var missingCount = _items.Count(p => p.IsBinaryMissing);
            var validCount = _items.Count(p => !p.IsBinaryMissing);

            if (missingCount == 0)
            {
                _lblSummary.Text = $"All {validCount} registered cryptographic provider(s) have valid DLL binaries.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {missingCount} orphaned cryptographic provider(s) pointing to deleted DLLs.";
                _lblSummary.ForeColor = Color.DarkOrange;
            }
        }
    }
}
