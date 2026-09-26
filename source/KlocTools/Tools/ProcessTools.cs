/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using Klocman.Extensions;
using Klocman.Properties;

namespace Klocman.Tools
{
    public static class ProcessTools
    {
        public static bool Is64BitProcess => IntPtr.Size == 8;

        /// <summary>
        ///     Kill all of process's children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        public static void KillChildProcesses(int pid)
        {
            foreach (var id in GetChildProcesses(pid))
            {
                KillProcessAndChildProcesses(id);
            }
        }

        /// <summary>
        ///     Get IDs of all child processes
        /// </summary>
        public static IEnumerable<int> GetChildProcesses(int pid)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid))
                {
                    var moc = searcher.Get();
                    var childProcesses = moc.Cast<ManagementObject>().Select(mo => Convert.ToInt32(mo["ProcessID"])).ToList();
                    return childProcesses;
                }
            }
            catch
            {
                Process processById;
                try
                {
                    processById = Process.GetProcessById(pid);
                }
                catch (Exception a)
                {
                    Console.WriteLine(a);
                    return Enumerable.Empty<int>();
                }
                return processById.GetChildProcesses().Select(x => x.Id);
            }
        }

        /// <summary>
        ///     Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        public static void KillProcessAndChildProcesses(int pid)
        {
            KillChildProcesses(pid);

            try
            {
                var proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        /// <exception cref="ArgumentException">processName</exception>
        /// <exception cref="InvalidOperationException">
        ///     There are problems accessing the performance counter API's used to get
        ///     process information. This exception is specific to Windows NT, Windows 2000, and Windows XP.
        /// </exception>
        public static bool SafeKillProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentException(@"Process name can't be null or empty", nameof(processName));

            foreach (var p in Process.GetProcessesByName(processName))
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(); // possibly with a timeout
                    return true;
                }
                catch (Win32Exception)
                {
                    // process was terminating or can't be terminated - deal with it
                    return false;
                }
                catch (InvalidOperationException)
                {
                    // process has already exited - might be able to let this one go
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private static readonly char[] SeparateArgsFromCommandInvalidChars =
            Path.GetInvalidFileNameChars().Concat(new[] { ',', ';' }).ToArray();

        //static readonly char[] pathFilterChars = StringTools.InvalidPathChars.Except(new char[] { '"' }).ToArray();
        /// <summary>
        ///     Attempts to separate filename (or filename with path) from the supplied arguments.
        /// </summary>
        /// <param name="fullCommand"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The value of 'fullCommand' cannot be null. </exception>
        /// <exception cref="ArgumentException">fullCommand can't be empty</exception>
        /// <exception cref="FormatException">Filename is in invalid format</exception>
        public static ProcessStartCommand SeparateArgsFromCommand(string fullCommand)
        {
            if (fullCommand == null)
                throw new ArgumentNullException(nameof(fullCommand));

            // Get rid of whitespaces
            fullCommand = fullCommand.Trim();

            if (string.IsNullOrEmpty(fullCommand))
                throw new ArgumentException(Localisation.Error_SeparateArgsFromCommand_Empty, nameof(fullCommand));

            var firstArgumentSpace = fullCommand.IndexOf(' ');
            var firstDot = fullCommand.IndexOf('.');
            if (firstDot < 0)
                return SeparateNonDottedCommand(fullCommand);

            // Check if the path is in format: ExecutableName C:\Argname.exe
            {
                var pathRoot = fullCommand.IndexOf(":\\", StringComparison.InvariantCulture);
                var firstSpace = fullCommand.IndexOf(' ');
                if (firstSpace >= 0 && firstSpace < pathRoot)
                {
                    var filenameBreaker = fullCommand.IndexOfAny(SeparateArgsFromCommandInvalidChars, 0, pathRoot - 1);
                    if (filenameBreaker < 0)
                    {
                        var slashIndex = fullCommand.IndexOf('\\');
                        if (slashIndex >= 0 && slashIndex > pathRoot)
                        {
                            var rootSpace = fullCommand.LastIndexOf(' ', pathRoot);
                            return new ProcessStartCommand(fullCommand.Substring(0, rootSpace).TrimEnd(),
                                fullCommand.Substring(rootSpace));
                        }
                    }
                }
            }

            // Check if the path is contained inside of quotation marks.
            // Assume that the quotation mark must come before the dot. Otherwise, it is likely that the arguments use quotations.
            var pathEnd = fullCommand.IndexOf('"', 0, firstDot);
            if (pathEnd >= 0)
            {
                // If yes, find the closing quotation mark and set its index as path end
                pathEnd = fullCommand.IndexOf('"', pathEnd + 1);

                if (pathEnd < 0)
                {
                    // If no ending quote has been found, explode gracefully.
                    throw new FormatException(Localisation.Error_SeparateArgsFromCommand_MissingQuotationMark);
                }
                pathEnd += 1; //?
            }

            // If quotation marks were missing, check for any invalid characters after last dot
            // in case of eg: c:\test.dir thing\filename.exe?0 used to get icons
            if (pathEnd < 0)
            {
                var endIndex = 0;
                while (true)
                {
                    var dot = fullCommand.IndexOf('.', endIndex);
                    if (dot < 0)
                        break;

                    var filenameBreaker = fullCommand.IndexOfAny(SeparateArgsFromCommandInvalidChars, dot);
                    var space = fullCommand.IndexOf(' ', dot);
                    if (filenameBreaker < 0)
                    {
                        if (space < 0) break;
                        filenameBreaker = space;
                    }

                    var dash = fullCommand.IndexOf('\\', dot);
                    if (filenameBreaker < dash || dash < 0)
                    {
                        pathEnd = space < 0 ? filenameBreaker : space;
                        break;
                    }

                    var nextBreaker = fullCommand.IndexOfAny(SeparateArgsFromCommandInvalidChars, filenameBreaker + 1);
                    var nextDash = fullCommand.IndexOf('\\', filenameBreaker + 1);

                    if (nextBreaker > 0 && (nextDash < 0 || nextBreaker < nextDash))
                    {
                        var nextDot = fullCommand.IndexOf('.', filenameBreaker + 1);
                        if (nextDot < 0 || nextBreaker < nextDot)
                        {
                            pathEnd = space < 0 ? filenameBreaker : space;
                            break;
                        }
                    }

                    endIndex = dash;
                }

                // old
                //pathEnd = fullCommand.IndexOfAny(" ,:;?-=", fullCommand.LastIndexOf('.'));
            }

            return SeparateBareExecutableIfNeeded(fullCommand, SeparateCommand(fullCommand, pathEnd), firstArgumentSpace);
        }

        private static ProcessStartCommand SeparateBareExecutableIfNeeded(string fullCommand, ProcessStartCommand parsedCommand, int firstArgumentSpace)
        {
            if (firstArgumentSpace <= 0 || fullCommand[0] == '"' || !string.IsNullOrEmpty(parsedCommand.Arguments) ||
                WindowsTools.IsExectuable(parsedCommand.FileName, false))
                return parsedCommand;

            var executableName = fullCommand.Substring(0, firstArgumentSpace);
            if (executableName.Contains('\\') || executableName.Contains('/') || executableName.Contains(':'))
                return parsedCommand;

            return new ProcessStartCommand(executableName, fullCommand.Substring(firstArgumentSpace + 1));
        }

        private static ProcessStartCommand SeparateCommand(string fullCommand, int splitIndex)
        {
            // Begin extracting filename and arguments
            string filename;
            var args = string.Empty;

            if (splitIndex < 0 || splitIndex >= fullCommand.Length)
            {
                // Looks like there were no arguments, assume whole command is a filename
                filename = fullCommand;
            }
            else
            {
                // pathEnd shows the end of the filename (and start of the arguments)
                filename = fullCommand.Substring(0, splitIndex).TrimEnd();
                args = fullCommand.Substring(splitIndex).TrimStart();
            }

            filename = filename.Trim('"'); // Get rid of the quotation marks
            return new ProcessStartCommand(filename, args);
        }

        private static ProcessStartCommand SeparateNonDottedCommand(string fullCommand)
        {
            // Look for the first root of a path
            var pathRoot = fullCommand.IndexOf(":\\", StringComparison.InvariantCulture);
            var pathRootEnd = pathRoot < 0 ? 0 : pathRoot + 2;

            var breakChars = SeparateArgsFromCommandInvalidChars.Except(new[] { '\\' }).ToArray();

            // Check if there are any invalid path chars before the start we found. If yes, our path is most likely an argument.
            if (pathRootEnd > 0 && fullCommand.IndexOfAny(breakChars, 0, pathRootEnd - 2) >= 0)
                pathRootEnd = 0;
            var breakIndex = fullCommand.IndexOfAny(breakChars, pathRootEnd);

            // If there are no invalid path chars, it's probably just a naked filename or directory path.
            if (breakIndex < 0)
                return new ProcessStartCommand(fullCommand.Trim('"'));

            // The invalid char has to have at least 1 space before it to count as an argument. Otherwise the input is likely garbage.
            if (breakIndex > 0 && fullCommand[breakIndex - 1] == ' ')
                return new ProcessStartCommand(fullCommand.Substring(0, breakIndex - 1).TrimEnd(),
                    fullCommand.Substring(breakIndex));

            throw new FormatException(Localisation.Error_SeparateArgsFromCommand_NoDot + "\n" + fullCommand);
        }

        /// <summary>
        ///     Change default culture info for new threads
        /// </summary>
        /// <param name="culture"></param>
        public static void SetDefaultCulture(CultureInfo culture)
        {
            var type = typeof(CultureInfo);

            if (Environment.Version.Major < 4)
            {
                // Fields used before .Net 4.0
                try
                {
                    type.InvokeMember("m_userDefaultCulture",
                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        culture,
                        new object[] { culture });

                    type.InvokeMember("m_userDefaultUICulture",
                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        culture,
                        new object[] { culture });

                    return;
                }
                catch
                {
                    //Ignore failure, try next
                }
            }

            try
            {
                type.InvokeMember("s_userDefaultCulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] { culture });

                type.InvokeMember("s_userDefaultUICulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] { culture });
            }
            catch
            {
                //Ignore failure
            }
        }

        /// <summary>
        ///     Bring to foreground main window of all processes with name of the executing process
        /// </summary>
        public static void ShowMainWindow()
        {
            ShowMainWindow(Process.GetCurrentProcess().ProcessName);
        }

        /// <summary>
        ///     Bring to foreground main window of all processes with supplied name
        /// </summary>
        /// <param name="processName">Name of the processes to bring to foreground</param>
        public static void ShowMainWindow(string processName)
        {
            foreach (var p in Process.GetProcessesByName(processName))
            {
                ShowWindow(p.MainWindowHandle, 1); //SW_SHOWNORMAL = 1
                SetForegroundWindow(p.MainWindowHandle);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        public static Icon GetIconFromEntryExe()
        {
            var location = Assembly.GetEntryAssembly()?.Location;
            if (location == null) throw new ArgumentException("Failed to get location of EntryAssembly");
            if (location.EndsWith(".dll")) location = location.Substring(0, location.Length - 3) + "exe";
            var icon = DrawingTools.ExtractAssociatedIcon(location);
            return icon;
        }

        /// <summary>
        /// Process.GetProcessById but doesn't throw on issues and instead returns null
        /// </summary>
        public static Process GetProcessByIdSafe(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch
            {
                return null;
            }
        }
    }
}