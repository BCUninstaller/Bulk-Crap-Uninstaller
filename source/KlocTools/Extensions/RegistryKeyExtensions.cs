/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Klocman.Extensions
{
    public static class RegistryKeyExtensions
    {
        /// <summary>
        ///     Get only the name of this key, instead of the whole path
        /// </summary>
        public static string GetKeyName(this RegistryKey obj)
        {
            return obj.Name.Substring(obj.Name.LastIndexOf('\\') + 1);
        }

        /// <summary>
        ///     Lazily open and return all subkeys.
        /// </summary>
        public static IEnumerable<RegistryKey> GetSubKeys(this RegistryKey obj, bool writable)
        {
            if (obj == null)
                throw new NullReferenceException();

            return obj.GetSubKeyNames().Select(x => obj.OpenSubKey(x, writable));
        }
        
        public static IEnumerable<string> TryGetValueNames(this RegistryKey key)
        {
            try
            {
                return key.GetValueNames();
            }
            catch (IOException)
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Get the specified value as a string. If the value is a string, then it is trimmed up until the 
        /// first null character to avoid buggy GetValue returning data after the end of string.
        /// </summary>
        public static string GetStringSafe(this RegistryKey key, string valueName)
        {
            var v = key.GetValue(valueName, null, RegistryValueOptions.None)?.ToString();

            if (string.IsNullOrEmpty(v)) return v;

            // Handle strings written with invalid (too large) lengths
            // https://blogs.msdn.microsoft.com/oldnewthing/20040824-00/?p=38063/
            var ni = v.IndexOf('\0');
            if (ni >= 0)
                v = v.Substring(0, ni);

            // Strip any other invalid data
            return v.SafeNormalize();
        }
    }
}