/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Klocman;
using Microsoft.Win32.Interop;

namespace WinUpdateHelper
{
    /// <summary>
    ///     https://msdn.microsoft.com/en-us/library/windows/desktop/aa386065(v=vs.85).aspx
    /// 
    ///     Commands
    ///     u[ninstall] UpdateID     - Uninstall an update
    ///     l[ist]                   - List updates
    /// </summary>
    internal static class Program
    {
        private static QueryType _queryType;

        private static string _updateId;

        private static int Main(string[] args)
        {
            try
            {
                try { Console.OutputEncoding = Encoding.Unicode; }
                catch (IOException) { /*Old .NET v4 without support for unicode output*/ }

                ProcessCommandlineArguments(args);

                switch (_queryType)
                {
                    case QueryType.Uninstall:
                        UpdateManager.UninstallUpdate(_updateId);
                        break;

                    case QueryType.List:
                        UpdateManager.WriteUpdateList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_queryType), _queryType, null);
                }
            }
            catch (OperationCanceledException)
            {
                return (int)ResultWin32.ERROR_CANCELLED;
            }
            catch (COMException ex)
            {
                LogWriter.WriteMessageToLog(ex.ToString());
                Console.WriteLine("Error: {0}", Hresult.ConvertHresultToDetails(ex.ErrorCode));
                return (int)ResultWin32.ERROR_UNEXP_NET_ERR;
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessageToLog(ex.ToString());
                Console.WriteLine("Error: {0}", ex.Message);
                return (int)ResultWin32.ERROR_UNEXP_NET_ERR;
            }
            return (int)ResultWin32.ERROR_SUCCESS;
        }

        private static void ProcessCommandlineArguments(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case @"u":
                    case @"uninstall":
                        if (_queryType != QueryType.None) throw new ArgumentException(@"Multiple commands specified");
                        _queryType = QueryType.Uninstall;
                        break;

                    case @"l":
                    case @"list":
                        if (_queryType != QueryType.None) throw new ArgumentException(@"Multiple commands specified");
                        _queryType = QueryType.List;
                        break;

                    default:
                        if (_queryType != QueryType.Uninstall)
                            throw new ArgumentException($@"Unknown argument: {arg}");
                        if (_updateId != null)
                            throw new ArgumentException(@"Multiple UpdateIDs specified");
                        _updateId = arg;
                        break;
                }
            }

            if (_queryType == QueryType.None)
                throw new ArgumentException(@"No commands specified");

            if (_queryType == QueryType.Uninstall && _updateId == null)
                throw new ArgumentException(@"No UpdateID specified");
        }

        private enum QueryType
        {
            None,
            Uninstall,
            List
        }
    }
}