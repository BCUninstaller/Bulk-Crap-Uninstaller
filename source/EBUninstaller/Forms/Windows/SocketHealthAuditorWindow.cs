/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Active Network Socket & Port Health Auditor Window
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
    public sealed class SocketHealthAuditorWindow : Form
    {
        private FastObjectListView _folvSockets;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<ActiveSocketInfo> _sockets = new();

        public SocketHealthAuditorWindow()
        {
            InitializeComponent();
            LoadSockets();
        }

        private void InitializeComponent()
        {
            Text = "Active Local Network Sockets & Port Security Auditor - EBUninstaller Pro";
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

            _folvSockets = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colProto = new OLVColumn("Protocol", nameof(ActiveSocketInfo.Protocol)) { Width = 80 };
            var colPort = new OLVColumn("Port", nameof(ActiveSocketInfo.LocalPort)) { Width = 80 };
            var colAddr = new OLVColumn("Bind Address", nameof(ActiveSocketInfo.LocalAddress)) { Width = 120 };
            var colState = new OLVColumn("State", nameof(ActiveSocketInfo.State)) { Width = 90 };
            var colName = new OLVColumn("Service Identification", nameof(ActiveSocketInfo.KnownServiceName)) { Width = 220 };
            var colAssess = new OLVColumn("Exposure & Security Assessment", nameof(ActiveSocketInfo.SecurityAssessment)) { Width = 300, FillsFreeSpace = true };

            _folvSockets.AllColumns.AddRange(new[] { colProto, colPort, colAddr, colState, colName, colAssess });
            _folvSockets.RebuildColumns();

            mainLayout.Controls.Add(_folvSockets, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing active TCP and UDP listening endpoints...",
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
            _btnRefresh = new Button { Text = "Refresh Sockets", AutoSize = true };
            _btnRefresh.Click += (s, e) => LoadSockets();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void LoadSockets()
        {
            _sockets = SocketHealthAuditorEngine.QueryActiveSockets();
            _folvSockets.SetObjects(_sockets);

            var publicCount = _sockets.Count(s => s.IsPubliclyExposed);
            var localCount = _sockets.Count(s => !s.IsPubliclyExposed);

            _lblSummary.Text = $"Detected {_sockets.Count} active listening socket(s) ({publicCount} publicly exposed, {localCount} loopback only).";
            _lblSummary.ForeColor = publicCount > 10 ? Color.DarkOrange : Color.DarkGreen;
        }
    }
}
