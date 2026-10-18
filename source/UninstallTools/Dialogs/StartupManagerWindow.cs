/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.Tools;
using UninstallTools.Properties;
using UninstallTools.Startup;
using UninstallTools.Startup.Browser;
using UninstallTools.Startup.Normal;
using UninstallTools.Startup.Service;
using UninstallTools.Startup.Task;

namespace UninstallTools.Dialogs
{
    public partial class StartupManagerWindow : Form
    {
        private StartupManagerWindow()
        {
            InitializeComponent();
            ModernStyleController.ApplyOnLoad(this);

            comboBoxFilter.SelectedIndex = 0;

            columnHeader5.AspectToStringConverter = x => (!(bool)x).ToYesNo();
            listView1.FormatRow += (sender, args) =>
            {
                if (args.Model is StartupEntryBase seb)
                    args.Item.ForeColor = seb.Disabled ? SystemColors.GrayText : SystemColors.ControlText;
            };
        }

        private List<StartupEntryBase> AllItems { get; set; }

        private IEnumerable<StartupEntryBase> Selection => listView1.SelectedObjects.Cast<StartupEntryBase>();

        /// <summary>
        ///     Show startup manager dialog. Returns latest startup entry list.
        /// </summary>
        /// <param name="owner">Parent form</param>
        public static IEnumerable<StartupEntryBase> ShowManagerDialog(Form owner)
        {
            using (var window = new StartupManagerWindow())
            {
                if (owner != null)
                {
                    window.StartPosition = FormStartPosition.CenterParent;
                    window.Icon = owner.Icon;
                }
                window.ShowDialog(owner);
                return window.AllItems;
            }
        }

        public static StartupManagerWindow ShowManagerWindow()
        {
            var window = new StartupManagerWindow();
            try
            {
                window.Icon = ProcessTools.GetIconFromEntryExe();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            return window;
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            exportDialog.ShowDialog();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ReloadItems(sender, e);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            moveToRegistryToolStripMenuItem.Enabled = false;

            CheckState? enableCheckState = null;
            CheckState? allUserCheckState = null;
            foreach (var item in Selection)
            {
                e.Cancel = false;

                if (!enableCheckState.HasValue)
                    enableCheckState = item.Disabled ? CheckState.Unchecked : CheckState.Checked;
                else if (enableCheckState.Value != (item.Disabled ? CheckState.Unchecked : CheckState.Checked))
                    enableCheckState = CheckState.Indeterminate;

                if (item is StartupEntry normalStartupEntry)
                {
                    if (!allUserCheckState.HasValue)
                        allUserCheckState = normalStartupEntry.AllUsers ? CheckState.Checked : CheckState.Unchecked;
                    else if (allUserCheckState.Value !=
                             (normalStartupEntry.AllUsers ? CheckState.Checked : CheckState.Unchecked))
                        allUserCheckState = CheckState.Indeterminate;

                    if (!normalStartupEntry.IsRegKey)
                        moveToRegistryToolStripMenuItem.Enabled = true;
                }
            }

            enableToolStripMenuItem.Enabled = enableCheckState.HasValue;
            enableToolStripMenuItem.CheckState = enableCheckState ?? CheckState.Unchecked;

            runForAllUsersToolStripMenuItem.Enabled = allUserCheckState.HasValue;
            runForAllUsersToolStripMenuItem.CheckState = allUserCheckState ?? CheckState.Unchecked;
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var parts = Selection.Select(x => x.ToLongString()).ToArray();
            if (parts.Any())
            {
                try
                {
                    Clipboard.SetText(string.Join(Environment.NewLine, parts));
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "copying to clipboard");
                }
            }
        }

