/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Colors used by the dark theme, and helpers to translate light-theme colors into dark equivalents.
    /// </summary>
    public static class DarkPalette
    {
        /// <summary> Background of forms, panels, toolbars (replaces SystemColors.Control) </summary>
        public static readonly Color Surface = Color.FromArgb(0x2B, 0x2B, 0x2B);
        /// <summary> Background of lists and text inputs (replaces SystemColors.Window) </summary>
        public static readonly Color Window = Color.FromArgb(0x1F, 0x1F, 0x1F);
        public static readonly Color Button = Color.FromArgb(0x3A, 0x3A, 0x3A);
        public static readonly Color ButtonHover = Color.FromArgb(0x48, 0x48, 0x48);
        public static readonly Color ButtonPressed = Color.FromArgb(0x55, 0x55, 0x55);
        public static readonly Color Border = Color.FromArgb(0x5A, 0x5A, 0x5A);
        public static readonly Color MenuHighlight = Color.FromArgb(0x45, 0x45, 0x45);
        public static readonly Color Text = Color.FromArgb(0xF0, 0xF0, 0xF0);
        public static readonly Color GrayText = Color.FromArgb(0x96, 0x96, 0x96);
        public static readonly Color Selection = Color.FromArgb(0x1E, 0x4F, 0x7A);
        public static readonly Color SelectionUnfocused = Color.FromArgb(0x3D, 0x3D, 0x3D);
        public static readonly Color Link = Color.FromArgb(0x5C, 0xAE, 0xFF);
        public static readonly Color LinkActive = Color.FromArgb(0x99, 0xCC, 0xFF);
        public static readonly Color LinkVisited = Color.FromArgb(0xB9, 0x9C, 0xF5);

        /// <summary>
        ///     Mix two colors. Ratio of 0 returns <paramref name="from" />, ratio of 1 returns <paramref name="to" />.
        /// </summary>
        public static Color Blend(Color from, Color to, double ratio)
        {
            ratio = Math.Max(0, Math.Min(1, ratio));
            return Color.FromArgb(
                (int)Math.Round(from.R + (to.R - from.R) * ratio),
                (int)Math.Round(from.G + (to.G - from.G) * ratio),
                (int)Math.Round(from.B + (to.B - from.B) * ratio));
        }

        /// <summary>
        ///     Turn a (usually pastel) light-theme highlight color into a muted color that works under light text.
        /// </summary>
        public static Color DarkenAccent(Color lightThemeColor)
        {
            return Blend(Window, lightThemeColor, 0.33);
        }

        /// <summary>
        ///     Translate a light-theme background color into its dark-theme counterpart.
        /// </summary>
        public static Color MapBackColor(Color c)
        {
            if (c.IsEmpty || c.A == 0) return c;

            if (c.IsSystemColor)
            {
                switch (c.ToKnownColor())
                {
                    case KnownColor.Window:
                    case KnownColor.ControlLightLight:
                        return Window;
                    case KnownColor.Control:
                    case KnownColor.ButtonFace:
                    case KnownColor.Menu:
                    case KnownColor.MenuBar:
                    case KnownColor.ControlLight:
                    case KnownColor.AppWorkspace:
                    case KnownColor.GradientActiveCaption:
                    case KnownColor.GradientInactiveCaption:
                    case KnownColor.Info:
                    case KnownColor.ScrollBar:
                        return Surface;
                    case KnownColor.ControlDark:
                    case KnownColor.ControlDarkDark:
                    case KnownColor.ButtonShadow:
                        return Border;
                    case KnownColor.Highlight:
                    case KnownColor.MenuHighlight:
                        return Selection;
                }
            }

            var lightness = c.GetBrightness();
            if (lightness < 0.55f) return c; // Already dark enough

            if (c.GetSaturation() < 0.2f || lightness > 0.96f)
                return lightness > 0.97f ? Window : Surface;

            // Colored light background, keep a hint of the hue
            return DarkenAccent(c);
        }

        /// <summary>
        ///     Translate a light-theme text color into its dark-theme counterpart.
        /// </summary>
        public static Color MapForeColor(Color c)
        {
            if (c.IsEmpty || c.A == 0) return c;

            if (c.IsSystemColor)
            {
                switch (c.ToKnownColor())
                {
                    case KnownColor.ControlText:
                    case KnownColor.WindowText:
                    case KnownColor.MenuText:
                    case KnownColor.InfoText:
                    case KnownColor.ActiveCaptionText:
                    case KnownColor.InactiveCaptionText:
                    case KnownColor.ControlDarkDark:
                        return Text;
                    case KnownColor.GrayText:
                    case KnownColor.ControlDark:
                        return GrayText;
                    case KnownColor.HotTrack:
                        return Link;
                    case KnownColor.HighlightText:
                        return Color.White;
                }
            }

            var lightness = c.GetBrightness();
            if (lightness >= 0.5f) return c; // Already light enough

            if (c.GetSaturation() < 0.25f)
                return lightness < 0.3f ? Text : GrayText;

            // Dark colored text (e.g. dark blue headings), lighten while keeping the hue
            return Blend(c, Color.White, 0.55);
        }
    }
}
