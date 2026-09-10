/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Diagnostic Data & Session History Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class DiagnosticDataCleanerWindow : Form
    {
        private FastObjectListView _folvLogs;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<DiagnosticLogItem> _items = new();

        public DiagnosticDataCleanerWindow()
        {
            InitializeComponent();
            ScanLogs();
        }

        private void InitializeComponent()
        {
            Text = "Windows Diagnostic Data & Telemetry Logs Cleaner - EBUninstaller Pro";
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

            _folvLogs = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colType = new OLVColumn("Log Category", nameof(DiagnosticLogItem.LogType)) { Width = 150 };
            var colSize = new OLVColumn("File Size", nameof(DiagnosticLogItem.SizeBytes))
            {
                Width = 100,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colModified = new OLVColumn("Modified Date", nameof(DiagnosticLogItem.LastModified))
            {
                Width = 140,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };
            var colPath = new OLVColumn("File Path", nameof(DiagnosticLogItem.FilePath)) { Width = 450, FillsFreeSpace = true };

            _folvLogs.AllColumns.AddRange(new[] { colType, colSize, colModified, colPath });
            _folvLogs.RebuildColumns();

            mainLayout.Controls.Add(_folvLogs, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows DiagTrack, WER report queues, and CBS/DISM setup logs...",
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
            _btnRefresh.Click += (s, e) => ScanLogs();

            _btnClean = new Button
            {
                Text = "Clean Diagnostic Logs",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnClean.Click += OnCleanClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnClean);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanLogs()
        {
            _items = DiagnosticDataSessionCleanerEngine.ScanDiagnosticLogs();
            _folvLogs.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.SizeBytes);
            _lblSummary.Text = $"Discovered {_items.Count} diagnostic & telemetry log files. Reclaimable space: {FormatBytes(totalBytes)}.";
            _lblSummary.ForeColor = _items.Count > 0 ? Color.DarkOrange : Color.DarkGreen;
            _btnClean.Enabled = _items.Count > 0;
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            if (_items.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Clean {_items.Count} diagnostic and telemetry log files?",
                "Confirm Cleanup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cleaned, freed) = DiagnosticDataSessionCleanerEngine.CleanDiagnosticLogs(_items);
                MessageBox.Show($"Cleaned {cleaned} diagnostic log files (Freed {FormatBytes(freed)}).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanLogs();
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }
    }
}
