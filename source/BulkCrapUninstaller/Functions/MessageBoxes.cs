/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman;
using Klocman.Forms;
using Klocman.Forms.Tools;
using Klocman.Localising;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions
{
    internal static class MessageBoxes
    {
        public enum PressedButton
        {
            Cancel,
            Yes,
            No
        }

        public static Form DefaultOwner { get; set; }

        public static void RatingsDisabled()
        {
            MessageBox.Show(DefaultOwner,
                Localisable.MessageBoxes_RatingsDisabled_Message,
                Localisable.MessageBoxes_RatingErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void RatingUnavailable()
        {
            MessageBox.Show(DefaultOwner,
                Localisable.MessageBoxes_RatingUnavailable_Message,
                Localisable.MessageBoxes_RatingErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        internal static CustomMessageBox.PressedButton AskToSubmitFeedback()
        {
            return CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Submit_feedback,
                    Localisable.MessageBoxes_AskToSubmitFeedback_Message,
                    Localisable.MessageBoxes_AskToSubmitFeedback_Details,
                    SystemIcons.Question, Buttons.ButtonRate, Buttons.ButtonSubmit, Buttons.ButtonClose));
        }

        internal static PressedButton BackupFailedQuestion(string exMessage, Form owner)
        {
            switch (
                CustomMessageBox.ShowDialog(owner ?? DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Leftover_removal,
                        Localisable.MessageBoxes_BackupFailedQuestion_Message,
                        Localisable.MessageBoxes_BackupFailedQuestion_Details + exMessage,
                        SystemIcons.Warning, Buttons.ButtonContinue, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.Yes;
                default:
                    return PressedButton.Cancel;
            }
        }

        internal static PressedButton BackupRegistryQuestion(Form owner)
        {
            var check = new CmbCheckboxSettings(Localisable.MessageBoxes_RememberChoiceCheckbox)
            { DisableRightButton = true };
            switch (
                CustomMessageBox.ShowDialog(owner ?? DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Leftover_removal,
                        Localisable.MessageBoxes_BackupRegistryQuestion_Message,
                        Localisable.MessageBoxes_BackupRegistryQuestion_Details,
                        SystemIcons.Question, Buttons.ButtonCreate, Buttons.ButtonDontCreate, Buttons.ButtonCancel),
                    check))
            {
                case CustomMessageBox.PressedButton.Left:
                    if (check.Result == true)
                        Settings.Default.BackupLeftovers = YesNoAsk.Yes;
                    return PressedButton.Yes;

                case CustomMessageBox.PressedButton.Middle:
                    if (check.Result == true)
                        Settings.Default.BackupLeftovers = YesNoAsk.No;
                    return PressedButton.No;

                default:
                    return PressedButton.Cancel;
            }
        }

        internal static void CanSelectOnlyOneItemInfo()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_CanSelectOnlyOneItemInfo_Title,
                    Localisable.MessageBoxes_CanSelectOnlyOneItemInfo_Message,
                    Localisable.MessageBoxes_CanSelectOnlyOneItemInfo_Details,
                    SystemIcons.Warning, Buttons.ButtonOk));
        }

        /// <summary>
        ///     Used when sorted uninstall task hits the first quiet uninstaller.
        /// </summary>
        internal static CustomMessageBox CanWalkAwayInfo(Form owner)
        {
            return CustomMessageBox.Show(owner, new CmbBasicSettings(Localisable.MessageBoxes_CanWalkAwayInfo_Title,
                Localisable.MessageBoxes_CanWalkAwayInfo_Message,
                Localisable.MessageBoxes_CanWalkAwayInfo_Details,
                SystemIcons.Information, Buttons.ButtonOk)
            { StartPosition = FormStartPosition.CenterParent, AlwaysOnTop = true });
        }

        internal static bool ConfirmLowConfidenceQuestion(Form owner)
        {
            return CustomMessageBox.ShowDialog(owner ?? DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Junk_Leftover_removal,
                    Localisable.MessageBoxes_ConfirmLowConfidenceQuestion_Message,
                    Localisable.MessageBoxes_ConfirmLowConfidenceQuestion_Details,
                    SystemIcons.Warning, Buttons.ButtonYes, Buttons.ButtonCancel)) ==
                   CustomMessageBox.PressedButton.Middle;
        }

        internal static bool DeleteRegKeysConfirmation(string[] affectedKeyNames)
        {
            return CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_DeleteRegKeysConfirmation_Title,
                    Localisable.MessageBoxes_DeleteRegKeysConfirmation_Message,
                    string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_DeleteRegKeysConfirmation_Details,
                        string.Join("\n", affectedKeyNames)),
                    SystemIcons.Question, Buttons.ButtonRemove, Buttons.ButtonCancel)) ==
                   CustomMessageBox.PressedButton.Middle;
        }

        internal static void ExportFailed(string exMessage, Form owner)
        {
            CustomMessageBox.ShowDialog(owner ?? DefaultOwner, new CmbBasicSettings(Localisable.MessageBoxes_ExportFailed_Title,
                Localisable.MessageBoxes_ExportFailed_Message,
                Localisable.MessageBoxes_ExportFailed_Details + Localisable.MessageBoxes_Error_details + exMessage,
                SystemIcons.Error, Buttons.ButtonOk));
        }

        internal static string GetSystemRestoreDescription(int count)
        {
            return string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_GetSystemRestoreDescription, count);
        }

        internal static void InvalidNewEntryName()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Rename_uninstaller,
                    Localisable.MessageBoxes_InvalidNewEntryName_Message,
                    string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_InvalidNewEntryName_Details,
                        string.Join(" ", StringTools.InvalidPathChars.Select(x => x.ToString()).ToArray())),
                    SystemIcons.Warning, Buttons.ButtonOk));
        }

        /// <summary>
        ///     True if user wants to look for junk
        /// </summary>
        /// <returns></returns>
        internal static bool LookForJunkQuestion()
        {
            switch (Settings.Default.MessagesRemoveJunk)
            {
                case YesNoAsk.Yes:
                    return true;
                case YesNoAsk.No:
                    return false;
            }

            var check = new CmbCheckboxSettings(Localisable.MessageBoxes_RememberChoiceCheckbox);
            var result =
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Junk_Leftover_removal,
                        Localisable.MessageBoxes_LookForJunkQuestion_Message,
                        Localisable.MessageBoxes_LookForJunkQuestion_Details,
                        SystemIcons.Question, Buttons.ButtonYes, Buttons.ButtonNo), check);

            if (check.Result.HasValue && check.Result.Value)
                Settings.Default.MessagesRemoveJunk = result == CustomMessageBox.PressedButton.Middle
                    ? YesNoAsk.Yes
                    : YesNoAsk.No;

            return result == CustomMessageBox.PressedButton.Middle;
        }

        internal static void NoJunkFoundInfo()
        {
            // If automatically searching for junk do not show this message
            if (Settings.Default.MessagesRemoveJunk == YesNoAsk.Yes)
                return;

            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Leftover_removal,
                    Localisable.MessageBoxes_NoJunkFoundInfo_Message,
                    Localisable.MessageBoxes_NoJunkFoundInfo_Details,
                    SystemIcons.Information, Buttons.ButtonOk));
        }

        internal static void NoNetworkConnected()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_NoNetworkConnected_Open_online_content,
                    Localisable.MessageBoxes_NoNetworkConnected_Message,
                    Localisable.MessageBoxes_NoNetworkConnected_Details,
                    SystemIcons.Error, Buttons.ButtonOk));
        }

        internal static void NothingToCopy()
        {
            MessageBox.Show(
                Localisable.MessageBoxes_NothingToCopy_Message,
                Localisable.MessageBoxes_Title_Copy_to_clipboard, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        internal static void NoUninstallersSelectedInfo()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_NoUninstallersSelectedInfo_Title,
                    Localisable.MessageBoxes_NoUninstallersSelectedInfo_Message,
                    Localisable.MessageBoxes_NoUninstallersSelectedInfo_Details,
                    SystemIcons.Warning, Buttons.ButtonOk));
        }

        internal static bool OpenDirectoriesMessageBox(int sourceDirCount)
        {
            if (sourceDirCount <= 0)
            {
                MessageBox.Show(Localisable.MessageBoxes_OpenDirectories_NoDirsToOpen,
                    Localisable.MessageBoxes_Title_Open_directories,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            return (sourceDirCount == 1) || (MessageBox.Show(
                string.Format(CultureInfo.CurrentCulture, Localisable.MessageBoxes_OpenDirectoriesMessageBox_OpenMultiple, sourceDirCount),
                Localisable.MessageBoxes_Title_Open_directories, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.Cancel);
        }

        internal static void OpenDirectoryError(Exception e)
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Open_directories,
                    Localisable.MessageBoxes_OpenDirectoryError_Message,
                    Localisable.MessageBoxes_Error_details + e.Message, SystemIcons.Error, Buttons.ButtonOk));
        }

        internal static void OpenUninstallListError(string exMessage)
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Open_Uninstall_List,
                    Localisable.MessageBoxes_OpenUninstallListError_Message,
                    exMessage, SystemIcons.Error, Buttons.ButtonClose));
        }

        internal static PressedButton OpenUninstallListQuestion()
        {
            switch (
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Open_Uninstall_List,
                        Localisable.MessageBoxes_OpenUninstallListQuestion_Message,
                        Localisable.MessageBoxes_OpenUninstallListQuestion_Details,
                        SystemIcons.Question, Buttons.ButtonKeep, Buttons.ButtonClear, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Left:
                    return PressedButton.Yes;
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.No;
                default:
                    return PressedButton.Cancel;
            }
        }

        internal static void OpenUrlError(Exception e)
        {
            CustomMessageBox.ShowDialog(DefaultOwner, new CmbBasicSettings(Localisable.MessageBoxes_Title_Open_urls,
                Localisable.MessageBoxes_OpenUrlError_Message,
                Localisable.MessageBoxes_Error_details + e.Message, SystemIcons.Error, Buttons.ButtonOk));
        }

        internal static bool OpenUrlsMessageBox(int sourceDirCount)
        {
            if (sourceDirCount <= 0)
            {
                MessageBox.Show(Localisable.MessageBoxes_OpenUrlsMessageBox_No_URLs_to_open_Title,
                    Localisable.MessageBoxes_Title_Open_urls, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            return (sourceDirCount == 1) || (MessageBox.Show(
                string.Format(CultureInfo.CurrentCulture, Localisable.MessageBoxes_OpenUrlsMessageBox_OpenMultiple_Message, sourceDirCount),
                Localisable.MessageBoxes_Title_Open_urls, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.Cancel);
        }

        /// <summary>
        ///     Returns false if user wants to cancel
        /// </summary>
        internal static void ProtectedItemError(string affectedKeyName)
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Modify_protected_items,
                    Localisable.MessageBoxes_ProtectedItemError_Message,
                    string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_ProtectedItemError_Details, affectedKeyName),
                    SystemIcons.Error, Buttons.ButtonOk));
        }

        /// <summary>
        ///     Returns false if user wants to cancel
        /// </summary>
        internal static PressedButton ProtectedItemsWarningQuestion(string[] affectedKeyNames)
        {
            if (!affectedKeyNames.Any())
                return PressedButton.Yes;

            switch (
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Modify_protected_items,
                        Localisable.MessageBoxes_ProtectedItemsWarningQuestion_Message,
                        string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_ProtectedItemsWarningQuestion_Details,
                            string.Join("\n", affectedKeyNames)),
                        SystemIcons.Warning, Buttons.ButtonRemove, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.Yes;

                default:
                    return PressedButton.Cancel;
            }
        }

        internal static PressedButton QuietUninstallersNotAvailableQuestion(string[] nonQuiet)
        {
            if (!Settings.Default.MessagesAskRemoveLoudItems)
                return PressedButton.Yes;

            var check = new CmbCheckboxSettings(Localisable.MessageBoxes_RememberChoiceCheckbox)
            {
                DisableRightButton = true,
                DisableMiddleButton = true
            };
            var result =
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Quiet_uninstall,
                        Localisable.MessageBoxes_QuietUninstallersNotAvailableQuestion_Message,
                        string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_QuietUninstallersNotAvailableQuestion_Details,
                            string.Join("\n", nonQuiet)),
                        SystemIcons.Question, Buttons.ButtonUseLoud, Buttons.ButtonRemove, Buttons.ButtonCancel), check);

            switch (result)
            {
                case CustomMessageBox.PressedButton.Left:
                    if (check.Result.HasValue && check.Result.Value)
                        Settings.Default.MessagesAskRemoveLoudItems = false;
                    return PressedButton.Yes;
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.No;
                default:
                    return PressedButton.Cancel;
            }
        }

        internal static PressedButton ResetSettingsConfirmation()
        {
            switch (
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Restore_default_settings,
                        Localisable.MessageBoxes_ResetSettingsConfirmation_Message,
                        Localisable.MessageBoxes_ResetSettingsConfirmation_Details,
                        SystemIcons.Warning, Buttons.ButtonReset, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.Yes;

                default:
                    return PressedButton.Cancel;
            }
        }

        internal static bool RestartNeededForSettingChangeQuestion()
        {
            var result = CustomMessageBox.ShowDialog(DefaultOwner, new CmbBasicSettings(
                Localisable.MessageBoxes_RestartNeededForSettingChangeQuestion_Title,
                Localisable.MessageBoxes_RestartNeededForSettingChangeQuestion_Message,
                Localisable.MessageBoxes_RestartNeededForSettingChangeQuestion_Details,
                SystemIcons.Question, Buttons.ButtonYes, Buttons.ButtonNo));
            return result == CustomMessageBox.PressedButton.Middle;
        }

        internal static void SaveUninstallListError(string exMessage)
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Save_Uninstall_List,
                    Localisable.MessageBoxes_SaveUninstallListError_Message,
                    exMessage, SystemIcons.Error, Buttons.ButtonClose));
        }

        internal static void SearchOnlineError(Exception e)
        {
            CustomMessageBox.ShowDialog(DefaultOwner, new CmbBasicSettings(Localisable.MessageBoxes_Title_Search_online,
                Localisable.MessageBoxes_SearchOnlineError_Message,
                Localisable.MessageBoxes_Error_details + e.Message, SystemIcons.Error, Buttons.ButtonOk));
        }

        internal static PressedButton SearchOnlineMessageBox(int sourceDirCount)
        {
            if (sourceDirCount <= 0)
            {
                MessageBox.Show(Localisable.MessageBoxes_SearchOnlineMessageBox_NothingToSearchFor_Message,
                    Localisable.MessageBoxes_Title_Search_online, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return PressedButton.Cancel;
            }

            if (sourceDirCount == 1)
                return PressedButton.Yes;

            switch (MessageBox.Show(
                string.Format(CultureInfo.CurrentCulture, Localisable.MessageBoxes_OpenDirectoriesMessageBox_OpenMultiple,
                    sourceDirCount),
                Localisable.MessageBoxes_Title_Open_directories, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                case DialogResult.OK:
                    return PressedButton.Yes;

                default:
                    return PressedButton.Cancel;
            }
        }

        internal static bool SelfUninstallQuestion()
        {
            return CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_SelfUninstallQuestion_Title,
                    Localisable.MessageBoxes_SelfUninstallQuestion_Message,
                    Localisable.MessageBoxes_SelfUninstallQuestion_Details,
                    SystemIcons.Question, Buttons.ButtonUninstall, Buttons.ButtonCancel)) ==
                   CustomMessageBox.PressedButton.Middle;
        }

        internal static PressedButton SysRestoreBeginQuestion()
        {
            switch (Settings.Default.MessagesRestorePoints)
            {
                case YesNoAsk.Yes:
                    return PressedButton.Yes;
                case YesNoAsk.No:
                    return PressedButton.No;
            }

            var check = new CmbCheckboxSettings(Localisable.MessageBoxes_RememberChoiceCheckbox)
            {
                DisableRightButton = true
            };
            switch (
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Create_restore_point,
                        Localisable.MessageBoxes_SysRestoreBeginQuestion_Message,
                        Localisable.MessageBoxes_SysRestoreBeginQuestion_Details,
                        SystemIcons.Question, Buttons.ButtonCreate, Buttons.ButtonDontCreate, Buttons.ButtonCancel),
                    check))
            {
                case CustomMessageBox.PressedButton.Left:
                    if (check.Result.HasValue && check.Result.Value)
                        Settings.Default.MessagesRestorePoints = YesNoAsk.Yes;
                    return PressedButton.Yes;

                case CustomMessageBox.PressedButton.Middle:
                    if (check.Result.HasValue && check.Result.Value)
                        Settings.Default.MessagesRestorePoints = YesNoAsk.No;
                    return PressedButton.No;

                default:
                    return PressedButton.Cancel;
            }
        }

        /// <summary>
        ///     Returns true if user wants to continue even though system restore failed
        /// </summary>
        internal static PressedButton SysRestoreContinueAfterError(string exMessage)
        {
            switch (
                CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_Title_Create_restore_point,
                        Localisable.MessageBoxes_SysRestoreContinueAfterError_Message,
                        Localisable.MessageBoxes_SysRestoreContinueAfterError_Details +
                        Localisable.MessageBoxes_Error_details + exMessage,
                        SystemIcons.Warning, Buttons.ButtonContinue, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.Yes;

                default:
                    return PressedButton.Cancel;
            }
        }

        /// <summary>
        ///     Yes if the task should be killed,
        ///     No if the process should continue without killing the task and
        ///     Cancel if the skip should be aborted.
        /// </summary>
        internal static CustomMessageBox TaskSkipCurrentKillTaskQuestion(Form parent)
        {
            return CustomMessageBox.Show(parent, new CmbBasicSettings(Localisable.MessageBoxes_TaskSkip_Title,
                Localisable.MessageBoxes_TaskSkip_Message, Localisable.MessageBoxes_TaskSkip_Details,
                SystemIcons.Question, Buttons.ButtonTerminate, Buttons.ButtonSkip, Buttons.ButtonCancel));
        }

        internal static PressedButton TaskStopConfirmation(Form ownerForm)
        {
            switch (
                CustomMessageBox.ShowDialog(ownerForm,
                    new CmbBasicSettings(Localisable.MessageBoxes_TaskStopConfirmation_Title,
                        Localisable.MessageBoxes_TaskStopConfirmation_Message,
                        Localisable.MessageBoxes_TaskStopConfirmation_Details,
                        SystemIcons.Question, Buttons.ButtonStop, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.Yes;

                default:
                    return PressedButton.Cancel;
            }
        }

        internal static void UninstallAlreadyRunning()
        {
            PremadeDialogs.GenericError(Localisable.MessageBoxes_UninstallAlreadyRunning_Message);
        }

        internal static void UninstallMsiGuidMissing()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_UninstallMsiGuidMissing_Title,
                    Localisable.MessageBoxes_UninstallMsiGuidMissing_Message,
                    Localisable.MessageBoxes_UninstallMsiGuidMissing_Details, SystemIcons.Warning, Buttons.ButtonOk));
        }

        internal static void ModifyCommandMissing()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_ModifyCommandMissing_Title,
                    Localisable.MessageBoxes_ModifyCommandMissing_Message,
                    Localisable.MessageBoxes_ModifyCommandMissing_Details, SystemIcons.Warning, Buttons.ButtonOk));
        }

        internal static bool UpdateAskToDownload(Version latestVersion)
        {
            if (latestVersion == null) throw new ArgumentNullException(nameof(latestVersion));

            return CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Search_for_updates,
                    string.Format(CultureInfo.CurrentCulture, Localisable.MessageBoxes_UpdateAskToDownload_Message, latestVersion),
            //      string.Format(CultureInfo.CurrentCulture, Localisable.MessageBoxes_UpdateAskToDownload_Details, string.Join("\n- ", changes)),
                    string.Format(CultureInfo.CurrentCulture, "Do you want to open the download page to get the latest version of BCUninstaller?"), //todo localize
                    SystemIcons.Information, Buttons.ButtonYes, Buttons.ButtonNo)) == CustomMessageBox.PressedButton.Middle;
        }

        internal static void UpdateFailed(string errorMessage)
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Search_for_updates,
                    Localisable.MessageBoxes_UpdateFailed_Message,
                    Localisable.MessageBoxes_UpdateFailed_Details + Localisable.MessageBoxes_Error_details +
                    errorMessage,
                    SystemIcons.Error, Buttons.ButtonClose));
        }

        internal static void UpdateUptodate()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_Title_Search_for_updates,
                    Localisable.MessageBoxes_UpdateUptodate_Message,
                    Localisable.MessageBoxes_UpdateUptodate_Details,
                    SystemIcons.Information, Buttons.ButtonClose));
        }

        internal static void ForceRunUninstallFailedError(Form owner, IEnumerable<string> failed)
        {
            CustomMessageBox.ShowDialog(owner,
                new CmbBasicSettings(Localisable.MessageBoxes_ForceRunUninstallFailedError_Title,
                    Localisable.MessageBoxes_ForceRunUninstallFailedError_Header,
                    string.Format(CultureInfo.InvariantCulture, Localisable.MessageBoxes_ForceRunUninstallFailedError_Message,
                        string.Join("\n", failed.ToArray())),
                    SystemIcons.Error, Buttons.ButtonClose));
        }

        //public static void Net4MissingInfo()
        //{
        //    CustomMessageBox.ShowDialog(DefaultOwner,
        //        new CmbBasicSettings(Localisable.MessageBoxes_Net4Missing_Title,
        //            Localisable.MessageBoxes_Net4Missing_Message,
        //            Localisable.MessageBoxes_Net4Missing_Details, SystemIcons.Warning, Buttons.ButtonOk));
        //}

        public static void DisplayHelp()
        {
            var path = GetBundledFilePath(Resources.HelpFilename);
            if (path != null) PremadeDialogs.StartProcessSafely(path);
        }

        public static void DisplayLicense()
        {
            var path = GetBundledFilePath(Resources.LicenceFilename);
            if (path != null) PremadeDialogs.StartProcessSafely(path);
        }

        public static void DisplayPrivacyPolicy()
        {
            var path = GetBundledFilePath(Resources.PrivacyPolicyFilename);
            if (path != null) PremadeDialogs.StartProcessSafely(path);
        }

        public static string GetBundledFilePath(string filename)
        {
            var path = Path.Combine(Program.AssemblyLocation.FullName, filename);
            if (!File.Exists(path))
            {
                path = Path.Combine(Program.AssemblyLocation.FullName, "..", filename);
                if (!File.Exists(path))
                {
                    MessageBox.Show("Could not find file " + filename);
                    path = null;
                }
            }

            return path;
        }

        public static bool AskToRetryFailedQuietAsLoud(Form owner, IEnumerable<string> failedNames)
        {
            return CustomMessageBox.ShowDialog(owner,
                new CmbBasicSettings(Localisable.MessageBoxes_AskToRetryFailedQuietAsLoud_Title,
                    Localisable.MessageBoxes_AskToRetryFailedQuietAsLoud_Header,
                    string.Format(CultureInfo.InvariantCulture, "{0}\n\n{1}", Localisable.MessageBoxes_AskToRetryFailedQuietAsLoud_Details,
                        string.Join("\n", failedNames.OrderBy(x => x).ToArray())),
                    SystemIcons.Question, Buttons.ButtonYes, Buttons.ButtonNo)) == CustomMessageBox.PressedButton.Middle;
        }

        public static void UninstallFromDirectoryNothingFound()
        {
            CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_UninstallFromDirectory_Title,
                    Localisable.MessageBoxes_UninstallFromDirectoryNothingFound_Message,
                    Localisable.MessageBoxes_UninstallFromDirectoryNothingFound_Details,
                    SystemIcons.Information, Buttons.ButtonOk));
        }

        public static bool UninstallFromDirectoryUninstallerFound(string displayName, string uninstallString)
        {
            return CustomMessageBox.ShowDialog(DefaultOwner,
                new CmbBasicSettings(Localisable.MessageBoxes_UninstallFromDirectory_Title,
                    string.Format(Localisable.MessageBoxes_UninstallFromDirectoryUninstallerFound_Message, displayName),
                    Localisable.MessageBoxes_UninstallFromDirectoryUninstallerFound_Details + "\n\n" + uninstallString,
                    SystemIcons.Information, Buttons.ButtonUninstall, Buttons.ButtonSkip)) == CustomMessageBox.PressedButton.Middle;
        }

        public static PressedButton AskToSaveUninstallList()
        {
            switch (CustomMessageBox.ShowDialog(DefaultOwner,
                    new CmbBasicSettings(Localisable.MessageBoxes_AskToSaveUninstallList_Title,
                        Localisable.MessageBoxes_AskToSaveUninstallList_Message,
                        Localisable.MessageBoxes_AskToSaveUninstallList_Details,
                        SystemIcons.Question, Buttons.ButtonYes, Buttons.ButtonNo, Buttons.ButtonCancel)))
            {
                case CustomMessageBox.PressedButton.Right:
                    return PressedButton.Yes;

                case CustomMessageBox.PressedButton.Middle:
                    return PressedButton.No;

                default:
                    return PressedButton.Cancel;
            }
        }

        public static string SelectFolder(string title)
        {
            try
            {
                var dialog = new FolderBrowserDialog
                {
                    AutoUpgradeEnabled = true,
                    Description = title,
                    UseDescriptionForTitle = true,
                };

                return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                var dialog = new FolderBrowserDialog
                {
                    RootFolder = Environment.SpecialFolder.Desktop,
                    Description = title,
                    ShowNewFolderButton = false
                };

                return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
            }
        }

        public static void CantRenameUninstallerKind(string displayName, UninstallerType uninstallerKind)
        {
            CustomMessageBox.ShowDialog(DefaultOwner, new CmbBasicSettings(Localisable.MessageBoxes_Title_Rename_uninstaller,
                                                                           string.Format(Localisable.MessageBoxes_CantRenameUninstallerKind_Title, displayName),
                                                                           string.Format(Localisable.MessageBoxes_CantRenameUninstallerKind_Details, uninstallerKind.GetLocalisedName()),
                                                                           SystemIcons.Error, Buttons.ButtonOk));
        }
    }
}