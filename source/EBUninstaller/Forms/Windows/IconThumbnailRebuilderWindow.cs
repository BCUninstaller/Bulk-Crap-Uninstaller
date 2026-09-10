/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Shell Icon & Thumbnail Database Rebuilder Window
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
    public sealed class IconThumbnailRebuilderWindow : Form
    {
        private FastObjectListView _folvCaches;
        private Label _lblSummary;
        private Button _btnRebuild;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<ShellCacheDatabaseItem> _items = new();

        public IconThumbnailRebuilderWindow()
        {
            InitializeComponent();
            ScanCaches();
        }

        private void InitializeComponent()
        {
            Text = "Windows Shell Icon & Thumbnail Cache Database Rebuilder - EBUninstaller Pro";
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

            _folvCaches = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("Cache Database File", nameof(ShellCacheDatabaseItem.FileName)) { Width = 220 };
            var colType = new OLVColumn("Cache Type", nameof(ShellCacheDatabaseItem.CacheType)) { Width = 180 };
            var colSize = new OLVColumn("Database Size", nameof(ShellCacheDatabaseItem.SizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colPath = new OLVColumn("Full Storage Path", nameof(ShellCacheDatabaseItem.FullPath)) { Width = 400, FillsFreeSpace = true };

            _folvCaches.AllColumns.AddRange(new[] { colName, colType, colSize, colPath });
            _folvCaches.RebuildColumns();

            mainLayout.Controls.Add(_folvCaches, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Shell iconcache and thumbcache database files...",
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
            _btnRefresh = new Button { Text = "Refresh", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanCaches();

            _btnRebuild = new Button
            {
                Text = "Purge & Rebuild All Caches",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnRebuild.Click += OnRebuildClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnRebuild);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanCaches()
        {
            _items = IconThumbnailDatabaseRebuilderEngine.ScanCacheDatabases();
            _folvCaches.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.SizeBytes);
            _lblSummary.Text = $"Detected {_items.Count} Shell icon/thumbnail cache database(s) ({FormatBytes(totalBytes)}). Rebuilding will fix corrupted/blank icons.";
            _lblSummary.ForeColor = Color.DarkGreen;
        }

        private void OnRebuildClick(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Windows Explorer will briefly restart to delete locked cache databases and rebuild fresh icon indexes.\n\nDo you want to proceed?",
                "Confirm Cache Rebuild",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (cleaned, freed) = IconThumbnailDatabaseRebuilderEngine.PurgeAndRebuildCaches(_items);
                MessageBox.Show($"Purged {cleaned} cache databases (Freed {FormatBytes(freed)}). Explorer restarted successfully.", "Rebuild Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanCaches();
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
