/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Klocman.Controls;
using Klocman.Extensions;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Applies a flat Windows 11 style to a form and all of its child controls, and allows reverting it.
    ///     Group boxes become section headings, check boxes become toggle switches, buttons become flat
    ///     and bordered panels become rounded cards.
    ///     Should be used together with ModernToolStripRenderer. Does nothing if a high contrast theme is active.
    /// </summary>
    public class ModernStyleController
    {
        private const int CardCornerRadius = 8;
        private const int TextBoxHorizontalPadding = 6;

        private readonly Form _reference;
        private readonly List<Action<bool>> _targets = new();
        private bool _enabled;

        /// <summary>
        ///     Style used by forms set up with <see cref="ApplyOnLoad"/>.
        /// </summary>
        public static bool EnabledByDefault { get; set; } = true;

        /// <summary>
        ///     Apply the default style to the form and all of its controls once it loads.
        /// </summary>
        public static void ApplyOnLoad(Form form)
        {
            form.Load += (sender, args) => new ModernStyleController(form).SetStyles(EnabledByDefault);
        }

        public ModernStyleController(Form parentForm)
        {
            _reference = parentForm;

            var originalFormColor = parentForm.BackColor;
            _targets.Add(x => parentForm.BackColor = x ? ModernColorTable.Background : originalFormColor);

            var textBoxes = new List<TextBox>();
            foreach (var control in parentForm.GetAllChildren())
            {
                switch (control)
                {
                    case GroupBox groupBox:
                        groupBox.Paint += GroupBoxOnPaint;
                        _targets.Add(x => groupBox.Invalidate());
                        break;

                    case CheckBox { Appearance: Appearance.Normal } checkBox:
                        AddToggleSwitch(checkBox);
                        break;

                    case Button button:
                        AddButton(button);
                        break;

                    case ComboBox { DropDownStyle: ComboBoxStyle.DropDownList } comboBox:
                        var comboBoxPainter = new ModernComboBoxPainter(comboBox);
                        _targets.Add(x => comboBoxPainter.Enabled = x);
                        break;

                    case SearchBox searchBox:
                        AddSearchBox(searchBox);
                        break;

                    case TextBox { Multiline: false, BorderStyle: BorderStyle.Fixed3D, Parent: not null } textBox:
                        textBoxes.Add(textBox);
                        break;

                    case ToolStrip toolStrip and not MenuStrip and not StatusStrip:
                        var originalPadding = toolStrip.Padding;
                        _targets.Add(x => toolStrip.Padding = x ? new Padding(4, 3, 4, 3) : originalPadding);
                        break;

                    case TabPage tabPage:
                        var originalTabColor = tabPage.BackColor;
                        var originalUseVisualStyleBackColor = tabPage.UseVisualStyleBackColor;
                        _targets.Add(x =>
                        {
                            tabPage.UseVisualStyleBackColor = !x && originalUseVisualStyleBackColor;
                            tabPage.BackColor = x ? ModernColorTable.Surface : originalTabColor;
                        });
                        break;

                    case Panel { BorderStyle: BorderStyle.FixedSingle } panel
                        when panel.Controls.Count == 1 && panel.Controls[0].Dock == DockStyle.Fill:
                        AddCard(panel, new Padding(0, 2, 8, 4));
                        break;
                }
            }

            // Text box frames are painted by their parents, this has to happen after group boxes paint their backgrounds
            foreach (var textBox in textBoxes)
                AddTextBox(textBox);

            parentForm.HandleCreated += (sender, args) => ApplyWindowFrameStyle();
        }

        /// <summary>
        ///     Switch between the modern style (if true) and the original style (if false).
        /// </summary>
        public void SetStyles(bool useModernStyle)
        {
            _enabled = useModernStyle && !SystemInformation.HighContrast;

            _reference.SuspendLayout();
            foreach (var target in _targets)
            {
                target(_enabled);
            }
            _reference.ResumeLayout();

            ApplyWindowFrameStyle();
        }

        /// <summary>
        ///     Draw the container as a rounded card with a border while the modern style is enabled.
        ///     The container's contents should be docked.
        /// </summary>
        /// <param name="container">Container to turn into a card</param>
        /// <param name="margin">Space around the card</param>
        public void AddCard(Control container, Padding margin)
        {
            var originalPadding = container.Padding;
            var originalBorderStyle = (container as Panel)?.BorderStyle;

            container.Paint += (sender, args) =>
            {
                if (_enabled) PaintCard(args.Graphics, container, margin);
            };
            container.Resize += (sender, args) =>
            {
                if (_enabled) container.Invalidate();
            };

            _targets.Add(x =>
            {
                if (container is Panel panel && originalBorderStyle.HasValue)
                    panel.BorderStyle = x ? BorderStyle.None : originalBorderStyle.Value;

                // Keep the contents away from the rounded corners
                var inset = CardCornerRadius / 2;
                container.Padding = x
                    ? new Padding(margin.Left + inset, margin.Top + inset, margin.Right + inset, margin.Bottom + inset)
                    : originalPadding;
                container.Invalidate();
            });
        }

        private static void PaintCard(Graphics g, Control container, Padding margin)
        {
            var background = container.Parent?.BackColor ?? ModernColorTable.Background;
            using (var brush = new SolidBrush(background))
                g.FillRectangle(brush, container.ClientRectangle);

            var bounds = new Rectangle(margin.Left, margin.Top,
                container.Width - margin.Horizontal, container.Height - margin.Vertical);
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ModernToolStripRenderer.CreateRoundedRectangle(bounds, CardCornerRadius))
            {
                using (var brush = new SolidBrush(ModernColorTable.Surface))
                    g.FillPath(brush, path);
                using (var pen = new Pen(ModernColorTable.Border))
                    g.DrawPath(pen, path);
            }
            g.SmoothingMode = oldMode;
        }

        private void AddButton(Button button)
        {
            var originalBackColor = button.BackColor;
            var originalUseVisualStyleBackColor = button.UseVisualStyleBackColor;
            _targets.Add(x =>
            {
                if (x)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackColor = ModernColorTable.Surface;
                    button.ForeColor = ModernColorTable.Text;
                    button.FlatAppearance.BorderColor = ModernColorTable.ControlBorder;
                    button.FlatAppearance.MouseOverBackColor = ModernColorTable.SurfaceHover;
                    button.FlatAppearance.MouseDownBackColor = ModernColorTable.Hover;
                }
                else
                {
                    button.BackColor = originalBackColor;
                    button.UseVisualStyleBackColor = originalUseVisualStyleBackColor;
                    button.ResetForeColor();
                }
            });
        }

        private void AddTextBox(TextBox textBox)
        {
            var parent = textBox.Parent;
            var originalBackColor = textBox.BackColor;
            var verticalPadding = 0;

            // The border is drawn by the parent so that it can have rounded corners and a focus underline
            parent.Paint += (sender, args) =>
            {
                if (_enabled && textBox.Visible)
                    PaintTextBoxFrame(args.Graphics, textBox, verticalPadding);
            };
            textBox.GotFocus += (sender, args) => parent.Invalidate(GetTextBoxFrame(textBox, verticalPadding));
            textBox.LostFocus += (sender, args) => parent.Invalidate(GetTextBoxFrame(textBox, verticalPadding));
            textBox.EnabledChanged += (sender, args) => parent.Invalidate(GetTextBoxFrame(textBox, verticalPadding));

            _targets.Add(x =>
            {
                var isModern = textBox.BorderStyle == BorderStyle.None;
                if (x == isModern) return;

                if (x)
                {
                    var frame = textBox.Bounds;
                    textBox.BorderStyle = BorderStyle.None;
                    textBox.BackColor = ModernColorTable.Surface;
                    verticalPadding = Math.Max((frame.Height - textBox.Height) / 2, 0);
                    textBox.SetBounds(frame.X + TextBoxHorizontalPadding, frame.Y + verticalPadding,
                        frame.Width - TextBoxHorizontalPadding * 2, textBox.Height);
                }
                else
                {
                    var frame = GetTextBoxFrame(textBox, verticalPadding);
                    textBox.BorderStyle = BorderStyle.Fixed3D;
                    textBox.BackColor = originalBackColor;
                    textBox.Bounds = frame;
                }
                parent.Invalidate();
            });
        }

        private void AddSearchBox(SearchBox searchBox)
        {
            var originalBorderStyle = searchBox.SearchBoxBorderStyle;
            var originalBackColor = searchBox.BackColor;
            var originalPadding = searchBox.Padding;
            var originalMinimumSize = searchBox.MinimumSize;
            var originalHeight = searchBox.Height;
            var textBox = searchBox.Controls.OfType<TextBox>().FirstOrDefault();

            searchBox.Paint += (sender, args) =>
            {
                if (!_enabled) return;

                using (var brush = new SolidBrush(GetEffectiveBackColor(searchBox.Parent)))
                    args.Graphics.FillRectangle(brush, searchBox.ClientRectangle);
                PaintTextBoxFrame(args.Graphics, searchBox.ClientRectangle, searchBox.Enabled,
                    textBox?.Focused ?? false);
            };
            if (textBox != null)
            {
                textBox.GotFocus += (sender, args) => searchBox.Invalidate();
                textBox.LostFocus += (sender, args) => searchBox.Invalidate();
            }

            _targets.Add(x =>
            {
                searchBox.SearchBoxBorderStyle = x ? BorderStyle.None : originalBorderStyle;
                searchBox.BackColor = x ? ModernColorTable.Surface : originalBackColor;
                if (x && textBox != null)
                {
                    // Keep the original height, the control shrinks to fit the borderless text box otherwise
                    searchBox.MinimumSize = new Size(originalMinimumSize.Width, originalHeight);
                    var verticalPadding = Math.Max((originalHeight - textBox.Height) / 2, 0);
                    searchBox.Padding = new Padding(TextBoxHorizontalPadding, verticalPadding, TextBoxHorizontalPadding, 0);
                }
                else
                {
                    searchBox.MinimumSize = originalMinimumSize;
                    searchBox.Padding = originalPadding;
                }
                searchBox.Invalidate();
            });
        }

        private static Rectangle GetTextBoxFrame(TextBox textBox, int verticalPadding)
        {
            var frame = textBox.Bounds;
            frame.Inflate(TextBoxHorizontalPadding, verticalPadding);
            return frame;
        }

        private static void PaintTextBoxFrame(Graphics g, TextBox textBox, int verticalPadding)
        {
            PaintTextBoxFrame(g, GetTextBoxFrame(textBox, verticalPadding), textBox.Enabled, textBox.Focused);
        }

        private static void PaintTextBoxFrame(Graphics g, Rectangle frame, bool enabled, bool focused)
        {
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ModernToolStripRenderer.CreateRoundedRectangle(frame, 4))
            {
                using (var brush = new SolidBrush(enabled ? ModernColorTable.Surface : ModernColorTable.Background))
                    g.FillPath(brush, path);
                using (var pen = new Pen(ModernColorTable.ControlBorder))
                    g.DrawPath(pen, path);
            }
            g.SmoothingMode = oldMode;

            if (!enabled) return;

            // Windows 11 text boxes have an underline that turns into a thicker accent line when focused
            using var underline = new Pen(focused ? ModernColorTable.Accent : ModernColorTable.SecondaryText, focused ? 2 : 1);
            var y = frame.Bottom - (focused ? 2 : 1);
            g.DrawLine(underline, frame.Left + 3, y, frame.Right - 4, y);
        }

        private void AddToggleSwitch(CheckBox checkBox)
        {
            var originalPadding = checkBox.Padding;

            checkBox.Paint += CheckBoxOnPaint;
            checkBox.MouseEnter += (sender, args) => checkBox.Invalidate();
            checkBox.MouseLeave += (sender, args) => checkBox.Invalidate();

            _targets.Add(x =>
            {
                // Make room for the switch, it's wider than the check box glyph
                var extraWidth = x ? checkBox.Font.Height : 0;
                checkBox.Padding = new Padding(originalPadding.Left + extraWidth, originalPadding.Top,
                    originalPadding.Right, originalPadding.Bottom);
                checkBox.Invalidate();
            });
        }

        private void CheckBoxOnPaint(object sender, PaintEventArgs e)
        {
            if (!_enabled) return;

            var checkBox = (CheckBox)sender;
            var g = e.Graphics;

            using (var brush = new SolidBrush(GetEffectiveBackColor(checkBox)))
                g.FillRectangle(brush, checkBox.ClientRectangle);

            var switchHeight = Math.Max(checkBox.Font.Height * 3 / 4, 10);
            var switchWidth = switchHeight * 2;
            var rightToLeft = checkBox.RightToLeft == RightToLeft.Yes;
            var switchX = rightToLeft ? checkBox.Width - switchWidth - 1 : 0;
            var switchBounds = new Rectangle(switchX, (checkBox.Height - switchHeight) / 2, switchWidth, switchHeight);

            var isOn = checkBox.CheckState != CheckState.Unchecked;
            var isHot = checkBox.Enabled && checkBox.ClientRectangle.Contains(checkBox.PointToClient(Control.MousePosition));

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ModernToolStripRenderer.CreateRoundedRectangle(switchBounds, switchHeight / 2))
            {
                if (isOn)
                {
                    var fill = !checkBox.Enabled ? ModernColorTable.ControlBorder
                        : isHot ? ModernColorTable.AccentHover : ModernColorTable.Accent;
                    using var brush = new SolidBrush(fill);
                    g.FillPath(brush, path);
                }
                else
                {
                    using (var brush = new SolidBrush(isHot ? ModernColorTable.Hover : ModernColorTable.Surface))
                        g.FillPath(brush, path);
                    using var pen = new Pen(checkBox.Enabled ? ModernColorTable.SecondaryText : ModernColorTable.ControlBorder);
                    g.DrawPath(pen, path);
                }
            }

            var knobSize = switchHeight - 6;
            var knobX = checkBox.CheckState switch
            {
                CheckState.Checked => switchBounds.Right - knobSize - 4,
                CheckState.Indeterminate => switchBounds.Left + (switchWidth - knobSize) / 2,
                _ => switchBounds.Left + 3
            };
            var knobColor = isOn ? Color.White : checkBox.Enabled ? ModernColorTable.SecondaryText : ModernColorTable.ControlBorder;
            using (var brush = new SolidBrush(knobColor))
                g.FillEllipse(brush, knobX, switchBounds.Top + 3, knobSize, knobSize);
            g.SmoothingMode = oldMode;

            var textX = switchWidth + 6;
            var textBounds = rightToLeft
                ? new Rectangle(0, 0, checkBox.Width - textX, checkBox.Height)
                : new Rectangle(textX, 0, checkBox.Width - textX, checkBox.Height);
            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis;
            if (rightToLeft) flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;
            TextRenderer.DrawText(g, checkBox.Text, checkBox.Font, textBounds,
                checkBox.Enabled ? ModernColorTable.Text : SystemColors.GrayText, flags);

            if (checkBox.Focused)
                ControlPaint.DrawFocusRectangle(g, textBounds);
        }

        private void GroupBoxOnPaint(object sender, PaintEventArgs e)
        {
            if (!_enabled) return;

            // Paint over the default 3D frame and draw a section heading instead
            var groupBox = (GroupBox)sender;
            using (var brush = new SolidBrush(GetEffectiveBackColor(groupBox)))
                e.Graphics.FillRectangle(brush, groupBox.ClientRectangle);

            using var font = new Font(groupBox.Font, FontStyle.Bold);
            TextRenderer.DrawText(e.Graphics, groupBox.Text, font, new Point(1, 0),
                groupBox.Enabled ? ModernColorTable.Text : SystemColors.GrayText);
        }

        /// <summary>
        ///     Get the color that is actually visible behind the control, controls can have a transparent background.
        /// </summary>
        private static Color GetEffectiveBackColor(Control control)
        {
            while (control != null && control.BackColor.A < 255)
                control = control.Parent;
            return control?.BackColor ?? ModernColorTable.Background;
        }

        /// <summary>
        ///     Give a popup window, like a menu or a drop down list, small rounded corners.
        ///     Only has an effect on Windows 11.
        /// </summary>
        internal static void SetRoundedCorners(IntPtr handle, bool round)
        {
            if (handle == IntPtr.Zero || Environment.OSVersion.Version.Build < 22000)
                return;

            var cornerPreference = round ? NativeMethods.DWMWCP_ROUNDSMALL : NativeMethods.DWMWCP_DEFAULT;
            NativeMethods.DwmSetWindowAttribute(handle, NativeMethods.DWMWA_WINDOW_CORNER_PREFERENCE,
                ref cornerPreference, sizeof(int));
        }

        private void ApplyWindowFrameStyle()
        {
            // Mica backdrop and rounded corners are only available on Windows 11 22H2 and newer
            if (!_reference.IsHandleCreated || Environment.OSVersion.Version.Build < 22621)
                return;

            var cornerPreference = _enabled ? NativeMethods.DWMWCP_ROUND : NativeMethods.DWMWCP_DEFAULT;
            NativeMethods.DwmSetWindowAttribute(_reference.Handle, NativeMethods.DWMWA_WINDOW_CORNER_PREFERENCE,
                ref cornerPreference, sizeof(int));

            var backdropType = _enabled ? NativeMethods.DWMSBT_MAINWINDOW : NativeMethods.DWMSBT_AUTO;
            NativeMethods.DwmSetWindowAttribute(_reference.Handle, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE,
                ref backdropType, sizeof(int));
        }

        private static class NativeMethods
        {
            public const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
            public const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

            public const int DWMWCP_DEFAULT = 0;
            public const int DWMWCP_ROUND = 2;
            public const int DWMWCP_ROUNDSMALL = 3;

            public const int DWMSBT_AUTO = 0;
            public const int DWMSBT_MAINWINDOW = 2;

            [DllImport("dwmapi.dll")]
            public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int pvAttribute, int cbAttribute);
        }
    }
}
