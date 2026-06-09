/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BulkCrapUninstaller.Functions.Tools
{
    public static class StartupArgumentTools
    {
        public static string GetStartupUninstallListPath(IEnumerable<string> commandLineArgs)
        {
            var candidate = commandLineArgs?.Skip(1).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(candidate))
                return null;

            try
            {
                if (!candidate.EndsWith(".bcul", StringComparison.OrdinalIgnoreCase))
                    return null;

                return File.Exists(candidate) ? candidate : null;
            }
            catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
            {
                return null;
            }
        }
    }
}
