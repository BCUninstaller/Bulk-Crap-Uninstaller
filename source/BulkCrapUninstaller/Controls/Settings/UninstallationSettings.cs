/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using Klocman;
using Klocman.Binding.Settings;
using Klocman.IO;

namespace BulkCrapUninstaller.Controls
{
    public partial class UninstallationSettings : UserControl
    {
        public UninstallationSettings()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        
            if (DesignMode) return;

            var sb = Properties.Settings.Default.SettingBinder;
        
            // Shutdown blocking not available below Windows Vista
            if (Environment.OSVersion.Version < new Version(6, 0))
                checkBoxShutdown.Enabled = false;
            else
                sb.BindControl(checkBoxShutdown, settings => settings.UninstallPreventShutdown, this);
        
            checkBoxRestorePoint.Enabled = SysRestore.SysRestoreAvailable();
            sb.Bind(x => checkBoxRestorePoint.Checked = x != YesNoAsk.No, () => checkBoxRestorePoint.Checked &&  sb.Settings.MessagesRestorePoints != YesNoAsk.No ? sb.Settings.MessagesRestorePoints : checkBoxRestorePoint.Checked ? YesNoAsk.Ask : YesNoAsk.No,
                    eh => checkBoxRestorePoint.CheckedChanged += eh, eh => checkBoxRestorePoint.CheckedChanged -= eh,
                    settings => settings.MessagesRestorePoints, this);
        
            sb.BindControl(checkBoxConcurrent, settings => settings.UninstallConcurrency, this);
        
            sb.BindControl(checkBoxConcurrentOneLoud, settings => settings.UninstallConcurrentOneLoud, this);
            sb.BindControl(checkBoxManualNoCollisionProtection, settings => settings.UninstallConcurrentDisableManualCollisionProtection, this);
        
            sb.Subscribe(OnMaxCountChanged, settings => settings.UninstallConcurrentMaxCount, this);
            numericUpDownMaxConcurrent.ValueChanged += NumericUpDownMaxConcurrentOnValueChanged;
        
            sb.BindControl(checkBoxBatchSortQuiet, x => x.AdvancedIntelligentUninstallerSorting, this);
            sb.BindControl(checkBoxDiisableProtection, x => x.AdvancedDisableProtection, this);
            sb.BindControl(checkBoxSimulate, x => x.AdvancedSimulate, this);
        
            sb.BindControl(checkBoxAutoKillQuiet, x => x.QuietAutoKillStuck, this);
            sb.BindControl(checkBoxRetryQuiet, x => x.QuietRetryFailedOnce, this);
            sb.BindControl(checkBoxGenerate, x => x.QuietAutomatization, this);
            sb.BindControl(checkBoxGenerateStuck, x => x.QuietAutomatizationKillStuck, this);
            sb.BindControl(checkBoxAutoDaemon, x => x.QuietUseDaemon, this);
        
            sb.Subscribe((sender, args) => checkBoxGenerateStuck.Enabled = args.NewValue, settings => settings.QuietAutomatization, this);
        
            sb.Subscribe(
                (x, y) => checkBoxSimulate.ForeColor = y.NewValue ? Color.OrangeRed : SystemColors.ControlText,
                x => x.AdvancedSimulate, this);
        
            sb.SendUpdates(this);
            Disposed += (x, y) => sb.RemoveHandlers(this);
        }

        private void NumericUpDownMaxConcurrentOnValueChanged(object sender, EventArgs eventArgs)
        {
            Properties.Settings.Default.UninstallConcurrentMaxCount = (int)numericUpDownMaxConcurrent.Value;
        }

        private void OnMaxCountChanged(object sender, SettingChangedEventArgs<int> args)
        {
            numericUpDownMaxConcurrent.Value = Math.Clamp(args.NewValue, numericUpDownMaxConcurrent.Minimum, numericUpDownMaxConcurrent.Maximum);
        }
    }
}
