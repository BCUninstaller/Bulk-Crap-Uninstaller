/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Search Indexer Residuals & Catalog Rebuilder Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class SearchIndexerCleanerWindow : Form
    {
        private Label _lblStatus;
        private Label _lblPath;
        private Label _lblSize;
        private Label _lblService;
        private Button _btnRebuild;
        private Button _btnRefresh;
        private Button _btnClose;
        private SearchIndexStats _stats;

        public SearchIndexerCleanerWindow()
        {
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            Text = "Windows Search Indexer Residuals & Catalog Rebuilder - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Search Catalog Database (Windows.edb) Maintenance",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Windows Search continuously indexes files and application data. After removing many applications,\nthe search catalog often retains residual metadata and becomes bloated. Rebuilding purges\nall deleted application entries and optimizes search speed.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Search Catalog Statistics",
                Location = new Point(16, 105),
                Size = new Size(740, 140)
            };

            _lblPath = new Label { Location = new Point(16, 28), AutoSize = true, Text = "Database Path: -" };
            _lblSize = new Label { Location = new Point(16, 56), AutoSize = true, Text = "Database Size: -" };
            _lblService = new Label { Location = new Point(16, 84), AutoSize = true, Text = "Service Status: -" };
            _lblStatus = new Label { Location = new Point(16, 112), AutoSize = true, Text = "Indexed Roots: -" };

            group.Controls.AddRange(new Control[] { _lblPath, _lblSize, _lblService, _lblStatus });

            _btnRebuild = new Button
            {
                Text = "Reset & Rebuild Search Index",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(340, 265),
                Size = new Size(210, 32)
            };
            _btnRebuild.Click += OnRebuildClick;

            _btnRefresh = new Button
            {
                Text = "Refresh Stats",
                Location = new Point(560, 265),
                Size = new Size(100, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshStats();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(670, 265),
                Size = new Size(86, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _btnRebuild, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshStats()
        {
            _stats = SearchIndexerResidualsCleanerEngine.QuerySearchIndexStats();

            _lblPath.Text = $"Database Path: {_stats.CatalogPath}";
            _lblSize.Text = $"Database Size: {FormatBytes(_stats.CatalogSizeBytes)}";
            _lblService.Text = $"Service Status: {(_stats.IsServiceRunning ? "Running (SearchIndexer.exe)" : "Stopped")}";
            _lblStatus.Text = $"Configured Index Scopes: {_stats.IndexedLocationCount} site(s)";
        }

        private void OnRebuildClick(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Windows Search Service will restart and rebuild its search catalog database from scratch.\nThis will clear all obsolete indexed file metadata.\n\nDo you wish to proceed?",
                "Confirm Index Rebuild",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var success = SearchIndexerResidualsCleanerEngine.ResetAndRebuildIndex();
                if (success)
                {
                    MessageBox.Show("Search catalog rebuild initiated successfully. Windows Search will re-index active drives in the background.", "Rebuild Initiated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshStats();
                }
                else
                {
                    MessageBox.Show("Failed to restart Windows Search service. Ensure EBUninstaller Pro is running with Administrator privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F2} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }
    }
}
