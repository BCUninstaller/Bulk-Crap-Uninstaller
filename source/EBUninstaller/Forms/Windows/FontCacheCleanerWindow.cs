/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Font Cache & Stale Font Registrations Cleaner Window
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
    public sealed class FontCacheCleanerWindow : Form
    {
        private FastObjectListView _folvFonts;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<StaleFontItem> _items = new();

        public FontCacheCleanerWindow()
        {
            InitializeComponent();
            ScanFonts();
        }

        private void InitializeComponent()
        {
            Text = "Windows Font Cache & Stale Font Registrations Cleaner - EBUninstaller Pro";
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

            _folvFonts = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colName = new OLVColumn("Font Display Name", nameof(StaleFontItem.FontName)) { Width = 250 };
            var colFile = new OLVColumn("Referenced Font File", nameof(StaleFontItem.FontFileName)) { Width = 200 };
            var colReason = new OLVColumn("Issue Reason", nameof(StaleFontItem.IssueReason)) { Width = 350, FillsFreeSpace = true };

            _folvFonts.AllColumns.AddRange(new[] { colName, colFile, colReason });
            _folvFonts.RebuildColumns();

            mainLayout.Controls.Add(_folvFonts, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Font registrations for missing font binaries...",
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
            _btnRefresh.Click += (s, e) => ScanFonts();

            _btnClean = new Button
            {
                Text = "Purge Font Cache & Clean Stale Entries",
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

        private void ScanFonts()
        {
            _items = FontCacheResidualsCleanerEngine.ScanStaleFonts();
            _folvFonts.SetObjects(_items);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "All registered fonts exist on disk. Windows Font Registry is consistent.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = true; // Still allow cache rebuild
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} stale font registration(s) pointing to missing font files.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                $"Clean {_items.Count} stale font registrations and rebuild Windows Font Cache databases?",
                "Confirm Font Cache Purge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cleaned, purged) = FontCacheResidualsCleanerEngine.PurgeFontCacheAndClean(_items);
                MessageBox.Show($"Cleaned {cleaned} stale font entries and purged {purged} font cache database(s).", "Font Cache Maintenance Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanFonts();
            }
        }
    }
}
