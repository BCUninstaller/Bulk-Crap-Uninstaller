/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Print Spooler Residuals Cleaner Window
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
    public sealed class PrintSpoolerCleanerWindow : Form
    {
        private FastObjectListView _folvSpooler;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<SpoolerResidualItem> _items = new();

        public PrintSpoolerCleanerWindow()
        {
            InitializeComponent();
            ScanSpooler();
        }

        private void InitializeComponent()
        {
            Text = "Windows Print Spooler Residuals & Stuck Jobs Cleaner - EBUninstaller Pro";
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

            _folvSpooler = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colType = new OLVColumn("Item Type", nameof(SpoolerResidualItem.ItemType)) { Width = 180 };
            var colSize = new OLVColumn("File Size", nameof(SpoolerResidualItem.SizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colCreated = new OLVColumn("Creation Time", nameof(SpoolerResidualItem.CreationTime))
            {
                Width = 140,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };
            var colPath = new OLVColumn("Spooler File Path", nameof(SpoolerResidualItem.FilePath)) { Width = 450, FillsFreeSpace = true };

            _folvSpooler.AllColumns.AddRange(new[] { colType, colSize, colCreated, colPath });
            _folvSpooler.RebuildColumns();

            mainLayout.Controls.Add(_folvSpooler, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Print Spooler queue for stuck jobs and residual files...",
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
            _btnRefresh.Click += (s, e) => ScanSpooler();

            _btnClean = new Button
            {
                Text = "Purge Spooler Residuals",
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

        private void ScanSpooler()
        {
            _items = PrintSpoolerResidualsCleanerEngine.ScanSpoolerResiduals();
            _folvSpooler.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.SizeBytes);
            if (_items.Count == 0)
            {
                _lblSummary.Text = "Print spooler queue is clean. No stuck print jobs or orphaned spooler files found.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Found {_items.Count} stuck print job file(s) ({FormatBytes(totalBytes)}). Purging will unblock the Print Spooler.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            if (_items.Count == 0) return;

            var confirm = MessageBox.Show(
                "Windows Print Spooler service will restart to clear stuck print files.\n\nDo you want to proceed?",
                "Confirm Spooler Cleanup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cleaned, freed) = PrintSpoolerResidualsCleanerEngine.CleanSpoolerResiduals(_items);
                MessageBox.Show($"Purged {cleaned} spooler files (Freed {FormatBytes(freed)}). Spooler service restarted.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanSpooler();
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
