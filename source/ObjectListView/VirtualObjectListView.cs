/*
 * VirtualObjectListView - A virtual listview delays fetching model objects until they are actually displayed.
 *
 * Author: Phillip Piper
 * Date: 27/09/2008 9:15 AM
 *
 * Change log:
 * 2015-06-14   JPP  - Moved handling of CheckBoxes on virtual lists into base class (ObjectListView).
 *                     This allows the property to be set correctly, even when set via an upcast reference.
 * 2015-03-25   JPP  - Subscribe to change notifications when objects are added
 * v2.8
 * 2014-09-26   JPP  - Correct an incorrect use of checkStateMap when setting CheckedObjects
 *                     and a CheckStateGetter is installed
 * v2.6
 * 2012-06-13   JPP  - Corrected several bugs related to groups on virtual lists.
 *                   - Added EnsureNthGroupVisible() since EnsureGroupVisible() can't work on virtual lists.
 * v2.5.1
 * 2012-05-04   JPP  - Avoid bug/feature in ListView.VirtalListSize setter that causes flickering
 *                     when the size of the list changes.
 * 2012-04-24   JPP  - Fixed bug that occurred when adding/removing item while the view was grouped.
 * v2.5
 * 2011-05-31   JPP  - Setting CheckedObjects is more efficient on large collections
 * 2011-04-05   JPP  - CheckedObjects now only returns objects that are currently in the list.
 *                     ClearObjects() now resets all check state info.
 * 2011-03-31   JPP  - Filtering on grouped virtual lists no longer behaves strangely.
 * 2011-03-17   JPP  - Virtual lists can (finally) set CheckBoxes back to false if it has been set to true.
 *                     (this is a little hacky and may not work reliably).
 *                   - GetNextItem() and GetPreviousItem() now work on grouped virtual lists.
 * 2011-03-08   JPP  - BREAKING CHANGE: 'DataSource' was renamed to 'VirtualListDataSource'. This was necessary
 *                     to allow FastDataListView which is both a DataListView AND a VirtualListView --
 *                     which both used a 'DataSource' property :(
 * v2.4
 * 2010-04-01   JPP  - Support filtering
 * v2.3
 * 2009-08-28   JPP  - BIG CHANGE. Virtual lists can now have groups!
 *                   - Objects property now uses "yield return" -- much more efficient for big lists
 * 2009-08-07   JPP  - Use new scheme for formatting rows/cells
 * v2.2.1
 * 2009-07-24   JPP  - Added specialised version of RefreshSelectedObjects() which works efficiently with virtual lists
 *                     (thanks to chriss85 for finding this bug)
 * 2009-07-03   JPP  - Standardized code format
 * v2.2
 * 2009-04-06   JPP  - ClearObjects() now works again
 * v2.1
 * 2009-02-24   JPP  - Removed redundant OnMouseDown() since checkbox
 *                     handling is now handled in the base class
 * 2009-01-07   JPP  - Made all public and protected methods virtual 
 * 2008-12-07   JPP  - Trigger Before/AfterSearching events
 * 2008-11-15   JPP  - Fixed some caching issues
 * 2008-11-05   JPP  - Rewrote handling of check boxes
 * 2008-10-28   JPP  - Handle SetSelectedObjects(null)
 * 2008-10-02   JPP  - MAJOR CHANGE: Use IVirtualListDataSource
 * 2008-09-27   JPP  - Separated from ObjectListView.cs
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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A virtual object list view operates in virtual mode, that is, it only gets model objects for
    /// a row when it is needed. This gives it the ability to handle very large numbers of rows with
    /// minimal resources.
    /// </summary>
    /// <remarks><para>A listview is not a great user interface for a large number of items. But if you've
    /// ever wanted to have a list with 10 million items, go ahead, knock yourself out.</para>
    /// <para>Virtual lists can never iterate their contents. That would defeat the whole purpose.</para>
    /// <para>Animated GIFs should not be used in virtual lists. Animated GIFs require some state
    /// information to be stored for each animation, but virtual lists specifically do not keep any state information.
    /// In any case, you really do not want to keep state information for 10 million animations!</para>
    /// <para>
    /// Although it isn't documented, .NET virtual lists cannot have checkboxes. This class codes around this limitation,
    /// but you must use the functions provided by ObjectListView: CheckedObjects, CheckObject(), UncheckObject() and their friends. 
    /// If you use the normal check box properties (CheckedItems or CheckedIndicies), they will throw an exception, since the
    /// list is in virtual mode, and .NET "knows" it can't handle checkboxes in virtual mode.
    /// </para>
    /// <para>Due to the limits of the underlying Windows control, virtual lists do not trigger ItemCheck/ItemChecked events. 
    /// Use a CheckStatePutter instead.</para>
    /// <para>To enable grouping, you must provide an implmentation of IVirtualGroups interface, via the GroupingStrategy property.</para>
    /// <para>Similarly, to enable filtering on the list, your VirtualListDataSource must also implement the IFilterableDataSource interface.</para>
    /// </remarks>
    public class VirtualObjectListView : ObjectListView
    {
        /// <summary>
        /// Create a VirtualObjectListView
        /// </summary>
        public VirtualObjectListView()
            : base() {
            VirtualMode = true; // Virtual lists have to be virtual -- no prizes for guessing that :)

            CacheVirtualItems += new CacheVirtualItemsEventHandler(HandleCacheVirtualItems);
            RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(HandleRetrieveVirtualItem);
            SearchForVirtualItem += new SearchForVirtualItemEventHandler(HandleSearchForVirtualItem);

            // At the moment, we don't need to handle this event. But we'll keep this comment to remind us about it.
            //this.VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(VirtualObjectListView_VirtualItemsSelectionRangeChanged);

            VirtualListDataSource = new VirtualListVersion1DataSource(this);

            // Virtual lists have to manage their own check state, since the normal ListView control 
            // doesn't even allow checkboxes on virtual lists
            PersistentCheckBoxes = true;
        }

        #region Public Properties

        /// <summary>
        /// Gets whether or not this listview is capabale of showing groups
        /// </summary>
        [Browsable(false)]
        public override bool CanShowGroups {
            get {
                // Virtual lists need Vista and a grouping strategy to show groups
                return (IsVistaOrLater && GroupingStrategy != null);
            }
        }

        /// <summary>
        /// Get or set the collection of model objects that are checked.
        /// When setting this property, any row whose model object isn't
        /// in the given collection will be unchecked. Setting to null is
        /// equivilent to unchecking all.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property returns a simple collection. Changes made to the returned
        /// collection do NOT affect the list. This is different to the behaviour of
        /// CheckedIndicies collection.
        /// </para>
        /// <para>
        /// When getting CheckedObjects, the performance of this method is O(n) where n is the number of checked objects.
        /// When setting CheckedObjects, the performance of this method is O(n) where n is the number of checked objects plus
        /// the number of objects to be checked.
        /// </para>
        /// <para>
        /// If the ListView is not currently showing CheckBoxes, this property does nothing. It does
        /// not remember any check box settings made.
        /// </para>
        /// <para>
        /// This class optimizes the management of CheckStates so that it will work efficiently even on
        /// large lists of item. However, those optimizations are impossible if you install a CheckStateGetter.
        /// With a CheckStateGetter installed, the performance of this method is O(n) where n is the size 
        /// of the list. This could be painfully slow.</para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IList CheckedObjects {
            get {
                // If we aren't should checkboxes, then no objects can be checked
                if (!CheckBoxes)
                    return new ArrayList();

                // If the data source has somehow vanished, we can't do anything
                if (VirtualListDataSource == null)
                    return new ArrayList();

                // If a custom check state getter is install, we can't use our check state management
                // We have to use the (slower) base version.
                if (CheckStateGetter != null)
                    return base.CheckedObjects;

                // Collect items that are checked AND that still exist in the list.
                ArrayList objects = new ArrayList();
                foreach (KeyValuePair<Object, CheckState> kvp in CheckStateMap)
                {
                    if (kvp.Value == CheckState.Checked && 
                        (!CheckedObjectsMustStillExistInList ||
                         VirtualListDataSource.GetObjectIndex(kvp.Key) >= 0))
                        objects.Add(kvp.Key);
                }
                return objects;
            }
            set {
                if (!CheckBoxes)
                    return;

                // If a custom check state getter is install, we can't use our check state management
                // We have to use the (slower) base version.
                if (CheckStateGetter != null) {
                    base.CheckedObjects = value;
                    return;
                }

                Stopwatch sw = Stopwatch.StartNew();

                // Set up an efficient way of testing for the presence of a particular model
                Hashtable table = new Hashtable(GetItemCount());
                if (value != null) {
                    foreach (object x in value)
                        table[x] = true;
                }

                BeginUpdate();

                // Uncheck anything that is no longer checked
                Object[] keys = new Object[CheckStateMap.Count];
                CheckStateMap.Keys.CopyTo(keys, 0);
                foreach (Object key in keys) {
                    if (!table.Contains(key))
                        SetObjectCheckedness(key, CheckState.Unchecked);
                }

                // Check all the new checked objects
                foreach (Object x in table.Keys)
                    SetObjectCheckedness(x, CheckState.Checked);

                EndUpdate();

                Debug.WriteLine(String.Format("PERF - Setting virtual CheckedObjects on {2} objects took {0}ms / {1} ticks", sw.ElapsedMilliseconds, sw.ElapsedTicks, GetItemCount()));
            }
        }

        /// <summary>
        /// Gets or sets whether or not an object will be included in the CheckedObjects
        /// collection, even if it is not present in the control at the moment
        /// </summary>
        /// <remarks>
        /// This property is an implementation detail and should not be altered.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        protected internal bool CheckedObjectsMustStillExistInList
        {
            get { return checkedObjectsMustStillExistInList; }
            set { checkedObjectsMustStillExistInList = value; }
        }
        private bool checkedObjectsMustStillExistInList = true;

        /// <summary>
        /// Gets the collection of objects that survive any filtering that may be in place.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable FilteredObjects {
            get {
                for (int i = 0; i < GetItemCount(); i++)
                    yield return GetModelObject(i);
            }
        }

        /// <summary>
        /// Gets or sets the strategy that will be used to create groups
        /// </summary>
        /// <remarks>
        /// This must be provided for a virtual list to show groups.
        /// </remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVirtualGroups GroupingStrategy {
            get { return groupingStrategy; }
            set { groupingStrategy = value; }
        }
        private IVirtualGroups groupingStrategy;

        /// <summary>
        /// Gets whether or not the current list is filtering its contents
        /// </summary>
        /// <remarks>
        /// This is only possible if our underlying data source supports filtering.
        /// </remarks>
        public override bool IsFiltering {
            get {
                return base.IsFiltering && (VirtualListDataSource is IFilterableDataSource);
            }
        }

        /// <summary>
        /// Get/set the collection of objects that this list will show
        /// </summary>
        /// <remarks>
        /// <para>
        /// The contents of the control will be updated immediately after setting this property.
        /// </para>
        /// <para>Setting this property preserves selection, if possible. Use SetObjects() if
        /// you do not want to preserve the selection. Preserving selection is the slowest part of this
        /// code -- performance is O(n) where n is the number of selected rows.</para>
        /// <para>This method is not thread safe.</para>
        /// <para>The property DOES work on virtual lists, but if you try to iterate through a list 
        /// of 10 million objects, it may take some time :)</para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable Objects {
            get {
                IFilterableDataSource filterable = VirtualListDataSource as IFilterableDataSource;
                try {
                    // If we are filtering, we have to temporarily disable filtering so we get
                    // the whole collection
                    if (filterable != null && UseFiltering)
                        filterable.ApplyFilters(null, null);
                    return FilteredObjects;
                } finally {
                    if (filterable != null && UseFiltering)
                        filterable.ApplyFilters(ModelFilter, ListFilter);
                }
            }
            set { base.Objects = value; }
        }

        /// <summary>
        /// This delegate is used to fetch a rowObject, given it's index within the list
        /// </summary>
        /// <remarks>Only use this property if you are not using a VirtualListDataSource.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RowGetterDelegate RowGetter {
            get { return ((VirtualListVersion1DataSource)virtualListDataSource).RowGetter; }
            set { ((VirtualListVersion1DataSource)virtualListDataSource).RowGetter = value; }
        }

        /// <summary>
        /// Should this list show its items in groups?
        /// </summary>
        [Category("Appearance"),
         Description("Should the list view show items in groups?"),
         DefaultValue(true)]
        override public bool ShowGroups {
            get {
                // Pre-Vista, virtual lists cannot show groups
                return IsVistaOrLater && showGroups;
            }
            set {
                showGroups = value;
                if (Created && !value) 
                    DisableVirtualGroups();
            }
        }
        private bool showGroups;


        /// <summary>
        /// Get/set the data source that is behind this virtual list
        /// </summary>
        /// <remarks>Setting this will cause the list to redraw.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IVirtualListDataSource VirtualListDataSource {
            get {
                return virtualListDataSource;
            }
            set {
                virtualListDataSource = value;
                CustomSorter = delegate(OLVColumn column, SortOrder sortOrder) {
                    ClearCachedInfo();
                    virtualListDataSource.Sort(column, sortOrder);
                };
                BuildList(false);
            }
        }
        private IVirtualListDataSource virtualListDataSource;

        /// <summary>
        /// Gets or sets the number of rows in this virtual list.
        /// </summary>
        /// <remarks>
        /// There is an annoying feature/bug in the .NET ListView class. 
        /// When you change the VirtualListSize property, it always scrolls so
        /// that the focused item is the top item. This is annoying since it makes
        /// the virtual list seem to flicker as the control scrolls to show the focused
        /// item and then scrolls back to where ObjectListView wants it to be.
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected new virtual int VirtualListSize {
            get { return base.VirtualListSize; }
            set {
                if (value == VirtualListSize || value < 0)
                    return;

                // Get around the 'private' marker on 'virtualListSize' field using reflection
                if (virtualListSizeFieldInfo == null) {
                    virtualListSizeFieldInfo = typeof(ListView).GetField("virtualListSize", BindingFlags.NonPublic | BindingFlags.Instance);
                    Debug.Assert(virtualListSizeFieldInfo != null);
                }

                // Set the base class private field so that it keeps on working
                virtualListSizeFieldInfo.SetValue(this, value);

                // Send a raw message to change the virtual list size *without* changing the scroll position
                if (IsHandleCreated && !DesignMode)
                    NativeMethods.SetItemCount(this, value);
            }
        }
        static private FieldInfo virtualListSizeFieldInfo;

        #endregion

        #region OLV accessing

        /// <summary>
        /// Return the number of items in the list
        /// </summary>
        /// <returns>the number of items in the list</returns>
        public override int GetItemCount() {
            return VirtualListSize;
        }

        /// <summary>
        /// Return the model object at the given index
        /// </summary>
        /// <param name="index">Index of the model object to be returned</param>
        /// <returns>A model object</returns>
        public override object GetModelObject(int index) {
            if (VirtualListDataSource != null && index >= 0 && index < GetItemCount())
                return VirtualListDataSource.GetNthObject(index);
            else
                return null;
        }

        /// <summary>
        /// Find the given model object within the listview and return its index
        /// </summary>
        /// <param name="modelObject">The model object to be found</param>
        /// <returns>The index of the object. -1 means the object was not present</returns>
        public override int IndexOf(Object modelObject) {
            if (VirtualListDataSource == null || modelObject == null)
                return -1;

            return VirtualListDataSource.GetObjectIndex(modelObject);
        }

        /// <summary>
        /// Return the OLVListItem that displays the given model object
        /// </summary>
        /// <param name="modelObject">The modelObject whose item is to be found</param>
        /// <returns>The OLVListItem that displays the model, or null</returns>
        /// <remarks>This method has O(n) performance.</remarks>
        public override OLVListItem ModelToItem(object modelObject) {
            if (VirtualListDataSource == null || modelObject == null)
                return null;

            int index = VirtualListDataSource.GetObjectIndex(modelObject);
            return index >= 0 ? GetItem(index) : null;
        }

        #endregion

        #region Object manipulation

        /// <summary>
        /// Add the given collection of model objects to this control.
        /// </summary>
        /// <param name="modelObjects">A collection of model objects</param>
        /// <remarks>
        /// <para>The added objects will appear in their correct sort position, if sorting
        /// is active. Otherwise, they will appear at the end of the list.</para>
        /// <para>No check is performed to see if any of the objects are already in the ListView.</para>
        /// <para>Null objects are silently ignored.</para>
        /// </remarks>
        public override void AddObjects(ICollection modelObjects) {
            if (VirtualListDataSource == null)
                return;

            // Give the world a chance to cancel or change the added objects
            ItemsAddingEventArgs args = new ItemsAddingEventArgs(modelObjects);
            OnItemsAdding(args);
            if (args.Canceled)
                return;

            try
            {
                BeginUpdate();
                VirtualListDataSource.AddObjects(args.ObjectsToAdd);
                BuildList();
                SubscribeNotifications(args.ObjectsToAdd);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Remove all items from this list
        /// </summary>
        /// <remark>This method can safely be called from background threads.</remark>
        public override void ClearObjects() {
            if (InvokeRequired)
                Invoke(new MethodInvoker(ClearObjects));
            else {
                CheckStateMap.Clear();
                SetObjects(new ArrayList());
            }
        }

        /// <summary>
        /// Scroll the listview so that the given group is at the top.
        /// </summary>
        /// <param name="groupIndex">The index of the group to be revealed</param>
        /// <remarks><para>
        /// If the group is already visible, the list will still be scrolled to move
        /// the group to the top, if that is possible.
        /// </para>
        /// <para>This only works when the list is showing groups (obviously).</para>
        /// </remarks>
        public virtual void EnsureNthGroupVisible(int groupIndex) {
            if (!ShowGroups)
                return;

            if (groupIndex <= 0 || groupIndex >= OLVGroups.Count) {
                // There is no easy way to scroll back to the beginning of the list
                int delta = 0 - NativeMethods.GetScrollPosition(this, false);
                NativeMethods.Scroll(this, 0, delta);
            } else {
                // Find the display rectangle of the last item in the previous group
                OLVGroup previousGroup = OLVGroups[groupIndex - 1];
                int lastItemInGroup = GroupingStrategy.GetGroupMember(previousGroup, previousGroup.VirtualItemCount - 1);
                Rectangle r = GetItemRect(lastItemInGroup);

                // Scroll so that the last item of the previous group is just out of sight,
                // which will make the desired group header visible.
                int delta = r.Y + r.Height / 2;
                NativeMethods.Scroll(this, 0, delta);
            }
        }

        /// <summary>
        /// Inserts the given collection of model objects to this control at hte given location
        /// </summary>
        /// <param name="modelObjects">A collection of model objects</param>
        /// <remarks>
        /// <para>The added objects will appear in their correct sort position, if sorting
        /// is active. Otherwise, they will appear at the given position of the list.</para>
        /// <para>No check is performed to see if any of the objects are already in the ListView.</para>
        /// <para>Null objects are silently ignored.</para>
        /// </remarks>
        public override void InsertObjects(int index, ICollection modelObjects)
        {
            if (VirtualListDataSource == null)
                return;

            // Give the world a chance to cancel or change the added objects
            ItemsAddingEventArgs args = new ItemsAddingEventArgs(index, modelObjects);
            OnItemsAdding(args);
            if (args.Canceled)
                return;

            try
            {
                BeginUpdate();
                VirtualListDataSource.InsertObjects(index, args.ObjectsToAdd);
                BuildList();
                SubscribeNotifications(args.ObjectsToAdd);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Update the rows that are showing the given objects
        /// </summary>
        /// <remarks>This method does not resort the items.</remarks>
        public override void RefreshObjects(IList modelObjects) {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(() => RefreshObjects(modelObjects)));
                return;
            }

            // Without a data source, we can't do this.
            if (VirtualListDataSource == null)
                return;

            try {
                BeginUpdate();
                ClearCachedInfo();
                foreach (object modelObject in modelObjects) {
                    int index = VirtualListDataSource.GetObjectIndex(modelObject);
                    if (index >= 0) {
                        VirtualListDataSource.UpdateObject(index, modelObject);
                        RedrawItems(index, index, true);
                    }
                }
            }
            finally {
                EndUpdate();
            }
        }

        /// <summary>
        /// Update the rows that are selected
        /// </summary>
        /// <remarks>This method does not resort or regroup the view.</remarks>
        public override void RefreshSelectedObjects() {
            foreach (int index in SelectedIndices)
                RedrawItems(index, index, true);
        }

        /// <summary>
        /// Remove all of the given objects from the control
        /// </summary>
        /// <param name="modelObjects">Collection of objects to be removed</param>
        /// <remarks>
        /// <para>Nulls and model objects that are not in the ListView are silently ignored.</para>
        /// <para>Due to problems in the underlying ListView, if you remove all the objects from
        /// the control using this method and the list scroll vertically when you do so,
        /// then when you subsequenially add more objects to the control,
        /// the vertical scroll bar will become confused and the control will draw one or more
        /// blank lines at the top of the list. </para>
        /// </remarks>
        public override void RemoveObjects(ICollection modelObjects) {
            if (VirtualListDataSource == null)
                return;

            // Give the world a chance to cancel or change the removed objects
            ItemsRemovingEventArgs args = new ItemsRemovingEventArgs(modelObjects);
            OnItemsRemoving(args);
            if (args.Canceled)
                return;

            try {
                BeginUpdate();
                VirtualListDataSource.RemoveObjects(args.ObjectsToRemove);
                BuildList();
                UnsubscribeNotifications(args.ObjectsToRemove);
            }
            finally {
                EndUpdate();
            }
        }

        /// <summary>
        /// Select the row that is displaying the given model object. All other rows are deselected.
        /// </summary>
        /// <param name="modelObject">Model object to select</param>
        /// <param name="setFocus">Should the object be focused as well?</param>
        public override void SelectObject(object modelObject, bool setFocus) {
            // Without a data source, we can't do this.
            if (VirtualListDataSource == null)
                return;

            // Check that the object is in the list (plus not all data sources can locate objects)
            int index = VirtualListDataSource.GetObjectIndex(modelObject);
            if (index < 0 || index >= VirtualListSize)
                return;

            // If the given model is already selected, don't do anything else (prevents an flicker)
            if (SelectedIndices.Count == 1 && SelectedIndices[0] == index)
                return;

            // Finally, select the row
            SelectedIndices.Clear();
            SelectedIndices.Add(index);
            if (setFocus && SelectedItem != null)
                SelectedItem.Focused = true;
        }

        /// <summary>
        /// Select the rows that is displaying any of the given model object. All other rows are deselected.
        /// </summary>
        /// <param name="modelObjects">A collection of model objects</param>
        /// <remarks>This method has O(n) performance where n is the number of model objects passed.
        /// Do not use this to select all the rows in the list -- use SelectAll() for that.</remarks>
        public override void SelectObjects(IList modelObjects) {
            // Without a data source, we can't do this.
            if (VirtualListDataSource == null)
                return;

            SelectedIndices.Clear();

            if (modelObjects == null)
                return;

            foreach (object modelObject in modelObjects) {
                int index = VirtualListDataSource.GetObjectIndex(modelObject);
                if (index >= 0 && index < VirtualListSize)
                    SelectedIndices.Add(index);
            }
        }

        /// <summary>
        /// Set the collection of objects that this control will show.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="preserveState">Should the state of the list be preserved as far as is possible.</param>
        public override void SetObjects(IEnumerable collection, bool preserveState) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate { SetObjects(collection, preserveState); });
                return;
            }

            if (VirtualListDataSource == null)
                return;

            // Give the world a chance to cancel or change the assigned collection
            ItemsChangingEventArgs args = new ItemsChangingEventArgs(null, collection);
            OnItemsChanging(args);
            if (args.Canceled)
                return;

            BeginUpdate();
            try {
                VirtualListDataSource.SetObjects(args.NewObjects);
                BuildList();
                UpdateNotificationSubscriptions(args.NewObjects);
            }
            finally {
                EndUpdate();
            }
        }

        #endregion

        #region Check boxes
//
//        /// <summary>
//        /// Check all rows
//        /// </summary>
//        /// <remarks>The performance of this method is O(n) where n is the number of rows in the control.</remarks>
//        public override void CheckAll()
//        {
//            if (!this.CheckBoxes)
//                return;
//
//            Stopwatch sw = Stopwatch.StartNew();
//
//            this.BeginUpdate();
//
//            foreach (Object x in this.Objects)
//                this.SetObjectCheckedness(x, CheckState.Checked);
//
//            this.EndUpdate();
//
//            Debug.WriteLine(String.Format("PERF - CheckAll() on {2} objects took {0}ms / {1} ticks", sw.ElapsedMilliseconds, sw.ElapsedTicks, this.GetItemCount()));
//
//        }
//
//        /// <summary>
//        /// Uncheck all rows
//        /// </summary>
//        /// <remarks>The performance of this method is O(n) where n is the number of rows in the control.</remarks>
//        public override void UncheckAll()
//        {
//            if (!this.CheckBoxes)
//                return;
//
//            Stopwatch sw = Stopwatch.StartNew();
//
//            this.BeginUpdate();
//
//            foreach (Object x in this.Objects)
//                this.SetObjectCheckedness(x, CheckState.Unchecked);
//
//            this.EndUpdate();
//
//            Debug.WriteLine(String.Format("PERF - UncheckAll() on {2} objects took {0}ms / {1} ticks", sw.ElapsedMilliseconds, sw.ElapsedTicks, this.GetItemCount()));
//        }

        /// <summary>
        /// Get the checkedness of an object from the model. Returning null means the
        /// model does know and the value from the control will be used.
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        protected override CheckState? GetCheckState(object modelObject)
        {
            if (CheckStateGetter != null)
                return base.GetCheckState(modelObject);

            CheckState state;
            if (modelObject != null && CheckStateMap.TryGetValue(modelObject, out state))
                return state;
            return CheckState.Unchecked;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Rebuild the list with its current contents.
        /// </summary>
        /// <remarks>
        /// Invalidate any cached information when we rebuild the list.
        /// </remarks>
        public override void BuildList(bool shouldPreserveSelection) {
            UpdateVirtualListSize();
            ClearCachedInfo();
            if (ShowGroups)
                BuildGroups();
            else
                Sort();
            Invalidate();
        }

        /// <summary>
        /// Clear any cached info this list may have been using
        /// </summary>
        public override void ClearCachedInfo() {
            lastRetrieveVirtualItemIndex = -1;
        }

        /// <summary>
        /// Do the work of creating groups for this control
        /// </summary>
        /// <param name="groups"></param>
        protected override void CreateGroups(IEnumerable<OLVGroup> groups) {

            // In a virtual list, we cannot touch the Groups property.
            // It was obviously not written for virtual list and often throws exceptions.

            NativeMethods.ClearGroups(this);

            EnableVirtualGroups();

            foreach (OLVGroup group in groups) {
                Debug.Assert(group.Items.Count == 0, "Groups in virtual lists cannot set Items. Use VirtualItemCount instead.");
                Debug.Assert(group.VirtualItemCount > 0, "VirtualItemCount must be greater than 0.");

                group.InsertGroupNewStyle(this);
            }
        }

        /// <summary>
        /// Do the plumbing to disable groups on a virtual list
        /// </summary>
        protected void DisableVirtualGroups() {
            NativeMethods.ClearGroups(this);
            //System.Diagnostics.Debug.WriteLine(err);

            const int LVM_ENABLEGROUPVIEW = 0x1000 + 157;
            IntPtr x = NativeMethods.SendMessage(Handle, LVM_ENABLEGROUPVIEW, 0, 0);
            //System.Diagnostics.Debug.WriteLine(x);

            const int LVM_SETOWNERDATACALLBACK = 0x10BB;
            IntPtr x2 = NativeMethods.SendMessage(Handle, LVM_SETOWNERDATACALLBACK, 0, 0);
            //System.Diagnostics.Debug.WriteLine(x2);
        }

        /// <summary>
        /// Do the plumbing to enable groups on a virtual list
        /// </summary>
        protected void EnableVirtualGroups() {

            // We need to implement the IOwnerDataCallback interface
            if (ownerDataCallbackImpl == null)
                ownerDataCallbackImpl = new OwnerDataCallbackImpl(this);

            const int LVM_SETOWNERDATACALLBACK = 0x10BB;
            IntPtr ptr = Marshal.GetComInterfaceForObject(ownerDataCallbackImpl, typeof(IOwnerDataCallback));
            IntPtr x = NativeMethods.SendMessage(Handle, LVM_SETOWNERDATACALLBACK, ptr, 0);
            //System.Diagnostics.Debug.WriteLine(x);
            Marshal.Release(ptr);

            const int LVM_ENABLEGROUPVIEW = 0x1000 + 157;
            x = NativeMethods.SendMessage(Handle, LVM_ENABLEGROUPVIEW, 1, 0);
            //System.Diagnostics.Debug.WriteLine(x);
        }
        private OwnerDataCallbackImpl ownerDataCallbackImpl;

        /// <summary>
        /// Return the position of the given itemIndex in the list as it currently shown to the user.
        /// If the control is not grouped, the display order is the same as the
        /// sorted list order. But if the list is grouped, the display order is different.
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public override int GetDisplayOrderOfItemIndex(int itemIndex) {
            if (!ShowGroups)
                return itemIndex;

            int groupIndex = GroupingStrategy.GetGroup(itemIndex);
            int displayIndex = 0;
            for (int i = 0; i < groupIndex - 1; i++)
                displayIndex += OLVGroups[i].VirtualItemCount;
            displayIndex += GroupingStrategy.GetIndexWithinGroup(OLVGroups[groupIndex], itemIndex);

            return displayIndex;
        }

        /// <summary>
        /// Return the last item in the order they are shown to the user.
        /// If the control is not grouped, the display order is the same as the
        /// sorted list order. But if the list is grouped, the display order is different.
        /// </summary>
        /// <returns></returns>
        public override OLVListItem GetLastItemInDisplayOrder() {
            if (!ShowGroups)
                return base.GetLastItemInDisplayOrder();

            if (OLVGroups.Count > 0) {
                OLVGroup lastGroup = OLVGroups[OLVGroups.Count - 1];
                if (lastGroup.VirtualItemCount > 0)
                    return GetItem(GroupingStrategy.GetGroupMember(lastGroup, lastGroup.VirtualItemCount - 1));
            }

            return null;
        }

        /// <summary>
        /// Return the n'th item (0-based) in the order they are shown to the user.
        /// If the control is not grouped, the display order is the same as the
        /// sorted list order. But if the list is grouped, the display order is different.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public override OLVListItem GetNthItemInDisplayOrder(int n) {
            if (!ShowGroups || OLVGroups == null || OLVGroups.Count == 0)
                return GetItem(n);

            foreach (OLVGroup group in OLVGroups) {
                if (n < group.VirtualItemCount)
                    return GetItem(GroupingStrategy.GetGroupMember(group, n));

                n -= group.VirtualItemCount;
            }

            return null;
        }

        /// <summary>
        /// Return the ListViewItem that appears immediately after the given item.
        /// If the given item is null, the first item in the list will be returned.
        /// Return null if the given item is the last item.
        /// </summary>
        /// <param name="itemToFind">The item that is before the item that is returned, or null</param>
        /// <returns>A OLVListItem</returns>
        public override OLVListItem GetNextItem(OLVListItem itemToFind) {
            if (!ShowGroups) 
                return base.GetNextItem(itemToFind);

            // Sanity
            if (OLVGroups == null || OLVGroups.Count == 0)
                return null;

            // If the given item is null, return the first member of the first group
            if (itemToFind == null) {
                return GetItem(GroupingStrategy.GetGroupMember(OLVGroups[0], 0));
            }

            // Find where this item occurs (which group and where in that group)
            int groupIndex = GroupingStrategy.GetGroup(itemToFind.Index);
            int indexWithinGroup = GroupingStrategy.GetIndexWithinGroup(OLVGroups[groupIndex], itemToFind.Index);

            // If it's not the last member, just return the next member
            if (indexWithinGroup < OLVGroups[groupIndex].VirtualItemCount - 1)
                return GetItem(GroupingStrategy.GetGroupMember(OLVGroups[groupIndex], indexWithinGroup + 1));
            
            // The item is the last member of its group. Return the first member of the next group
            // (unless there isn't a next group)
            if (groupIndex < OLVGroups.Count - 1)
                return GetItem(GroupingStrategy.GetGroupMember(OLVGroups[groupIndex + 1], 0));

            return null;
        }

        /// <summary>
        /// Return the ListViewItem that appears immediately before the given item.
        /// If the given item is null, the last item in the list will be returned.
        /// Return null if the given item is the first item.
        /// </summary>
        /// <param name="itemToFind">The item that is before the item that is returned</param>
        /// <returns>A ListViewItem</returns>
        public override OLVListItem GetPreviousItem(OLVListItem itemToFind) {
            if (!ShowGroups) 
                return base.GetPreviousItem(itemToFind);

            // Sanity
            if (OLVGroups == null || OLVGroups.Count == 0)
                return null;

            // If the given items is null, return the last member of the last group
            if (itemToFind == null) {
                OLVGroup lastGroup = OLVGroups[OLVGroups.Count - 1];
                return GetItem(GroupingStrategy.GetGroupMember(lastGroup, lastGroup.VirtualItemCount - 1));
            }

            // Find where this item occurs (which group and where in that group)
            int groupIndex = GroupingStrategy.GetGroup(itemToFind.Index);
            int indexWithinGroup = GroupingStrategy.GetIndexWithinGroup(OLVGroups[groupIndex], itemToFind.Index);

            // If it's not the first member of the group, just return the previous member
            if (indexWithinGroup > 0)
                return GetItem(GroupingStrategy.GetGroupMember(OLVGroups[groupIndex], indexWithinGroup - 1));

            // The item is the first member of its group. Return the last member of the previous group
            // (if there is one)
            if (groupIndex > 0) {
                OLVGroup previousGroup = OLVGroups[groupIndex - 1];
                return GetItem(GroupingStrategy.GetGroupMember(previousGroup, previousGroup.VirtualItemCount - 1));
            }

            return null;
        }

        /// <summary>
        /// Make a list of groups that should be shown according to the given parameters
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override IList<OLVGroup> MakeGroups(GroupingParameters parms) {
            if (GroupingStrategy == null)
                return new List<OLVGroup>();
            else
                return GroupingStrategy.GetGroups(parms);
        }

        /// <summary>
        /// Create a OLVListItem for given row index
        /// </summary>
        /// <param name="itemIndex">The index of the row that is needed</param>
        /// <returns>An OLVListItem</returns>
        public virtual OLVListItem MakeListViewItem(int itemIndex) {
            OLVListItem olvi = new OLVListItem(GetModelObject(itemIndex));
            FillInValues(olvi, olvi.RowObject);

            PostProcessOneRow(itemIndex, GetDisplayOrderOfItemIndex(itemIndex), olvi);

            if (HotRowIndex == itemIndex)
                UpdateHotRow(olvi);

            return olvi;
        }

        /// <summary>
        /// On virtual lists, this cannot work.
        /// </summary>
        protected override void PostProcessRows() {
        }

        /// <summary>
        /// Record the change of checkstate for the given object in the model.
        /// This does not update the UI -- only the model
        /// </summary>
        /// <param name="modelObject"></param>
        /// <param name="state"></param>
        /// <returns>The check state that was recorded and that should be used to update
        /// the control.</returns>
        protected override CheckState PutCheckState(object modelObject, CheckState state) {
            state = base.PutCheckState(modelObject, state);
            CheckStateMap[modelObject] = state;
            return state;
        }

        /// <summary>
        /// Refresh the given item in the list
        /// </summary>
        /// <param name="olvi">The item to refresh</param>
        public override void RefreshItem(OLVListItem olvi) {
            ClearCachedInfo();
            RedrawItems(olvi.Index, olvi.Index, true);
        }

        /// <summary>
        /// Change the size of the list
        /// </summary>
        /// <param name="newSize"></param>
        protected virtual void SetVirtualListSize(int newSize) {
            if (newSize < 0 || VirtualListSize == newSize)
                return;

            int oldSize = VirtualListSize;

            ClearCachedInfo();

            // There is a bug in .NET when a virtual ListView is cleared
            // (i.e. VirtuaListSize set to 0) AND it is scrolled vertically: the scroll position 
            // is wrong when the list is next populated. To avoid this, before 
            // clearing a virtual list, we make sure the list is scrolled to the top.
            // [6 weeks later] Damn this is a pain! There are cases where this can also throw exceptions!
            try {
                if (newSize == 0 && TopItemIndex > 0)
                    TopItemIndex = 0;
            }
            catch (Exception) {
                // Ignore any failures
            }

            // In strange cases, this can throw the exceptions too. The best we can do is ignore them :(
            try {
                VirtualListSize = newSize;
            }
            catch (ArgumentOutOfRangeException) {
                // pass
            }
            catch (NullReferenceException) {
                // pass
            }

            // Tell the world that the size of the list has changed
            OnItemsChanged(new ItemsChangedEventArgs(oldSize, VirtualListSize));
        }

        /// <summary>
        /// Take ownership of the 'objects' collection. This separates our collection from the source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method
        /// separates the 'objects' instance variable from its source, so that any AddObject/RemoveObject
        /// calls will modify our collection and not the original colleciton.
        /// </para>
        /// <para>
        /// VirtualObjectListViews always own their collections, so this is a no-op.
        /// </para>
        /// </remarks>
        protected override void TakeOwnershipOfObjects() {
        }

        /// <summary>
        /// Change the state of the control to reflect changes in filtering
        /// </summary>
        protected override void UpdateFiltering() {
            if (VirtualListDataSource is not IFilterableDataSource filterable)
                return;

            BeginUpdate();
            try {
                int originalSize = VirtualListSize;
                filterable.ApplyFilters(ModelFilter, ListFilter);
                BuildList();

                //// If the filtering actually did something, rebuild the groups if they are being shown
                //if (originalSize != this.VirtualListSize && this.ShowGroups)
                //    this.BuildGroups();
            }
            finally {
                EndUpdate();
            }
        }

        /// <summary>
        /// Change the size of the virtual list so that it matches its data source
        /// </summary>
        public virtual void UpdateVirtualListSize() {
            if (VirtualListDataSource != null)
                SetVirtualListSize(VirtualListDataSource.GetObjectCount());
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handle the CacheVirtualItems event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleCacheVirtualItems(object sender, CacheVirtualItemsEventArgs e) {
            if (VirtualListDataSource != null)
                VirtualListDataSource.PrepareCache(e.StartIndex, e.EndIndex);
        }

        /// <summary>
        /// Handle a RetrieveVirtualItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            // .NET 2.0 seems to generate a lot of these events. Before drawing *each* sub-item,
            // this event is triggered 4-8 times for the same index. So we save lots of CPU time
            // by caching the last result.
            //System.Diagnostics.Debug.WriteLine(String.Format("HandleRetrieveVirtualItem({0})", e.ItemIndex));

            if (lastRetrieveVirtualItemIndex != e.ItemIndex) {
                lastRetrieveVirtualItemIndex = e.ItemIndex;
                lastRetrieveVirtualItem = MakeListViewItem(e.ItemIndex);
            }
            e.Item = lastRetrieveVirtualItem;
        }

        /// <summary>
        /// Handle the SearchForVirtualList event, which is called when the user types into a virtual list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleSearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e) {
            // The event has e.IsPrefixSearch, but as far as I can tell, this is always false (maybe that's different under Vista)
            // So we ignore IsPrefixSearch and IsTextSearch and always to a case insensitve prefix match.

            // We can't do anything if we don't have a data source
            if (VirtualListDataSource == null)
                return;

            // Where should we start searching? If the last row is focused, the SearchForVirtualItemEvent starts searching
            // from the next row, which is actually an invalidate index -- so we make sure we never go past the last object.
            int start = Math.Min(e.StartIndex, VirtualListDataSource.GetObjectCount() - 1);

            // Give the world a chance to fiddle with or completely avoid the searching process
            BeforeSearchingEventArgs args = new BeforeSearchingEventArgs(e.Text, start);
            OnBeforeSearching(args);
            if (args.Canceled)
                return;

            // Do the search
            int i = FindMatchingRow(args.StringToFind, args.StartSearchFrom, e.Direction);

            // Tell the world that a search has occurred
            AfterSearchingEventArgs args2 = new AfterSearchingEventArgs(args.StringToFind, i);
            OnAfterSearching(args2);

            // If we found a match, tell the event
            if (i != -1)
                e.Index = i;
        }

        /// <summary>
        /// Find the first row in the given range of rows that prefix matches the string value of the given column.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="column"></param>
        /// <returns>The index of the matched row, or -1</returns>
        protected override int FindMatchInRange(string text, int first, int last, OLVColumn column) {
            return VirtualListDataSource.SearchText(text, first, last, column);
        }

        #endregion

        #region Variable declaractions

        private OLVListItem lastRetrieveVirtualItem;
        private int lastRetrieveVirtualItemIndex = -1;

        #endregion
    }
}