        private void ShowSecurityOrGenericError(Exception ex, string context, string extraInfo = null)
        {
            if (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
            {
                PremadeDialogs.GenericError(
                    $"Access denied while {context}." + (string.IsNullOrWhiteSpace(extraInfo) ? "" : $" {extraInfo}") +
                    $"\nError: {ex.Message}\n\n" +
                    "You may not have sufficient permissions to perform this operation. Here are some possible causes:\n" +
                    "1.\tService/File Permissions: The user (even admin) may lack permissions on the specific item. Security descriptors can restrict who can modify or delete a service/file.\n" +
                    "2.\tWMI Namespace Permissions: The user may lack permissions on the root\\CIMV2 namespace.\n" +
                    "3.\tService/File in Use: Some system services cannot be deleted or modified, even by administrators.\n" +
                    "4.\tAnti-malware/AV: Security software may block service modifications.");
            }
            else
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void DeleteSelected()
        {
            if (CustomMessageBox.ShowDialog(this, new CmbBasicSettings(
                Localisation.StartupManager_Message_Delete_Title, Localisation.StartupManager_Message_Delete_Header,
                Localisation.StartupManager_Message_Delete_Details,
                SystemIcons.Question, Buttons.ButtonRemove, Buttons.ButtonCancel)) ==
                CustomMessageBox.PressedButton.Middle)
            {
                try
                {
                    foreach (var item in Selection)
                    {
                        try
                        {
                            item.Delete();
                        }
                        catch (Exception ex)
                        {
                            ShowSecurityOrGenericError(ex, "deleting service", $"Service name: {item.ProgramName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "deleting service");
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected();

            ReloadItems(sender, e);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count > 0 && listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 && listView1.FocusedItem.Bounds.Contains(e.Location))
            {
                openFileLocationToolStripMenuItem_Click(sender, e);
            }
        }

        private void openFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in Selection)
            {
                try
                {
                    if (item.CommandFilePath == null)
                        throw new IOException(Localisation.Error_InvalidPath + "\n" + item.Command);
                    WindowsTools.OpenExplorerFocusedOnObject(item.CommandFilePath);
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "opening file location", $"Path: {item.CommandFilePath}");
                }
            }
        }

        private void openLinkLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StartupManager.OpenStartupEntryLocations(Selection);
            }
            catch (Exception ex)
            {
                ShowSecurityOrGenericError(ex, "opening link location");
            }
        }

        private void ReloadItems(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (listView1.Items.Count < 1)
            {
                listView1.EmptyListMsg = Localisation.StartupManager_Loading;
                listView1.Update();
            }

            SelectionChanged(sender, e);

            listView1.BeginUpdate();

            AllItems = StartupManager.GetAllStartupItems().ToList();

            // Get icons
            if (listView1.SmallImageList == null)
                listView1.SmallImageList = new ImageList();
            listView1.SmallImageList.Images.Clear();
            listView1.SmallImageList.Images.Add(SystemIcons.Warning);
            foreach (var entry in AllItems)
            {
                if (entry.CommandFilePath != null && !listView1.SmallImageList.Images.ContainsKey(entry.ProgramName))
                {
                    var icon = UninstallToolsGlobalConfig.TryExtractAssociatedIcon(entry.CommandFilePath);
                    if (icon != null)
                        listView1.SmallImageList.Images.Add(entry.ProgramName, icon);
                }
            }

            UpdateList(false);

            listView1.EndUpdate();
            Cursor = Cursors.Default;
            listView1.EmptyListMsg = null;
        }

        private void UpdateList(bool pauseLvUpdates = true)
        {
            if (pauseLvUpdates)
            {
                Cursor = Cursors.WaitCursor;
                listView1.BeginUpdate();
            }

            listView1.ClearObjects();

            listView1.Sort(columnHeader1, SortOrder.Ascending);

            var query = from item in AllItems
                        where comboBoxFilter.SelectedIndex == 0 ||
                              comboBoxFilter.SelectedIndex == 1 && item is StartupEntry ||
                              comboBoxFilter.SelectedIndex == 2 && item is TaskEntry ||
                              comboBoxFilter.SelectedIndex == 3 && item is BrowserHelperEntry ||
                              comboBoxFilter.SelectedIndex == 4 && item is ServiceEntry
                        orderby item.ProgramName ascending
                        select item;

            // Populate list items
            listView1.SetObjects(query);

            if (pauseLvUpdates)
            {
                listView1.EndUpdate();
                Cursor = Cursors.Default;
            }
        }

        private void runCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var command in Selection)
            {
                if (!PremadeDialogs.StartProcessSafely(command.Command))
                    break;
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var parts = Selection.Select(x => x.ToLongString()).ToArray();
            if (parts.Any())
            {
                try
                {
                    File.WriteAllText(exportDialog.FileName, string.Join(Environment.NewLine, parts));
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "saving file", $"File: {exportDialog.FileName}");
                    e.Cancel = true;
                }
            }
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            var sel = listView1.SelectedItems.Count > 0;
            buttonExport.Enabled = sel;
        }

        private void StartupManagerWindow_Shown(object sender, EventArgs e)
        {
            Refresh();
            ReloadItems(sender, e);
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableToolStripMenuItem.CheckState = enableToolStripMenuItem.CheckState == CheckState.Unchecked
                ? CheckState.Checked
                : CheckState.Unchecked;

            foreach (var item in Selection)
            {
                try
                {
                    item.Disabled = enableToolStripMenuItem.CheckState == CheckState.Unchecked;
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "enabling/disabling entry", $"Entry: {item.ProgramName}");
                }
            }

            UpdateList();
        }

        private void runForAllUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runForAllUsersToolStripMenuItem.CheckState = runForAllUsersToolStripMenuItem.CheckState ==
                                                         CheckState.Unchecked
                ? CheckState.Checked
                : CheckState.Unchecked;

            foreach (var item in Selection.OfType<StartupEntry>())
            {
                try
                {
                    item.AllUsers = runForAllUsersToolStripMenuItem.CheckState == CheckState.Checked;
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "setting run for all users", $"Entry: {item.ProgramName}");
                }
            }

            UpdateList();
        }

        private void moveToRegistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in Selection.OfType<StartupEntry>())
            {
                try
                {
                    StartupEntryManager.MoveToRegistry(item);
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "moving to registry", $"Entry: {item.ProgramName}");
                }
            }

            UpdateList();
        }

        private void createBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog(this);
            if (!Directory.Exists(folderBrowserDialog.SelectedPath)) return;

            foreach (var item in Selection)
            {
                try
                {
                    item.CreateBackup(folderBrowserDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    ShowSecurityOrGenericError(ex, "creating backup", $"Entry: {item.ProgramName}");
                }
            }

            try
            {
                Process.Start(new ProcessStartInfo(folderBrowserDialog.SelectedPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ShowSecurityOrGenericError(ex, "opening backup folder", $"Path: {folderBrowserDialog.SelectedPath}");
            }

            UpdateList();
        }

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AllItems != null)
                UpdateList();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}