/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Forms.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using UninstallTools;
using UninstallTools.Lists;

namespace BulkCrapUninstaller.Controls
{
    public partial class AdvancedFilters : UserControl
    {
        public event EventHandler CurrentListChanged;
        public event EventHandler CurrentListFileNameChanged;
        public event EventHandler FiltersChanged;
        public event EventHandler UnsavedChangesChanged;

        private static readonly string DefaultUninstallListPath = Path.Combine(Program.AssemblyLocation.FullName, Resources.DefaultUninstallListFilename);
        private string _currentListFileName;
        private bool _unsavedChanges;

        public UninstallList CurrentList => uninstallListEditor1.CurrentList;

        public bool UnsavedChanges
        {
            get { return _unsavedChanges; }
            private set
            {
                if (_unsavedChanges != value)
                {
                    _unsavedChanges = value;
                    UnsavedChangesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string CurrentListFileName
        {
            get { return _currentListFileName; }
            private set
            {
                if (_currentListFileName != value)
                {
                    _currentListFileName = value;
                    CurrentListFileNameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public AdvancedFilters()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            uninstallListEditor1.CurrentListChanged += OnCurrentListChanged;
            uninstallListEditor1.FiltersChanged += OnFiltersChanged;

            toolStripButtonDelete.Enabled = File.Exists(DefaultUninstallListPath);
        }

        private bool AskToSaveUnsaved()
        {
            if (!UnsavedChanges || uninstallListEditor1.CurrentList == null)
                return true;

            switch (MessageBoxes.AskToSaveUninstallList())
            {
                case MessageBoxes.PressedButton.Cancel:
                    return false;
                case MessageBoxes.PressedButton.Yes:
                    return ShowSaveDialog();
                case MessageBoxes.PressedButton.No:
                    return true;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public void LoadUninstallList(UninstallList list)
        {
            CurrentListFileName = string.Empty;
            uninstallListEditor1.CurrentList = list;
        }

        /// <summary>
        /// Load a list silently from filename
        /// </summary>
        /// <param name="fileName">Filename of the list</param>
        public void LoadUninstallList(string fileName)
        {
            try
            {
                var result = UninstallList.ReadFromFile(fileName);

                CurrentListFileName = fileName;
                uninstallListEditor1.CurrentList = result;
                UnsavedChanges = false;
            }
            catch (SecurityException ex)
            {
                PremadeDialogs.GenericError(ex);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError("File is not an uninstall list or it can't be opened",
                    "Please note that uninstall lists are saved in the \"Advanced filtering\" view, not by exporting. Lists should have the .bcul extension.\n\nError message: " + ex.Message);
            }
        }

        /// <summary>
        /// Show file select gui
        /// </summary>
        public void LoadUninstallList()
        {
            toolStripButtonOpenUl_Click(this, EventArgs.Empty);
        }

        private void OnCurrentListChanged(object sender, EventArgs e)
        {
            if (CurrentList == null)
                CurrentListFileName = string.Empty;

            UnsavedChanges = false;
            CurrentListChanged?.Invoke(sender, e);
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            UnsavedChanges = true;
            FiltersChanged?.Invoke(sender, e);
        }

        private void openUlDialog_FileOk(object sender, CancelEventArgs e)
        {
            LoadUninstallList(openUlDialog.FileName);
        }

        private void saveUlDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                CurrentList.SaveToFile(saveUlDialog.FileName);
                CurrentListFileName = saveUlDialog.FileName;
                UnsavedChanges = false;
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void ShowSaveDialog(object sender, EventArgs e)
        {
            ShowSaveDialog();
        }

        private bool ShowSaveDialog()
        {
            if (!string.IsNullOrEmpty(CurrentListFileName))
            {
                try
                {
                    saveUlDialog.InitialDirectory = Path.GetDirectoryName(CurrentListFileName);
                    saveUlDialog.FileName = Path.GetFileName(CurrentListFileName);
                }
                catch (ArgumentException)
                {
                }
                catch (PathTooLongException)
                {
                }
            }
            return saveUlDialog.ShowDialog(this) == DialogResult.OK;
        }

        private void toolStripButtonOpenUl_Click(object sender, EventArgs e)
        {
            if (!AskToSaveUnsaved()) return;

            if (!string.IsNullOrEmpty(CurrentListFileName))
            {
                try
                {
                    openUlDialog.InitialDirectory = Path.GetDirectoryName(CurrentListFileName);
                    openUlDialog.FileName = Path.GetFileName(CurrentListFileName);
                }
                catch (ArgumentException)
                {
                }
                catch (PathTooLongException)
                {
                }
            }
            openUlDialog.ShowDialog(this);
        }

        private void toolStripButtonSaveUlDef_Click(object sender, EventArgs e)
        {
            CurrentList.SaveToFile(DefaultUninstallListPath);
            CurrentListFileName = DefaultUninstallListPath;

            toolStripButtonDelete.Enabled = true;
        }

        private void toolStripButtonToBasicFilters_Click(object sender, EventArgs e)
        {
            if (!AskToSaveUnsaved()) return;

            uninstallListEditor1.CurrentList = null;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<IEnumerable<ApplicationUninstallerEntry>> SelectedEntryGetter { get; set; }

        private void toolStripButtonAddSelectedAsFilters_Click(object sender, EventArgs e)
        {
            if (SelectedEntryGetter == null) throw new InvalidOperationException(nameof(SelectedEntryGetter) + " is null");
            if (CurrentList == null) throw new InvalidOperationException(nameof(CurrentList) + " is null");

            var entries = SelectedEntryGetter();
            var filters = entries.Select(x => new Filter(x.DisplayName, false,
                new FilterCondition(x.DisplayName, ComparisonMethod.Equals,
                    nameof(ApplicationUninstallerEntry.DisplayName))));

            CurrentList.AddItems(filters);

            RepopulateList();
        }

        public void RepopulateList()
        {
            uninstallListEditor1.PopulateList();
            //OnCurrentListChanged(this, EventArgs.Empty);
            OnFiltersChanged(this, EventArgs.Empty);
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(DefaultUninstallListPath);

                toolStripButtonDelete.Enabled = false;
            }
            catch (SystemException ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }
    }
}
