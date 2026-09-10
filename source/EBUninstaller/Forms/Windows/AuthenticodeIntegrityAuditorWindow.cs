/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Authenticode Binary Signature Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class AuthenticodeIntegrityAuditorWindow : Form
    {
        private FastObjectListView _folvCerts;
        private Label _lblSummary;
        private TextBox _tbPath;
        private Button _btnBrowse;
        private Button _btnScan;
        private Button _btnClose;
        private List<AuthenticodeAuditResult> _results = new();

        public AuthenticodeIntegrityAuditorWindow()
        {
            InitializeComponent();
            _tbPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            RunAudit();
        }

        private void InitializeComponent()
        {
            Text = "Authenticode Digital Signature & Certificate Auditor - EBUninstaller Pro";
            Size = new Size(1000, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Top bar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Top Bar
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            var lblTarget = new Label { Text = "Audit Folder / Application:", AutoSize = true, Padding = new Padding(0, 6, 8, 0) };
            _tbPath = new TextBox { Width = 500 };
            _btnBrowse = new Button { Text = "Browse...", AutoSize = true };
            _btnBrowse.Click += OnBrowseClick;
            _btnScan = new Button { Text = "Audit Signatures", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            _btnScan.Click += (s, e) => RunAudit();

            topPanel.Controls.AddRange(new Control[] { lblTarget, _tbPath, _btnBrowse, _btnScan });
            mainLayout.Controls.Add(topPanel, 0, 0);

            // List
            _folvCerts = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colFile = new OLVColumn("Executable / Binary File", nameof(AuthenticodeAuditResult.FileName)) { Width = 190 };
            var colStatus = new OLVColumn("Signature Status", nameof(AuthenticodeAuditResult.IntegrityStatus)) { Width = 180 };
            var colSigner = new OLVColumn("Signer / Subject", nameof(AuthenticodeAuditResult.SignerSubject)) { Width = 220 };
            var colIssuer = new OLVColumn("Certificate Authority (Issuer)", nameof(AuthenticodeAuditResult.Issuer)) { Width = 200 };
            var colExpiry = new OLVColumn("Valid Until", nameof(AuthenticodeAuditResult.ValidTo)) { Width = 100 };

            _folvCerts.AllColumns.AddRange(new[] { colFile, colStatus, colSigner, colIssuer, colExpiry });
            _folvCerts.RebuildColumns();

            mainLayout.Controls.Add(_folvCerts, 0, 1);

            // Summary
            _lblSummary = new Label
            {
                Text = "Auditing binary Authenticode signatures and digital certificates...",
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
            btnPanel.Controls.Add(_btnClose);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void OnBrowseClick(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = _tbPath.Text;
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                _tbPath.Text = fbd.SelectedPath;
                RunAudit();
            }
        }

        private void RunAudit()
        {
            var p = _tbPath.Text;
            if (!Directory.Exists(p)) return;

            _results = AuthenticodeIntegrityAuditorEngine.AuditDirectory(p);
            _folvCerts.SetObjects(_results);

            var signedCount = _results.Count(r => r.IsSigned && r.IsValid);
            var unsignedCount = _results.Count(r => !r.IsSigned);
            var expiredCount = _results.Count(r => r.IsSigned && !r.IsValid);

            _lblSummary.Text = $"Audited {_results.Count} binary file(s): {signedCount} verified valid, {unsignedCount} unsigned, {expiredCount} invalid/expired.";
            _lblSummary.ForeColor = unsignedCount > 0 ? Color.DarkOrange : Color.DarkGreen;
        }
    }
}
