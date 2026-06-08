/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Functions.ApplicationList;
using BulkCrapUninstaller.Functions.Tools;
using BulkCrapUninstaller.Functions.Tracking;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Events;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.IO;
using Klocman.Native;
using Klocman.Subsystems;
using Klocman.Tools;
using SimpleTreeMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using UninstallTools;
using UninstallTools.Dialogs;
using UninstallTools.Factory;
using UninstallTools.Lists;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    internal sealed partial class MainWindow : Form
    {
        private string MainTitleBarText { get; }

        public static string CertificateCacheFilename { get; } = Path.Combine(Program.AssemblyLocation.FullName, "CertCache.xml");
        public static CertificateCache CertificateCache { get; } = new(CertificateCacheFilename);

        private readonly UninstallerListViewUpdater _listView;
        private readonly SettingTools _setMan;
        private readonly WindowStyleController _styleController;
        private readonly AppUninstaller _appUninstaller;
        private readonly UninstallerListConfigurator _uninstallerListConfigurator;

        private readonly ListLegendWindow _listLegendWindow = new();
        private DebugWindow _debugWindow;

        private bool _previousListLegendState = true;
        private bool _anyStoreApps;
        private bool _anyWinFeatures;
        private bool _anyOrphans;
        private bool _anyProtected;
        private bool _anySysComponents;
        private bool _anyUpdates;
        private bool _anyInvalid;
        private bool _anyTweaks;

        /// <summary>
        ///     Set to false in the list view clicked event. Prevents firing of extra CellEditStarting events.
        ///     Used to fix buggy ObjectListView.
        /// </summary>
        private bool _ignoreCellEdit;

        private readonly UninstallerListPostProcesser _uninstallerListPostProcesser;

        public MainWindow()
        {
            Opacity = 0;
            Application.DoEvents();

            InitializeComponent();

            // Setup settings
            _setMan = new SettingTools(Settings.Default.SettingBinder, this);
            _setMan.LoadSettings();
            BindControlsToSettings();

            // Finish up setting controls and window, suspend after settings have loaded
            SuspendLayout();
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new StandardSystemColorTable())
            {
                RoundedEdges = true
            };

            // Disable until the first list refresh finishes
            LockApplication(true);

            SetupBasicSettingBindings();

            // Setup list view
            _listView = new UninstallerListViewUpdater(this);

            if (_setMan.Selected.Settings.CacheCertificates)
                CertificateCache.LoadCertificateCache();
            _uninstallerListPostProcesser = new UninstallerListPostProcesser(objects =>
            {
                uninstallerObjectListView.RefreshObjects(objects);
            }, CertificateCache);

            // Start the processing thread when user changes the test certificates option
            _setMan.Selected.Subscribe(OnTestCertificatesChanged, x => x.AdvancedTestCertificates, this);

            _uninstallerListConfigurator = new UninstallerListConfigurator(this);
            _uninstallerListConfigurator.AfterFiltering += (x, y) => _uninstallerListPostProcesser.StartProcessingThread(_listView.FilteredUninstallers);
            _uninstallerListConfigurator.AfterFiltering += RefreshStatusbarTotalLabel;

            _appUninstaller = new AppUninstaller(_listView.InitiateListRefresh, LockApplication, SetVisible);

            toolStripButtonSelAll.Click += _listView.SelectAllItems;
            toolStripButtonSelNone.Click += _listView.DeselectAllItems;
            toolStripButtonSelInv.Click += _listView.InvertSelectedItems;
            _uninstallerListPostProcesser.UninstallerPostprocessingProgressUpdate += UpdateStatusbarOnPostprocessingUpdate;
            _uninstallerListPostProcesser.UninstallerFileLock = _appUninstaller.PublicUninstallLock;
            _listView.ListRefreshIsRunningChanged += listView_ListRefreshIsRunningChanged;

            // Filter changed events
            advancedFilters1.CurrentListChanged += RefreshSidebarVisibility;
            advancedFilters1.CurrentListChanged += (sender, args) =>
            {
                _uninstallerListConfigurator.FilteringOverride = advancedFilters1.CurrentList;
                _uninstallerListConfigurator.UpdateColumnFiltering(_listView.AllUninstallers.Any());
            };
            advancedFilters1.FiltersChanged += (sender, args) =>
            {
                if (_uninstallerListConfigurator.FilteringOverride != null)
                    _uninstallerListConfigurator.UpdateColumnFiltering(_listView.AllUninstallers.Any());
            };
            advancedFilters1.CurrentListFileNameChanged += RefreshTitleBar;
            advancedFilters1.UnsavedChangesChanged += RefreshTitleBar;
            advancedFilters1.SelectedEntryGetter = () => _listView.SelectedUninstallers;

            // Setup update manager, skip at first boot to let user change the setting
            UpdateGrabber.Setup();
            if (!_setMan.Selected.Settings.MiscFirstRun)
            {
                BackgroundSearchForUpdates();
            }

            // Setup the main window
            Icon = Resources.Icon_Logo;
            // This is a duplicate of the about window logic-it doesn't have overhead but should probably be refactored.
            Assembly.GetExecutingAssembly().Modules.First().GetPEKind(out var pekind, out var ifmachine);
            bool isProbablyARM64;
            if (Enum.IsDefined<ImageFileMachine>(ifmachine))
            {
                isProbablyARM64 = false;
            }
            else // Work around .NET 6 not having WoA64 definitions in GetPEKind

            {
                isProbablyARM64 = true;
            }
            MainTitleBarText = Text.Append(" v", Program.AssemblyVersion.ToString(Program.AssemblyVersion.Build != 0 ? 3 : 2))
                .AppendIf(!Program.IsInstalled, " ", Localisable.StrIsPortable)
                .AppendIf(ProcessTools.Is64BitProcess, " ", Localisable.Str64Bit)
                .AppendIf(isProbablyARM64, " ", "ARM")
                .AppendIf(Program.EnableDebug, " ", Localisable.StrDebug);
            Text = MainTitleBarText;

            Console.WriteLine(MainTitleBarText);

            _styleController = new WindowStyleController(this);

            // Initialize the status bar
            toolStripLabelStatus_TextChanged(this, EventArgs.Empty);

            // Debug stuff
            var isDebug = Program.EnableDebug;
            debugToolStripMenuItem.Enabled = isDebug;
            debugToolStripMenuItem.Visible = isDebug;
            if (isDebug)
                _setMan.Selected.Settings.AdvancedSimulate = true;

            // Tracking
            UsageManager.DataSender = new DatabaseStatSender(Settings.Default.MiscUserId);

            // Misc
            filterEditor1.ComparisonMethodChanged += SearchCriteriaChanged;

            MessageBoxes.DefaultOwner = this;
            LoadingDialog.DefaultOwner = this;
            PremadeDialogs.DefaultOwner = this;
            PremadeDialogs.SendErrorAction = NBug.Exceptions.Report;

            SetupHotkeys();

            treeMap1.ObjectNameGetter = o => ((ApplicationUninstallerEntry)o).DisplayName;
            treeMap1.ObjectValueGetter = o => ((ApplicationUninstallerEntry)o).EstimatedSize.GetKbSize();
            treeMap1.ObjectColorGetter = o => ApplicationListConstants.GetApplicationTreemapColor((ApplicationUninstallerEntry)o);

            _uninstallerListPostProcesser.UninstallerPostprocessingProgressUpdate += UpdateTreemapOnPostprocessingUpdate;

            _uninstallerListConfigurator.AfterFiltering += UpdateTreeMap;

            uninstallerObjectListView.SelectionChanged +=
                (sender, args) => treeMap1.SetSelectedObjects(uninstallerObjectListView.SelectedObjects.Cast<object>());

            treeMap1.SliceClicked += OnTreeMapSliceClicked;
            treeMap1.SliceRightClicked += OnTreeMapSliceRightClicked;
            treeMap1.SliceHovered += OnTreeMapSliceHovered;

            _setMan.Selected.BindControl(showTreemapToolStripMenuItem, settings => settings.ShowTreeMap, this);
            _setMan.Selected.Subscribe((x, y) => splitContainerListAndMap.Panel2Collapsed = !y.NewValue, settings => settings.ShowTreeMap, this);

            uninstallerObjectListView.ContextMenuStrip = uninstallListContextMenuStrip;
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);

            try
            {
                var scaleChange = e.DeviceDpiNew / (double)e.DeviceDpiOld;

                if (toolStripLabelSize != null)
                    toolStripLabelSize.Width = (int)Math.Round(toolStripLabelSize.Width * scaleChange);
                if (toolStripLabelTotal != null)
                    toolStripLabelTotal.Width = (int)Math.Round(toolStripLabelTotal.Width * scaleChange);
            }
            catch (SystemException exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                new Thread(UsageTrackerSendData).Start();

                DisposeListPostProcessor(this, e);
                _listLegendWindow?.Dispose();
                _uninstallerListConfigurator?.Dispose();
                _debugWindow?.Dispose();
                _uninstallerListPostProcesser?.Dispose();
            }
            catch (Exception exception)
            {
                // Eat non-critical exceptions to let the app close in peace
                Console.WriteLine(exception);
            }

            base.OnFormClosed(e);
        }

        private void OnTreeMapSliceHovered(object sender, TreeMap.SliceEventArgs args)
        {
            toolStripLabelStatus.Text = args.Rectangle.Slice.ToElementNames();
            toolStripLabelSize.Text = FileSize.SumFileSizes(args.Objects.Cast<ApplicationUninstallerEntry>().Select(x => x.EstimatedSize)).ToString();
        }

        private void OnTreeMapSliceRightClicked(object sender, TreeMap.SliceClickedEventArgs args)
        {
            if (args.AddToSelection || !uninstallerObjectListView.SelectedObjects.Contains(args.Objects.FirstOrDefault()))
                OnTreeMapSliceClicked(sender, args);
            uninstallListContextMenuStrip.Show(MousePosition);
        }

        private void SetupBasicSettingBindings()
        {
            _setMan.Selected.Subscribe((x, y) =>
            {
                var paths = y.NewValue.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);
                var trimmed = paths.Select(path => path.Trim().Trim('"').Trim()).ToArray();
                UninstallToolsGlobalConfig.CustomProgramFiles = trimmed;
            }, x => x.FoldersCustomProgramDirs, this);

            _setMan.Selected.Subscribe((x, y) => _listView.RefreshList(), x => x.AdvancedHighlightSpecial, this);

            _setMan.Selected.Subscribe(OnApplicationListVisibleItemsChanged, x => x.AdvancedTestCertificates, this);
            _setMan.Selected.Subscribe(OnApplicationListVisibleItemsChanged, x => x.AdvancedTestInvalid, this);
            _setMan.Selected.Subscribe(OnApplicationListVisibleItemsChanged, x => x.AdvancedHighlightSpecial, this);
            _setMan.Selected.Subscribe(OnApplicationListVisibleItemsChanged, x => x.FilterShowStoreApps, this);
            _setMan.Selected.Subscribe(OnApplicationListVisibleItemsChanged, x => x.FilterShowWinFeatures, this);
            _setMan.Selected.Subscribe(OnApplicationListVisibleItemsChanged, x => x.AdvancedDisplayOrphans, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanSteam = y.NewValue, x => x.ScanSteam, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanStoreApps = y.NewValue, x => x.ScanStoreApps, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanWinFeatures = y.NewValue, x => x.ScanWinFeatures, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanWinUpdates = y.NewValue, x => x.ScanWinUpdates, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanDrives = y.NewValue, x => x.ScanDrives, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanRegistry = y.NewValue, x => x.ScanRegistry, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanPreDefined = y.NewValue, x => x.ScanPreDefined, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanChocolatey = y.NewValue, x => x.ScanChocolatey, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanOculus = y.NewValue, x => x.ScanOculus, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.ScanScoop = y.NewValue, x => x.ScanScoop, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.AutoDetectCustomProgramFiles = y.NewValue, x => x.FoldersAutoDetect, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.AutoDetectScanRemovable = y.NewValue, x => x.FoldersScanRemovable, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.QuietAutomatization = y.NewValue, x => x.QuietAutomatization, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.QuietAutomatizationKillStuck = y.NewValue, x => x.QuietAutomatizationKillStuck, this);
            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.UseQuietUninstallDaemon = y.NewValue, x => x.QuietUseDaemon, this);

            _setMan.Selected.Subscribe((x, y) => UninstallToolsGlobalConfig.EnableAppInfoCache = y.NewValue, x => x.CacheAppInfo, this);

            _setMan.Selected.Subscribe((o, args) => _listView.RefreshList(), x => x.MiscColorblind, this);
        }

        private void UpdateTreemapOnPostprocessingUpdate(object x, CountingUpdateEventArgs y)
        {
            var update = y.Value == y.Maximum || (y.Value - 1) % 100 == 0;

            if (update)
                this.SafeInvoke(() => UpdateTreeMap(x, y));
        }

        private void UpdateStatusbarOnPostprocessingUpdate(object x, CountingUpdateEventArgs y)
        {
            string result = null;

            if (y.Value == y.Maximum)
                result = string.Empty;
            else if (y.Maximum % 15 == 0)
            {
                // Can crash on some locales even though format string is correct
                try { result = string.Format(CultureInfo.CurrentCulture, Localisable.MainWindow_Statusbar_ProcessingUninstallers, y.Maximum); }
                catch (FormatException ex) { Console.WriteLine(ex); }
            }

            if (result != null)
                this.SafeInvoke(() => toolStripLabelStatus.Text = result);
        }

        private void DisposeListPostProcessor(object x, FormClosedEventArgs y)
        {
            try
            {
                if (_setMan.Selected.Settings.CacheCertificates)
                    CertificateCache.SaveCertificateCache();
                else
                    CertificateCache.ClearChache();

                if (!_setMan.Selected.Settings.CacheAppInfo)
                    File.Delete(UninstallToolsGlobalConfig.AppInfoCachePath);
            }
            catch (SystemException e)
            {
                Console.WriteLine(@"Failed to delete cache: " + e);
            }

            _uninstallerListPostProcesser.Dispose();
        }

        private void OnTestCertificatesChanged(object x, SettingChangedEventArgs<bool> y)
        {
            if (!_listView.FirstRefreshCompleted)
                return;
            if (y.NewValue) _uninstallerListPostProcesser.StartProcessingThread(_listView.FilteredUninstallers);
            else
            {
                _uninstallerListPostProcesser.StopProcessingThread();

                SafeRefreshObjects(_listView.AllUninstallers.Where(u => u.IsCertificateValid(true).HasValue));
            }
        }

        private void SafeRefreshObjects(IEnumerable<ApplicationUninstallerEntry> itemsToUpdate)
        {
            if (!_listView.CheckIsAppDisposed())
            {
                uninstallerObjectListView.SuspendLayout();
                uninstallerObjectListView.RefreshObjects(itemsToUpdate.ToList());
                uninstallerObjectListView.ResumeLayout();
            }
        }

        private void OnTreeMapSliceClicked(object sender, TreeMap.SliceClickedEventArgs args)
        {
            var list = args.Objects.ToList();

            if (args.AddToSelection)
                list.AddRange(uninstallerObjectListView.SelectedObjects.Cast<object>());

            uninstallerObjectListView.SelectObjects(list);

            uninstallerObjectListView.EnsureModelVisible(list.FirstOrDefault());
            uninstallerObjectListView.Focus();
        }

        private void UpdateTreeMap(object sender, EventArgs args)
        {
            treeMap1.Populate(_listView.FilteredUninstallers);
        }

        private void OnApplicationListVisibleItemsChanged(object sender, EventArgs e)
        {
            UpdateListView();
        }

        private void UpdateListView()
        {
            var force = advancedFilters1.CurrentList != null;
            _listLegendWindow.ListLegend.CertificatesEnabled = force || _setMan.Selected.Settings.AdvancedTestCertificates;
            _listLegendWindow.ListLegend.InvalidEnabled = force || _setMan.Selected.Settings.AdvancedTestInvalid && _anyInvalid;
            _listLegendWindow.ListLegend.StoreAppEnabled = force || _setMan.Selected.Settings.FilterShowStoreApps && _anyStoreApps && _setMan.Selected.Settings.AdvancedHighlightSpecial;
            _listLegendWindow.ListLegend.OrphanedEnabled = force || _setMan.Selected.Settings.AdvancedDisplayOrphans && _anyOrphans && _setMan.Selected.Settings.AdvancedHighlightSpecial;
            _listLegendWindow.ListLegend.WinFeatureEnabled = force || _setMan.Selected.Settings.FilterShowWinFeatures && _anyWinFeatures && _setMan.Selected.Settings.AdvancedHighlightSpecial;
            _listLegendWindow.UpdatePosition(uninstallerObjectListView);
        }

        private void RefreshTitleBar(object sender, EventArgs e)
        {
            var result = MainTitleBarText;
            if (!string.IsNullOrEmpty(advancedFilters1.CurrentListFileName) || advancedFilters1.UnsavedChanges)
            {
                var changedDot = advancedFilters1.UnsavedChanges ? "*" : string.Empty;
                result = string.Format(CultureInfo.CurrentCulture, "{0} [{1}{2}]",
                    result, advancedFilters1.CurrentListFileName ?? string.Empty, changedDot);
            }
            Text = result;
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    components?.Dispose();
                }
                catch (NullReferenceException)
                {
                    // ObjectListView sometimes throws it at exit
                }

                _listView?.Dispose();
            }
            try { base.Dispose(disposing); }
            catch (InvalidOperationException) { }
        }

        private void SearchCriteriaChanged(object sender, EventArgs e)
        {
            _uninstallerListConfigurator.UpdateColumnFiltering(_listView.AllUninstallers.Any());
        }

        public void LockApplication(bool value)
        {
            this.SafeInvoke(() =>
            {
                UseWaitCursor = value;

                foreach (Control control in Controls)
                    control.Enabled = !value;

                Refresh();
            });
        }

        private void SetVisible(bool val)
        {
            this.SafeInvoke(() =>
            {
                Visible = val;
                if (_listLegendWindow != null)
                {
                    if (val)
                    {
                        _setMan.Selected.Settings.UninstallerListShowLegend = _previousListLegendState;
                        //_listLegendWindow.Visible = _previousListLegendState;
                    }
                    else
                    {
                        _previousListLegendState = _setMan.Selected.Settings.UninstallerListShowLegend;
                        _listLegendWindow.Visible = false;
                    }
                }
            });
        }

        internal static void OpenUrls(IEnumerable<Uri> urls)
        {
            if (WindowsTools.IsNetworkAvailable())
            {
                var urlList = urls as IList<Uri> ?? urls.ToList();
                if (MessageBoxes.OpenUrlsMessageBox(urlList.Count))
                {
                    try
                    {
                        urlList.ForEach(x => Process.Start(new ProcessStartInfo(x.AbsoluteUri) { UseShellExecute = true }));
                    }
                    catch (Exception e)
                    {
                        MessageBoxes.OpenUrlError(e);
                    }
                }
            }
            else
                MessageBoxes.NoNetworkConnected();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var abox = new AboutBox())
            {
                abox.ShowDialog();
            }
        }

        private void advancedOperationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectionCount = _listView.SelectedUninstallerCount;
            openKeyInRegeditToolStripMenuItem.Enabled = selectionCount == 1;
            deleteToolStripMenuItem.Enabled = selectionCount > 0;
            createBackupToolStripMenuItem.Enabled = selectionCount > 0;
            msiUninstalltoolStripMenuItem.Enabled = selectionCount == 1;

            var autostart = _listView.SelectedUninstallers.Any(
                u => u.StartupEntries != null && u.StartupEntries.Any(se => !se.Disabled));
            disableAutostartToolStripMenuItem.Enabled = autostart;

            // Take ownership list
            var ownershipList = takeOwnershipToolStripMenuItem.DropDownItems.Cast<ToolStripItem>().ToList();
            takeOwnershipToolStripMenuItem.DropDownItems.Clear();
            foreach (var dropDownItem in ownershipList)
                dropDownItem.Dispose();

            takeOwnershipToolStripMenuItem.DropDownItems.AddRange(
                _listView.SelectedUninstallers
                .SelectMany(x => new[] { x.InstallLocation, x.UninstallerLocation })
                .Where(dir => !string.IsNullOrEmpty(dir))
                .DistinctBy(x => x.ToLowerInvariant())
                .OrderBy(x => x)
                .Select(dir => (ToolStripItem)new ToolStripMenuItem(dir, null, (obj, args) => TakeOwnership(dir)))
                .ToArray());
        }

        private static void TakeOwnership(string directoryPath)
        {
            PremadeDialogs.StartProcessSafely("cmd.exe", $"/c takeown /f \"{directoryPath}\" && icacls \"{directoryPath}\" /grant administrators:F && pause");
        }

        private void BackgroundSearchForUpdates()
        {
            UpdateGrabber.AutoUpdate(() => _listView.FirstRefreshCompleted,
                v => this.SafeInvoke(() => UpdateGrabber.AskAndBeginUpdate(v)));
        }

        private void basicOperationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectionCount = _listView.SelectedUninstallerCount;
            uninstallToolStripMenuItem.Enabled = selectionCount > 0;
            quietUninstallToolStripMenuItem.Enabled = selectionCount > 0;
            propertiesToolStripMenuItem.Enabled = selectionCount > 0;
            modifyToolStripMenuItem.Enabled = selectionCount == 1 &&
                !string.IsNullOrEmpty(_listView.SelectedUninstallers.FirstOrDefault()?.ModifyPath);
        }

        private void BindControlsToSettings()
        {
            var settings = _setMan.Selected;

            // Bind controls to their respective settings
            settings.BindControl(displayToolbarToolStripMenuItem, x => x.ToolbarsShowToolbar, this);
            settings.BindControl(displaySettingsToolStripMenuItem, x => x.ToolbarsShowSettings, this);
            settings.BindControl(useSystemThemeToolStripMenuItem, x => x.WindowUseSystemTheme, this);
            settings.BindControl(displayStatusbarToolStripMenuItem, x => x.ToolbarsShowStatusbar, this);

            settings.BindControl(showColorLegendToolStripMenuItem, x => x.UninstallerListShowLegend, this);

            settings.Subscribe(RefreshSidebarVisibility,
                x => x.ToolbarsShowSettings, this);
            settings.Subscribe((x, y) => toolStrip.Visible = y.NewValue,
                x => x.ToolbarsShowToolbar, this);
            settings.Subscribe((x, y) => _styleController.SetStyles(y.NewValue),
                x => x.WindowUseSystemTheme, this);
            settings.Subscribe((x, y) => statusStrip1.Visible = y.NewValue,
                x => x.ToolbarsShowStatusbar, this);

            settings.Subscribe((x, y) =>
            {
                if (_listView.CheckIsAppDisposed()) return;

                try
                {
                    uninstallerObjectListView.CheckBoxes = y.NewValue;
                }
                catch (InvalidOperationException)
                {
                    // Setting CheckBoxes value throws this exception (even though it works fine).
                }
                _listView.RefreshList();
                uninstallerObjectListView_SelectedChanged(this, EventArgs.Empty);
            }, x => x.UninstallerListUseCheckboxes, this);

            settings.Subscribe((x, y) =>
            {
                if (_listView.CheckIsAppDisposed()) return;

                uninstallerObjectListView.ShowGroups = y.NewValue;
                _listView.RefreshList();
            }, x => x.UninstallerListUseGroups, this);

            settings.Subscribe(RefreshList, x => x.FilterHideMicrosoft, this);
            settings.Subscribe(RefreshList, x => x.FilterShowUpdates, this);
            settings.Subscribe(RefreshList, x => x.FilterShowSystemComponents, this);
            settings.Subscribe(RefreshList, x => x.FilterShowProtected, this);
            settings.Subscribe(RefreshList, x => x.FilterShowStoreApps, this);
            settings.Subscribe(RefreshList, x => x.FilterShowWinFeatures, this);
            settings.Subscribe(RefreshList, x => x.FilterShowTweaks, this);

            settings.Subscribe((sender, args) =>
            {
                if (_listView.CheckIsAppDisposed()) return;

                olvColumnRating.IsVisible = args.NewValue;
                uninstallerObjectListView.RebuildColumns();
            }, x => x.MiscUserRatings, this);
        }

        private void RefreshSidebarVisibility(object sender, EventArgs e)
        {
            this.BeginControlUpdate();
            SuspendLayout();
            bool ulistOpen = IsAdvancedFilteringEnabled;
            splitContainer1.Panel1Collapsed = !ulistOpen;
            splitContainer1.Panel1.Enabled = ulistOpen;

            var sidebarOpen = _setMan.Selected.Settings.ToolbarsShowSettings && !ulistOpen;
            settingsSidebarPanel.Visible = sidebarOpen;
            settingsSidebarPanel.Enabled = sidebarOpen;

            OnApplicationListVisibleItemsChanged(sender, e);

            ResumeLayout();
            this.EndControlUpdate();
        }

        private bool IsAdvancedFilteringEnabled => advancedFilters1.CurrentList != null;

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WindowsTools.IsNetworkAvailable())
            {
                LockApplication(true);
                UpdateGrabber.LookForUpdates();
                LockApplication(false);
            }
            else
                MessageBoxes.NoNetworkConnected();
        }

        private void cleanUpProgramFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.SearchForAndRemoveProgramFilesJunk(_listView.AllUninstallers);
        }

        private void ClipboardCopyFullInformation(object x, EventArgs y)
        {
            ImportExport.CopyFullInformationToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyGuids(object x, EventArgs y)
        {
            ImportExport.CopyGuidsToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyProgramName(object x, EventArgs y)
        {
            ImportExport.CopyNamesToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyRegistryPath(object x, EventArgs y)
        {
            ImportExport.CopyRegKeysToClipboard(_listView.SelectedUninstallers);
        }

        private void ClipboardCopyUninstallString(object x, EventArgs y)
        {
            ImportExport.CopyUninstallStringsToClipboard(_listView.SelectedUninstallers);
        }

        private void createBackupFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!_listView.SelectedUninstallers.Any())
            {
                e.Cancel = true;
                return;
            }

            try
            {
                RegistryTools.ExportRegistry(createBackupFileDialog.FileName,
                    _listView.SelectedUninstallers.Select(x => x.RegistryPath));
            }
            catch (Exception ex)
            {
                MessageBoxes.ExportFailed(ex.Message, this);
                e.Cancel = true;
            }
        }

        private void CreateRegistryBackup(object sender, EventArgs e)
        {
            createBackupFileDialog.ShowDialog();
        }

        private void DeleteRegistryEntries(object sender, EventArgs eventArgs)
        {
            if (_listView.SelectedUninstallerCount == 0)
                return;

            var items = _listView.SelectedUninstallers.ToArray();
            var protectedItems = items.Where(x => x.IsProtected).ToArray();

            if (!_setMan.Selected.Settings.AdvancedDisableProtection && protectedItems.Any())
            {
                var affectedKeyNames = protectedItems.Select(x => x.DisplayName).ToArray();
                if (MessageBoxes.ProtectedItemsWarningQuestion(affectedKeyNames) == MessageBoxes.PressedButton.Cancel)
                    return;

                items = _listView.SelectedUninstallers.Where(x => !x.IsProtected).ToArray();
            }

            if (!items.Any() || !MessageBoxes.DeleteRegKeysConfirmation(items.Select(x => x.DisplayName).ToArray()))
                return;

            foreach (var item in items)
            {
                try
                {
                    if (item.IsRegistered) RegistryTools.RemoveRegistryKey(item.RegistryPath);
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }

            _listView.InitiateListRefresh();
        }

        private void donateButton_Click(object sender, EventArgs e)
        {
            OpenUrls(new[] { new Uri(Resources.DonateLink) });
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!AppUninstaller.ExportUninstallers(_listView.SelectedUninstallers, exportDialog.FileName))
                e.Cancel = true;
        }

        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportDialog.ShowDialog();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var anySelected = _listView.SelectedUninstallerCount > 0;
            exportSelectedToolStripMenuItem.Enabled = anySelected;
            exportToABatchUninstallScriptToolStripMenuItem.Enabled = anySelected;

            exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem.Enabled =
                _listView.SelectedUninstallers.Any(x => x.UninstallerKind == UninstallerType.StoreApp);
        }

        private void HandleListViewMenuKeystroke(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps)
            {
                if (_listView.SelectedUninstallerCount > 0)
                {
                    uninstallListContextMenuStrip.Show(uninstallerObjectListView.PointToScreen(Point.Empty));
                }
                e.SuppressKeyPress = true;
            }
        }

        private void helpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //uninstallBCUninstallToolstripMenuItem.Visible = Program.IsInstalled;
            uninstallBCUninstallToolstripMenuItem.Enabled = Program.IsInstalled;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _setMan.SaveSettings();
            SystemRestore.CancelSysRestore();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || _setMan.Selected.Settings.MiscFirstRun ||
                !WindowsTools.IsNetworkAvailable())
                return;

            if (!_setMan.Selected.Settings.MiscFeedbackNagNeverShow)
            {
                if (!_setMan.Selected.Settings.MiscFeedbackNagShown &&
                    DateTime.Now - Process.GetCurrentProcess().StartTime > TimeSpan.FromMinutes(3))
                {
                    FeedbackBox.ShowFeedbackBox(this, true);
                }

                // Show the nag every other time
                _setMan.Selected.Settings.MiscFeedbackNagShown = !_setMan.Selected.Settings.MiscFeedbackNagShown;
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            _setMan.Selected.SendUpdates();

            // Work around a bug in Object list view
            try
            {
                ResumeLayout();
            }
            catch (ObjectDisposedException)
            {
                Application.DoEvents();
                ResumeLayout();
            }

            _listView.InitiateListRefresh();

            settingsSidebarPanel.Width = propertiesSidebar.GetSuggestedWidth() +
                settingsSidebarPanel.Padding.Left +
                settingsSidebarPanel.Padding.Right;
        }

        private void SetupAndShowLegendWindow()
        {
            if (IsDisposed || Disposing)
                return;

            _listLegendWindow.Show(this);
            AddOwnedForm(_listLegendWindow);

            _listLegendWindow.UpdatePosition(uninstallerObjectListView);
            listViewPanel.Resize += (o, args) => _listLegendWindow.UpdatePosition(uninstallerObjectListView);
            Move += (o, args) => _listLegendWindow.UpdatePosition(uninstallerObjectListView);
            Controls[0].EnabledChanged += (o, args) => _listLegendWindow.Enabled = Controls[0].Enabled;

            var settings = _setMan.Selected;
            settings.Subscribe((x, y) => _listLegendWindow.Visible = y.NewValue, x => x.UninstallerListShowLegend, this);
            _listLegendWindow.VisibleChanged += (x, y) =>
            {
                if (!_listLegendWindow.Visible && settings.Settings.UninstallerListShowLegend)
                    settings.Settings.UninstallerListShowLegend = false;
            };
        }

        private void msiInstallContextMenuStripItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.UninstallUsingMsi(MsiUninstallModes.InstallModify, _listView.SelectedUninstallers);
        }

        private void msiQuietUninstallContextMenuStripItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.UninstallUsingMsi(MsiUninstallModes.QuietUninstall, _listView.SelectedUninstallers);
        }

        private void msiUninstallContextMenuStripItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.UninstallUsingMsi(MsiUninstallModes.Uninstall, _listView.SelectedUninstallers);
        }

        private void OnFirstApplicationStart()
        {
            StartSetupWizard(false);

            // On first start the updates are not searched from constructor to give user a chance to disable them.
            BackgroundSearchForUpdates();
        }

        private void OpenAssociatedWebPage(object sender, EventArgs eventArgs)
        {
            var urls = _listView.SelectedUninstallers.Select(y => y.GetAboutUri()).Where(x => x != null).ToList();

            OpenUrls(urls);
        }

        private void OpenDebugWindow(object sender, EventArgs e)
        {
            if (_debugWindow == null || _debugWindow.IsDisposed)
            {
                _debugWindow = new DebugWindow(this, _listView, _appUninstaller);
            }

            _debugWindow.Show();
        }

        private void OpenInRegedit(object sender, EventArgs e)
        {
            var targetEntry = _listView.SelectedUninstallers.FirstOrDefault(x => x.RegKeyStillExists());
            if (targetEntry != null)
            {
                try
                {
                    RegistryTools.OpenRegKeyInRegedit(targetEntry.RegistryPath);
                }
                catch (IOException ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
        }

        private void OpenInstallationSource(object sender, EventArgs eventArgs)
        {
            var sourceDirs =
                _listView.SelectedUninstallers.Where(x => x.InstallSource.IsNotEmpty())
                    .Select(y => y.InstallSource)
                    .ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x => Process.Start(new ProcessStartInfo(x) { UseShellExecute = true }));
                }
                catch (Exception e)
                {
                    MessageBoxes.OpenDirectoryError(e);
                }
            }
        }

        private void OpenInstallLocation(object sender, EventArgs eventArgs)
        {
            var sourceDirs =
                _listView.SelectedUninstallers.Where(x => x.InstallLocation.IsNotEmpty())
                    .Select(y => y.InstallLocation)
                    .ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x => Process.Start(new ProcessStartInfo(x) { UseShellExecute = true }));
                }
                catch (Exception e)
                {
                    MessageBoxes.OpenDirectoryError(e);
                }
            }
        }

        private void openProgramsAndFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WindowsTools.OpenControlPanelApplet(ControlPanelCanonicalNames.ProgramsAndFeatures);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void OpenProperties(object sender, EventArgs eventArgs)
        {
            if (_listView.SelectedUninstallerCount == 0) return;

            using (var propertiesWindow = new PropertiesWindow())
            {
                propertiesWindow.ShowPropertiesDialog(_listView.SelectedUninstallers);
            }
        }

        private void OpenSubmitFeedbackWindow(object sender, EventArgs e)
        {
            FeedbackBox.ShowFeedbackBox(this, false);
        }

        private void OpenUninstallerLocation(object sender, EventArgs eventArgs)
        {
            var sourceDirs = _listView.SelectedUninstallers.Where(x => x.UninstallerFullFilename.IsNotEmpty()).ToList();

            if (MessageBoxes.OpenDirectoriesMessageBox(sourceDirs.Count))
            {
                try
                {
                    sourceDirs.ForEach(x =>
                    {
                        if (File.Exists(x.UninstallerFullFilename))
                            WindowsTools.OpenExplorerFocusedOnObject(x.UninstallerFullFilename);
                        else
                            Process.Start(new ProcessStartInfo(x.UninstallerLocation) { UseShellExecute = true });
                    });
                }
                catch (Exception e)
                {
                    MessageBoxes.OpenDirectoryError(e);
                }
            }
        }

        private void OpenUninstallLists(object sender, EventArgs e)
        {
            advancedFilters1.LoadUninstallList();
        }

        private void RefreshList(object sender, EventArgs e)
        {
            _listView.RefreshList();
        }

        private void RefreshStatusbarTotalLabel(object sender, EventArgs e)
        {
            toolStripLabelTotal.Text = string.Format(CultureInfo.CurrentCulture, Localisable.MainWindow_Statusbar_Total,
                _listView.FilteredUninstallers.Count(), _listView.GetFilteredSize());
        }

        private void ReloadUninstallers(object sender, EventArgs e)
        {
            _listView.InitiateListRefresh();
        }

        private void RenameEntries(object sender, EventArgs eventArgs)
        {
            var selectedUninstallers = _listView.SelectedUninstallers.ToList();
            if (selectedUninstallers.Count != 1)
            {
                MessageBoxes.CanSelectOnlyOneItemInfo();
                return;
            }

            var selected = selectedUninstallers[0];

            if (!_setMan.Selected.Settings.AdvancedDisableProtection && selected.IsProtected)
            {
                MessageBoxes.ProtectedItemError(selected.DisplayName);
                return;
            }

            if (!selected.IsRegistered)
            {
                MessageBoxes.CantRenameUninstallerKind(selected.DisplayName, selected.UninstallerKind);
                return;
            }

            if (StringEditBox.ShowDialog(string.Format(CultureInfo.InvariantCulture, Localisable.MainWindow_Rename_Description, selected.DisplayName),
                                         Localisable.MainWindow_Rename_Title, selected.DisplayName, Buttons.ButtonOk, Buttons.ButtonCancel,
                                         out var output))
            {
                try
                {
                    if (selected.Rename(output))
                        _listView.InitiateListRefresh();
                    else
                        MessageBoxes.InvalidNewEntryName();
                }
                catch (Exception exception)
                {
                    PremadeDialogs.GenericError(exception);
                }
            }
        }

        private void ResetSettingsDialog(object sender, EventArgs e)
        {
            _setMan.ResetSettingsDialog();
        }

        private void RunAdvancedUninstall(object sender, EventArgs e)
        {
            var items = _listView.SelectedUninstallers.ToArray();
            var protectedItems = items.Where(x => x.IsProtected).ToArray();

            if (!_setMan.Selected.Settings.AdvancedDisableProtection && protectedItems.Any())
            {
                var affectedKeyNames = protectedItems.Select(x => x.DisplayName).ToArray();
                if (MessageBoxes.ProtectedItemsWarningQuestion(affectedKeyNames) == MessageBoxes.PressedButton.Cancel)
                    return;

                items = _listView.SelectedUninstallers.Where(x => !x.IsProtected).ToArray();
            }

            if (!items.Any())
            {
                MessageBoxes.NoUninstallersSelectedInfo();
                return;
            }

            _appUninstaller.AdvancedUninstall(items, _listView.AllUninstallers);
        }

        private void RunLoudUninstall(object x, EventArgs y)
        {
            _appUninstaller.RunUninstall(_listView.SelectedUninstallers, _listView.AllUninstallers, false);
        }

        private void RunQuietUninstall(object x, EventArgs y)
        {
            _appUninstaller.RunUninstall(_listView.SelectedUninstallers, _listView.AllUninstallers, true);

            /*var nonQuiet =
                _listView.SelectedUninstallers.Where(o => !o.QuietUninstallPossible)
                    .Select(p => p.DisplayName)
                    .ToArray();

            if (!nonQuiet.Any())
                _uninstaller.RunUninstall(_listView.SelectedUninstallers, _listView.AllUninstallers, true);
            else
            {
                switch (MessageBoxes.QuietUninstallersNotAvailableQuestion(nonQuiet))
                {
                    case MessageBoxes.PressedButton.Yes:
                        _uninstaller.RunUninstall(_listView.SelectedUninstallers,
                            _listView.AllUninstallers, true);
                        break;
                    case MessageBoxes.PressedButton.No:
                        _uninstaller.RunUninstall(_listView.SelectedUninstallers.Where(p => p.QuietUninstallPossible),
                            _listView.AllUninstallers, true);
                        break;
                    default:
                        return;
                }
            }*/
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_setMan.Selected.Settings.ToolbarsShowSettings)
                _setMan.Selected.Settings.ToolbarsShowSettings = true;
            filterEditor1.FocusSearchbox();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sw = new SettingsWindow())
                sw.ShowDialog();
        }

        private void SetupHotkeys()
        {
            // File
            globalHotkeys1.Add(new HotkeyEntry(Keys.F5, reloadUninstallersToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.O, false, true, false, loadUninstallerListToolStripMenuItem));
            globalHotkeys1.Add(new HotkeyEntry(Keys.F4, true, false, false, exitToolStripMenuItem));

            // View
            globalHotkeys1.Add(new HotkeyEntry(Keys.F, false, true, false, searchToolStripMenuItem_Click, null));
            globalHotkeys1.Add(new HotkeyEntry(Keys.F3, searchToolStripMenuItem));

            // Basic operations
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, uninstallToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, false, false, true, quietUninstallToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.C, false, true, false, copyFullInformationToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.Enter, true, false, false, propertiesToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));

            // Advanced operations
            globalHotkeys1.Add(new HotkeyEntry(Keys.Delete, false, true, true, manualUninstallToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.B, false, true, false, createBackupToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));
            globalHotkeys1.Add(new HotkeyEntry(Keys.R, false, true, false, openKeyInRegeditToolStripMenuItem,
                () => !_listView.CheckIsAppDisposed() && uninstallerObjectListView.ContainsFocus));

            // Tools
            globalHotkeys1.Add(new HotkeyEntry(Keys.P, false, true, false, settingsToolStripMenuItem_Click,
                settingsToolStripMenuItem));
        }

        private void OnClickStartSetupWizard(object sender, EventArgs e)
        {
            StartSetupWizard(true);
        }

        private void StartSetupWizard(bool canExit)
        {
            using (var wizard = new FirstStartBox(canExit))
            {
                wizard.StartPosition = FormStartPosition.CenterParent;
                wizard.ShowDialog(this);
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            viewWindowsFeaturesToolStripMenuItem.Enabled = DismTools.DismIsAvailable;
            tryToInstallNETV35ToolStripMenuItem.Enabled = DismTools.DismIsAvailable && !WindowsTools.CheckNetFramework35Installed();
            createRestorePointToolStripMenuItem.Enabled = SysRestore.SysRestoreAvailable();
        }

        private void toolStripLabelStatus_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripLabelStatus.Text))
            {
                toolStripLabelStatus.TextChanged -= toolStripLabelStatus_TextChanged;
                toolStripLabelStatus.Text = Localisable.MainWindow_Statusbar_StatusReady;
                toolStripLabelStatus.TextChanged += toolStripLabelStatus_TextChanged;
            }
        }

        private void uninstallBCUninstallToolstripMenuItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.AskToSelfUninstall();
        }

        private void uninstallerObjectListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            e.Cancel = true;

            if (_ignoreCellEdit || e.RowObject == null)
                return;

            _ignoreCellEdit = true;

            if (uninstallerObjectListView.CheckBoxes && !uninstallerObjectListView.IsChecked(e.RowObject))
            {
                uninstallerObjectListView.UncheckAll();
                uninstallerObjectListView.CheckObject(e.RowObject);
            }

            switch (Settings.Default.UninstallerListDoubleClickAction)
            {
                case UninstallerListDoubleClickAction.DoNothing:
                    break;
                case UninstallerListDoubleClickAction.OpenProperties:
                    OpenProperties(sender, e);
                    break;
                case UninstallerListDoubleClickAction.Uninstall:
                    RunLoudUninstall(sender, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(UninstallerListDoubleClickAction), Settings.Default.UninstallerListDoubleClickAction, "Unhandled value");
            }

            //uninstallerObjectListView.CancelCellEdit();
        }

        private void uninstallerObjectListView_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Model == null)
                return;

            if (uninstallerObjectListView.CheckBoxes && !uninstallerObjectListView.IsChecked(e.Model))
            {
                //uninstallerObjectListView.UncheckAll();
                uninstallerObjectListView.CheckObject(e.Model);
            }
        }

        private void uninstallerObjectListView_Click(object sender, EventArgs e)
        {
            _ignoreCellEdit = false;
        }

        private void uninstallerObjectListView_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = _listView.SelectItemFromKeystroke(e.KeyCode);
        }

        private void uninstallerObjectListView_SelectedChanged(object sender, EventArgs e)
        {
            toolStripLabelStatus.Text = _listView.SelectedUninstallerCount > 0
                ? string.Format(CultureInfo.CurrentCulture, Localisable.MainWindow_Statusbar_StatusSelection, _listView.SelectedUninstallerCount)
                : string.Empty;

            toolStripLabelSize.Text = _listView.GetSelectedSize().ToString();

            // Disable/enable edit menus
            var anySelected = _listView.SelectedUninstallerCount > 0;
            basicOperationsToolStripMenuItem.Enabled = anySelected;
            advancedOperationsToolStripMenuItem.Enabled = anySelected;

            toolStripButtonModify.Enabled = _listView.SelectedUninstallerCount == 1 &&
                _listView.SelectedUninstallers.Count(x => !string.IsNullOrEmpty(x.ModifyPath)) == 1;
        }

        private void UpdateUninstallListContextMenuStrip(object sender, CancelEventArgs e)
        {
            if (_listView.SelectedUninstallerCount == 0)
            {
                e.Cancel = true;
                return;
            }
            var advancedFiltering = advancedFilters1.CurrentList != null;
            toolStripSeparatorFiltering.Visible = advancedFiltering;
            excludeToolStripMenuItem.Visible = advancedFiltering;
            includeToolStripMenuItem.Visible = advancedFiltering;

            var selectedUninstallers = _listView.SelectedUninstallers.ToList();

            var singleItem = selectedUninstallers.Count == 1;
            uninstallUsingMsiExecContextMenuStripItem.Enabled =
                singleItem && !selectedUninstallers.First().BundleProviderKey.IsEmpty();

            foreach (var itemToDisable in new[]
            {
                uninstallContextMenuStripItem,
                quietUninstallContextMenuStripItem,
                gUIDProductCodeCopyContextMenuStripItem,
                uninstallStringCopyContextMenuStripItem,
                installLocationOpenInExplorerContextMenuStripItem,
                uninstallerLocationOpenInExplorerContextMenuStripItem,
                sourceLocationOpenInExplorerContextMenuStripItem,
                openWebPageContextMenuStripItem,
                runToolStripMenuItem,
                manualUninstallToolStripMenuItem1
            })
                itemToDisable.Enabled = false;

            runToolStripMenuItem.DropDownItems.Clear();

            foreach (var item in selectedUninstallers)
            {
                if (item.IsValid)
                {
                    if (item.UninstallPossible) uninstallContextMenuStripItem.Enabled = true;
                    if (item.QuietUninstallPossible) quietUninstallContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }

                if (singleItem)
                {
                    foreach (var executable in item.GetSortedExecutables())
                    {
                        if (!runToolStripMenuItem.Enabled) runToolStripMenuItem.Enabled = true;

                        runToolStripMenuItem.DropDownItems.Add(executable);
                    }
                }

                if (item.IsRegistered)
                    manualUninstallToolStripMenuItem1.Enabled = true;

                if (!item.BundleProviderKey.IsEmpty()) gUIDProductCodeCopyContextMenuStripItem.Enabled = true;
                if (item.UninstallPossible)
                {
                    uninstallStringCopyContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }

                if (item.InstallLocation.IsNotEmpty())
                {
                    installLocationOpenInExplorerContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }
                if (item.UninstallerLocation.IsNotEmpty())
                {
                    uninstallerLocationOpenInExplorerContextMenuStripItem.Enabled = true;
                    manualUninstallToolStripMenuItem1.Enabled = true;
                }
                if (item.InstallSource.IsNotEmpty()) sourceLocationOpenInExplorerContextMenuStripItem.Enabled = true;

                if (item.AboutUrl.IsNotEmpty()) openWebPageContextMenuStripItem.Enabled = true;
            }

            openInExplorerContextMenuStripItem.Enabled = installLocationOpenInExplorerContextMenuStripItem.Enabled ||
                                                         uninstallerLocationOpenInExplorerContextMenuStripItem.Enabled ||
                                                         sourceLocationOpenInExplorerContextMenuStripItem.Enabled;
        }

        private void UsageTrackerSendData()
        {
            if (_setMan.Selected.Settings.MiscSendStatistics)
            {
                UsageManager.FinishCollectingData();

                if (Program.EnableDebug || !WindowsTools.IsNetworkAvailable()) return;

                var count = UsageManager.AppLaunchCount;

                //Reduce frequency of the uploads
                if (count != 2 && (count <= 0 || count % 5 != 0)) return;

                try
                {
                    UsageManager.SendUsageData();
                }
                catch
                {
                    // Ignore, will try again next time
                }
            }
            else
            {
                UsageManager.RemoveStoredData();
            }
        }

        private void listView_ListRefreshIsRunningChanged(object sender,
            UninstallerListViewUpdater.ListRefreshEventArgs e)
        {
            if (e.RefreshIsRunning)
            {
                _uninstallerListPostProcesser.StopProcessingThread();
                return;
            }

            // If refresh has finished update the interface
            _anyStoreApps = _listView.AllUninstallers.Any(x => x.UninstallerKind == UninstallerType.StoreApp);
            _anyWinFeatures = _listView.AllUninstallers.Any(x => x.UninstallerKind == UninstallerType.WindowsFeature);

            _anyOrphans = _listView.AllUninstallers.Any(x => x.IsOrphaned);
            _anyProtected = _listView.AllUninstallers.Any(x => x.IsProtected);
            _anySysComponents = _listView.AllUninstallers.Any(x => x.SystemComponent);
            _anyUpdates = _listView.AllUninstallers.Any(x => x.IsUpdate);
            _anyInvalid = _listView.AllUninstallers.Any(x => !x.IsValid);
            _anyTweaks = _listView.AllUninstallers.Any(x => x.RatingId != null && x.RatingId.StartsWith("tweak", StringComparison.OrdinalIgnoreCase));

            propertiesSidebar.StoreAppsEnabled = _anyStoreApps;
            propertiesSidebar.WinFeaturesEnabled = _anyWinFeatures;
            propertiesSidebar.ShowTweaksEnabled = _anyTweaks;

            propertiesSidebar.OrphansEnabled = _anyOrphans;
            propertiesSidebar.ProtectedEnabled = _anyProtected;
            propertiesSidebar.SysCompEnabled = _anySysComponents;
            propertiesSidebar.UpdatesEnabled = _anyUpdates;
            propertiesSidebar.InvalidEnabled = _anyInvalid;

            if (e.FirstRefresh)
            {
                _setMan.LoadSorting();

                var args = Environment.GetCommandLineArgs();
                var dir = args.Skip(1).FirstOrDefault();
                if (!string.IsNullOrEmpty(dir))
                {
                    try
                    {
                        advancedFilters1.LoadUninstallList(dir);
                    }
                    catch (Exception ex)
                    { PremadeDialogs.GenericError(ex); }
                }
                if (advancedFilters1.CurrentList == null && _setMan.Selected.Settings.MiscAutoLoadDefaultList)
                {
                    try
                    {
                        var defaultUninstallListPath = Path.Combine(Program.AssemblyLocation.FullName, Resources.DefaultUninstallListFilename);
                        if (File.Exists(defaultUninstallListPath))
                            advancedFilters1.LoadUninstallList(defaultUninstallListPath);
                    }
                    catch (Exception ex)
                    { PremadeDialogs.GenericError(ex); }
                }

                splashScreen1.CloseSplashScreen();

                // Display the legend first so it is hidden under the splash
                _listLegendWindow.Opacity = 0;
                SetupAndShowLegendWindow();
                // Needed in case main window starts maximized
                _listLegendWindow.UpdatePosition(uninstallerObjectListView);
                _listLegendWindow.Visible = _setMan.Selected.Settings.UninstallerListShowLegend;

                new Thread(() =>
                {
                    this.SafeInvoke(() =>
                    {
                        var isFirstRun = _setMan.Selected.Settings.MiscFirstRun;
                        if (isFirstRun)
                        {
                            // Run the welcome wizard at first start of the application
                            OnFirstApplicationStart();
                        }

                        if (Program.IsAfterUpgrade || isFirstRun)
                        {
                            NewsPopup.ShowPopup(this);
                        }

                        //if (!_setMan.Selected.Settings.MiscNet4NagShown && !Program.Net4IsAvailable)
                        //{
                        //    _setMan.Selected.Settings.MiscNet4NagShown = true;
                        //    MessageBoxes.Net4MissingInfo();
                        //}
                    });
                }).Start();
            }

            OnApplicationListVisibleItemsChanged(sender, e);

            filterEditor1.FocusSearchbox();
        }

        private void openStartupManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var results = StartupManagerWindow.ShowManagerDialog(this);
            toolStripLabelStatus.Text = Localisable.MainWindow_Statusbar_RefreshingStartup;

            //Application.DoEvents();
            //if (_listView.CheckIsAppDisposed())
            //    return;

            Cursor = Cursors.WaitCursor;
            statusStrip1.Refresh();

            _listView.ReassignStartupEntries(true, results);
            toolStripLabelStatus.Text = string.Empty;
            Cursor = DefaultCursor;
        }

        private void disableAutostartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var uninstaller in _listView.SelectedUninstallers)
            {
                if (uninstaller.StartupEntries == null)
                    continue;

                foreach (var entry in uninstaller.StartupEntries)
                {
                    try
                    {
                        entry.Disabled = true;
                    }
                    catch (Exception ex)
                    {
                        PremadeDialogs.GenericError(ex);
                    }
                }

                uninstallerObjectListView.RefreshObject(uninstaller);
            }
        }

        private void rateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _uninstallerListConfigurator.RatingManagerWrapper.RateEntries(_listView.SelectedUninstallers.ToArray(), Point.Empty);
        }

        private void OpenTargetWindow(object sender, EventArgs e)
        {
            var results = TargetWindow.ShowDialog(this, SetVisible);

            if (results == null) return;

            var apps = AppUninstaller.GetApplicationsFromDirectories(_listView.AllUninstallers, results);

            if (apps.Count == 0)
            {
                MessageBoxes.UninstallFromDirectoryNothingFound();
                return;
            }

            _appUninstaller.RunUninstall(apps, _listView.AllUninstallers, true);
        }

        private void viewWindowsFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.FilterShowWinFeatures = true;
            filterEditor1.Search(nameof(UninstallerType.WindowsFeature), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind));
        }

        private void viewWindowsStoreAppsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.FilterShowStoreApps = true;
            filterEditor1.Search(nameof(UninstallerType.StoreApp), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind));
        }

        private void buttonAdvFiltering_Click(object sender, EventArgs e)
        {
            advancedFilters1.LoadUninstallList(new UninstallList(_uninstallerListConfigurator.GenerateEquivalentFilter()));
        }

        private void OpenAdvancedClipboardCopy(object sender, EventArgs e)
        {
            AdvancedClipboardCopyWindow.ShowDialog(this, _listView.SelectedUninstallers);
        }

        private void openHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxes.DisplayHelp();
        }

        private void uninstallFromDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.UninstallFromDirectory(_listView.AllUninstallers);
        }

        private void openSystemRestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WindowsTools.OpenControlPanelApplet(ControlPanelCanonicalNames.Recovery);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void runToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            PremadeDialogs.StartProcessSafely(e.ClickedItem.Text);
        }

        private void viewUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.FilterShowUpdates = true;
            filterEditor1.Search(true.ToString(), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsUpdate));
        }

        private void filterEditor1_FocusSearchTarget(object sender, EventArgs e)
        {
            uninstallerObjectListView.Focus();
        }

        private void googleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchGoogle(_listView.SelectedUninstallers);
        }

        private void alternativeToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchAlternativeTo(_listView.SelectedUninstallers);
        }

        private void slantcoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchSlantCo(_listView.SelectedUninstallers);
        }

        private void fossHubcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchFosshub(_listView.SelectedUninstallers);
        }

        private void sourceForgecomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchSourceforge(_listView.SelectedUninstallers);
        }

        private void fileHippocomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchFilehippo(_listView.SelectedUninstallers);
        }

        private void gitHubcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnlineSearchTools.SearchGithub(_listView.SelectedUninstallers);
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _appUninstaller.Modify(_listView.SelectedUninstallers);
        }

        private void excludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSelectedAsAdvancedFilters(true);
        }

        private void includeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSelectedAsAdvancedFilters(false);
        }

        private void AddSelectedAsAdvancedFilters(bool exclude)
        {
            var selectedUninstallers = _listView.SelectedUninstallers;
            var filters = advancedFilters1.CurrentList.Filters;

            var existingFilters = filters.Where(x => selectedUninstallers.Any(y => x.Name == y.DisplayName));
            filters.RemoveAll(existingFilters.ToList());

            filters.AddRange(selectedUninstallers.Select(x => new Filter(x.DisplayName, exclude, new FilterCondition(x.DisplayName, ComparisonMethod.Equals,
                    nameof(ApplicationUninstallerEntry.DisplayName)))));
            advancedFilters1.RepopulateList();
        }

        private void viewUnregisteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setMan.Selected.Settings.AdvancedDisplayOrphans = true;
            filterEditor1.Search(true.ToString(), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsOrphaned));
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
        }

        private void tryToInstallNETV35ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PremadeDialogs.StartProcessSafely(DismTools.DismFullPath, "/Online /Enable-Feature /FeatureName:NetFx3 /All");
        }

        private void startDiskCleanupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PremadeDialogs.StartProcessSafely("cleanmgr.exe");
        }

        private void exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new SaveFileDialog())
            {
                d.OverwritePrompt = true;
                d.CreatePrompt = false;
                d.AddExtension = true;
                d.DefaultExt = ".ps1";
                d.Filter = "PowerShell script|*.ps1";
                d.Title = exportStoreAppsToPowerShellRemoveScriptToolStripMenuItem.Text;
                d.RestoreDirectory = true;
                d.InitialDirectory = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_DESKTOPDIRECTORY);

                if (d.ShowDialog() == DialogResult.OK)
                {
                    try { File.WriteAllLines(d.FileName, StoreAppFactory.ToPowerShellRemoveCommands(_listView.SelectedUninstallers).ToArray()); }
                    catch (SystemException ex) { PremadeDialogs.GenericError(ex); }
                }
            }
        }

        private void exportToABatchUninstallScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new SaveFileDialog())
            {
                d.OverwritePrompt = true;
                d.CreatePrompt = false;
                d.AddExtension = true;
                d.DefaultExt = ".bat";
                d.Filter = "Batch file|*.bat";
                d.Title = exportToABatchUninstallScriptToolStripMenuItem.Text;
                d.RestoreDirectory = true;
                d.InitialDirectory = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_DESKTOPDIRECTORY);

                if (d.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllLines(d.FileName,
                            new[] { "@echo off", "echo BCU Generated batch uninstall script" }.Concat(
                            _listView.SelectedUninstallers
                                .Select(x => x.QuietUninstallPossible ? x.QuietUninstallString : x.UninstallString)
                                .Where(x => !string.IsNullOrEmpty(x))
                            ).Concat(new[] { "pause", "exit" }).ToArray());
                    }
                    catch (SystemException ex)
                    {
                        PremadeDialogs.GenericError(ex);
                    }
                }
            }
        }

        private void troubleshootUninstallProblemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var toolPath = Path.Combine(Program.AssemblyLocation.FullName,
                @"Resources\MicrosoftProgram_Install_and_Uninstall.meta.diagcab");
            PremadeDialogs.StartProcessSafely(toolPath);
        }

        private void filteringToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var isSimpleFiltering = !IsAdvancedFilteringEnabled;

            viewUnregisteredToolStripMenuItem.Enabled = isSimpleFiltering;
            viewUpdatesToolStripMenuItem.Enabled = isSimpleFiltering;
            viewWindowsStoreAppsToolStripMenuItem.Enabled = isSimpleFiltering;
            viewWindowsFeaturesToolStripMenuItem.Enabled = isSimpleFiltering;
        }

        private void automaticallyStartedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterEditor1.Search(true.ToString(), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.HasStartups));
            var s = _setMan.Selected.Settings;
            s.AdvancedDisplayOrphans = true;
            s.FilterHideMicrosoft = false;
            s.FilterShowSystemComponents = true;
            s.FilterShowProtected = true;
            s.FilterShowUpdates = true;
            s.FilterShowWinFeatures = true;
            s.FilterShowStoreApps = true;
            s.FilterShowTweaks = true;
        }

        private void basicApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterEditor1.Search(null, ComparisonMethod.Any);
            var s = _setMan.Selected.Settings;
            s.AdvancedDisplayOrphans = false;
            s.FilterHideMicrosoft = true;
            s.FilterShowSystemComponents = false;
            s.FilterShowProtected = false;
            s.FilterShowUpdates = false;
            s.FilterShowWinFeatures = false;
            s.FilterShowStoreApps = true;
            s.FilterShowTweaks = false;
        }

        private void advancedApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterEditor1.Search(null, ComparisonMethod.Any);
            var s = _setMan.Selected.Settings;
            s.AdvancedDisplayOrphans = true;
            s.FilterHideMicrosoft = true;
            s.FilterShowSystemComponents = false;
            s.FilterShowProtected = false;
            s.FilterShowUpdates = false;
            s.FilterShowWinFeatures = false;
            s.FilterShowStoreApps = true;
            s.FilterShowTweaks = true;
        }

        private void systemComponentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterEditor1.Search(true.ToString(), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.SystemComponent));
            var s = _setMan.Selected.Settings;
            s.AdvancedDisplayOrphans = false;
            s.FilterHideMicrosoft = false;
            s.FilterShowSystemComponents = true;
            s.FilterShowProtected = false;
            s.FilterShowUpdates = false;
            s.FilterShowWinFeatures = true;
            s.FilterShowStoreApps = true;
            s.FilterShowTweaks = true;
        }

        private void everythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterEditor1.Search(null, ComparisonMethod.Any);
            var s = _setMan.Selected.Settings;
            s.AdvancedDisplayOrphans = true;
            s.FilterHideMicrosoft = false;
            s.FilterShowSystemComponents = true;
            s.FilterShowProtected = true;
            s.FilterShowUpdates = true;
            s.FilterShowWinFeatures = true;
            s.FilterShowStoreApps = true;
            s.FilterShowTweaks = true;
        }

        private void onlyWebBrowsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterEditor1.Search(true.ToString(), ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsWebBrowser));
            var s = _setMan.Selected.Settings;
            s.AdvancedDisplayOrphans = true;
            s.FilterHideMicrosoft = false;
            s.FilterShowSystemComponents = true;
            s.FilterShowProtected = true;
            s.FilterShowUpdates = true;
            s.FilterShowWinFeatures = true;
            s.FilterShowStoreApps = true;
            s.FilterShowTweaks = true;
        }

        private void viewTweaksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            everythingToolStripMenuItem_Click(sender, e);
            filterEditor1.Search(@"\Resources\Scripts\Tweak", ComparisonMethod.Contains, nameof(ApplicationUninstallerEntry.UninstallString));
        }

        private void createRestorePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LockApplication(true);
            try
            {
                SystemRestore.BeginSysRestore(0, false, this);
            }
            catch (Exception exception)
            {
                PremadeDialogs.GenericError(exception);
            }
            finally
            {
                LockApplication(false);
            }
        }

        private void autosizeAllColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uninstallerObjectListView.AutoResizeColumns();
        }
    }
}
