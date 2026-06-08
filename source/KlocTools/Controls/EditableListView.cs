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
    public partial class EditableListView : UserControl
    {
        #region Delegates

        public delegate string GetStringDelegate(string previous);

        #endregion Delegates

        #region Constructors

        public EditableListView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        public event Action<EditableListView> ListViewChanged;

        #endregion Events

        #region Properties

        public IEnumerable<string> Items
        {
            get { return listBox.Items.Cast<string>(); }
        }

        [Description("Sorting of the list box enabled/disabled"),
         Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ListBoxSorted
        {
            get { return listBox.Sorted; }
            set { listBox.Sorted = value; }
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

        public void AddRange(object[] items)
        {
            listBox.Items.AddRange(items);
            FireListViewChanged();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var output = GetString(null);
            if (string.IsNullOrEmpty(output) || Items.Contains(output))
                return;

            listBox.Items.Add(output);
            FireListViewChanged();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                listBox.Items.Remove(listBox.SelectedItem);
                FireListViewChanged();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            var output = GetString(listBox.SelectedItem as string);
            if (string.IsNullOrEmpty(output) || Items.Contains(output))
                return;

            listBox.Items.Remove(listBox.SelectedItem);
            listBox.Items.Add(output);
            FireListViewChanged();
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