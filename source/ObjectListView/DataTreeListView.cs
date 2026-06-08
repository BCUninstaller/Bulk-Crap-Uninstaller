/*
 * DataTreeListView - A data bindable TreeListView
 *
 * Author: Phillip Piper
 * Date: 05/05/2012 3:26 PM
 *
 * Change log:

 * 2012-05-05  JPP  Initial version
 *
 * TO DO:

 * 
 * Copyright (C) 2012 Phillip Piper
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
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A DataTreeListView is a TreeListView that calculates its hierarchy based on
    /// information in the data source.
    /// </summary>
    /// <remarks>
    /// <para>Like a <see cref="DataListView"/>, a DataTreeListView sources all its information
    /// from a combination of <see cref="DataSource"/> and <see cref="DataMember"/>. 
    /// <see cref="DataSource"/> can be a DataTable, DataSet,
    /// or anything that implements <see cref="IList"/>. 
    /// </para>
    /// <para>
    /// To function properly, the DataTreeListView requires:
    /// <list type="bullet">
    /// <item>the table to have a column which holds a unique for the row. The name of this column must be set in <see cref="KeyAspectName"/>.</item>
    /// <item>the table to have a column which holds id of the hierarchical parent of the row. The name of this column must be set in <see cref="ParentKeyAspectName"/>.</item>
    /// <item>a value which identifies which rows are the roots of the tree (<see cref="RootKeyValue"/>).</item>
    /// </list>
    /// The hierarchy structure is determined finding all the rows where the parent key is equal to <see cref="RootKeyValue"/>. These  rows
    /// become the root objects of the hierarchy.
    /// </para>
    /// <para>Like a TreeListView, the hierarchy must not contain cycles. Bad things will happen if the data is cyclic.</para>
    /// </remarks>
    public partial class DataTreeListView : TreeListView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets whether or not columns will be automatically generated to show the
        /// columns when the DataSource is set. 
        /// </summary>
        /// <remarks>This must be set before the DataSource is set. It has no effect afterwards.</remarks>
        [Category("Data"),
         Description("Should the control automatically generate columns from the DataSource"),
         DefaultValue(true)]
        public bool AutoGenerateColumns
        {
            get { return Adapter.AutoGenerateColumns; }
            set { Adapter.AutoGenerateColumns = value; }
        }

        /// <summary>
        /// Get or set the DataSource that will be displayed in this list view.
        /// </summary>
        /// <remarks>The DataSource should implement either <see cref="IList"/>, <see cref="IBindingList"/>,
        /// or <see cref="IListSource"/>. Some common examples are the following types of objects:
        /// <list type="unordered">
        /// <item><description><see cref="DataView"/></description></item>
        /// <item><description><see cref="DataTable"/></description></item>
        /// <item><description><see cref="DataSet"/></description></item>
        /// <item><description><see cref="DataViewManager"/></description></item>
        /// <item><description><see cref="BindingSource"/></description></item>
        /// </list>
        /// <para>When binding to a list container (i.e. one that implements the
        /// <see cref="IListSource"/> interface, such as <see cref="DataSet"/>)
        /// you must also set the <see cref="DataMember"/> property in order
        /// to identify which particular list you would like to display. You
        /// may also set the <see cref="DataMember"/> property even when
        /// DataSource refers to a list, since <see cref="DataMember"/> can
        /// also be used to navigate relations between lists.</para>
        /// </remarks>
        [Category("Data"),
        TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Object DataSource
        {
            get { return Adapter.DataSource; }
            set { Adapter.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the DataListView is displaying data.
        /// </summary>
        /// <remarks>If the data source is not a DataSet or DataViewManager, this property has no effect</remarks>
        [Category("Data"),
         Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(UITypeEditor)),
         DefaultValue("")]
        public virtual string DataMember {
            get { return Adapter.DataMember; }
            set { Adapter.DataMember = value; }
        }

        /// <summary>
        /// Gets or sets the name of the property/column that uniquely identifies each row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value contained by this column must be unique across all rows 
        /// in the data source. Odd and unpredictable things will happen if two
        /// rows have the same id.
        /// </para>
        /// <para>Null cannot be a valid key value.</para>
        /// </remarks>
        [Category("Data"),
         Description("The name of the property/column that holds the key of a row"),
         DefaultValue(null)]
        public virtual string KeyAspectName {
            get { return Adapter.KeyAspectName; }
            set { Adapter.KeyAspectName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the property/column that contains the key of
        /// the parent of a row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test condition for deciding if one row is the parent of another is functionally
        /// equivilent to this:
        /// <code>
        /// Object.Equals(candidateParentRow[this.KeyAspectName], row[this.ParentKeyAspectName])
        /// </code>
        /// </para>
        /// <para>Unlike key value, parent keys can be null but a null parent key can only be used
        /// to identify root objects.</para>
        /// </remarks>
        [Category("Data"),
         Description("The name of the property/column that holds the key of the parent of a row"),
         DefaultValue(null)]
        public virtual string ParentKeyAspectName {
            get { return Adapter.ParentKeyAspectName; }
            set { Adapter.ParentKeyAspectName = value; }
        }

        /// <summary>
        /// Gets or sets the value that identifies a row as a root object.
        /// When the ParentKey of a row equals the RootKeyValue, that row will
        /// be treated as root of the TreeListView.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test condition for deciding a root object is functionally
        /// equivilent to this:
        /// <code>
        /// Object.Equals(candidateRow[this.ParentKeyAspectName], this.RootKeyValue)
        /// </code>
        /// </para>
        /// <para>The RootKeyValue can be null. Actually, it can be any value that can 
        /// be compared for equality against a basic type.</para>
        /// <para>If this is set to the wrong value (i.e. to a value that no row
        /// has in the parent id column), the list will be empty.</para>
        /// </remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object RootKeyValue {
            get { return Adapter.RootKeyValue; }
            set { Adapter.RootKeyValue = value; }
        }

        /// <summary>
        /// Gets or sets the value that identifies a row as a root object.
        /// <see cref="RootKeyValue"/>. The RootKeyValue can be of any type,
        /// but the IDE cannot sensibly represent a value of any type,
        /// so this is a typed wrapper around that property.
        /// </summary>
        /// <remarks>
        /// If you want the root value to be something other than a string,
        /// you will have set it yourself.
        /// </remarks>
        [Category("Data"),
        Description("The parent id value that identifies a row as a root object"),
        DefaultValue(null)]
        public virtual string RootKeyValueString {
            get { return Convert.ToString(Adapter.RootKeyValue); }
            set { Adapter.RootKeyValue = value; }
        }

        /// <summary>
        /// Gets or sets whether or not the key columns (id and parent id) should
        /// be shown to the user.
        /// </summary>
        /// <remarks>This must be set before the DataSource is set. It has no effect
        /// afterwards.</remarks>
        [Category("Data"),
         Description("Should the keys columns (id and parent id) be shown to the user?"),
         DefaultValue(true)]
        public virtual bool ShowKeyColumns {
            get { return Adapter.ShowKeyColumns; }
            set { Adapter.ShowKeyColumns = value; }
        }

        #endregion

        #region Implementation properties

        /// <summary>
        /// Gets or sets the DataSourceAdaptor that does the bulk of the work needed
        /// for data binding.
        /// </summary>
        protected TreeDataSourceAdapter Adapter {
            get {
                if (adapter == null)
                    adapter = new TreeDataSourceAdapter(this);
                return adapter;
            }
            set { adapter = value; }
        }
        private TreeDataSourceAdapter adapter;

        #endregion
    }
}
