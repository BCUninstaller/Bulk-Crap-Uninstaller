/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using Klocman;
using Microsoft.Win32.Interop;

namespace StoreAppHelper
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return (int)ResultWin32.ERROR_BAD_ARGUMENTS;

            HelperTools.SetupEncoding();

            if (args.Length == 1 && string.Equals(args[0], @"/query", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = AppManager.QueryApps();
                    foreach (var app in result)
                        Console.WriteLine(HelperTools.ObjectToConsoleOutput(app));
                    return (int)ResultWin32.ERROR_SUCCESS;
                }
                catch (IOException ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return (int)HelperTools.ExtractHrefCode(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return (int)ResultWin32.ERROR_FUNCTION_FAILED;
                }
            }

            if (args.Length == 2 && string.Equals(args[0], @"/uninstall", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    AppManager.UninstallApp(args[1]);
                    return (int)ResultWin32.ERROR_SUCCESS;
                }
                catch (IOException ex)
                {
                    return (int)HelperTools.ExtractHrefCode(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return (int)ResultWin32.ERROR_FUNCTION_FAILED;
                }
            }

            return (int)ResultWin32.ERROR_BAD_ARGUMENTS;
        }
    }
}