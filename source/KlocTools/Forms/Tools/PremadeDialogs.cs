/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Properties;
using Klocman.Tools;

namespace Klocman.Forms.Tools
{
    public static class PremadeDialogs
    {
        public static Form DefaultOwner { get; set; }

        public static Action<Exception> SendErrorAction { get; set; }

        /// <summary>
        ///     Attempt to run the specified command then catch and display any exceptions in a message box.
        ///     True is returned if there were no exceptions thrown.
        /// </summary>
        public static bool StartProcessSafely(string command)
        {
            try
            {
                Process.Start(new ProcessStartInfo(command) { UseShellExecute = true });
                return true;
            }
            catch (Win32Exception)
            {
                // Thrown then user cancels the unverified file execute dialog
            }
            catch (Exception ex)
            {
                GenericError(ex);
            }
            return false;
        }

        /// <summary>
        ///     Attempt to run the specified command then catch and display any exceptions in a message box.
        ///     True is returned if there were no exceptions thrown.
        /// </summary>
        public static bool StartProcessSafely(string command, string arguments)
        {
            try
            {
                Process.Start(new ProcessStartInfo(command, arguments) { UseShellExecute = true });
                return true;
            }
            catch (Win32Exception)
            {
                // Thrown then user cancels the unverified file execute dialog
            }
            catch (Exception ex)
            {
                GenericError(ex);
            }
            return false;
        }

        /// <summary>
        ///     Show a generic error message with supplied exception info. Shows all inner exceptions and stack traces as well as a
        ///     copy button.
        /// </summary>
        public static void GenericError(Exception ex)
        {
            if (ex == null)
                return;

            Console.WriteLine(@"Showing error message: " + ex);

            if (SendErrorAction != null)
                SendErrorQuestion(ex);
            else
                GenericError(ex.Message, GetExceptionDetailString(ex));
        }

        private static string GetExceptionDetailString(Exception ex)
        {
            if (ex == null) return string.Empty;

            var sb = new StringBuilder();
            do
            {
                sb.Append(ex.GetType().FullName);
                sb.AppendLine();
                sb.Append(ex.Message);
                sb.AppendLine();
                sb.Append(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.Append(Localisation.PremadeDialogs_GenericError_InnerExceptionTitle);
                    sb.AppendLine();

                    ex = ex.InnerException;
                }
                else
                {
                    ex = null;
                }
            } while (ex != null);

            var details = sb.ToString();
            return details;
        }

        public static bool KillRunningProcessesQuestion()
        {
            var result = false;

            if (DefaultOwner != null)
                DefaultOwner.SafeInvoke(() => result = KillRunningProcessesQuestionSafe());
            else
                result = KillRunningProcessesQuestionSafe();

            return result;
        }

        /// <summary>
        ///     Show a generic error message.
        /// </summary>
        public static void GenericError(string errorType, string additionalInfo = null)
        {
            if (string.IsNullOrEmpty(errorType))
                return;

            if (string.IsNullOrEmpty(additionalInfo))
                additionalInfo = errorType;

            var entryAsy = Assembly.GetEntryAssembly();
            if (entryAsy != null)
            {
                entryAsy.ManifestModule.GetPEKind(out var peKind, out var machine);
                var bits = ProcessTools.Is64BitProcess ? "64bit" : "32bit";
                additionalInfo = $"{entryAsy.FullName} | {peKind} | {machine} | {Environment.OSVersion} | {bits}\n{additionalInfo}";
            }

            if (DefaultOwner != null)
                DefaultOwner.SafeInvoke(() => GenericErrorSafe(errorType, additionalInfo));
            else
                GenericErrorSafe(errorType, additionalInfo);
        }

        private static bool KillRunningProcessesQuestionSafe()
        {
            return CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisation.PremadeDialogs_KillRunningProcessesQuestion_Title,
                    Localisation.PremadeDialogs_KillRunningProcessesQuestion_Message,
                    Localisation.PremadeDialogs_KillRunningProcessesQuestion_Details
                    , SystemIcons.Question, Buttons.ButtonOk, Buttons.ButtonCancel))
                   == CustomMessageBox.PressedButton.Middle;
        }

        /// <summary>
        ///     If user choses to send error information, SendErrorAction is called.
        /// </summary>
        private static void SendErrorQuestion(Exception ex)
        {
            switch (CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisation.PremadeDialogs_GenericError_Title,
                    Localisation.PremadeDialogs_GenericError_Heading,
                    string.Format(Localisation.PremadeDialogs_GenericError_Details, ex.Message), SystemIcons.Error,
                    Buttons.ButtonSubmit, Buttons.ButtonCopy, Buttons.ButtonClose)))
            {
                case CustomMessageBox.PressedButton.Left:
                    SendErrorAction.Invoke(ex);
                    break;

                case CustomMessageBox.PressedButton.Middle:
                    var fullInfo = GetExceptionDetailString(ex);
                    try
                    {
                        if (DefaultOwner != null)
                            DefaultOwner.SafeInvoke(() => Clipboard.SetText(fullInfo));
                        else
                            Clipboard.SetText(fullInfo);
                    }
                    catch (ExternalException)
                    {
                    }
                    break;
            }
        }

        private static void GenericErrorSafe(string errorType, string fullInfo)
        {
            switch (CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisation.PremadeDialogs_GenericError_Title,
                    Localisation.PremadeDialogs_GenericError_Heading,
                    string.Format(Localisation.PremadeDialogs_GenericError_Details, errorType), SystemIcons.Error,
                    Buttons.ButtonCopy, Buttons.ButtonDetails, Buttons.ButtonClose)))
            {
                case CustomMessageBox.PressedButton.Middle:
                    GenericErrorExtendedSafe(fullInfo);
                    break;

                case CustomMessageBox.PressedButton.Left:
                    try
                    {
                        if (DefaultOwner != null)
                            DefaultOwner.SafeInvoke(() => Clipboard.SetText(fullInfo));
                        else
                            Clipboard.SetText(fullInfo);
                    }
                    catch (ExternalException)
                    {
                    }
                    break;
            }
        }

        private static void GenericErrorExtendedSafe(string fullInfo)
        {
            var trimmed = fullInfo.Length > 1000 ? fullInfo.Substring(0, 997) + "..." : fullInfo;

            if (CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisation.PremadeDialogs_GenericError_Title,
                    Localisation.PremadeDialogs_GenericErrorExtendedSafe_Heading, trimmed, SystemIcons.Error,
                    Buttons.ButtonCopy, Buttons.ButtonClose)) == CustomMessageBox.PressedButton.Middle)
            {
                try
                {
                    if (DefaultOwner != null)
                        DefaultOwner.SafeInvoke(() => Clipboard.SetText(fullInfo));
                    else
                        Clipboard.SetText(fullInfo);
                }
                catch (ExternalException)
                {
                }
            }
        }
    }
}