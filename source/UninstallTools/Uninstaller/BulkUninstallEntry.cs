/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Factory;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller
{
    public class BulkUninstallEntry
    {
        private static readonly string[] NamesOfIgnoredProcesses =
            WindowsTools.GetInstalledWebBrowsers().Select(s =>
            {
                try
                {
                    return Path.GetFileNameWithoutExtension(s);
                }
                catch (ArgumentException)
                {
                    try
                    {
                        var dash = s.LastIndexOf('\\');
                        return s.Substring(dash + 1, s.LastIndexOf('.') - dash - 1);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }).Where(x => !string.IsNullOrEmpty(x)).Concat(new[] { "explorer" }).Distinct().ToArray();

        private readonly object _operationLock = new();

        private readonly Dictionary<string, PerfCounterEntry> _perfCounterBuffer = new();

        private bool _canRetry = true;
        private SkipCurrentLevel _skipLevel = SkipCurrentLevel.None;
        private Thread _worker;

        public BulkUninstallEntry(ApplicationUninstallerEntry uninstallerEntry, bool isSilentPossible,
            UninstallStatus startingStatus)
        {
            CurrentStatus = startingStatus;
            IsSilentPossible = isSilentPossible;
            UninstallerEntry = uninstallerEntry;
        }

        public Exception CurrentError { get; private set; }

        public UninstallStatus CurrentStatus { get; private set; }

        public bool Finished { get; private set; }

        public int Id { get; internal set; }

        public bool IsRunning
        {
            get
            {
                lock (_operationLock)
                    return _worker != null && _worker.IsAlive;
            }
        }

        public bool IsSilentPossible { get; set; }

        public ApplicationUninstallerEntry UninstallerEntry { get; }

        private static void KillProcesses(IEnumerable<Process> processes)
        {
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException)
                {
                }
                catch (Win32Exception)
                {
                }
            }
        }

        public void Reset()
        {
            //bug handle already running
            CurrentError = null;
            CurrentStatus = UninstallStatus.Waiting;
            Finished = false;
        }

        /// <summary>
        ///     Run the uninstaller on a new thread.
        /// </summary>
        internal void RunUninstaller(RunUninstallerOptions options)
        {
            lock (_operationLock)
            {
                if (Finished || IsRunning || CurrentStatus != UninstallStatus.Waiting)
                    return;

                if (UninstallerEntry.IsRegistered && !UninstallerEntry.RegKeyStillExists())
                {
                    CurrentStatus = UninstallStatus.Completed;
                    Finished = true;
                    return;
                }

                if (UninstallerEntry.UninstallerKind == UninstallerType.Msiexec)
                {
                    var uninstallString = IsSilentPossible && UninstallerEntry.QuietUninstallPossible
                        ? UninstallerEntry.QuietUninstallString
                        : UninstallerEntry.UninstallString;

                    // Always reenumerate products in case any were uninstalled
                    if (ApplicationEntryTools.PathPointsToMsiExec(uninstallString) &&
                        MsiTools.MsiEnumProducts().All(g => !g.Equals(UninstallerEntry.BundleProviderKey)))
                    {
                        CurrentStatus = UninstallStatus.Completed;
                        Finished = true;
                        return;
                    }
                }

                CurrentStatus = UninstallStatus.Uninstalling;

                try
                {
                    _worker = new Thread(UninstallThread) { Name = "RunBulkUninstall_Worker", IsBackground = false };
                    _worker.Start(options);
                }
                catch
                {
                    CurrentStatus = UninstallStatus.Failed;
                    Finished = true;
                    throw;
                }
            }
        }

        public void SkipWaiting(bool terminate)
        {
            lock (_operationLock)
            {
                if (Finished)
                    return;

                if (!IsRunning && CurrentStatus == UninstallStatus.Waiting)
                    CurrentStatus = UninstallStatus.Skipped;

                // Do not allow skipping of Msiexec uninstallers because they will hold up the rest of Msiexec uninstallers in the task
                if (CurrentStatus == UninstallStatus.Uninstalling &&
                    UninstallerEntry.UninstallerKind == UninstallerType.Msiexec &&
                    !terminate)
                    return;

                _skipLevel = terminate ? SkipCurrentLevel.Terminate : SkipCurrentLevel.Skip;
            }
        }

        /// <summary>
        /// Try to mark this entry as finished. If it is running and can't be safely skipped, mark it for termination instead.
        /// Do not use unless the entry was uninstalled externally.
        /// </summary>
        public void ForceFinished()
        {
            lock (_operationLock)
            {
                if (Finished)
                    return;

                if (IsRunning)
                {
                    // Do not allow skipping of Msiexec uninstallers because they will hold up the rest of Msiexec uninstallers in the task
                    if (CurrentStatus == UninstallStatus.Uninstalling &&
                        UninstallerEntry.UninstallerKind == UninstallerType.Msiexec)
                    {
                        SkipWaiting(true);
                        return;
                    }
                }

                CurrentStatus = UninstallStatus.Completed;
                Finished = true;
            }
        }

        /// <summary>
        ///     Returns true if uninstaller appears to be stalled. Blocks for 1000ms to gather data.
        /// </summary>
        private bool TestUninstallerForStalls(IEnumerable<string> childProcesses)
        {
            var childProcessNames = childProcesses as IList<string> ?? childProcesses.ToList();

            foreach (var perfCounterEntry in _perfCounterBuffer.ToList())
            {
                if (!childProcessNames.Contains(perfCounterEntry.Key))
                {
                    _perfCounterBuffer.Remove(perfCounterEntry.Key);
                    perfCounterEntry.Value.Dispose();
                }
            }

            foreach (var childProcessName in childProcessNames)
            {
                PerformanceCounter[] perfCounters = null;
                try
                {
                    perfCounters = new[]
                    {
                        new PerformanceCounter("Process", "% Processor Time", childProcessName, true),
                        new PerformanceCounter("Process", "IO Data Bytes/sec", childProcessName, true)
                    };
                    // Important to NextSample now, they will collect data when we sleep
                    _perfCounterBuffer.Add(childProcessName, new PerfCounterEntry(
                        perfCounters, new[] { perfCounters[0].NextSample(), perfCounters[1].NextSample() }));
                }
                catch
                {
                    // Ignore errors caused by counters derping
                    if (perfCounters != null && perfCounters.Length == 2)
                    {
                        perfCounters[0].Dispose();
                        perfCounters[1].Dispose();
                    }
                }
            }

            // Let the counters gather some data
            Thread.Sleep(1100);

            bool? anyWorking = null;

            foreach (var perfCounterEntry in _perfCounterBuffer.ToList())
            {
                try
                {
                    var new0 = perfCounterEntry.Value.Counter[0].NextSample();
                    var new1 = perfCounterEntry.Value.Counter[1].NextSample();
                    var c0 = CounterSample.Calculate(perfCounterEntry.Value.Sample[0], new0);
                    var c1 = CounterSample.Calculate(perfCounterEntry.Value.Sample[1], new1);
                    perfCounterEntry.Value.Sample[0] = new0;
                    perfCounterEntry.Value.Sample[1] = new1;

                    Debug.WriteLine("CPU " + c0 + "%, IO " + c1 + "B");

                    // Check if process seems to be doing anything. Use 1% for CPU and 10KB for I/O
                    if (c0 <= 1 && c1 <= 10240)
                    {
                        anyWorking = false;
                    }
                    else
                    {
                        anyWorking = true;
                        break;
                    }
                }
                catch
                {
                    perfCounterEntry.Value.Dispose();
                    _perfCounterBuffer.Remove(perfCounterEntry.Key);
                }
            }

            // Only return true if we had at least one process to test and it tested negatively
            return anyWorking.HasValue && !anyWorking.Value;
        }

        private void UninstallThread(object parameters)
        {
            var options = parameters as RunUninstallerOptions;
            Debug.Assert(options != null, "options != null");

            Exception error = null;
            var retry = false;
            try
            {
                var processSnapshot = Process.GetProcesses().Select(x => x.Id).ToArray();

                using (var uninstaller = UninstallerEntry.RunUninstaller(options.PreferQuiet, options.Simulate, _canRetry))
                {
                    // Can be null during simulation
                    if (uninstaller == null) return;

                    if (options.PreferQuiet && UninstallerEntry.QuietUninstallPossible)
                    {
                        try
                        {
                            uninstaller.PriorityClass = ProcessPriorityClass.BelowNormal;
                        }
                        catch
                        {
                            // Don't care if setting this fails
                        }
                    }

                    var checkCounters = options.PreferQuiet && options.AutoKillStuckQuiet &&
                                        UninstallerEntry.QuietUninstallPossible;

                    var watchedProcesses = new List<Process> { uninstaller };
                    int[] previousWatchedProcessIds = { };

                    var idleCounter = 0;

                    while (true)
                    {
                        if (_skipLevel == SkipCurrentLevel.Skip)
                            break;

                        foreach (var watchedProcess in watchedProcesses.ToList())
                            watchedProcesses.AddRange(watchedProcess.GetChildProcesses());

                        if (UninstallerEntry.UninstallerKind == UninstallerType.Msiexec)
                        {
                            foreach (var watchedProcess in Process.GetProcessesByName("msiexec"))
                                watchedProcesses.AddRange(watchedProcess.GetChildProcesses());
                        }

                        watchedProcesses = CleanupDeadProcesses(watchedProcesses, processSnapshot).ToList();

                        // Check if we are done, or if there are some proceses left that we missed.
                        // We are done when the entry process and all of its spawns exit.
                        if (watchedProcesses.Count == 0)
                        {
                            if (string.IsNullOrEmpty(UninstallerEntry.InstallLocation))
                                break;

                            FindAndAddProcessesToWatch(watchedProcesses, processSnapshot);

                            if (watchedProcesses.Count == 0)
                                break;
                        }

                        // Only try to automate first try. If it fails, don't try to automate 
                        // the rerun in case user or app itself can resolve the issue.
                        if (IsSilentPossible && UninstallToolsGlobalConfig.UseQuietUninstallDaemon && _canRetry)
                        {
                            // There is no point in trying to automatize command line interface programs, or our own helpers
                            if (!UninstallerEntry.QuietUninstallerIsCLI() && !UninstallerEntry.QuietUninstallString
                                .Contains(UninstallToolsGlobalConfig.AppLocation, StringComparison.OrdinalIgnoreCase))
                            {
                                var processIds = SafeGetProcessIds(watchedProcesses).ToArray();

                                options.Owner.SendProcessesToWatchToDeamon(processIds.Except(previousWatchedProcessIds));

                                previousWatchedProcessIds = processIds;
                            }
                        }

                        // Check for deadlocks during silent uninstall. Prevents the task from getting stuck 
                        // idefinitely on stuck uninstallers and unrelated processes spawned by uninstallers.
                        if (checkCounters)
                        {
                            var processNames = SafeGetProcessNames(watchedProcesses);

                            if (TestUninstallerForStalls(processNames))
                                idleCounter++;
                            else
                                idleCounter = 0;

                            // Kill the uninstaller (and children) if they were idle/stalled for too long
                            if (idleCounter > 30)
                            {
                                KillProcesses(watchedProcesses);
                                throw new IOException(Localisation.UninstallError_UninstallerTimedOut);
                            }
                        }
                        else Thread.Sleep(1000);

                        // Kill the uninstaller (and children) if user told us to or if it was idle for too long
                        if (_skipLevel == SkipCurrentLevel.Terminate)
                        {
                            if (UninstallerEntry.UninstallerKind == UninstallerType.Msiexec)
                                watchedProcesses.AddRange(Process.GetProcessesByName("Msiexec"));

                            KillProcesses(watchedProcesses);
                            break;
                        }
                    }

                    if (_skipLevel == SkipCurrentLevel.None)
                    {
                        var exitVar = uninstaller.ExitCode;
                        if (exitVar != 0)
                        {
                            if (UninstallerEntry.UninstallerKind == UninstallerType.Msiexec &&
                                exitVar == 1602)
                            {
                                // 1602 ERROR_INSTALL_USEREXIT - The user has cancelled the installation.
                                _skipLevel = SkipCurrentLevel.Skip;
                            }
                            else if (UninstallerEntry.UninstallerKind == UninstallerType.Nsis &&
                                     (exitVar == 1 || exitVar == 2))
                            {
                                // 1 - Installation aborted by user (cancel button)
                                // 2 - Installation aborted by script (often after user clicks cancel)
                                _skipLevel = SkipCurrentLevel.Skip;
                            }
                            else if (UninstallerEntry.UninstallerKind == UninstallerType.Nsis &&
                                     exitVar == 1627)
                            {
                                // Nsis OK return code
                            }
                            else if (UninstallerEntry.UninstallerKind == UninstallerType.SimpleDelete &&
                                     exitVar == 1)
                            {
                                // 1 - Installation aborted by user (cancel button)
                                _skipLevel = SkipCurrentLevel.Skip;
                            }
                            else if (exitVar == -1073741510)
                            {
                                /* 3221225786 / 0xC000013A / -1073741510 
                                    The application terminated as a result of a CTRL+C. 
                                    Indicates that the application has been terminated either by user's 
                                    keyboard input CTRL+C or CTRL+Break or closing command prompt window. */
                                _skipLevel = SkipCurrentLevel.Terminate;
                            }
                            else
                            {
                                switch (exitVar)
                                {
                                    case 2:
                                        throw new Exception("The system cannot find the file specified. Indicates that the file can not be found in specified location.");
                                    case 3:
                                        throw new Exception("The system cannot find the path specified. Indicates that the specified path can not be found.");
                                    case 5:
                                        throw new Exception("Access is denied. Indicates that user has no access right to specified resource.");
                                    case 9009:
                                        throw new Exception("!(Program is recognized as) an internal or external command, operable program or batch file.");
                                    case -2147024846:
                                        throw new Exception("0x80070032 - This app is part of Windows and cannot be uninstalled on a per-user basis.");

                                    default:
                                        if (options.RetryFailedQuiet || (UninstallerEntry.UninstallerKind == UninstallerType.Nsis && !options.PreferQuiet))
                                            retry = true;
                                        throw new IOException(Localisation.UninstallError_UninstallerReturnedCode + exitVar);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
                Trace.WriteLine(@$"Exception when uninstalling {UninstallerEntry.DisplayName}: {ex}");
            }
            finally
            {
                try
                {
                    _perfCounterBuffer.ForEach(x => x.Value.Dispose());
                }
                catch
                {
                    // Ignore any errors to make sure rest of this code runs
                }
                _perfCounterBuffer.Clear();

                // Take care of the aftermath
                if (_skipLevel != SkipCurrentLevel.None)
                {
                    _skipLevel = SkipCurrentLevel.None;

                    CurrentStatus = UninstallStatus.Skipped;
                    CurrentError = new OperationCanceledException(Localisation.ManagerError_Skipped);
                }
                else if (error != null)
                {
                    //Localisation.ManagerError_PrematureWorkerStop is unused
                    CurrentStatus = UninstallStatus.Failed;
                    CurrentError = error;
                }
                else
                {
                    CurrentStatus = UninstallStatus.Completed;
                }

                if (retry && _canRetry)
                {
                    CurrentStatus = UninstallStatus.Waiting;
                    _canRetry = false;
                }
                else
                {
                    Finished = true;
                }
            }
        }

        private static IEnumerable<string> SafeGetProcessNames(IEnumerable<Process> processes)
        {
            return processes.Select(x =>
            {
                try
                {
                    return x.ProcessName;
                }
                catch
                {
                    // Ignore errors caused by processes that exited
                    return null;
                }
            }).Where(x => !string.IsNullOrEmpty(x));
        }

        private static IEnumerable<int> SafeGetProcessIds(IEnumerable<Process> processes)
        {
            return processes.Select(x =>
            {
                try
                {
                    x.Refresh();
                    if (x.MainWindowHandle == IntPtr.Zero)
                        return -1;

                    Debug.WriteLine("Process ID " + x.Id + " is running: " + !Process.GetProcessById(x.Id).HasExited);
                    return x.Id;
                }
                catch
                {
                    // Ignore errors caused by processes that exited
                    return -1;
                }
            }).Where(x => x >= 0);
        }

        private void FindAndAddProcessesToWatch(ICollection<Process> watchedProcesses, int[] runningProcessIds)
        {
            var candidates = Process.GetProcesses().Where(x => !runningProcessIds.Contains(x.Id));
            foreach (var process in candidates)
            {
                try
                {
                    if (process.MainModule!.FileName!.Contains(
                            UninstallerEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase) ||
                        process.GetCommandLine().Contains(
                            UninstallerEntry.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                    {
                        watchedProcesses.Add(process);
                    }
                }
                catch
                {
                    // Ignore permission and access errors
                }
            }
        }

        /// <summary>
        /// Remove duplicate, dead, and blacklisted processes
        /// </summary>
        private static IEnumerable<Process> CleanupDeadProcesses(IEnumerable<Process> watchedProcesses, int[] runningProcessIds)
        {
            return watchedProcesses.DistinctBy(x => x.Id).Where(p =>
            {
                try
                {
                    if (p.HasExited)
                        return false;

                    var pName = p.ProcessName;
                    if (NamesOfIgnoredProcesses.Any(n =>
                        pName.Equals(n, StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                }
                catch (Win32Exception)
                {
                    return false;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }

                return !runningProcessIds.Contains(p.Id);
            });
        }

        private sealed class PerfCounterEntry : IDisposable
        {
            public PerfCounterEntry(PerformanceCounter[] counter, CounterSample[] sample)
            {
                Counter = counter;
                Sample = sample;
            }

            public PerformanceCounter[] Counter { get; }

            public CounterSample[] Sample { get; }

            public void Dispose()
            {
                foreach (var performanceCounter in Counter)
                {
                    performanceCounter?.Dispose();
                }
            }
        }

        internal sealed class RunUninstallerOptions
        {
            public RunUninstallerOptions(bool autoKillStuckQuiet, bool retryFailedQuiet, bool preferQuiet, bool simulate, BulkUninstallTask owner)
            {
                AutoKillStuckQuiet = autoKillStuckQuiet;
                RetryFailedQuiet = retryFailedQuiet;
                PreferQuiet = preferQuiet;
                Simulate = simulate;
                Owner = owner;
            }

            public bool AutoKillStuckQuiet { get; }

            public bool PreferQuiet { get; }

            public bool RetryFailedQuiet { get; }

            public bool Simulate { get; }

            public BulkUninstallTask Owner { get; }
        }

        internal enum SkipCurrentLevel
        {
            None = 0,
            Terminate,
            Skip
        }

        public void Pause()
        {
            lock (_operationLock)
            {
                if (CurrentStatus == UninstallStatus.Waiting)
                    CurrentStatus = UninstallStatus.Paused;
            }
        }

        public void Resume()
        {
            lock (_operationLock)
            {
                if (CurrentStatus == UninstallStatus.Paused)
                    CurrentStatus = UninstallStatus.Waiting;
            }
        }
    }
}