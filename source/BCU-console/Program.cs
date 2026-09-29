/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UninstallTools;
using UninstallTools.Factory;
using UninstallTools.Junk;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Lists;
using UninstallTools.Uninstaller;

namespace BCU_console
{
    internal static class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine(@"BCU-console [help | /?] - Show help (this screen)

BCU-console uninstall [drive:][path]filename [/Q] [/U] [/V] [/N] [/J=<Level>] - Uninstall applications.
 [drive:][path]	– Specifies drive and directory of the uninstall list.
 filename       – Specifies filename of the .bcul uninstall list that contains information about
                  what applications to uninstall.

BCU-console export [drive:][path]filename [/Q] [/U] [/V] [/F=<Format>] - Export installed application data to a file.
 [drive:][path]	– Specifies drive and directory to where the export should be saved.
 filename       – Specifies filename of the file to save the exported application information to
                  (xml by default, json when /F=json is passed).

BCU-console list [/Q] [/U] [/V] [/F=<Format>] - Display a list of installed applications.

Switches:
 /Q             - Use quiet uninstallers wherever possible (by default only use loud).
 /U             - Unattended mode (do not ask user for confirmation). WARNING: ONLY USE AFTER
                  THOROUGH TESTING. UNINSTALL LISTS SHOULD BE AS SPECIFIC AS POSSIBLE TO AVOID
                  FALSE POSITIVES. THERE ARE NO WARRANTIES, USE WITH CAUTION.
 /J=<Level>     - Attempt to clean up leftover ""junk"" (Registry entries and files/folders) after
                  uninstall. If no level is passed then defaults to ""VeryGood"". ***WARNING***: USE
                  EXTREME CAUTION WHEN CHOOSING ANY LEVEL BELOW VeryGood. THERE ARE NO WARRANTIES.
                  Valid levels are: VeryGood, Good, Questionable, Bad, Unknown
 /V             - Verbose logging mode (show more information about what is currently happening).
 /N             - Dry run (preview) mode. Runs the uninstall task in simulation mode to show which
                  applications would be uninstalled and, when combined with /J, which junk items
                  would be removed, without uninstalling or deleting anything. ""--dry-run"" is
                  also accepted.
 /F=<Format>    - Output format, valid formats are ""json"" and ""xml"" (the default is a plaintext
                  table for list and an xml file for export). ""--format=<Format>"" is also accepted.
                  When this switch is used all progress messages are written to standard error, so
                  that standard output of ""list"" contains only the serialized document.

Return codes:
 0	- The operation completed successfully.
 1	- Invalid arguments, or a dry run matched no applications.
 1223	- The operation was canceled by the user.");
        }

        /// <summary>
        /// Holds the console window open after printing help, but only when doing so is both safe and useful:
        /// skipped when input is redirected (a pipe, file, Task Scheduler or CI - where ReadKey would throw),
        /// and only used when this process owns its console (a double-click) so it isn't shown to shell users.
        /// </summary>
        private static void WaitForKeyIfStandalone()
        {
            if (Console.IsInputRedirected || !LaunchedStandalone())
                return;

            Console.WriteLine();
            Console.WriteLine(@"Press any key to exit...");
            Console.ReadKey(true);
        }

        private static bool LaunchedStandalone()
        {
            // When double-clicked the process gets its own console, so it is the only one attached (count 1).
            // When launched from a shell the console is shared, so more than one process is attached.
            try
            {
                var processIds = new uint[2];
                var count = GetConsoleProcessList(processIds, (uint)processIds.Length);
                // count 0 means the call failed (e.g. no console); don't pause in that case.
                return count == 1;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleProcessList(uint[] lpdwProcessList, uint dwProcessCount);

        private static TextWriter _dataOutput;
      
        private static int Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var isMachineReadable = GetFormatArgument(args) != null;

            try { Console.OutputEncoding = isMachineReadable ? new UTF8Encoding(false) : Encoding.Unicode; }
            catch (SystemException) { }

            _dataOutput = Console.Out;
            if (isMachineReadable)
                Console.SetOut(Console.Error);

            var info = Assembly.GetExecutingAssembly();
            Console.WriteLine(info.FullName);
            Console.WriteLine();

            if (args.Length == 0 || args.Any(x =>
                    x.Equals("help", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("/?", StringComparison.OrdinalIgnoreCase)))
            {
                ShowHelp();
                WaitForKeyIfStandalone();
                return 0;
            }

            try
            {
                switch (args[0].Normalize().ToLowerInvariant())
                {
                    case "uninstall":
                        return ProcessUninstallCommand(args.Skip(1).ToArray());
                    case "list":
                        return ProcessListCommand(args.Skip(1).ToArray());
                    case "export":
                        return ProcessExportCommand(args.Skip(1).ToArray());
                    default:
                        Console.WriteLine($"Invalid command \"{args[0]}\"\n");
                        ShowHelp();
                        return 1;
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(@"Encountered an unexpected error!");
                Console.WriteLine(ex);
                return 13;
            }
        }

        private static int ProcessListCommand(string[] args)
        {
            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase));
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase));

