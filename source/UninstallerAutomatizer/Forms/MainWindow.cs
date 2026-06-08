/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallerAutomatizer
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            _handler = new UninstallHandler();
            InitializeComponent();

            try
            {
                Icon = ProcessTools.GetIconFromEntryExe();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _timeSinceStart = Process.GetCurrentProcess().StartTime.ToUniversalTime();
        }

        private readonly UninstallHandler _handler;
        private readonly DateTime _timeSinceStart;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            _handler.StatusUpdate += HandlerOnStatusUpdate;
            _handler.Start();

            if (_handler.IsDaemon)
                WindowState = FormWindowState.Minimized;

            timerOpacity.Start();
        }

        private void HandlerOnStatusUpdate(object sender, UninstallHandlerUpdateArgs uninstallHandlerUpdateArgs)
        {
            this.SafeInvoke(() => AddLogEntry(uninstallHandlerUpdateArgs.Message, uninstallHandlerUpdateArgs.UpdateKind));
        }

        private void AddLogEntry(string message, UninstallHandlerUpdateKind updateKind)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var timeSinceStart = DateTime.UtcNow - _timeSinceStart;
                var timeStr = "[" + Math.Round(timeSinceStart.TotalSeconds) + "s] ";
                var fullMessage = timeStr + message;

                if (textBoxStatus.TextLength > 0)
                    textBoxStatus.AppendText("\r\n");
                textBoxStatus.AppendText(fullMessage);

                Console.WriteLine(fullMessage);
            }

            switch (updateKind)
            {
                case UninstallHandlerUpdateKind.Normal:
                    break;
                case UninstallHandlerUpdateKind.Failed:
                    CloseAfterDelay(3000);
                    break;
                case UninstallHandlerUpdateKind.Succeeded:
                    CloseAfterDelay(1000);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void CloseAfterDelay(int interval)
        {
            buttonAbort.Enabled = false;
            timerClose.Interval = interval;
            timerClose.Start();
        }
        
        [Browsable(false)]
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRunning
        {
            get { return buttonPause.Visible; }
            set
            {
                if (value)
                    _handler.Resume();
                else
                    _handler.Pause();

                buttonPause.Visible = value;
                buttonResume.Visible = !value;
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            IsRunning = false;
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            IsRunning = true;
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            _handler.Abort();
            Enabled = false;
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBoxUninstallerVisible_CheckedChanged(object sender, EventArgs e)
        {
            AutomatedUninstallManager.HideAutomatizedWindows = !checkBoxUninstallerVisible.Checked;
        }

        private void timerOpacity_Tick(object sender, EventArgs e)
        {
            if (ContainsFocus || ClientRectangle.Contains(PointToClient(MousePosition)))
                Opacity = Math.Min(1, Opacity + 0.1);
            else
                Opacity = Math.Max(0.4, Opacity - 0.1);
        }
    }
}
