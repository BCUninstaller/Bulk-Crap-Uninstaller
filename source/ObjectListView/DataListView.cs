/*
 * DataListView - A data-bindable listview
 *
 * Author: Phillip Piper
 * Date: 27/09/2008 9:15 AM
 *
 * Change log:
 * 2015-02-02   JPP  - Made Unfreezing more efficient by removing a redundant BuildList() call
 * v2.6
 * 2011-02-27   JPP  - Moved most of the logic to DataSourceAdapter (where it
 *                     can be used by FastDataListView too)
 * v2.3
 * 2009-01-18   JPP  - Boolean columns are now handled as checkboxes
 *                   - Auto-generated columns would fail if the data source was 
 *                     reseated, even to the same data source
 * v2.0.1
 * 2009-01-07   JPP  - Made all public and protected methods virtual 
 * 2008-10-03   JPP  - Separated from ObjectListView.cs
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{

    /// <summary>
    /// A DataListView is a ListView that can be bound to a datasource (which would normally be a DataTable or DataView).
    /// </summary>
    /// <remarks>
    /// <para>This listview keeps itself in sync with its source datatable by listening for change events.</para>
    /// <para>The DataListView will automatically create columns to show all of the data source's columns/properties, if there is not already
    /// a column showing that property. This allows you to define one or two columns in the designer and then have the others generated automatically.
    /// If you don't want any column to be auto generated, set <see cref="AutoGenerateColumns"/> to false.
    /// These generated columns will be only the simplest view of the world, and would look more interesting with a few delegates installed.</para>
    /// <para>This listview will also automatically generate missing aspect getters to fetch the values from the data view.</para>
    /// <para>Changing data sources is possible, but error prone. Before changing data sources, the programmer is responsible for modifying/resetting
    /// the column collection to be valid for the new data source.</para>
    /// <para>Internally, a CurrencyManager controls keeping the data source in-sync with other users of the data source (as per normal .NET
    /// behavior). This means that the model objects in the DataListView are DataRowView objects. If you write your own AspectGetters/Setters,
    /// they will be given DataRowView objects.</para>
    /// </remarks>
    public class DataListView : ObjectListView
    {
        #region Life and death

        /// <summary>
        /// Make a DataListView
        /// </summary>
        public DataListView()
        {
            Adapter = new DataSourceAdapter(this);
        }

        protected override void Dispose(bool disposing) {
            Adapter.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether or not columns will be automatically generated to show the
        /// columns when the DataSource is set. 
        /// </summary>
        /// <remarks>This must be set before the DataSource is set. It has no effect afterwards.</remarks>
        [Category("Data"),
         Description("Should the control automatically generate columns from the DataSource"),
         DefaultValue(true)]
        public bool AutoGenerateColumns {
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
        /// <para>When a DataSource is set, the control will create OLVColumns to show any
        /// data source columns that are not already shown.</para>
        /// <para>If the DataSource is changed, you will have to remove any previously
        /// created columns, since they will be configured for the previous DataSource.
        /// <see cref="ObjectListView.Reset()"/>.</para>
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
        /// <remarks>
        /// Adaptors cannot be shared between controls. Each DataListView needs its own adapter.
        /// </remarks>
        protected DataSourceAdapter Adapter {
            get {
                Debug.Assert(adapter != null, "Data adapter should not be null");
                return adapter; 
            }
            set { adapter = value; }
        }
        private DataSourceAdapter adapter;

        #endregion

        #region Object manipulations

        /// <summary>
        /// Add the given collection of model objects to this control.
        /// </summary>
        /// <param name="modelObjects">A collection of model objects</param>
        /// <remarks>This is a no-op for data lists, since the data
        /// is controlled by the DataSource. Manipulate the data source
        /// rather than this view of the data source.</remarks>
        public override void AddObjects(ICollection modelObjects)
        {
        }

        /// <summary>
        /// Insert the given collection of objects before the given position
        /// </summary>
        /// <param name="index">Where to insert the objects</param>
        /// <param name="modelObjects">The objects to be inserted</param>
        /// <remarks>This is a no-op for data lists, since the data
        /// is controlled by the DataSource. Manipulate the data source
        /// rather than this view of the data source.</remarks>
        public override void InsertObjects(int index, ICollection modelObjects) {
        }

        /// <summary>
        /// Remove the given collection of model objects from this control.
        /// </summary>
        /// <remarks>This is a no-op for data lists, since the data
        /// is controlled by the DataSource. Manipulate the data source
        /// rather than this view of the data source.</remarks>
        public override void RemoveObjects(ICollection modelObjects)
        {
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Change the Unfreeze behaviour 
        /// </summary>
        protected override void DoUnfreeze() {

            // Copied from base method, but we don't need to BuildList() since we know that our
            // data adaptor is going to do that immediately after this method exits.
            EndUpdate();
            ResizeFreeSpaceFillingColumns();
           // this.BuildList();
        }

        /// <summary>
        /// Handles parent binding context changes
        /// </summary>
        /// <param name="e">Unused EventArgs.</param>
        protected override void OnParentBindingContextChanged(EventArgs e)
        {
            base.OnParentBindingContextChanged(e);

            // BindingContext is an ambient property - by default it simply picks
            // up the parent control's context (unless something has explicitly
            // given us our own). So we must respond to changes in our parent's
            // binding context in the same way we would changes to our own
            // binding context.

            // THINK: Do we need to forward this to the adapter?
        }

        #endregion
    }
}
