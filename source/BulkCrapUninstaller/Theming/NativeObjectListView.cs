using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BulkCrapUninstaller.Theming;

internal sealed class NativeObjectListView : ObjectListView
{
    public NativeObjectListView()
    {
        DefaultRenderer = new ContrastListRenderer();
    }

    internal event EventHandler ListRebuilt;

    public override void BuildList(bool shouldPreserveState)
    {
        if (Frozen || IsDisposed || Disposing) return;
        base.BuildList(shouldPreserveState);
        // AfterSorting is skipped for empty lists and precedes selection restore.
        // Consumers need the completed filtered view, including zero results.
        if (!IsDisposed && !Disposing) ListRebuilt?.Invoke(this, EventArgs.Empty);
    }

    // Pair foregrounds with the background actually used for each selection state.
    public override Color SelectedForeColor
    {
        get => SystemInformation.HighContrast ? SystemColors.HighlightText : base.SelectedForeColor;
        set => base.SelectedForeColor = value;
    }
    public override Color SelectedBackColor
    {
        get => SystemInformation.HighContrast ? SystemColors.Highlight : base.SelectedBackColor;
        set => base.SelectedBackColor = value;
    }
    public override Color UnfocusedSelectedForeColor
    {
        get => SystemInformation.HighContrast ? SystemColors.ControlText : base.UnfocusedSelectedForeColor;
        set => base.UnfocusedSelectedForeColor = value;
    }
    public override Color UnfocusedSelectedBackColor
    {
        get => SystemInformation.HighContrast ? SystemColors.Control : base.UnfocusedSelectedBackColor;
        set => base.UnfocusedSelectedBackColor = value;
    }

    protected override void ApplyCellStyle(OLVListItem item, int columnIndex, IItemStyle style)
    {
        base.ApplyCellStyle(item, columnIndex, style);
        if (SystemInformation.HighContrast && HyperlinkStyle != null
            && (ReferenceEquals(style, HyperlinkStyle.Normal) || ReferenceEquals(style, HyperlinkStyle.Visited)
                || ReferenceEquals(style, HyperlinkStyle.Over)))
            item.SubItems[columnIndex].ForeColor = SystemColors.HotTrack;
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        RefreshHyperlinkColors();
    }

    private void RefreshHyperlinkColors()
    {
        // Existing subitems cache RGB values. Recolor them without rebuilding rows,
        // rerunning format callbacks, changing selection or creating new fonts.
        if (IsDisposed || VirtualMode || !UseHyperlinks || HyperlinkStyle == null) return;
        foreach (OLVListItem item in Items)
            for (var column = 0; column < Columns.Count; column++)
            {
                var subItem = item.GetSubItem(column);
                if (!GetColumn(column).Hyperlink || string.IsNullOrEmpty(subItem?.Url)) continue;
                var style = IsUrlVisited(subItem.Url) ? HyperlinkStyle.Visited : HyperlinkStyle.Normal;
                if (item.Index == HotRowIndex && column == HotColumnIndex
                    && HotCellHitLocation == HitTestLocation.Text && !HyperlinkStyle.Over.ForeColor.IsEmpty)
                    style = HyperlinkStyle.Over;
                subItem.ForeColor = SystemInformation.HighContrast ? SystemColors.HotTrack : style.ForeColor;
            }
        Invalidate();
    }

#if NET10_0_OR_GREATER
    private const int GroupItem = 1; // LVCDI_GROUP
    private const int PrePaint = 1; // CDDS_PREPAINT (group notifications use this, not ITEMPREPAINT)
    private const int PostPaint = 2; // CDDS_POSTPAINT
    private const int NotifyPostPaint = 0x10; // CDRF_NOTIFYPOSTPAINT
    private const int GetGroupRect = 0x1000 + 98; // LVM_GETGROUPRECT
    private const int GroupLabel = 2; // LVGGR_LABEL

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool RepaintGroupCaptions { get; set; } = true;

