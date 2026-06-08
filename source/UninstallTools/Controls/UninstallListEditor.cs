/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using UninstallTools.Lists;
using UninstallTools.Properties;

namespace UninstallTools.Controls
{
    public partial class UninstallListEditor : UserControl
    {
        private UninstallList _currentList;

        public UninstallListEditor()
        {
            InitializeComponent();

            comboBoxFilterType.SelectedIndex = 0;

            filterEditor.ComparisonMethodChanged += OnFiltersChanged;
            filterEditor.LostFocus += EditorFocusLost;

            groupBoxFilterSettings.Enabled = false;
            splitContainer1.Panel2.Enabled = false;
            filterEditor.TargetFilterCondition = null;
        }

        [ReadOnly(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UninstallList CurrentList
        {
            get { return _currentList; }
            set
            {
                _currentList = value;
                Enabled = _currentList != null;
                PopulateList();
                OnCurrentListChanged(this, EventArgs.Empty);
            }
        }

        private Filter CurrentlySelected
        {
            get
            {
                if (listView1.SelectedItems.Count <= 0)
                    return null;
                return listView1.SelectedItems[0].Tag as Filter;
            }
        }

        public event EventHandler CurrentListChanged;

        private void OnCurrentListChanged(object sender, EventArgs e)
        {
            OnFiltersChanged(sender, e);
            CurrentListChanged?.Invoke(sender, e);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Filter newItem;
            try
            {
                newItem = new Filter(Localisation.UninstallListEditor_NewFilter, Localisation.UninstallListEditor_NewFilter);
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                return;
            }

            CurrentList.Add(newItem);
            PopulateList();
            OnFiltersChanged(sender, e);
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            var item = CurrentlySelected;
            if (item == null)
                return;

            CurrentList.Remove(item);
            PopulateList();
            OnFiltersChanged(sender, e);
        }

        private void EditorFocusLost(object sender, EventArgs e)
        {
            RefreshSelectedFilter();
        }

        private void OnSelectedFilterChanged(object sender, EventArgs e)
        {
            var selection = CurrentlySelected;
            if (selection != null)
            {
                textBoxFilterName.Text = selection.Name;
                comboBoxFilterType.SelectedIndex = selection.Exclude ? 1 : 0;

                splitContainer1.Panel2.Enabled = true;
                groupBoxFilterSettings.Enabled = true;
            }
            else
            {
                groupBoxFilterSettings.Enabled = false;
                splitContainer1.Panel2.Enabled = false;
            }

            PopulateConditions();
        }

        private void PopulateConditions()
        {
            listBoxConditions.SelectedItem = null;
            listBoxConditions.Items.Clear();

            var selection = CurrentlySelected;
            if (selection?.ComparisonEntries != null)
            {
                groupBoxConditions.Enabled = true;
                listBoxConditions.Items.AddRange(selection.ComparisonEntries.AsEnumerable().Reverse().Cast<object>().ToArray());
            }
            else
            {
                groupBoxConditions.Enabled = false;
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                CurrentList.AddItems(UninstallList.ReadFromFile(openFileDialog.FileName).Filters);
                PopulateList();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                e.Cancel = true;
            }
        }

        public void PopulateList()
        {
            groupBoxConditions.Enabled = false;
            listView1.Items.Clear();

            textBoxFilterName.Text = string.Empty;
            comboBoxFilterType.SelectedIndex = 0;

            if (CurrentList == null)
                return;

            listView1.Items.AddRange(CurrentList.Filters.Select(x => new ListViewItem(
                new[]
                {
                    x.Name,
                    Filter.ExcludeToString(x.Exclude),
                    x.ComparisonEntries.Count.ToString()
                })
            { Tag = x }).ToArray());
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            listBoxConditions.Update();
            FiltersChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Fires whenever the filters can potentially give a different result
        /// </summary>
        public event EventHandler FiltersChanged;

        private void RefreshSelectedFilter()
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            var item = listView1.SelectedItems[0];
            var tag = CurrentlySelected;

            item.SubItems[0].Text = tag.Name;
            item.SubItems[1].Text = Filter.ExcludeToString(tag.Exclude);
            item.SubItems[2].Text = tag.ComparisonEntries.Count.ToString();
            item.EnsureVisible();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxConditions.SelectedItem is not FilterCondition item)
            {
                filterEditor.TargetFilterCondition = null;
            }
            else if (!ReferenceEquals(filterEditor.TargetFilterCondition, item))
            {
                filterEditor.TargetFilterCondition = item;
            }
        }

        private void toolStripButtonAddCondition_Click(object sender, EventArgs e)
        {
            filterEditor.TargetFilterCondition = null;

            CurrentlySelected.ComparisonEntries.Add(new FilterCondition());

            PopulateConditions();
            OnFiltersChanged(sender, e);
        }

        private void toolStripButtonRemoveCondition_Click(object sender, EventArgs e)
        {
            if (listBoxConditions.SelectedItem is not FilterCondition item) return;
            filterEditor.TargetFilterCondition = null;

            CurrentlySelected.ComparisonEntries.Remove(item);
            PopulateConditions();
            OnFiltersChanged(sender, e);
        }

        private void textBoxFilterName_TextChanged(object sender, EventArgs e)
        {
            if (CurrentlySelected == null) return;
            CurrentlySelected.Name = textBoxFilterName.Text ?? string.Empty;
            RefreshSelectedFilter();
        }

        private void comboBoxFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentlySelected == null) return;
            CurrentlySelected.Exclude = comboBoxFilterType.SelectedIndex != 0;
            RefreshSelectedFilter();
            OnFiltersChanged(sender, e);
        }
    }
}