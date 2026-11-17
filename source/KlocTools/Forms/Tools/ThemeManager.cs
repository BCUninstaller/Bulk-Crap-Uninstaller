/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Klocman.Controls;
using Microsoft.Win32;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Called for every control while the dark theme is being applied. Use the scope to remember
    ///     original values of any property you change - they are restored automatically when going back to light.
    /// </summary>
    public delegate void ControlThemeCustomizer(Control control, ThemeScope scope);

    /// <summary>
    ///     Keeps the original (light theme) values of properties changed by the dark theme.
    /// </summary>
    public sealed class ThemeScope
    {
        private readonly Dictionary<string, object> _originals = new();
        private readonly List<Action> _restorers = new();

        internal bool ControlAddedHooked;
        internal bool HandleCreatedHooked;
        internal bool NativeThemeApplied;

        /// <summary>
        ///     Save the current value of a property (only the first time this key is seen) and return the saved value.
        /// </summary>
        public T Remember<T>(string key, Func<T> getter, Action<T> setter)
        {
            if (_originals.TryGetValue(key, out var existing))
                return (T)existing;

            var value = getter();
            _originals[key] = value;
            _restorers.Add(() => setter(value));
            return value;
        }

        /// <summary>
        ///     Save the current value of a property only the first time, using a custom restore action.
        /// </summary>
        public T Remember<T>(string key, Func<T> getter, Action restorer)
        {
            if (_originals.TryGetValue(key, out var existing))
                return (T)existing;

            var value = getter();
            _originals[key] = value;
            _restorers.Add(restorer);
            return value;
        }

        public bool HasRemembered(string key) => _originals.ContainsKey(key);

        internal bool IsEmpty => _restorers.Count == 0;

        internal void Restore()
        {
            for (var i = _restorers.Count - 1; i >= 0; i--)
            {
                try
                {
                    _restorers[i]();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to restore theme value: " + ex.Message);
                }
            }
            _restorers.Clear();
            _originals.Clear();
        }
    }

    /// <summary>
    ///     Applies the light or dark theme to all open windows of the application, including windows opened later.
    ///     The light theme is the standard look of the controls - switching back to it restores original values.
    /// </summary>
    public static class ThemeManager
    {
        private static readonly ConditionalWeakTable<Control, ThemeScope> Scopes = new();
        private static readonly ConditionalWeakTable<Form, object> DarkForms = new();
        private static readonly List<ControlThemeCustomizer> CustomizerList = new();
        private static readonly object DarkMarker = new();

        private static bool _initialized;
        private static bool _applied;
        private static ToolStripRenderer _lightRenderer;

        /// <summary>
        ///     Theme selected by the user.
        /// </summary>
        public static AppTheme SelectedTheme { get; private set; } = AppTheme.Light;

        /// <summary>
        ///     True if the dark theme is currently in effect.
        /// </summary>
        public static bool IsDark { get; private set; }

        /// <summary>
        ///     Raised after the effective theme changed and was applied to all open windows.
        /// </summary>
        public static event EventHandler ThemeChanged;

        /// <summary>
        ///     Register extra theming logic, e.g. for third party controls.
        /// </summary>
        public static void AddCustomizer(ControlThemeCustomizer customizer)
        {
            if (customizer == null) throw new ArgumentNullException(nameof(customizer));
            CustomizerList.Add(customizer);
        }

        /// <summary>
        ///     Select a theme and apply it to all open windows. Windows opened later are themed automatically.
        ///     Must be called from the UI thread.
        /// </summary>
        public static void SetTheme(AppTheme theme)
        {
            if (!_initialized)
            {
                _initialized = true;
                Application.Idle += OnApplicationIdle;
                Application.AddMessageFilter(new PaintMessageFilter());
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            }

            SelectedTheme = theme;
            UpdateEffectiveTheme();
        }

        /// <summary>
        ///     Set the global ToolStrip renderer used by the light theme. Use this instead of setting
        ///     ToolStripManager.Renderer directly, otherwise the dark theme renderer could get overwritten.
        /// </summary>
        public static void SetLightRenderer(ToolStripRenderer renderer)
        {
            if (IsDark)
                _lightRenderer = renderer;
            else
                ToolStripManager.Renderer = renderer;
        }

        /// <summary>
        ///     Check if Windows is set to use dark mode for apps.
        /// </summary>
        public static bool IsSystemDarkModeEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
            }
            catch (SystemException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Apply the current theme to a control and all of its children. Normally not necessary to call manually,
        ///     forms are picked up automatically. Useful after a control changed its own styles.
        /// </summary>
        public static void ApplyTo(Control control)
        {
            if (control == null || control.IsDisposed) return;

            if (IsDark)
                ApplyDark(control);
            else
                ApplyLight(control);

            if (control is Form form)
            {
                if (IsDark) DarkForms.AddOrUpdate(form, DarkMarker);
                else DarkForms.Remove(form);

                if (form.TransparencyKey.IsEmpty)
                    SetTitleBarTheme(form, IsDark);
            }

            control.Invalidate(true);
        }

        private static void UpdateEffectiveTheme()
        {
            var dark = SelectedTheme switch
            {
                AppTheme.Dark => true,
                AppTheme.System => IsSystemDarkModeEnabled(),
                _ => false
            };

            // Don't fight high contrast themes, they need system colors
            if (SystemInformation.HighContrast) dark = false;

            if (_applied && dark == IsDark) return;
            _applied = true;
            IsDark = dark;

            Native.SetAppDarkMode(dark);

            if (dark)
            {
                if (ToolStripManager.Renderer is not DarkToolStripRenderer)
                    _lightRenderer = ToolStripManager.Renderer;
                ToolStripManager.Renderer = new DarkToolStripRenderer();
            }
            else if (_lightRenderer != null)
            {
                ToolStripManager.Renderer = _lightRenderer;
                _lightRenderer = null;
            }

            foreach (var form in Application.OpenForms.Cast<Form>().ToList())
                ApplyTo(form);

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (SelectedTheme != AppTheme.System && e.Category != UserPreferenceCategory.Accessibility) return;

            // This event is raised on a system events thread, marshal to the UI thread
            var owner = Application.OpenForms.Cast<Form>().FirstOrDefault(x => x.IsHandleCreated && !x.IsDisposed);
            if (owner == null) return;
            try
            {
                owner.BeginInvoke(new Action(UpdateEffectiveTheme));
            }
            catch (InvalidOperationException)
            {
                // Form is closing
            }
        }

        /// <summary>
        ///     Theme any newly opened forms. Idle is raised by modal dialog message loops as well.
        /// </summary>
        private static void OnApplicationIdle(object sender, EventArgs e)
        {
            if (!IsDark) return;

            var openForms = Application.OpenForms;
            for (var i = 0; i < openForms.Count; i++)
            {
                var form = openForms[i];
                if (!form.IsDisposed && !DarkForms.TryGetValue(form, out _))
                    ApplyTo(form);
            }
        }

        /// <summary>
        ///     Theme forms right before they are painted for the first time, so they don't flash in light colors.
        /// </summary>
        private sealed class PaintMessageFilter : IMessageFilter
        {
            private const int WM_PAINT = 0x000F;

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != WM_PAINT || !IsDark) return false;

                try
                {
                    if (Control.FromHandle(m.HWnd)?.TopLevelControl is Form form && !form.IsDisposed &&
                        !DarkForms.TryGetValue(form, out _))
                        ApplyTo(form);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to theme form before paint: " + ex.Message);
                }

                return false;
            }
        }

        #region Dark

        private static void ApplyDark(Control c)
        {
            var scope = Scopes.GetValue(c, _ => new ThemeScope());

            try
            {
                ApplyDarkToControl(c, scope);

                foreach (var customizer in CustomizerList)
                    customizer(c, scope);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to apply dark theme to {c.GetType().Name}: {ex.Message}");
            }

            HookNative(c, scope);

            if (!scope.ControlAddedHooked)
            {
                scope.ControlAddedHooked = true;
                c.ControlAdded += OnControlAdded;
            }

            // Hosted controls of a ToolStrip are also in its Controls collection
            foreach (Control child in c.Controls)
                ApplyDark(child);

            if (c is Form or UserControl)
            {
                foreach (var strip in GetToolStripFields(c))
                    ApplyDark(strip);
                HookToolTips(c);
            }

            if (c.ContextMenuStrip != null)
                ApplyDark(c.ContextMenuStrip);
        }

        private static void ApplyDarkToControl(Control c, ThemeScope scope)
        {
            // SearchBox sets the colors of its text box itself (from its BackColor and search colors), changing them
            // here as well would get out of sync and leave the text box dark after switching back to light
            if (c.Parent is SearchBox) return;

            switch (c)
            {
                case SearchBox searchBox:
                    scope.Remember("NormalSearchColor", () => searchBox.NormalSearchColor, x => searchBox.NormalSearchColor = x);
                    scope.Remember("InactiveSearchColor", () => searchBox.InactiveSearchColor, x => searchBox.InactiveSearchColor = x);
                    searchBox.NormalSearchColor = DarkPalette.MapForeColor(searchBox.NormalSearchColor);
                    searchBox.InactiveSearchColor = DarkPalette.MapForeColor(searchBox.InactiveSearchColor);
                    // The BackColor setter passes the color on to the text box
                    RememberColor(searchBox, scope, "BackColor", out var searchBack, true);
                    searchBox.BackColor = DarkPalette.MapBackColor(searchBack);
                    return;

                case TabPage page:
                    scope.Remember("UseVisualStyleBackColor", () => page.UseVisualStyleBackColor, x => page.UseVisualStyleBackColor = x);
                    page.UseVisualStyleBackColor = false;
                    // TabControl.BackColor always reports the light system color, so pages can't inherit a dark one
                    RememberColor(page, scope, "BackColor", out _, true);
                    page.BackColor = DarkPalette.Surface;
                    ApplyForeColor(page, scope);
                    return;

                case TabControl tabs:
                    if (!scope.HasRemembered("TabPainter"))
                    {
                        var painter = new TabControlPainter(tabs);
                        scope.Remember("TabPainter", () => painter, () => painter.Detach());
                    }
                    break;

                case GroupBox groupBox:
                    // The system draws group box borders in a bright color, paint over them
                    if (!scope.HasRemembered("GroupBoxPaint"))
                    {
                        scope.Remember("GroupBoxPaint", () => true, () => groupBox.Paint -= OnGroupBoxPaint);
                        groupBox.Paint += OnGroupBoxPaint;
                    }
                    break;

                case ToolStrip strip:
                    var mode = strip.RenderMode;
                    var renderer = mode == ToolStripRenderMode.Custom ? strip.Renderer : null;
                    scope.Remember("RenderMode", () => mode, () =>
                    {
                        if (renderer != null) strip.Renderer = renderer;
                        else strip.RenderMode = mode;
                    });
                    strip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                    break;
            }

            if (c is ButtonBase buttonBase && (c is Button || (c is CheckBox cb && cb.Appearance == Appearance.Button) ||
                                               (c is RadioButton rb && rb.Appearance == Appearance.Button)))
            {
                scope.Remember("FlatStyle", () => buttonBase.FlatStyle, x => buttonBase.FlatStyle = x);
                scope.Remember("UseVisualStyleBackColor", () => buttonBase.UseVisualStyleBackColor, x => buttonBase.UseVisualStyleBackColor = x);
                var fa = buttonBase.FlatAppearance;
                scope.Remember("FlatBorderColor", () => fa.BorderColor, x => fa.BorderColor = x);
                scope.Remember("FlatMouseOverBackColor", () => fa.MouseOverBackColor, x => fa.MouseOverBackColor = x);
                scope.Remember("FlatMouseDownBackColor", () => fa.MouseDownBackColor, x => fa.MouseDownBackColor = x);
                scope.Remember("FlatCheckedBackColor", () => fa.CheckedBackColor, x => fa.CheckedBackColor = x);
                RememberColor(c, scope, "BackColor", out _, true);

                buttonBase.FlatStyle = FlatStyle.Flat;
                buttonBase.UseVisualStyleBackColor = false;
                fa.BorderColor = DarkPalette.Border;
                fa.MouseOverBackColor = DarkPalette.ButtonHover;
                fa.MouseDownBackColor = DarkPalette.ButtonPressed;
                fa.CheckedBackColor = DarkPalette.Selection;
                c.BackColor = DarkPalette.Button;

                ApplyForeColor(c, scope);
                return;
            }

            // System style ignores custom colors, standard style respects them
            switch (c)
            {
                case ButtonBase bb when bb.FlatStyle == FlatStyle.System:
                    scope.Remember("FlatStyle", () => bb.FlatStyle, x => bb.FlatStyle = x);
                    bb.FlatStyle = FlatStyle.Standard;
                    break;
                case GroupBox gb when gb.FlatStyle == FlatStyle.System:
                    scope.Remember("FlatStyle", () => gb.FlatStyle, x => gb.FlatStyle = x);
                    gb.FlatStyle = FlatStyle.Standard;
                    break;
                case Label lb when lb.FlatStyle == FlatStyle.System:
                    scope.Remember("FlatStyle", () => lb.FlatStyle, x => lb.FlatStyle = x);
                    lb.FlatStyle = FlatStyle.Standard;
                    break;
                case ComboBox combo when combo.DropDownStyle == ComboBoxStyle.DropDownList:
                    // Visual styles draw the closed drop down list in light colors no matter the BackColor
                    scope.Remember("FlatStyle", () => combo.FlatStyle, x => combo.FlatStyle = x);
                    combo.FlatStyle = FlatStyle.Flat;
                    break;
            }

            if (c is LinkLabel link)
            {
                scope.Remember("LinkColor", () => link.LinkColor, x => link.LinkColor = x);
                scope.Remember("ActiveLinkColor", () => link.ActiveLinkColor, x => link.ActiveLinkColor = x);
                scope.Remember("VisitedLinkColor", () => link.VisitedLinkColor, x => link.VisitedLinkColor = x);
                scope.Remember("DisabledLinkColor", () => link.DisabledLinkColor, x => link.DisabledLinkColor = x);
                link.LinkColor = DarkPalette.Link;
                link.ActiveLinkColor = DarkPalette.LinkActive;
                link.VisitedLinkColor = DarkPalette.LinkVisited;
                link.DisabledLinkColor = DarkPalette.GrayText;
            }

            if (c is DataGridView grid)
                ApplyDarkToDataGridView(grid, scope);

            ApplyBackColor(c, scope);
            ApplyForeColor(c, scope);
        }

        private static void ApplyDarkToDataGridView(DataGridView grid, ThemeScope scope)
        {
            scope.Remember("BackgroundColor", () => grid.BackgroundColor, x => grid.BackgroundColor = x);
            scope.Remember("GridColor", () => grid.GridColor, x => grid.GridColor = x);
            scope.Remember("EnableHeadersVisualStyles", () => grid.EnableHeadersVisualStyles, x => grid.EnableHeadersVisualStyles = x);
            RememberCellStyle(scope, "DefaultCellStyle", () => grid.DefaultCellStyle, x => grid.DefaultCellStyle = x);
            RememberCellStyle(scope, "ColumnHeadersDefaultCellStyle", () => grid.ColumnHeadersDefaultCellStyle, x => grid.ColumnHeadersDefaultCellStyle = x);
            RememberCellStyle(scope, "RowHeadersDefaultCellStyle", () => grid.RowHeadersDefaultCellStyle, x => grid.RowHeadersDefaultCellStyle = x);

            grid.BackgroundColor = DarkPalette.Window;
            grid.GridColor = DarkPalette.Border;
            grid.EnableHeadersVisualStyles = false;
            SetCellStyle(grid.DefaultCellStyle, DarkPalette.Window);
            SetCellStyle(grid.ColumnHeadersDefaultCellStyle, DarkPalette.Surface);
            SetCellStyle(grid.RowHeadersDefaultCellStyle, DarkPalette.Surface);

            static void SetCellStyle(DataGridViewCellStyle style, Color back)
            {
                style.BackColor = back;
                style.ForeColor = DarkPalette.Text;
                style.SelectionBackColor = DarkPalette.Selection;
                style.SelectionForeColor = Color.White;
            }

            static void RememberCellStyle(ThemeScope s, string key, Func<DataGridViewCellStyle> get, Action<DataGridViewCellStyle> set)
            {
                s.Remember(key, () => get().Clone(), set);
            }
        }

        private static void ApplyBackColor(Control c, ThemeScope scope)
        {
            if (!RememberColor(c, scope, "BackColor", out var original)) return;

            // Windows with a transparency key are see-through where that color is used (e.g. ObjectListView's
            // overlay panel, the color legend). Changing it would cover whatever is below with a solid color.
            var transparencyKey = (c as Form ?? c.FindForm())?.TransparencyKey ?? Color.Empty;
            if (!transparencyKey.IsEmpty && original.ToArgb() == transparencyKey.ToArgb()) return;

            var mapped = DarkPalette.MapBackColor(original);
            if (mapped != original) c.BackColor = mapped;
        }

        private static void ApplyForeColor(Control c, ThemeScope scope)
        {
            if (!RememberColor(c, scope, "ForeColor", out var original)) return;
            var mapped = DarkPalette.MapForeColor(original);
            if (mapped != original) c.ForeColor = mapped;
        }

        /// <summary>
        ///     Remember original color. Returns false if the color is inherited from the parent (ambient), in which case
        ///     it should not be touched since the parent's color will be used.
        /// </summary>
        private static bool RememberColor(Control c, ThemeScope scope, string propertyName, out Color original,
            bool rememberEvenIfInherited = false)
        {
            if (scope.HasRemembered(propertyName))
            {
                original = scope.Remember<Color>(propertyName, () => default, () => { });
                return true;
            }

            var descriptor = TypeDescriptor.GetProperties(c)[propertyName];
            var value = (Color)descriptor.GetValue(c)!;
            var isExplicit = descriptor.ShouldSerializeValue(c);

            if (!isExplicit && !rememberEvenIfInherited && c.Parent != null)
            {
                var parentValue = (Color)TypeDescriptor.GetProperties(c.Parent)[propertyName].GetValue(c.Parent)!;
                if (parentValue == value)
                {
                    original = value;
                    return false;
                }
            }

            original = scope.Remember(propertyName, () => value, () =>
            {
                if (isExplicit) descriptor.SetValue(c, value);
                else descriptor.ResetValue(c);
            });
            return true;
        }

        /// <summary>
        ///     Paints the whole tab strip of a TabControl. Owner drawing only covers the tab buttons, while the strip
        ///     behind them and the frame would stay light. Tab pages are separate windows and paint themselves.
        /// </summary>
        private sealed class TabControlPainter : NativeWindow
        {
            private const int WM_PAINT = 0x000F;
            private const int WM_ERASEBKGND = 0x0014;
            private static readonly Color UnselectedTabText = Color.FromArgb(0xC8, 0xC8, 0xC8);

            private readonly TabControl _tabs;

            public TabControlPainter(TabControl tabs)
            {
                _tabs = tabs;
                _tabs.HandleCreated += OnHandleCreated;
                if (_tabs.IsHandleCreated) Attach();
            }

            public void Detach()
            {
                _tabs.HandleCreated -= OnHandleCreated;
                if (Handle != IntPtr.Zero) ReleaseHandle();
                if (_tabs.IsHandleCreated) _tabs.Invalidate();
            }

            private void OnHandleCreated(object sender, EventArgs e) => Attach();

            private void Attach()
            {
                // The old handle is released automatically when its window is destroyed
                if (Handle != IntPtr.Zero) ReleaseHandle();
                AssignHandle(_tabs.Handle);
                _tabs.Invalidate();
            }

            protected override void WndProc(ref Message m)
            {
                if (IsDark)
                {
                    if (m.Msg == WM_ERASEBKGND)
                    {
                        m.Result = (IntPtr)1;
                        return;
                    }

                    if (m.Msg == WM_PAINT)
                    {
                        var ps = new Native.PAINTSTRUCT { rgbReserved = new byte[32] };
                        var hdc = Native.BeginPaint(m.HWnd, ref ps);
                        try
                        {
                            PaintBuffered(hdc);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to paint tab control: " + ex.Message);
                        }
                        finally
                        {
                            Native.EndPaint(m.HWnd, ref ps);
                        }
                        m.Result = IntPtr.Zero;
                        return;
                    }
                }

                base.WndProc(ref m);
            }

            private void PaintBuffered(IntPtr hdc)
            {
                var client = _tabs.ClientRectangle;
                if (client.Width <= 0 || client.Height <= 0) return;

                using var target = Graphics.FromHdc(hdc);
                using var buffer = BufferedGraphicsManager.Current.Allocate(target, client);
                Paint(buffer.Graphics);
                buffer.Render(target);
            }

            private void Paint(Graphics g)
            {
                var back = _tabs.Parent?.BackColor is { A: 255 } parentBack ? parentBack : DarkPalette.Surface;
                g.Clear(back);

                // Frame around the page area
                var display = _tabs.DisplayRectangle;
                display.Inflate(1, 1);
                using (var pen = new Pen(DarkPalette.Border))
                    g.DrawRectangle(pen, display.X, display.Y, display.Width - 1, display.Height - 1);

                const TextFormatFlags textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                                                  TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis;

                for (var i = 0; i < _tabs.TabCount; i++)
                {
                    var bounds = _tabs.GetTabRect(i);
                    if (bounds.Width <= 0 || bounds.Height <= 0) continue;

                    var page = _tabs.TabPages[i];
                    var selected = i == _tabs.SelectedIndex;

                    using (var fill = new SolidBrush(selected ? DarkPalette.Surface : DarkPalette.Window))
                        g.FillRectangle(fill, bounds);
                    using (var pen = new Pen(selected ? DarkPalette.Link : DarkPalette.Border))
                        g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

                    var textBounds = bounds;
                    var image = GetTabImage(page);
                    if (image != null)
                    {
                        var y = bounds.Top + (bounds.Height - image.Height) / 2;
                        g.DrawImage(image, bounds.Left + 4, y);
                        textBounds = new Rectangle(bounds.Left + image.Width + 4, bounds.Top,
                            bounds.Width - image.Width - 4, bounds.Height);
                    }

                    TextRenderer.DrawText(g, page.Text, _tabs.Font, textBounds,
                        !page.Enabled ? DarkPalette.GrayText : selected ? DarkPalette.Text : UnselectedTabText,
                        textFlags);
                }
            }

            private Image GetTabImage(TabPage page)
            {
                var images = _tabs.ImageList;
                if (images == null) return null;
                if (page.ImageIndex >= 0 && page.ImageIndex < images.Images.Count) return images.Images[page.ImageIndex];
                return !string.IsNullOrEmpty(page.ImageKey) ? images.Images[page.ImageKey] : null;
            }
        }

        /// <summary>
        ///     Apply the current theme to a native window that is not wrapped by a Control, e.g. a tooltip.
        /// </summary>
        public static void ApplyNativeTheme(IntPtr handle)
        {
            if (handle == IntPtr.Zero) return;

            // Changing the theme of a tooltip makes it pop up again, which would loop forever if done on every popup.
            // The state is stored on the window itself so it goes away together with the window.
            const string propName = "KlocmanThemeState";
            var wanted = (IntPtr)(IsDark ? 2 : 1);
            var current = Native.GetProp(handle, propName);
            if (current == wanted || (current == IntPtr.Zero && !IsDark)) return;

            Native.SetProp(handle, propName, wanted);
            Native.AllowDarkModeForWindow(handle, IsDark);
            Native.SetWindowTheme(handle, IsDark ? "DarkMode_Explorer" : null);
        }

        private static readonly ConditionalWeakTable<ToolTip, object> HookedToolTips = new();
        private static readonly PropertyInfo ToolTipHandleProperty =
            typeof(ToolTip).GetProperty("Handle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static void HookToolTips(Control c)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            for (var type = c.GetType(); type != null && type != typeof(Form) && type != typeof(UserControl); type = type.BaseType)
            {
                foreach (var field in type.GetFields(flags))
                {
                    if (!typeof(ToolTip).IsAssignableFrom(field.FieldType)) continue;
                    if (field.GetValue(c) is not ToolTip toolTip || HookedToolTips.TryGetValue(toolTip, out _)) continue;

                    HookedToolTips.Add(toolTip, DarkMarker);
                    // The tooltip window is created lazily, theme it right before it is shown (also restores light)
                    toolTip.Popup += (sender, args) =>
                    {
                        if (ToolTipHandleProperty?.GetValue(sender) is IntPtr handle)
                            ApplyNativeTheme(handle);
                    };
                }
            }
        }

        /// <summary>
        ///     Runs after GroupBox finished its own painting, replaces the border and caption.
        ///     Child controls are separate windows, so they are not affected.
        /// </summary>
        private static void OnGroupBoxPaint(object sender, PaintEventArgs e)
        {
            if (!IsDark) return;

            var box = (GroupBox)sender;
            var g = e.Graphics;
            var back = box.BackColor.A == 255 ? box.BackColor : box.Parent?.BackColor ?? DarkPalette.Surface;

            g.Clear(back);

            const TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
            var hasText = !string.IsNullOrEmpty(box.Text);
            var textSize = hasText ? TextRenderer.MeasureText(g, box.Text, box.Font, Size.Empty, flags) : Size.Empty;
            var top = hasText ? textSize.Height / 2 : 0;

            using (var pen = new Pen(DarkPalette.Border))
                g.DrawRectangle(pen, 0, top, box.Width - 1, box.Height - top - 1);

            if (!hasText) return;

            var x = box.RightToLeft == RightToLeft.Yes ? box.Width - 8 - textSize.Width : 8;
            var textRect = new Rectangle(x - 2, 0, textSize.Width + 4, textSize.Height);
            using (var brush = new SolidBrush(back))
                g.FillRectangle(brush, textRect);

            TextRenderer.DrawText(g, box.Text, box.Font, new Point(x, 0),
                box.Enabled ? DarkPalette.MapForeColor(box.ForeColor) : DarkPalette.GrayText, flags);
        }

        private static IEnumerable<ToolStrip> GetToolStripFields(Control c)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            for (var type = c.GetType(); type != null && type != typeof(Form) && type != typeof(UserControl); type = type.BaseType)
            {
                foreach (var field in type.GetFields(flags))
                {
                    if (!typeof(ToolStripDropDown).IsAssignableFrom(field.FieldType)) continue;
                    if (field.GetValue(c) is ToolStrip strip && !strip.IsDisposed)
                        yield return strip;
                }
            }
        }

        private static void OnControlAdded(object sender, ControlEventArgs e)
        {
            if (IsDark && e.Control != null)
                ApplyDark(e.Control);
        }

        #endregion

        #region Light

        private static void ApplyLight(Control c)
        {
            if (Scopes.TryGetValue(c, out var scope))
            {
                scope.Restore();
                if (scope.NativeThemeApplied)
                    ApplyNativeTheme(c, scope, false);
            }

            foreach (Control child in c.Controls)
                ApplyLight(child);

            if (c is Form or UserControl)
            {
                foreach (var strip in GetToolStripFields(c))
                    ApplyLight(strip);
            }

            if (c.ContextMenuStrip != null)
                ApplyLight(c.ContextMenuStrip);
        }

        #endregion

        #region Native

        private static void HookNative(Control c, ThemeScope scope)
        {
            if (GetNativeThemeName(c) == null) return;

            if (!scope.HandleCreatedHooked)
            {
                scope.HandleCreatedHooked = true;
                // Handles can get recreated when some properties change, re-apply every time
                c.HandleCreated += (sender, args) =>
                {
                    if (IsDark) ApplyNativeTheme((Control)sender, scope, true);
                };
            }

            if (c.IsHandleCreated)
                ApplyNativeTheme(c, scope, true);
        }

        private static string GetNativeThemeName(Control c)
        {
            switch (c)
            {
                case ComboBox:
                    return "DarkMode_CFD";
                case TextBoxBase tb:
                    return tb.Multiline ? "DarkMode_Explorer" : "DarkMode_CFD";
                case ListView:
                    // Same as Explorer's file list. Unlike DarkMode_Explorer it draws group headers in a light color
                    return "DarkMode_ItemsView";
                case TreeView:
                case ListBox:
                case ScrollBar:
                case DataGridView:
                    return "DarkMode_Explorer";
                case ScrollableControl sc when sc.AutoScroll && c is not ToolStrip:
                    return "DarkMode_Explorer";
                default:
                    return null;
            }
        }

        private static void ApplyNativeTheme(Control c, ThemeScope scope, bool dark)
        {
            if (!c.IsHandleCreated) return;

            var themeName = GetNativeThemeName(c);
            if (themeName == null) return;

            scope.NativeThemeApplied = dark;

            Native.AllowDarkModeForWindow(c.Handle, dark);
            Native.SetWindowTheme(c.Handle, dark ? themeName : null);

            if (c is ListView)
            {
                var header = Native.GetListViewHeader(c.Handle);
                if (header != IntPtr.Zero)
                {
                    Native.AllowDarkModeForWindow(header, dark);
                    Native.SetWindowTheme(header, dark ? "DarkMode_ItemsView" : null);
                }
            }

            // Tooltips showing truncated item text
            var toolTip = c switch
            {
                ListView => Native.SendMessage(c.Handle, Native.LVM_GETTOOLTIPS, IntPtr.Zero, IntPtr.Zero),
                TreeView => Native.SendMessage(c.Handle, Native.TVM_GETTOOLTIPS, IntPtr.Zero, IntPtr.Zero),
                _ => IntPtr.Zero
            };
            if (toolTip != IntPtr.Zero)
            {
                Native.AllowDarkModeForWindow(toolTip, dark);
                Native.SetWindowTheme(toolTip, dark ? "DarkMode_Explorer" : null);
            }
        }

        private static void SetTitleBarTheme(Form form, bool dark)
        {
            if (form.IsHandleCreated)
            {
                Native.SetTitleBarDark(form.Handle, dark);
                return;
            }

            void OnCreated(object sender, EventArgs e)
            {
                form.HandleCreated -= OnCreated;
                Native.SetTitleBarDark(form.Handle, IsDark);
            }

            form.HandleCreated -= OnCreated;
            form.HandleCreated += OnCreated;
        }

        private static class Native
        {
            private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
            private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
            private const int LVM_GETHEADER = 0x1000 + 31;
            public const int LVM_GETTOOLTIPS = 0x1000 + 78;
            public const int TVM_GETTOOLTIPS = 0x1100 + 25;

            private const uint SWP_NOSIZE = 0x0001, SWP_NOMOVE = 0x0002, SWP_NOZORDER = 0x0004,
                SWP_NOACTIVATE = 0x0010, SWP_FRAMECHANGED = 0x0020;

            // Undocumented uxtheme APIs exist since Windows 10 1903 (build 18362)
            private static readonly bool SupportsDarkApis = Environment.OSVersion.Version.Major >= 10 &&
                                                             Environment.OSVersion.Version.Build >= 18362;

            private static bool _darkApisBroken;

            public static void SetAppDarkMode(bool dark)
            {
                if (!SupportsDarkApis || _darkApisBroken) return;
                try
                {
                    // 0 = Default, 2 = ForceDark. Makes native menus (e.g. text box context menu) dark
                    SetPreferredAppMode(dark ? 2 : 0);
                    FlushMenuThemes();
                }
                catch (Exception ex) when (ex is EntryPointNotFoundException or DllNotFoundException)
                {
                    _darkApisBroken = true;
                }
            }

            public static void AllowDarkModeForWindow(IntPtr handle, bool allow)
            {
                if (!SupportsDarkApis || _darkApisBroken) return;
                try
                {
                    AllowDarkModeForWindowNative(handle, allow);
                }
                catch (Exception ex) when (ex is EntryPointNotFoundException or DllNotFoundException)
                {
                    _darkApisBroken = true;
                }
            }

            public static void SetWindowTheme(IntPtr handle, string themeName)
            {
                try
                {
                    SetWindowThemeNative(handle, themeName, null);
                }
                catch (Exception ex) when (ex is EntryPointNotFoundException or DllNotFoundException)
                {
                    // Themes not available
                }
            }

            public static IntPtr GetListViewHeader(IntPtr listView) => SendMessage(listView, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            public static void SetTitleBarDark(IntPtr handle, bool dark)
            {
                try
                {
                    AllowDarkModeForWindow(handle, dark);

                    var value = dark ? 1 : 0;
                    if (DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int)) != 0)
                        DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref value, sizeof(int));

                    // Force the non-client area to redraw
                    SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
                        SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
                }
                catch (Exception ex) when (ex is EntryPointNotFoundException or DllNotFoundException)
                {
                    // Not supported before Windows 10
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct PAINTSTRUCT
            {
                public IntPtr hdc;
                public bool fErase;
                public int rcLeft, rcTop, rcRight, rcBottom;
                public bool fRestore;
                public bool fIncUpdate;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public byte[] rgbReserved;
            }

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr GetProp(IntPtr hWnd, string name);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern bool SetProp(IntPtr hWnd, string name, IntPtr data);

            [DllImport("user32.dll")]
            public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT paint);

            [DllImport("user32.dll")]
            public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT paint);

            [DllImport("dwmapi.dll")]
            private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

            [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowTheme")]
            private static extern int SetWindowThemeNative(IntPtr hwnd, string subAppName, string subIdList);

            [DllImport("uxtheme.dll", EntryPoint = "#133")]
            private static extern bool AllowDarkModeForWindowNative(IntPtr hwnd, bool allow);

            [DllImport("uxtheme.dll", EntryPoint = "#135")]
            private static extern int SetPreferredAppMode(int appMode);

            [DllImport("uxtheme.dll", EntryPoint = "#136")]
            private static extern void FlushMenuThemes();

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
        }

        #endregion
    }
}
