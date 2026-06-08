using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32.Interop
{
    internal static class ResultWin32Extensions
    {
        /// <summary>
        /// Converts a ResultWin32 error code to an appropriate .Net Exception.
        /// </summary>
        /// <param name="errorCode">The ResultWin32 error code to convert.</param>
        /// <param name="additionalMessage">An optional message to append to the exception message.</param>
        /// <param name="getSystemMessage">Whether to include the system message for the error code, if any.</param>
        /// <returns>An Exception corresponding to the specified error code.</returns>
        public static Exception ToException(this ResultWin32 errorCode, string additionalMessage = null, bool getSystemMessage = true)
        {
            var message = errorCode.ToString();

            if (getSystemMessage)
            {
                var systemMessage = GetSystemMessage(errorCode);
                if (!string.IsNullOrWhiteSpace(systemMessage))
                    message += " | " + systemMessage;
            }

            if (!string.IsNullOrWhiteSpace(additionalMessage))
                message += " | " + additionalMessage;

            switch (errorCode)
            {
                case ResultWin32.ERROR_BAD_ENVIRONMENT:
                case ResultWin32.ERROR_INVALID_FUNCTION:
                    return new InvalidOperationException(message);

                case ResultWin32.ERROR_INVALID_DATA:
                    return new InvalidDataException(message);

                case ResultWin32.ERROR_ACCESS_DENIED:
                case ResultWin32.ERROR_NETWORK_ACCESS_DENIED:
                    return new UnauthorizedAccessException();

                case ResultWin32.ERROR_FILE_NOT_FOUND:
                case ResultWin32.ERROR_PATH_NOT_FOUND:
                    return new FileNotFoundException(message);

                case ResultWin32.ERROR_NOT_ENOUGH_MEMORY:
                case ResultWin32.ERROR_NOT_ENOUGH_SERVER_MEMORY:
                    return new InsufficientMemoryException(message);

                case ResultWin32.ERROR_INVALID_PARAMETER:
                case ResultWin32.ERROR_DIRECTORY:
                case ResultWin32.ERROR_UNKNOWN_PROPERTY:
                case ResultWin32.ERROR_UNKNOWN_PRODUCT:
                case ResultWin32.ERROR_UNKNOWN_FEATURE:
                case ResultWin32.ERROR_UNKNOWN_COMPONENT:
                    return new ArgumentException(message);

                case ResultWin32.ERROR_INSTALL_USEREXIT:
                    return new OperationCanceledException(message);

                case ResultWin32.ERROR_CALL_NOT_IMPLEMENTED:
                    return new NotImplementedException(message);

                case ResultWin32.WAIT_TIMEOUT:
                case ResultWin32.ERROR_SEM_TIMEOUT:
                    return new TimeoutException(message);

                default:
                    return new IOException(message, (int)errorCode);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW")]
        private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, uint nSize, IntPtr arguments);

        /// <summary>
        /// Retrieves the system error message associated with the specified Win32 error code, if any.
        /// </summary>
        /// <param name="errorCode">The Win32 error code for which to obtain the system message.</param>
        /// <returns>The corresponding system error message, or null if no message is found.</returns>
        public static string GetSystemMessage(this ResultWin32 errorCode)
        {
            const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

            var buf = new StringBuilder(1024);
            var formatCount = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                                            IntPtr.Zero, (uint)errorCode, 0, buf, (uint)buf.Capacity, IntPtr.Zero);

            return formatCount != 0 ? buf.ToString().Trim() : null;
        }

        /// <summary>
        /// Determines whether the specified ResultCom value represents an error code.
        /// </summary>
        /// <param name="comCode">The ResultCom value to evaluate.</param>
        /// <returns>true if the value represents an error; otherwise, false.</returns>
        public static bool IsError(this ResultCom comCode) => (int)comCode < 0;
        /// <summary>
        /// Determines whether the specified ResultCom value represents a success code.
        /// </summary>
        /// <param name="comCode">The ResultCom value to evaluate.</param>
        /// <returns>true if the value is greater than or equal to zero; otherwise, false.</returns>
        public static bool IsSuccess(this ResultCom comCode) => (int)comCode >= 0;
    }
}