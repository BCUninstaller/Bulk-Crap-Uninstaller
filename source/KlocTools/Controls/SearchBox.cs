/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Controls
{
    public partial class SearchBox : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color InactiveSearchColor
        {
            get { return _inactiveSearchColor; }
            set
            {
                _inactiveSearchColor = value;
                UpdateFieldColor();
            }
        }

        private void UpdateFieldColor()
        {
            if (filteringTextBox == null) return;

            filteringTextBox.ForeColor = string.IsNullOrEmpty(SearchString) ? _inactiveSearchColor : _normalSearchColor;

            filteringTextBox.BackColor = base.BackColor;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BorderStyle SearchBoxBorderStyle
        {
            get { return filteringTextBox.BorderStyle; }
            set { filteringTextBox.BorderStyle = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color NormalSearchColor
        {
            get { return _normalSearchColor; }
            set
            {
                _normalSearchColor = value;
                UpdateFieldColor();
            }
        }

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                UpdateFieldColor();
            }
        }

        private static string _inactiveSearchText;
        private Color _inactiveSearchColor;
        private Color _normalSearchColor;

        public SearchBox()
        {
            InactiveSearchColor = SystemColors.GrayText;
            NormalSearchColor = SystemColors.WindowText;

            InitializeComponent();
            _inactiveSearchText = filteringTextBox.Text;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return filteringTextBox.AutoCompleteCustomSource; }
            set { filteringTextBox.AutoCompleteCustomSource = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string[] SearchParts
        {
            get
            {
                if (string.IsNullOrEmpty(filteringTextBox.Text) || filteringTextBox.Text.Equals(_inactiveSearchText))
                    return new string[] { };
                return filteringTextBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string SearchString { get; private set; } = string.Empty;

        public event EventHandler FocusSearchTarget;
        public event EventHandler<SearchEventArgs> SearchTextChanged;

        public void ClearSearchBox()
        {
            filteringTextBox.Text = _inactiveSearchText;
            filteringTextBox.ForeColor = InactiveSearchColor;
        }

        private void filteringTextBox_Enter(object sender, EventArgs e)
        {
            FocusSearchBox();
        }

        private void filteringTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Stop the Ding noise
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                default:
                    break;
            }
        }

        private void filteringTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    ClearSearchBox();
                    OnFocusSearchTarget();
                    break;

                default:
                    e.Handled = false;
                    e.SuppressKeyPress = false;
                    break;
            }
        }

        private void filteringTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filteringTextBox.Text))
                ClearSearchBox();
        }

        private void filteringTextBox_TextChanged(object sender, EventArgs e)
        {
            var text = filteringTextBox.Text;
            if (_inactiveSearchText.Equals(text))
                text = string.Empty;

            if (!text.Equals(SearchString))
            {
                SearchString = text;
                OnSearchTextChanged();
            }
        }

        public void FocusSearchBox()
        {
            filteringTextBox.Focus();
            filteringTextBox.ForeColor = NormalSearchColor;

            if (_inactiveSearchText.Equals(filteringTextBox.Text))
                filteringTextBox.Text = string.Empty;

            filteringTextBox.SelectAll();
        }

        protected virtual void OnFocusSearchTarget()
        {
            if (FocusSearchTarget != null)
                FocusSearchTarget(this, EventArgs.Empty);
            else
                Parent.Focus();
        }

        protected virtual void OnSearchTextChanged()
        {
            SearchTextChanged?.Invoke(this, new SearchEventArgs(this, SearchString));
        }

        public void Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString) || searchString.Equals(_inactiveSearchText))
                ClearSearchBox();
            else
            {
                filteringTextBox.Text = searchString;
                filteringTextBox.ForeColor = NormalSearchColor;
            }
        }

        public sealed class SearchEventArgs : EventArgs
        {
            public SearchEventArgs(SearchBox origin, string searchText)
            {
                Origin = origin;
                SearchText = searchText;
            }

            public SearchBox Origin { get; }

            public string SearchText { get; }
        }
    }
}