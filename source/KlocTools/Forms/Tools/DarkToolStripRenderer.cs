/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Renders menus, toolbars, status bars and context menus with the dark palette.
    /// </summary>
    public class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        public DarkToolStripRenderer() : base(new DarkColorTable())
        {
            RoundedEdges = true;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled ? DarkPalette.MapForeColor(e.TextColor) : DarkPalette.GrayText;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = e.Item == null || e.Item.Enabled ? DarkPalette.Text : DarkPalette.GrayText;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            // Items with their own icon get the default highlight box behind the icon
            if (e.Item.Image != null)
            {
                base.OnRenderItemCheck(e);
                return;
            }

            // The default check mark glyph is black, draw a light one instead
            var rect = e.ImageRectangle;
            if (rect.IsEmpty) return;

            var g = e.Graphics;
            using (var bg = new SolidBrush(ColorTable.CheckBackground))
                g.FillRectangle(bg, rect);
            using (var border = new Pen(ColorTable.ButtonSelectedBorder))
                g.DrawRectangle(border, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(e.Item.Enabled ? DarkPalette.Text : DarkPalette.GrayText, rect.Height / 8f))
            {
                pen.StartCap = pen.EndCap = LineCap.Round;
                g.DrawLines(pen, new[]
                {
                    new PointF(rect.Left + rect.Width * 0.25f, rect.Top + rect.Height * 0.52f),
                    new PointF(rect.Left + rect.Width * 0.43f, rect.Top + rect.Height * 0.70f),
                    new PointF(rect.Left + rect.Width * 0.76f, rect.Top + rect.Height * 0.32f)
                });
            }
            g.SmoothingMode = oldMode;
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public DarkColorTable()
            {
                UseSystemColors = false;
            }

            private static Color Surface => DarkPalette.Surface;
            private static Color Hover => DarkPalette.MenuHighlight;
            private static Color Pressed => DarkPalette.ButtonPressed;
            private static Color BorderC => DarkPalette.Border;

            public override Color ToolStripGradientBegin => Surface;
            public override Color ToolStripGradientMiddle => Surface;
            public override Color ToolStripGradientEnd => Surface;
            public override Color ToolStripBorder => Surface;
            public override Color ToolStripDropDownBackground => Surface;
            public override Color ToolStripContentPanelGradientBegin => Surface;
            public override Color ToolStripContentPanelGradientEnd => Surface;
            public override Color ToolStripPanelGradientBegin => Surface;
            public override Color ToolStripPanelGradientEnd => Surface;

            public override Color MenuStripGradientBegin => Surface;
            public override Color MenuStripGradientEnd => Surface;
            public override Color StatusStripGradientBegin => Surface;
            public override Color StatusStripGradientEnd => Surface;

            public override Color MenuBorder => BorderC;
            public override Color MenuItemBorder => BorderC;
            public override Color MenuItemSelected => Hover;
            public override Color MenuItemSelectedGradientBegin => Hover;
            public override Color MenuItemSelectedGradientEnd => Hover;
            public override Color MenuItemPressedGradientBegin => Surface;
            public override Color MenuItemPressedGradientMiddle => Surface;
            public override Color MenuItemPressedGradientEnd => Surface;

            public override Color ImageMarginGradientBegin => Surface;
            public override Color ImageMarginGradientMiddle => Surface;
            public override Color ImageMarginGradientEnd => Surface;
            public override Color ImageMarginRevealedGradientBegin => Surface;
            public override Color ImageMarginRevealedGradientMiddle => Surface;
            public override Color ImageMarginRevealedGradientEnd => Surface;

            public override Color ButtonSelectedHighlight => Hover;
            public override Color ButtonSelectedHighlightBorder => BorderC;
            public override Color ButtonSelectedBorder => BorderC;
            public override Color ButtonSelectedGradientBegin => Hover;
            public override Color ButtonSelectedGradientMiddle => Hover;
            public override Color ButtonSelectedGradientEnd => Hover;
            public override Color ButtonPressedHighlight => Pressed;
            public override Color ButtonPressedHighlightBorder => BorderC;
            public override Color ButtonPressedBorder => BorderC;
            public override Color ButtonPressedGradientBegin => Pressed;
            public override Color ButtonPressedGradientMiddle => Pressed;
            public override Color ButtonPressedGradientEnd => Pressed;
            public override Color ButtonCheckedHighlight => Pressed;
            public override Color ButtonCheckedHighlightBorder => BorderC;
            public override Color ButtonCheckedGradientBegin => Pressed;
            public override Color ButtonCheckedGradientMiddle => Pressed;
            public override Color ButtonCheckedGradientEnd => Pressed;

            public override Color CheckBackground => Pressed;
            public override Color CheckSelectedBackground => Pressed;
            public override Color CheckPressedBackground => Pressed;

            public override Color GripDark => BorderC;
            public override Color GripLight => Surface;
            public override Color SeparatorDark => BorderC;
            public override Color SeparatorLight => Surface;

            public override Color OverflowButtonGradientBegin => Surface;
            public override Color OverflowButtonGradientMiddle => Surface;
            public override Color OverflowButtonGradientEnd => Surface;
            public override Color RaftingContainerGradientBegin => Surface;
            public override Color RaftingContainerGradientEnd => Surface;
        }
    }
}
