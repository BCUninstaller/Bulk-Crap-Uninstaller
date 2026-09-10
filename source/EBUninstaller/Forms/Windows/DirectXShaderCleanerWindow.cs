/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DirectX & GPU Shader Cache Cleaner Window
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
    public sealed class DirectXShaderCleanerWindow : Form
    {
        private FastObjectListView _folvShaders;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<ShaderCacheStoreItem> _items = new();

        public DirectXShaderCleanerWindow()
        {
            InitializeComponent();
            ScanShaders();
        }

        private void InitializeComponent()
        {
            Text = "DirectX & GPU Shader Cache Cleaner - EBUninstaller Pro";
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

            _folvShaders = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colArch = new OLVColumn("Shader Pipeline / GPU Store", nameof(ShaderCacheStoreItem.Architecture)) { Width = 230 };
            var colCount = new OLVColumn("Compiled Shaders", nameof(ShaderCacheStoreItem.FileCount)) { Width = 130 };
            var colSize = new OLVColumn("Cache Size", nameof(ShaderCacheStoreItem.TotalSizeBytes))
            {
                Width = 120,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colPath = new OLVColumn("Cache Store Directory", nameof(ShaderCacheStoreItem.DirectoryPath)) { Width = 380, FillsFreeSpace = true };

            _folvShaders.AllColumns.AddRange(new[] { colArch, colCount, colSize, colPath });
            _folvShaders.RebuildColumns();

            mainLayout.Controls.Add(_folvShaders, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing DirectX D3DSCache and GPU shader cache stores...",
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
            _btnRefresh.Click += (s, e) => ScanShaders();

            _btnClean = new Button
            {
                Text = "Purge Selected Shader Caches",
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

        private void ScanShaders()
        {
            _items = DirectXShaderCacheCleanerEngine.ScanShaderCaches();
            _folvShaders.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.TotalSizeBytes);
            var totalFiles = _items.Sum(i => i.FileCount);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No GPU shader cache stores found. Shader cache is clear.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {totalFiles} compiled shader(s) occupying {FormatBytes(totalBytes)} across {_items.Count} GPU cache store(s).";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvShaders.SelectedObjects.Count > 0 ? _folvShaders.SelectedObjects.Cast<ShaderCacheStoreItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Purge {selected.Count} GPU/DirectX shader cache store(s)?\n(Games will recompile fresh shaders on next run, resolving graphical glitches and stutter).",
                "Confirm Shader Purge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (files, bytes) = DirectXShaderCacheCleanerEngine.CleanShaderCaches(selected);
                MessageBox.Show($"Purged {files} shader binaries (Freed {FormatBytes(bytes)}).", "Shader Purge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanShaders();
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
