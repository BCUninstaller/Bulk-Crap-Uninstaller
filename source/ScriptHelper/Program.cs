/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman;
using Microsoft.Win32.Interop;

namespace ScriptHelper
{
    internal static class Program
    {
        private static QueryType _queryType;
        private static string _appId;

        [STAThread]
        private static int Main(string[] args)
        {
            HelperTools.SetupEncoding();

            try
            {
                ProcessCommandlineArguments(args);

                switch (_queryType)
                {
                    case QueryType.Uninstall:
                        Console.WriteLine("Uninstalling " + _appId);
                        var tweakEntry = Tweaks.GetEntry(_appId);
                        if (tweakEntry == null) throw new ArgumentException("Could not find a tweak with this name to uninstall");
                        tweakEntry.OnUninstall(true);
                        break;
                    case QueryType.List:
                        foreach (var result in Tweaks.GetConsoleOutput())
                        {
                            var converted = result.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)).ToList();
                            Console.WriteLine(HelperTools.KeyValueListToConsoleOutput(converted));
                        }
                        break;
                    default:
                        Console.WriteLine("Accepted commands:\nlist\nuninstall <id>");
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                return (int)ResultWin32.ERROR_CANCELLED;
            }
            catch (FormatException ex)
            {
                LogWriter.WriteExceptionToLog(ex);
                return (int)ResultWin32.ERROR_BAD_ARGUMENTS;
            }
            catch (Exception ex)
            {
                LogWriter.WriteExceptionToLog(ex);
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
                        if (_queryType != QueryType.None) throw new FormatException(@"Multiple commands specified");
                        _queryType = QueryType.Uninstall;
                        break;

                    case @"l":
                    case @"list":
                        if (_queryType != QueryType.None) throw new FormatException(@"Multiple commands specified");
                        _queryType = QueryType.List;
                        break;

                    default:
                        if (_appId != null) throw new FormatException(@"Too many parameters");
                        _appId = arg;
                        break;
                }
            }

            if (_queryType == QueryType.None)
                throw new FormatException(@"No commands specified");
            if (_queryType == QueryType.Uninstall && _appId == null)
                throw new FormatException(@"Missing ID parameter for what to uninstall");
        }

        private enum QueryType
        {
            None,
            Uninstall,
            List,
        }
    }
}