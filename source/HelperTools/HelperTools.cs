using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32.Interop;

namespace Klocman
{
    internal static partial class HelperTools
    {
        public static void SetupEncoding()
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
            }
            catch (IOException)
            {
                /*Old .NET v4 without support for unicode output*/
            }
        }

        [GeneratedRegex(@"0x[\d\w]{8}")] private static partial Regex HrefRegex();

        /// <summary>
        /// Try to extract the error code from an exception. The message is expected to contain a code in the format 0xXXXXXXXX
        /// otherwise HResult is returned as-is instead.
        /// </summary>
        public static ResultWin32 ExtractHrefCode(Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            var code = ExtractHrefCode(error.Message);
            if (code == ResultWin32.INVALID_ERROR_CODE) return (ResultWin32)error.HResult;
            return code;
        }
        /// <summary>
        /// Try to extract the error code from an error message. The message is expected to contain a code in the format 0xXXXXXXXX
        /// where X is a hexadecimal digit. If the code is not found or is invalid, ResultWin32.INVALID_ERROR_CODE is returned.
        /// </summary>
        public static ResultWin32 ExtractHrefCode(string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            var errorCode = HrefRegex().Match(errorMessage).Captures.FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(errorCode) || errorCode.Length < 8)
                return ResultWin32.INVALID_ERROR_CODE;

            int.TryParse(errorCode[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var errorNumber);

            var code = (ResultWin32)errorNumber;
            
            return Enum.IsDefined(code) ? code : ResultWin32.INVALID_ERROR_CODE;
        }

        /// <summary>
        ///     Convert object to PropertyName: Value format for writing it to console.
        ///     Only public properties with getters are processed.
        ///     Can be written directly with Console.WriteLine for an empty line after object.
        /// </summary>
        /// <param name="obj">Object to convert to string</param>
        /// <param name="provider">Provider used for converting values to strings</param>
        public static string ObjectToConsoleOutput(object obj, IFormatProvider provider = null)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            provider ??= CultureInfo.InvariantCulture;

            var propInfos = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.CanRead)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
            return KeyValueListToConsoleOutput(propInfos, provider);
        }

        public static string KeyValueListToConsoleOutput(ICollection<KeyValuePair<string, object>> propertyKeyValues,
            IFormatProvider provider = null)
        {
            if (propertyKeyValues == null) throw new ArgumentNullException(nameof(propertyKeyValues));
            provider ??= CultureInfo.InvariantCulture;

            var maxLen = propertyKeyValues.Max(x => x.Key.Length) + 2;

            var sb = new StringBuilder();

            foreach (var prop in propertyKeyValues)
            {
                sb.Append(prop.Key);
                sb.Append(':');
                sb.Append(' ', maxLen - prop.Key.Length);

                if (prop.Value is string s)
                    sb.Append(s.Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' '));
                else if (prop.Value is IConvertible convertible)
                    sb.Append(convertible.ToString(provider));
                else
                    sb.Append(prop.Value);

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}