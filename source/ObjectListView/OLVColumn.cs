/*
 * OLVColumn - A column in an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 31-March-2011 5:53 pm
 *
 * Change log:
 * 2015-06-12  JPP  - HeaderTextAlign became nullable so that it can be "not set" (this was always the intent)
 * 2014-09-07  JPP  - Added ability to have checkboxes in headers
 * 
 * 2011-05-27  JPP  - Added Sortable, Hideable, Groupable, Searchable, ShowTextInHeader properties
 * 2011-04-12  JPP  - Added HasFilterIndicator
 * 2011-03-31  JPP  - Split into its own file
 * 
 * Copyright (C) 2011-2014 Phillip Piper
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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Drawing.Design;

namespace BrightIdeasSoftware {

    // TODO
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    //public class CheckBoxSettings
    //{
    //    private bool useSettings;
    //    private Image checkedImage;

    //    public bool UseSettings {
    //        get { return useSettings; }
    //        set { useSettings = value; }
    //    }

    //    public Image CheckedImage {
    //        get { return checkedImage; }
    //        set { checkedImage = value; }
    //    }

    //    public Image UncheckedImage {
    //        get { return checkedImage; }
    //        set { checkedImage = value; }
    //    }

    //    public Image IndeterminateImage {
    //        get { return checkedImage; }
    //        set { checkedImage = value; }
    //    }
    //}

    /// <summary>
    /// An OLVColumn knows which aspect of an object it should present.
    /// </summary>
    /// <remarks>
    /// The column knows how to:
    /// <list type="bullet">
    ///	<item><description>extract its aspect from the row object</description></item>
    ///	<item><description>convert an aspect to a string</description></item>
    ///	<item><description>calculate the image for the row object</description></item>
    ///	<item><description>extract a group "key" from the row object</description></item>
    ///	<item><description>convert a group "key" into a title for the group</description></item>
    /// </list>
    /// <para>For sorting to work correctly, aspects from the same column
    /// must be of the same type, that is, the same aspect cannot sometimes
    /// return strings and other times integers.</para>
    /// </remarks>
    [Browsable(false)]
    public partial class OLVColumn : ColumnHeader {

        /// <summary>
        /// How should the button be sized?
        /// </summary>
        public enum ButtonSizingMode
        {
            /// <summary>
            /// Every cell will have the same sized button, as indicated by ButtonSize property
            /// </summary>
            FixedBounds,

            /// <summary>
            /// Every cell will draw a button that fills the cell, inset by ButtonPadding
            /// </summary>
            CellBounds,

            /// <summary>
            /// Each button will be resized to contain the text of the Aspect
            /// </summary>
            TextBounds
        }

        #region Life and death

        /// <summary>
        /// Create an OLVColumn
        /// </summary>
        public OLVColumn() {
        }

        /// <summary>
        /// Initialize a column to have the given title, and show the given aspect
        /// </summary>
        /// <param name="title">The title of the column</param>
        /// <param name="aspect">The aspect to be shown in the column</param>
        public OLVColumn(string title, string aspect)
            : this() {
            Text = title;
            AspectName = aspect;
        }

        #endregion 

        #region Public Properties

        /// <summary>
        /// This delegate will be used to extract a value to be displayed in this column.
        /// </summary>
        /// <remarks>
        /// If this is set, AspectName is ignored.
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AspectGetterDelegate AspectGetter {
            get { return aspectGetter; }
            set { aspectGetter = value; }
        }
        private AspectGetterDelegate aspectGetter;

        /// <summary>
        /// Remember if this aspect getter for this column was generated internally, and can therefore
        /// be regenerated at will
        /// </summary>
        [Obsolete("This property is no longer maintained", true),
         Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AspectGetterAutoGenerated {
            get { return aspectGetterAutoGenerated; }
            set { aspectGetterAutoGenerated = value; }
        }
        private bool aspectGetterAutoGenerated;

        /// <summary>
        /// The name of the property or method that should be called to get the value to display in this column.
        /// This is only used if a ValueGetterDelegate has not been given.
        /// </summary>
        /// <remarks>This name can be dotted to chain references to properties or parameter-less methods.</remarks>
        /// <example>"DateOfBirth"</example>
        /// <example>"Owner.HomeAddress.Postcode"</example>
        [Category("ObjectListView"),
         Description("The name of the property or method that should be called to get the aspect to display in this column"),
         DefaultValue(null)]
        public string AspectName {
            get { return aspectName; }
            set {
                aspectName = value;
                aspectMunger = null;
            }
        }
        private string aspectName;

        /// <summary>
        /// This delegate will be used to put an edited value back into the model object.
        /// </summary>
        /// <remarks>
        /// This does nothing if IsEditable == false.
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AspectPutterDelegate AspectPutter {
            get { return aspectPutter; }
            set { aspectPutter = value; }
        }
        private AspectPutterDelegate aspectPutter;

        /// <summary>
        /// The delegate that will be used to translate the aspect to display in this column into a string.
        /// </summary>
        /// <remarks>If this value is set, AspectToStringFormat will be ignored.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AspectToStringConverterDelegate AspectToStringConverter {
            get { return aspectToStringConverter; }
            set { aspectToStringConverter = value; }
        }
        private AspectToStringConverterDelegate aspectToStringConverter;

        /// <summary>
        /// This format string will be used to convert an aspect to its string representation.
        /// </summary>
        /// <remarks>
        /// This string is passed as the first parameter to the String.Format() method.
        /// This is only used if AspectToStringConverter has not been set.</remarks>
        /// <example>"{0:C}" to convert a number to currency</example>
        [Category("ObjectListView"),
         Description("The format string that will be used to convert an aspect to its string representation"),
         DefaultValue(null)]
        public string AspectToStringFormat {
            get { return aspectToStringFormat; }
            set { aspectToStringFormat = value; }
        }
        private string aspectToStringFormat;

        /// <summary>
        /// Gets or sets whether the cell editor should use AutoComplete
        /// </summary>
        [Category("ObjectListView"),
         Description("Should the editor for cells of this column use AutoComplete"),
         DefaultValue(true)]
        public bool AutoCompleteEditor {
            get { return AutoCompleteEditorMode != AutoCompleteMode.None; }
            set {
                if (value) {
                    if (AutoCompleteEditorMode == AutoCompleteMode.None)
                        AutoCompleteEditorMode = AutoCompleteMode.Append;
                } else
                    AutoCompleteEditorMode = AutoCompleteMode.None;
            }
        }

        /// <summary>
        /// Gets or sets whether the cell editor should use AutoComplete
        /// </summary>
        [Category("ObjectListView"),
         Description("Should the editor for cells of this column use AutoComplete"),
         DefaultValue(AutoCompleteMode.Append)]
        public AutoCompleteMode AutoCompleteEditorMode {
            get { return autoCompleteEditorMode; }
            set { autoCompleteEditorMode = value; }
        }
        private AutoCompleteMode autoCompleteEditorMode = AutoCompleteMode.Append;

        /// <summary>
        /// Gets whether this column can be hidden by user actions
        /// </summary>
        /// <remarks>This take into account both the Hideable property and whether this column
        /// is the primary column of the listview (column 0).</remarks>
        [Browsable(false)]
        public bool CanBeHidden {
            get {
                return Hideable && (Index != 0);
            }
        }

        /// <summary>
        /// When a cell is edited, should the whole cell be used (minus any space used by checkbox or image)?
        /// </summary>
        /// <remarks>
        /// <para>This is always treated as true when the control is NOT owner drawn.</para>
        /// <para>
        /// When this is false (the default) and the control is owner drawn, 
        /// ObjectListView will try to calculate the width of the cell's
        /// actual contents, and then size the editing control to be just the right width. If this is true,
        /// the whole width of the cell will be used, regardless of the cell's contents.
        /// </para>
        /// <para>If this property is not set on the column, the value from the control will be used
        /// </para>
        /// <para>This value is only used when the control is in Details view.</para>
        /// <para>Regardless of this setting, developers can specify the exact size of the editing control
        /// by listening for the CellEditStarting event.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("When a cell is edited, should the whole cell be used?"),
         DefaultValue(null)]
        public virtual bool? CellEditUseWholeCell
        {
            get { return cellEditUseWholeCell; }
            set { cellEditUseWholeCell = value; }
        }
        private bool? cellEditUseWholeCell;

        /// <summary>
        /// Get whether the whole cell should be used when editing a cell in this column
        /// </summary>
        /// <remarks>This calculates the current effective value, which may be different to CellEditUseWholeCell</remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CellEditUseWholeCellEffective {
            get {
                bool? columnSpecificValue = ListView.View == View.Details ? CellEditUseWholeCell : (bool?) null;
                return (columnSpecificValue ?? ((ObjectListView) ListView).CellEditUseWholeCell);
            }
        }

        /// <summary>
        /// Gets or sets how many pixels will be left blank around this cells in this column
        /// </summary>
        /// <remarks>This setting only takes effect when the control is owner drawn.</remarks>
        [Category("ObjectListView"),
         Description("How many pixels will be left blank around the cells in this column?"),
         DefaultValue(null)]
        public Rectangle? CellPadding
        {
            get { return cellPadding; }
            set { cellPadding = value; }
        }
        private Rectangle? cellPadding;

        /// <summary>
        /// Gets or sets how cells in this column will be vertically aligned.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This setting only takes effect when the control is owner drawn.
        /// </para>        
        /// <para>
        /// If this is not set, the value from the control itself will be used.
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
        /// Gets or sets whether this column will show a checkbox.
        /// </summary>
        /// <remarks>
        /// Setting this on column 0 has no effect. Column 0 check box is controlled
        /// by the CheckBoxes property on the ObjectListView itself.
        /// </remarks>
        [Category("ObjectListView"),
         Description("Should values in this column be treated as a checkbox, rather than a string?"),
         DefaultValue(false)]
        public virtual bool CheckBoxes {
            get { return checkBoxes; }
            set {
                if (checkBoxes == value)
                    return;

                checkBoxes = value;
                if (checkBoxes) {
                    if (Renderer == null)
                        Renderer = new CheckStateRenderer();
                } else {
                    if (Renderer is CheckStateRenderer)
                        Renderer = null;
                }
            }
        }
        private bool checkBoxes;

        /// <summary>
        /// Gets or sets the clustering strategy used for this column. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The clustering strategy is used to build a Filtering menu for this item. 
        /// If this is null, a useful default will be chosen. 
        /// </para>
        /// <para>
        /// To disable filtering on this colummn, set UseFiltering to false.
        /// </para>
        /// <para>
        /// Cluster strategies belong to a particular column. The same instance
        /// cannot be shared between multiple columns.
        /// </para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IClusteringStrategy ClusteringStrategy {
            get {
                if (clusteringStrategy == null)
                    ClusteringStrategy = DecideDefaultClusteringStrategy();
                return clusteringStrategy;
            }
            set {
                clusteringStrategy = value;
                if (clusteringStrategy != null)
                    clusteringStrategy.Column = this;
            }
        }
        private IClusteringStrategy clusteringStrategy;

        /// <summary>
        /// Gets or sets whether the button in this column (if this column is drawing buttons) will be enabled
        /// even if the row itself is disabled
        /// </summary>
        [Category("ObjectListView"),
         Description("If this column contains a button, should the button be enabled even if the row is disabled?"),
         DefaultValue(false)]
        public bool EnableButtonWhenItemIsDisabled
        {
            get { return enableButtonWhenItemIsDisabled; }
            set { enableButtonWhenItemIsDisabled = value; }
        }
        private bool enableButtonWhenItemIsDisabled;

        /// <summary>
        /// Should this column resize to fill the free space in the listview?
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you want two (or more) columns to equally share the available free space, set this property to True.
        /// If you want this column to have a larger or smaller share of the free space, you must
        /// set the FreeSpaceProportion property explicitly.
        /// </para>
        /// <para>
        /// Space filling columns are still governed by the MinimumWidth and MaximumWidth properties.
        /// </para>
        /// /// </remarks>
        [Category("ObjectListView"),
         Description("Will this column resize to fill unoccupied horizontal space in the listview?"),
         DefaultValue(false)]
        public bool FillsFreeSpace {
            get { return FreeSpaceProportion > 0; }
            set { FreeSpaceProportion = value ? 1 : 0; }
        }

        /// <summary>
        /// What proportion of the unoccupied horizontal space in the control should be given to this column?
        /// </summary>
        /// <remarks>
        /// <para>
        /// There are situations where it would be nice if a column (normally the rightmost one) would expand as
        /// the list view expands, so that as much of the column was visible as possible without having to scroll
        /// horizontally (you should never, ever make your users have to scroll anything horizontally!).
        /// </para>
        /// <para>
        /// A space filling column is resized to occupy a proportion of the unoccupied width of the listview (the
        /// unoccupied width is the width left over once all the the non-filling columns have been given their space).
        /// This property indicates the relative proportion of that unoccupied space that will be given to this column.
        /// The actual value of this property is not important -- only its value relative to the value in other columns.
        /// For example:
        /// <list type="bullet">
        /// <item><description>
        /// If there is only one space filling column, it will be given all the free space, regardless of the value in FreeSpaceProportion.
        /// </description></item>
        /// <item><description>
        /// If there are two or more space filling columns and they all have the same value for FreeSpaceProportion,
        /// they will share the free space equally.
        /// </description></item>
        /// <item><description>
        /// If there are three space filling columns with values of 3, 2, and 1
        /// for FreeSpaceProportion, then the first column with occupy half the free space, the second will
        /// occupy one-third of the free space, and the third column one-sixth of the free space.
        /// </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FreeSpaceProportion {
            get { return freeSpaceProportion; }
            set { freeSpaceProportion = Math.Max(0, value); }
        }
        private int freeSpaceProportion;

        /// <summary>
        /// Gets or sets whether groups will be rebuild on this columns values when this column's header is clicked.
        /// </summary>
        /// <remarks>
        /// <para>This setting is only used when ShowGroups is true.</para>
        /// <para>
        /// If this is false, clicking the header will not rebuild groups. It will not provide
        /// any feedback as to why the list is not being regrouped. It is the programmers responsibility to
        /// provide appropriate feedback.
        /// </para>
        /// <para>When this is false, BeforeCreatingGroups events are still fired, which can be used to allow grouping
        /// or give feedback, on a case by case basis.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Will the list create groups when this header is clicked?"),
         DefaultValue(true)]
        public bool Groupable {
            get { return groupable; }
            set { groupable = value; }
        }
        private bool groupable = true;

        /// <summary>
        /// This delegate is called when a group has been created but not yet made
        /// into a real ListViewGroup. The user can take this opportunity to fill
        /// in lots of other details about the group.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupFormatterDelegate GroupFormatter {
            get { return groupFormatter; }
            set { groupFormatter = value; }
        }
        private GroupFormatterDelegate groupFormatter;

        /// <summary>
        /// This delegate is called to get the object that is the key for the group
        /// to which the given row belongs.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupKeyGetterDelegate GroupKeyGetter {
            get { return groupKeyGetter; }
            set { groupKeyGetter = value; }
        }
        private GroupKeyGetterDelegate groupKeyGetter;

        /// <summary>
        /// This delegate is called to convert a group key into a title for that group.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupKeyToTitleConverterDelegate GroupKeyToTitleConverter {
            get { return groupKeyToTitleConverter; }
            set { groupKeyToTitleConverter = value; }
        }
        private GroupKeyToTitleConverterDelegate groupKeyToTitleConverter;

        /// <summary>
        /// When the listview is grouped by this column and group title has an item count,
        /// how should the lable be formatted?
        /// </summary>
        /// <remarks>
        /// The given format string can/should have two placeholders:
        /// <list type="bullet">
        /// <item><description>{0} - the original group title</description></item>
        /// <item><description>{1} - the number of items in the group</description></item>
        /// </list>
        /// </remarks>
        /// <example>"{0} [{1} items]"</example>
        [Category("ObjectListView"),
         Description("The format to use when suffixing item counts to group titles"),
         DefaultValue(null),
         Localizable(true)]
        public string GroupWithItemCountFormat {
            get { return groupWithItemCountFormat; }
            set { groupWithItemCountFormat = value; }
        }
        private string groupWithItemCountFormat;

        /// <summary>
        /// Gets this.GroupWithItemCountFormat or a reasonable default
        /// </summary>
        /// <remarks>
        /// If GroupWithItemCountFormat is not set, its value will be taken from the ObjectListView if possible.
        /// </remarks>
        [Browsable(false)]
        public string GroupWithItemCountFormatOrDefault {
            get {
                if (!String.IsNullOrEmpty(GroupWithItemCountFormat))
                    return GroupWithItemCountFormat;

                if (ListView != null) {
                    cachedGroupWithItemCountFormat = ((ObjectListView)ListView).GroupWithItemCountFormatOrDefault;
                    return cachedGroupWithItemCountFormat;
                }

                // There is one rare but pathelogically possible case where the ListView can
                // be null (if the column is grouping a ListView, but is not one of the columns
                // for that ListView) so we have to provide a workable default for that rare case.
                return cachedGroupWithItemCountFormat ?? "{0} [{1} items]";
            }
        }
        private string cachedGroupWithItemCountFormat;

        /// <summary>
        /// When the listview is grouped by this column and a group title has an item count,
        /// how should the lable be formatted if there is only one item in the group?
        /// </summary>
        /// <remarks>
        /// The given format string can/should have two placeholders:
        /// <list type="bullet">
        /// <item><description>{0} - the original group title</description></item>
        /// <item><description>{1} - the number of items in the group (always 1)</description></item>
        /// </list>
        /// </remarks>
        /// <example>"{0} [{1} item]"</example>
        [Category("ObjectListView"),
         Description("The format to use when suffixing item counts to group titles"),
         DefaultValue(null),
         Localizable(true)]
        public string GroupWithItemCountSingularFormat {
            get { return groupWithItemCountSingularFormat; }
            set { groupWithItemCountSingularFormat = value; }
        }
        private string groupWithItemCountSingularFormat;

        /// <summary>
        /// Get this.GroupWithItemCountSingularFormat or a reasonable default
        /// </summary>
        /// <remarks>
        /// <para>If this value is not set, the values from the list view will be used</para>
        /// </remarks>
        [Browsable(false)]
        public string GroupWithItemCountSingularFormatOrDefault {
            get {
                if (!String.IsNullOrEmpty(GroupWithItemCountSingularFormat))
                    return GroupWithItemCountSingularFormat;

                if (ListView != null) {
                    cachedGroupWithItemCountSingularFormat = ((ObjectListView)ListView).GroupWithItemCountSingularFormatOrDefault;
                    return cachedGroupWithItemCountSingularFormat;
                }

                // There is one rare but pathelogically possible case where the ListView can
                // be null (if the column is grouping a ListView, but is not one of the columns
                // for that ListView) so we have to provide a workable default for that rare case.
                return cachedGroupWithItemCountSingularFormat ?? "{0} [{1} item]";
            }
        }
        private string cachedGroupWithItemCountSingularFormat;

        /// <summary>
        /// Gets whether this column should be drawn with a filter indicator in the column header.
        /// </summary>
        [Browsable(false)]
        public bool HasFilterIndicator {
            get {
                return UseFiltering && ValuesChosenForFiltering != null && ValuesChosenForFiltering.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets a delegate that will be used to own draw header column.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HeaderDrawingDelegate HeaderDrawing {
            get { return headerDrawing; }
            set { headerDrawing = value; }
        }
        private HeaderDrawingDelegate headerDrawing;

        /// <summary>
        /// Gets or sets the style that will be used to draw the header for this column
        /// </summary>
        /// <remarks>This is only uses when the owning ObjectListView has HeaderUsesThemes set to false.</remarks>
        [Category("ObjectListView"),
         Description("What style will be used to draw the header of this column"),
         DefaultValue(null)]
        public HeaderFormatStyle HeaderFormatStyle {
            get { return headerFormatStyle; }
            set { headerFormatStyle = value; }
        }
        private HeaderFormatStyle headerFormatStyle;

        /// <summary>
        /// Gets or sets the font in which the header for this column will be drawn
        /// </summary>
        /// <remarks>You should probably use a HeaderFormatStyle instead of this property</remarks>
        /// <remarks>This is only uses when HeaderUsesThemes is false.</remarks>
        [Category("ObjectListView"),
        Description("Which font will be used to draw the header?"),
        DefaultValue(null)]
        public Font HeaderFont {
            get { return HeaderFormatStyle == null ? null : HeaderFormatStyle.Normal.Font; }
            set {
                if (value == null && HeaderFormatStyle == null)
                    return;

                if (HeaderFormatStyle == null)
                    HeaderFormatStyle = new HeaderFormatStyle();

                HeaderFormatStyle.SetFont(value);
            }
        }

        /// <summary>
        /// Gets or sets the color in which the text of the header for this column will be drawn
        /// </summary>
        /// <remarks>You should probably use a HeaderFormatStyle instead of this property</remarks>
        /// <remarks>This is only uses when HeaderUsesThemes is false.</remarks>
        [Category("ObjectListView"),
         Description("In what color will the header text be drawn?"),
         DefaultValue(typeof(Color), "")]
        public Color HeaderForeColor {
            get { return HeaderFormatStyle == null ? Color.Empty : HeaderFormatStyle.Normal.ForeColor; }
            set {
                if (value.IsEmpty && HeaderFormatStyle == null)
                    return;

                if (HeaderFormatStyle == null)
                    HeaderFormatStyle = new HeaderFormatStyle();

                HeaderFormatStyle.SetForeColor(value);
            }
        }

        /// <summary>
        /// Gets or sets whether the text values in this column will act like hyperlinks
        /// </summary>
        /// <remarks>This is only taken into account when HeaderUsesThemes is false.</remarks>
        [Category("ObjectListView"),
         Description("Name of the image that will be shown in the column header."),
         DefaultValue(null),
         TypeConverter(typeof(ImageKeyConverter)),
         Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
         RefreshProperties(RefreshProperties.Repaint)]
        public string HeaderImageKey {
            get { return headerImageKey; }
            set { headerImageKey = value; }
        }
        private string headerImageKey;


        /// <summary>
        /// Gets or sets how the text of the header will be drawn?
        /// </summary>
        [Category("ObjectListView"),
         Description("How will the header text be aligned? If this is not set, the alignment of the header will follow the alignment of the column"),
         DefaultValue(null)]
        public HorizontalAlignment? HeaderTextAlign {
            get { return headerTextAlign; }
            set { headerTextAlign = value; }
        }
        private HorizontalAlignment? headerTextAlign;

        /// <summary>
        /// Return the text alignment of the header. This will either have been set explicitly,
        /// or will follow the alignment of the text in the column
        /// </summary>
        [Browsable(false)]
        public HorizontalAlignment HeaderTextAlignOrDefault
        {
            get { return headerTextAlign.HasValue ? headerTextAlign.Value : TextAlign; }
        }

        /// <summary>
        /// Gets the header alignment converted to a StringAlignment
        /// </summary>
        [Browsable(false)]
        public StringAlignment HeaderTextAlignAsStringAlignment {
            get {
                switch (HeaderTextAlignOrDefault) {
                    case HorizontalAlignment.Left: return StringAlignment.Near;
                    case HorizontalAlignment.Center: return StringAlignment.Center;
                    case HorizontalAlignment.Right: return StringAlignment.Far;
                    default: return StringAlignment.Near;
                }
            }
        }

        /// <summary>
        /// Gets whether or not this column has an image in the header
        /// </summary>
        [Browsable(false)]
        public bool HasHeaderImage {
            get {
                return (ListView != null &&
                    ListView.SmallImageList != null &&
                    ListView.SmallImageList.Images.ContainsKey(HeaderImageKey));
            }
        }

        /// <summary>
        /// Gets or sets whether this header will place a checkbox in the header
        /// </summary>
        [Category("ObjectListView"),
         Description("Draw a checkbox in the header of this column"),
         DefaultValue(false)]
        public bool HeaderCheckBox
        {
            get { return headerCheckBox; }
            set { headerCheckBox = value; }
        }
        private bool headerCheckBox;

        /// <summary>
        /// Gets or sets whether this header will place a tri-state checkbox in the header
        /// </summary>
        [Category("ObjectListView"),
        Description("Draw a tri-state checkbox in the header of this column"),
         DefaultValue(false)]
        public bool HeaderTriStateCheckBox
        {
            get { return headerTriStateCheckBox; }
            set { headerTriStateCheckBox = value; }
        }
        private bool headerTriStateCheckBox;

        /// <summary>
        /// Gets or sets the checkedness of the checkbox in the header of this column
        /// </summary>
        [Category("ObjectListView"),
         Description("Checkedness of the header checkbox"),
         DefaultValue(CheckState.Unchecked)]
        public CheckState HeaderCheckState
        {
            get { return headerCheckState; }
            set { headerCheckState = value; }
        }
        private CheckState headerCheckState = CheckState.Unchecked;

        /// <summary>
        /// Gets or sets whether the 
        /// checking/unchecking the value of the header's checkbox will result in the
        /// checkboxes for all cells in this column being set to the same checked/unchecked.
        /// Defaults to true.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There is no reverse of this function that automatically updates the header when the 
        /// checkedness of a cell changes.
        /// </para>
        /// <para>
        /// This property's behaviour on a TreeListView is probably best describes as undefined 
        /// and should be avoided.
        /// </para>
        /// <para>
        /// The performance of this action (checking/unchecking all rows) is O(n) where n is the 
        /// number of rows. It will work on large virtual lists, but it may take some time.
        /// </para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Update row checkboxs when the header checkbox is clicked by the user"),
         DefaultValue(true)]
        public bool HeaderCheckBoxUpdatesRowCheckBoxes {
            get { return headerCheckBoxUpdatesRowCheckBoxes; }
            set { headerCheckBoxUpdatesRowCheckBoxes = value; }
        }
        private bool headerCheckBoxUpdatesRowCheckBoxes = true;

        /// <summary>
        /// Gets or sets whether the checkbox in the header is disabled
        /// </summary>
        /// <remarks>
        /// Clicking on a disabled checkbox does not change its value, though it does raise
        /// a HeaderCheckBoxChanging event, which allows the programmer the opportunity to do 
        /// something appropriate.</remarks>
        [Category("ObjectListView"),
        Description("Is the checkbox in the header of this column disabled"),
         DefaultValue(false)]
        public bool HeaderCheckBoxDisabled
        {
            get { return headerCheckBoxDisabled; }
            set { headerCheckBoxDisabled = value; }
        }
        private bool headerCheckBoxDisabled;

        /// <summary>
        /// Gets or sets whether this column can be hidden by the user.
        /// </summary>
        /// <remarks>
        /// <para>Column 0 can never be hidden, regardless of this setting.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Will the user be able to choose to hide this column?"),
         DefaultValue(true)]
        public bool Hideable {
            get { return hideable; }
            set { hideable = value; }
        }
        private bool hideable = true;

        /// <summary>
        /// Gets or sets whether the text values in this column will act like hyperlinks
        /// </summary>
        [Category("ObjectListView"),
         Description("Will the text values in the cells of this column act like hyperlinks?"),
         DefaultValue(false)]
        public bool Hyperlink {
            get { return hyperlink; }
            set { hyperlink = value; }
        }
        private bool hyperlink;

        /// <summary>
        /// This is the name of property that will be invoked to get the image selector of the
        /// image that should be shown in this column.
        /// It can return an int, string, Image or null.
        /// </summary>
        /// <remarks>
        /// <para>This is ignored if ImageGetter is not null.</para>
        /// <para>The property can use these return value to identify the image:</para>
        /// <list type="bullet">
        /// <item><description>null or -1 -- indicates no image</description></item>
        /// <item><description>an int -- the int value will be used as an index into the image list</description></item>
        /// <item><description>a String -- the string value will be used as a key into the image list</description></item>
        /// <item><description>an Image -- the Image will be drawn directly (only in OwnerDrawn mode)</description></item>
        /// </list>
        /// </remarks>
        [Category("ObjectListView"),
         Description("The name of the property that holds the image selector"),
         DefaultValue(null)]
        public string ImageAspectName {
            get { return imageAspectName; }
            set { imageAspectName = value; }
        }
        private string imageAspectName;

        /// <summary>
        /// This delegate is called to get the image selector of the image that should be shown in this column.
        /// It can return an int, string, Image or null.
        /// </summary>
        /// <remarks><para>This delegate can use these return value to identify the image:</para>
        /// <list type="bullet">
        /// <item><description>null or -1 -- indicates no image</description></item>
        /// <item><description>an int -- the int value will be used as an index into the image list</description></item>
        /// <item><description>a String -- the string value will be used as a key into the image list</description></item>
        /// <item><description>an Image -- the Image will be drawn directly (only in OwnerDrawn mode)</description></item>
        /// </list>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageGetterDelegate ImageGetter {
            get { return imageGetter; }
            set { imageGetter = value; }
        }
        private ImageGetterDelegate imageGetter;

        /// <summary>
        /// Gets or sets whether this column will draw buttons in its cells
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this is set to true, the renderer for the column is become a ColumnButtonRenderer
        /// if it isn't already. If this is set to false, any previous button renderer will be discarded
        /// </para>
        /// If the cell's aspect is null or empty, nothing will be drawn in the cell.</remarks>
        [Category("ObjectListView"),
         Description("Does this column draw its cells as buttons?"),
         DefaultValue(false)]
        public bool IsButton {
            get { return isButton; }
            set {
                isButton = value;
                if (value) {
                    if (Renderer is not ColumnButtonRenderer buttonRenderer) {
                        Renderer = CreateColumnButtonRenderer();
                        FillInColumnButtonRenderer();
                    }
                } else {
                    if (Renderer is ColumnButtonRenderer)
                        Renderer = null;
                }
            }
        }
        private bool isButton;

        /// <summary>
        /// Create a ColumnButtonRenderer to draw buttons in this column
        /// </summary>
        /// <returns></returns>
        protected virtual ColumnButtonRenderer CreateColumnButtonRenderer() {
            return new ColumnButtonRenderer();
        }

        /// <summary>
        /// Fill in details to our ColumnButtonRenderer based on the properties set on the column
        /// </summary>
        protected virtual void FillInColumnButtonRenderer() {
            if (Renderer is not ColumnButtonRenderer buttonRenderer)
                return;

            buttonRenderer.SizingMode = ButtonSizing;
            buttonRenderer.ButtonSize = ButtonSize;
            buttonRenderer.ButtonPadding = ButtonPadding;
            buttonRenderer.MaxButtonWidth = ButtonMaxWidth;
        }

        /// <summary>
        /// Gets or sets the maximum width that a button can occupy.
        /// -1 means there is no maximum width.
        /// </summary>
        /// <remarks>This is only considered when the SizingMode is TextBounds</remarks>
        [Category("ObjectListView"),
         Description("The maximum width that a button can occupy when the SizingMode is TextBounds"),
         DefaultValue(-1)]
        public int ButtonMaxWidth {
            get { return buttonMaxWidth; }
            set {
                buttonMaxWidth = value;
                FillInColumnButtonRenderer();
            }
        }
        private int buttonMaxWidth = -1;

        /// <summary>
        /// Gets or sets the extra space that surrounds the cell when the SizingMode is TextBounds
        /// </summary>
        [Category("ObjectListView"),
         Description("The extra space that surrounds the cell when the SizingMode is TextBounds"),
         DefaultValue(null)]
        public Size? ButtonPadding {
            get { return buttonPadding; }
            set {
                buttonPadding = value;
                FillInColumnButtonRenderer();
            }
        }
        private Size? buttonPadding;

        /// <summary>
        /// Gets or sets the size of the button when the SizingMode is FixedBounds
        /// </summary>
        /// <remarks>If this is not set, the bounds of the cell will be used</remarks>
        [Category("ObjectListView"),
         Description("The size of the button when the SizingMode is FixedBounds"),
         DefaultValue(null)]
        public Size? ButtonSize {
            get { return buttonSize; }
            set {
                buttonSize = value;
                FillInColumnButtonRenderer();
            }
        }
        private Size? buttonSize;

        /// <summary>
        /// Gets or sets how each button will be sized if this column is displaying buttons
        /// </summary>
        [Category("ObjectListView"),
         Description("If this column is showing buttons, how each button will be sized"),
         DefaultValue(ButtonSizingMode.TextBounds)]
        public ButtonSizingMode ButtonSizing {
            get { return buttonSizing; }
            set {
                buttonSizing = value;
                FillInColumnButtonRenderer();
            }
        }
        private ButtonSizingMode buttonSizing = ButtonSizingMode.TextBounds;

        /// <summary>
        /// Can the values shown in this column be edited?
        /// </summary>
        /// <remarks>This defaults to true, since the primary means to control the editability of a listview
        /// is on the listview itself. Once a listview is editable, all the columns are too, unless the
        /// programmer explicitly marks them as not editable</remarks>
        [Category("ObjectListView"),
         Description("Can the value in this column be edited?"),
         DefaultValue(true)]
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; }
        }
        private bool isEditable = true;

        /// <summary>
        /// Is this column a fixed width column?
        /// </summary>
        [Browsable(false)]
        public bool IsFixedWidth {
            get {
                return (MinimumWidth != -1 && MaximumWidth != -1 && MinimumWidth >= MaximumWidth);
            }
        }

        /// <summary>
        /// Get/set whether this column should be used when the view is switched to tile view.
        /// </summary>
        /// <remarks>Column 0 is always included in tileview regardless of this setting.
        /// Tile views do not work well with many "columns" of information. 
        /// Two or three works best.</remarks>
        [Category("ObjectListView"),
         Description("Will this column be used when the view is switched to tile view"),
         DefaultValue(false)]
        public bool IsTileViewColumn {
            get { return isTileViewColumn; }
            set { isTileViewColumn = value; }
        }
        private bool isTileViewColumn;

        /// <summary>
        /// Gets or sets whether the text of this header should be rendered vertically.
        /// </summary>
        /// <remarks>
        /// <para>If this is true, it is a good idea to set ToolTipText to the name of the column so it's easy to read.</para>
        /// <para>Vertical headers are text only. They do not draw their image.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Will the header for this column be drawn vertically?"),
         DefaultValue(false)]
        public bool IsHeaderVertical {
            get { return isHeaderVertical; }
            set { isHeaderVertical = value; }
        }
        private bool isHeaderVertical;

        /// <summary>
        /// Can this column be seen by the user?
        /// </summary>
        /// <remarks>After changing this value, you must call RebuildColumns() before the changes will take effect.</remarks>
        [Category("ObjectListView"),
         Description("Can this column be seen by the user?"),
         DefaultValue(true)]
        public bool IsVisible {
            get { return isVisible; }
            set
            {
                if (isVisible == value)
                    return;

                isVisible = value;
                OnVisibilityChanged(EventArgs.Empty);
            }
        }
        private bool isVisible = true;

        /// <summary>
        /// Where was this column last positioned within the Detail view columns
        /// </summary>
        /// <remarks>DisplayIndex is volatile. Once a column is removed from the control,
        /// there is no way to discover where it was in the display order. This property
        /// guards that information even when the column is not in the listview's active columns.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LastDisplayIndex {
            get { return lastDisplayIndex; }
            set { lastDisplayIndex = value;  }
        }
        private int lastDisplayIndex = -1;

        /// <summary>
        /// What is the maximum width that the user can give to this column?
        /// </summary>
        /// <remarks>-1 means there is no maximum width. Give this the same value as MinimumWidth to make a fixed width column.</remarks>
        [Category("ObjectListView"),
         Description("What is the maximum width to which the user can resize this column? -1 means no limit"),
         DefaultValue(-1)]
        public int MaximumWidth {
            get { return maxWidth; }
            set {
                maxWidth = value;
                if (maxWidth != -1 && Width > maxWidth)
                    Width = maxWidth;
            }
        }
        private int maxWidth = -1;

        /// <summary>
        /// What is the minimum width that the user can give to this column?
        /// </summary>
        /// <remarks>-1 means there is no minimum width. Give this the same value as MaximumWidth to make a fixed width column.</remarks>
        [Category("ObjectListView"),
         Description("What is the minimum width to which the user can resize this column? -1 means no limit"),
         DefaultValue(-1)]
        public int MinimumWidth {
            get { return minWidth; }
            set {
                minWidth = value;
                if (Width < minWidth)
                    Width = minWidth;
            }
        }
        private int minWidth = -1;

        /// <summary>
        /// Get/set the renderer that will be invoked when a cell needs to be redrawn
        /// </summary>
        [Category("ObjectListView"),
        Description("The renderer will draw this column when the ListView is owner drawn"),
        DefaultValue(null)]
        public IRenderer Renderer {
            get { return renderer; }
            set { renderer = value; }
        }
        private IRenderer renderer;

        /// <summary>
        /// This delegate is called when a cell needs to be drawn in OwnerDrawn mode.
        /// </summary>
        /// <remarks>This method is kept primarily for backwards compatibility.
        /// New code should implement an IRenderer, though this property will be maintained.</remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RenderDelegate RendererDelegate {
            get {
                return Renderer is Version1Renderer version1Renderer ? version1Renderer.RenderDelegate : null;
            }
            set {
                Renderer = value == null ? null : new Version1Renderer(value);
            }
        }

        /// <summary>
        /// Gets or sets whether the text in this column's cell will be used when doing text searching.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is false, text filters will not trying searching this columns cells when looking for matches.
        /// </para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Will the text of the cells in this column be considered when searching?"),
         DefaultValue(true)]
        public bool Searchable {
            get { return searchable; }
            set { searchable = value; }
        }
        private bool searchable = true;

        /// <summary>
        /// Gets or sets a delegate which will return the array of text values that should be 
        /// considered for text matching when using a text based filter.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SearchValueGetterDelegate SearchValueGetter {
            get { return searchValueGetter; }
            set { searchValueGetter = value; }
        }
        private SearchValueGetterDelegate searchValueGetter;

        /// <summary>
        /// Gets or sets whether the header for this column will include the column's Text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is false, the only thing rendered in the column header will be the image from <see cref="HeaderImageKey"/>.
        /// </para>
        /// <para>This setting is only considered when <see cref="ObjectListView.HeaderUsesThemes"/> is false on the owning ObjectListView.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Will the header for this column include text?"),
         DefaultValue(true)]
        public bool ShowTextInHeader {
            get { return showTextInHeader; }
            set { showTextInHeader = value; }
        }
        private bool showTextInHeader = true;

        /// <summary>
        /// Gets or sets whether the contents of the list will be resorted when the user clicks the 
        /// header of this column.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is false, clicking the header will not sort the list, but will not provide
        /// any feedback as to why the list is not being sorted. It is the programmers responsibility to
        /// provide appropriate feedback.
        /// </para>
        /// <para>When this is false, BeforeSorting events are still fired, which can be used to allow sorting
        /// or give feedback, on a case by case basis.</para>
        /// </remarks>
        [Category("ObjectListView"),
         Description("Will clicking this columns header resort the list?"),
         DefaultValue(true)]
        public bool Sortable {
            get { return sortable; }
            set { sortable = value; }
        }
        private bool sortable = true;

        /// <summary>
        /// Gets or sets the horizontal alignment of the contents of the column.
        /// </summary>
        /// <remarks>.NET will not allow column 0 to have any alignment except
        /// to the left. We can't change the basic behaviour of the listview,
        /// but when owner drawn, column 0 can now have other alignments.</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        new public HorizontalAlignment TextAlign
        {
            get {
                return textAlign.HasValue ? textAlign.Value : base.TextAlign;
            }
            set {
                textAlign = value;
                base.TextAlign = value;
            }
        }
        private HorizontalAlignment? textAlign;

        /// <summary>
        /// Gets the StringAlignment equivilent of the column text alignment
        /// </summary>
        [Browsable(false)]
        public StringAlignment TextStringAlign {
            get {
                switch (TextAlign) {
                case HorizontalAlignment.Center:
                    return StringAlignment.Center;
                case HorizontalAlignment.Left:
                    return StringAlignment.Near;
                case HorizontalAlignment.Right:
                    return StringAlignment.Far;
                default:
                    return StringAlignment.Near;
                }
            }
        }

        /// <summary>
        /// What string should be displayed when the mouse is hovered over the header of this column?
        /// </summary>
        /// <remarks>If a HeaderToolTipGetter is installed on the owning ObjectListView, this
        /// value will be ignored.</remarks>
        [Category("ObjectListView"),
         Description("The tooltip to show when the mouse is hovered over the header of this column"),
         DefaultValue((String)null),
         Localizable(true)]
        public String ToolTipText {
            get { return toolTipText; }
            set { toolTipText = value; }
        }
        private String toolTipText;

        /// <summary>
        /// Should this column have a tri-state checkbox?
        /// </summary>
        /// <remarks>
        /// If this is true, the user can choose the third state (normally Indeterminate).
        /// </remarks>
        [Category("ObjectListView"),
         Description("Should values in this column be treated as a tri-state checkbox?"),
         DefaultValue(false)]
        public virtual bool TriStateCheckBoxes {
            get { return triStateCheckBoxes; }
            set {
                triStateCheckBoxes = value;
                if (value && !CheckBoxes)
                    CheckBoxes = true;
            }
        }
        private bool triStateCheckBoxes;

        /// <summary>
        /// Group objects by the initial letter of the aspect of the column
        /// </summary>
        /// <remarks>
        /// One common pattern is to group column by the initial letter of the value for that group.
        /// The aspect must be a string (obviously).
        /// </remarks>
        [Category("ObjectListView"),
         Description("The name of the property or method that should be called to get the aspect to display in this column"),
         DefaultValue(false)]
        public bool UseInitialLetterForGroup {
            get { return useInitialLetterForGroup; }
            set { useInitialLetterForGroup = value; }
        }
        private bool useInitialLetterForGroup;

        /// <summary>
        /// Gets or sets whether or not this column should be user filterable
        /// </summary>
        [Category("ObjectListView"),
         Description("Does this column want to show a Filter menu item when its header is right clicked"),
         DefaultValue(true)]
        public bool UseFiltering {
            get { return useFiltering; }
            set { useFiltering = value; }
        }
        private bool useFiltering = true;

        /// <summary>
        /// Gets or sets a filter that will only include models where the model's value
        /// for this column is one of the values in ValuesChosenForFiltering
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IModelFilter ValueBasedFilter {
            get {
                if (!UseFiltering)
                    return null;

                if (valueBasedFilter != null)
                    return valueBasedFilter;

                if (ClusteringStrategy == null)
                    return null;

                if (ValuesChosenForFiltering == null || ValuesChosenForFiltering.Count == 0)
                    return null;

                return ClusteringStrategy.CreateFilter(ValuesChosenForFiltering);
            }
            set { valueBasedFilter = value; }
        }
        private IModelFilter valueBasedFilter;

        /// <summary>
        /// Gets or sets the values that will be used to generate a filter for this
        /// column. For a model to be included by the generated filter, its value for this column
        /// must be in this list. If the list is null or empty, this column will
        /// not be used for filtering.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList ValuesChosenForFiltering {
            get { return valuesChosenForFiltering; }
            set { valuesChosenForFiltering = value; }
        }
        private IList valuesChosenForFiltering = new ArrayList();

        /// <summary>
        /// What is the width of this column?
        /// </summary>
        [Category("ObjectListView"),
        Description("The width in pixels of this column"),
        DefaultValue(60)]
        new public int Width {
            get { return base.Width; }
            set {
                if (MaximumWidth != -1 && value > MaximumWidth)
                    base.Width = MaximumWidth;
                else
                    base.Width = Math.Max(MinimumWidth, value);
            }
        }

        /// <summary>
        /// Gets or set whether the contents of this column's cells should be word wrapped
        /// </summary>
        /// <remarks>If this column uses a custom IRenderer (that is, one that is not descended
        /// from BaseRenderer), then that renderer is responsible for implementing word wrapping.</remarks>
        [Category("ObjectListView"),
         Description("Draw this column cell's word wrapped"),
         DefaultValue(false)]
        public bool WordWrap {
            get { return wordWrap; }
            set {
                wordWrap = value;

                // If there isn't a renderer and they are turning word wrap off, we don't need to do anything
                if (Renderer == null && !wordWrap)
                    return;

                // All other cases require a renderer of some sort
                if (Renderer == null)
                    Renderer = new HighlightTextRenderer();

                // If there is a custom renderer (not descended from BaseRenderer), 
                // we leave it up to them to implement wrapping
                if (Renderer is not BaseRenderer baseRenderer)
                    return;

                baseRenderer.CanWrap = wordWrap;
            }
        }
        private bool wordWrap;

        #endregion

        #region Object commands

        /// <summary>
        /// For a given group value, return the string that should be used as the groups title.
        /// </summary>
        /// <param name="value">The group key that is being converted to a title</param>
        /// <returns>string</returns>
        public string ConvertGroupKeyToTitle(object value) {
            if (groupKeyToTitleConverter != null)
                return groupKeyToTitleConverter(value);

            return value == null ? ObjectListView.GroupTitleDefault : ValueToString(value);
        }

        /// <summary>
        /// Get the checkedness of the given object for this column
        /// </summary>
        /// <param name="rowObject">The row object that is being displayed</param>
        /// <returns>The checkedness of the object</returns>
        public CheckState GetCheckState(object rowObject) {
            if (!CheckBoxes)
                return CheckState.Unchecked;

            if (GetValue(rowObject) is bool aspectAsBool) {
                if (aspectAsBool)
                    return CheckState.Checked;
                else
                    return CheckState.Unchecked;
            } else
                return CheckState.Indeterminate;
        }

        /// <summary>
        /// Put the checkedness of the given object for this column
        /// </summary>
        /// <param name="rowObject">The row object that is being displayed</param>
        /// <param name="newState"></param>
        /// <returns>The checkedness of the object</returns>
        public void PutCheckState(object rowObject, CheckState newState) {
            if (newState == CheckState.Checked)
                PutValue(rowObject, true);
            else
                if (newState == CheckState.Unchecked)
                    PutValue(rowObject, false);
                else
                    PutValue(rowObject, null);
        }

        /// <summary>
        /// For a given row object, extract the value indicated by the AspectName property of this column.
        /// </summary>
        /// <param name="rowObject">The row object that is being displayed</param>
        /// <returns>An object, which is the aspect named by AspectName</returns>
        public object GetAspectByName(object rowObject) {
            if (aspectMunger == null)
                aspectMunger = new Munger(AspectName);

            return aspectMunger.GetValue(rowObject);
        }
        private Munger aspectMunger;

        /// <summary>
        /// For a given row object, return the object that is the key of the group that this row belongs to.
        /// </summary>
        /// <param name="rowObject">The row object that is being displayed</param>
        /// <returns>Group key object</returns>
        public object GetGroupKey(object rowObject) {
            if (groupKeyGetter != null)
                return groupKeyGetter(rowObject);

            object key = GetValue(rowObject);

            if (UseInitialLetterForGroup) {
                String keyAsString = key as String;
                if (!String.IsNullOrEmpty(keyAsString))
                    return keyAsString.Substring(0, 1).ToUpper();
                }

            return key;
        }

        /// <summary>
        /// For a given row object, return the image selector of the image that should displayed in this column.
        /// </summary>
        /// <param name="rowObject">The row object that is being displayed</param>
        /// <returns>int or string or Image. int or string will be used as index into image list. null or -1 means no image</returns>
        public Object GetImage(object rowObject) {
            if (CheckBoxes)
                return GetCheckStateImage(rowObject);

            if (ImageGetter != null)
                return ImageGetter(rowObject);

            if (!String.IsNullOrEmpty(ImageAspectName)) {
                if (imageAspectMunger == null)
                    imageAspectMunger = new Munger(ImageAspectName);

                return imageAspectMunger.GetValue(rowObject);
            }

            // I think this is wrong. ImageKey is meant for the image in the header, not in the rows
            if (!String.IsNullOrEmpty(ImageKey))
                return ImageKey;

            return ImageIndex;
        }
        private Munger imageAspectMunger;

        /// <summary>
        /// Return the image that represents the check box for the given model
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public string GetCheckStateImage(Object rowObject) {
            CheckState checkState = GetCheckState(rowObject);

            if (checkState == CheckState.Checked)
                return ObjectListView.CHECKED_KEY;

            if (checkState == CheckState.Unchecked)
                return ObjectListView.UNCHECKED_KEY;

            return ObjectListView.INDETERMINATE_KEY;
        }

        /// <summary>
        /// For a given row object, return the strings that will be searched when trying to filter by string.
        /// </summary>
        /// <remarks>
        /// This will normally be the simple GetStringValue result, but if this column is non-textual (e.g. image)
        /// you might want to install a SearchValueGetter delegate which can return something that could be used
        /// for text filtering.
        /// </remarks>
        /// <param name="rowObject"></param>
        /// <returns>The array of texts to be searched. If this returns null, search will not match that object.</returns>
        public string[] GetSearchValues(object rowObject) {
            if (SearchValueGetter != null)
                return SearchValueGetter(rowObject);

            var stringValue = GetStringValue(rowObject);

            if (Renderer is DescribedTaskRenderer dtr) {
                return new string[] { stringValue, dtr.GetDescription(rowObject) };
            }

            return new string[] { stringValue };
        }

        /// <summary>
        /// For a given row object, return the string representation of the value shown in this column.
        /// </summary>
        /// <remarks>
        /// For aspects that are string (e.g. aPerson.Name), the aspect and its string representation are the same.
        /// For non-strings (e.g. aPerson.DateOfBirth), the string representation is very different.
        /// </remarks>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public string GetStringValue(object rowObject)
        {
            return ValueToString(GetValue(rowObject));
        }

        /// <summary>
        /// For a given row object, return the object that is to be displayed in this column.
        /// </summary>
        /// <param name="rowObject">The row object that is being displayed</param>
        /// <returns>An object, which is the aspect to be displayed</returns>
        public object GetValue(object rowObject) {
            if (AspectGetter == null)
                return GetAspectByName(rowObject);
            else
                return AspectGetter(rowObject);
        }

        /// <summary>
        /// Update the given model object with the given value using the column's
        /// AspectName.
        /// </summary>
        /// <param name="rowObject">The model object to be updated</param>
        /// <param name="newValue">The value to be put into the model</param>
        public void PutAspectByName(Object rowObject, Object newValue) {
            if (aspectMunger == null)
                aspectMunger = new Munger(AspectName);

            aspectMunger.PutValue(rowObject, newValue);
        }

        /// <summary>
        /// Update the given model object with the given value
        /// </summary>
        /// <param name="rowObject">The model object to be updated</param>
        /// <param name="newValue">The value to be put into the model</param>
        public void PutValue(Object rowObject, Object newValue) {
            if (aspectPutter == null)
                PutAspectByName(rowObject, newValue);
            else
                aspectPutter(rowObject, newValue);
        }

        /// <summary>
        /// Convert the aspect object to its string representation.
        /// </summary>
        /// <remarks>
        /// If the column has been given a AspectToStringConverter, that will be used to do
        /// the conversion, otherwise just use ToString(). 
        /// The returned value will not be null. Nulls are always converted
        /// to empty strings.
        /// </remarks>
        /// <param name="value">The value of the aspect that should be displayed</param>
        /// <returns>A string representation of the aspect</returns>
        public string ValueToString(object value) {
            // Give the installed converter a chance to work (even if the value is null)
            if (AspectToStringConverter != null)
                return AspectToStringConverter(value) ?? String.Empty;

            // Without a converter, nulls become simple empty strings
            if (value == null)
                return String.Empty;

            string fmt = AspectToStringFormat;
            if (String.IsNullOrEmpty(fmt))
                return value.ToString();
            else
                return String.Format(fmt, value);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Decide the clustering strategy that will be used for this column
        /// </summary>
        /// <returns></returns>
        private IClusteringStrategy DecideDefaultClusteringStrategy() {
            if (!UseFiltering)
                return null;

            if (DataType == typeof(DateTime))
                return new DateTimeClusteringStrategy();

            return new ClustersFromGroupsStrategy();
        }

        /// <summary>
        /// Gets or sets the type of data shown in this column.
        /// </summary>
        /// <remarks>If this is not set, it will try to get the type
        /// by looking through the rows of the listview.</remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type DataType {
            get {
                if (dataType == null) {
                    if (ListView is ObjectListView olv) {
                        object value = olv.GetFirstNonNullValue(this);
                        if (value != null)
                            return value.GetType(); // THINK: Should we cache this?
                    }
                }
                return dataType;
            }
            set {
                dataType = value;
            }
        }
        private Type dataType;

        #region Events

		/// <summary>
		/// This event is triggered when the visibility of this column changes.
		/// </summary>
		[Category("ObjectListView"),
        Description("This event is triggered when the visibility of the column changes.")]
		public event EventHandler<EventArgs> VisibilityChanged;

		/// <summary>
		/// Tell the world when visibility of a column changes.
		/// </summary>
		public virtual void OnVisibilityChanged(EventArgs e)
		{
			if (VisibilityChanged != null)
				VisibilityChanged(this, e);
		}

		#endregion

        /// <summary>
        /// Create groupies
        /// This is an untyped version to help with Generator and OLVColumn attributes
        /// </summary>
        /// <param name="values"></param>
        /// <param name="descriptions"></param>
        public void MakeGroupies(object[] values, string[] descriptions) {
            MakeGroupies(values, descriptions, null, null, null);
        }

        /// <summary>
        /// Create groupies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="descriptions"></param>
        public void MakeGroupies<T>(T[] values, string[] descriptions) {
            MakeGroupies(values, descriptions, null, null, null);
        }

        /// <summary>
        /// Create groupies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="descriptions"></param>
        /// <param name="images"></param>
        public void MakeGroupies<T>(T[] values, string[] descriptions, object[] images) {
            MakeGroupies(values, descriptions, images, null, null);
        }

        /// <summary>
        /// Create groupies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="descriptions"></param>
        /// <param name="images"></param>
        /// <param name="subtitles"></param>
        public void MakeGroupies<T>(T[] values, string[] descriptions, object[] images, string[] subtitles) {
            MakeGroupies(values, descriptions, images, subtitles, null);
        }

        /// <summary>
        /// Create groupies.
        /// Install delegates that will group the columns aspects into progressive partitions.
        /// If an aspect is less than value[n], it will be grouped with description[n].
        /// If an aspect has a value greater than the last element in "values", it will be grouped
        /// with the last element in "descriptions".
        /// </summary>
        /// <param name="values">Array of values. Values must be able to be
        /// compared to the aspect (using IComparable)</param>
        /// <param name="descriptions">The description for the matching value. The last element is the default description.
        /// If there are n values, there must be n+1 descriptions.</param>
        /// <example>
        /// this.salaryColumn.MakeGroupies(
        ///     new UInt32[] { 20000, 100000 },
        ///     new string[] { "Lowly worker",  "Middle management", "Rarified elevation"});
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="images"></param>
        /// <param name="subtitles"></param>
        /// <param name="tasks"></param>
        public void MakeGroupies<T>(T[] values, string[] descriptions, object[] images, string[] subtitles, string[] tasks) {
            // Sanity checks
            if (values == null)
                throw new ArgumentNullException("values");
            if (descriptions == null)
                throw new ArgumentNullException("descriptions");
            if (values.Length + 1 != descriptions.Length)
                throw new ArgumentException("descriptions must have one more element than values.");

            // Install a delegate that returns the index of the description to be shown
            GroupKeyGetter = delegate(object row) {
                Object aspect = GetValue(row);
                if (aspect == null || aspect == DBNull.Value)
                    return -1;
                IComparable comparable = (IComparable)aspect;
                for (int i = 0; i < values.Length; i++) {
                    if (comparable.CompareTo(values[i]) < 0)
                        return i;
                }

                // Display the last element in the array
                return descriptions.Length - 1;
            };

            // Install a delegate that simply looks up the given index in the descriptions.
            GroupKeyToTitleConverter = delegate(object key) {
                if ((int)key < 0)
                    return "";

                return descriptions[(int)key];
            };

            // Install one delegate that does all the other formatting
            GroupFormatter = delegate(OLVGroup group, GroupingParameters parms) {
                int key = (int)group.Key; // we know this is an int since we created it in GroupKeyGetter

                if (key >= 0) {
                    if (images != null && key < images.Length)
                        group.TitleImage = images[key];

                    if (subtitles != null && key < subtitles.Length)
                        group.Subtitle = subtitles[key];

                    if (tasks != null && key < tasks.Length)
                        group.Task = tasks[key];
                }
            };
        }
        /// <summary>
        /// Create groupies based on exact value matches.
        /// </summary>
        /// <remarks>
        /// Install delegates that will group rows into partitions based on equality of this columns aspects.
        /// If an aspect is equal to value[n], it will be grouped with description[n].
        /// If an aspect is not equal to any value, it will be grouped with "[other]".
        /// </remarks>
        /// <param name="values">Array of values. Values must be able to be
        /// equated to the aspect</param>
        /// <param name="descriptions">The description for the matching value.</param>
        /// <example>
        /// this.marriedColumn.MakeEqualGroupies(
        ///     new MaritalStatus[] { MaritalStatus.Single, MaritalStatus.Married, MaritalStatus.Divorced, MaritalStatus.Partnered },
        ///     new string[] { "Looking",  "Content", "Looking again", "Mostly content" });
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="images"></param>
        /// <param name="subtitles"></param>
        /// <param name="tasks"></param>
        public void MakeEqualGroupies<T>(T[] values, string[] descriptions, object[] images, string[] subtitles, string[] tasks) {
            // Sanity checks
            if (values == null)
                throw new ArgumentNullException("values");
            if (descriptions == null)
                throw new ArgumentNullException("descriptions");
            if (values.Length != descriptions.Length)
                throw new ArgumentException("descriptions must have the same number of elements as values.");

            ArrayList valuesArray = new ArrayList(values);

            // Install a delegate that returns the index of the description to be shown
            GroupKeyGetter = delegate(object row) {
                return valuesArray.IndexOf(GetValue(row));
            };

            // Install a delegate that simply looks up the given index in the descriptions.
            GroupKeyToTitleConverter = delegate(object key) {
                int intKey = (int)key; // we know this is an int since we created it in GroupKeyGetter
                return (intKey < 0) ? "[other]" : descriptions[intKey];
            };

            // Install one delegate that does all the other formatting
            GroupFormatter = delegate(OLVGroup group, GroupingParameters parms) {
                int key = (int)group.Key; // we know this is an int since we created it in GroupKeyGetter

                if (key >= 0) {
                    if (images != null && key < images.Length)
                        group.TitleImage = images[key];

                    if (subtitles != null && key < subtitles.Length)
                        group.Subtitle = subtitles[key];

                    if (tasks != null && key < tasks.Length)
                        group.Task = tasks[key];
                }
            };
        }

        #endregion
    }
}
