/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Functions.ApplicationList;
using BulkCrapUninstaller.Functions.Tracking;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Forms.Tools;
using Klocman.IO;

namespace BulkCrapUninstaller.Forms
{
    public partial class DebugWindow : Form
    {
        private readonly UninstallerListViewUpdater _listView;
        private readonly MainWindow _reference;
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private readonly AppUninstaller _appUninstaller;

        internal DebugWindow(MainWindow reference, UninstallerListViewUpdater listview, AppUninstaller appUninstaller)
        {
            _reference = reference;
            _listView = listview;
            _appUninstaller = appUninstaller;

            InitializeComponent();

            if (DesignMode) return;

            _settings.Subscribe(TestHandler, x => x.FilterHideMicrosoft, this);
            _settings.BindControl(checkBox1, x => x.FilterHideMicrosoft, this);

            _settings.BindControl(checkBoxDebug, x => x.Debug, this);
            checkBoxDebug.Checked = Settings.Default.Debug;

            var messageboxes = typeof(MessageBoxes)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var x in messageboxes)
            {
                var wr = new ComboBoxWrapper<MethodInfo>(x, y =>
                {
                    var name = y.ToString();
                    return name!.Substring(name.IndexOf(' ') + 1);
                });
                comboBoxMessages.Items.Add(wr);
            }

            checkBox2.Checked = Program.IsInstalled;
            checkBoxSysRestoreAvail.Checked = SysRestore.SysRestoreAvailable();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            UsageManager.FinishCollectingData();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _settings.SendUpdates(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Default.FilterHideMicrosoft = !Settings.Default.FilterHideMicrosoft;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //UpdateSystem.BeginUpdate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(UpdateSystem.CheckForUpdates().ToString());
            //if (UpdateSystem.LastError != null)
            //    MessageBox.Show(UpdateSystem.LastError.Message);
            //
            //if (UpdateSystem.LatestReply != null)
            //{
            //    MessageBox.Show(UpdateSystem.LatestReply.FullReply.ToString());
            //    labelVersion.Text = UpdateSystem.LatestReply.GetUpdateVersion().ToString();
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (var settingsW = new SettingsWindow())
            {
                settingsW.ShowDialog();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UsageManager.SendUsageData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            EntryPoint.Restart();
        }

        private void buttonGoMessages_Click(object sender, EventArgs e)
        {
            try
            {
                var wrapper = comboBoxMessages.SelectedItem as ComboBoxWrapper<MethodInfo>;
                if (wrapper?.WrappedObject == null) return;

                var methodInfo = wrapper.WrappedObject;
                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 0)
                    methodInfo.Invoke(null, Array.Empty<object>());
                else
                {
                    var first = parameters.First();
                    if (first.ParameterType.IsArray)
                    {
                        methodInfo.Invoke(null, new object[] { textBoxMessages.Lines });
                    }
                    else if (first.ParameterType == typeof(int))
                    {
                        methodInfo.Invoke(null, new object[] { (int)numericUpDownMessages.Value });
                    }
                    else if (first.ParameterType == typeof(Form))
                    {
                        methodInfo.Invoke(null, new object[] { _reference });
                    }
                    else if (first.ParameterType == typeof(Exception))
                    {
#pragma warning disable CA2201 // Do not raise reserved exception types
                        methodInfo.Invoke(null, new object[] { new Exception(textBoxMessages.Text) });
#pragma warning restore CA2201 // Do not raise reserved exception types
                    }
                    else
                    {
                        methodInfo.Invoke(null, new object[] { textBoxMessages.Text });
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Program.IsInstalled = checkBox2.Checked;
        }

        private void TestCrashBackgroundThread(object sender, EventArgs e)
        {
            NBug.Settings.ReleaseMode = false;
            new Thread(() => throw new ArgumentException("TestCrashBackgroundThread", new IOException("Inner exception"))).Start();
        }

        private void TestCrashUiThread(object sender, EventArgs e)
        {
            NBug.Settings.ReleaseMode = false;
            throw new ArgumentException("TestCrashUiThread", new IOException("Inner exception"));
        }

        private void TestHandler(object sender, SettingChangedEventArgs<bool> args)
        {
            button2.Text = args.NewValue.ToString();
        }

        private void TestJunkSearcher(object sender, EventArgs e)
        {
            _appUninstaller.AdvancedUninstall(_listView.SelectedUninstallers,
                _listView.AllUninstallers.Except(_listView.SelectedUninstallers));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Join(Environment.NewLine,
                _reference.globalHotkeys1.GetHotkeyList().Select(x => x.ToString()).ToArray()));
        }

        private void SoftCrash(object sender, EventArgs e)
        {
            try
            {
                throw new ArithmeticException("Soft crash test", new ArgumentException("Yer a bit bored, eh?"));
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            FeedbackBox.ShowFeedbackBox(this, true);
        }

        private static long _currentRestoreId;
        private void button15_Click(object sender, EventArgs e)
        {
            SysRestore.StartRestore("BCU RESTORE POINT TEST " + DateTime.Now.ToShortTimeString(), out _currentRestoreId);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            SysRestore.EndRestore(_currentRestoreId);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            SysRestore.CancelRestore(_currentRestoreId);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            throw new DllNotFoundException(@"Unable to load DLL 'api-ms-win-core-com-l1-1-0.dll' or one of its dependencies: The specified module could not be found. (0x8007007E)");
        }
    }
}