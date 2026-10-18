/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using BrightIdeasSoftware;
using Klocman.Forms.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    /// <summary>
    ///     Renders the cell normally and draws a stripe with the highlight color of the entry on its left edge.
    ///     The cell contents are moved to the right to make room for the stripe.
    /// </summary>
    internal class ColorStripeRenderer : BaseRenderer
    {
        private const int StripeWidth = 3;
        private const int ContentOffset = StripeWidth + 6;

        public override void Render(Graphics g, Rectangle r)
        {
            DrawBackground(g, r);
            base.Render(g, OffsetContent(r));

            if (RowObject is not ApplicationUninstallerEntry entry) return;

            var color = ApplicationListConstants.GetApplicationBackColor(entry);
            if (color.IsEmpty) return;

            var stripe = new Rectangle(r.X + 1, r.Y + 4, StripeWidth + 1, r.Height - 7);
            if (stripe.Height <= 0) return;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ModernToolStripRenderer.CreateRoundedRectangle(stripe, 1))
            using (var brush = new SolidBrush(GetStripeColor(color)))
                g.FillPath(brush, path);
            g.SmoothingMode = oldMode;
        }

        protected override void HandleHitTest(Graphics g, OlvListViewHitTestInfo hti, int x, int y)
        {
            // Hit test against the moved contents so that check boxes can still be clicked
            var r = CalculateAlignedRectangle(g, ApplyCellPadding(OffsetContent(Bounds)));
            StandardHitTest(g, hti, r, x, y);
        }

        private static Rectangle OffsetContent(Rectangle r)
        {
            r.X += ContentOffset;
            r.Width -= ContentOffset;
            return r;
        }

        /// <summary>
        ///     Highlight colors are pale backgrounds, the stripe needs a stronger version of the same hue.
        /// </summary>
        public static Color GetStripeColor(Color color)
        {
            var saturation = Math.Max(color.GetSaturation(), 0.65f);
            var lightness = Math.Min(color.GetBrightness(), 0.5f);
            return FromHsl(color.GetHue(), saturation, lightness);
        }

        private static Color FromHsl(float hue, float saturation, float lightness)
        {
            var chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            var x = chroma * (1 - Math.Abs(hue / 60f % 2 - 1));
            var m = lightness - chroma / 2;

            float r, g, b;
            if (hue < 60) (r, g, b) = (chroma, x, 0);
            else if (hue < 120) (r, g, b) = (x, chroma, 0);
            else if (hue < 180) (r, g, b) = (0, chroma, x);
            else if (hue < 240) (r, g, b) = (0, x, chroma);
            else if (hue < 300) (r, g, b) = (x, 0, chroma);
            else (r, g, b) = (chroma, 0, x);

            return Color.FromArgb((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
        }
    }
}
