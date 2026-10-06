/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using System.Windows.Forms;

namespace Klocman.Forms
{
    public sealed class CmbBasicSettings
    {
        public CmbBasicSettings(string title, string largeHeading, string smallExplanation, Icon iconSet,
            string rightButton)
            : this(title, largeHeading, smallExplanation, iconSet, null, null,
                rightButton)
        {
        }

        public CmbBasicSettings(string title, string largeHeading, string smallExplanation, Icon iconSet,
            string middleButton, string rightButton)
            : this(title, largeHeading, smallExplanation, iconSet, null, middleButton,
                rightButton)
        {
        }

        /*
        public CustomMessageBoxBasicSettings(string title, string largeHeading, string smallExplanation, Icon iconSet,
            string leftButton, string middleButton, string rightButton):this(title, largeHeading, smallExplanation, iconSet, leftButton, middleButton, rightButton){}

        public CustomMessageBoxBasicSettings(string title, string largeHeading, string smallExplanation, Image iconSet,
            string rightButton):this(title, largeHeading, smallExplanation, iconSet, null, null,
                rightButton) { }

        public CustomMessageBoxBasicSettings(string title, string largeHeading, string smallExplanation, Image iconSet,
            string middleButton, string rightButton):this(title, largeHeading, smallExplanation, iconSet, null, middleButton,
                rightButton) { }
        */

        /// <summary>
        ///     Create a special dialog in the style of Windows XP or Vista. A dialog has a custom icon, an optional large
        ///     title in the form, body text, window text, and one or two custom-labeled buttons.
        /// </summary>
        /// <param name="title">This string will be displayed in the system window frame.</param>
        /// <param name="largeHeading">This is the first string to appear in the dialog. It will be most prominent.</param>
        /// <param name="smallExplanation">
        ///     This string appears either under the big string, or is null, which means it is
        ///     not displayed at all.
        /// </param>
        /// <param name="leftButton">
        ///     This is the left button, typically the "accept" button--label it with an
        ///     action verb (or "OK").
        /// </param>
        /// <param name="middleButton"></param>
        /// <param name="rightButton">The right button--typically "Cancel", but could be "No".</param>
        /// <param name="iconSet">An image to be displayed on the left side of the dialog. Should be 32 x 32 pixels.</param>
        public CmbBasicSettings(string title, string largeHeading, string smallExplanation, Icon iconSet,
            string leftButton, string middleButton, string rightButton)
        {
            Title = title;
            LargeHeading = largeHeading;
            SmallExplanation = smallExplanation;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            IconSet = iconSet;

            StartPosition = FormStartPosition.CenterParent;
            AlwaysOnTop = false;
        }

        public bool AlwaysOnTop { get; set; }
        /// <summary>
        ///     Optional heading foreground. Null preserves the dialog's default styling.
        /// </summary>
        public Color? HeadingColor { get; set; }
        //public Image IconSet { get; private set; }
        public Icon IconSet { get; }
        public string LargeHeading { get; }
        public string LeftButton { get; }
        public string MiddleButton { get; }
        public string RightButton { get; }
        public string SmallExplanation { get; }
        //public Form Parent { get; set; }
        public FormStartPosition StartPosition { get; set; }
        public string Title { get; }
        public Icon WindowIcon { get; set; }
    }
}