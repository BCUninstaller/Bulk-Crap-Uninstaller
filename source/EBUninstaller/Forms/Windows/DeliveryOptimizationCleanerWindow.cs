/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Delivery Optimization & Package Cache Cleaner Window
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
    public sealed class DeliveryOptimizationCleanerWindow : Form
    {
        private FastObjectListView _folvCaches;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<PackageCacheGroup> _items = new();

        public DeliveryOptimizationCleanerWindow()
        {
            InitializeComponent();
            ScanCaches();
        }

        private void InitializeComponent()
        {
            Text = "Delivery Optimization & Package Cache Cleaner - EBUninstaller Pro";
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
                MultiSelect = true,
                GridLines = true
            };

            var colCategory = new OLVColumn("Cache Category", nameof(PackageCacheGroup.Category)) { Width = 250 };
            var colFiles = new OLVColumn("File Count", nameof(PackageCacheGroup.FileCount)) { Width = 110 };
            var colSize = new OLVColumn("Total Space", nameof(PackageCacheGroup.TotalSizeBytes))
            {
                Width = 120,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colPath = new OLVColumn("Location", nameof(PackageCacheGroup.DirectoryPath)) { Width = 380, FillsFreeSpace = true };

            _folvCaches.AllColumns.AddRange(new[] { colCategory, colFiles, colSize, colPath });
            _folvCaches.RebuildColumns();

            mainLayout.Controls.Add(_folvCaches, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Delivery Optimization and developer cache repositories...",
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
            _btnRefresh.Click += (s, e) => ScanCaches();

            _btnClean = new Button
            {
                Text = "Purge Selected Caches",
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

        private void ScanCaches()
        {
            _items = DeliveryOptimizationCleanerEngine.ScanPackageCaches();
            _folvCaches.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.TotalSizeBytes);
            var totalFiles = _items.Sum(i => i.FileCount);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No Delivery Optimization or developer cache residuals found. Caches are clear.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {totalFiles} cached file(s) across {_items.Count} cache store(s) taking up {FormatBytes(totalBytes)}.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvCaches.SelectedObjects.Count > 0 ? _folvCaches.SelectedObjects.Cast<PackageCacheGroup>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Purge {selected.Count} selected package cache location(s)?",
                "Confirm Cache Purge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (files, bytes) = DeliveryOptimizationCleanerEngine.CleanCaches(selected);
                MessageBox.Show($"Purged {files} files (Freed {FormatBytes(bytes)}).", "Cache Purge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanCaches();
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
