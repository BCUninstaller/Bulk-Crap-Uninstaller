/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    ETW AutoLogger Trace Sessions Cleaner Window
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
    public sealed class EtwSessionCleanerWindow : Form
    {
        private FastObjectListView _folvLoggers;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<EtwSessionResidualItem> _items = new();

        public EtwSessionCleanerWindow()
        {
            InitializeComponent();
            ScanLoggers();
        }

        private void InitializeComponent()
        {
            Text = "Event Tracing (ETW) AutoLogger Sessions Cleaner - EBUninstaller Pro";
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

            _folvLoggers = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colName = new OLVColumn("Trace Session Name", nameof(EtwSessionResidualItem.SessionName)) { Width = 260 };
            var colType = new OLVColumn("Classification", nameof(EtwSessionResidualItem.IsThirdParty))
            {
                Width = 140,
                AspectToStringConverter = v => (bool)v ? "Third-Party App Session" : "Windows Core Logger"
            };
            var colSize = new OLVColumn("Trace Log Size", nameof(EtwSessionResidualItem.LogSizeBytes))
            {
                Width = 120,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colPath = new OLVColumn("ETL Log Output Path", nameof(EtwSessionResidualItem.LogFileName)) { Width = 380, FillsFreeSpace = true };

            _folvLoggers.AllColumns.AddRange(new[] { colName, colType, colSize, colPath });
            _folvLoggers.RebuildColumns();

            mainLayout.Controls.Add(_folvLoggers, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Event Tracing for Windows (ETW) AutoLogger sessions...",
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
            _btnRefresh = new Button { Text = "Refresh Sessions", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanLoggers();

            _btnClean = new Button
            {
                Text = "Clean Third-Party AutoLoggers",
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

        private void ScanLoggers()
        {
            _items = EtwSessionResidualsCleanerEngine.ScanAutoLoggers();
            _folvLoggers.SetObjects(_items);

            var thirdPartyCount = _items.Count(i => i.IsThirdParty);

            if (thirdPartyCount == 0)
            {
                _lblSummary.Text = $"Found {_items.Count} active AutoLogger session(s). All are standard Windows system loggers.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {thirdPartyCount} third-party application trace session(s) writing background ETL logs.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvLoggers.SelectedObjects.Count > 0 ? _folvLoggers.SelectedObjects.Cast<EtwSessionResidualItem>().Where(i => i.IsThirdParty).ToList() : _items.Where(i => i.IsThirdParty).ToList();
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Remove {selected.Count} third-party ETW AutoLogger session(s) and associated ETL log files?",
                "Confirm ETW Cleanup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = EtwSessionResidualsCleanerEngine.CleanThirdPartyLoggers(selected);
                MessageBox.Show($"Removed {cleaned} third-party trace session(s).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanLoggers();
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
