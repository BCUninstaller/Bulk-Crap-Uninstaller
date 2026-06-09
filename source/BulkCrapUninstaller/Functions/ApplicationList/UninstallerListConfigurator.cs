/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Functions.Ratings;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Localising;
using Klocman.Resources;
using UninstallTools;
using UninstallTools.Factory;
using UninstallTools.Lists;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class UninstallerListConfigurator : IDisposable
    {
        private readonly FilterCondition _filteringFilterCondition = new() { FilterText = string.Empty };
        private readonly TypedObjectListView<ApplicationUninstallerEntry> _listView;
        private readonly MainWindow _reference;

        private readonly Timer _updateThrottleTimer;

        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;

        public UninstallerListConfigurator(MainWindow reference)
        {
            _reference = reference;
            _listView = new TypedObjectListView<ApplicationUninstallerEntry>(reference.uninstallerObjectListView);

            _reference.filterEditor1.TargetFilterCondition = _filteringFilterCondition;

            _updateThrottleTimer = new Timer { Interval = 500 };
            _updateThrottleTimer.Tick += (sender, args) =>
            {
                _updateThrottleTimer.Stop();
                _listView.ListView.UpdateColumnFiltering();
            };

            SetupListView();

            RatingManagerWrapper = new RatingManagerWrapper();
            RatingManagerWrapper.InitializeRatingColumn(_reference.olvColumnRating, _reference.uninstallerObjectListView);
            _reference.FormClosed += (x, y) => { RatingManagerWrapper.ProcessGatheredRatings(); };

            _settings.Subscribe((sender, args) => RatingManagerWrapper.InitializeRatings(), x => x.MiscUserRatings, this);
        }

        public ITestEntry FilteringOverride { get; set; }

        public RatingManagerWrapper RatingManagerWrapper { get; }

        public void Dispose()
        {
            _updateThrottleTimer.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1713:Events should not have 'Before' or 'After' prefix", Justification = "<Pending>")]
        public event EventHandler AfterFiltering;

        /// <summary>
        /// Return a filter equivalent to current basic filtering settings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Filter> GenerateEquivalentFilter()
        {
            var results = new List<Filter>();

            if (string.IsNullOrEmpty(_filteringFilterCondition.FilterText))
                results.Add(new Filter("Include all", false, new FilterCondition("!",
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsOrphaned))
                { InvertResults = true }));
            else
                results.Add(new Filter(_filteringFilterCondition.FilterText, false,
                    (FilterCondition)_filteringFilterCondition.Clone()));

            if (_settings.Settings.FilterHideMicrosoft)
                results.Add(new Filter("Published by Microsoft", true, new FilterCondition("Microsoft",
                    ComparisonMethod.Contains, nameof(ApplicationUninstallerEntry.Publisher))));

            if (!_settings.Settings.FilterShowStoreApps)
                results.Add(new Filter("Store Apps", true, new FilterCondition(nameof(UninstallerType.StoreApp),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind))));

            if (!_settings.Settings.FilterShowWinFeatures)
                results.Add(new Filter("Windows Features", true,
                    new FilterCondition(nameof(UninstallerType.WindowsFeature),
                        ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.UninstallerKind))));

            if (!_settings.Settings.AdvancedDisplayOrphans)
                results.Add(new Filter("Orphaned apps", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsOrphaned))));

            if (!_settings.Settings.FilterShowProtected)
                results.Add(new Filter("Protected apps", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsProtected))));

            if (!_settings.Settings.FilterShowSystemComponents)
                results.Add(new Filter("System Components", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.SystemComponent))));

            if (!_settings.Settings.FilterShowUpdates)
                results.Add(new Filter("Updates", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsUpdate))));

            if (!_settings.Settings.FilterShowTweaks)
                results.Add(new Filter("Tweaks", true, new FilterCondition(true.ToString(),
                    ComparisonMethod.Equals, nameof(ApplicationUninstallerEntry.IsScriptTweak))));

            return results;
        }

        private bool ListViewFilter(object obj)
        {
            if (obj is not ApplicationUninstallerEntry entry) return false;

            if (FilteringOverride != null) return FilteringOverride.TestEntry(entry) == true;

            if (_settings.Settings.FilterHideMicrosoft && !string.IsNullOrEmpty(entry.Publisher) &&
                entry.Publisher.Contains("Microsoft"))
                return false;

            if (!_settings.Settings.FilterShowStoreApps && entry.UninstallerKind == UninstallerType.StoreApp)
                return false;

            if (!_settings.Settings.FilterShowWinFeatures && entry.UninstallerKind == UninstallerType.WindowsFeature)
                return false;

            if (!_settings.Settings.AdvancedDisplayOrphans && entry.IsOrphaned) return false;

            if (!_settings.Settings.FilterShowProtected && entry.IsProtected) return false;

            if (!_settings.Settings.FilterShowSystemComponents && entry.SystemComponent) return false;

            if (!_settings.Settings.FilterShowUpdates && entry.IsUpdate) return false;

            if (!_settings.Settings.FilterShowTweaks && entry.IsScriptTweak)
                return false;

            if (string.IsNullOrEmpty(_filteringFilterCondition.FilterText)) return true;

            return _filteringFilterCondition.TestEntry(entry) == true;
        }

        public void SetupListView()
        {
            _reference.uninstallerObjectListView.VirtualMode = false;

            _reference.olvColumnDisplayName.AspectName = RegistryFactory.RegistryNameDisplayName;
            _reference.olvColumnDisplayName.GroupKeyGetter = ListViewDelegates.GetFirstCharGroupKeyGetter;

            _reference.olvColumnStartup.AspectGetter = x =>
            {
                var obj = x as ApplicationUninstallerEntry;
                return (obj?.HasStartups).ToYesNo();
            };

            _reference.olvColumnPublisher.AspectName = RegistryFactory.RegistryNamePublisher;
            _reference.olvColumnPublisher.GroupKeyGetter = ListViewDelegates.ColumnPublisherGroupKeyGetter;

            _reference.olvColumnDisplayVersion.AspectName = RegistryFactory.RegistryNameDisplayVersion;
            _reference.olvColumnDisplayVersion.GroupKeyGetter = ListViewDelegates.DisplayVersionGroupKeyGetter;

            _reference.olvColumnUninstallString.AspectGetter = ListViewDelegates.ColumnUninstallStringGetter;
            _reference.olvColumnUninstallString.GroupKeyGetter = ListViewDelegates.ColumnUninstallStringGroupKeyGetter;

            _reference.olvColumnQuietUninstallString.AspectGetter = ListViewDelegates.ColumnQuietUninstallStringGetter;
            _reference.olvColumnQuietUninstallString.GroupKeyGetter = ListViewDelegates.ColumnQuietUninstallStringGroupKeyGetter;

            _reference.olvColumnInstallDate.AspectGetter = x =>
            {
                var obj = x as ApplicationUninstallerEntry;
                return obj?.InstallDate.Date ?? DateTime.MinValue;
            };
            //_reference.olvColumnInstallDate.AspectName = ApplicationUninstallerEntry.RegistryNameInstallDate;
            _reference.olvColumnInstallDate.AspectToStringConverter = x =>
            {
                if (x is not DateTime time) return Localisable.Empty;
                try
                {
                    return time.IsDefault() ? Localisable.Empty : time.ToShortDateString();
                }
                catch (SystemException)
                {
                    return Localisable.NotAvailable;
                }
            };

            _reference.olvColumnGuid.AspectGetter = ListViewDelegates.ColumnGuidAspectGetter;
            _reference.olvColumnGuid.GroupKeyGetter = ListViewDelegates.ColumnGuidGroupKeyGetter;

            _reference.olvColumnSystemComponent.AspectName = RegistryFactory.RegistryNameSystemComponent;
            _reference.olvColumnSystemComponent.AspectToStringConverter = ListViewDelegates.BoolToYesNoAspectConverter;
            _reference.olvColumnSystemComponent.GroupKeyToTitleConverter = ListViewDelegates.BoolToYesNoAspectConverter;

            _reference.olvColumnCertificate.AspectGetter = y => ApplicationListConstants.GetApplicationCertificateText(y as ApplicationUninstallerEntry);

            _reference.olvColumnIntegrity.AspectGetter = y => ApplicationListConstants.GetApplicationIntegrityText(y as ApplicationUninstallerEntry);
            _reference.olvColumnIntegrity.AspectToStringConverter = x => x is string[] arr ? string.Join(", ", arr) : x?.ToString();

            _reference.olvColumnIs64.AspectGetter =
                y => (y as ApplicationUninstallerEntry)?.Is64Bit.GetLocalisedName();

            _reference.olvColumnProtected.AspectToStringConverter = ListViewDelegates.BoolToYesNoAspectConverter;
            _reference.olvColumnProtected.GroupKeyToTitleConverter = ListViewDelegates.BoolToYesNoAspectConverter;

            _reference.olvColumnInstallLocation.AspectName = RegistryFactory.RegistryNameInstallLocation;
            _reference.olvColumnInstallLocation.GroupKeyGetter = ListViewDelegates.ColumnInstallLocationGroupKeyGetter;

            _reference.olvColumnInstallSource.AspectName = RegistryFactory.RegistryNameInstallSource;
            _reference.olvColumnInstallSource.GroupKeyGetter = ListViewDelegates.ColumnInstallSourceGroupKeyGetter;

            _reference.olvColumnRegistryKeyName.AspectName = "RegistryKeyName";

            _reference.olvColumnUninstallerKind.AspectGetter =
                y => (y as ApplicationUninstallerEntry)?.UninstallerKind.GetLocalisedName();

            _reference.olvColumnAbout.AspectName = "AboutUrl";
            _reference.olvColumnAbout.GroupKeyGetter = x =>
            {
                var entry = x as ApplicationUninstallerEntry;
                var aboutUri = entry?.GetAboutUri();
                return aboutUri?.Host ?? Localisable.Empty;
            };

            _reference.olvColumnSize.TextAlign = HorizontalAlignment.Right;
            _reference.olvColumnSize.AspectGetter = ListViewDelegates.ColumnSizeAspectGetter;
            _reference.olvColumnSize.AspectToStringConverter = ListViewDelegates.AspectToStringConverter;
            _reference.olvColumnSize.GroupKeyGetter = ListViewDelegates.ColumnSizeGroupKeyGetter;
            _reference.olvColumnSize.GroupKeyToTitleConverter = x => x is long num and > 0 ? FileSize.GetUnitName(num) : CommonStrings.Unknown;

            _reference.uninstallerObjectListView.PrimarySortColumn = _reference.olvColumnDisplayName;
            _reference.uninstallerObjectListView.SecondarySortColumn = _reference.olvColumnPublisher;
            _reference.uninstallerObjectListView.Sorting = SortOrder.Ascending;

            _reference.uninstallerObjectListView.AdditionalFilter = new ModelFilter(ListViewFilter);
            _reference.uninstallerObjectListView.UseFiltering = true;

            _reference.uninstallerObjectListView.FormatRow += UninstallerObjectListView_FormatRow;

            _listView.ListView.AfterSorting += (x, y) => { AfterFiltering?.Invoke(x, y); };
        }

        private void UninstallerObjectListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (e.Model is not ApplicationUninstallerEntry entry) return;

            var color = ApplicationListConstants.GetApplicationBackColor(entry);
            if (!color.IsEmpty)
                e.Item.BackColor = color;
        }

        public void UpdateColumnFiltering(bool anyUninstallers)
        {
            _listView.ListView.EmptyListMsg = anyUninstallers
                ? Localisable.SearchNothingFoundMessage
                : null;

            _updateThrottleTimer.Stop();
            _updateThrottleTimer.Start();
        }
    }
}
