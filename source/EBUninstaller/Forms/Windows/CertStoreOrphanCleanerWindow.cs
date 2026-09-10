/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Certificate Store Orphan & Residuals Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SecurityHardening;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class CertStoreOrphanCleanerWindow : Form
    {
        private FastObjectListView _folvCerts;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<OrphanCertItem> _items = new();

        public CertStoreOrphanCleanerWindow()
        {
            InitializeComponent();
            ScanCerts();
        }

        private void InitializeComponent()
        {
            Text = "Certificate Store Residuals & Expired Cert Cleaner - EBUninstaller Pro";
            Size = new Size(1000, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 420);
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

            _folvCerts = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colRisk = new OLVColumn("Risk / Type", nameof(OrphanCertItem.RiskCategory)) { Width = 180 };
            var colSubject = new OLVColumn("Subject (Issued To)", nameof(OrphanCertItem.Subject)) { Width = 220 };
            var colIssuer = new OLVColumn("Issuer (Authority)", nameof(OrphanCertItem.Issuer)) { Width = 200 };
            var colStore = new OLVColumn("Store", nameof(OrphanCertItem.StoreNameLabel)) { Width = 110 };
            var colExpiry = new OLVColumn("Expiry Date", nameof(OrphanCertItem.NotAfter))
            {
                Width = 110,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd")
            };
            var colThumb = new OLVColumn("Thumbprint", nameof(OrphanCertItem.Thumbprint)) { Width = 160, FillsFreeSpace = true };

            _folvCerts.AllColumns.AddRange(new[] { colRisk, colSubject, colIssuer, colStore, colExpiry, colThumb });
            _folvCerts.RebuildColumns();

            mainLayout.Controls.Add(_folvCerts, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows User and Machine certificate stores...",
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
            _btnRefresh.Click += (s, e) => ScanCerts();

            _btnClean = new Button
            {
                Text = "Remove Selected Certificates",
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

        private void ScanCerts()
        {
            _items = CertStoreOrphanCleanerEngine.ScanResidualCertificates();
            _folvCerts.SetObjects(_items);

            var expiredCount = _items.Count(c => c.IsExpired);
            var selfSignedCount = _items.Count(c => c.IsSelfSigned && !c.IsExpired);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No expired or untrusted residual certificates found. Certificate store is clean.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} residual certificate(s) ({expiredCount} expired, {selfSignedCount} self-signed / proxy certificates).";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvCerts.SelectedObjects.Count > 0 ? _folvCerts.SelectedObjects.Cast<OrphanCertItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to remove {selected.Count} selected certificate(s) from Windows stores?",
                "Confirm Certificate Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = CertStoreOrphanCleanerEngine.CleanCertificates(selected);
                MessageBox.Show($"Removed {cleaned} certificate(s) successfully.", "Certificate Removal Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanCerts();
            }
        }
    }
}
