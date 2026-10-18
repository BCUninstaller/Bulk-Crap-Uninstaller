/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Flat color palette in the style of Windows 11.
    /// </summary>
    public class ModernColorTable : ProfessionalColorTable
    {
        public static Color Background { get; } = Color.FromArgb(243, 243, 243);
        public static Color Surface { get; } = Color.White;
        public static Color Border { get; } = Color.FromArgb(229, 229, 229);
        public static Color ControlBorder { get; } = Color.FromArgb(208, 208, 208);
        public static Color Hover { get; } = Color.FromArgb(232, 232, 232);
        public static Color SurfaceHover { get; } = Color.FromArgb(245, 245, 245);
        public static Color Pressed { get; } = Color.FromArgb(222, 222, 222);
        public static Color Accent { get; } = Color.FromArgb(0, 95, 184);
        public static Color AccentHover { get; } = Color.FromArgb(25, 117, 197);
        public static Color AccentPressed { get; } = Color.FromArgb(49, 131, 202);
        public static Color AccentLight { get; } = Color.FromArgb(204, 228, 247);
        public static Color Text { get; } = Color.FromArgb(27, 27, 27);
        public static Color SecondaryText { get; } = Color.FromArgb(96, 96, 96);

        public override Color ToolStripGradientBegin => Background;
        public override Color ToolStripGradientMiddle => Background;
        public override Color ToolStripGradientEnd => Background;
        public override Color MenuStripGradientBegin => Background;
        public override Color MenuStripGradientEnd => Background;
        public override Color StatusStripGradientBegin => Background;
        public override Color StatusStripGradientEnd => Background;
        public override Color ToolStripDropDownBackground => Surface;
        public override Color SeparatorDark => Border;
        public override Color SeparatorLight => Border;
    }
}
