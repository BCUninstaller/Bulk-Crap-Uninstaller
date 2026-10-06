using System;
using System.Drawing;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Theming;

internal static class ContrastCheckBoxPainter
{
    internal static void Draw(Graphics graphics, Rectangle box, CheckState state, bool disabled, bool hot)
    {
        var ink = disabled ? SystemColors.GrayText : SystemColors.WindowText;
        var border = !disabled && hot ? SystemColors.HotTrack : ink;

        using (var fill = new SolidBrush(SystemColors.Window)) graphics.FillRectangle(fill, box);
        if (state == CheckState.Checked)
            ControlPaint.DrawMenuGlyph(graphics, box, MenuGlyph.Checkmark, ink, SystemColors.Window);
        else if (state == CheckState.Indeterminate)
        {
            var inset = Math.Max(2, box.Width / 4);
            using var fill = new SolidBrush(ink);
            graphics.FillRectangle(fill, Rectangle.Inflate(box, -inset, -inset));
        }

        var stroke = Math.Max(1, (int)Math.Round(graphics.DpiX / 96f));
        ControlPaint.DrawBorder(graphics, box,
            border, stroke, ButtonBorderStyle.Solid, border, stroke, ButtonBorderStyle.Solid,
            border, stroke, ButtonBorderStyle.Solid, border, stroke, ButtonBorderStyle.Solid);
    }
}
