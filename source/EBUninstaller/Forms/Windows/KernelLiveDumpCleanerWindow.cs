/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Kernel LiveDump, MiniDump & WER Crash Cleaner Window
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
    public sealed class KernelLiveDumpCleanerWindow : Form
    {
        private FastObjectListView _folvDumps;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<CrashDumpResidualItem> _items = new();

        public KernelLiveDumpCleanerWindow()
        {
            InitializeComponent();
            ScanDumps();
        }

        private void InitializeComponent()
        {
            Text = "Kernel LiveDumps, MiniDumps & WER Crash Residuals Cleaner - EBUninstaller Pro";
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

            _folvDumps = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colType = new OLVColumn("Dump Classification", nameof(CrashDumpResidualItem.DumpType)) { Width = 220 };
            var colSize = new OLVColumn("File Size", nameof(CrashDumpResidualItem.SizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colCreated = new OLVColumn("Timestamp", nameof(CrashDumpResidualItem.CreationTime))
            {
                Width = 140,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };
            var colPath = new OLVColumn("Dump Path", nameof(CrashDumpResidualItem.FilePath)) { Width = 450, FillsFreeSpace = true };

            _folvDumps.AllColumns.AddRange(new[] { colType, colSize, colCreated, colPath });
            _folvDumps.RebuildColumns();

            mainLayout.Controls.Add(_folvDumps, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows crash dump directories (Minidump, LiveKernelReports, WER)...",
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
            _btnRefresh.Click += (s, e) => ScanDumps();

            _btnClean = new Button
            {
                Text = "Purge Selected Crash Dumps",
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

        private void ScanDumps()
        {
            _items = KernelLiveDumpCleanerEngine.ScanCrashDumps();
            _folvDumps.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.SizeBytes);
            if (_items.Count == 0)
            {
                _lblSummary.Text = "No crash dump residuals found. Crash dump repositories are clean.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Found {_items.Count} crash dump / report file(s) occupying {FormatBytes(totalBytes)}.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvDumps.SelectedObjects.Count > 0 ? _folvDumps.SelectedObjects.Cast<CrashDumpResidualItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete {selected.Count} crash dump / error report file(s)?",
                "Confirm Dump Cleanup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cleaned, freed) = KernelLiveDumpCleanerEngine.CleanCrashDumps(selected);
                MessageBox.Show($"Purged {cleaned} crash dump file(s) (Freed {FormatBytes(freed)}).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanDumps();
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
