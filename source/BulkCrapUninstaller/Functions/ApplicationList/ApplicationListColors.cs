/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class ApplicationListColors
    {
        public static ApplicationListColors Normal = new(
            Color.FromArgb(unchecked((int)0xffccffcc)), Color.FromArgb(unchecked((int)0xffbbddff)),
            Color.FromArgb(unchecked((int)0xffE0E0E0)), Color.FromArgb(unchecked((int) 0xffffdbcd)),
            Color.FromArgb(unchecked((int) 0xffe7cfff)), Color.FromArgb(unchecked((int)0xffa3ffff)));

        public static ApplicationListColors ColorBlind = new(
            Color.FromArgb(unchecked((int) 0xfff6382d)), Color.FromArgb(unchecked((int)0xfffc8d59)),
            Color.FromArgb(unchecked((int) 0xff5189d3)), Color.FromArgb(unchecked((int)0xff91bfdb)),
            Color.FromArgb(unchecked((int)0xfffee090)), Color.FromArgb(unchecked((int) 0xffc9dade)));

        public ApplicationListColors(Color verifiedColor, Color unverifiedColor, Color invalidColor,
            Color unregisteredColor, Color windowsFeatureColor, Color windowsStoreAppColor)
        {
            VerifiedColor = verifiedColor;
            UnverifiedColor = unverifiedColor;
            InvalidColor = invalidColor;
            UnregisteredColor = unregisteredColor;
            WindowsFeatureColor = windowsFeatureColor;
            WindowsStoreAppColor = windowsStoreAppColor;
        }

        private ApplicationListColors _darkVariant;

        /// <summary>
        ///     Muted variant of these colors that is readable with light text on the dark theme.
        /// </summary>
        public ApplicationListColors DarkVariant => _darkVariant ??= new ApplicationListColors(
            DarkPalette.DarkenAccent(VerifiedColor), DarkPalette.DarkenAccent(UnverifiedColor),
            DarkPalette.DarkenAccent(InvalidColor), DarkPalette.DarkenAccent(UnregisteredColor),
            DarkPalette.DarkenAccent(WindowsFeatureColor), DarkPalette.DarkenAccent(WindowsStoreAppColor));

        public Color VerifiedColor { get; }
        public Color UnverifiedColor { get; }
        public Color InvalidColor { get; }
        public Color UnregisteredColor { get; }
        public Color WindowsFeatureColor { get; }
        public Color WindowsStoreAppColor { get; }
    }
}