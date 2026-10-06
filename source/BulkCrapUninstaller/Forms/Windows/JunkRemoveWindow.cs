/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Localising;
using Klocman.Resources;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;

namespace BulkCrapUninstaller.Forms
{
    public partial class JunkRemoveWindow : Form
    {
        private static readonly string SelectionBoxText = Localisable.JunkRemove_SelectionBoxText;

        private static readonly string BackupDateFormat =
            new CultureInfo("en-US", false).DateTimeFormat.SortableDateTimePattern.Replace(':', '-').Replace('T', '_');

        private bool _confirmLowConfidenceMessageShown;
        private TypedObjectListView<IJunkResult> _listViewWrapper;

        public JunkRemoveWindow(IEnumerable<IJunkResult> junk)
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;
            Theming.ThemeManager.ApplyList(objectListViewMain);
            Theming.ThemeManager.ApplyControls(this, listViewContextMenuStrip);

            var junkNodes = junk as IList<IJunkResult> ?? junk.ToList();

            SetupListView(junkNodes);

            if (junkNodes.All(x => x.Confidence.GetRawConfidence() < 0))
            {
                _confirmLowConfidenceMessageShown = true;
                checkBoxHideLowConfidence.Checked = true;
                checkBoxHideLowConfidence.Enabled = false;
            }
            else if (junkNodes.All(x => x.Confidence.GetRawConfidence() >= 0))
                checkBoxHideLowConfidence.Enabled = false;

            new[] { ConfidenceLevel.VeryGood, ConfidenceLevel.Good, ConfidenceLevel.Questionable, ConfidenceLevel.Bad }
                .ForEach(x => comboBoxChecker.Items.Add(new LocalisedEnumWrapper(x)));
            comboBoxChecker_DropDownClosed(this, EventArgs.Empty);
        }

        public IEnumerable<IJunkResult> SelectedJunk => _listViewWrapper.CheckedObjects;

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            Enabled = false;
            try
            {
                var filters = SelectedJunk.OfType<FileSystemJunk>().Select(x => x.Path.FullName).Distinct().ToArray();
                if (!AppUninstaller.CheckForRunningProcesses(filters, false, this))
                    return;

                if (SelectedJunk.Any(x => !(x is FileSystemJunk)))
                {
                    if (Settings.Default.BackupLeftovers == YesNoAsk.Ask)
                    {
                        switch (MessageBoxes.BackupRegistryQuestion(this))
                        {
                            case MessageBoxes.PressedButton.Yes:
                                var path = MessageBoxes.SelectFolder(
                                    Localisable.JunkRemoveWindow_SelectBackupDirectoryTitle);

                                if (string.IsNullOrEmpty(path))
                                    return;

                                try
                                {
                                    CreateBackup(path);
                                    Settings.Default.BackupLeftoversDirectory = path;
                                }
                                catch (OperationCanceledException)
                                {
                                    goto case MessageBoxes.PressedButton.Yes;
                                }

                                break;

                            case MessageBoxes.PressedButton.No:
                                break;

                            default:
                                return;
                        }
                    }
                    else if (Settings.Default.BackupLeftovers == YesNoAsk.Yes)
                    {
                        while (true)
                        {
                            if (Directory.Exists(Settings.Default.BackupLeftoversDirectory))
                            {
                                try
                                {
                                    CreateBackup(Settings.Default.BackupLeftoversDirectory);
                                    break;
                                }
                                catch (OperationCanceledException)
                                {
                                }
                            }

                            Settings.Default.BackupLeftoversDirectory =
                                MessageBoxes.SelectFolder(Localisable.JunkRemoveWindow_SelectBackupDirectoryTitle);

                            if (string.IsNullOrEmpty(Settings.Default.BackupLeftoversDirectory))
                            {
                                Settings.Default.BackupLeftoversDirectory = string.Empty;
                                Settings.Default.BackupLeftovers = YesNoAsk.Ask;
                                return;
                            }
                        }
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            finally
            {
                Enabled = true;
            }
        }

        private void CreateBackup(string backupPath)
        {
            var dir = Path.Combine(backupPath, GetUniqueBackupName());
            if (string.IsNullOrEmpty(backupPath) || !Directory.Exists(backupPath))
            {
                PremadeDialogs.GenericError($"The parent directory does not exist:\n{backupPath}\n\nPlease choose a different backup location.");
                throw new OperationCanceledException();
            }
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                // https://github.com/dotnet/runtime/issues/24783
                if (ex is FileNotFoundException or UnauthorizedAccessException)
                    PremadeDialogs.GenericError("You do not have access to this path, choose a different path. If you use controlled folders, try turning them off or adding BCU to exclusions.");

                throw new OperationCanceledException();
            }

            try
            {
                FilesystemTools.CompressDirectory(dir, TimeSpan.FromMinutes(2));
            }
            catch
            {
                // Ignore, not important
            }

            RunBackup(dir);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            exportDialog.ShowDialog();
        }

        private void checkBoxHideLowConfidence_CheckedChanged(object sender, EventArgs e)
        {
            objectListViewMain.UpdateColumnFiltering();
        }

        private void checkBoxHideLowConfidence_Click(object sender, EventArgs e)
        {
            if (!checkBoxHideLowConfidence.Checked)
            {
                if (_confirmLowConfidenceMessageShown || MessageBoxes.ConfirmLowConfidenceQuestion(this))
                    _confirmLowConfidenceMessageShown = true;
                else
                    checkBoxHideLowConfidence.Checked = true;
            }
        }

        private void comboBoxChecker_DropDown(object sender, EventArgs e)
        {
            comboBoxChecker.Items.Remove(SelectionBoxText);
            //comboBoxChecker.ForeColor = SystemColors.WindowText;
        }

        private void comboBoxChecker_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxChecker.SelectedItem is LocalisedEnumWrapper localisedEnumWrapper)
            {
                var selectedConfidence = (ConfidenceLevel)localisedEnumWrapper.TargetEnum;

                if ((selectedConfidence != ConfidenceLevel.Bad && selectedConfidence != ConfidenceLevel.Questionable)
                    || MessageBoxes.ConfirmLowConfidenceQuestion(this)) //Ask if selected low confidence
                {
                    SelectUpTo(selectedConfidence);
                }
            }

