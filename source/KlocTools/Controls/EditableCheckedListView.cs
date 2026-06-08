/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public partial class EditableCheckedListView : UserControl
    {
        #region Delegates

        public delegate string GetStringDelegate(string previous);

        #endregion Delegates

        #region Constructors

        public EditableCheckedListView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        public event Action<EditableCheckedListView> ListViewChanged;

        #endregion Events

        #region Properties

        public IEnumerable<string> CheckedItems
        {
            get { return checkedListBox1.CheckedItems.Cast<string>(); }
        }

        public IEnumerable<string> Items
        {
            get { return checkedListBox1.Items.Cast<string>(); }
        }

        [Description("Sorting of the list box enabled/disabled"),
         Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ListBoxSorted
        {
            get { return checkedListBox1.Sorted; }
            set { checkedListBox1.Sorted = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public GetStringDelegate StringGetter { get; set; }

        [Description("Text shown in the title of this control"),
         Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get { return groupBox1.Text; }
            set { groupBox1.Text = value; }
        }

        #endregion Properties

        #region Methods

        public void AddRange(object[] items, object[] checkedItems)
        {
            foreach (var item in items)
            {
                checkedListBox1.Items.Add(item, checkedItems.Contains(item));
            }
            FireListViewChanged();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var output = GetString(null);
            if (string.IsNullOrEmpty(output) || Items.Contains(output))
                return;

            checkedListBox1.Items.Add(output, true);
            FireListViewChanged();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItem != null)
            {
                checkedListBox1.Items.Remove(checkedListBox1.SelectedItem);
                FireListViewChanged();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            var output = GetString(checkedListBox1.SelectedItem as string);
            if (string.IsNullOrEmpty(output) || Items.Contains(output))
                return;

            checkedListBox1.Items.Remove(checkedListBox1.SelectedItem);
            checkedListBox1.Items.Add(output);
            FireListViewChanged();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            FireListViewChanged(); //TODO fires before change happens
        }

        private void FireListViewChanged()
        {
            if (ListViewChanged != null)
                ListViewChanged(this);
        }

        private string GetString(string previous)
        {
            if (StringGetter != null)
                return StringGetter(previous);
            return null;
        }

        #endregion Methods
    }
}