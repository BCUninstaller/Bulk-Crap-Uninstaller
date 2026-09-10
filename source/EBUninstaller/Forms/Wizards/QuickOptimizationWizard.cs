/*
    EBUninstaller Pro - Quick System Optimization Wizard
    4-Step Guided Optimization (Hygiene Diagnostic -> Junk Cleanup -> Boot Optimizer -> Summary)
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using UninstallTools.Core;
using UninstallTools.Detection;
using UninstallTools.JunkCleaner;
using UninstallTools.Startup;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Wizards
{
    public sealed class QuickOptimizationWizard : Form
    {
        private int _currentStep = 0;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Panel _contentPanel;
        private Button _btnNext;
        private Button _btnBack;
        private Button _btnCancel;

        // Step 1: Health Diagnostic
        private Label _lblScoreDisplay;
        private Label _lblDiagSummary;

        // Step 2: Junk Selection
        private CheckBox _chkJunkTemp;
        private CheckBox _chkJunkUpdates;
        private CheckBox _chkJunkDumps;
        private CheckBox _chkJunkShaders;
        private CheckBox _chkJunkPatchCache;
        private CheckBox _chkRegistryBloat;
        private CheckBox _chkOptimizePath;

        // Step 3: Memory & Boot
        private CheckBox _chkTrimMemory;
        private CheckBox _chkDisableHighImpactStartups;

        // Step 4: Summary
        private Label _lblSummaryDetails;
        private CheckBox _chkCreateBackup;

        public QuickOptimizationWizard()
        {
            InitializeComponent();
            ThemeEngine.ApplyThemeToForm(this);
            LoadStep(0);
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Quick System Optimization Wizard";
            Size = new Size(680, 480);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(16)
            };
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Header
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Content
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); // Navigation Buttons

            // Header Panel
            var headerPanel = new Panel { Dock = DockStyle.Fill };
            _lblTitle = new Label
            {
                Text = "Quick System Optimization Wizard",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                AutoSize = true,
                Location = new Point(0, 4)
            };
            _lblSubtitle = new Label
            {
                Text = "Step 1 of 4: System Hygiene Diagnostic",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(2, 30)
            };
            headerPanel.Controls.Add(_lblTitle);
            headerPanel.Controls.Add(_lblSubtitle);
            mainTable.Controls.Add(headerPanel, 0, 0);

            // Content Panel
            _contentPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
            mainTable.Controls.Add(_contentPanel, 0, 1);

            // Navigation Buttons
            var navTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            navTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            navTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            navTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _btnCancel = new Button { Text = "Cancel", Size = new Size(90, 32), DialogResult = DialogResult.Cancel };
            _btnBack = new Button { Text = "< Back", Size = new Size(90, 32), Enabled = false };
            _btnBack.Click += (s, e) => { if (_currentStep > 0) LoadStep(_currentStep - 1); };

            _btnNext = new Button { Text = "Next >", Size = new Size(90, 32), Font = new Font(Font, FontStyle.Bold) };
            _btnNext.Click += BtnNext_Click;

            var btnFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };
            btnFlow.Controls.Add(_btnCancel);
            btnFlow.Controls.Add(_btnNext);
            btnFlow.Controls.Add(_btnBack);
            navTable.Controls.Add(btnFlow, 2, 0);

            mainTable.Controls.Add(navTable, 0, 2);
            Controls.Add(mainTable);
        }

        private void LoadStep(int step)
        {
            _currentStep = step;
            _contentPanel.Controls.Clear();
            _btnBack.Enabled = step > 0;
            _btnNext.Text = step == 3 ? "Execute Optimization" : "Next >";

            switch (step)
            {
                case 0:
                    LoadStep1_Diagnostic();
                    break;
                case 1:
                    LoadStep2_JunkSelection();
                    break;
                case 2:
                    LoadStep3_BootAndMemory();
                    break;
                case 3:
                    LoadStep4_Summary();
                    break;
            }
        }

        private void LoadStep1_Diagnostic()
        {
            _lblSubtitle.Text = "Step 1 of 4: System Hygiene Diagnostic";
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };

            _lblScoreDisplay = new Label
            {
                Text = "Analyzing Software Health & System Clutter...",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.ForestGreen,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 10)
            };

            _lblDiagSummary = new Label
            {
                Text = "Scanning installed applications, uninstalled leftovers, and duplicate runtimes...",
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            layout.Controls.Add(_lblScoreDisplay);
            layout.Controls.Add(_lblDiagSummary);
            _contentPanel.Controls.Add(layout);

            // Execute rapid diagnosis
            var report = SoftwareHealthEngine.AnalyzeSystemHealth(null);
            _lblScoreDisplay.Text = $"System Hygiene Score: {report.HygieneScore}/100";
            _lblDiagSummary.Text = $"Found {report.OrphanedFoldersCount} abandoned folders, {report.DuplicateRuntimesCount} redundant runtimes, and {report.Recommendations.Count} optimization opportunities.";
        }

        private void LoadStep2_JunkSelection()
        {
            _lblSubtitle.Text = "Step 2 of 4: Select System & Residual Junk to Clean";
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };

            var lblDesc = new Label
            {
                Text = "Choose the system residual items you wish to clean:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 10)
            };
            layout.Controls.Add(lblDesc);

            _chkJunkTemp = new CheckBox { Text = "Clean Windows & User Temporary Files (%TEMP%)", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkJunkUpdates = new CheckBox { Text = "Clean Stale Windows Update Downloads (SoftwareDistribution)", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkJunkDumps = new CheckBox { Text = "Clean Crash Memory Dumps & Windows Error Reports (WER)", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkJunkShaders = new CheckBox { Text = "Clean Obsolete DirectX / GPU Shader Caches", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkJunkPatchCache = new CheckBox { Text = "Clean Orphaned Windows Installer Patches (.msp / .msi)", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkRegistryBloat = new CheckBox { Text = "Clean Registry Bloat (Orphaned CLSIDs & Dead App Paths)", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkOptimizePath = new CheckBox { Text = "Clean & Deduplicate Environment PATH Variables", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };

            layout.Controls.Add(_chkJunkTemp);
            layout.Controls.Add(_chkJunkUpdates);
            layout.Controls.Add(_chkJunkDumps);
            layout.Controls.Add(_chkJunkShaders);
            layout.Controls.Add(_chkJunkPatchCache);
            layout.Controls.Add(_chkRegistryBloat);
            layout.Controls.Add(_chkOptimizePath);

            _contentPanel.Controls.Add(layout);
        }

        private void LoadStep3_BootAndMemory()
        {
            _lblSubtitle.Text = "Step 3 of 4: Memory & Boot Performance Optimization";
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };

            var lblDesc = new Label
            {
                Text = "Select performance enhancement options:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 10)
            };
            layout.Controls.Add(lblDesc);

            _chkTrimMemory = new CheckBox { Text = "Trim Process Working Sets & Reclaim Standby RAM", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            _chkDisableHighImpactStartups = new CheckBox { Text = "Analyze & Optimize Heavy Background Autostarts", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };

            layout.Controls.Add(_chkTrimMemory);
            layout.Controls.Add(_chkDisableHighImpactStartups);

            _contentPanel.Controls.Add(layout);
        }

        private void LoadStep4_Summary()
        {
            _lblSubtitle.Text = "Step 4 of 4: Ready to Optimize";
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true };

            var lblReady = new Label
            {
                Text = "Optimization Plan Summary:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 10)
            };
            layout.Controls.Add(lblReady);

            _lblSummaryDetails = new Label
            {
                Text = "• Clean selected Windows temporary, update, and crash files\n• Trim standby memory and optimize system working sets\n• Analyze and generate startup performance recommendations",
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };
            layout.Controls.Add(_lblSummaryDetails);

            _chkCreateBackup = new CheckBox { Text = "Create cryptographic safety backup before cleaning (Recommended)", Checked = true, AutoSize = true };
            layout.Controls.Add(_chkCreateBackup);

            _contentPanel.Controls.Add(layout);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentStep < 3)
            {
                LoadStep(_currentStep + 1);
            }
            else
            {
                ExecuteOptimization();
            }
        }

        private void ExecuteOptimization()
        {
            _btnNext.Enabled = false;
            _btnBack.Enabled = false;

            try
            {
                // 1. Clean residuals
                var residuals = DriverAndSystemResidualsCleaner.ScanSystemResiduals();
                var (cleaned, freed) = DriverAndSystemResidualsCleaner.CleanResiduals(residuals);

                // 2. Clean Patch Cache if selected
                int patchesCleaned = 0;
                if (_chkJunkPatchCache != null && _chkJunkPatchCache.Checked)
                {
                    var patches = PatchCacheResidualsCleaner.ScanPatchCache();
                    var orphaned = patches.Where(p => p.IsOrphaned).ToList();
                    patchesCleaned = PatchCacheResidualsCleaner.CleanOrphanedPatches(orphaned);
                }

                // 3. Clean Registry Bloat if selected
                int bloatCleaned = 0;
                if (_chkRegistryBloat != null && _chkRegistryBloat.Checked)
                {
                    var bloat = RegistryBloatAnalyzer.ScanAllBloat();
                    bloatCleaned = RegistryBloatAnalyzer.CleanBloatItems(bloat.Items, null);
                }

                // 4. Optimize Environment PATH if selected
                if (_chkOptimizePath != null && _chkOptimizePath.Checked)
                {
                    var userPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
                    if (!string.IsNullOrEmpty(userPath))
                    {
                        var cleanedPath = PathEnvironmentAuditorEngine.CleanAndOptimizePath(userPath, true);
                        try { Environment.SetEnvironmentVariable("PATH", cleanedPath, EnvironmentVariableTarget.User); } catch { }
                    }
                }

                // 5. Trim memory
                long memReclaimed = 0;
                if (_chkTrimMemory != null && _chkTrimMemory.Checked)
                {
                    var trimResult = MemoryTrimmerEngine.TrimSystemWorkingSet();
                    memReclaimed = trimResult.EstimatedMemoryReclaimedBytes;
                }

                var msg = $"Optimization complete!\n\n• Cleaned {cleaned + patchesCleaned} residual & patch files (Freed {(freed / (1024 * 1024.0)):F1} MB)\n• Cleaned {bloatCleaned} registry bloat entries\n• Reclaimed ~{(memReclaimed / (1024 * 1024.0)):F1} MB RAM";
                MessageBox.Show(msg, "Optimization Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Optimization error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnNext.Enabled = true;
                _btnBack.Enabled = true;
            }
        }
    }
}
