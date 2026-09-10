/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Error Reporting (WER) Crash Dump Policy Optimizer Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class WerCrashPolicyOptimizerWindow : Form
    {
        private CheckBox _cbLimitStorage;
        private CheckBox _cbPreventUpload;
        private Label _lblSummary;
        private Button _btnApply;
        private Button _btnClose;
        private WerPolicySettings _policy;

        public WerCrashPolicyOptimizerWindow()
        {
            InitializeComponent();
            LoadPolicy();
        }

        private void InitializeComponent()
        {
            Text = "Windows Error Reporting (WER) Crash Policy Optimizer - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Error Reporting & Crash Dump Storage Policy",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Unconfigured Windows Error Reporting can allow crashing applications to write full memory dumps\n(10+ GB each) repeatedly to the disk and transmit crash logs over the internet.\nOptimizing this policy enforces lightweight minidumps and suppresses outbound telemetry.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Optimization Options",
                Location = new Point(16, 105),
                Size = new Size(740, 120)
            };

            _cbLimitStorage = new CheckBox
            {
                Text = "Limit local crash dumps to lightweight Minidumps (Max 3 dumps, saves gigabytes of disk space)",
                Location = new Point(16, 30),
                AutoSize = true,
                Checked = true
            };

            _cbPreventUpload = new CheckBox
            {
                Text = "Disable outbound error telemetry upload (Prevent transmitting memory logs to Microsoft/Third-parties)",
                Location = new Point(16, 65),
                AutoSize = true,
                Checked = true
            };

            group.Controls.AddRange(new Control[] { _cbLimitStorage, _cbPreventUpload });

            _lblSummary = new Label
            {
                Location = new Point(16, 235),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Current Status: Loading..."
            };

            _btnApply = new Button
            {
                Text = "Apply Optimized Policy",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(480, 265),
                Size = new Size(180, 32)
            };
            _btnApply.Click += OnApplyClick;

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(670, 265),
                Size = new Size(86, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblSummary, _btnApply, _btnClose });
            Controls.Add(panel);
        }

        private void LoadPolicy()
        {
            _policy = WerCrashPolicyOptimizerEngine.QueryCurrentPolicy();
            _lblSummary.Text = $"Current State: Dump Limit: {_policy.DumpCount} files | Outbound Uploads: {(!_policy.DontSendAdditionalData ? "Enabled" : "Blocked")}";
        }

        private void OnApplyClick(object sender, EventArgs e)
        {
            var success = WerCrashPolicyOptimizerEngine.ApplyOptimizedPolicy(_cbLimitStorage.Checked, _cbPreventUpload.Checked);
            if (success)
            {
                MessageBox.Show("WER Crash Dump & Telemetry Policy updated successfully.", "Policy Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPolicy();
            }
            else
            {
                MessageBox.Show("Failed updating WER policy. Ensure Administrator rights.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