    protected override bool HandleCustomDraw(ref Message m)
    {
        // Deliberately scoped to this prototype's ordinary Details view. Virtual lists,
        // RTL and richer group layouts need their own validation before integration.
        if (!ThemeManager.IsEnabled || SystemInformation.HighContrast
            || VirtualMode || View != View.Details || RightToLeftLayout)
            return base.HandleCustomDraw(ref m);
        var draw = Marshal.PtrToStructure<ListCustomDraw>(m.LParam);
        // Native disabled painting ignores LVM_SETBKCOLOR. Fill only the background
        // at PREPAINT; upstream ObjectListView still draws every row and overlay.
        // This requires its owner-drawn rows (ordinary native rows still turn white).
        if (draw.ItemType != GroupItem)
        {
            if (!Enabled && OwnerDraw && draw.Draw.Stage == PrePaint && draw.Draw.Hdc != IntPtr.Zero)
            {
                using var graphics = Graphics.FromHdc(draw.Draw.Hdc);
                using var fill = new SolidBrush(BackColor);
                graphics.FillRectangle(fill, ClientRectangle);
            }
            return base.HandleCustomDraw(ref m);
        }
        if (!RepaintGroupCaptions) return base.HandleCustomDraw(ref m);
        if (draw.Draw.Stage == PostPaint && draw.Draw.Hdc != IntPtr.Zero)
        {
            var group = OLVGroups?.FirstOrDefault(x => x.GroupId == draw.Draw.Item.ToInt32());
            var label = new NativeRect { Top = GroupLabel };
            if (group != null && group.HeaderAlignment == HorizontalAlignment.Left
                && string.IsNullOrEmpty(group.Subtitle) && string.IsNullOrEmpty(group.Task)
                && group.TitleImage is null or -1
                && SendMessage(Handle, GetGroupRect, draw.Draw.Item, ref label) != IntPtr.Zero)
            {
                var bounds = Rectangle.FromLTRB(label.Left, label.Top, label.Right, label.Bottom);
                if (bounds.Width > 0 && bounds.Height > 0 && bounds.IntersectsWith(ClientRectangle))
                {
                    // rcText spans the entire header, including its collapse glyph.
                    // Query the actual label instead; keep native layout, glyph and hit testing.
                    using var graphics = Graphics.FromHdc(draw.Draw.Hdc);
                    graphics.SetClip(bounds, System.Drawing.Drawing2D.CombineMode.Intersect);
                    using var fill = new SolidBrush(BackColor);
                    graphics.FillRectangle(fill, bounds);
                    TextRenderer.DrawText(graphics, group.Header, Font, bounds, ForeColor,
                        TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine
                        | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                        | TextFormatFlags.PreserveGraphicsClipping);
                }
            }
        }
        // Never SKIPDEFAULT: native rendering must still draw the group's rows and glyph.
        m.Result = draw.Draw.Stage == PrePaint ? (IntPtr)NotifyPostPaint : IntPtr.Zero;
        return true;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, ref NativeRect lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct NotificationHeader { public IntPtr Window, Id; public int Code; }
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left, Top, Right, Bottom;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct CustomDraw
    {
        public NotificationHeader Header;
        public int Stage;
        public IntPtr Hdc;
        public NativeRect Bounds;
        public IntPtr Item;
        public int ItemState;
        public IntPtr ItemParam;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct ListCustomDraw
    {
        public CustomDraw Draw;
        public int TextColor, TextBackColor, SubItem, ItemType;
        public int FaceColor, IconEffect, IconPhase, Part, State;
        public NativeRect TextBounds;
        public uint Alignment;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            // Documented .NET 10 opt-in, before the base class creates its native handle.
            // Keep the upstream control unchanged in light and native-only comparisons.
            if (ThemeManager.IsEnabled) SetStyle(ControlStyles.ApplyThemingImplicitly, true);
            return base.CreateParams;
        }
    }
#endif
}
