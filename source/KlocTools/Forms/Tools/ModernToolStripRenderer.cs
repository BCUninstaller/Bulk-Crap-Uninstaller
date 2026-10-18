/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Flat tool strip renderer with rounded hover highlights, uses ModernColorTable.
    ///     Falls back to the professional renderer with system colors when a high contrast theme is active.
    /// </summary>
    public class ModernToolStripRenderer : ToolStripProfessionalRenderer
    {
        private const int CornerRadius = 4;

        private static readonly ConditionalWeakTable<ToolStripItem, object> AccentItems = new();
        private static readonly ConditionalWeakTable<ToolStripItem, object> OutlinedItems = new();

        /// <summary>
        ///     Style of a tool strip button.
        /// </summary>
        public enum ButtonStyle
        {
            /// <summary>Background is only drawn on hover</summary>
            Default,
            /// <summary>Filled with the accent color, for the primary action</summary>
            Accent,
            /// <summary>Surface colored with a border, for secondary actions</summary>
            Outlined
        }

        /// <summary>
        ///     Change how the button is drawn by this renderer.
        /// </summary>
        public static void SetButtonStyle(ToolStripItem item, ButtonStyle style)
        {
            AccentItems.Remove(item);
            OutlinedItems.Remove(item);

            if (style == ButtonStyle.Accent)
                AccentItems.Add(item, null);
            else if (style == ButtonStyle.Outlined)
                OutlinedItems.Add(item, null);

            item.Invalidate();
        }

        public ModernToolStripRenderer() : base(SystemInformation.HighContrast
            ? new StandardSystemColorTable()
            : new ModernColorTable())
        {
            RoundedEdges = false;
        }

        private static bool UseFallback => SystemInformation.HighContrast;

        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);

            if (UseFallback || toolStrip is not ToolStripDropDownMenu dropDown) return;

            // Give menus more room around their items, and round their corners on Windows 11
            dropDown.Padding = new Padding(dropDown.Padding.Left, 4, dropDown.Padding.Right, 4);
            if (dropDown.IsHandleCreated)
                ModernStyleController.SetRoundedCorners(dropDown.Handle, true);
            else
                dropDown.HandleCreated += (sender, args) => ModernStyleController.SetRoundedCorners(dropDown.Handle, true);
        }

        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);

            if (!UseFallback && item is ToolStripMenuItem { IsOnDropDown: true })
                item.Padding = new Padding(item.Padding.Left, 4, item.Padding.Right, 4);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderToolStripBackground(e);
                return;
            }

            var color = e.ToolStrip is ToolStripDropDown ? ModernColorTable.Surface : ModernColorTable.Background;
            using var brush = new SolidBrush(color);
            e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderToolStripBorder(e);
                return;
            }

            if (e.ToolStrip is ToolStripDropDown)
            {
                using var pen = new Pen(ModernColorTable.ControlBorder);
                e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            }
            else if (e.ToolStrip is StatusStrip)
            {
                using var pen = new Pen(ModernColorTable.Border);
                e.Graphics.DrawLine(pen, 0, 0, e.ToolStrip.Width, 0);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (UseFallback)
                base.OnRenderImageMargin(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!UseFallback && e.Item.Enabled)
            {
                if (AccentItems.TryGetValue(e.Item, out _))
                    e.TextColor = Color.White;
                else if (e.TextColor == SystemColors.ControlText)
                    e.TextColor = ModernColorTable.Text;
            }

            base.OnRenderItemText(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderMenuItemBackground(e);
                return;
            }

            var item = e.Item;
            if (!item.Selected && !item.Pressed)
                return;
            if (!item.Enabled && item.IsOnDropDown)
                return;

            var bounds = new Rectangle(Point.Empty, item.Size);
            bounds.Inflate(item.IsOnDropDown ? -4 : -1, -1);

            var color = item.IsOnDropDown
                ? ModernColorTable.SurfaceHover
                : item.Pressed ? ModernColorTable.Pressed : ModernColorTable.Hover;
            FillRounded(e.Graphics, bounds, color);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderButtonBackground(e);
                return;
            }

            var item = e.Item;
            var bounds = new Rectangle(Point.Empty, item.Size);
            bounds.Inflate(-1, -1);

            if (AccentItems.TryGetValue(item, out _))
            {
                Color color;
                if (!item.Enabled)
                    color = ModernColorTable.ControlBorder;
                else if (item.Pressed)
                    color = ModernColorTable.AccentPressed;
                else if (item.Selected)
                    color = ModernColorTable.AccentHover;
                else
                    color = ModernColorTable.Accent;

                FillRounded(e.Graphics, bounds, color);
            }
            else if (OutlinedItems.TryGetValue(item, out _))
            {
                Color color;
                if (item.Enabled && item.Pressed)
                    color = ModernColorTable.Hover;
                else if (item.Enabled && item.Selected)
                    color = ModernColorTable.SurfaceHover;
                else
                    color = ModernColorTable.Surface;

                FillRounded(e.Graphics, bounds, color);
                DrawRoundedBorder(e.Graphics, bounds, ModernColorTable.ControlBorder);
            }
            else
            {
                var isChecked = item is ToolStripButton { Checked: true };
                RenderItemHighlight(e.Graphics, item, isChecked);
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderDropDownButtonBackground(e);
                return;
            }

            RenderItemHighlight(e.Graphics, e.Item, false);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderSplitButtonBackground(e);
                return;
            }

            RenderItemHighlight(e.Graphics, e.Item, false);

            if (e.Item is ToolStripSplitButton splitButton)
                DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, splitButton, splitButton.DropDownButtonBounds,
                    splitButton.ForeColor, ArrowDirection.Down));
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderItemCheck(e);
                return;
            }

            var bounds = e.ImageRectangle;
            bounds.Inflate(2, 2);
            FillRounded(e.Graphics, bounds, ModernColorTable.AccentLight);

            if (e.Image != null)
                e.Graphics.DrawImage(e.Image, e.ImageRectangle);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (UseFallback)
            {
                base.OnRenderSeparator(e);
                return;
            }

            var size = e.Item.Size;
            using var pen = new Pen(ModernColorTable.ControlBorder);
            if (e.Vertical)
            {
                var x = size.Width / 2;
                e.Graphics.DrawLine(pen, x, 6, x, size.Height - 6);
            }
            else
            {
                var y = size.Height / 2;
                e.Graphics.DrawLine(pen, 8, y, size.Width - 8, y);
            }
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            if (UseFallback)
                base.OnRenderGrip(e);
        }

        private static void RenderItemHighlight(Graphics g, ToolStripItem item, bool isChecked)
        {
            if (!item.Enabled || (!item.Selected && !item.Pressed && !isChecked))
                return;

            var bounds = new Rectangle(Point.Empty, item.Size);
            bounds.Inflate(-1, -1);

            Color color;
            if (isChecked)
                color = ModernColorTable.AccentLight;
            else if (item.Pressed)
                color = ModernColorTable.Pressed;
            else
                color = ModernColorTable.Hover;

            FillRounded(g, bounds, color);
        }

        private static void FillRounded(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var brush = new SolidBrush(color))
                g.FillPath(brush, path);

            g.SmoothingMode = oldMode;
        }

        private static void DrawRoundedBorder(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedRectangle(bounds, CornerRadius))
            using (var pen = new Pen(color))
                g.DrawPath(pen, path);

            g.SmoothingMode = oldMode;
        }

        /// <summary>
        ///     Create a path of a rectangle with rounded corners that fits inside of the bounds.
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            bounds.Width -= 1;
            bounds.Height -= 1;

            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