            comboBoxChecker.Items.Add(SelectionBoxText);
            comboBoxChecker.SelectedItem = SelectionBoxText;
            //comboBoxChecker.ForeColor = SystemColors.Control;
        }

        private void comboBoxChecker_SelectedIndexChanged(object sender, EventArgs e)
        {
            objectListViewMain.BuildList(true);
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var items = objectListViewMain.SelectedObjects.Cast<IJunkResult>().Select(x => x.ToLongString()).ToArray();

                if (items.Any())
                {
                    Clipboard.SetText(string.Join(Environment.NewLine, items));
                }
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListViewMain.SelectedObject is not IJunkResult item) return;
            DisplayDetails(item);
        }

        private void DisplayDetails(IJunkResult item)
        {
            var groups = item.Confidence.ConfidenceParts.GroupBy(part => part.Change > 0).ToList();

            var positives = Localisable.Empty;
            if (groups.Any(x => x.Key))
            {
                var items = groups.First(x => x.Key)
                    .Where(x => x.Reason.IsNotEmpty())
                    .Select(x => x.Reason)
                    .ToArray();
                if (items.Any())
                    positives = string.Join("\n", items);
            }

            var negatives = Localisable.Empty;
            if (groups.Any(x => !x.Key))
            {
                var items = groups.First(x => !x.Key)
                    .Where(x => x.Reason.IsNotEmpty())
                    .Select(x => x.Reason)
                    .ToArray();
                if (items.Any())
                    negatives = string.Join("\n", items);
            }

            Theming.ThemeManager.ShowJunkDetails(this, string.Format(CultureInfo.CurrentCulture, Localisable.JunkRemove_Details_Message,
                item.Confidence.GetRawConfidence(), positives, negatives), Localisable.JunkRemove_Details_Title);
        }

        private void exportDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                File.WriteAllLines(exportDialog.FileName,
                    objectListViewMain.FilteredObjects.Cast<IJunkResult>().Select(x => x.ToLongString()).ToArray());
            }
            catch (Exception ex)
            {
                MessageBoxes.ExportFailed(ex.Message, this);
            }
        }

        private static string GetUniqueBackupName()
        {
            return "BCU Backup " + DateTime.Now.ToString(BackupDateFormat, CultureInfo.InvariantCulture);
        }

        private bool JunkListFilter(object obj)
        {
            if (obj is not IJunkResult item)
                return false;

            if (checkBoxHideLowConfidence.Checked && item.Confidence.GetRawConfidence() < 0)
                return false;

            return true;
        }

        private void objectListViewMain_CellEditStarting(object sender, CellEditEventArgs e)
        {
            e.Cancel = true;
            if (e.RowObject is not IJunkResult item) return;

            EnsureSingleSelection(e.ListViewItem);
            OpenJunkNodePreview(item);
        }

        private void objectListViewMain_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Model == null)
                return;

            EnsureSingleSelection(e.Item);

            e.MenuStrip = listViewContextMenuStrip;
        }

        private void EnsureSingleSelection(ListViewItem clickedItem)
        {
            if (objectListViewMain.SelectedItems.Count != 1)
            {
                objectListViewMain.DeselectAll();
                clickedItem.Selected = true;
            }
        }

        private static void OpenJunkNodePreview(IJunkResult item)
        {
            try
            {
                item.Open();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListViewMain.SelectedObject is not IJunkResult item) return;
            OpenJunkNodePreview(item);
        }

        private void RunBackup(string targetdir)
        {
            var failed = new List<string>();
            foreach (var junkNode in SelectedJunk)
            {
                try
                {
                    junkNode.Backup(targetdir);
                }
                catch (Exception ex)
                {
                    var displayName = junkNode.GetDisplayName();
                    failed.Add(ex.Message + " - " + displayName);
                    Console.WriteLine($"Backup failed for item {displayName} with exception: {ex}");
                }
            }

            if (failed.Any())
            {
                failed.Sort();

                // Prevent the dialog from getting too large
                if (failed.Count > 6) failed = failed.Take(5).Concat(new[] { "... (check log for the full list)" }).ToList();

                if (MessageBoxes.BackupFailedQuestion(string.Join("\n", failed.ToArray()), this)
                    != MessageBoxes.PressedButton.Yes)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private void SelectUpTo(ConfidenceLevel selectedConfidenceLevel)
        {
            objectListViewMain.BeginControlUpdate();
            objectListViewMain.BeginUpdate();

            objectListViewMain.DeselectAll();
            objectListViewMain.UncheckAll();

            objectListViewMain.CheckObjects(objectListViewMain.FilteredObjects.Cast<IJunkResult>()
                .Where(x => x.Confidence.GetConfidence() >= selectedConfidenceLevel).ToList());

            objectListViewMain.EndUpdate();
            objectListViewMain.EndControlUpdate();
        }

        private void SetupListView(IEnumerable<IJunkResult> junk)
        {
            _listViewWrapper = new TypedObjectListView<IJunkResult>(objectListViewMain);

            olvColumnSafety.AspectGetter = x => ((x as IJunkResult)?.Confidence?.GetConfidence() ?? ConfidenceLevel.Unknown).GetLocalisedName();
            olvColumnPath.GroupKeyGetter = x => (x as IJunkResult)?.Source?.CategoryName ?? CommonStrings.Unknown;
            olvColumnPath.AspectGetter = rowObject => (rowObject as IJunkResult)?.GetDisplayName();
            olvColumnUninstallerName.AspectGetter = rowObject =>
            {
                if (rowObject is not IJunkResult junkResult)
                    return null;

                var displayName = junkResult.Application?.DisplayName;
                if (!string.IsNullOrEmpty(displayName))
                    return displayName;

                var categoryName = junkResult.Source?.CategoryName;
                if (!string.IsNullOrEmpty(categoryName))
                    return categoryName;

                return Localisable.NotAvailable;
            };

            objectListViewMain.BeginUpdate();

            objectListViewMain.UseFiltering = true;
            objectListViewMain.AdditionalFilter = new ModelFilter(JunkListFilter);

            objectListViewMain.PrimarySortColumn = olvColumnUninstallerName;
            objectListViewMain.PrimarySortOrder = SortOrder.Ascending;

            objectListViewMain.SetObjects(junk);

            SelectUpTo(ConfidenceLevel.Good);

            objectListViewMain.EndUpdate();
        }
    }
}