            var format = GetFormatArgument(args);
            if (format != null && !IsSupportedFormat(format))
                return ShowInvalidSyntaxError($"Unsupported format \"{format}\", valid formats are \"json\" and \"xml\"");

            var apps = QueryApps(isQuiet, isUnattended, isVerbose);

            if (format != null)
            {
                _dataOutput.WriteLine(format.Equals("json", StringComparison.OrdinalIgnoreCase)
                    ? ApplicationEntrySerializer.SerializeApplicationEntriesToJson(apps)
                    : ApplicationEntrySerializer.SerializeApplicationEntriesToXml(apps));
                return 0;
            }

            var serializer = new ApplicationEntrySerializer(apps);

            Console.WriteLine($@"{"Display Name",-40}  {"Version",-20}  {"Source",-40}");
            var sb = new StringBuilder(82);
            for (var i = 0; i < 102; i++)
            {
                sb.Append('-');
            }
            Console.WriteLine(sb.ToString());

            foreach (var entry in serializer.Items)
            {
                var displayName = entry.DisplayNameTrimmed;
                if (displayName.Length > 40)
                {
                    displayName = displayName.Substring(0, 40);
                }

                var version = entry.DisplayVersion ?? string.Empty;
                if (version.Length > 20)
                {
                    version = version.Substring(0, 20);
                }

                var source = entry.AboutUrl ?? string.Empty;
                if (source.Length > 40)
                {
                    source = source.Substring(0, 40);
                }

                Console.WriteLine($@"{displayName,-40}  {version,-20}  {source,-40}");
            }
            return 0;
        }

        private static int ProcessExportCommand(string[] args)
        {
            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase));
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase));

            var format = GetFormatArgument(args);
            if (format != null && !IsSupportedFormat(format))
                return ShowInvalidSyntaxError($"Unsupported format \"{format}\", valid formats are \"json\" and \"xml\"");

            args = args.Where(x => !x.StartsWith("/", StringComparison.Ordinal) &&
                                   !x.StartsWith("--", StringComparison.Ordinal)).ToArray();
            if (args.Length != 1)
                return ShowInvalidSyntaxError("Missing export filename or invalid arguments");

            Console.WriteLine($@"Starting export to {args[0]}");
            var apps = QueryApps(isQuiet, isUnattended, isVerbose);

            Console.WriteLine(@"Exporting data...");
            if (format != null && format.Equals("json", StringComparison.OrdinalIgnoreCase))
                ApplicationEntrySerializer.SerializeApplicationEntriesToJson(args[0], apps);
            else
                ApplicationEntrySerializer.SerializeApplicationEntries(args[0], apps);
            Console.WriteLine(@"Success!");
            return 0;
        }

        private static int ProcessUninstallCommand(string[] args)
        {
            if (args.Length < 1)
                return ShowInvalidSyntaxError("Missing path argument");

            if (!File.Exists(args[0]))
                return ShowInvalidSyntaxError("Invalid path or missing list file");

            UninstallList list;
            try
            {
                list = UninstallList.ReadFromFile(args[0]);
                if (list == null || list.Filters.Count == 0)
                    throw new IOException("List is empty");
            }
            catch (SystemException ex)
            {
                return ShowInvalidSyntaxError(
                    $"Invalid or damaged uninstall list file - \"{args[0]}\"\nError: {ex.Message}\n");
            }

            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase));
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase));
            var isDryRun = args.Any(x => x.Equals("/N", StringComparison.OrdinalIgnoreCase) ||
                                         x.Equals("--dry-run", StringComparison.OrdinalIgnoreCase));

            int junkArgumentIndex = Array.FindIndex(args, a => a.Equals("/J", StringComparison.OrdinalIgnoreCase));
            string junkArg = args.Where(a => a.Equals("/J", StringComparison.OrdinalIgnoreCase) || a.StartsWith("/J=", StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ?? string.Empty;
            ConfidenceLevel? junkConfidenceLevel = null;
            if(junkArg.IsNotEmpty()) {
                string[] junkSplit = junkArg.Split('=', 2);
                junkConfidenceLevel = ConfidenceLevel.VeryGood;
                if (junkSplit.Length == 2) {
                    if (!Enum.TryParse(junkSplit[1], out ConfidenceLevel parsedJunkConfidenceLevel)) {
                        Console.WriteLine($"An invalid junk confidence level was passed: {junkSplit[1]}");
                        ShowHelp();
                        return 1;
                    }
                    junkConfidenceLevel = parsedJunkConfidenceLevel;
                }
            }
            if (isUnattended && !isDryRun)
                Console.WriteLine(@"WARNING: Running in unattended mode. To abort press Ctrl+C or close the window.");

            return RunUninstall(list, isQuiet, isUnattended, isVerbose, isDryRun, junkConfidenceLevel);
        }

        private static int RunUninstall(UninstallList list, bool isQuiet, bool isUnattended, bool isVerbose, bool isDryRun, ConfidenceLevel? junkConfidenceLevel = null)
        {
            if (isDryRun)
                Console.WriteLine(@"DRY RUN: Nothing will be uninstalled and no files or registry entries will be deleted.");

            Console.WriteLine(@"Starting bulk uninstall...");
            var allApps = QueryApps(isQuiet, isUnattended, isVerbose);

            var apps = allApps.Where(a => list.TestEntry(a) == true).OrderBy(x => x.DisplayName).ToList();

            if (apps.Count == 0)
            {
                Console.WriteLine(@"No applications matched the supplied uninstall list.");
                return isDryRun ? 1 : 0;
            }

            Console.WriteLine(@"{0} application(s) were matched by the list: {1}", apps.Count,
                          string.Join("; ", apps.Select(x => x.DisplayName)));

            if (!isDryRun)
                Console.WriteLine(@"These applications will now be uninstalled PERMANENTLY.");

            if (!isUnattended && !isDryRun)
            {
                Console.WriteLine(@"Do you want to continue? [Y]es/[N]o");
                if (Console.ReadKey(true).Key != ConsoleKey.Y)
                    return CancelledByUser();
            }

            Console.WriteLine(@"Setting-up for the uninstall task...");
            var targets = apps.Select(a => new BulkUninstallEntry(a, a.QuietUninstallPossible, UninstallStatus.Waiting))
                .ToList();
            var task = UninstallManager.CreateBulkUninstallTask(targets,
                new BulkUninstallConfiguration(false, isQuiet, isDryRun, true, true));
            var isDone = false;
            task.OnStatusChanged += (sender, args) =>
            {
                ClearCurrentConsoleLine();

                var running = task.AllUninstallersList.Count(x => x.IsRunning);
                var waiting = task.AllUninstallersList.Count(x => x.CurrentStatus == UninstallStatus.Waiting);
                var finished = task.AllUninstallersList.Count(x => x.Finished);
                var errors = task.AllUninstallersList.Count(x => x.CurrentStatus == UninstallStatus.Failed ||
                    x.CurrentStatus == UninstallStatus.Invalid);
                Console.Write("Running: {0}, Waiting: {1}, Finished: {2}, Failed: {3}",
                    running, waiting, finished, errors);

                if (task.Finished)
                {
                    isDone = true;
                    Console.WriteLine();
                    Console.WriteLine(@"Uninstall task Finished.");

                    foreach (var error in task.AllUninstallersList.Where(x =>
                        x.CurrentStatus != UninstallStatus.Completed && x.CurrentError != null))
                        Console.WriteLine($@"Error: {error.UninstallerEntry.DisplayName} - {error.CurrentError.Message}");
                }
            };
            task.Start();

            while (!isDone)
                Thread.Sleep(250);

            if (junkConfidenceLevel is not null) {
                Console.WriteLine($"Starting junk cleanup with a minimum confidence level of {junkConfidenceLevel}");
                if (isDryRun)
                    Console.WriteLine(@"Note: The applications are still installed, so results may differ from the actual cleanup performed after they are uninstalled.");
                var remainingApps = isDryRun ? allApps : QueryApps(isQuiet, isUnattended, isVerbose);

                List<IJunkResult> remainingJunk = JunkManager.FindJunk(apps, remainingApps, _ => { })
                    .Where(j => j.Confidence.GetConfidence() >= junkConfidenceLevel)
                    .ToList();

                if (!remainingJunk.Any()) {
                    Console.WriteLine($"No remaining junk found for any target applications.");
                    if (isDryRun)
                        Console.WriteLine(@"Dry run finished. No changes were made.");
                    return 0;
                }

                Console.WriteLine(isDryRun
                    ? "The following junk items would be permanently deleted:"
                    : "The following junk items will be permanently deleted:");
                remainingJunk.ForEach(Console.WriteLine);
                if (!isUnattended && !isDryRun) {
                    Console.WriteLine(@"Do you want to continue? [Y]es/[N]o");
                    if (Console.ReadKey(true).Key != ConsoleKey.Y)
                        return CancelledByUser();
                }

                foreach (ApplicationUninstallerEntry entry in apps) {
                    // ApplicationUninstallerEntry doesn't currently implement an equality operator so ToLongString() will do as an object "hash".
                    List<IJunkResult> appJunk = remainingJunk.Where(j => j.Application.ToLongString().Equals(entry.ToLongString())).ToList();
                    Console.WriteLine($"{entry.DisplayName} Junk - {appJunk.Count} Entries Found");
                    if (!isDryRun)
                        appJunk.ForEach(j => j.Delete());
                }
            }

            if (isDryRun)
                Console.WriteLine(@"Dry run finished. No changes were made.");
            return 0;
        }

        public static void ClearCurrentConsoleLine()
        {
            if (Console.IsOutputRedirected)
            {
                Console.WriteLine();
                return;
            }

            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static int CancelledByUser()
        {
            Console.WriteLine(@"Operation cancelled by the user.");
            return 1223;
        }

        private static IList<ApplicationUninstallerEntry> QueryApps(bool isQuiet, bool isUnattended, bool isVerbose)
        {
            ConfigureUninstallTools();

            Console.WriteLine(@"Looking for applications...");
            string previousMain = null;

            IList<ApplicationUninstallerEntry> result;
            if (isQuiet || isUnattended)
            {
                result = ApplicationUninstallerFactory.GetUninstallerEntries(_ => { });
            }
            else
            {
                result = ApplicationUninstallerFactory.GetUninstallerEntries(report =>
                            {
                                if (previousMain != report.Message)
                                {
                                    previousMain = report.Message;
                                    Console.WriteLine(report.Message);
                                }
                                if (isVerbose)
                                {
                                    if (!string.IsNullOrEmpty(report.Inner?.Message))
                                    {
                                        Console.Write("-> ");
                                        Console.WriteLine(report.Inner.Message);
                                    }
                                }
                            });
            }

            Console.WriteLine("Found {0} applications.", result.Count);
            return result;
        }

        private static void ConfigureUninstallTools()
        {
            UninstallToolsGlobalConfig.ScanWinUpdates = false;
            UninstallToolsGlobalConfig.QuietAutomatizationKillStuck = true;
            UninstallToolsGlobalConfig.QuietAutomatization = true;
            UninstallToolsGlobalConfig.UseQuietUninstallDaemon = true;
            UninstallToolsGlobalConfig.AutoDetectCustomProgramFiles = true;
            UninstallToolsGlobalConfig.EnableAppInfoCache = false;
        }

        private static int ShowInvalidSyntaxError(string message)
        {
            Console.WriteLine("Invalid command syntax. " + message);
            return 87;
        }

        private static string GetFormatArgument(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("/F=", StringComparison.OrdinalIgnoreCase))
                    return arg.Substring("/F=".Length);
                if (arg.StartsWith("--format=", StringComparison.OrdinalIgnoreCase))
                    return arg.Substring("--format=".Length);
            }
            return null;
        }

        private static bool IsSupportedFormat(string format)
        {
            return format.Equals("json", StringComparison.OrdinalIgnoreCase) ||
                   format.Equals("xml", StringComparison.OrdinalIgnoreCase);
        }
    }
}