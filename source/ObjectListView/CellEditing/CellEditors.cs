/*
 * CellEditors - Several slightly modified controls that are used as celleditors within ObjectListView.
 *
 * Author: Phillip Piper
 * Date: 20/10/2008 5:15 PM
 *
 * Change log:
 * v2.6
 * 2012-08-02   JPP  - Make most editors public so they can be reused/subclassed
 * v2.3
 * 2009-08-13   JPP  - Standardized code formatting
 * v2.2.1
 * 2008-01-18   JPP  - Added special handling for enums
 * 2008-01-16   JPP  - Added EditorRegistry
 * v2.0.1
 * 2008-10-20   JPP  - Separated from ObjectListView.cs
 * 
 * Copyright (C) 2006-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// These items allow combo boxes to remember a value and its description.
    /// </summary>
    public class ComboBoxItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        public ComboBoxItem(Object key, String description) {
            this.key = key;
            this.description = description;
        }
        private readonly String description;

        /// <summary>
        /// 
        /// </summary>
        public Object Key {
            get { return key; }
        }
        private readonly Object key;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            return description;
        }
    } 

    //-----------------------------------------------------------------------
    // Cell editors
    // These classes are simple cell editors that make it easier to get and set
    // the value that the control is showing.
    // In many cases, you can intercept the CellEditStarting event to 
    // change the characteristics of the editor. For example, changing
    // the acceptable range for a numeric editor or changing the strings
    // that respresent true and false values for a boolean editor.

    /// <summary>
    /// This editor shows and auto completes values from the given listview column.
    /// </summary>
    [ToolboxItem(false)]
    public class AutoCompleteCellEditor : ComboBox
    {
        /// <summary>
        /// Create an AutoCompleteCellEditor
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="column"></param>
        public AutoCompleteCellEditor(ObjectListView lv, OLVColumn column) {
            DropDownStyle = ComboBoxStyle.DropDown;

            Dictionary<String, bool> alreadySeen = new Dictionary<string, bool>();
            for (int i = 0; i < Math.Min(lv.GetItemCount(), 1000); i++) {
                String str = column.GetStringValue(lv.GetModelObject(i));
                if (!alreadySeen.ContainsKey(str)) {
                    Items.Add(str);
                    alreadySeen[str] = true;
                }
            }

            Sorted = true;
            AutoCompleteSource = AutoCompleteSource.ListItems;
            AutoCompleteMode = AutoCompleteMode.Append;
        }
    }

    /// <summary>
    /// This combo box is specialised to allow editing of an enum.
    /// </summary>
    [ToolboxItem(false)]
    public class EnumCellEditor : ComboBox
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public EnumCellEditor(Type type) {
            DropDownStyle = ComboBoxStyle.DropDownList;
            ValueMember = "Key";

            ArrayList values = new ArrayList();
            foreach (object value in Enum.GetValues(type))
                values.Add(new ComboBoxItem(value, Enum.GetName(type, value)));

            DataSource = values;
        }
    }

    /// <summary>
    /// This editor simply shows and edits integer values.
    /// </summary>
    [ToolboxItem(false)]
    public class IntUpDown : NumericUpDown
    {
        /// <summary>
        /// 
        /// </summary>
        public IntUpDown() {
            DecimalPlaces = 0;
            Minimum = -9999999;
            Maximum = 9999999;
        }

        /// <summary>
        /// Gets or sets the value shown by this editor
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        new public int Value
        {
            get { return Decimal.ToInt32(base.Value); }
            set { base.Value = new Decimal(value); }
        }
    }

    /// <summary>
    /// This editor simply shows and edits unsigned integer values.
    /// </summary>
    /// <remarks>This class can't be made public because unsigned int is not a
    /// CLS-compliant type. If you want to use, just copy the code to this class
    /// into your project and use it from there.</remarks>
    [ToolboxItem(false)]
    internal class UintUpDown : NumericUpDown
    {
        public UintUpDown() {
            DecimalPlaces = 0;
            Minimum = 0;
            Maximum = 9999999;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        new public uint Value
        {
            get { return Decimal.ToUInt32(base.Value); }
            set { base.Value = new Decimal(value); }
        }
    }

    /// <summary>
    /// This editor simply shows and edits boolean values.
    /// </summary>
    [ToolboxItem(false)]
    public class BooleanCellEditor : ComboBox
    {
        /// <summary>
        /// 
        /// </summary>
        public BooleanCellEditor() {
            DropDownStyle = ComboBoxStyle.DropDownList;
            ValueMember = "Key";

            ArrayList values = new ArrayList();
            values.Add(new ComboBoxItem(false, "False"));
            values.Add(new ComboBoxItem(true, "True"));

            DataSource = values;
        }
    }

    /// <summary>
    /// This editor simply shows and edits boolean values using a checkbox
    /// </summary>
    [ToolboxItem(false)]
    public class BooleanCellEditor2 : CheckBox
    {
        /// <summary>
        /// Gets or sets the value shown by this editor
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool? Value {
            get {
                switch (CheckState) {
                    case CheckState.Checked: return true;
                    case CheckState.Indeterminate: return null;
                    case CheckState.Unchecked: 
                    default: return false;
                }
            }
            set {
                if (value.HasValue) 
                    CheckState = value.Value ? CheckState.Checked : CheckState.Unchecked;
                else
                    CheckState = CheckState.Indeterminate;
            }
        }

        /// <summary>
        /// Gets or sets how the checkbox will be aligned
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new HorizontalAlignment TextAlign
        {
            get {
                switch (CheckAlign) {
                    case ContentAlignment.MiddleRight: return HorizontalAlignment.Right;
                    case ContentAlignment.MiddleCenter: return HorizontalAlignment.Center;
                    case ContentAlignment.MiddleLeft: 
                    default: return HorizontalAlignment.Left;
                }
            }
            set {
                switch (value) {
                    case HorizontalAlignment.Left:
                        CheckAlign = ContentAlignment.MiddleLeft;
                        break;
                    case HorizontalAlignment.Center:
                        CheckAlign = ContentAlignment.MiddleCenter;
                        break;
                    case HorizontalAlignment.Right:
                        CheckAlign = ContentAlignment.MiddleRight;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// This editor simply shows and edits floating point values.
    /// </summary>
    /// <remarks>You can intercept the CellEditStarting event if you want
    /// to change the characteristics of the editor. For example, by increasing
    /// the number of decimal places.</remarks>
    [ToolboxItem(false)]
    public class FloatCellEditor : NumericUpDown
    {
        /// <summary>
        /// 
        /// </summary>
        public FloatCellEditor() {
            DecimalPlaces = 2;
            Minimum = -9999999;
            Maximum = 9999999;
        }

        /// <summary>
        /// Gets or sets the value shown by this editor
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        new public double Value
        {
            get { return Convert.ToDouble(base.Value); }
            set { base.Value = Convert.ToDecimal(value); }
        }
    }
}
