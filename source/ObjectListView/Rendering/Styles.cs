/*
 * Styles - A style is a group of formatting attributes that can be applied to a row or a cell
 *
 * Author: Phillip Piper
 * Date: 29/07/2009 23:09
 *
 * Change log:
 * v2.4
 * 2010-03-23  JPP  - Added HeaderFormatStyle and support
 * v2.3
 * 2009-08-15  JPP  - Added Decoration and Overlay properties to HotItemStyle
 * 2009-07-29  JPP  - Initial version
 *
 * To do:
 * - These should be more generally available. It should be possible to do something like this:
 *       this.olv.GetItem(i).Style = new ItemStyle();
 *       this.olv.GetItem(i).GetSubItem(j).Style = new CellStyle();
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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// The common interface supported by all style objects
    /// </summary>
    public interface IItemStyle
    {
        /// <summary>
        /// Gets or set the font that will be used by this style
        /// </summary>
        Font Font { get; set; }

        /// <summary>
        /// Gets or set the font style
        /// </summary>
        FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the ForeColor
        /// </summary>
        Color ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the BackColor
        /// </summary>
        Color BackColor { get; set; }
    }

    /// <summary>
    /// Basic implementation of IItemStyle
    /// </summary>
    public class SimpleItemStyle : Component, IItemStyle
    {
        /// <summary>
        /// Gets or sets the font that will be applied by this style
        /// </summary>
        [DefaultValue(null)]
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        private Font font;

        /// <summary>
        /// Gets or sets the style of font that will be applied by this style
        /// </summary>
        [DefaultValue(FontStyle.Regular)]
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }

        private FontStyle fontStyle;

        /// <summary>
        /// Gets or sets the color of the text that will be applied by this style
        /// </summary>
        [DefaultValue(typeof (Color), "")]
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        private Color foreColor;

        /// <summary>
        /// Gets or sets the background color that will be applied by this style
        /// </summary>
        [DefaultValue(typeof (Color), "")]
        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        private Color backColor;
    }


    /// <summary>
    /// Instances of this class specify how should "hot items" (non-selected
    /// rows under the cursor) be renderered.
    /// </summary>
    public class HotItemStyle : SimpleItemStyle
    {
        /// <summary>
        /// Gets or sets the overlay that should be drawn as part of the hot item
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IOverlay Overlay {
            get { return overlay; }
            set { overlay = value; }
        }
        private IOverlay overlay;

        /// <summary>
        /// Gets or sets the decoration that should be drawn as part of the hot item
        /// </summary>
        /// <remarks>A decoration is different from an overlay in that an decoration
        /// scrolls with the listview contents, whilst an overlay does not.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDecoration Decoration {
            get { return decoration; }
            set { decoration = value; }
        }
        private IDecoration decoration;
    }

    /// <summary>
    /// This class defines how a cell should be formatted
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CellStyle : IItemStyle
    {
        /// <summary>
        /// Gets or sets the font that will be applied by this style
        /// </summary>
        public Font Font {
            get { return font; }
            set { font = value; }
        }
        private Font font;

        /// <summary>
        /// Gets or sets the style of font that will be applied by this style
        /// </summary>
        [DefaultValue(FontStyle.Regular)]
        public FontStyle FontStyle {
            get { return fontStyle; }
            set { fontStyle = value; }
        }
        private FontStyle fontStyle;

        /// <summary>
        /// Gets or sets the color of the text that will be applied by this style
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color ForeColor {
            get { return foreColor; }
            set { foreColor = value; }
        }
        private Color foreColor;

        /// <summary>
        /// Gets or sets the background color that will be applied by this style
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color BackColor {
            get { return backColor; }
            set { backColor = value; }
        }
        private Color backColor;
    }

    /// <summary>
    /// Instances of this class describe how hyperlinks will appear
    /// </summary>
    public class HyperlinkStyle : Component
    {
        /// <summary>
        /// Create a HyperlinkStyle
        /// </summary>
        public HyperlinkStyle() {
            Normal = new CellStyle();
            Normal.ForeColor = Color.Blue;
            Over = new CellStyle();
            Over.FontStyle = FontStyle.Underline;
            Visited = new CellStyle();
            Visited.ForeColor = Color.Purple;
            OverCursor = Cursors.Hand;
        }

        /// <summary>
        /// What sort of formatting should be applied to hyperlinks in their normal state?
        /// </summary>
        [Category("Appearance"),
         Description("How should hyperlinks be drawn")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public CellStyle Normal
        {
            get { return normalStyle; }
            set { normalStyle = value; }
        }
        private CellStyle normalStyle;

        /// <summary>
        /// What sort of formatting should be applied to hyperlinks when the mouse is over them?
        /// </summary>
        [Category("Appearance"),
         Description("How should hyperlinks be drawn when the mouse is over them?")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public CellStyle Over
        {
            get { return overStyle; }
            set { overStyle = value; }
        }
        private CellStyle overStyle;

        /// <summary>
        /// What sort of formatting should be applied to hyperlinks after they have been clicked?
        /// </summary>
        [Category("Appearance"),
         Description("How should hyperlinks be drawn after they have been clicked")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public CellStyle Visited
        {
            get { return visitedStyle; }
            set { visitedStyle = value; }
        }
        private CellStyle visitedStyle;

        /// <summary>
        /// Gets or sets the cursor that should be shown when the mouse is over a hyperlink.
        /// </summary>
        [Category("Appearance"),
         Description("What cursor should be shown when the mouse is over a link?")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Cursor OverCursor
        {
            get { return overCursor; }
            set { overCursor = value; }
        }
        private Cursor overCursor;
    }

    /// <summary>
    /// Instances of this class control one the styling of one particular state
    /// (normal, hot, pressed) of a header control
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HeaderStateStyle
    {
        /// <summary>
        /// Gets or sets the font that will be applied by this style
        /// </summary>
        [DefaultValue(null)]
        public Font Font {
            get { return font; }
            set { font = value; }
        }
        private Font font;

        /// <summary>
        /// Gets or sets the color of the text that will be applied by this style
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color ForeColor {
            get { return foreColor; }
            set { foreColor = value; }
        }
        private Color foreColor;

        /// <summary>
        /// Gets or sets the background color that will be applied by this style
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color BackColor {
            get { return backColor; }
            set { backColor = value; }
        }
        private Color backColor;

        /// <summary>
        /// Gets or sets the color in which a frame will be drawn around the header for this column
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color FrameColor {
            get { return frameColor; }
            set { frameColor = value; }
        }
        private Color frameColor;

        /// <summary>
        /// Gets or sets the width of the frame that will be drawn around the header for this column
        /// </summary>
        [DefaultValue(0.0f)]
        public float FrameWidth {
            get { return frameWidth; }
            set { frameWidth = value; }
        }
        private float frameWidth;
    }

    /// <summary>
    /// This class defines how a header should be formatted in its various states.
    /// </summary>
    public class HeaderFormatStyle : Component
    {
        /// <summary>
        /// Create a new HeaderFormatStyle
        /// </summary>
        public HeaderFormatStyle() {
            Hot = new HeaderStateStyle();
            Normal = new HeaderStateStyle();
            Pressed = new HeaderStateStyle();
        }

        /// <summary>
        /// What sort of formatting should be applied to a column header when the mouse is over it?
        /// </summary>
        [Category("Appearance"),
         Description("How should the header be drawn when the mouse is over it?")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HeaderStateStyle Hot
        {
            get { return hotStyle; }
            set { hotStyle = value; }
        }
        private HeaderStateStyle hotStyle;

        /// <summary>
        /// What sort of formatting should be applied to a column header in its normal state?
        /// </summary>
        [Category("Appearance"),
         Description("How should a column header normally be drawn")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HeaderStateStyle Normal
        {
            get { return normalStyle; }
            set { normalStyle = value; }
        }
        private HeaderStateStyle normalStyle;

        /// <summary>
        /// What sort of formatting should be applied to a column header when pressed?
        /// </summary>
        [Category("Appearance"),
         Description("How should a column header be drawn when it is pressed")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HeaderStateStyle Pressed
        {
            get { return pressedStyle; }
            set { pressedStyle = value; }
        }
        private HeaderStateStyle pressedStyle;

        /// <summary>
        /// Set the font for all three states
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(Font font) {
            Normal.Font = font;
            Hot.Font = font;
            Pressed.Font = font;
        }

        /// <summary>
        /// Set the fore color for all three states
        /// </summary>
        /// <param name="color"></param>
        public void SetForeColor(Color color) {
            Normal.ForeColor = color;
            Hot.ForeColor = color;
            Pressed.ForeColor = color;
        }

        /// <summary>
        /// Set the back color for all three states
        /// </summary>
        /// <param name="color"></param>
        public void SetBackColor(Color color) {
            Normal.BackColor = color;
            Hot.BackColor = color;
            Pressed.BackColor = color;
        }
    }
}
