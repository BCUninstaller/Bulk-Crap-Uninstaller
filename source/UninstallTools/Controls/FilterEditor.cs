/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Controls;
using Klocman.Localising;
using UninstallTools.Lists;

namespace UninstallTools.Controls
{
    public partial class FilterEditor : UserControl
    {
        public event EventHandler FocusSearchTarget
        {
            add { searchBox1.FocusSearchTarget += value; }
            remove { searchBox1.FocusSearchTarget -= value; }
        }

        private static readonly LocalisedEnumWrapper[] FilteringOptions;
        private static readonly Dictionary<string, ComparisonTargetInfo> PropertyTargets;
        private FilterCondition _targetFilterCondition;

        static FilterEditor()
        {
            FilteringOptions = Enum.GetValues(typeof(ComparisonMethod))
                .Cast<ComparisonMethod>().Select(x => new LocalisedEnumWrapper(x)).ToArray();

            PropertyTargets = new Dictionary<string, ComparisonTargetInfo>();
            foreach (var comparisonTarget in ComparisonTargetInfo.ComparisonTargets.OrderBy(x => x.DisplayName))
            {
                PropertyTargets.Add(comparisonTarget.DisplayName, comparisonTarget);
            }
        }

        public FilterEditor()
        {
            InitializeComponent();

            comboBoxCompareMethod.Items.AddRange(FilteringOptions.Cast<object>().ToArray());
            comboBoxCompareMethod.SelectedIndex = 0;

            comboBox1.Items.Add(ComparisonTargetInfo.AllTargetComparison.DisplayName);
            comboBox1.Items.AddRange(PropertyTargets.Keys.Cast<object>().ToArray());
            comboBox1.SelectedIndex = 0;

            var autoCompleteStringCollection = new AutoCompleteStringCollection();

            textBoxFilterText.AutoCompleteCustomSource = autoCompleteStringCollection;
            textBoxFilterText.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxFilterText.AutoCompleteSource = AutoCompleteSource.CustomSource;

            searchBox1.AutoCompleteCustomSource = autoCompleteStringCollection;
        }

        private ComparisonTargetInfo SelectedTargetInfo
        {
            get
            {
                var selection = comboBox1.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selection))
                    return ComparisonTargetInfo.AllTargetComparison;

                ComparisonTargetInfo c;
                return PropertyTargets.TryGetValue(selection, out c) ? c : ComparisonTargetInfo.AllTargetComparison;
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterCondition TargetFilterCondition
        {
            get
            {

                return _targetFilterCondition;
            }
            set
            {
                _targetFilterCondition = value;

                RefreshEditor();
                //event
            }
        }

        public void RefreshEditor()
        {
            if (_targetFilterCondition == null)
            {
                Enabled = false;
                textBoxFilterText.Text = string.Empty;
                checkBoxInvert.Checked = false;
                comboBox1.SelectedIndex = 0;
                comboBoxCompareMethod.SelectedIndex = 0;
                return;
            }
            Enabled = true;

            var option = FilteringOptions.FirstOrDefault(x => _targetFilterCondition.ComparisonMethod.Equals(x.TargetEnum));
            comboBoxCompareMethod.SelectedItem = option ?? FilteringOptions[0];

            comboBox1.SelectedIndex = string.IsNullOrEmpty(_targetFilterCondition.TargetPropertyId)
                ? 0
                : Math.Max(0, comboBox1.Items.IndexOf(PropertyTargets.First(x=>x.Value.Id.Equals(_targetFilterCondition.TargetPropertyId)).Key));

            if (textBoxFilterText.Text != _targetFilterCondition.FilterText)
                textBoxFilterText.Text = _targetFilterCondition.FilterText;

            if (searchBox1.SearchString != _targetFilterCondition.FilterText)
            {
                searchBox1.SearchTextChanged -= searchBox1_SearchTextChanged;
                searchBox1.Search(_targetFilterCondition.FilterText);
                searchBox1.SearchTextChanged += searchBox1_SearchTextChanged;
            }

            if (checkBoxInvert.Checked != _targetFilterCondition.InvertResults)
                checkBoxInvert.Checked = _targetFilterCondition.InvertResults;
        }

