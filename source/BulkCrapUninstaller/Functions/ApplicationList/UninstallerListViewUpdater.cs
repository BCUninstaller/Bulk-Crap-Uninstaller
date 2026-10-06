/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools;
using UninstallTools.Controls;
using UninstallTools.Factory;
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class UninstallerListViewUpdater : IDisposable
    {
        private readonly UninstallerIconGetter _iconGetter;
        private readonly TypedObjectListView<ApplicationUninstallerEntry> _listView;
        private readonly MainWindow _reference;
        readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private IEnumerable<ApplicationUninstallerEntry> _allUninstallers;
        private bool _firstRefresh = true;
        private bool _listRefreshIsRunning;

        internal UninstallerListViewUpdater(MainWindow reference)
        {
            _reference = reference;
            _listView = new TypedObjectListView<ApplicationUninstallerEntry>(reference.uninstallerObjectListView);

            _iconGetter = new UninstallerIconGetter();
            _reference.olvColumnDisplayName.ImageGetter = _iconGetter.ColumnImageGetter;

            // Refresh items marked as invalid after corresponding setting change
            _settings.Subscribe((x, y) =>
            {
                if (CheckIsAppDisposed()) return;

                if (!_firstRefresh)
                    _listView.ListView.RefreshObjects(AllUninstallers.Where(u => !u.IsValid).ToList());
            }, x => x.AdvancedTestInvalid, this);

            // Refresh items marked as orphans after corresponding setting change
            _settings.Subscribe((x, y) =>
            {
                if (CheckIsAppDisposed()) return;

                if (!_firstRefresh)
                    _listView.ListView.UpdateColumnFiltering();
            }, x => x.AdvancedDisplayOrphans, this);

        }

        public IEnumerable<ApplicationUninstallerEntry> AllUninstallers
        {
            get { return _allUninstallers ?? Enumerable.Empty<ApplicationUninstallerEntry>(); }
            private set { _allUninstallers = value; }
        }

        public IEnumerable<ApplicationUninstallerEntry> FilteredUninstallers
            =>
                CheckIsAppDisposed()
                    ? AllUninstallers
                    : _listView.ListView.FilteredObjects.Cast<ApplicationUninstallerEntry>();

        public bool FirstRefreshCompleted => !_firstRefresh;

        public bool ListRefreshIsRunning
        {
            get { return _listRefreshIsRunning; }
            private set
            {
                if (value != _listRefreshIsRunning)
                {
                    _listRefreshIsRunning = value;
                    ListRefreshIsRunningChanged?.Invoke(this, new ListRefreshEventArgs(value, !FirstRefreshCompleted));
                }
            }
        }

        /// <summary>
        /// Faster than SelectedUninstallers.Count()
        /// </summary>
        public int SelectedUninstallerCount => _listView.ListView.CheckBoxes
            ? _listView.CheckedObjects.Count
            : _listView.SelectedObjects.Count;

        public IEnumerable<ApplicationUninstallerEntry> SelectedUninstallers => _listView.ListView.CheckBoxes
            ? _listView.ListView.GetAllObjectsWithMappedCheckState(CheckState.Checked).Cast<ApplicationUninstallerEntry>().Where(e => e != null && AllUninstallers.Contains(e))
            : _listView.SelectedObjects.Where(e => e != null);

        public void Dispose()
        {
            _iconGetter?.Dispose();
        }

        public event EventHandler<ListRefreshEventArgs> ListRefreshIsRunningChanged;

        private void ChangeSelection(IEnumerable<ApplicationUninstallerEntry> newSelection)
        {
            _listView.ListView.BeginUpdate();

            var items = newSelection.ToList();
            if (_listView.ListView.CheckBoxes)
                _listView.ListView.CheckedObjects = items;
            _listView.ListView.SelectedObjects = items;

            _listView.ListView.EndUpdate();
            _listView.ListView.Refresh();
            _listView.ListView.Focus();
        }

        public bool CheckIsAppDisposed()
        {
            return _listView.ListView.IsDisposed || _listView.ListView.Disposing
                   || _reference.IsDisposed || _reference.Disposing;
        }

        public void DeselectAllItems(object sender, EventArgs e)
        {
            var selected = _listView.ListView.CheckBoxes ? _listView.CheckedObjects : _listView.SelectedObjects;
            var subtracted = selected.Except(FilteredUninstallers);
            ChangeSelection(subtracted);
        }

        /// <summary>
        /// Get total size of all visible uninstallers.
        /// </summary>
        public FileSize GetFilteredSize()
        {
            return FilteredUninstallers.Select(x => x.EstimatedSize).DefaultIfEmpty(FileSize.Empty)
                .Aggregate((size1, size2) => size1 + size2);
        }

        /// <summary>
        /// Get total size of selected uninstallers
        /// </summary>
        /// <returns></returns>
        public FileSize GetSelectedSize()
        {
            return SelectedUninstallers.Select(x => x.EstimatedSize).DefaultIfEmpty(FileSize.Empty)
                .Aggregate((size1, size2) => size1 + size2);
        }

        public void InitiateListRefresh()
        {
            if (ListRefreshIsRunning || CheckIsAppDisposed())
                return;

            ListRefreshIsRunning = true;

            _reference.LockApplication(true);

            if (CheckIsAppDisposed())
                return;

            _listView.ListView.SuspendLayout();
            _listView.ListView.BeginUpdate();

            var dialog = LoadingDialog.Show(_reference, Localisable.LoadingDialogTitlePopulatingList,
                ListRefreshThread, new Point(-35, -35), ContentAlignment.BottomRight);

            Theming.ThemeManager.ApplyLoadingDialog(dialog);
            dialog.FormClosed += OnRefreshFinished;

            void OnRefreshFinished(object sender, FormClosedEventArgs args)
            {
                dialog.FormClosed -= OnRefreshFinished;

                if (dialog.Error != null)
                {
                    if (dialog.Error is OperationCanceledException)
                        return;
                    throw new InvalidOperationException("Uncaught exception in ListRefreshThread", dialog.Error);
                }

                if (CheckIsAppDisposed() || args.CloseReason == CloseReason.WindowsShutDown ||
                    args.CloseReason == CloseReason.ApplicationExitCall ||
                    args.CloseReason == CloseReason.FormOwnerClosing ||
                    args.CloseReason == CloseReason.TaskManagerClosing) return;

                var oldList = _listView.ListView.SmallImageList;
                _listView.ListView.SmallImageList = _iconGetter.IconList;
                oldList?.Dispose();

                _listView.ListView.SetObjects(AllUninstallers);

                try
                {
                    _listView.ListView.EndUpdate();
                    _listView.ListView.ResumeLayout();
                    _listView.ListView.Focus();
                }
                catch (ObjectDisposedException)
                {
                }

                _reference.LockApplication(false);

                // Run events
                ListRefreshIsRunning = false;

                // Set after running events
                _firstRefresh = false;
            }

            dialog.StartWork();
        }

        public void InvertSelectedItems(object sender, EventArgs e)
        {
            var selected = _listView.ListView.CheckBoxes ? _listView.CheckedObjects : _listView.SelectedObjects;
            var inverted = FilteredUninstallers.Except(selected);
            ChangeSelection(inverted);
        }

        private void ListRefreshThread(LoadingDialogInterface dialogInterface)
        {
            dialogInterface.SetSubProgressVisible(true);
            var progressMax = 0;
            var uninstallerEntries = ApplicationUninstallerFactory.GetUninstallerEntries(x =>
            {
                progressMax = x.TotalCount + 1;
                dialogInterface.SetMaximum(progressMax);
                dialogInterface.SetProgress(x.CurrentCount, x.Message);

                var inner = x.Inner;
                if (inner != null)
                {
                    dialogInterface.SetSubMaximum(inner.TotalCount);
                    dialogInterface.SetSubProgress(inner.CurrentCount, inner.Message);
                }
                else
                {
                    dialogInterface.SetSubMaximum(-1);
                    dialogInterface.SetSubProgress(0, string.Empty);
                }

                if (dialogInterface.Abort)
                    throw new OperationCanceledException();
            });

            dialogInterface.SetProgress(progressMax, Localisable.Progress_Finishing, true);
            dialogInterface.SetSubMaximum(2);
            dialogInterface.SetSubProgress(0, string.Empty);

            if (!string.IsNullOrEmpty(Program.InstalledRegistryKeyName))
                uninstallerEntries.RemoveAll(
                    x => PathTools.PathsEqual(x.RegistryKeyName, Program.InstalledRegistryKeyName));

            AllUninstallers = uninstallerEntries;

            dialogInterface.SetSubProgress(1, Localisable.Progress_Finishing_Icons);
            try
            {
                _iconGetter.UpdateIconList(AllUninstallers);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }

            dialogInterface.SetSubProgressVisible(false);

            // Fixes loading gettings stuck on finalizing if main window is minimized
            _reference.SafeInvoke(() =>
            {
                if (_reference.WindowState == FormWindowState.Minimized)
                    _reference.WindowState = FormWindowState.Normal;
            });
        }

        internal void ReassignStartupEntries(bool refreshListView, IEnumerable<StartupEntryBase> items)
        {
            ApplicationUninstallerFactory.AttachStartupEntries(AllUninstallers, items);

            if (refreshListView)
                RefreshList();
        }

        public void RefreshList()
        {
            if (CheckIsAppDisposed())
                return;

            _listView.ListView.UpdateColumnFiltering();
            //_listView.ListView.BuildList(true); No need, UpdateColumnFiltering already does this
        }

        public void SelectAllItems(object sender, EventArgs e)
        {
            var selected = _listView.ListView.CheckBoxes ? _listView.CheckedObjects : _listView.SelectedObjects;
            var added = selected.Union(FilteredUninstallers);
            ChangeSelection(added);
        }

        /// <summary>
        /// Select first item starting with the keycode.
        /// If keycode leads to a valid selection true is returned. Otherwise, if there is nothing relevant to select false is
        /// returned.
        /// </summary>
        public bool SelectItemFromKeystroke(Keys keyCode)
        {
            var keyName = keyCode.ToLetterOrNumberString();

            if (keyName != null)
            {
                var selectedObj =
                    FilteredUninstallers.FirstOrDefault(
                        x => x.DisplayName.StartsWith(keyName, StringComparison.InvariantCultureIgnoreCase));

                _listView.ListView.DeselectAll();

                if (selectedObj != null)
                {
                    _listView.ListView.SelectObject(selectedObj, true);
                    _listView.ListView.EnsureModelVisible(selectedObj);

                    return true;
                }
            }
            return false;
        }

        public sealed class ListRefreshEventArgs : EventArgs
        {
            public ListRefreshEventArgs(bool refreshIsRunning, bool firstRefresh)
            {
                RefreshIsRunning = refreshIsRunning;
                FirstRefresh = firstRefresh;
            }

            public bool FirstRefresh { get; }

            public bool RefreshIsRunning { get; }
        }
    }
}