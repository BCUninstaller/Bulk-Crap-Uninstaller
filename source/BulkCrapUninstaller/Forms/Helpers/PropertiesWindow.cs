/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Forms
{
    sealed partial class PropertiesWindow : Form
    {
        private readonly string _titleBeginning;

        public PropertiesWindow()
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;

            _titleBeginning = Text;
            Theming.ThemeManager.ApplyProperties(this, dataGridView1);
        }

        private InfoType CurrentlyVisiblePage
        {
            get
            {
                switch (tabControl2.SelectedIndex)
                {
                    case 0:
                        return InfoType.Overview;
                    case 1:
                        return InfoType.FileInfo;
                    case 2:
                        return InfoType.Registry;
                    case 3:
                        return InfoType.Certificate;
                    default:
                        return InfoType.Invalid;
                }
            }
        }

        public void ShowPropertiesDialog(IEnumerable<ApplicationUninstallerEntry> targets)
        {
            var applicationUninstallerEntries = targets as IList<ApplicationUninstallerEntry> ?? targets.ToList();

            if (applicationUninstallerEntries.Count <= 0) return;

            UseWaitCursor = true;

            tabControl1.Visible = applicationUninstallerEntries.Count > 1;
            tabControl1.TabPages.Clear();
            applicationUninstallerEntries.OrderBy(x => x.DisplayName)
                .ForEach(x => tabControl1.TabPages.Add(new TabPage(x.DisplayName) {Tag = x}));

            OnSelectedTabChanged(this, EventArgs.Empty);

            StartPosition = FormStartPosition.CenterParent;
            ShowDialog();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count < 0)
                e.Cancel = true;
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var content = dataGridView1.GetClipboardContent();
                if (content != null) Clipboard.SetDataObject(content, true);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps)
            {
                if (dataGridView1.SelectedCells.Count > 0)
                    contextMenuStrip1.Show(dataGridView1.PointToScreen(Point.Empty));
                e.Handled = true;
            }
        }

        private void OnSelectedTabChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null)
                return;

            UseWaitCursor = true;
            Application.DoEvents();

            dataGridView1.DataSource = AppPropertiesGatherer.GetInfo(tabControl1.SelectedTab.Tag as ApplicationUninstallerEntry, CurrentlyVisiblePage);

            // Make first column shorter, since it contains much less text. Default FillWeight is around 34
            var firstColumn = dataGridView1.Columns.GetFirstColumn(new DataGridViewElementStates());
            if (firstColumn != null)
            {
                firstColumn.FillWeight = 35f;
                dataGridView1.Sort(firstColumn, ListSortDirection.Ascending);
            }

            Text = _titleBeginning + tabControl1.SelectedTab.Text;

            UseWaitCursor = false;
        }

        private void PropertiesWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (contextMenuStrip1.Visible)
                    contextMenuStrip1.Hide();
                else
                    Hide();

                e.Handled = true;
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var content = dataGridView1.GetClipboardContent();
            if (content != null)
            {
                try
                {
                    File.WriteAllText(saveFileDialog1.FileName, content.GetText());
                }
                catch (Exception ex)
                {
                    MessageBoxes.ExportFailed(ex.Message, this);
                }
            }
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }
    }
}