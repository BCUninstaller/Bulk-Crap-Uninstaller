/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Renders icons from the Windows symbol font (Segoe Fluent Icons on Windows 11, Segoe MDL2 Assets on Windows 10).
    /// </summary>
    public static class SymbolIcons
    {
        public const char Refresh = '';
        public const char SelectAll = '';
        public const char Checkbox = '';
        public const char Switch = '';
        public const char Location = '';
        public const char Delete = '';
        public const char Mute = '';
        public const char Edit = '';
        public const char Document = '';
        public const char Settings = '';
        public const char Help = '';
        public const char More = '';

        private static readonly string FontName = GetInstalledFontName();

        /// <summary>
        ///     True if one of the symbol fonts is installed.
        /// </summary>
        public static bool IsAvailable => FontName != null;

        /// <summary>
        ///     Create an image of the specified symbol, or null if the symbol font is not available.
        /// </summary>
        public static Bitmap CreateImage(char symbol, Size size, Color color)
        {
            if (FontName == null || size.Width <= 0 || size.Height <= 0)
                return null;

            var bitmap = new Bitmap(size.Width, size.Height);
            using var g = Graphics.FromImage(bitmap);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            using var font = new Font(FontName, size.Height * 0.75f, FontStyle.Regular, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(color);
            using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(symbol.ToString(), font, brush, new RectangleF(0, 0, size.Width, size.Height), format);

            return bitmap;
        }

        private static string GetInstalledFontName()
        {
            using var fonts = new InstalledFontCollection();
            var names = fonts.Families.Select(x => x.Name).ToList();
            return new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" }.FirstOrDefault(names.Contains);
        }
    }
}
