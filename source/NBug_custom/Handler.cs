// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Handler.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32.Interop;
using NBug.Core.Reporting;
using NBug.Core.UI;
using NBug.Core.Util;
using NBug.Core.Util.Logging;
using Dispatcher = NBug.Core.Submission.Dispatcher;

namespace NBug
{
    public static class Handler
    {
        static Handler()
        {
            // Submit any queued reports on a seperate thread asynchronously, while exceptions handlers are being set);
            if (!Settings.SkipDispatching)
            {
                new Dispatcher(Settings.DispatcherIsAsynchronous);
            }
        }

        // Using delegates to make sure that static constructor gets called on delegate access

        /// <summary>
        ///     Used for handling WinForms exceptions bound to the UI thread.
        ///     Handles the <see cref="Application.ThreadException" /> events in <see cref="System.Windows.Forms" /> namespace.
        /// </summary>
        public static ThreadExceptionEventHandler ThreadException
        {
            get
            {
                {
                    return ThreadExceptionHandler;
                }
            }
        }

        /// <summary>
        ///     Used for handling general exceptions bound to the main thread.
        ///     Handles the <see cref="AppDomain.UnhandledException" /> events in <see cref="System" /> namespace.
        /// </summary>
        public static UnhandledExceptionEventHandler UnhandledException
        {
            get { return UnhandledExceptionHandler; }
        }

        /// <summary>
        ///     Used for handling WinForms exceptions bound to the UI thread.
        ///     Handles the <see cref="Application.ThreadException" /> events in <see cref="System.Windows.Forms" /> namespace.
        /// </summary>
        /// <param name="sender">Exception sender object.</param>
        /// <param name="e">Real exception is in: e.Exception</param>
        private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            if (HandleOutdatedWindowsCrash(e.Exception) && Settings.HandleExceptions)
            {
                Logger.Trace("Starting to handle a System.Windows.Forms.Application.ThreadException.");

                // WinForms UI thread exceptions do not propagate to more general handlers unless: Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                var executionFlow = new BugReport().Report(e.Exception, ExceptionThread.UI_WinForms);
                if (executionFlow == ExecutionFlow.BreakExecution)
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        ///     Used for handling general exceptions bound to the main thread.
        ///     Handles the <see cref="AppDomain.UnhandledException" /> events in <see cref="System" /> namespace.
        /// </summary>
        /// <param name="sender">Exception sender object.</param>
        /// <param name="e">Real exception is in: ((Exception)e.ExceptionObject)</param>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (HandleOutdatedWindowsCrash(e.ExceptionObject as Exception) && Settings.HandleExceptions)
            {
                Logger.Trace("Starting to handle a System.AppDomain.UnhandledException.");
                var executionFlow = new BugReport().Report((Exception) e.ExceptionObject, ExceptionThread.Main);
                if (executionFlow == ExecutionFlow.BreakExecution)
                {
                    Environment.Exit(0);
                }
            }
        }
        
        private static bool HandleOutdatedWindowsCrash(Exception dnfe)
        {
            // DllNotFoundException, EntryPointNotFoundException, possibly others
            if (dnfe != null)
            {
                if (dnfe.Message.Contains(@"'api-ms-win-core-com-l1-1-0.dll'"))
                {
                    MessageBox.Show("It seems like you're running an unsupported version of Windows. Please make sure you have all of the latest Windows service packs and updates installed and try again.\n\n" +
                                    "If updating didn't help you may need to use an older version of BCUninstaller. Check the README.md file for more information.",
                                    "Unsupported Windows Version", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Do not let NBug handle this, it only clogs up the error reports
                    Environment.Exit((int)ResultWin32.ERROR_DLL_NOT_FOUND);
                    return false;
                }
            }

            return true;
        }
    }
}