/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Properties;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    /// <summary>
    ///     Renders the certificate status as a colored badge.
    /// </summary>
    internal class CertificateBadgeRenderer : BaseRenderer
    {
        private static readonly Color VerifiedBackColor = Color.FromArgb(223, 246, 221);
        private static readonly Color VerifiedForeColor = Color.FromArgb(16, 124, 16);
        private static readonly Color UnverifiedBackColor = Color.FromArgb(255, 244, 206);
        private static readonly Color UnverifiedForeColor = Color.FromArgb(157, 93, 0);

        public override void Render(Graphics g, Rectangle r)
        {
            DrawBackground(g, r);

            var text = GetText();
            if (string.IsNullOrEmpty(text))
                return;

            r = ApplyCellPadding(r);

            // Use the displayed text to pick the color so that the two can't get out of sync
            Color backColor, foreColor;
            if (text == Localisable.CertificateColumn_Verified)
            {
                backColor = VerifiedBackColor;
                foreColor = VerifiedForeColor;
            }
            else if (text == Localisable.CertificateColumn_Unverified)
            {
                backColor = UnverifiedBackColor;
                foreColor = UnverifiedForeColor;
            }
            else
            {
                backColor = ModernColorTable.Border;
                foreColor = ModernColorTable.SecondaryText;
            }

            // The list draws on a translated buffer which TextRenderer ignores, so GDI+ has to be used here
            var textSize = Size.Ceiling(g.MeasureString(text, Font));
            var badgeHeight = textSize.Height + 2;
            var badge = new Rectangle(r.X + 2, r.Y + (r.Height - badgeHeight) / 2, textSize.Width + 12, badgeHeight);
            badge.Width = Math.Min(badge.Width, r.Width - 2);
            if (badge.Width <= 0 || badge.Height <= 0) return;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ModernToolStripRenderer.CreateRoundedRectangle(badge, badgeHeight / 2))
            using (var brush = new SolidBrush(backColor))
                g.FillPath(brush, path);
            g.SmoothingMode = oldMode;

            using var textBrush = new SolidBrush(foreColor);
            using var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
            g.DrawString(text, Font, textBrush, badge, format);
        }
    }
}
