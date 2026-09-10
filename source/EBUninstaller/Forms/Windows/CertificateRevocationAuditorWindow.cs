/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Software Code Signing Certificate & Revocation Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class CertificateRevocationAuditorWindow : Form
    {
        private FastObjectListView _folvCerts;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<SoftwareCertificateAuditResult> _results = new();
        private readonly List<ApplicationUninstallerEntry> _apps;

        public CertificateRevocationAuditorWindow(IEnumerable<ApplicationUninstallerEntry> apps = null)
        {
            _apps = apps?.ToList() ?? new List<ApplicationUninstallerEntry>();
            InitializeComponent();
            AuditCertificates();
        }

        private void InitializeComponent()
        {
            Text = "Software Code Signing & Certificate Revocation Auditor - EBUninstaller Pro";
            Size = new Size(980, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(750, 420);
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
                MultiSelect = false,
                GridLines = true
            };

            var colApp = new OLVColumn("Application", nameof(SoftwareCertificateAuditResult.ApplicationName)) { Width = 220 };
            var colStatus = new OLVColumn("Signature Status", nameof(SoftwareCertificateAuditResult.Status)) { Width = 110 };
            var colSubject = new OLVColumn("Signer / Subject", nameof(SoftwareCertificateAuditResult.SubjectName)) { Width = 180 };
            var colIssuer = new OLVColumn("Certificate Authority (Issuer)", nameof(SoftwareCertificateAuditResult.IssuerName)) { Width = 160 };
            var colAlg = new OLVColumn("Algorithm", nameof(SoftwareCertificateAuditResult.SignatureAlgorithm)) { Width = 90 };
            var colExpiry = new OLVColumn("Expiration Date", nameof(SoftwareCertificateAuditResult.ExpirationDate))
            {
                Width = 120,
                AspectToStringConverter = v => v != null ? ((DateTime)v).ToString("yyyy-MM-dd") : "-"
            };
            var colAssess = new OLVColumn("Security Assessment", nameof(SoftwareCertificateAuditResult.SecurityAssessment)) { Width = 280, FillsFreeSpace = true };

            _folvCerts.AllColumns.AddRange(new[] { colApp, colStatus, colSubject, colIssuer, colAlg, colExpiry, colAssess });
            _folvCerts.RebuildColumns();

            mainLayout.Controls.Add(_folvCerts, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Auditing Authenticode digital signatures across installed applications...",
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
            _btnRefresh = new Button { Text = "Refresh Audit", AutoSize = true };
            _btnRefresh.Click += (s, e) => AuditCertificates();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void AuditCertificates()
        {
            _results = CertificateRevocationAuditorEngine.AuditApplications(_apps);
            _folvCerts.SetObjects(_results);

            var valid = _results.Count(r => r.Status == CertValidityStatus.Valid);
            var expired = _results.Count(r => r.Status == CertValidityStatus.Expired);
            var unsigned = _results.Count(r => r.Status == CertValidityStatus.Unsigned);
            var weak = _results.Count(r => r.IsWeakAlgorithm);

            _lblSummary.Text = $"Audited {_results.Count} application(s): {valid} valid signatures, {expired} expired, {unsigned} unsigned, {weak} weak SHA-1 algorithms.";
            _lblSummary.ForeColor = expired > 0 || weak > 0 ? Color.DarkOrange : Color.DarkGreen;
        }
    }
}
