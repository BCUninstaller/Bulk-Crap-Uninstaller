/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    COM+ Applications & Component Services Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class ComPlusCatalogAuditorWindow : Form
    {
        private FastObjectListView _folvApps;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<ComPlusAppItem> _items = new();

        public ComPlusCatalogAuditorWindow()
        {
            InitializeComponent();
            ScanComPlus();
        }

        private void InitializeComponent()
        {
            Text = "COM+ Applications & Component Services Catalog Auditor - EBUninstaller Pro";
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

            _folvApps = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("COM+ Application Name", nameof(ComPlusAppItem.ApplicationName)) { Width = 260 };
            var colId = new OLVColumn("Security Identity", nameof(ComPlusAppItem.Identity)) { Width = 180 };
            var colSys = new OLVColumn("Classification", nameof(ComPlusAppItem.IsSystemApp))
            {
                Width = 140,
                AspectToStringConverter = v => (bool)v ? "Windows System App" : "Third-Party COM+ App"
            };
            var colGuid = new OLVColumn("AppID (GUID)", nameof(ComPlusAppItem.AppId)) { Width = 280, FillsFreeSpace = true };

            _folvApps.AllColumns.AddRange(new[] { colName, colId, colSys, colGuid });
            _folvApps.RebuildColumns();

            mainLayout.Controls.Add(_folvApps, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Component Services COM+ application catalog...",
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
            _btnRefresh = new Button { Text = "Refresh Catalog", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanComPlus();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanComPlus()
        {
            _items = ComPlusCatalogAuditorEngine.ScanComPlusApps();
            _folvApps.SetObjects(_items);

            var thirdPartyCount = _items.Count(i => !i.IsSystemApp);
            _lblSummary.Text = $"Detected {_items.Count} COM+ application package(s) in catalog ({thirdPartyCount} third-party application services).";
            _lblSummary.ForeColor = Color.DarkSlateBlue;
        }
    }
}
