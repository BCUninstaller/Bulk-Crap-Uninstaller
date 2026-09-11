/*
 * DesignSupport - Design time support for the various classes within ObjectListView
 *
 * Author: Phillip Piper
 * Date: 12/08/2009 8:36 PM
 *
 * Change log:
 * 2012-08-27   JPP  - Fall back to more specific type name for the ListViewDesigner if
 *                     the first GetType() fails.
 * v2.5.1
 * 2012-04-26   JPP  - Filter group events from TreeListView since it can't have groups
 * 2011-06-06   JPP  - Vastly improved ObjectListViewDesigner, based off information in
 *                     "'Inheriting' from an Internal WinForms Designer" on CodeProject.
 * v2.3
 * 2009-08-12   JPP  - Initial version
 *
 * To do:
 *
 * Copyright (C) 2009-2014 Phillip Piper
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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BrightIdeasSoftware.Design
{

    /// <summary>
    /// Designer for <see cref="ObjectListView"/> and its subclasses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This designer removes properties and events that are available on ListView but that are not
    /// useful on ObjectListView.
    /// </para>
    /// <para>
    /// We can't inherit from System.Windows.Forms.Design.ListViewDesigner, since it is marked internal.
    /// So, this class uses reflection to create a ListViewDesigner and then forwards messages to that designer.
    /// </para>
    /// </remarks>
    public class ObjectListViewDesigner : ControlDesigner
    {

        #region Initialize & Dispose

        /// <summary>
        /// Initializes the designer with the specified component.
        /// </summary>
        /// <param name="component">The <see cref="T:System.ComponentModel.IComponent"/> to associate the designer with. This component must always be an instance of, or derive from, <see cref="T:System.Windows.Forms.Control"/>. </param>
        public override void Initialize(IComponent component) {
            // Debug.WriteLine("ObjectListViewDesigner.Initialize");

            // Use reflection to bypass the "internal" marker on ListViewDesigner
            // If we can't get the unversioned designer, look specifically for .NET 4.0 version of it.
            Type tListViewDesigner = Type.GetType("System.Windows.Forms.Design.ListViewDesigner, System.Design") ??
                                     Type.GetType("System.Windows.Forms.Design.ListViewDesigner, System.Design, " +
                                                  "Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            if (tListViewDesigner == null) throw new ArgumentException("Could not load ListViewDesigner");

            listViewDesigner = (ControlDesigner)Activator.CreateInstance(tListViewDesigner, BindingFlags.Instance | BindingFlags.Public, null, null, null);
            designerFilter = listViewDesigner;

            // Fetch the methods from the ListViewDesigner that we know we want to use
            listViewDesignGetHitTest = tListViewDesigner.GetMethod("GetHitTest", BindingFlags.Instance | BindingFlags.NonPublic);
            listViewDesignWndProc = tListViewDesigner.GetMethod("WndProc", BindingFlags.Instance | BindingFlags.NonPublic);

            Debug.Assert(listViewDesignGetHitTest != null, "Required method (GetHitTest) not found on ListViewDesigner");
            Debug.Assert(listViewDesignWndProc != null, "Required method (WndProc) not found on ListViewDesigner");

            // Tell the Designer to use properties of default designer as well as the properties of this class (do before base.Initialize)
            TypeDescriptor.CreateAssociation(component, listViewDesigner);

            IServiceContainer site = (IServiceContainer)component.Site;
            if (site != null && GetService(typeof(DesignerCommandSet)) == null) {
                site.AddService(typeof(DesignerCommandSet), new CDDesignerCommandSet(this));
            } else {
                Debug.Fail("site != null && GetService(typeof (DesignerCommandSet)) == null");
            }

            listViewDesigner.Initialize(component);
            base.Initialize(component);

            RemoveDuplicateDockingActionList();
        }

        /// <summary>
        /// Initializes a newly created component.
        /// </summary>
        /// <param name="defaultValues">A name/value dictionary of default values to apply to properties. May be null if no default values are specified.</param>
        public override void InitializeNewComponent(IDictionary defaultValues) {
            // Debug.WriteLine("ObjectListViewDesigner.InitializeNewComponent");
            base.InitializeNewComponent(defaultValues);
            listViewDesigner.InitializeNewComponent(defaultValues);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Design.ControlDesigner"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing) {
            // Debug.WriteLine("ObjectListViewDesigner.Dispose");
            if (disposing) {
                if (listViewDesigner != null) {
                    listViewDesigner.Dispose();
                    // Normally we would now null out the designer, but this designer
                    // still has methods called AFTER it is disposed.
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Removes the duplicate DockingActionList added by this designer to the <see cref="DesignerActionService"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ControlDesigner.Initialize"/> adds an internal DockingActionList : 'Dock/Undock in Parent Container'.
        /// But the default designer has already added that action list. So we need to remove one.
        /// </remarks>
        private void RemoveDuplicateDockingActionList() {
            // This is a true hack -- in a class that is basically a huge hack itself.
            // Reach into the bowel of our base class, get a private field, and use that fields value to
            // remove an action from the designer.
            // In ControlDesigner, there is "private DockingActionList dockingAction;"
            // Don't you just love Reflector?!
            FieldInfo fi = typeof(ControlDesigner).GetField("dockingAction", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null) {
                DesignerActionList dockingAction = (DesignerActionList)fi.GetValue(this);
                if (dockingAction != null) {
                    DesignerActionService service = (DesignerActionService)GetService(typeof(DesignerActionService));
                    if (service != null) {
                        service.Remove(Control, dockingAction);
                    }
                }
            }
        }

        #endregion

        #region IDesignerFilter overrides

        /// <summary>
        /// Adjusts the set of properties the component exposes through a <see cref="T:System.ComponentModel.TypeDescriptor"/>.
        /// </summary>
        /// <param name="properties">An <see cref="T:System.Collections.IDictionary"/> containing the properties for the class of the component. </param>
        protected override void PreFilterProperties(IDictionary properties) {
            // Debug.WriteLine("ObjectListViewDesigner.PreFilterProperties");

            // Always call the base PreFilterProperties implementation 
            // before you modify the properties collection.
            base.PreFilterProperties(properties);

            // Give the listviewdesigner a chance to filter the properties
            // (though we already know it's not going to do anything)
            designerFilter.PreFilterProperties(properties);

            // I'd like to just remove the redundant properties, but that would
            // break backward compatibility. The deserialiser that handles the XXX.Designer.cs file
            // works off the designer, so even if the property exists in the class, the deserialiser will
            // throw an error if the associated designer actually removes that property.
            // So we shadow the unwanted properties, and give the replacement properties
            // non-browsable attributes so that they are hidden from the user

            List<string> unwantedProperties = new List<string>(new string[] { 
                "BackgroundImage", "BackgroundImageTiled", "HotTracking", "HoverSelection", 
                "LabelEdit", "VirtualListSize", "VirtualMode" });

            // Also hid Tooltip properties, since giving a tooltip to the control through the IDE
            // messes up the tooltip handling
            foreach (string propertyName in properties.Keys) {
                if (propertyName.StartsWith("ToolTip")) {
                    unwantedProperties.Add(propertyName);
                }
            }

            // If we are looking at a TreeListView, remove group related properties
            // since TreeListViews can't show groups
            if (Control is TreeListView) {
                unwantedProperties.AddRange(new string[] {
                    "GroupImageList", "GroupWithItemCountFormat", "GroupWithItemCountSingularFormat", "HasCollapsibleGroups", 
                    "SpaceBetweenGroups", "ShowGroups", "SortGroupItemsByPrimaryColumn", "ShowItemCountOnGroups"
                });
            }

            // Shadow the unwanted properties, and give the replacement properties
            // non-browsable attributes so that they are hidden from the user
            foreach (string unwantedProperty in unwantedProperties) {
                PropertyDescriptor propertyDesc = TypeDescriptor.CreateProperty(
                    typeof(ObjectListView),
                    (PropertyDescriptor)properties[unwantedProperty],
                    new BrowsableAttribute(false));
                properties[unwantedProperty] = propertyDesc;
            }
        }

        /// <summary>
        /// Allows a designer to add to the set of events that it exposes through a <see cref="T:System.ComponentModel.TypeDescriptor"/>.
        /// </summary>
        /// <param name="events">The events for the class of the component. </param>
        protected override void PreFilterEvents(IDictionary events) {
            // Debug.WriteLine("ObjectListViewDesigner.PreFilterEvents");
            base.PreFilterEvents(events);
            designerFilter.PreFilterEvents(events);

            // Remove the events that don't make sense for an ObjectListView.
            // See PreFilterProperties() for why we do this dance rather than just remove the event.
            List<string> unwanted = new List<string>(new string[] {
                "AfterLabelEdit",
                "BeforeLabelEdit",
                "DrawColumnHeader",
                "DrawItem",
                "DrawSubItem",
                "RetrieveVirtualItem",
                "SearchForVirtualItem",
                "VirtualItemsSelectionRangeChanged"
            });

            // If we are looking at a TreeListView, remove group related events
            // since TreeListViews can't show groups
            if (Control is TreeListView) {
                unwanted.AddRange(new string[] {
                    "AboutToCreateGroups",
                    "AfterCreatingGroups",
                    "BeforeCreatingGroups",
                    "GroupTaskClicked",
                    "GroupExpandingCollapsing", 
                    "GroupStateChanged"
                });
            }

            foreach (string unwantedEvent in unwanted) {
                EventDescriptor eventDesc = TypeDescriptor.CreateEvent(
                   typeof(ObjectListView),
                    (EventDescriptor)events[unwantedEvent],
                    new BrowsableAttribute(false));
                events[unwantedEvent] = eventDesc;
            }
        }

        /// <summary>
        /// Allows a designer to change or remove items from the set of attributes that it exposes through a <see cref="T:System.ComponentModel.TypeDescriptor"/>.
        /// </summary>
        /// <param name="attributes">The attributes for the class of the component. </param>
        protected override void PostFilterAttributes(IDictionary attributes) {
            // Debug.WriteLine("ObjectListViewDesigner.PostFilterAttributes");
            designerFilter.PostFilterAttributes(attributes);
            base.PostFilterAttributes(attributes);
        }

        /// <summary>
        /// Allows a designer to change or remove items from the set of events that it exposes through a <see cref="T:System.ComponentModel.TypeDescriptor"/>.
        /// </summary>
        /// <param name="events">The events for the class of the component. </param>
        protected override void PostFilterEvents(IDictionary events) {
            // Debug.WriteLine("ObjectListViewDesigner.PostFilterEvents");
            designerFilter.PostFilterEvents(events);
            base.PostFilterEvents(events);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the design-time action lists supported by the component associated with the designer.
        /// </summary>
        /// <returns>
        /// The design-time action lists supported by the component associated with the designer.
        /// </returns>
        public override DesignerActionListCollection ActionLists {
            get {
                // We want to change the first action list so it only has the commands we want
                DesignerActionListCollection actionLists = listViewDesigner.ActionLists;
                if (actionLists.Count > 0 && !(actionLists[0] is ListViewActionListAdapter)) {
                    actionLists[0] = new ListViewActionListAdapter(this, actionLists[0]);
                }
                return actionLists;
            }
        }

        /// <summary>
        /// Gets the collection of components associated with the component managed by the designer.
        /// </summary>
        /// <returns>
        /// The components that are associated with the component managed by the designer.
        /// </returns>
        public override ICollection AssociatedComponents {
            get {
                ArrayList components = new ArrayList(base.AssociatedComponents);
                components.AddRange(listViewDesigner.AssociatedComponents);
                return components;
            }
        }

        /// <summary>
        /// Indicates whether a mouse click at the specified point should be handled by the control.
        /// </summary>
        /// <returns>
        /// true if a click at the specified point is to be handled by the control; otherwise, false.
        /// </returns>
        /// <param name="point">A <see cref="T:System.Drawing.Point"/> indicating the position at which the mouse was clicked, in screen coordinates. </param>
        protected override bool GetHitTest(Point point) {
            // The ListViewDesigner wants to allow column dividers to be resized
            return (bool)listViewDesignGetHitTest.Invoke(listViewDesigner, new object[] { point });
        }

        /// <summary>
        /// Processes Windows messages and optionally routes them to the control.
        /// </summary>
        /// <param name="m">The <see cref="T:System.Windows.Forms.Message"/> to process. </param>
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case 0x4e:
                case 0x204e:
                    // The listview designer is interested in HDN_ENDTRACK notifications
                    listViewDesignWndProc.Invoke(listViewDesigner, new object[] { m });
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        #endregion

        #region Implementation variables

        private ControlDesigner listViewDesigner;
        private IDesignerFilter designerFilter;
        private MethodInfo listViewDesignGetHitTest;
        private MethodInfo listViewDesignWndProc;

        #endregion

        #region Custom action list

        /// <summary>
        /// This class modifies a ListViewActionList, by removing the "Edit Items" and "Edit Groups" actions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// That class is internal, so we cannot simply subclass it, which would be simplier.
        /// </para>
        /// <para>
        /// Action lists use reflection to determine if that action can be executed, so we not
        /// only have to modify the returned collection of actions, but we have to implement
        /// the properties and commands that the returned actions use. </para>
        /// </remarks>
        private class ListViewActionListAdapter : DesignerActionList
        {
            public ListViewActionListAdapter(ObjectListViewDesigner designer, DesignerActionList wrappedList)
                : base(wrappedList.Component) {
                this.designer = designer;
                this.wrappedList = wrappedList;
            }

            public override DesignerActionItemCollection GetSortedActionItems() {
                DesignerActionItemCollection items = wrappedList.GetSortedActionItems();
                items.RemoveAt(2); // remove Edit Groups
                items.RemoveAt(0); // remove Edit Items
                return items;
            }

            private void EditValue(ComponentDesigner componentDesigner, IComponent iComponent, string propertyName) {
                // One more complication. The ListViewActionList classes uses an internal class, EditorServiceContext, to 
                // edit the items/columns/groups collections. So, we use reflection to bypass the data hiding.
                Type tEditorServiceContext = Type.GetType("System.Windows.Forms.Design.EditorServiceContext, System.Design");
                tEditorServiceContext.InvokeMember("EditValue", BindingFlags.InvokeMethod | BindingFlags.Static, null, null, new object[] { componentDesigner, iComponent, propertyName });
            }

            private void SetValue(object target, string propertyName, object value) {
                TypeDescriptor.GetProperties(target)[propertyName].SetValue(target, value);
            }

            public void InvokeColumnsDialog() {
                EditValue(designer, Component, "Columns");
            }

            // Don't need these since we removed their corresponding actions from the list.
            // Keep the methods just in case.

            //public void InvokeGroupsDialog() {
            //    EditValue(this.designer, base.Component, "Groups");
            //}

            //public void InvokeItemsDialog() {
            //    EditValue(this.designer, base.Component, "Items");
            //}

            public ImageList LargeImageList {
                get { return ((ListView)Component).LargeImageList; }
                set { SetValue(Component, "LargeImageList", value); }
            }

            public ImageList SmallImageList {
                get { return ((ListView)Component).SmallImageList; }
                set { SetValue(Component, "SmallImageList", value); }
            }

            public View View {
                get { return ((ListView)Component).View; }
                set { SetValue(Component, "View", value); }
            }

            ObjectListViewDesigner designer;
            DesignerActionList wrappedList;
        }

        #endregion

        #region DesignerCommandSet

        private class CDDesignerCommandSet : DesignerCommandSet
        {

            public CDDesignerCommandSet(ComponentDesigner componentDesigner) {
                this.componentDesigner = componentDesigner;
            }

            public override ICollection GetCommands(string name) {
                // Debug.WriteLine("CDDesignerCommandSet.GetCommands:" + name);
                if (componentDesigner != null) {
                    if (name.Equals("Verbs")) {
                        return componentDesigner.Verbs;
                    }
                    if (name.Equals("ActionLists")) {
                        return componentDesigner.ActionLists;
                    }
                }
                return base.GetCommands(name);
            }

            private readonly ComponentDesigner componentDesigner;
        }

        #endregion
    }

    /// <summary>
    /// This class works in conjunction with the OLVColumns property to allow OLVColumns
    /// to be added to the ObjectListView.
    /// </summary>
    public class OLVColumnCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Create a OLVColumnCollectionEditor
        /// </summary>
        /// <param name="t"></param>
        public OLVColumnCollectionEditor(Type t)
            : base(t) {
        }

        /// <summary>
        /// What type of object does this editor create?
        /// </summary>
        /// <returns></returns>
        protected override Type CreateCollectionItemType() {
            return typeof(OLVColumn);
        }

        /// <summary>
        /// Edit a given value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            if (context == null)
                throw new ArgumentNullException("context");
            if (provider == null)
                throw new ArgumentNullException("provider");

            // Figure out which ObjectListView we are working on. This should be the Instance of the context.
            ObjectListView olv = context.Instance as ObjectListView;
            Debug.Assert(olv != null, "Instance must be an ObjectListView");

            // Edit all the columns, not just the ones that are visible
            base.EditValue(context, provider, olv.AllColumns);

            // Set the columns on the ListView to just the visible columns
            List<OLVColumn> newColumns = olv.GetFilteredColumns(View.Details);
            olv.Columns.Clear();
            olv.Columns.AddRange(newColumns.ToArray());

            return olv.Columns;
        }

        /// <summary>
        /// What text should be shown in the list for the given object?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override string GetDisplayText(object value) {
            if (!(value is OLVColumn col) || String.IsNullOrEmpty(col.AspectName))
                return base.GetDisplayText(value);

            return String.Format("{0} ({1})", base.GetDisplayText(value), col.AspectName);
        }
    }

    /// <summary>
    /// Control how the overlay is presented in the IDE
    /// </summary>
    internal class OverlayConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                if (value is ImageOverlay imageOverlay) {
                    return imageOverlay.Image == null ? "(none)" : "(set)";
                }

                if (value is TextOverlay textOverlay) {
                    return String.IsNullOrEmpty(textOverlay.Text) ? "(none)" : "(set)";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
