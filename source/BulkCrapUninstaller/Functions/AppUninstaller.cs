/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using BulkCrapUninstaller.Functions.Tools;
using BulkCrapUninstaller.Properties;
using Klocman;
using Klocman.Extensions;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools;
using UninstallTools.Factory;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Junk;
using UninstallTools.Junk.Containers;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Functions
{
    internal class AppUninstaller
    {
        private readonly Action _initiateListRefresh;
        private readonly Action<bool> _lockApplication;
        private readonly Action<bool> _visibleCallback;
        private readonly Settings _settings = Settings.Default;
        private readonly object _uninstallLock = new();

        /// <summary>
        ///     Uninstall tasks will wait until this is released to continue. Keep it short to prevent ui unresponsiveness.
        /// </summary>
        public readonly object PublicUninstallLock = new();

        private static readonly int MyProcessId = Environment.ProcessId;

        /// <exception cref="ArgumentNullException"> One of arguments is <see langword="null" />.</exception>
        internal AppUninstaller(Action listRefreshCallback, Action<bool> applicationLockCallback, Action<bool> visibleCallback)
        {
            _initiateListRefresh = listRefreshCallback ?? throw new ArgumentNullException(nameof(listRefreshCallback));
            _lockApplication = applicationLockCallback ?? throw new ArgumentNullException(nameof(applicationLockCallback));
            _visibleCallback = visibleCallback;
        }

        /// <summary>
        ///     Returns false if export failed, else true.
        /// </summary>
        /// <param name="itemsToExport">What to export</param>
        /// <param name="filename">Full path with filename and extension to write the export result to.</param>
        /// <returns></returns>
        public static bool ExportUninstallers(IEnumerable<ApplicationUninstallerEntry> itemsToExport, string filename)
        {
            var applicationUninstallerEntries = itemsToExport as List<ApplicationUninstallerEntry> ??
                                                itemsToExport.ToList();
            if (applicationUninstallerEntries.Count <= 0)
                return false;

            try
            {
                ApplicationEntrySerializer.SerializeApplicationEntries(filename, applicationUninstallerEntries);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExportFailed(ex.Message, null);
                return false;
            }
            return true;
        }

        internal static int[] GetRelatedProcessIds(IEnumerable<ApplicationUninstallerEntry> entries, bool doNotKillSteam)
        {
            var filters = entries.SelectMany(e => new[] { e.InstallLocation, e.UninstallerLocation })
                .Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();

            return GetRelatedProcessIds(filters, doNotKillSteam);
        }

        private static bool CheckForRunningProcessesBeforeUninstall(IEnumerable<ApplicationUninstallerEntry> entries, bool doNotKillSteam)
        {
            var filters = entries.SelectMany(e => new[] { e.InstallLocation, e.UninstallerLocation })
                .Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();

            return CheckForRunningProcesses(filters, doNotKillSteam);
        }

        private static bool CheckForRunningProcessesBeforeCleanup(IEnumerable<IJunkResult> entries)
        {
            var filters = entries.OfType<FileSystemJunk>()
                .Select(x => x.Path.FullName)
                .Distinct().ToArray();

            return CheckForRunningProcesses(filters, false);
        }

        public static IEnumerable<ApplicationUninstallerEntry> GetApplicationsFromProcess(
            IEnumerable<ApplicationUninstallerEntry> allApplications, Process targetProcess)
        {
            if (targetProcess == null) throw new ArgumentNullException(nameof(targetProcess));
            if (targetProcess.MainModule?.FileName == null) throw new ArgumentException("MainModule is null");

            var mainFilename = targetProcess.MainModule.FileName;
            return from app in allApplications
                   where
                       (app.IsInstallLocationValid() &&
                        mainFilename.Contains(app.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                       ||
                       (!string.IsNullOrEmpty(app.UninstallerLocation) &&
                        mainFilename.Contains(app.UninstallerLocation, StringComparison.InvariantCultureIgnoreCase))
                   select app;
        }

        internal static bool CheckForRunningProcesses(string[] filters, bool doNotKillSteam, Form parentForm = null)
        {
            var idsToCheck = GetRelatedProcessIds(filters, doNotKillSteam);

            if (idsToCheck.Length > 0)
            {
                if (!ProcessWaiter.ShowDialog(parentForm ?? MessageBoxes.DefaultOwner, idsToCheck.ToArray(), false))
                    return false;
            }

            return true;
        }

        private static int[] GetRelatedProcessIds(string[] filters, bool doNotKillSteam)
        {
            var idsToCheck = new List<int>();
            foreach (var pr in Process.GetProcesses())
            {
                try
                {
                    if (pr.Id == MyProcessId || pr.HasExited)
                        continue;

                    if (doNotKillSteam && pr.ProcessName.Equals("steam", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (string.IsNullOrEmpty(pr.MainModule?.FileName) ||
                        pr.MainModule.FileName.StartsWith(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_SYSTEM), StringComparison.OrdinalIgnoreCase))
                        continue;

                    var filenames = pr.Modules.Cast<ProcessModule>()
                        .Select(x => x.FileName)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct();

                    if (filenames.Any(filename => filters.Any(filter =>
                    {
                        if (string.IsNullOrEmpty(filename))
                            return false;

                        if (!Path.IsPathRooted(filename))
                            return false;

                        return filename.StartsWith(filter, StringComparison.OrdinalIgnoreCase);
                    })))
                    {
                        idsToCheck.Add(pr.Id);
                    }
                }
                catch
                {
                    // Ignore invalid processes
                }
            }

            return idsToCheck.ToArray();
        }

        public void RunUninstall(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers, bool quiet)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                var targetList = new List<ApplicationUninstallerEntry>(selectedUninstallers);
                var allUninstallerList = allUninstallers as IList<ApplicationUninstallerEntry> ?? allUninstallers.ToList();

                if (!_settings.AdvancedDisableProtection)
                {
                    var protectedTargets = targetList.Where(x => x.IsProtected).ToList();
                    if (
                        MessageBoxes.ProtectedItemsWarningQuestion(protectedTargets.Select(x => x.DisplayName).ToArray()) ==
                        MessageBoxes.PressedButton.Cancel)
                        return;

                    targetList.RemoveAll(protectedTargets);
                }

                if (targetList.Any())
                {
                    _lockApplication(true);

                    IReadOnlyList<BulkUninstallEntry> taskEntries;

                    using (var wizard = new BeginUninstallTaskWizard())
                    {
                        wizard.Initialize(targetList, allUninstallerList.ToList(), quiet);

                        wizard.StartPosition = FormStartPosition.CenterParent;
                        if (wizard.ShowDialog(MessageBoxes.DefaultOwner) != DialogResult.OK || wizard.Results.Count == 0)
                            return;

                        taskEntries = wizard.Results;
                    }

                    _visibleCallback(false);

                    // No turning back at this point (kind of)
                    listRefreshNeeded = true;

                    if (_settings.MessagesRestorePoints != YesNoAsk.No)
                    {
                        try
                        {
                            SystemRestore.BeginSysRestore(taskEntries.Count, true);
                        }
                        catch (Exception exception)
                        {
                            PremadeDialogs.GenericError(exception);
                        }
                    }

                    if (_settings.ExternalEnable && _settings.ExternalPreCommands.IsNotEmpty())
                    {
                        LoadingDialog.ShowDialog(
                            MessageBoxes.DefaultOwner, Localisable.LoadingDialogTitlePreUninstallCommands,
                            controller => { RunExternalCommands(_settings.ExternalPreCommands, controller); });
                    }

                    var status = UninstallManager.CreateBulkUninstallTask(taskEntries, GetConfiguration(quiet));
                    status.OneLoudLimit = _settings.UninstallConcurrentOneLoud;
                    status.ConcurrentUninstallerCount = _settings.UninstallConcurrency
                        ? _settings.UninstallConcurrentMaxCount
                        : 1;
                    status.Start();

                    UninstallProgressWindow.ShowUninstallDialog(status, entries => SearchForAndRemoveJunk(entries, allUninstallerList));

                    var junkRemoveTargetsQuery = from bulkUninstallEntry in status.AllUninstallersList
                                                 where bulkUninstallEntry.CurrentStatus == UninstallStatus.Completed
                                                       || bulkUninstallEntry.CurrentStatus == UninstallStatus.Invalid
                                                       || (bulkUninstallEntry.CurrentStatus == UninstallStatus.Skipped
                                                           && !bulkUninstallEntry.UninstallerEntry.RegKeyStillExists())
                                                 select bulkUninstallEntry.UninstallerEntry;

                    if (MessageBoxes.LookForJunkQuestion())
                        SearchForAndRemoveJunk(junkRemoveTargetsQuery, allUninstallerList);

                    if (_settings.ExternalEnable && _settings.ExternalPostCommands.IsNotEmpty())
                    {
                        LoadingDialog.ShowDialog(
                            MessageBoxes.DefaultOwner, Localisable.LoadingDialogTitlePostUninstallCommands,
                            controller => { RunExternalCommands(_settings.ExternalPostCommands, controller); });
                    }
                }
                else
                {
                    MessageBoxes.NoUninstallersSelectedInfo();
                }
            }
            catch (ObjectDisposedException ex)
            {
                // ODE at CreateHandle can be caused by closing main window in the middle of the process
                // It gets thrown at ShowDialog, it's safe to cancel the process at these points
                if (ex.TargetSite?.Name != "CreateHandle") throw;
            }
            finally
            {
                SystemRestore.EndSysRestore();
                ReleaseUninstallLock();
                _lockApplication(false);
                _visibleCallback(true);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        public static IEnumerable<T> SortIntelligently<T>(IEnumerable<T> entries, Func<T, BulkUninstallEntry> entryGetter)
        {
            var query = from x in entries
                        let item = entryGetter(x)
                        orderby
                            // For safety always run simple deletes last so that actual uninstallers have a chance to run
                            item.UninstallerEntry.UninstallerKind == UninstallerType.SimpleDelete ascending,
                            // Always run loud first so later user can have some time to watch cat pics
                            item.IsSilentPossible ascending,
                            // Updates usually get uninstalled by their parent uninstallers
                            item.UninstallerEntry.IsUpdate ascending,
                            // SysCmps and Protected usually get uninstalled by their parent, user-visible uninstallers
                            item.UninstallerEntry.SystemComponent ascending,
                            item.UninstallerEntry.IsProtected ascending,
                            // Calculate number of digits (Floor of Log10 + 1) and divide it by 4 to create buckets of sizes
                            Math.Round(Math.Floor(Math.Log10(item.UninstallerEntry.EstimatedSize.GetKbSize(true)) + 1) / 4) descending,
                            // Prioritize Msi uninstallers because they tend to take the longest
                            item.UninstallerEntry.UninstallerKind == UninstallerType.Msiexec descending,
                            // Final sorting to get things deterministic
                            item.UninstallerEntry.EstimatedSize.GetKbSize(true) descending
                        select x;
            return query;
        }

        public static IEnumerable<BulkUninstallEntry> SortIntelligently(List<BulkUninstallEntry> taskEntries)
        {
            return SortIntelligently(taskEntries, entry => entry);
        }

        public void AdvancedUninstall(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                _lockApplication(true);

                listRefreshNeeded = SearchForAndRemoveJunk(selectedUninstallers, allUninstallers);
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        /// <summary>
        ///     Returns true if things were actually removed, false if user cancelled the operation.
        /// </summary>
        private bool SearchForAndRemoveJunk(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            var junk = new List<IJunkResult>();
            var error = LoadingDialog.ShowDialog(MessageBoxes.DefaultOwner, Localisable.LoadingDialogTitleLookingForJunk,
                dialogInterface =>
                {
                    var allValidUninstallers = allUninstallers.Where(y => y.RegKeyStillExists());

                    dialogInterface.SetSubProgressVisible(true);
                    junk.AddRange(JunkManager.FindJunk(selectedUninstallers, allValidUninstallers.ToList(), x =>
                    {
                        if (x.TotalCount <= 1)
                        {
                            // Don't update the title label to uninstaller name when there's only one uninstaller
                            dialogInterface.SetMaximum(-1);
                        }
                        else
                        {
                            dialogInterface.SetMaximum(x.TotalCount);
                            dialogInterface.SetProgress(x.CurrentCount, x.Message);
                        }

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
                    }));
                });

            if (error != null)
            {
                PremadeDialogs.GenericError(error);
                return false;
            }

            return ShowJunkWindow(junk);
        }

        private bool ShowJunkWindow(List<IJunkResult> junk)
        {
            if (!junk.Any(x => _settings.MessagesShowAllBadJunk || x.Confidence.GetRawConfidence() >= 0))
            {
                MessageBoxes.NoJunkFoundInfo();
                return false;
            }

            using (var junkWindow = new JunkRemoveWindow(junk))
            {
                if (junkWindow.ShowDialog() != DialogResult.OK) return false;

                var selectedJunk = junkWindow.SelectedJunk.ToList();

                if (!CheckForRunningProcessesBeforeCleanup(selectedJunk)) return false;

                //Removing the junk
                LoadingDialog.ShowDialog(MessageBoxes.DefaultOwner, Localisable.LoadingDialogTitleRemovingJunk, controller =>
                {
                    var top = selectedJunk.Count;
                    controller.SetMaximum(top);
                    var itemsRemoved = 0; // current value

                    var sortedJunk = from item in selectedJunk
                                         // Run commands before deleting any files or reg keys to avoid missing files
                                     orderby item is RunProcessJunk descending,
                                     // Need to stop and unregister service before deleting its exe
                                             item is StartupJunkNode descending
                                     select item;

                    foreach (var junkNode in sortedJunk)
                    {
                        controller.SetProgress(itemsRemoved++);

                        if (_settings.AdvancedSimulate)
                        {
                            Thread.Sleep(100);
                        }
                        else
                        {
                            try
                            {
                                junkNode.Delete();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception while removing junk: " + ex);
                            }
                        }
                    }
                });

                return true;
            }
        }

        public void SearchForAndRemoveProgramFilesJunk(IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            if (!TryGetUninstallLock()) return;

            try
            {
                _lockApplication(true);

                var junk = new List<IJunkResult>();
                var error = LoadingDialog.ShowDialog(null, Localisable.LoadingDialogTitleLookingForJunk,
                    x => junk.AddRange(JunkManager.FindProgramFilesJunk(allUninstallers.Where(y => y.RegKeyStillExists()).ToList())));

                if (error != null)
                    PremadeDialogs.GenericError(error);
                else
                    ShowJunkWindow(junk);
            }
            catch (ObjectDisposedException) { }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
            }
        }

        public void UninstallUsingMsi(MsiUninstallModes mode,
            IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                _lockApplication(true);

                var results = selectedUninstallers.Take(2).ToList();

                if (results.Count != 1)
                {
                    MessageBoxes.CanSelectOnlyOneItemInfo();
                    return;
                }

                var selected = results.First();

                if (!_settings.AdvancedDisableProtection && selected.IsProtected)
                {
                    MessageBoxes.ProtectedItemError(selected.DisplayName);
                    return;
                }

                if (selected.BundleProviderKey.IsEmpty())
                {
                    MessageBoxes.UninstallMsiGuidMissing();
                    return;
                }

                if (!CheckForRunningProcessesBeforeUninstall(new[] { selected }, true))
                    return;

                try
                {
                    selected.UninstallUsingMsi(mode, _settings.AdvancedSimulate);
                    listRefreshNeeded = true;
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        public void UninstallFromDirectory(IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            var applicationUninstallerEntries = allUninstallers as IList<ApplicationUninstallerEntry> ?? allUninstallers.ToList();

            try
            {
                var result = MessageBoxes.SelectFolder(Localisable.UninstallFromDirectory_FolderBrowse);

                if (result == null) return;

                var items = new List<ApplicationUninstallerEntry>();
                LoadingDialog.ShowDialog(MessageBoxes.DefaultOwner, Localisable.UninstallFromDirectory_ScanningTitle,
                    _ => items.AddRange(DirectoryFactory.TryCreateFromDirectory(new DirectoryInfo(result), Array.Empty<string>())));

                if (items.Count == 0)
                    items.AddRange(applicationUninstallerEntries.Where(x => PathTools.PathsEqual(result, x.InstallLocation)));

                if (items.Count == 0)
                    MessageBoxes.UninstallFromDirectoryNothingFound();
                else
                {

                    foreach (var item in items.ToList())
                    {
                        if (item.UninstallPossible && item.UninstallerKind != UninstallerType.SimpleDelete &&
                            MessageBoxes.UninstallFromDirectoryUninstallerFound(item.DisplayName, item.UninstallString))
                        {
                            try { item.RunUninstaller(false, Settings.Default.AdvancedSimulate).WaitForExit(60000); }
                            catch (Exception ex) { PremadeDialogs.GenericError(ex); }
                            items.Remove(item);
                            listRefreshNeeded = true;
                        }
                        else
                        {
                            var found = applicationUninstallerEntries.Where(
                                x => PathTools.PathsEqual(item.InstallLocation, x.InstallLocation)).ToList();

                            if (!found.Any()) continue;

                            items.Remove(item);

                            foreach (var entry in found)
                            {
                                if (entry.UninstallPossible && entry.UninstallerKind != UninstallerType.SimpleDelete &&
                                    MessageBoxes.UninstallFromDirectoryUninstallerFound(entry.DisplayName, entry.UninstallString))
                                {
                                    try { item.RunUninstaller(false, Settings.Default.AdvancedSimulate).WaitForExit(60000); }
                                    catch (Exception ex) { PremadeDialogs.GenericError(ex); }

                                    listRefreshNeeded = true;
                                }
                                else
                                    items.Add(entry);
                            }
                        }
                    }

                    AdvancedUninstall(items, applicationUninstallerEntries.Where(
                        x => !items.Any(y => PathTools.PathsEqual(y.InstallLocation, x.InstallLocation))));
                }
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }

        /// <summary>
        ///     Ask to self uninstall and do so if user agrees, else return and do nothing.
        /// </summary>
        internal void AskToSelfUninstall()
        {
            if (MessageBoxes.SelfUninstallQuestion())
            {
                if (!TryGetUninstallLock()) return;

                try
                {
                    var fullPath = MessageBoxes.GetBundledFilePath("unins000.exe");
                    if (fullPath != null)
                    {
                        Process.Start(fullPath);
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
                finally
                {
                    ReleaseUninstallLock();
                }
            }
        }

        private BulkUninstallConfiguration GetConfiguration(bool quiet)
        {
            return new BulkUninstallConfiguration(_settings.AdvancedDisableProtection, quiet, _settings.AdvancedSimulate,
                _settings.QuietAutoKillStuck, _settings.QuietRetryFailedOnce);
        }

        private void ReleaseUninstallLock()
        {
            Monitor.Exit(PublicUninstallLock);
            Monitor.Exit(_uninstallLock);
        }

        private static void RunExternalCommands(string commands, LoadingDialogInterface controller)
        {
            var lines = commands.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);
            controller.SetMaximum(lines.Length);

            for (var i = 0; i < lines.Length; i++)
            {
                controller.SetProgress(i);

                var line = lines[i];
                try
                {
                    var filename = ProcessTools.SeparateArgsFromCommand(line);
                    filename.FileName = Path.GetFullPath(filename.FileName);
                    if (!File.Exists(filename.FileName))
                        throw new IOException(Localisable.Error_FileNotFound);
                    filename.ToProcessStartInfo().StartAndWait();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Localisable.MessageBoxes_ExternalCommandFailed_Message, line)
                                    + Localisable.MessageBoxes_Error_details + ex.Message,
                        Localisable.MessageBoxes_ExternalCommandFailed_Title,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        ///     Attempt to get the lock and display error popup if the process fails.
        /// </summary>
        /// <returns>True if lock was succesfully acquired, otherwise false.</returns>
        private bool TryGetUninstallLock()
        {
            if (Monitor.TryEnter(_uninstallLock))
            {
                Monitor.Enter(PublicUninstallLock);
                return true;
            }

            MessageBoxes.UninstallAlreadyRunning();
            return false;
        }

        public static ICollection<ApplicationUninstallerEntry> GetApplicationsFromDirectories(
            IEnumerable<ApplicationUninstallerEntry> allUninstallers, ICollection<DirectoryInfo> results)
        {
            var output = new List<ApplicationUninstallerEntry>();
            var processed = new List<DirectoryInfo>();
            foreach (var dir in results.Distinct(PathTools.PathsEqual).OrderBy(x => x.FullName))
            {
                if (processed.Any(x => x.FullName.Contains(dir.FullName, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                processed.Add(dir);
            }

            var exceptions = allUninstallers.Where(x => x.InstallLocation != null).ToList();

            var infoAdder = new InfoAdderManager();
            foreach (var dir in processed)
            {
                var items = exceptions.Where(
                    x => x.InstallLocation.Contains(dir.FullName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                if (items.Count > 0)
                {
                    output.AddRange(items);
                    continue;
                }

                var res = DirectoryFactory.TryCreateFromDirectory(dir, exceptions).ToList();
                if (res.Count == 0)
                {
                    MessageBox.Show(
                        string.Format(Localisable.Uninstaller_GetApplicationsFromDirectories_NothingFound_Message,
                            dir.FullName), Localisable.Uninstaller_GetApplicationsFromDirectories_NothingFound_Title,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                foreach (var result in res)
                {
                    infoAdder.AddMissingInformation(result);
                    output.Add(result);
                }
            }

            return output;
        }

        public void Modify(IEnumerable<ApplicationUninstallerEntry> selectedUninstallers)
        {
            if (!TryGetUninstallLock()) return;
            var listRefreshNeeded = false;

            try
            {
                _lockApplication(true);

                var results = selectedUninstallers.Take(2).ToList();

                if (results.Count != 1)
                {
                    MessageBoxes.CanSelectOnlyOneItemInfo();
                    return;
                }

                var selected = results.First();

                if (!_settings.AdvancedDisableProtection && selected.IsProtected)
                {
                    MessageBoxes.ProtectedItemError(selected.DisplayName);
                    return;
                }

                if (string.IsNullOrEmpty(selected.ModifyPath))
                {
                    MessageBoxes.ModifyCommandMissing();
                    return;
                }

                if (!CheckForRunningProcessesBeforeUninstall(new[] { selected }, true))
                    return;

                try
                {
                    selected.Modify(_settings.AdvancedSimulate);
                    listRefreshNeeded = true;
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
            finally
            {
                ReleaseUninstallLock();
                _lockApplication(false);
                if (listRefreshNeeded)
                    _initiateListRefresh();
            }
        }
    }
}