/*
 * FastDataListView - A data bindable listview that has the speed of a virtual list
 *
 * Author: Phillip Piper
 * Date: 22/09/2010 8:11 AM
 *
 * Change log:
 * 2015-02-02   JPP  - Made Unfreezing more efficient by removing a redundant BuildList() call
 * v2.6
 * 2010-09-22   JPP  - Initial version
 *
 * Copyright (C) 2006-2015 Phillip Piper
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
using System.Data;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A FastDataListView virtualizes the display of data from a DataSource. It operates on
    /// DataSets and DataTables in the same way as a DataListView, but does so much more efficiently.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A FastDataListView still has to load all its data from the DataSource. If you have SQL statement
    /// that returns 1 million rows, all 1 million rows will still need to read from the database.
    /// However, once the rows are loaded, the FastDataListView will only build rows as they are displayed.
    /// </para>
    /// </remarks>
    public class FastDataListView : FastObjectListView
    {
        protected override void Dispose(bool disposing)
        {
            if (adapter != null) {
                adapter.Dispose();
                adapter = null;
            }

            base.Dispose(disposing);
        }

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
        /// Get or set the VirtualListDataSource that will be displayed in this list view.
        /// </summary>
        /// <remarks>The VirtualListDataSource should implement either <see cref="IList"/>, <see cref="IBindingList"/>,
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
        /// VirtualListDataSource refers to a list, since <see cref="DataMember"/> can
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual string DataMember
        {
            get { return Adapter.DataMember; }
            set { Adapter.DataMember = value; }
        }

        #endregion

        #region Implementation properties

        /// <summary>
        /// Gets or sets the DataSourceAdaptor that does the bulk of the work needed
        /// for data binding.
        /// </summary>
        protected DataSourceAdapter Adapter {
            get {
                if (adapter == null)
                    adapter = CreateDataSourceAdapter();
                return adapter;
            }
            set { adapter = value; }
        }
        private DataSourceAdapter adapter;

        #endregion

        #region Implementation 

        /// <summary>
        /// Create the DataSourceAdapter that this control will use.
        /// </summary>
        /// <returns>A DataSourceAdapter configured for this list</returns>
        /// <remarks>Subclasses should override this to create their
        /// own specialized adapters</remarks>
        protected virtual DataSourceAdapter CreateDataSourceAdapter() {
            return new DataSourceAdapter(this);
        }

        /// <summary>
        /// Change the Unfreeze behaviour 
        /// </summary>
        protected override void DoUnfreeze()
        {

            // Copied from base method, but we don't need to BuildList() since we know that our
            // data adaptor is going to do that immediately after this method exits.
            EndUpdate();
            ResizeFreeSpaceFillingColumns();
            // this.BuildList();
        }

        #endregion
    }
}
