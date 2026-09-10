/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Update Version Differ Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class SoftwareUpdateDifferWindow : Form
    {
        private FastObjectListView _folvDiff;
        private TextBox _txtOldDir;
        private TextBox _txtNewDir;
        private Label _lblSummary;
        private Button _btnCompare;
        private Button _btnClose;
        private SoftwareVersionDiffReport _diffReport;

        public SoftwareUpdateDifferWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Software Version & Package Update Differ - EBUninstaller Pro";
            Size = new Size(960, 540);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(750, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Paths inputs
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Paths Input Group
            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 8)
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblOld = new Label { Text = "Base / Old Folder:", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
            _txtOldDir = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 2, 8, 4) };
            var btnBrowseOld = new Button { Text = "Browse...", AutoSize = true };
            btnBrowseOld.Click += (s, e) => BrowseFolder(_txtOldDir);

            var lblNew = new Label { Text = "Updated / New Folder:", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
            _txtNewDir = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 2, 8, 4) };
            var btnBrowseNew = new Button { Text = "Browse...", AutoSize = true };
            btnBrowseNew.Click += (s, e) => BrowseFolder(_txtNewDir);

            topPanel.Controls.Add(lblOld, 0, 0);
            topPanel.Controls.Add(_txtOldDir, 1, 0);
            topPanel.Controls.Add(btnBrowseOld, 2, 0);

            topPanel.Controls.Add(lblNew, 0, 1);
            topPanel.Controls.Add(_txtNewDir, 1, 1);
            topPanel.Controls.Add(btnBrowseNew, 2, 1);

            mainLayout.Controls.Add(topPanel, 0, 0);

            // ObjectListView
            _folvDiff = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colChange = new OLVColumn("Change", nameof(SoftwareDiffItem.ChangeType)) { Width = 90 };
            var colType = new OLVColumn("Type", nameof(SoftwareDiffItem.FileType)) { Width = 150 };
            var colPath = new OLVColumn("Relative Path", nameof(SoftwareDiffItem.RelativePath)) { Width = 380 };
            var colOldSize = new OLVColumn("Old Size", nameof(SoftwareDiffItem.OldSizeBytes))
            {
                Width = 100,
                AspectToStringConverter = v => FormatSize((long)v)
            };
            var colNewSize = new OLVColumn("New Size", nameof(SoftwareDiffItem.NewSizeBytes))
            {
                Width = 100,
                AspectToStringConverter = v => FormatSize((long)v)
            };
            var colDelta = new OLVColumn("Size Delta", nameof(SoftwareDiffItem.SizeDelta))
            {
                Width = 100,
                AspectToStringConverter = v => FormatDelta((long)v)
            };

            _folvDiff.AllColumns.AddRange(new[] { colChange, colType, colPath, colOldSize, colNewSize, colDelta });
            _folvDiff.RebuildColumns();

            mainLayout.Controls.Add(_folvDiff, 0, 1);

            // Summary
            _lblSummary = new Label
            {
                Text = "Select base and updated software folders, then click 'Compare Versions'.",
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 8),
                Font = new Font(Font, FontStyle.Bold)
            };
            mainLayout.Controls.Add(_lblSummary, 0, 2);

            // Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK, AutoSize = true };
            _btnCompare = new Button { Text = "Compare Versions", AutoSize = true, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.DarkSlateBlue };
            _btnCompare.Click += OnCompareClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnCompare);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void BrowseFolder(TextBox target)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                target.Text = fbd.SelectedPath;
            }
        }

        private void OnCompareClick(object sender, EventArgs e)
        {
            var oldDir = _txtOldDir.Text?.Trim();
            var newDir = _txtNewDir.Text?.Trim();

            if (string.IsNullOrEmpty(oldDir) || string.IsNullOrEmpty(newDir))
            {
                MessageBox.Show("Please specify both base and updated directory paths.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _diffReport = SoftwareUpdateDifferEngine.CompareDirectories(oldDir, newDir);
            _folvDiff.SetObjects(_diffReport.Items);

            var added = _diffReport.AddedCount;
            var removed = _diffReport.RemovedCount;
            var modified = _diffReport.ModifiedCount;
            var delta = FormatDelta(_diffReport.NetSizeDeltaBytes);

            _lblSummary.Text = $"Comparison complete: {added} added, {removed} removed, {modified} modified. Net size change: {delta}";
            _lblSummary.ForeColor = Color.DarkGreen;
        }

        private static string FormatSize(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }

        private static string FormatDelta(long bytes)
        {
            if (bytes == 0) return "0 B";
            var prefix = bytes > 0 ? "+" : "-";
            var abs = Math.Abs(bytes);
            return prefix + FormatSize(abs);
        }
    }
}
