/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DirectShow & Media Foundation Codec Residuals Auditor Window
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
    public sealed class MediaCodecAuditorWindow : Form
    {
        private FastObjectListView _folvCodecs;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<MediaCodecResidualItem> _items = new();

        public MediaCodecAuditorWindow()
        {
            InitializeComponent();
            ScanCodecs();
        }

        private void InitializeComponent()
        {
            Text = "DirectShow & Media Foundation Codec Residuals Auditor - EBUninstaller Pro";
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

            _folvCodecs = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("Codec / Filter Name", nameof(MediaCodecResidualItem.CodecName)) { Width = 260 };
            var colStatus = new OLVColumn("Health Status", nameof(MediaCodecResidualItem.HealthStatus)) { Width = 190 };
            var colType = new OLVColumn("Framework Type", nameof(MediaCodecResidualItem.FrameworkType)) { Width = 140 };
            var colPath = new OLVColumn("Server DLL Binary", nameof(MediaCodecResidualItem.ServerBinaryPath)) { Width = 380, FillsFreeSpace = true };

            _folvCodecs.AllColumns.AddRange(new[] { colName, colStatus, colType, colPath });
            _folvCodecs.RebuildColumns();

            mainLayout.Controls.Add(_folvCodecs, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing DirectShow and Media Foundation codec registrations...",
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
            _btnRefresh = new Button { Text = "Refresh Codecs", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanCodecs();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanCodecs()
        {
            _items = MediaCodecResidualsAuditorEngine.ScanMediaCodecs();
            _folvCodecs.SetObjects(_items);

            var missingCount = _items.Count(c => c.IsBinaryMissing);
            var validCount = _items.Count(c => !c.IsBinaryMissing);

            if (missingCount == 0)
            {
                _lblSummary.Text = $"All {validCount} registered DirectShow media codec(s) have verified server binaries.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {missingCount} orphaned media codec(s) with missing DLLs (Potential playback/thumbnail crash causes).";
                _lblSummary.ForeColor = Color.DarkOrange;
            }
        }
    }
}
