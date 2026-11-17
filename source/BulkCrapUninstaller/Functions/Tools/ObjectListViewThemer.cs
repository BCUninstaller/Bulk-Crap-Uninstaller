/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Functions.Tools
{
    /// <summary>
    ///     Dark theme support for ObjectListView, which draws most of its parts by itself.
    /// </summary>
    internal static class ObjectListViewThemer
    {
        private static bool _registered;
        private static readonly List<WeakReference<ObjectListView>> SeenLists = new();

        public static void Register()
        {
            if (_registered) return;
            _registered = true;
            ThemeManager.AddCustomizer(Customize);
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        /// <summary>
        ///     Rows copy the list's colors when they are created, so lists that were themed have to recreate their rows
        ///     after going back to the light theme too.
        /// </summary>
        private static void OnThemeChanged(object sender, EventArgs e)
        {
            SeenLists.RemoveAll(x => !x.TryGetTarget(out var list) || list.IsDisposed);
            foreach (var reference in SeenLists)
            {
                if (reference.TryGetTarget(out var list))
                {
                    RebuildRows(list);
                    ThemeToolTips(list);
                }
            }
        }

        /// <summary>
        ///     Tooltip colors can't be changed on Vista+, use the native dark theme instead. It has to be applied before
        ///     a tooltip is shown, changing it while the tooltip is asking for its text only affects the next one.
        /// </summary>
        private static void ThemeToolTips(ObjectListView olv)
        {
            if (olv.IsDisposed || !olv.IsHandleCreated) return;
            try
            {
                ThemeManager.ApplyNativeTheme(olv.CellToolTip.Handle);
                ThemeManager.ApplyNativeTheme(olv.HeaderToolTip?.Handle ?? IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to theme list tooltips: " + ex.Message);
            }
        }

        private static void RebuildRows(ObjectListView olv)
        {
            // Virtual lists create their rows on demand, so they already use current colors
            if (olv.VirtualMode || !olv.IsHandleCreated || olv.IsDisposed) return;

            try
            {
                olv.BuildList(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to rebuild list after theme change: " + ex.Message);
            }
        }

        private static void Customize(Control control, ThemeScope scope)
        {
            if (control is not ObjectListView olv) return;

            if (!SeenLists.Any(x => x.TryGetTarget(out var list) && ReferenceEquals(list, olv)))
            {
                SeenLists.Add(new WeakReference<ObjectListView>(olv));

                // The list creates its tooltip windows while its own window is being created, theme them right after.
                // The handle can be recreated when some list properties change, so do it every time.
                olv.HandleCreated += (sender, args) =>
                {
                    try { olv.BeginInvoke(new Action(() => ThemeToolTips(olv))); }
                    catch (InvalidOperationException) { }
                };

                // Fallback in case a tooltip window was recreated in some other way, affects the following tooltips
                olv.CellToolTipShowing += (sender, args) => ThemeManager.ApplyNativeTheme(olv.CellToolTip.Handle);
                olv.HeaderToolTipShowing += (sender, args) => ThemeManager.ApplyNativeTheme(olv.HeaderToolTip?.Handle ?? IntPtr.Zero);
            }

            ThemeToolTips(olv);

            scope.Remember("OlvHeaderUsesThemes", () => olv.HeaderUsesThemes, x => olv.HeaderUsesThemes = x);
            scope.Remember("OlvHeaderFormatStyle", () => olv.HeaderFormatStyle, x => olv.HeaderFormatStyle = x);
            scope.Remember("OlvSelectedBackColor", () => olv.SelectedBackColor, x => olv.SelectedBackColor = x);
            scope.Remember("OlvSelectedForeColor", () => olv.SelectedForeColor, x => olv.SelectedForeColor = x);
            scope.Remember("OlvUnfocusedSelectedBackColor", () => olv.UnfocusedSelectedBackColor, x => olv.UnfocusedSelectedBackColor = x);
            scope.Remember("OlvUnfocusedSelectedForeColor", () => olv.UnfocusedSelectedForeColor, x => olv.UnfocusedSelectedForeColor = x);

            olv.HeaderUsesThemes = false;
            olv.HeaderFormatStyle = CreateHeaderStyle();
            olv.SelectedBackColor = DarkPalette.Selection;
            olv.SelectedForeColor = Color.White;
            olv.UnfocusedSelectedBackColor = DarkPalette.SelectionUnfocused;
            olv.UnfocusedSelectedForeColor = DarkPalette.Text;

            // The dark list theme draws group headers in dark blue, and a disabled list (e.g. while BCU is busy)
            // would get a light background with invisible text
            scope.Remember("OlvGroupHeaderForeColor", () => olv.GroupHeaderForeColor, x => olv.GroupHeaderForeColor = x);
            scope.Remember("OlvDisabledBackColor", () => olv.DisabledBackColor, x => olv.DisabledBackColor = x);
            olv.GroupHeaderForeColor = DarkPalette.Text;
            olv.DisabledBackColor = DarkPalette.Surface;

            if (olv.EmptyListMsgOverlay is TextOverlay overlay)
            {
                scope.Remember("OlvEmptyTextColor", () => overlay.TextColor, x => overlay.TextColor = x);
                scope.Remember("OlvEmptyBackColor", () => overlay.BackColor, x => overlay.BackColor = x);
                scope.Remember("OlvEmptyBorderColor", () => overlay.BorderColor, x => overlay.BorderColor = x);
                overlay.TextColor = DarkPalette.GrayText;
                overlay.BackColor = DarkPalette.Surface;
                overlay.BorderColor = DarkPalette.Border;
            }

            var links = olv.HyperlinkStyle;
            if (links != null)
            {
                scope.Remember("OlvLinkNormal", () => links.Normal.ForeColor, x => links.Normal.ForeColor = x);
                scope.Remember("OlvLinkOver", () => links.Over.ForeColor, x => links.Over.ForeColor = x);
                scope.Remember("OlvLinkVisited", () => links.Visited.ForeColor, x => links.Visited.ForeColor = x);
                links.Normal.ForeColor = DarkPalette.Link;
                links.Over.ForeColor = DarkPalette.LinkActive;
                links.Visited.ForeColor = DarkPalette.LinkVisited;
            }

            // Lists in windows that are opened while already in dark mode have their rows created with light colors
            RebuildRows(olv);
        }

        private static HeaderFormatStyle CreateHeaderStyle()
        {
            var style = new HeaderFormatStyle();
            Set(style.Normal, DarkPalette.Surface);
            Set(style.Hot, DarkPalette.ButtonHover);
            Set(style.Pressed, DarkPalette.ButtonPressed);
            return style;

            static void Set(HeaderStateStyle state, Color back)
            {
                state.BackColor = back;
                state.ForeColor = DarkPalette.Text;
                state.FrameColor = DarkPalette.Border;
                state.FrameWidth = 1f;
            }
        }
    }
}