        [DefaultValue(false)]
        public bool ShowAsSearch
        {
            get { return searchBox1.Visible; }
            set
            {
                searchBox1.Visible = value;
                searchBox1.Enabled = value;
                labelText.Visible = !value;
                textBoxFilterText.Visible = !value;
                textBoxFilterText.Enabled = !value;
            }
        }


        /// <summary>
        /// Fires after any part of the edited comparison method is changed.
        /// </summary>
        public event EventHandler ComparisonMethodChanged;

        private void OnComparisonMethodChanged(object sender, EventArgs e)
        {
            if (!IsDisposed && !Disposing && Enabled)
                ComparisonMethodChanged?.Invoke(sender, e);
        }

        public void FocusSearchbox()
        {
            if (searchBox1.Visible)
                searchBox1.FocusSearchBox();
            else
                textBoxFilterText.Focus();
        }

        public void Search(string searchStr, ComparisonMethod method, string targetPropertyName = null, bool negate = false)
        {
            _targetFilterCondition.ComparisonMethod = method;
            _targetFilterCondition.InvertResults = negate;
            _targetFilterCondition.FilterText = searchStr;
            _targetFilterCondition.TargetPropertyId = targetPropertyName;

            searchBox1.SearchTextChanged -= searchBox1_SearchTextChanged;
            searchBox1.Search(searchStr);
            searchBox1.SearchTextChanged += searchBox1_SearchTextChanged;
            
            RefreshEditor();
            OnComparisonMethodChanged(this, EventArgs.Empty);
        }

        private void comboBoxCompareMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCompareMethod.SelectedItem is not LocalisedEnumWrapper localisedEnumWrapper || _targetFilterCondition == null
                                                                                                   || _targetFilterCondition.ComparisonMethod == (ComparisonMethod)localisedEnumWrapper.TargetEnum)
                return;

            _targetFilterCondition.ComparisonMethod = (ComparisonMethod)localisedEnumWrapper.TargetEnum;
            OnComparisonMethodChanged(sender, e);
        }

        private void searchBox1_SearchTextChanged(object sender, SearchBox.SearchEventArgs searchEventArgs)
        {
            if (_targetFilterCondition == null || _targetFilterCondition.FilterText == searchEventArgs.SearchText) return;

            _targetFilterCondition.FilterText = searchEventArgs.SearchText;
            OnComparisonMethodChanged(sender, searchEventArgs);
        }

        private void textBoxFilterText_TextChanged(object sender, EventArgs e)
        {
            if (_targetFilterCondition == null || _targetFilterCondition.FilterText == textBoxFilterText.Text) return;

            _targetFilterCondition.FilterText = textBoxFilterText.Text;
            OnComparisonMethodChanged(sender, e);
        }

        private void checkBoxInvert_CheckedChanged(object sender, EventArgs e)
        {
            if (_targetFilterCondition == null || _targetFilterCondition.InvertResults == checkBoxInvert.Checked) return;

            _targetFilterCondition.InvertResults = checkBoxInvert.Checked;
            OnComparisonMethodChanged(sender, e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_targetFilterCondition == null)
                return;

            textBoxFilterText.AutoCompleteCustomSource.Clear();
            var targetInfo = SelectedTargetInfo;
            if (targetInfo == null)
                return;

            if (targetInfo.PossibleStrings != null)
            {
                textBoxFilterText.AutoCompleteCustomSource.AddRange(SelectedTargetInfo.PossibleStrings);
                //textBoxFilterText.Text = SelectedTargetInfo.PossibleStrings.First();
            }

            if (_targetFilterCondition.TargetPropertyId
                .Equals(targetInfo.Id, StringComparison.InvariantCultureIgnoreCase)) return;

            _targetFilterCondition.TargetPropertyId = targetInfo.Id;
            OnComparisonMethodChanged(sender, e);
        }
    }
}