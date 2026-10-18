/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class ApplicationListColors
    {
        public static ApplicationListColors Normal = new(
            Color.FromArgb(unchecked((int)0xffd9f5d9)), Color.FromArgb(unchecked((int)0xffd3e7fb)),
            Color.FromArgb(unchecked((int)0xffe6e6e6)), Color.FromArgb(unchecked((int)0xfffde3d9)),
            Color.FromArgb(unchecked((int)0xffecdcfc)), Color.FromArgb(unchecked((int)0xffc8f3f5)));

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

        public Color VerifiedColor { get; }
        public Color UnverifiedColor { get; }
        public Color InvalidColor { get; }
        public Color UnregisteredColor { get; }
        public Color WindowsFeatureColor { get; }
        public Color WindowsStoreAppColor { get; }
    }
}