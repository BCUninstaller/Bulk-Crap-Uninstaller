/*
 * DragSource.cs - Add drag source functionality to an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 2009-03-17 5:15 PM
 *
 * Change log:
 * 2011-03-29   JPP  - Separate OLVDataObject.cs
 * v2.3
 * 2009-07-06   JPP  - Make sure Link is acceptable as an drop effect by default
 *                     (since MS didn't make it part of the 'All' value)
 * v2.2
 * 2009-04-15   JPP  - Separated DragSource.cs into DropSink.cs
 * 2009-03-17   JPP  - Initial version
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
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// An IDragSource controls how drag out from the ObjectListView will behave
    /// </summary>
    public interface IDragSource
    {
        /// <summary>
        /// A drag operation is beginning. Return the data object that will be used 
        /// for data transfer. Return null to prevent the drag from starting. The data
        /// object will normally include all the selected objects.
        /// </summary>
        /// <remarks>
        /// The returned object is later passed to the GetAllowedEffect() and EndDrag()
        /// methods.
        /// </remarks>
        /// <param name="olv">What ObjectListView is being dragged from.</param>
        /// <param name="button">Which mouse button is down?</param>
        /// <param name="item">What item was directly dragged by the user? There may be more than just this 
        /// item selected.</param>
        /// <returns>The data object that will be used for data transfer. This will often be a subclass
        /// of DataObject, but does not need to be.</returns>
        Object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item);

        /// <summary>
        /// What operations are possible for this drag? This controls the icon shown during the drag
        /// </summary>
        /// <param name="dragObject">The data object returned by StartDrag()</param>
        /// <returns>A combination of DragDropEffects flags</returns>
        DragDropEffects GetAllowedEffects(Object dragObject);

        /// <summary>
        /// The drag operation is complete. Do whatever is necessary to complete the action.
        /// </summary>
        /// <param name="dragObject">The data object returned by StartDrag()</param>
        /// <param name="effect">The value returned from GetAllowedEffects()</param>
        void EndDrag(Object dragObject, DragDropEffects effect);
    }

    /// <summary>
    /// A do-nothing implementation of IDragSource that can be safely subclassed.
    /// </summary>
    public class AbstractDragSource : IDragSource
    {
        #region IDragSource Members

        /// <summary>
        /// See IDragSource documentation
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="button"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item) {
            return null;
        }

        /// <summary>
        /// See IDragSource documentation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual DragDropEffects GetAllowedEffects(Object data) {
            return DragDropEffects.None;
        }

        /// <summary>
        /// See IDragSource documentation
        /// </summary>
        /// <param name="dragObject"></param>
        /// <param name="effect"></param>
        public virtual void EndDrag(Object dragObject, DragDropEffects effect) {
        }

        #endregion
    }

    /// <summary>
    /// A reasonable implementation of IDragSource that provides normal
    /// drag source functionality. It creates a data object that supports
    /// inter-application dragging of text and HTML representation of 
    /// the dragged rows. It can optionally force a refresh of all dragged
    /// rows when the drag is complete.
    /// </summary>
    /// <remarks>Subclasses can override GetDataObject() to add new
    /// data formats to the data transfer object.</remarks>
    public class SimpleDragSource : IDragSource
    {
        #region Constructors

        /// <summary>
        /// Construct a SimpleDragSource
        /// </summary>
        public SimpleDragSource() {
        }

        /// <summary>
        /// Construct a SimpleDragSource that refreshes the dragged rows when
        /// the drag is complete
        /// </summary>
        /// <param name="refreshAfterDrop"></param>
        public SimpleDragSource(bool refreshAfterDrop) {
            RefreshAfterDrop = refreshAfterDrop;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets whether the dragged rows should be refreshed when the 
        /// drag operation is complete.
        /// </summary>
        public bool RefreshAfterDrop {
            get { return refreshAfterDrop; }
            set { refreshAfterDrop = value;  }
        }
        private bool refreshAfterDrop;

        #endregion

        #region IDragSource Members

        /// <summary>
        /// Create a DataObject when the user does a left mouse drag operation.
        /// See IDragSource for further information.
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="button"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item) {
            // We only drag on left mouse
            if (button != MouseButtons.Left)
                return null;

            return CreateDataObject(olv);
        }

        /// <summary>
        /// Which operations are allowed in the operation? By default, all operations are supported.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>All opertions are supported</returns>
        public virtual DragDropEffects GetAllowedEffects(Object data) {
            return DragDropEffects.All | DragDropEffects.Link; // why didn't MS include 'Link' in 'All'??
        }

        /// <summary>
        /// The drag operation is finished. Refreshe the dragged rows if so configured.
        /// </summary>
        /// <param name="dragObject"></param>
        /// <param name="effect"></param>
        public virtual void EndDrag(Object dragObject, DragDropEffects effect) {
            if (!(dragObject is OLVDataObject data))
                return;

            if (RefreshAfterDrop)
                data.ListView.RefreshObjects(data.ModelObjects);
        }

        /// <summary>
        /// Create a data object that will be used to as the data object
        /// for the drag operation.
        /// </summary>
        /// <remarks>
        /// Subclasses can override this method add new formats to the data object.
        /// </remarks>
        /// <param name="olv">The ObjectListView that is the source of the drag</param>
        /// <returns>A data object for the drag</returns>
        protected virtual object CreateDataObject(ObjectListView olv) {
            return new OLVDataObject(olv);
        }

        #endregion
    }
}
