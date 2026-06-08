/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Klocman;
using Microsoft.Win32.Interop;

namespace UninstallerAutomatizer
{
    /// <summary>
    /// UninstallerAutomatizer.exe UninstallerType [/K] UninstallCommand
    /// </summary>
    internal static class Program
    {
        public static readonly string AutomatizerProcessName = Process.GetCurrentProcess().ProcessName;

        public static ResultWin32 ReturnValue { get; set; } = ResultWin32.ERROR_SUCCESS;

        [STAThread]
        private static int Main()
        {
            using (LogWriter.StartLogging())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new MainWindow());

                return (int)ReturnValue;
            }
        }
    }
}