/*
 * Renderers - A collection of useful renderers that are used to owner draw a cell in an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 27/09/2008 9:15 AM
 *
 * Change log: 
 * v2.9
 * 2015-08-22   JPP  - Allow selected row back/fore colours to be specified for each row
 * 2015-06-23   JPP  - Added ColumnButtonRenderer plus general support for Buttons
 * 2015-06-22   JPP  - Added BaseRenderer.ConfigureItem() and ConfigureSubItem() to easily allow
 *                     other renderers to be chained for use within a primary renderer.
 *                   - Lots of tightening of hit tests and edit rectangles
 * 2015-05-15   JPP  - Handle renderering an Image when that Image is returned as an aspect.
 * v2.8
 * 2014-09-26   JPP  - Dispose of animation timer in a more robust fashion.
 * 2014-05-20   JPP  - Handle rendering disabled rows
 * v2.7
 * 2013-04-29   JPP  - Fixed bug where Images were not vertically aligned
 * v2.6
 * 2012-10-26   JPP  - Hit detection will no longer report check box hits on columns without checkboxes.
 * 2012-07-13   JPP  - [Breaking change] Added preferedSize parameter to IRenderer.GetEditRectangle().
 * v2.5.1
 * 2012-07-14   JPP  - Added CellPadding to various places. Replaced DescribedTaskRenderer.CellPadding.
 * 2012-07-11   JPP  - Added CellVerticalAlignment to various places allow cell contents to be vertically
 *                     aligned (rather than always being centered).
 * v2.5
 * 2010-08-24   JPP  - CheckBoxRenderer handles hot boxes and correctly vertically centers the box.
 * 2010-06-23   JPP  - Major rework of HighlightTextRenderer. Now uses TextMatchFilter directly.
 *                     Draw highlighting underneath text to improve legibility. Works with new
 *                     TextMatchFilter capabilities.
 * v2.4
 * 2009-10-30   JPP  - Plugged possible resource leak by using using() with CreateGraphics()
 * v2.3
 * 2009-09-28   JPP  - Added DescribedTaskRenderer
 * 2009-09-01   JPP  - Correctly handle an ImageRenderer's handling of an aspect that holds
 *                     the image to be displayed at Byte[].
 * 2009-08-29   JPP  - Fixed bug where some of a cell's background was not erased. 
 * 2009-08-15   JPP  - Correctly MeasureText() using the appropriate graphic context
 *                   - Handle translucent selection setting
 * v2.2.1
 * 2009-07-24   JPP  - Try to honour CanWrap setting when GDI rendering text.
 * 2009-07-11   JPP  - Correctly calculate edit rectangle for subitems of a tree view
 *                     (previously subitems were indented in the same way as the primary column)
 * v2.2
 * 2009-06-06   JPP  - Tweaked text rendering so that column 0 isn't ellipsed unnecessarily.
 * 2009-05-05   JPP  - Added Unfocused foreground and background colors 
 *                     (thanks to Christophe Hosten)
 * 2009-04-21   JPP  - Fixed off-by-1 error when calculating text widths. This caused
 *                     middle and right aligned columns to always wrap one character
 *                     when printed using ListViewPrinter (SF#2776634).
 * 2009-04-11   JPP  - Correctly renderer checkboxes when RowHeight is non-standard
 * 2009-04-06   JPP  - Allow for item indent when calculating edit rectangle
 * v2.1
 * 2009-02-24   JPP  - Work properly with ListViewPrinter again
 * 2009-01-26   JPP  - AUSTRALIA DAY (why aren't I on holidays!)
 *                   - Major overhaul of renderers. Now uses IRenderer interface.
 *                   - ImagesRenderer and FlagsRenderer<T> are now defunct.
 *                     The names are retained for backward compatibility.
 * 2009-01-23   JPP  - Align bitmap AND text according to column alignment (previously
 *                     only text was aligned and bitmap was always to the left).
 * 2009-01-21   JPP  - Changed to use TextRenderer rather than native GDI routines.
 * 2009-01-20   JPP  - Draw images directly from image list if possible. 30% faster!
 *                   - Tweaked some spacings to look more like native ListView
 *                   - Text highlight for non FullRowSelect is now the right color
 *                     when the control doesn't have focus.
 *                   - Commented out experimental animations. Still needs work.
 * 2009-01-19   JPP  - Changed to draw text using GDI routines. Looks more like
 *                     native control this way. Set UseGdiTextRendering to false to 
 *                     revert to previous behavior.
 * 2009-01-15   JPP  - Draw background correctly when control is disabled
 *                   - Render checkboxes using CheckBoxRenderer
 * v2.0.1
 * 2008-12-29   JPP  - Render text correctly when HideSelection is true.
 * 2008-12-26   JPP  - BaseRenderer now works correctly in all Views
 * 2008-12-23   JPP  - Fixed two small bugs in BarRenderer
 * v2.0
 * 2008-10-26   JPP  - Don't owner draw when in Design mode
 * 2008-09-27   JPP  - Separated from ObjectListView.cs
 * 
 * Copyright (C) 2006-2014 Phillip Piper
 * 
 * TO DO:
 * - Hit detection on renderers doesn't change the controls standard selection behavior
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Timer = System.Threading.Timer;

namespace BrightIdeasSoftware {
    /// <summary>
    /// Renderers are the mechanism used for owner drawing cells. As such, they can also handle
    /// hit detection and positioning of cell editing rectangles.
    /// </summary>
    public interface IRenderer {
        /// <summary>
        /// Render the whole item within an ObjectListView. This is only used in non-Details views.
        /// </summary>
        /// <param name="e">The event</param>
        /// <param name="g">A Graphics for rendering</param>
        /// <param name="itemBounds">The bounds of the item</param>
        /// <param name="rowObject">The model object to be drawn</param>
        /// <returns>Return true to indicate that the event was handled and no further processing is needed.</returns>
        bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, Object rowObject);

        /// <summary>
        /// Render one cell within an ObjectListView when it is in Details mode.
        /// </summary>
        /// <param name="e">The event</param>
        /// <param name="g">A Graphics for rendering</param>
        /// <param name="cellBounds">The bounds of the cell</param>
        /// <param name="rowObject">The model object to be drawn</param>
        /// <returns>Return true to indicate that the event was handled and no further processing is needed.</returns>
        bool RenderSubItem(DrawListViewSubItemEventArgs e, Graphics g, Rectangle cellBounds, Object rowObject);

        /// <summary>
        /// What is under the given point?
        /// </summary>
        /// <param name="hti"></param>
        /// <param name="x">x co-ordinate</param>
        /// <param name="y">y co-ordinate</param>
        /// <remarks>This method should only alter HitTestLocation and/or UserData.</remarks>
        void HitTest(OlvListViewHitTestInfo hti, int x, int y);

        /// <summary>
        /// When the value in the given cell is to be edited, where should the edit rectangle be placed?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        Rectangle GetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize);
    }

    /// <summary>
    /// Renderers that implement this interface will have the filter property updated,
    /// each time the filter on the ObjectListView is updated.
    /// </summary>
    public interface IFilterAwareRenderer
    {
        IModelFilter Filter { get; set; }
    }

    /// <summary>
    /// An AbstractRenderer is a do-nothing implementation of the IRenderer interface.
    /// </summary>
    [Browsable(true),
     ToolboxItem(false)]
    public class AbstractRenderer : Component, IRenderer {
        #region IRenderer Members

        /// <summary>
        /// Render the whole item within an ObjectListView. This is only used in non-Details views.
        /// </summary>
        /// <param name="e">The event</param>
        /// <param name="g">A Graphics for rendering</param>
        /// <param name="itemBounds">The bounds of the item</param>
        /// <param name="rowObject">The model object to be drawn</param>
        /// <returns>Return true to indicate that the event was handled and no further processing is needed.</returns>
        public virtual bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, object rowObject) {
            return true;
        }

        /// <summary>
        /// Render one cell within an ObjectListView when it is in Details mode.
        /// </summary>
        /// <param name="e">The event</param>
        /// <param name="g">A Graphics for rendering</param>
        /// <param name="cellBounds">The bounds of the cell</param>
        /// <param name="rowObject">The model object to be drawn</param>
        /// <returns>Return true to indicate that the event was handled and no further processing is needed.</returns>
        public virtual bool RenderSubItem(DrawListViewSubItemEventArgs e, Graphics g, Rectangle cellBounds, object rowObject) {
            return false;
        }

        /// <summary>
        /// What is under the given point?
        /// </summary>
        /// <param name="hti"></param>
        /// <param name="x">x co-ordinate</param>
        /// <param name="y">y co-ordinate</param>
        /// <remarks>This method should only alter HitTestLocation and/or UserData.</remarks>
        public virtual void HitTest(OlvListViewHitTestInfo hti, int x, int y) {}

        /// <summary>
        /// When the value in the given cell is to be edited, where should the edit rectangle be placed?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        public virtual Rectangle GetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize) {
            return cellBounds;
        }

        #endregion
    }

    /// <summary>
    /// This class provides compatibility for v1 RendererDelegates
    /// </summary>
    [ToolboxItem(false)]
    internal class Version1Renderer : AbstractRenderer {
        public Version1Renderer(RenderDelegate renderDelegate) {
            RenderDelegate = renderDelegate;
        }

        /// <summary>
        /// The renderer delegate that this renderer wraps
        /// </summary>
        public RenderDelegate RenderDelegate;

        #region IRenderer Members

        public override bool RenderSubItem(DrawListViewSubItemEventArgs e, Graphics g, Rectangle cellBounds, object rowObject) {
            if (RenderDelegate == null)
                return base.RenderSubItem(e, g, cellBounds, rowObject);
            else
                return RenderDelegate(e, g, cellBounds, rowObject);
        }

        #endregion
    }

    /// <summary>
    /// A BaseRenderer provides useful base level functionality for any custom renderer.
    /// </summary>
    /// <remarks>
    /// <para>Subclasses will normally override the Render or OptionalRender method, and use the other
    /// methods as helper functions.</para>
    /// </remarks>
    [Browsable(true),
     ToolboxItem(true)]
    public class BaseRenderer : AbstractRenderer {
        internal const TextFormatFlags NormalTextFormatFlags = TextFormatFlags.NoPrefix |
                                                               TextFormatFlags.EndEllipsis |
                                                               TextFormatFlags.PreserveGraphicsTranslateTransform;

        #region Configuration Properties

        /// <summary>
        /// Can the renderer wrap lines that do not fit completely within the cell?
        /// </summary>
        /// <remarks>Wrapping text doesn't work with the GDI renderer.</remarks>
        [Category("Appearance"),
         Description("Can the renderer wrap text that does not fit completely within the cell"),
         DefaultValue(false)]
        public bool CanWrap {
            get { return canWrap; }
            set {
                canWrap = value;
                if (canWrap)
                    UseGdiTextRendering = false;
            }
        }
        private bool canWrap;

        /// <summary>
        /// Gets or sets how many pixels will be left blank around this cell
        /// </summary>
        /// <remarks>
        /// <para>
        /// This setting only takes effect when the control is owner drawn.
        /// </para>
        /// <para><see cref="ObjectListView.CellPadding"/> for more details.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("The number of pixels that renderer will leave empty around the edge of the cell"),
         DefaultValue(null)]
        public Rectangle? CellPadding {
            get { return cellPadding; }
            set { cellPadding = value; }
        }
        private Rectangle? cellPadding;

        /// <summary>
        /// Gets the horiztonal alignment of the column
        /// </summary>
        [Browsable(false)]
        public HorizontalAlignment CellHorizontalAlignment
        {
            get { return Column == null ? HorizontalAlignment.Left : Column.TextAlign; }
        }

        /// <summary>
        /// Gets or sets how cells drawn by this renderer will be vertically aligned.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is not set, the value from the column or control itself will be used.
        /// </para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("How will cell values be vertically aligned?"),
         DefaultValue(null)]
        public virtual StringAlignment? CellVerticalAlignment {
            get { return cellVerticalAlignment; }
            set { cellVerticalAlignment = value; }
        }
        private StringAlignment? cellVerticalAlignment;

        /// <summary>
        /// Gets the optional padding that this renderer should apply before drawing.
        /// This property considers all possible sources of padding
        /// </summary>
        [Browsable(false)]
        protected virtual Rectangle? EffectiveCellPadding {
            get {
                if (cellPadding.HasValue)
                    return cellPadding.Value;

                if (OLVSubItem != null && OLVSubItem.CellPadding.HasValue)
                    return OLVSubItem.CellPadding.Value;

                if (ListItem != null && ListItem.CellPadding.HasValue)
                    return ListItem.CellPadding.Value;

                if (Column != null && Column.CellPadding.HasValue)
                    return Column.CellPadding.Value;

                if (ListView != null && ListView.CellPadding.HasValue)
                    return ListView.CellPadding.Value;

                return null;
            }
        }

        /// <summary>
        /// Gets the vertical cell alignment that should govern the rendering.
        /// This property considers all possible sources.
        /// </summary>
        [Browsable(false)]
        protected virtual StringAlignment EffectiveCellVerticalAlignment {
            get {
                if (cellVerticalAlignment.HasValue)
                    return cellVerticalAlignment.Value;

                if (OLVSubItem != null && OLVSubItem.CellVerticalAlignment.HasValue)
                    return OLVSubItem.CellVerticalAlignment.Value;

                if (ListItem != null && ListItem.CellVerticalAlignment.HasValue)
                    return ListItem.CellVerticalAlignment.Value;

                if (Column != null && Column.CellVerticalAlignment.HasValue)
                    return Column.CellVerticalAlignment.Value;

                if (ListView != null)
                    return ListView.CellVerticalAlignment;

                return StringAlignment.Center;
            }
        }

        /// <summary>
        /// Gets or sets the image list from which keyed images will be fetched
        /// </summary>
        [Category("Appearance"),
         Description("The image list from which keyed images will be fetched for drawing. If this is not given, the small ImageList from the ObjectListView will be used"),
         DefaultValue(null)]
        public ImageList ImageList {
            get { return imageList; }
            set { imageList = value; }
        }

        private ImageList imageList;

        /// <summary>
        /// When rendering multiple images, how many pixels should be between each image?
        /// </summary>
        [Category("Appearance"),
         Description("When rendering multiple images, how many pixels should be between each image?"),
         DefaultValue(1)]
        public int Spacing {
            get { return spacing; }
            set { spacing = value; }
        }

        private int spacing = 1;

        /// <summary>
        /// Should text be rendered using GDI routines? This makes the text look more
        /// like a native List view control.
        /// </summary>
        [Category("Appearance"),
         Description("Should text be rendered using GDI routines?"),
         DefaultValue(true)]
        public virtual bool UseGdiTextRendering {
            get {
                // Can't use GDI routines on a GDI+ printer context
                return !IsPrinting && useGdiTextRendering;
            }
            set { useGdiTextRendering = value; }
        }
        private bool useGdiTextRendering = true;

        #endregion

        #region State Properties

        /// <summary>
        /// Get or set the aspect of the model object that this renderer should draw
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object Aspect {
            get {
                if (aspect == null)
                    aspect = column.GetValue(rowObject);
                return aspect;
            }
            set { aspect = value; }
        }

        private Object aspect;

        /// <summary>
        /// What are the bounds of the cell that is being drawn?
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle Bounds {
            get { return bounds; }
            set { bounds = value; }
        }

        private Rectangle bounds;

        /// <summary>
        /// Get or set the OLVColumn that this renderer will draw
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OLVColumn Column {
            get { return column; }
            set { column = value; }
        }

        private OLVColumn column;

        /// <summary>
        /// Get/set the event that caused this renderer to be called
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DrawListViewItemEventArgs DrawItemEvent {
            get { return drawItemEventArgs; }
            set { drawItemEventArgs = value; }
        }

        private DrawListViewItemEventArgs drawItemEventArgs;

        /// <summary>
        /// Get/set the event that caused this renderer to be called
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DrawListViewSubItemEventArgs Event {
            get { return eventArgs; }
            set { eventArgs = value; }
        }

        private DrawListViewSubItemEventArgs eventArgs;

        /// <summary>
        /// Gets or  sets the font to be used for text in this cell
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font Font {
            get {
                if (font != null || ListItem == null)
                    return font;

                if (SubItem == null || ListItem.UseItemStyleForSubItems)
                    return ListItem.Font;

                return SubItem.Font;
            }
            set { font = value; }
        }

        private Font font;

        /// <summary>
        /// Gets the image list from which keyed images will be fetched
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageList ImageListOrDefault {
            get { return ImageList ?? ListView.SmallImageList; }
        }

        /// <summary>
        /// Should this renderer fill in the background before drawing?
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDrawBackground {
            get { return !IsPrinting; }
        }

        /// <summary>
        /// Cache whether or not our item is selected
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsItemSelected {
            get { return isItemSelected; }
            set { isItemSelected = value; }
        }

        private bool isItemSelected;

        /// <summary>
        /// Is this renderer being used on a printer context?
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPrinting {
            get { return isPrinting; }
            set { isPrinting = value; }
        }

        private bool isPrinting;

        /// <summary>
        /// Get or set the listitem that this renderer will be drawing
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OLVListItem ListItem {
            get { return listItem; }
            set { listItem = value; }
        }

        private OLVListItem listItem;

        /// <summary>
        /// Get/set the listview for which the drawing is to be done
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectListView ListView {
            get { return objectListView; }
            set { objectListView = value; }
        }

        private ObjectListView objectListView;

        /// <summary>
        /// Get the specialized OLVSubItem that this renderer is drawing
        /// </summary>
        /// <remarks>This returns null for column 0.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OLVListSubItem OLVSubItem {
            get { return listSubItem; }
        }

        /// <summary>
        /// Get or set the model object that this renderer should draw
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object RowObject {
            get { return rowObject; }
            set { rowObject = value; }
        }

        private Object rowObject;

        /// <summary>
        /// Get or set the list subitem that this renderer will be drawing
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OLVListSubItem SubItem {
            get { return listSubItem; }
            set { listSubItem = value; }
        }

        private OLVListSubItem listSubItem;

        /// <summary>
        /// The brush that will be used to paint the text
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush TextBrush {
            get {
                if (textBrush == null)
                    return new SolidBrush(GetForegroundColor());
                else
                    return textBrush;
            }
            set { textBrush = value; }
        }

        private Brush textBrush;

        /// <summary>
        /// Will this renderer use the custom images from the parent ObjectListView
        /// to draw the checkbox images.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is true, the renderer will use the images from the 
        /// StateImageList to represent checkboxes. 0 - unchecked, 1 - checked, 2 - indeterminate.
        /// </para>
        /// <para>If this is false (the default), then the renderer will use .NET's standard
        /// CheckBoxRenderer.</para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseCustomCheckboxImages {
            get { return useCustomCheckboxImages; }
            set { useCustomCheckboxImages = value; }
        }

        private bool useCustomCheckboxImages;

        private void ClearState() {
            Event = null;
            DrawItemEvent = null;
            Aspect = null;
            Font = null;
            TextBrush = null;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Align the second rectangle with the first rectangle,
        /// according to the alignment of the column
        /// </summary>
        /// <param name="outer">The cell's bounds</param>
        /// <param name="inner">The rectangle to be aligned within the bounds</param>
        /// <returns>An aligned rectangle</returns>
        protected virtual Rectangle AlignRectangle(Rectangle outer, Rectangle inner) {
            Rectangle r = new Rectangle(outer.Location, inner.Size);

            // Align horizontally depending on the column alignment
            if (inner.Width < outer.Width) {
                r.X = AlignHorizontally(outer, inner);
            }

            // Align vertically too
            if (inner.Height < outer.Height) {
                r.Y = AlignVertically(outer, inner);
            }

            return r;
        }

        /// <summary>
        /// Calculate the left edge of the rectangle that aligns the outer rectangle with the inner one 
        /// according to this renderer's horizontal alignment
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        protected int AlignHorizontally(Rectangle outer, Rectangle inner) {
            HorizontalAlignment alignment = CellHorizontalAlignment;
            switch (alignment) {
                case HorizontalAlignment.Left:
                    return outer.Left + 1;
                case HorizontalAlignment.Center:
                    return outer.Left + ((outer.Width - inner.Width) / 2);
                case HorizontalAlignment.Right:
                    return outer.Right - inner.Width - 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// Calculate the top of the rectangle that aligns the outer rectangle with the inner rectangle
        /// according to this renders vertical alignment
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        protected int AlignVertically(Rectangle outer, Rectangle inner) {
            return AlignVertically(outer, inner.Height);
        }

        /// <summary>
        /// Calculate the top of the rectangle that aligns the outer rectangle with a rectangle of the given height
        /// according to this renderer's vertical alignment
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="innerHeight"></param>
        /// <returns></returns>
        protected int AlignVertically(Rectangle outer, int innerHeight) {
            switch (EffectiveCellVerticalAlignment) {
                case StringAlignment.Near:
                    return outer.Top + 1;
                case StringAlignment.Center:
                    return outer.Top + ((outer.Height - innerHeight) / 2);
                case StringAlignment.Far:
                    return outer.Bottom - innerHeight - 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Calculate the space that our rendering will occupy and then align that space
        /// with the given rectangle, according to the Column alignment
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r">Pre-padded bounds of the cell</param>
        /// <returns></returns>
        protected virtual Rectangle CalculateAlignedRectangle(Graphics g, Rectangle r) {
            if (Column == null)
                return r;

            Rectangle contentRectangle = new Rectangle(Point.Empty, CalculateContentSize(g, r));
            return AlignRectangle(r, contentRectangle);
        }

        /// <summary>
        /// Calculate the size of the content of this cell.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r">Pre-padded bounds of the cell</param>
        /// <returns>The width and height of the content</returns>
        protected virtual Size CalculateContentSize(Graphics g, Rectangle r)
        {
            Size checkBoxSize = CalculatePrimaryCheckBoxSize(g);
            Size imageSize = CalculateImageSize(g, GetImageSelector());
            Size textSize = CalculateTextSize(g, GetText(), r.Width - (checkBoxSize.Width + imageSize.Width));

            // If the combined width is greater than the whole cell,  we just use the cell itself

            int width = Math.Min(r.Width, checkBoxSize.Width + imageSize.Width + textSize.Width);
            int componentMaxHeight = Math.Max(checkBoxSize.Height, Math.Max(imageSize.Height, textSize.Height));
            int height = Math.Min(r.Height, componentMaxHeight);

            return new Size(width, height);
        }

        /// <summary>
        /// Calculate the bounds of a checkbox given the (pre-padded) cell bounds
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds">Pre-padded cell bounds</param>
        /// <returns></returns>
        protected Rectangle CalculateCheckBoxBounds(Graphics g, Rectangle cellBounds) {
            Size checkBoxSize = CalculateCheckBoxSize(g);
            return AlignRectangle(cellBounds, new Rectangle(0, 0, checkBoxSize.Width, checkBoxSize.Height));
        }
        
        
        /// <summary>
        /// How much space will the check box for this cell occupy?
        /// </summary>
        /// <remarks>Only column 0 can have check boxes. Sub item checkboxes are
        /// treated as images</remarks>
        /// <param name="g"></param>
        /// <returns></returns>
        protected virtual Size CalculateCheckBoxSize(Graphics g)
        {
            if (UseCustomCheckboxImages && ListView.StateImageList != null)
                return ListView.StateImageList.ImageSize;

            return CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
        }

        /// <summary>
        /// How much space will the check box for this row occupy? 
        /// If the list doesn't have checkboxes, or this isn't the primary column,
        /// this returns an empty size.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        protected virtual Size CalculatePrimaryCheckBoxSize(Graphics g) {
            if (!ListView.CheckBoxes || !ColumnIsPrimary)
                return Size.Empty;
            
            Size size = CalculateCheckBoxSize(g);
            size.Width += 6;
            return size;
        }

        /// <summary>
        /// How much horizontal space will the image of this cell occupy?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="imageSelector"></param>
        /// <returns></returns>
        protected virtual int CalculateImageWidth(Graphics g, object imageSelector)
        {
            return CalculateImageSize(g, imageSelector).Width + 2;
        }

        /// <summary>
        /// How much vertical space will the image of this cell occupy?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="imageSelector"></param>
        /// <returns></returns>
        protected virtual int CalculateImageHeight(Graphics g, object imageSelector)
        {
            return CalculateImageSize(g, imageSelector).Height;
        }

        /// <summary>
        /// How much space will the image of this cell occupy?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="imageSelector"></param>
        /// <returns></returns>
        protected virtual Size CalculateImageSize(Graphics g, object imageSelector)
        {
            if (imageSelector == null || imageSelector == DBNull.Value)
                return Size.Empty;

            // Check for the image in the image list (most common case)
            ImageList il = ImageListOrDefault;
            if (il != null)
            {
                int selectorAsInt = -1;

                if (imageSelector is Int32)
                    selectorAsInt = (Int32)imageSelector;
                else
                {
                    if (imageSelector is string selectorAsString)
                        selectorAsInt = il.Images.IndexOfKey(selectorAsString);
                }
                if (selectorAsInt >= 0)
                    return il.ImageSize;
            }

            // Is the selector actually an image?
            if (imageSelector is Image image)
                return image.Size;

            return Size.Empty;
        }

        /// <summary>
        /// How much horizontal space will the text of this cell occupy?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="txt"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        protected virtual int CalculateTextWidth(Graphics g, string txt, int width)
        {
            if (String.IsNullOrEmpty(txt))
                return 0;

            return CalculateTextSize(g, txt, width).Width;
        }

        /// <summary>
        /// How much space will the text of this cell occupy?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="txt"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        protected virtual Size CalculateTextSize(Graphics g, string txt, int width)
        {
            if (String.IsNullOrEmpty(txt))
                return Size.Empty;

            if (UseGdiTextRendering)
            {
                Size proposedSize = new Size(width, Int32.MaxValue);
                return TextRenderer.MeasureText(g, txt, Font, proposedSize, NormalTextFormatFlags);
            }
            
            // Using GDI+ renderering
            using (StringFormat fmt = new StringFormat()) {
                fmt.Trimming = StringTrimming.EllipsisCharacter;
                SizeF sizeF = g.MeasureString(txt, Font, width, fmt);
                return new Size(1 + (int)sizeF.Width, 1 + (int)sizeF.Height);
            }
        }

        /// <summary>
        /// Return the Color that is the background color for this item's cell
        /// </summary>
        /// <returns>The background color of the subitem</returns>
        public virtual Color GetBackgroundColor() {
            if (!ListView.Enabled)
                return SystemColors.Control;

            if (IsItemSelected && !ListView.UseTranslucentSelection && ListView.FullRowSelect)
                return GetSelectedBackgroundColor();

            if (SubItem == null || ListItem.UseItemStyleForSubItems)
                return ListItem.BackColor;

            return SubItem.BackColor;
        }

        /// <summary>
        /// Return the color of the background color when the item is selected
        /// </summary>
        /// <returns>The background color of the subitem</returns>
        public virtual Color GetSelectedBackgroundColor() {
            if (ListView.Focused) 
                return ListItem.SelectedBackColor ?? ListView.SelectedBackColorOrDefault;

            if (!ListView.HideSelection)
                return ListView.UnfocusedSelectedBackColorOrDefault;

            return ListItem.BackColor;
        }

        /// <summary>
        /// Return the color to be used for text in this cell
        /// </summary>
        /// <returns>The text color of the subitem</returns>
        public virtual Color GetForegroundColor() {
            if (IsItemSelected && 
                !ListView.UseTranslucentSelection &&
                (ColumnIsPrimary || ListView.FullRowSelect)) 
                return GetSelectedForegroundColor();

            return SubItem == null || ListItem.UseItemStyleForSubItems ? ListItem.ForeColor : SubItem.ForeColor;
        }

        /// <summary>
        /// Return the color of the foreground color when the item is selected
        /// </summary>
        /// <returns>The foreground color of the subitem</returns>
        public virtual Color GetSelectedForegroundColor()
        {
            if (ListView.Focused)
                return ListItem.SelectedForeColor ?? ListView.SelectedForeColorOrDefault;

            if (!ListView.HideSelection)
                return ListView.UnfocusedSelectedForeColorOrDefault;

            return SubItem == null || ListItem.UseItemStyleForSubItems ? ListItem.ForeColor : SubItem.ForeColor;
        }

        /// <summary>
        /// Return the image that should be drawn against this subitem
        /// </summary>
        /// <returns>An Image or null if no image should be drawn.</returns>
        protected virtual Image GetImage() {
            return GetImage(GetImageSelector());
        }

        /// <summary>
        /// Return the actual image that should be drawn when keyed by the given image selector.
        /// An image selector can be: <list type="bullet">
        /// <item><description>an int, giving the index into the image list</description></item>
        /// <item><description>a string, giving the image key into the image list</description></item>
        /// <item><description>an Image, being the image itself</description></item>
        /// </list>
        /// </summary>
        /// <param name="imageSelector">The value that indicates the image to be used</param>
        /// <returns>An Image or null</returns>
        protected virtual Image GetImage(Object imageSelector) {
            if (imageSelector == null || imageSelector == DBNull.Value)
                return null;

            ImageList il = ImageListOrDefault;
            if (il != null) {
                if (imageSelector is Int32) {
                    Int32 index = (Int32) imageSelector;
                    if (index < 0 || index >= il.Images.Count)
                        return null;

                    return il.Images[index];
                }

                if (imageSelector is string str) {
                    if (il.Images.ContainsKey(str))
                        return il.Images[str];

                    return null;
                }
            }

            return imageSelector as Image;
        }

        /// <summary>
        /// </summary>
        protected virtual Object GetImageSelector() {
            return ColumnIsPrimary ? ListItem.ImageSelector : OLVSubItem.ImageSelector;
        }

        /// <summary>
        /// Return the string that should be drawn within this
        /// </summary>
        /// <returns></returns>
        protected virtual string GetText() {
            return SubItem == null ? ListItem.Text : SubItem.Text;
        }

        /// <summary>
        /// Return the Color that is the background color for this item's text
        /// </summary>
        /// <returns>The background color of the subitem's text</returns>
        [Obsolete("Use GetBackgroundColor() instead")]
        protected virtual Color GetTextBackgroundColor() {
            return Color.Red; // just so it shows up if it is used
        }

        #endregion

        #region IRenderer members

        /// <summary>
        /// Render the whole item in a non-details view.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="g"></param>
        /// <param name="itemBounds"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, object model) {
            ConfigureItem(e, itemBounds, model);
            return OptionalRender(g, itemBounds);
        }

        /// <summary>
        /// Prepare this renderer to draw in response to the given event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="itemBounds"></param>
        /// <param name="model"></param>
        /// <remarks>Use this if you want to chain a second renderer within a primary renderer.</remarks>
        public virtual void ConfigureItem(DrawListViewItemEventArgs e, Rectangle itemBounds, object model)
        {
            ClearState();

            DrawItemEvent = e;
            ListItem = (OLVListItem)e.Item;
            SubItem = null;
            ListView = (ObjectListView)ListItem.ListView;
            Column = ListView.GetColumn(0);
            RowObject = model;
            Bounds = itemBounds;
            IsItemSelected = ListItem.Selected && ListItem.Enabled;
        }

        /// <summary>
        /// Render one cell
        /// </summary>
        /// <param name="e"></param>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool RenderSubItem(DrawListViewSubItemEventArgs e, Graphics g, Rectangle cellBounds, object model) {
            ConfigureSubItem(e, cellBounds, model);
            return OptionalRender(g, cellBounds);
        }

        /// <summary>
        /// Prepare this renderer to draw in response to the given event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="cellBounds"></param>
        /// <param name="model"></param>
        /// <remarks>Use this if you want to chain a second renderer within a primary renderer.</remarks>
        public virtual void ConfigureSubItem(DrawListViewSubItemEventArgs e, Rectangle cellBounds, object model) {
            ClearState();

            Event = e;
            ListItem = (OLVListItem)e.Item;
            SubItem = (OLVListSubItem)e.SubItem;
            ListView = (ObjectListView)ListItem.ListView;
            Column = (OLVColumn)e.Header;
            RowObject = model;
            Bounds = cellBounds;
            IsItemSelected = ListItem.Selected && ListItem.Enabled;
        }

        /// <summary>
        /// Calculate which part of this cell was hit
        /// </summary>
        /// <param name="hti"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void HitTest(OlvListViewHitTestInfo hti, int x, int y) {
            ClearState();

            ListView = hti.ListView;
            ListItem = hti.Item;
            SubItem = hti.SubItem;
            Column = hti.Column;
            RowObject = hti.RowObject;
            IsItemSelected = ListItem.Selected && ListItem.Enabled;
            if (SubItem == null)
                Bounds = ListItem.Bounds;
            else
                Bounds = ListItem.GetSubItemBounds(Column.Index);

            using (Graphics g = ListView.CreateGraphics()) {
                HandleHitTest(g, hti, x, y);
            }
        }

        /// <summary>
        /// Calculate the edit rectangle
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        public override Rectangle GetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize) {
            ClearState();

            ListView = (ObjectListView) item.ListView;
            ListItem = item;
            SubItem = item.GetSubItem(subItemIndex);
            Column = ListView.GetColumn(subItemIndex);
            RowObject = item.RowObject;
            IsItemSelected = ListItem.Selected && ListItem.Enabled;
            Bounds = cellBounds;

            return HandleGetEditRectangle(g, cellBounds, item, subItemIndex, preferredSize);
        }

        #endregion

        #region IRenderer implementation

        // Subclasses will probably want to override these methods rather than the IRenderer
        // interface methods.

        /// <summary>
        /// Draw our data into the given rectangle using the given graphics context.
        /// </summary>
        /// <remarks>
        /// <para>Subclasses should override this method.</para></remarks>
        /// <param name="g">The graphics context that should be used for drawing</param>
        /// <param name="r">The bounds of the subitem cell</param>
        /// <returns>Returns whether the rendering has already taken place.
        /// If this returns false, the default processing will take over.
        /// </returns>
        public virtual bool OptionalRender(Graphics g, Rectangle r) {
            if (ListView.View != View.Details)
                return false;

            Render(g, r);
            return true;
        }

        /// <summary>
        /// Draw our data into the given rectangle using the given graphics context.
        /// </summary>
        /// <remarks>
        /// <para>Subclasses should override this method if they never want
        /// to fall back on the default processing</para></remarks>
        /// <param name="g">The graphics context that should be used for drawing</param>
        /// <param name="r">The bounds of the subitem cell</param>
        public virtual void Render(Graphics g, Rectangle r) {
            StandardRender(g, r);
        }

        /// <summary>
        /// Do the actual work of hit testing. Subclasses should override this rather than HitTest()
        /// </summary>
        /// <param name="g"></param>
        /// <param name="hti"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected virtual void HandleHitTest(Graphics g, OlvListViewHitTestInfo hti, int x, int y) {
            Rectangle r = CalculateAlignedRectangle(g, ApplyCellPadding(Bounds));
            StandardHitTest(g, hti, r, x, y);
        }

        /// <summary>
        /// Handle a HitTest request after all state information has been initialized
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        protected virtual Rectangle HandleGetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize) {
            // MAINTAINER NOTE: This type testing is wrong (design-wise). The base class should return cell bounds,
            // and a more specialized class should return StandardGetEditRectangle(). But BaseRenderer is used directly
            // to draw most normal cells, as well as being directly subclassed for user implemented renderers. And this
            // method needs to return different bounds in each of those cases. We should have a StandardRenderer and make
            // BaseRenderer into an ABC -- but that would break too much existing code. And so we have this hack :(

            // If we are a standard renderer, return the position of the text, otherwise, use the whole cell.
            if (GetType() == typeof (BaseRenderer))
                return StandardGetEditRectangle(g, cellBounds, preferredSize);

            // Center the editor vertically
            if (cellBounds.Height != preferredSize.Height)
                cellBounds.Y += (cellBounds.Height - preferredSize.Height) / 2;

            return cellBounds;
        }

        #endregion

        #region Standard IRenderer implementations

        /// <summary>
        /// Draw the standard "[checkbox] [image] [text]" cell after the state properties have been initialized.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        protected void StandardRender(Graphics g, Rectangle r) {
            DrawBackground(g, r);

            // Adjust the first columns rectangle to match the padding used by the native mode of the ListView
            if (ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left ) {
                r.X += 3;
                r.Width -= 1;
            }
            r = ApplyCellPadding(r);
            DrawAlignedImageAndText(g, r);

            // Show where the bounds of the cell padding are (debugging)
            if (ObjectListView.ShowCellPaddingBounds)
                g.DrawRectangle(Pens.Purple, r);
        }

        /// <summary>
        /// Change the bounds of the given rectangle to take any cell padding into account
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual Rectangle ApplyCellPadding(Rectangle r) {
            Rectangle? padding = EffectiveCellPadding;
            if (!padding.HasValue)
                return r;
            // The two subtractions below look wrong, but are correct!
            Rectangle paddingRectangle = padding.Value;
            r.Width -= paddingRectangle.Right;
            r.Height -= paddingRectangle.Bottom;
            r.Offset(paddingRectangle.Location);
            return r;
        }

        /// <summary>
        /// Perform normal hit testing relative to the given aligned content bounds
        /// </summary>
        /// <param name="g"></param>
        /// <param name="hti"></param>
        /// <param name="bounds"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected virtual void StandardHitTest(Graphics g, OlvListViewHitTestInfo hti, Rectangle alignedContentRectangle, int x, int y) {
            Rectangle r = alignedContentRectangle;

            // Match tweaking from renderer
            if (ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left && !(this is TreeListView.TreeRenderer)) {
                r.X += 3;
                r.Width -= 1;
            }
            int width = 0;

            // Did they hit a check box on the primary column?
            if (ColumnIsPrimary && ListView.CheckBoxes) {
                Size checkBoxSize = CalculateCheckBoxSize(g);
                int checkBoxTop = AlignVertically(r, checkBoxSize.Height);
                Rectangle r3 = new Rectangle(r.X, checkBoxTop, checkBoxSize.Width, checkBoxSize.Height);
                width = r3.Width + 6;
                // g.DrawRectangle(Pens.DarkGreen, r3);
                if (r3.Contains(x, y)) {
                    hti.HitTestLocation = HitTestLocation.CheckBox;
                    return;
                }
            }

            // Did they hit the image? If they hit the image of a 
            // non-primary column that has a checkbox, it counts as a 
            // checkbox hit
            r.X += width;
            r.Width -= width;
            width = CalculateImageWidth(g, GetImageSelector());
            Rectangle rTwo = r;
            rTwo.Width = width;
            // g.DrawRectangle(Pens.Red, rTwo);
            if (rTwo.Contains(x, y)) {
                if (Column != null && (Column.Index > 0 && Column.CheckBoxes))
                    hti.HitTestLocation = HitTestLocation.CheckBox;
                else
                    hti.HitTestLocation = HitTestLocation.Image;
                return;
            }

            // Did they hit the text?
            r.X += width;
            r.Width -= width;
            width = CalculateTextWidth(g, GetText(), r.Width);
            rTwo = r;
            rTwo.Width = width;
            // g.DrawRectangle(Pens.Blue, rTwo);
            if (rTwo.Contains(x, y)) {
                hti.HitTestLocation = HitTestLocation.Text;
                return;
            }

            hti.HitTestLocation = HitTestLocation.InCell;
        }

        /// <summary>
        /// This method calculates the bounds of the text within a standard layout
        /// (i.e. optional checkbox, optional image, text)
        /// </summary>
        /// <remarks>This method only works correctly if the state of the renderer
        /// has been fully initialized (see BaseRenderer.GetEditRectangle)</remarks>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        protected virtual Rectangle StandardGetEditRectangle(Graphics g, Rectangle cellBounds, Size preferredSize) {

            Size contentSize = CalculateContentSize(g, cellBounds);
            int contentWidth = Column.CellEditUseWholeCellEffective ? cellBounds.Width : contentSize.Width;
            Rectangle editControlBounds = CalculatePaddedAlignedBounds(g, cellBounds, new Size(contentWidth, preferredSize.Height));

            Size checkBoxSize = CalculatePrimaryCheckBoxSize(g);
            int imageWidth = CalculateImageWidth(g, GetImageSelector());

            int width = checkBoxSize.Width + imageWidth;

            // Indent the primary column by the required amount
            if (ListItem.IndentCount > 0) {
                int indentWidth = ListView.SmallImageSize.Width * ListItem.IndentCount;
                width += indentWidth;
            }

            editControlBounds.X += width;
            editControlBounds.Width -= width;

            if (editControlBounds.Width < 50)
                editControlBounds.Width = 50;
            if (editControlBounds.Right > cellBounds.Right)
                editControlBounds.Width = cellBounds.Right - editControlBounds.Left;

            return editControlBounds;
        }

        /// <summary>
        /// Apply any padding to the given bounds, and then align a rectangle of the given
        /// size within that padded area.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="preferredSize"></param>
        /// <returns></returns>
        protected Rectangle CalculatePaddedAlignedBounds(Graphics g, Rectangle cellBounds, Size preferredSize) {
            Rectangle r = ApplyCellPadding(cellBounds);
            r = AlignRectangle(r, new Rectangle(Point.Empty, preferredSize));
            return r;
        }

        #endregion

        #region Drawing routines

        /// <summary>
        /// Draw the given image aligned horizontally within the column.
        /// </summary>
        /// <remarks>
        /// Over tall images are scaled to fit. Over-wide images are
        /// truncated. This is by design!
        /// </remarks>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">Bounds of the cell</param>
        /// <param name="image">The image to be drawn</param>
        protected virtual void DrawAlignedImage(Graphics g, Rectangle r, Image image) {
            if (image == null)
                return;

            // By default, the image goes in the top left of the rectangle
            Rectangle imageBounds = new Rectangle(r.Location, image.Size);

            // If the image is too tall to be drawn in the space provided, proportionally scale it down.
            // Too wide images are not scaled.
            if (image.Height > r.Height) {
                float scaleRatio = (float) r.Height / (float) image.Height;
                imageBounds.Width = (int) ((float) image.Width * scaleRatio);
                imageBounds.Height = r.Height - 1;
            }

            // Align and draw our (possibly scaled) image
            Rectangle alignRectangle = AlignRectangle(r, imageBounds);
            if (ListItem.Enabled)
                g.DrawImage(image, alignRectangle);
            else
                ControlPaint.DrawImageDisabled(g, image, alignRectangle.X, alignRectangle.Y, GetBackgroundColor());
        }

        /// <summary>
        /// Draw our subitems image and text
        /// </summary>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">Pre-padded bounds of the cell</param>
        protected virtual void DrawAlignedImageAndText(Graphics g, Rectangle r)
        {
            DrawImageAndText(g, r); //this.CalculateAlignedRectangle(g, r));
        }

        /// <summary>
        /// Fill in the background of this cell
        /// </summary>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">Bounds of the cell</param>
        protected virtual void DrawBackground(Graphics g, Rectangle r) {
            if (!IsDrawBackground)
                return;

            Color backgroundColor = GetBackgroundColor();

            using (Brush brush = new SolidBrush(backgroundColor)) {
                g.FillRectangle(brush, r.X - 1, r.Y - 1, r.Width + 2, r.Height + 2);
            }
        }

        /// <summary>
        /// Draw the primary check box of this row (checkboxes in other sub items use a different method)
        /// </summary>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">The pre-aligned and padded target rectangle</param>
        protected virtual int DrawCheckBox(Graphics g, Rectangle r) {
            // The odd constants are to match checkbox placement in native mode (on XP at least)
            // TODO: Unify this with CheckStateRenderer

            // The rectangle r is already horizontally aligned. We still need to align it vertically.
            Size checkBoxSize = CalculateCheckBoxSize(g);
            Point checkBoxLocation = new Point(r.X, AlignVertically(r, checkBoxSize.Height));

            if (IsPrinting || UseCustomCheckboxImages) {
                int imageIndex = ListItem.StateImageIndex;
                if (ListView.StateImageList == null || imageIndex < 0 || imageIndex >= ListView.StateImageList.Images.Count)
                    return 0;

                return DrawImage(g, new Rectangle(checkBoxLocation, checkBoxSize), ListView.StateImageList.Images[imageIndex]) + 4;
            }

            CheckBoxState boxState = GetCheckBoxState(ListItem.CheckState);
            CheckBoxRenderer.DrawCheckBox(g, checkBoxLocation, boxState);
            return checkBoxSize.Width;
        }

        /// <summary>
        /// Calculate the CheckBoxState we need to correctly draw the given state
        /// </summary>
        /// <param name="checkState"></param>
        /// <returns></returns>
        protected virtual CheckBoxState GetCheckBoxState(CheckState checkState) {

            // Should the checkbox be drawn as disabled?
            if (IsCheckBoxDisabled) {
                switch (checkState) {
                    case CheckState.Checked:
                        return CheckBoxState.CheckedDisabled;
                    case CheckState.Unchecked:
                        return CheckBoxState.UncheckedDisabled;
                    default:
                        return CheckBoxState.MixedDisabled;
                }
            }

            // Is the cursor currently over this checkbox?
            if (IsCheckboxHot) {
                switch (checkState) {
                    case CheckState.Checked:
                        return CheckBoxState.CheckedHot;
                    case CheckState.Unchecked:
                        return CheckBoxState.UncheckedHot;
                    default:
                        return CheckBoxState.MixedHot;
                }
            }

            // Not hot and not disabled -- just draw it normally
            switch (checkState) {
                case CheckState.Checked:
                    return CheckBoxState.CheckedNormal;
                case CheckState.Unchecked:
                    return CheckBoxState.UncheckedNormal;
                default:
                    return CheckBoxState.MixedNormal;
            }

        }

        /// <summary>
        /// Should this checkbox be drawn as disabled?
        /// </summary>
        protected virtual bool IsCheckBoxDisabled {
            get {
                if (ListItem != null && !ListItem.Enabled)
                    return true;

                if (!ListView.RenderNonEditableCheckboxesAsDisabled)
                    return false;

                return (ListView.CellEditActivation == ObjectListView.CellEditActivateMode.None ||
                        (Column != null && !Column.IsEditable));
            }
        }

        /// <summary>
        /// Is the current item hot (i.e. under the mouse)?
        /// </summary>
        protected bool IsCellHot {
            get {
                return ListView != null &&
                       ListView.HotRowIndex == ListItem.Index &&
                       ListView.HotColumnIndex == (Column == null ? 0 : Column.Index);
            }
        }

        /// <summary>
        /// Is the mouse over a checkbox in this cell?
        /// </summary>
        protected bool IsCheckboxHot {
            get {
                return IsCellHot && ListView.HotCellHitLocation == HitTestLocation.CheckBox;
            }
        }

        /// <summary>
        /// Draw the given text and optional image in the "normal" fashion
        /// </summary>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">Bounds of the cell</param>
        /// <param name="imageSelector">The optional image to be drawn</param>
        protected virtual int DrawImage(Graphics g, Rectangle r, Object imageSelector) {
            if (imageSelector == null || imageSelector == DBNull.Value)
                return 0;

            // Draw from the image list (most common case)
            ImageList il = ImageListOrDefault;
            if (il != null) {

                // Try to translate our imageSelector into a valid ImageList index
                int selectorAsInt = -1;
                if (imageSelector is Int32) {
                    selectorAsInt = (Int32) imageSelector;
                    if (selectorAsInt >= il.Images.Count)
                        selectorAsInt = -1;
                } else {
                    if (imageSelector is string selectorAsString)
                        selectorAsInt = il.Images.IndexOfKey(selectorAsString);
                }

                // If we found a valid index into the ImageList, draw it.
                // We want to draw using the native DrawImageList calls, since that let's us do some nice effects
                // But the native call does not work on PrinterDCs, so if we're printing we have to skip this bit.
                if (selectorAsInt >= 0) {
                    if (!IsPrinting) {
                        if (il.ImageSize.Height < r.Height)
                            r.Y = AlignVertically(r, new Rectangle(Point.Empty, il.ImageSize));

                        // If we are not printing, it's probable that the given Graphics object is double buffered using a BufferedGraphics object.
                        // But the ImageList.Draw method doesn't honor the Translation matrix that's probably in effect on the buffered
                        // graphics. So we have to calculate our drawing rectangle, relative to the cells natural boundaries.
                        // This effectively simulates the Translation matrix.

                        Rectangle r2 = new Rectangle(r.X - Bounds.X, r.Y - Bounds.Y, r.Width, r.Height);
                        NativeMethods.DrawImageList(g, il, selectorAsInt, r2.X, r2.Y, IsItemSelected, !ListItem.Enabled);
                        return il.ImageSize.Width;
                    }

                    // For some reason, printing from an image list doesn't work onto a printer context
                    // So get the image from the list and FALL THROUGH to the "print an image" case
                    imageSelector = il.Images[selectorAsInt];
                }
            }

            // Is the selector actually an image?
            if (imageSelector is not Image image)
                return 0; // no, give up

            if (image.Size.Height < r.Height)
                r.Y = AlignVertically(r, new Rectangle(Point.Empty, image.Size));

            if (ListItem.Enabled)
                g.DrawImageUnscaled(image, r.X, r.Y);
            else
                ControlPaint.DrawImageDisabled(g, image, r.X, r.Y, GetBackgroundColor());

            return image.Width;
        }

        /// <summary>
        /// Draw our subitems image and text
        /// </summary>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">Bounds of the cell</param>
        protected virtual void DrawImageAndText(Graphics g, Rectangle r) {
            int offset = 0;
            if (ListView.CheckBoxes && ColumnIsPrimary) {
                offset = DrawCheckBox(g, r) + 6;
                r.X += offset;
                r.Width -= offset;
            }

            offset = DrawImage(g, r, GetImageSelector());
            r.X += offset;
            r.Width -= offset;

            DrawText(g, r, GetText());
        }

        /// <summary>
        /// Draw the given collection of image selectors
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="imageSelectors"></param>
        protected virtual int DrawImages(Graphics g, Rectangle r, ICollection imageSelectors) {
            // Collect the non-null images
            List<Image> images = new List<Image>();
            foreach (Object selector in imageSelectors) {
                Image image = GetImage(selector);
                if (image != null)
                    images.Add(image);
            }

            // Figure out how much space they will occupy
            int width = 0;
            int height = 0;
            foreach (Image image in images) {
                width += (image.Width + Spacing);
                height = Math.Max(height, image.Height);
            }

            // Align the collection of images within the cell
            Rectangle r2 = AlignRectangle(r, new Rectangle(0, 0, width, height));

            // Finally, draw all the images in their correct location
            Color backgroundColor = GetBackgroundColor();
            Point pt = r2.Location;
            foreach (Image image in images) {
                if (ListItem.Enabled)
                    g.DrawImage(image, pt);
                else
                    ControlPaint.DrawImageDisabled(g, image, pt.X, pt.Y, backgroundColor);
                pt.X += (image.Width + Spacing);
            }

            // Return the width that the images occupy
            return width;
        }

        /// <summary>
        /// Draw the given text and optional image in the "normal" fashion
        /// </summary>
        /// <param name="g">Graphics context to use for drawing</param>
        /// <param name="r">Bounds of the cell</param>
        /// <param name="txt">The string to be drawn</param>
        public virtual void DrawText(Graphics g, Rectangle r, String txt) {
            if (String.IsNullOrEmpty(txt))
                return;

            if (UseGdiTextRendering)
                DrawTextGdi(g, r, txt);
            else
                DrawTextGdiPlus(g, r, txt);
        }

        /// <summary>
        /// Print the given text in the given rectangle using only GDI routines
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="txt"></param>
        /// <remarks>
        /// The native list control uses GDI routines to do its drawing, so using them
        /// here makes the owner drawn mode looks more natural.
        /// <para>This method doesn't honour the CanWrap setting on the renderer. All
        /// text is single line</para>
        /// </remarks>
        protected virtual void DrawTextGdi(Graphics g, Rectangle r, String txt) {
            Color backColor = Color.Transparent;
            if (IsDrawBackground && IsItemSelected && ColumnIsPrimary && !ListView.FullRowSelect)
                backColor = GetSelectedBackgroundColor();

            TextFormatFlags flags = NormalTextFormatFlags | CellVerticalAlignmentAsTextFormatFlag;

            // I think there is a bug in the TextRenderer. Setting or not setting SingleLine doesn't make 
            // any difference -- it is always single line.
            if (!CanWrap)
                flags |= TextFormatFlags.SingleLine;
            TextRenderer.DrawText(g, txt, Font, r, GetForegroundColor(), backColor, flags);
        }

        private bool ColumnIsPrimary {
            get { return Column != null && Column.Index == 0; }
        }

        /// <summary>
        /// Gets the cell's vertical alignment as a TextFormatFlag
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected TextFormatFlags CellVerticalAlignmentAsTextFormatFlag {
            get {
                switch (EffectiveCellVerticalAlignment) {
                    case StringAlignment.Near:
                        return TextFormatFlags.Top;
                    case StringAlignment.Center:
                        return TextFormatFlags.VerticalCenter;
                    case StringAlignment.Far:
                        return TextFormatFlags.Bottom;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets the StringFormat needed when drawing text using GDI+
        /// </summary>
        protected virtual StringFormat StringFormatForGdiPlus {
            get {
                StringFormat fmt = new StringFormat();
                fmt.LineAlignment = EffectiveCellVerticalAlignment;
                fmt.Trimming = StringTrimming.EllipsisCharacter;
                fmt.Alignment = Column == null ? StringAlignment.Near : Column.TextStringAlign;
                if (!CanWrap)
                    fmt.FormatFlags = StringFormatFlags.NoWrap;
                return fmt;
            }
        }

        /// <summary>
        /// Print the given text in the given rectangle using normal GDI+ .NET methods
        /// </summary>
        /// <remarks>Printing to a printer dc has to be done using this method.</remarks>
        protected virtual void DrawTextGdiPlus(Graphics g, Rectangle r, String txt) {
            using (StringFormat fmt = StringFormatForGdiPlus) {
                // Draw the background of the text as selected, if it's the primary column
                // and it's selected and it's not in FullRowSelect mode.
                Font f = Font;
                if (IsDrawBackground && IsItemSelected && ColumnIsPrimary && !ListView.FullRowSelect) {
                    SizeF size = g.MeasureString(txt, f, r.Width, fmt);
                    Rectangle r2 = r;
                    r2.Width = (int) size.Width + 1;
                    using (Brush brush = new SolidBrush(GetSelectedBackgroundColor())) {
                        g.FillRectangle(brush, r2);
                    }
                }
                RectangleF rf = r;
                g.DrawString(txt, f, TextBrush, rf, fmt);
            }

            // We should put a focus rectangle around the column 0 text if it's selected --
            // but we don't because:
            // - I really dislike this UI convention
            // - we are using buffered graphics, so the DrawFocusRecatangle method of the event doesn't work

            //if (this.ColumnIsPrimary) {
            //    Size size = TextRenderer.MeasureText(this.SubItem.Text, this.ListView.ListFont);
            //    if (r.Width > size.Width)
            //        r.Width = size.Width;
            //    this.Event.DrawFocusRectangle(r);
            //}
        }

        #endregion
    }

    /// <summary>
    /// This renderer highlights substrings that match a given text filter. 
    /// </summary>
    public class HighlightTextRenderer : BaseRenderer, IFilterAwareRenderer {
        #region Life and death

        /// <summary>
        /// Create a HighlightTextRenderer
        /// </summary>
        public HighlightTextRenderer() {
            FramePen = Pens.DarkGreen;
            FillBrush = Brushes.Yellow;
        }

        /// <summary>
        /// Create a HighlightTextRenderer
        /// </summary>
        /// <param name="filter"></param>
        public HighlightTextRenderer(TextMatchFilter filter)
            : this() {
            Filter = filter;
        }

        /// <summary>
        /// Create a HighlightTextRenderer
        /// </summary>
        /// <param name="text"></param>
        [Obsolete("Use HighlightTextRenderer(TextMatchFilter) instead", true)]
        public HighlightTextRenderer(string text) {}

        #endregion

        #region Configuration properties

        /// <summary>
        /// Gets or set how rounded will be the corners of the text match frame
        /// </summary>
        [Category("Appearance"),
         DefaultValue(3.0f),
         Description("How rounded will be the corners of the text match frame?")]
        public float CornerRoundness {
            get { return cornerRoundness; }
            set { cornerRoundness = value; }
        }

        private float cornerRoundness = 3.0f;

        /// <summary>
        /// Gets or set the brush will be used to paint behind the matched substrings.
        /// Set this to null to not fill the frame.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush FillBrush {
            get { return fillBrush; }
            set { fillBrush = value; }
        }

        private Brush fillBrush;

        /// <summary>
        /// Gets or sets the filter that is filtering the ObjectListView and for
        /// which this renderer should highlight text
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextMatchFilter Filter {
            get { return filter; }
            set { filter = value; }
        }

        private TextMatchFilter filter;

        /// <summary>
        /// When a filter changes, keep track of the text matching filters
        /// </summary>
        IModelFilter IFilterAwareRenderer.Filter
        {
            get { return filter; }
            set { RegisterNewFilter(value); }
        }

        internal void RegisterNewFilter(IModelFilter newFilter) {
            if (newFilter is TextMatchFilter textFilter)
            {
                Filter = textFilter;
                return;
            }

            if (newFilter is CompositeFilter composite)
            {
                foreach (TextMatchFilter textSubFilter in composite.TextFilters)
                {
                    Filter = textSubFilter;
                    return;
                }
            }
            Filter = null; 
        } 

        /// <summary>
        /// Gets or set the pen will be used to frame the matched substrings.
        /// Set this to null to not draw a frame.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Pen FramePen {
            get { return framePen; }
            set { framePen = value; }
        }

        private Pen framePen;

        /// <summary>
        /// Gets or sets whether the frame around a text match will have rounded corners
        /// </summary>
        [Category("Appearance"),
         DefaultValue(true),
         Description("Will the frame around a text match will have rounded corners?")]
        public bool UseRoundedRectangle {
            get { return useRoundedRectangle; }
            set { useRoundedRectangle = value; }
        }

        private bool useRoundedRectangle = true;

        #endregion

        #region Compatibility properties

        /// <summary>
        /// Gets or set the text that will be highlighted
        /// </summary>
        [Obsolete("Set the Filter directly rather than just the text", true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TextToHighlight {
            get { return String.Empty; }
            set { }
        }

        /// <summary>
        /// Gets or sets the manner in which substring will be compared.
        /// </summary>
        /// <remarks>
        /// Use this to control if substring matches are case sensitive or insensitive.</remarks>
        [Obsolete("Set the Filter directly rather than just this setting", true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringComparison StringComparison {
            get { return StringComparison.CurrentCultureIgnoreCase; }
            set { }
        }

        #endregion

        #region IRenderer interface overrides

        /// <summary>
        /// Handle a HitTest request after all state information has been initialized
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        protected override Rectangle HandleGetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize) {
            return StandardGetEditRectangle(g, cellBounds, preferredSize);
        }

        #endregion

        #region Rendering

        // This class has two implement two highlighting schemes: one for GDI, another for GDI+.
        // Naturally, GDI+ makes the task easier, but we have to provide something for GDI
        // since that it is what is normally used.

        /// <summary>
        /// Draw text using GDI
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="txt"></param>
        protected override void DrawTextGdi(Graphics g, Rectangle r, string txt) {
            if (ShouldDrawHighlighting)
                DrawGdiTextHighlighting(g, r, txt);

            base.DrawTextGdi(g, r, txt);
        }

        /// <summary>
        /// Draw the highlighted text using GDI
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="txt"></param>
        protected virtual void DrawGdiTextHighlighting(Graphics g, Rectangle r, string txt) {

            // TextRenderer puts horizontal padding around the strings, so we need to take
            // that into account when measuring strings
            const int paddingAdjustment = 6;

            // Cache the font
            Font f = Font;

            foreach (CharacterRange range in Filter.FindAllMatchedRanges(txt)) {
                // Measure the text that comes before our substring
                Size precedingTextSize = Size.Empty;
                if (range.First > 0) {
                    string precedingText = txt.Substring(0, range.First);
                    precedingTextSize = TextRenderer.MeasureText(g, precedingText, f, r.Size, NormalTextFormatFlags);
                    precedingTextSize.Width -= paddingAdjustment;
                }

                // Measure the length of our substring (may be different each time due to case differences)
                string highlightText = txt.Substring(range.First, range.Length);
                Size textToHighlightSize = TextRenderer.MeasureText(g, highlightText, f, r.Size, NormalTextFormatFlags);
                textToHighlightSize.Width -= paddingAdjustment;

                float textToHighlightLeft = r.X + precedingTextSize.Width + 1;
                float textToHighlightTop = AlignVertically(r, textToHighlightSize.Height);

                // Draw a filled frame around our substring
                DrawSubstringFrame(g, textToHighlightLeft, textToHighlightTop, textToHighlightSize.Width, textToHighlightSize.Height);
            }
        }

        /// <summary>
        /// Draw an indication around the given frame that shows a text match
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected virtual void DrawSubstringFrame(Graphics g, float x, float y, float width, float height) {
            if (UseRoundedRectangle) {
                using (GraphicsPath path = GetRoundedRect(x, y, width, height, 3.0f)) {
                    if (FillBrush != null)
                        g.FillPath(FillBrush, path);
                    if (FramePen != null)
                        g.DrawPath(FramePen, path);
                }
            } else {
                if (FillBrush != null)
                    g.FillRectangle(FillBrush, x, y, width, height);
                if (FramePen != null)
                    g.DrawRectangle(FramePen, x, y, width, height);
            }
        }

        /// <summary>
        /// Draw the text using GDI+
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="txt"></param>
        protected override void DrawTextGdiPlus(Graphics g, Rectangle r, string txt) {
            if (ShouldDrawHighlighting)
                DrawGdiPlusTextHighlighting(g, r, txt);

            base.DrawTextGdiPlus(g, r, txt);
        }

        /// <summary>
        /// Draw the highlighted text using GDI+
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="txt"></param>
        protected virtual void DrawGdiPlusTextHighlighting(Graphics g, Rectangle r, string txt) {
            // Find the substrings we want to highlight
            List<CharacterRange> ranges = new List<CharacterRange>(Filter.FindAllMatchedRanges(txt));

            if (ranges.Count == 0)
                return;

            using (StringFormat fmt = StringFormatForGdiPlus) {
                RectangleF rf = r;
                fmt.SetMeasurableCharacterRanges(ranges.ToArray());
                Region[] stringRegions = g.MeasureCharacterRanges(txt, Font, rf, fmt);

                foreach (Region region in stringRegions) {
                    RectangleF bounds = region.GetBounds(g);
                    DrawSubstringFrame(g, bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height);
                }
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets whether the renderer should actually draw highlighting
        /// </summary>
        protected bool ShouldDrawHighlighting {
            get { return Column == null || (Column.Searchable && Filter != null && Filter.HasComponents); }
        }

        /// <summary>
        /// Return a GraphicPath that is a round cornered rectangle
        /// </summary>
        /// <returns>A round cornered rectangle path</returns>
        /// <remarks>If I could rely on people using C# 3.0+, this should be
        /// an extension method of GraphicsPath.</remarks>        
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="diameter"></param>
        protected GraphicsPath GetRoundedRect(float x, float y, float width, float height, float diameter) {
            return GetRoundedRect(new RectangleF(x, y, width, height), diameter);
        }

        /// <summary>
        /// Return a GraphicPath that is a round cornered rectangle
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="diameter">The diameter of the corners</param>
        /// <returns>A round cornered rectangle path</returns>
        /// <remarks>If I could rely on people using C# 3.0+, this should be
        /// an extension method of GraphicsPath.</remarks>
        protected GraphicsPath GetRoundedRect(RectangleF rect, float diameter) {
            GraphicsPath path = new GraphicsPath();

            if (diameter > 0) {
                RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arc, 180, 90);
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
                path.CloseFigure();
            } else {
                path.AddRectangle(rect);
            }

            return path;
        }

        #endregion
    }

    /// <summary>
    /// This class maps a data value to an image that should be drawn for that value.
    /// </summary>
    /// <remarks><para>It is useful for drawing data that is represented as an enum or boolean.</para></remarks>
    public class MappedImageRenderer : BaseRenderer {
        /// <summary>
        /// Return a renderer that draw boolean values using the given images
        /// </summary>
        /// <param name="trueImage">Draw this when our data value is true</param>
        /// <param name="falseImage">Draw this when our data value is false</param>
        /// <returns>A Renderer</returns>
        public static MappedImageRenderer Boolean(Object trueImage, Object falseImage) {
            return new MappedImageRenderer(true, trueImage, false, falseImage);
        }

        /// <summary>
        /// Return a renderer that draw tristate boolean values using the given images
        /// </summary>
        /// <param name="trueImage">Draw this when our data value is true</param>
        /// <param name="falseImage">Draw this when our data value is false</param>
        /// <param name="nullImage">Draw this when our data value is null</param>
        /// <returns>A Renderer</returns>
        public static MappedImageRenderer TriState(Object trueImage, Object falseImage, Object nullImage) {
            return new MappedImageRenderer(new Object[] {true, trueImage, false, falseImage, null, nullImage});
        }

        /// <summary>
        /// Make a new empty renderer
        /// </summary>
        public MappedImageRenderer() {
            map = new Hashtable();
        }

        /// <summary>
        /// Make a new renderer that will show the given image when the given key is the aspect value
        /// </summary>
        /// <param name="key">The data value to be matched</param>
        /// <param name="image">The image to be shown when the key is matched</param>
        public MappedImageRenderer(Object key, Object image)
            : this() {
            Add(key, image);
        }

        /// <summary>
        /// Make a new renderer that will show the given images when it receives the given keys
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="image1"></param>
        /// <param name="key2"></param>
        /// <param name="image2"></param>
        public MappedImageRenderer(Object key1, Object image1, Object key2, Object image2)
            : this() {
            Add(key1, image1);
            Add(key2, image2);
        }

        /// <summary>
        /// Build a renderer from the given array of keys and their matching images
        /// </summary>
        /// <param name="keysAndImages">An array of key/image pairs</param>
        public MappedImageRenderer(Object[] keysAndImages)
            : this() {
            if ((keysAndImages.GetLength(0) % 2) != 0)
                throw new ArgumentException("Array must have key/image pairs");

            for (int i = 0; i < keysAndImages.GetLength(0); i += 2)
                Add(keysAndImages[i], keysAndImages[i + 1]);
        }

        /// <summary>
        /// Register the image that should be drawn when our Aspect has the data value.
        /// </summary>
        /// <param name="value">Value that the Aspect must match</param>
        /// <param name="image">An ImageSelector -- an int, string or image</param>
        public void Add(Object value, Object image) {
            if (value == null)
                nullImage = image;
            else
                map[value] = image;
        }

        /// <summary>
        /// Render our value
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);
            r = ApplyCellPadding(r);

            if (Aspect is not ICollection aspectAsCollection)
                RenderOne(g, r, Aspect);
            else
                RenderCollection(g, r, aspectAsCollection);
        }

        /// <summary>
        /// Draw a collection of images
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="imageSelectors"></param>
        protected void RenderCollection(Graphics g, Rectangle r, ICollection imageSelectors) {
            ArrayList images = new ArrayList();
            Image image = null;
            foreach (Object selector in imageSelectors) {
                if (selector == null)
                    image = GetImage(nullImage);
                else if (map.ContainsKey(selector))
                    image = GetImage(map[selector]);
                else
                    image = null;

                if (image != null)
                    images.Add(image);
            }

            DrawImages(g, r, images);
        }

        /// <summary>
        /// Draw one image
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="selector"></param>
        protected void RenderOne(Graphics g, Rectangle r, Object selector) {
            Image image = null;
            if (selector == null)
                image = GetImage(nullImage);
            else if (map.ContainsKey(selector))
                image = GetImage(map[selector]);

            if (image != null)
                DrawAlignedImage(g, r, image);
        }

        #region Private variables

        private Hashtable map; // Track the association between values and images
        private Object nullImage; // image to be drawn for null values (since null can't be a key)

        #endregion
    }

    /// <summary>
    /// This renderer draws just a checkbox to match the check state of our model object.
    /// </summary>
    public class CheckStateRenderer : BaseRenderer {
        /// <summary>
        /// Draw our cell
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);
            if (Column == null)
                return;
            r = ApplyCellPadding(r);
            CheckState state = Column.GetCheckState(RowObject);
            if (IsPrinting) {
                // Renderers don't work onto printer DCs, so we have to draw the image ourselves
                string key = ObjectListView.CHECKED_KEY;
                if (state == CheckState.Unchecked)
                    key = ObjectListView.UNCHECKED_KEY;
                if (state == CheckState.Indeterminate)
                    key = ObjectListView.INDETERMINATE_KEY;
                DrawAlignedImage(g, r, ImageListOrDefault.Images[key]);
            } else {
                r = CalculateCheckBoxBounds(g, r);
                CheckBoxRenderer.DrawCheckBox(g, r.Location, GetCheckBoxState(state));
            }
        }


        /// <summary>
        /// Handle the GetEditRectangle request
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        protected override Rectangle HandleGetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize) {
            return CalculatePaddedAlignedBounds(g, cellBounds, preferredSize);
        }

        /// <summary>
        /// Handle the HitTest request
        /// </summary>
        /// <param name="g"></param>
        /// <param name="hti"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void HandleHitTest(Graphics g, OlvListViewHitTestInfo hti, int x, int y) {
            Rectangle r = CalculateCheckBoxBounds(g, Bounds);
            if (r.Contains(x, y))
                hti.HitTestLocation = HitTestLocation.CheckBox;
        }
    }

    /// <summary>
    /// Render an image that comes from our data source.
    /// </summary>
    /// <remarks>The image can be sourced from:
    /// <list type="bullet">
    /// <item><description>a byte-array (normally when the image to be shown is
    /// stored as a value in a database)</description></item>
    /// <item><description>an int, which is treated as an index into the image list</description></item>
    /// <item><description>a string, which is treated first as a file name, and failing that as an index into the image list</description></item>
    /// <item><description>an ICollection of ints or strings, which will be drawn as consecutive images</description></item>
    /// </list>
    /// <para>If an image is an animated GIF, it's state is stored in the SubItem object.</para>
    /// <para>By default, the image renderer does not render animations (it begins life with animations paused).
    /// To enable animations, you must call Unpause().</para>
    /// <para>In the current implementation (2009-09), each column showing animated gifs must have a 
    /// different instance of ImageRenderer assigned to it. You cannot share the same instance of
    /// an image renderer between two animated gif columns. If you do, only the last column will be
    /// animated.</para>
    /// </remarks>
    public class ImageRenderer : BaseRenderer {
        /// <summary>
        /// Make an empty image renderer
        /// </summary>
        public ImageRenderer() {
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Make an empty image renderer that begins life ready for animations
        /// </summary>
        public ImageRenderer(bool startAnimations)
            : this() {
            Paused = !startAnimations;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        protected override void Dispose(bool disposing) {
            Paused = true;
            base.Dispose(disposing);
        }

        #region Properties

        /// <summary>
        /// Should the animations in this renderer be paused?
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Paused {
            get { return isPaused; }
            set {
                if (isPaused == value)
                    return;

                isPaused = value;
                if (isPaused) {
                    StopTickler();
                    stopwatch.Stop();
                } else {
                    Tickler.Change(1, Timeout.Infinite);
                    stopwatch.Start();
                }
            }
        }

        private bool isPaused = true;

        private void StopTickler() {
            if (tickler == null)
                return;

            tickler.Dispose();
            tickler = null;
        }

        /// <summary>
        /// Gets a timer that can be used to trigger redraws on animations
        /// </summary>
        protected Timer Tickler {
            get {
                if (tickler == null)
                    tickler = new Timer(new TimerCallback(OnTimer), null, Timeout.Infinite, Timeout.Infinite);
                return tickler;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Pause any animations
        /// </summary>
        public void Pause() {
            Paused = true;
        }

        /// <summary>
        /// Unpause any animations
        /// </summary>
        public void Unpause() {
            Paused = false;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draw our image
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);

            if (Aspect == null || Aspect == DBNull.Value)
                return;
            r = ApplyCellPadding(r);

            if (Aspect is Byte[]) {
                DrawAlignedImage(g, r, GetImageFromAspect());
            } else {
                if (Aspect is not ICollection imageSelectors)
                    DrawAlignedImage(g, r, GetImageFromAspect());
                else
                    DrawImages(g, r, imageSelectors);
            }
        }

        /// <summary>
        /// Translate our Aspect into an image.
        /// </summary>
        /// <remarks>The strategy is:<list type="bullet">
        /// <item><description>If its a byte array, we treat it as an in-memory image</description></item>
        /// <item><description>If it's an int, we use that as an index into our image list</description></item>
        /// <item><description>If it's a string, we try to load a file by that name. If we can't, 
        /// we use the string as an index into our image list.</description></item>
        ///</list></remarks>
        /// <returns>An image</returns>
        protected Image GetImageFromAspect() {
            // If we've already figured out the image, don't do it again
            if (OLVSubItem != null && OLVSubItem.ImageSelector is Image) {
                if (OLVSubItem.AnimationState == null)
                    return (Image) OLVSubItem.ImageSelector;
                else
                    return OLVSubItem.AnimationState.image;
            }

            // Try to convert our Aspect into an Image
            // If its a byte array, we treat it as an in-memory image
            // If it's an int, we use that as an index into our image list
            // If it's a string, we try to find a file by that name.
            //    If we can't, we use the string as an index into our image list.
            Image image = Aspect as Image;
            if (image != null) {
                // Don't do anything else
            } else if (Aspect is Byte[]) {
                using (MemoryStream stream = new MemoryStream((Byte[]) Aspect)) {
                    try {
                        image = Image.FromStream(stream);
                    }
                    catch (ArgumentException) {
                        // ignore
                    }
                }
            } else if (Aspect is Int32) {
                image = GetImage(Aspect);
            } else {
                String str = Aspect as String;
                if (!String.IsNullOrEmpty(str)) {
                    try {
                        image = Image.FromFile(str);
                    }
                    catch (FileNotFoundException) {
                        image = GetImage(Aspect);
                    }
                    catch (OutOfMemoryException) {
                        image = GetImage(Aspect);
                    }
                }
            }

            // If this image is an animation, initialize the animation process
            if (OLVSubItem != null && AnimationState.IsAnimation(image)) {
                OLVSubItem.AnimationState = new AnimationState(image);
            }

            // Cache the image so we don't repeat this dreary process
            if (OLVSubItem != null)
                OLVSubItem.ImageSelector = image;

            return image;
        }

        #endregion

        #region Events

        /// <summary>
        /// This is the method that is invoked by the timer. It basically switches control to the listview thread.
        /// </summary>
        /// <param name="state">not used</param>
        public void OnTimer(Object state) {

            if (IsListViewDead)
                return;

            if (Paused)
                return;

            if (ListView.InvokeRequired)
                ListView.Invoke((MethodInvoker) delegate { OnTimer(state); });
            else
                OnTimerInThread();
        }

        private bool IsListViewDead {
            get {
                // Apply a whole heap of sanity checks, which basically ensure that the ListView is still alive
                return ListView == null ||
                       ListView.Disposing ||
                       ListView.IsDisposed ||
                       !ListView.IsHandleCreated;
            }
        }

        /// <summary>
        /// This is the OnTimer callback, but invoked in the same thread as the creator of the ListView.
        /// This method can use all of ListViews methods without creating a CrossThread exception.
        /// </summary>
        protected void OnTimerInThread() {
            // MAINTAINER NOTE: This method must renew the tickler. If it doesn't the animations will stop.

            // If this listview has been destroyed, we can't do anything, so we return without
            // renewing the tickler, effectively killing all animations on this renderer

            if (IsListViewDead)
                return;

            if (Paused)
                return;

            // If we're not in Detail view or our column has been removed from the list,
            // we can't do anything at the moment, but we still renew the tickler because the view may change later.
            if (ListView.View != View.Details || Column == null || Column.Index < 0) {
                Tickler.Change(1000, Timeout.Infinite);
                return;
            }

            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            int subItemIndex = Column.Index;
            long nextCheckAt = elapsedMilliseconds + 1000; // wait at most one second before checking again
            Rectangle updateRect = new Rectangle(); // what part of the view must be updated to draw the changed gifs?

            // Run through all the subitems in the view for our column, and for each one that
            // has an animation attached to it, see if the frame needs updating.

            for (int i = 0; i < ListView.GetItemCount(); i++) {
                OLVListItem lvi = ListView.GetItem(i);

                // Get the animation state from the subitem. If there isn't an animation state, skip this row.
                OLVListSubItem lvsi = lvi.GetSubItem(subItemIndex);
                AnimationState state = lvsi.AnimationState;
                if (state == null || !state.IsValid)
                    continue;

                // Has this frame of the animation expired?
                if (elapsedMilliseconds >= state.currentFrameExpiresAt) {
                    state.AdvanceFrame(elapsedMilliseconds);

                    // Track the area of the view that needs to be redrawn to show the changed images
                    if (updateRect.IsEmpty)
                        updateRect = lvsi.Bounds;
                    else
                        updateRect = Rectangle.Union(updateRect, lvsi.Bounds);
                }

                // Remember the minimum time at which a frame is next due to change
                nextCheckAt = Math.Min(nextCheckAt, state.currentFrameExpiresAt);
            }

            // Update the part of the listview where frames have changed
            if (!updateRect.IsEmpty)
                ListView.Invalidate(updateRect);

            // Renew the tickler in time for the next frame change
            Tickler.Change(nextCheckAt - elapsedMilliseconds, Timeout.Infinite);
        }

        #endregion

        /// <summary>
        /// Instances of this class kept track of the animation state of a single image.
        /// </summary>
        internal class AnimationState {
            private const int PropertyTagTypeShort = 3;
            private const int PropertyTagTypeLong = 4;
            private const int PropertyTagFrameDelay = 0x5100;
            private const int PropertyTagLoopCount = 0x5101;

            /// <summary>
            /// Is the given image an animation
            /// </summary>
            /// <param name="image">The image to be tested</param>
            /// <returns>Is the image an animation?</returns>
            public static bool IsAnimation(Image image) {
                if (image == null)
                    return false;
                else
                    return (new List<Guid>(image.FrameDimensionsList)).Contains(FrameDimension.Time.Guid);
            }

            /// <summary>
            /// Create an AnimationState in a quiet state
            /// </summary>
            public AnimationState() {
                imageDuration = new List<int>();
            }

            /// <summary>
            /// Create an animation state for the given image, which may or may not
            /// be an animation
            /// </summary>
            /// <param name="image">The image to be rendered</param>
            public AnimationState(Image image)
                : this() {
                if (!IsAnimation(image))
                    return;

                // How many frames in the animation?
                this.image = image;
                frameCount = this.image.GetFrameCount(FrameDimension.Time);

                // Find the delay between each frame.
                // The delays are stored an array of 4-byte ints. Each int is the
                // number of 1/100th of a second that should elapsed before the frame expires
                foreach (PropertyItem pi in this.image.PropertyItems) {
                    if (pi.Id == PropertyTagFrameDelay) {
                        for (int i = 0; i < pi.Len; i += 4) {
                            //TODO: There must be a better way to convert 4-bytes to an int
                            int delay = (pi.Value[i + 3] << 24) + (pi.Value[i + 2] << 16) + (pi.Value[i + 1] << 8) + pi.Value[i];
                            imageDuration.Add(delay * 10); // store delays as milliseconds
                        }
                        break;
                    }
                }

                // There should be as many frame durations as frames
                Debug.Assert(imageDuration.Count == frameCount, "There should be as many frame durations as there are frames.");
            }

            /// <summary>
            /// Does this state represent a valid animation
            /// </summary>
            public bool IsValid {
                get { return (image != null && frameCount > 0); }
            }

            /// <summary>
            /// Advance our images current frame and calculate when it will expire
            /// </summary>
            public void AdvanceFrame(long millisecondsNow) {
                currentFrame = (currentFrame + 1) % frameCount;
                currentFrameExpiresAt = millisecondsNow + imageDuration[currentFrame];
                image.SelectActiveFrame(FrameDimension.Time, currentFrame);
            }

            internal int currentFrame;
            internal long currentFrameExpiresAt;
            internal Image image;
            internal List<int> imageDuration;
            internal int frameCount;
        }

        #region Private variables

        private Timer tickler; // timer used to tickle the animations
        private Stopwatch stopwatch; // clock used to time the animation frame changes

        #endregion
    }

    /// <summary>
    /// Render our Aspect as a progress bar
    /// </summary>
    public class BarRenderer : BaseRenderer {
        #region Constructors

        /// <summary>
        /// Make a BarRenderer
        /// </summary>
        public BarRenderer()
            : base() {}

        /// <summary>
        /// Make a BarRenderer for the given range of data values
        /// </summary>
        public BarRenderer(int minimum, int maximum)
            : this() {
            MinimumValue = minimum;
            MaximumValue = maximum;
        }

        /// <summary>
        /// Make a BarRenderer using a custom bar scheme
        /// </summary>
        public BarRenderer(Pen pen, Brush brush)
            : this() {
            Pen = pen;
            Brush = brush;
            UseStandardBar = false;
        }

        /// <summary>
        /// Make a BarRenderer using a custom bar scheme
        /// </summary>
        public BarRenderer(int minimum, int maximum, Pen pen, Brush brush)
            : this(minimum, maximum) {
            Pen = pen;
            Brush = brush;
            UseStandardBar = false;
        }

        /// <summary>
        /// Make a BarRenderer that uses a horizontal gradient
        /// </summary>
        public BarRenderer(Pen pen, Color start, Color end)
            : this() {
            Pen = pen;
            SetGradient(start, end);
        }

        /// <summary>
        /// Make a BarRenderer that uses a horizontal gradient
        /// </summary>
        public BarRenderer(int minimum, int maximum, Pen pen, Color start, Color end)
            : this(minimum, maximum) {
            Pen = pen;
            SetGradient(start, end);
        }

        #endregion

        #region Configuration Properties

        /// <summary>
        /// Should this bar be drawn in the system style?
        /// </summary>
        [Category("ObjectListView"),
         Description("Should this bar be drawn in the system style?"),
         DefaultValue(true)]
        public bool UseStandardBar {
            get { return useStandardBar; }
            set { useStandardBar = value; }
        }

        private bool useStandardBar = true;

        /// <summary>
        /// How many pixels in from our cell border will this bar be drawn
        /// </summary>
        [Category("ObjectListView"),
         Description("How many pixels in from our cell border will this bar be drawn"),
         DefaultValue(2)]
        public int Padding {
            get { return padding; }
            set { padding = value; }
        }

        private int padding = 2;

        /// <summary>
        /// What color will be used to fill the interior of the control before the 
        /// progress bar is drawn?
        /// </summary>
        [Category("ObjectListView"),
         Description("The color of the interior of the bar"),
         DefaultValue(typeof (Color), "AliceBlue")]
        public Color BackgroundColor {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        private Color backgroundColor = Color.AliceBlue;

        /// <summary>
        /// What color should the frame of the progress bar be?
        /// </summary>
        [Category("ObjectListView"),
         Description("What color should the frame of the progress bar be"),
         DefaultValue(typeof (Color), "Black")]
        public Color FrameColor {
            get { return frameColor; }
            set { frameColor = value; }
        }

        private Color frameColor = Color.Black;

        /// <summary>
        /// How many pixels wide should the frame of the progress bar be?
        /// </summary>
        [Category("ObjectListView"),
         Description("How many pixels wide should the frame of the progress bar be"),
         DefaultValue(1.0f)]
        public float FrameWidth {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        private float frameWidth = 1.0f;

        /// <summary>
        /// What color should the 'filled in' part of the progress bar be?
        /// </summary>
        /// <remarks>This is only used if GradientStartColor is Color.Empty</remarks>
        [Category("ObjectListView"),
         Description("What color should the 'filled in' part of the progress bar be"),
         DefaultValue(typeof (Color), "BlueViolet")]
        public Color FillColor {
            get { return fillColor; }
            set { fillColor = value; }
        }

        private Color fillColor = Color.BlueViolet;

        /// <summary>
        /// Use a gradient to fill the progress bar starting with this color
        /// </summary>
        [Category("ObjectListView"),
         Description("Use a gradient to fill the progress bar starting with this color"),
         DefaultValue(typeof (Color), "CornflowerBlue")]
        public Color GradientStartColor {
            get { return startColor; }
            set { startColor = value; }
        }

        private Color startColor = Color.CornflowerBlue;

        /// <summary>
        /// Use a gradient to fill the progress bar ending with this color
        /// </summary>
        [Category("ObjectListView"),
         Description("Use a gradient to fill the progress bar ending with this color"),
         DefaultValue(typeof (Color), "DarkBlue")]
        public Color GradientEndColor {
            get { return endColor; }
            set { endColor = value; }
        }

        private Color endColor = Color.DarkBlue;

        /// <summary>
        /// Regardless of how wide the column become the progress bar will never be wider than this
        /// </summary>
        [Category("Behavior"),
         Description("The progress bar will never be wider than this"),
         DefaultValue(100)]
        public int MaximumWidth {
            get { return maximumWidth; }
            set { maximumWidth = value; }
        }

        private int maximumWidth = 100;

        /// <summary>
        /// Regardless of how high the cell is  the progress bar will never be taller than this
        /// </summary>
        [Category("Behavior"),
         Description("The progress bar will never be taller than this"),
         DefaultValue(16)]
        public int MaximumHeight {
            get { return maximumHeight; }
            set { maximumHeight = value; }
        }

        private int maximumHeight = 16;

        /// <summary>
        /// The minimum data value expected. Values less than this will given an empty bar
        /// </summary>
        [Category("Behavior"),
         Description("The minimum data value expected. Values less than this will given an empty bar"),
         DefaultValue(0.0)]
        public double MinimumValue {
            get { return minimumValue; }
            set { minimumValue = value; }
        }

        private double minimumValue = 0.0;

        /// <summary>
        /// The maximum value for the range. Values greater than this will give a full bar
        /// </summary>
        [Category("Behavior"),
         Description("The maximum value for the range. Values greater than this will give a full bar"),
         DefaultValue(100.0)]
        public double MaximumValue {
            get { return maximumValue; }
            set { maximumValue = value; }
        }

        private double maximumValue = 100.0;

        #endregion

        #region Public Properties (non-IDE)

        /// <summary>
        /// The Pen that will draw the frame surrounding this bar
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Pen Pen {
            get {
                if (pen == null && !FrameColor.IsEmpty)
                    return new Pen(FrameColor, FrameWidth);
                else
                    return pen;
            }
            set { pen = value; }
        }

        private Pen pen;

        /// <summary>
        /// The brush that will be used to fill the bar
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush Brush {
            get {
                if (brush == null && !FillColor.IsEmpty)
                    return new SolidBrush(FillColor);
                else
                    return brush;
            }
            set { brush = value; }
        }

        private Brush brush;

        /// <summary>
        /// The brush that will be used to fill the background of the bar
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush BackgroundBrush {
            get {
                if (backgroundBrush == null && !BackgroundColor.IsEmpty)
                    return new SolidBrush(BackgroundColor);
                else
                    return backgroundBrush;
            }
            set { backgroundBrush = value; }
        }

        private Brush backgroundBrush;

        #endregion

        /// <summary>
        /// Draw this progress bar using a gradient
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetGradient(Color start, Color end) {
            GradientStartColor = start;
            GradientEndColor = end;
        }

        /// <summary>
        /// Draw our aspect
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);

            r = ApplyCellPadding(r);

            Rectangle frameRect = Rectangle.Inflate(r, 0 - Padding, 0 - Padding);
            frameRect.Width = Math.Min(frameRect.Width, MaximumWidth);
            frameRect.Height = Math.Min(frameRect.Height, MaximumHeight);
            frameRect = AlignRectangle(r, frameRect);

            // Convert our aspect to a numeric value
            if (Aspect is not IConvertible convertable)
                return;
            double aspectValue = convertable.ToDouble(NumberFormatInfo.InvariantInfo);

            Rectangle fillRect = Rectangle.Inflate(frameRect, -1, -1);
            if (aspectValue <= MinimumValue)
                fillRect.Width = 0;
            else if (aspectValue < MaximumValue)
                fillRect.Width = (int) (fillRect.Width * (aspectValue - MinimumValue) / MaximumValue);

            // MS-themed progress bars don't work when printing
            if (UseStandardBar && ProgressBarRenderer.IsSupported && !IsPrinting) {
                ProgressBarRenderer.DrawHorizontalBar(g, frameRect);
                ProgressBarRenderer.DrawHorizontalChunks(g, fillRect);
            } else {
                g.FillRectangle(BackgroundBrush, frameRect);
                if (fillRect.Width > 0) {
                    // FillRectangle fills inside the given rectangle, so expand it a little
                    fillRect.Width++;
                    fillRect.Height++;
                    if (GradientStartColor == Color.Empty)
                        g.FillRectangle(Brush, fillRect);
                    else {
                        using (LinearGradientBrush gradient = new LinearGradientBrush(frameRect, GradientStartColor, GradientEndColor, LinearGradientMode.Horizontal)) {
                            g.FillRectangle(gradient, fillRect);
                        }
                    }
                }
                g.DrawRectangle(Pen, frameRect);
            }
        }

        /// <summary>
        /// Handle the GetEditRectangle request
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellBounds"></param>
        /// <param name="item"></param>
        /// <param name="subItemIndex"></param>
        /// <param name="preferredSize"> </param>
        /// <returns></returns>
        protected override Rectangle HandleGetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize) {
            return CalculatePaddedAlignedBounds(g, cellBounds, preferredSize);
        }
    }

    /// <summary>
    /// An ImagesRenderer draws zero or more images depending on the data returned by its Aspect.
    /// </summary>
    /// <remarks><para>This renderer's Aspect must return a ICollection of ints, strings or Images,
    /// each of which will be drawn horizontally one after the other.</para>
    /// <para>As of v2.1, this functionality has been absorbed into ImageRenderer and this is now an
    /// empty shell, solely for backwards compatibility.</para>
    /// </remarks>
    [ToolboxItem(false)]
    public class ImagesRenderer : ImageRenderer {}

    /// <summary>
    /// A MultiImageRenderer draws the same image a number of times based on our data value
    /// </summary>
    /// <remarks><para>The stars in the Rating column of iTunes is a good example of this type of renderer.</para></remarks>
    public class MultiImageRenderer : BaseRenderer {
        /// <summary>
        /// Make a quiet renderer
        /// </summary>
        public MultiImageRenderer()
            : base() {}

        /// <summary>
        /// Make an image renderer that will draw the indicated image, at most maxImages times.
        /// </summary>
        /// <param name="imageSelector"></param>
        /// <param name="maxImages"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public MultiImageRenderer(Object imageSelector, int maxImages, int minValue, int maxValue)
            : this() {
            ImageSelector = imageSelector;
            MaxNumberImages = maxImages;
            MinimumValue = minValue;
            MaximumValue = maxValue;
        }

        #region Configuration Properties

        /// <summary>
        /// The index of the image that should be drawn
        /// </summary>
        [Category("Behavior"),
         Description("The index of the image that should be drawn"),
         DefaultValue(-1)]
        public int ImageIndex {
            get {
                if (imageSelector is Int32)
                    return (Int32) imageSelector;
                else
                    return -1;
            }
            set { imageSelector = value; }
        }

        /// <summary>
        /// The name of the image that should be drawn
        /// </summary>
        [Category("Behavior"),
         Description("The index of the image that should be drawn"),
         DefaultValue(null)]
        public string ImageName {
            get { return imageSelector as String; }
            set { imageSelector = value; }
        }

        /// <summary>
        /// The image selector that will give the image to be drawn
        /// </summary>
        /// <remarks>Like all image selectors, this can be an int, string or Image</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object ImageSelector {
            get { return imageSelector; }
            set { imageSelector = value; }
        }

        private Object imageSelector;

        /// <summary>
        /// What is the maximum number of images that this renderer should draw?
        /// </summary>
        [Category("Behavior"),
         Description("The maximum number of images that this renderer should draw"),
         DefaultValue(10)]
        public int MaxNumberImages {
            get { return maxNumberImages; }
            set { maxNumberImages = value; }
        }

        private int maxNumberImages = 10;

        /// <summary>
        /// Values less than or equal to this will have 0 images drawn
        /// </summary>
        [Category("Behavior"),
         Description("Values less than or equal to this will have 0 images drawn"),
         DefaultValue(0)]
        public int MinimumValue {
            get { return minimumValue; }
            set { minimumValue = value; }
        }

        private int minimumValue = 0;

        /// <summary>
        /// Values greater than or equal to this will have MaxNumberImages images drawn
        /// </summary>
        [Category("Behavior"),
         Description("Values greater than or equal to this will have MaxNumberImages images drawn"),
         DefaultValue(100)]
        public int MaximumValue {
            get { return maximumValue; }
            set { maximumValue = value; }
        }

        private int maximumValue = 100;

        #endregion

        /// <summary>
        /// Draw our data value
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);
            r = ApplyCellPadding(r);

            Image image = GetImage(ImageSelector);
            if (image == null)
                return;

            // Convert our aspect to a numeric value
            if (Aspect is not IConvertible convertable)
                return;
            double aspectValue = convertable.ToDouble(NumberFormatInfo.InvariantInfo);

            // Calculate how many images we need to draw to represent our aspect value
            int numberOfImages;
            if (aspectValue <= MinimumValue)
                numberOfImages = 0;
            else if (aspectValue < MaximumValue)
                numberOfImages = 1 + (int) (MaxNumberImages * (aspectValue - MinimumValue) / MaximumValue);
            else
                numberOfImages = MaxNumberImages;

            // If we need to shrink the image, what will its on-screen dimensions be?
            int imageScaledWidth = image.Width;
            int imageScaledHeight = image.Height;
            if (r.Height < image.Height) {
                imageScaledWidth = (int) ((float) image.Width * (float) r.Height / (float) image.Height);
                imageScaledHeight = r.Height;
            }
            // Calculate where the images should be drawn
            Rectangle imageBounds = r;
            imageBounds.Width = (MaxNumberImages * (imageScaledWidth + Spacing)) - Spacing;
            imageBounds.Height = imageScaledHeight;
            imageBounds = AlignRectangle(r, imageBounds);

            // Finally, draw the images
            Rectangle singleImageRect = new Rectangle(imageBounds.X, imageBounds.Y, imageScaledWidth, imageScaledHeight);
            Color backgroundColor = GetBackgroundColor();
            for (int i = 0; i < numberOfImages; i++) {
                if (ListItem.Enabled) {
                    DrawImage(g, singleImageRect, ImageSelector);
                }  else
                    ControlPaint.DrawImageDisabled(g, image, singleImageRect.X, singleImageRect.Y, backgroundColor);
                singleImageRect.X += (imageScaledWidth + Spacing);
            }
        }
    }


    /// <summary>
    /// A class to render a value that contains a bitwise-OR'ed collection of values.
    /// </summary>
    public class FlagRenderer : BaseRenderer {
        /// <summary>
        /// Register the given image to the given value
        /// </summary>
        /// <param name="key">When this flag is present...</param>
        /// <param name="imageSelector">...draw this image</param>
        public void Add(Object key, Object imageSelector) {
            Int32 k2 = ((IConvertible) key).ToInt32(NumberFormatInfo.InvariantInfo);

            imageMap[k2] = imageSelector;
            keysInOrder.Remove(k2);
            keysInOrder.Add(k2);
        }

        /// <summary>
        /// Draw the flags
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);

            if (Aspect is not IConvertible convertable)
                return;

            r = ApplyCellPadding(r);

            Int32 v2 = convertable.ToInt32(NumberFormatInfo.InvariantInfo);
            ArrayList images = new ArrayList();
            foreach (Int32 key in keysInOrder) {
                if ((v2 & key) == key) {
                    Image image = GetImage(imageMap[key]);
                    if (image != null)
                        images.Add(image);
                }
            }
            if (images.Count > 0)
                DrawImages(g, r, images);
        }

        /// <summary>
        /// Do the actual work of hit testing. Subclasses should override this rather than HitTest()
        /// </summary>
        /// <param name="g"></param>
        /// <param name="hti"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void HandleHitTest(Graphics g, OlvListViewHitTestInfo hti, int x, int y) {
            if (Aspect is not IConvertible convertable)
                return;

            Int32 v2 = convertable.ToInt32(NumberFormatInfo.InvariantInfo);

            Point pt = Bounds.Location;
            foreach (Int32 key in keysInOrder) {
                if ((v2 & key) == key) {
                    Image image = GetImage(imageMap[key]);
                    if (image != null) {
                        Rectangle imageRect = new Rectangle(pt, image.Size);
                        if (imageRect.Contains(x, y)) {
                            hti.UserData = key;
                            return;
                        }
                        pt.X += (image.Width + Spacing);
                    }
                }
            }
        }

        private List<Int32> keysInOrder = new();
        private Dictionary<Int32, Object> imageMap = new();
    }

    /// <summary>
    /// This renderer draws an image, a single line title, and then multi-line description
    /// under the title.
    /// </summary>
    /// <remarks>
    /// <para>This class works best with FullRowSelect = true.</para>
    /// <para>It's not designed to work with cell editing -- it will work but will look odd.</para>
    /// <para>
    /// It's not RightToLeft friendly.
    /// </para>
    /// </remarks>
    public class DescribedTaskRenderer : BaseRenderer, IFilterAwareRenderer
    {
        private readonly StringFormat noWrapStringFormat;
        private readonly HighlightTextRenderer highlightTextRenderer = new();

        /// <summary>
        /// Create a DescribedTaskRenderer
        /// </summary>
        public DescribedTaskRenderer() {
            noWrapStringFormat = new StringFormat(StringFormatFlags.NoWrap);
            noWrapStringFormat.Trimming = StringTrimming.EllipsisCharacter;
            noWrapStringFormat.Alignment = StringAlignment.Near;
            noWrapStringFormat.LineAlignment = StringAlignment.Near;
            highlightTextRenderer.CellVerticalAlignment = StringAlignment.Near;
        }

        #region Configuration properties

        /// <summary>
        /// Should text be rendered using GDI routines? This makes the text look more
        /// like a native List view control.
        /// </summary>
        public override bool UseGdiTextRendering
        {
            get { return base.UseGdiTextRendering; }
            set
            {
                base.UseGdiTextRendering = value;
                highlightTextRenderer.UseGdiTextRendering = value;
            }
        }

        /// <summary>
        /// Gets or set the font that will be used to draw the title of the task
        /// </summary>
        /// <remarks>If this is null, the ListView's font will be used</remarks>
        [Category("ObjectListView"),
         Description("The font that will be used to draw the title of the task"),
         DefaultValue(null)]
        public Font TitleFont {
            get { return titleFont; }
            set { titleFont = value; }
        }

        private Font titleFont;

        /// <summary>
        /// Return a font that has been set for the title or a reasonable default
        /// </summary>
        [Browsable(false)]
        public Font TitleFontOrDefault {
            get { return TitleFont ?? ListView.Font; }
        }

        /// <summary>
        /// Gets or set the color of the title of the task
        /// </summary>
        /// <remarks>This color is used when the task is not selected or when the listview
        /// has a translucent selection mechanism.</remarks>
        [Category("ObjectListView"),
         Description("The color of the title"),
         DefaultValue(typeof (Color), "")]
        public Color TitleColor {
            get { return titleColor; }
            set { titleColor = value; }
        }

        private Color titleColor;

        /// <summary>
        /// Return the color of the title of the task or a reasonable default
        /// </summary>
        [Browsable(false)]
        public Color TitleColorOrDefault {
            get {
                if (!ListItem.Enabled)
                    return SubItem.ForeColor;
                if (IsItemSelected || TitleColor.IsEmpty)
                    return GetForegroundColor();
                
                return TitleColor;
            }
        }

        /// <summary>
        /// Gets or set the font that will be used to draw the description of the task
        /// </summary>
        /// <remarks>If this is null, the ListView's font will be used</remarks>
        [Category("ObjectListView"),
         Description("The font that will be used to draw the description of the task"),
         DefaultValue(null)]
        public Font DescriptionFont {
            get { return descriptionFont; }
            set { descriptionFont = value; }
        }

        private Font descriptionFont;

        /// <summary>
        /// Return a font that has been set for the title or a reasonable default
        /// </summary>
        [Browsable(false)]
        public Font DescriptionFontOrDefault {
            get { return DescriptionFont ?? ListView.Font; }
        }

        /// <summary>
        /// Gets or set the color of the description of the task
        /// </summary>
        /// <remarks>This color is used when the task is not selected or when the listview
        /// has a translucent selection mechanism.</remarks>
        [Category("ObjectListView"),
         Description("The color of the description"),
         DefaultValue(typeof (Color), "")]
        public Color DescriptionColor {
            get { return descriptionColor; }
            set { descriptionColor = value; }
        }
        private Color descriptionColor = Color.Empty;

        /// <summary>
        /// Return the color of the description of the task or a reasonable default
        /// </summary>
        [Browsable(false)]
        public Color DescriptionColorOrDefault {
            get {
                if (!ListItem.Enabled)
                    return SubItem.ForeColor;
                if (IsItemSelected && !ListView.UseTranslucentSelection)
                    return GetForegroundColor();
                return DescriptionColor.IsEmpty ? defaultDescriptionColor : DescriptionColor;
            }
        }
        private static Color defaultDescriptionColor = Color.FromArgb(45, 46, 49);

        /// <summary>
        /// Gets or sets the number of pixels that will be left between the image and the text
        /// </summary>
        [Category("ObjectListView"),
         Description("The number of pixels that that will be left between the image and the text"),
         DefaultValue(4)]
        public int ImageTextSpace
        {
            get { return imageTextSpace; }
            set { imageTextSpace = value; }
        }
        private int imageTextSpace = 4;

        /// <summary>
        /// Gets or sets the number of pixels that will be left between the title and the description
        /// </summary>
        [Category("ObjectListView"),
         Description("The number of pixels that that will be left between the title and the description"),
         DefaultValue(2)]
        public int TitleDescriptionSpace
        {
            get { return titleDescriptionSpace; }
            set { titleDescriptionSpace = value; }
        }
        private int titleDescriptionSpace = 2;

        /// <summary>
        /// Gets or sets the name of the aspect of the model object that contains the task description
        /// </summary>
        [Category("ObjectListView"),
         Description("The name of the aspect of the model object that contains the task description"),
         DefaultValue(null)]
        public string DescriptionAspectName {
            get { return descriptionAspectName; }
            set { descriptionAspectName = value; }
        }
        private string descriptionAspectName;

        #endregion

        #region Text highlighting

        /// <summary>
        /// Gets or sets the filter that is filtering the ObjectListView and for
        /// which this renderer should highlight text
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextMatchFilter Filter
        {
            get { return highlightTextRenderer.Filter; }
            set { highlightTextRenderer.Filter = value; }
        }

        /// <summary>
        /// When a filter changes, keep track of the text matching filters
        /// </summary>
        IModelFilter IFilterAwareRenderer.Filter {
            get { return Filter; }
            set { highlightTextRenderer.RegisterNewFilter(value); }
        }

        #endregion

        #region Calculating

        /// <summary>
        /// Fetch the description from the model class
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual string GetDescription(object model) {
            if (String.IsNullOrEmpty(DescriptionAspectName))
                return String.Empty;

            if (descriptionGetter == null)
                descriptionGetter = new Munger(DescriptionAspectName);

            return descriptionGetter.GetValue(model) as string;
        }
        private Munger descriptionGetter;

        #endregion

        #region Rendering

        public override void ConfigureSubItem(DrawListViewSubItemEventArgs e, Rectangle cellBounds, object model) {
            base.ConfigureSubItem(e, cellBounds, model);
            highlightTextRenderer.ConfigureSubItem(e, cellBounds, model);
        }

        /// <summary>
        /// Draw our item
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public override void Render(Graphics g, Rectangle r) {
            DrawBackground(g, r);
            r = ApplyCellPadding(r);
            DrawDescribedTask(g, r, GetText(), GetDescription(RowObject), GetImageSelector());
        }

        /// <summary>
        /// Draw the task
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="imageSelector"></param>
        protected virtual void DrawDescribedTask(Graphics g, Rectangle r, string title, string description, object imageSelector) {

            //Debug.WriteLine(String.Format("DrawDescribedTask({0}, {1}, {2}, {3})", r, title, description, imageSelector));

            // Draw the image if one's been given
            Rectangle textBounds = r;
            if (imageSelector != null) {
                int imageWidth = DrawImage(g, r, imageSelector);
                int gapToText = imageWidth + ImageTextSpace;
                textBounds.X += gapToText;
                textBounds.Width -= gapToText;
            }

            // Draw the title
            if (!String.IsNullOrEmpty(title)) {
                using (SolidBrush b = new SolidBrush(TitleColorOrDefault)) {
                    highlightTextRenderer.CanWrap = false;
                    highlightTextRenderer.Font = TitleFontOrDefault;
                    highlightTextRenderer.TextBrush = b;
                    highlightTextRenderer.DrawText(g, textBounds, title);
                }

                // How tall was the title?
                SizeF size = g.MeasureString(title, TitleFontOrDefault, textBounds.Width, noWrapStringFormat);
                int pixelsToDescription = TitleDescriptionSpace + (int)size.Height;
                textBounds.Y += pixelsToDescription;
                textBounds.Height -= pixelsToDescription;
            }

            // Draw the description
            if (!String.IsNullOrEmpty(description)) {
                using (SolidBrush b = new SolidBrush(DescriptionColorOrDefault)) {
                    highlightTextRenderer.CanWrap = true;
                    highlightTextRenderer.Font = DescriptionFontOrDefault;
                    highlightTextRenderer.TextBrush = b;
                    highlightTextRenderer.DrawText(g, textBounds, description); 
                }
            }

            //g.DrawRectangle(Pens.OrangeRed, r);
        }

        #endregion

        #region Hit Testing

        /// <summary>
        /// Handle the HitTest request
        /// </summary>
        /// <param name="g"></param>
        /// <param name="hti"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void HandleHitTest(Graphics g, OlvListViewHitTestInfo hti, int x, int y) {
            if (Bounds.Contains(x, y))
                hti.HitTestLocation = HitTestLocation.Text;
        }

        #endregion
    }

    /// <summary>
    /// This renderer draws a functioning button in its cell
    /// </summary>
    public class ColumnButtonRenderer : BaseRenderer {

        #region Properties

        /// <summary>
        /// Gets or sets how each button will be sized
        /// </summary>
        [Category("ObjectListView"),
        Description("How each button will be sized"),
        DefaultValue(OLVColumn.ButtonSizingMode.TextBounds)]
        public OLVColumn.ButtonSizingMode SizingMode
        {
            get { return sizingMode; }
            set { sizingMode = value; }
        }
        private OLVColumn.ButtonSizingMode sizingMode = OLVColumn.ButtonSizingMode.TextBounds;

        /// <summary>
        /// Gets or sets the size of the button when the SizingMode is FixedBounds
        /// </summary>
        /// <remarks>If this is not set, the bounds of the cell will be used</remarks>
        [Category("ObjectListView"),
        Description("The size of the button when the SizingMode is FixedBounds"),
        DefaultValue(null)]
        public Size? ButtonSize
        {
            get { return buttonSize; }
            set { buttonSize = value; }
        }
        private Size? buttonSize;

        /// <summary>
        /// Gets or sets the extra space that surrounds the cell when the SizingMode is TextBounds
        /// </summary>
        [Category("ObjectListView"),
        Description("The extra space that surrounds the cell when the SizingMode is TextBounds")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size? ButtonPadding
        {
            get { return buttonPadding; }
            set { buttonPadding = value; }
        }
        private Size? buttonPadding = new Size(10, 10);

        private Size ButtonPaddingOrDefault {
            get { return ButtonPadding ?? new Size(10, 10); }
        }

        /// <summary>
        /// Gets or sets the maximum width that a button can occupy.
        /// -1 means there is no maximum width.
        /// </summary>
        /// <remarks>This is only considered when the SizingMode is TextBounds</remarks>
        [Category("ObjectListView"),
        Description("The maximum width that a button can occupy when the SizingMode is TextBounds"),
        DefaultValue(-1)]
        public int MaxButtonWidth
        {
            get { return maxButtonWidth; }
            set { maxButtonWidth = value; }
        }
        private int maxButtonWidth = -1;

        /// <summary>
        /// Gets or sets the minimum width that a button can occupy.
        /// -1 means there is no minimum width.
        /// </summary>
        /// <remarks>This is only considered when the SizingMode is TextBounds</remarks>
        [Category("ObjectListView"),
         Description("The minimum width that a button can be when the SizingMode is TextBounds"),
         DefaultValue(-1)]
        public int MinButtonWidth {
            get { return minButtonWidth; }
            set { minButtonWidth = value; }
        }
        private int minButtonWidth = -1;

        #endregion

        #region Rendering

        /// <summary>
        /// Calculate the size of the contents
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        protected override Size CalculateContentSize(Graphics g, Rectangle r) {
            if (SizingMode == OLVColumn.ButtonSizingMode.CellBounds)
                return r.Size;

            if (SizingMode == OLVColumn.ButtonSizingMode.FixedBounds)
                return ButtonSize ?? r.Size;

            // Ok, SizingMode must be TextBounds. So figure out the size of the text
            Size textSize = CalculateTextSize(g, GetText(), r.Width);

            // Allow for padding and max width
            textSize.Height += ButtonPaddingOrDefault.Height * 2;
            textSize.Width += ButtonPaddingOrDefault.Width * 2;
            if (MaxButtonWidth != -1 && textSize.Width > MaxButtonWidth)
                textSize.Width = MaxButtonWidth;
            if (textSize.Width < MinButtonWidth)
                textSize.Width = MinButtonWidth;

            return textSize;
        }

        /// <summary>
        /// Draw the button
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        protected override void DrawImageAndText(Graphics g, Rectangle r) {
            TextFormatFlags textFormatFlags = TextFormatFlags.HorizontalCenter |
                                              TextFormatFlags.VerticalCenter |
                                              TextFormatFlags.EndEllipsis |
                                              TextFormatFlags.NoPadding |
                                              TextFormatFlags.SingleLine |
                                              TextFormatFlags.PreserveGraphicsTranslateTransform;
            if (ListView.RightToLeftLayout)
                textFormatFlags |= TextFormatFlags.RightToLeft;

            string buttonText = GetText();
            if (!String.IsNullOrEmpty(buttonText))
                ButtonRenderer.DrawButton(g, r, buttonText, Font, textFormatFlags, false, CalculatePushButtonState());
        }

        /// <summary>
        /// What part of the control is under the given point?
        /// </summary>
        /// <param name="g"></param>
        /// <param name="hti"></param>
        /// <param name="bounds"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void StandardHitTest(Graphics g, OlvListViewHitTestInfo hti, Rectangle bounds, int x, int y) {
            Rectangle r = ApplyCellPadding(bounds);
            if (r.Contains(x, y))
                hti.HitTestLocation = HitTestLocation.Button;
        }

        /// <summary>
        /// What is the state of the button?
        /// </summary>
        /// <returns></returns>
        protected PushButtonState CalculatePushButtonState() {
            if (!ListItem.Enabled && !Column.EnableButtonWhenItemIsDisabled)
                return PushButtonState.Disabled;

            if (IsButtonHot)
                return ObjectListView.IsLeftMouseDown ? PushButtonState.Pressed : PushButtonState.Hot;

            return PushButtonState.Normal;
        }

        /// <summary>
        /// Is the mouse over the button?
        /// </summary>
        protected bool IsButtonHot {
            get {
                return IsCellHot && ListView.HotCellHitLocation == HitTestLocation.Button;
            }
        }

        #endregion
    }
}
