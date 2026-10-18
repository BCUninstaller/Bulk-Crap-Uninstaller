/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Klocman.Forms.Tools
{
    /// <summary>
    ///     Draws a drop down list combo box in the style of Windows 11. The closed box is painted over after
    ///     the system paints it, and the items of the open list are owner drawn.
    /// </summary>
    internal sealed class ModernComboBoxPainter : NativeWindow
    {
        private const int WM_PAINT = 0x000F;
        private const int CornerRadius = 4;

        private readonly ComboBox _comboBox;
        private readonly DrawMode _originalDrawMode;
        private readonly int _originalDropDownHeight;
        private readonly bool _originalIntegralHeight;
        private bool _enabled;
        private bool _isHot;

        public ModernComboBoxPainter(ComboBox comboBox)
        {
            _comboBox = comboBox;
            _originalDrawMode = comboBox.DrawMode;
            _originalDropDownHeight = comboBox.DropDownHeight;
            _originalIntegralHeight = comboBox.IntegralHeight;

            // Changing DrawMode recreates the handle
            comboBox.HandleCreated += (sender, args) => AttachToHandle();
            comboBox.HandleDestroyed += (sender, args) => ReleaseHandle();
            comboBox.MouseEnter += (sender, args) => SetHot(true);
            comboBox.MouseLeave += (sender, args) => SetHot(false);
            comboBox.GotFocus += (sender, args) => comboBox.Invalidate();
            comboBox.LostFocus += (sender, args) => comboBox.Invalidate();
            comboBox.MeasureItem += OnMeasureItem;
            comboBox.DrawItem += OnDrawItem;
            comboBox.DropDown += (sender, args) =>
            {
                UpdateDropDownHeight();
                RoundDropDownCorners();
            };

            AttachToHandle();
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;

                // Variable mode only changes the height of list items, the closed box keeps its size
                // Changing these recreates the handle, so it can't be done while the list is opening
                _comboBox.DrawMode = value ? DrawMode.OwnerDrawVariable : _originalDrawMode;
                _comboBox.IntegralHeight = !value && _originalIntegralHeight;
                if (value)
                    UpdateDropDownHeight();
                else
                    _comboBox.DropDownHeight = _originalDropDownHeight;
                _comboBox.Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && _enabled)
            {
                using var g = Graphics.FromHwnd(Handle);
                PaintClosedBox(g);
            }
        }

        private void AttachToHandle()
        {
            if (_comboBox.IsHandleCreated && Handle != _comboBox.Handle)
            {
                if (Handle != IntPtr.Zero) ReleaseHandle();
                AssignHandle(_comboBox.Handle);
            }
        }

        private void SetHot(bool isHot)
        {
            _isHot = isHot;
            if (_enabled) _comboBox.Invalidate();
        }

        private void PaintClosedBox(Graphics g)
        {
            var bounds = _comboBox.ClientRectangle;
            var enabled = _comboBox.Enabled;

            using (var brush = new SolidBrush(GetParentBackColor()))
                g.FillRectangle(brush, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = ModernToolStripRenderer.CreateRoundedRectangle(bounds, CornerRadius))
            {
                var fill = !enabled ? ModernColorTable.Background
                    : _isHot || _comboBox.DroppedDown ? ModernColorTable.SurfaceHover : ModernColorTable.Surface;
                using (var brush = new SolidBrush(fill))
                    g.FillPath(brush, path);
                using (var pen = new Pen(ModernColorTable.ControlBorder))
                    g.DrawPath(pen, path);
            }

            var foreColor = enabled ? ModernColorTable.Text : SystemColors.GrayText;
            var chevronColor = enabled ? ModernColorTable.SecondaryText : SystemColors.GrayText;
            var rightToLeft = _comboBox.RightToLeft == RightToLeft.Yes;

            // Chevron pointing down
            var centerX = rightToLeft ? bounds.Left + 14 : bounds.Right - 14;
            var centerY = bounds.Top + bounds.Height / 2;
            using (var pen = new Pen(chevronColor, 1.3f))
                g.DrawLines(pen, new[]
                {
                    new PointF(centerX - 4, centerY - 2), new PointF(centerX, centerY + 2), new PointF(centerX + 4, centerY - 2)
                });
            g.SmoothingMode = SmoothingMode.Default;

            var textBounds = rightToLeft
                ? Rectangle.FromLTRB(bounds.Left + 26, bounds.Top, bounds.Right - 8, bounds.Bottom)
                : Rectangle.FromLTRB(bounds.Left + 8, bounds.Top, bounds.Right - 26, bounds.Bottom);
            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;
            if (rightToLeft) flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;
            TextRenderer.DrawText(g, _comboBox.Text, _comboBox.Font, textBounds, foreColor, flags);

            if (enabled && _comboBox.Focused && !_comboBox.DroppedDown)
            {
                using var pen = new Pen(ModernColorTable.Accent, 2);
                g.DrawLine(pen, bounds.Left + 3, bounds.Bottom - 2, bounds.Right - 4, bounds.Bottom - 2);
            }
        }

        private Color GetParentBackColor()
        {
            Control control = _comboBox.Parent;
            while (control != null && control.BackColor.A < 255)
                control = control.Parent;
            return control?.BackColor ?? ModernColorTable.Background;
        }

        private int ItemHeight => _comboBox.Font.Height + 12;

        private void OnMeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = ItemHeight;
        }

        private void UpdateDropDownHeight()
        {
            // The default height is meant for normal sized items, show the same number of the taller items
            if (!_enabled) return;
            var visibleItems = Math.Max(Math.Min(_comboBox.Items.Count, _comboBox.MaxDropDownItems), 1);
            _comboBox.DropDownHeight = visibleItems * ItemHeight + 2;
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (!_enabled) return;

            var g = e.Graphics;
            var isEditField = (e.State & DrawItemState.ComboBoxEdit) != 0;

            using (var brush = new SolidBrush(ModernColorTable.Surface))
                g.FillRectangle(brush, e.Bounds);

            if (e.Index < 0) return;

            var text = _comboBox.GetItemText(_comboBox.Items[e.Index]);
            var textBounds = e.Bounds;
            textBounds.Inflate(-8, 0);

            if (!isEditField)
            {
                var highlight = e.Bounds;
                highlight.Inflate(-4, -2);
                if ((e.State & DrawItemState.Selected) != 0)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = ModernToolStripRenderer.CreateRoundedRectangle(highlight, CornerRadius))
                    using (var brush = new SolidBrush(ModernColorTable.Hover))
                        g.FillPath(brush, path);
                    g.SmoothingMode = SmoothingMode.Default;
                }

                // Mark the current value with an accent pill, like Windows 11 does
                if (e.Index == _comboBox.SelectedIndex)
                {
                    var pill = new Rectangle(highlight.Left, highlight.Top + highlight.Height / 4, 3, highlight.Height / 2);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = ModernToolStripRenderer.CreateRoundedRectangle(pill, 1))
                    using (var brush = new SolidBrush(ModernColorTable.Accent))
                        g.FillPath(brush, path);
                    g.SmoothingMode = SmoothingMode.Default;
                }

                textBounds = Rectangle.FromLTRB(highlight.Left + 10, e.Bounds.Top, e.Bounds.Right - 8, e.Bounds.Bottom);
            }

            TextRenderer.DrawText(g, text, e.Font, textBounds, ModernColorTable.Text,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        private void RoundDropDownCorners()
        {
            if (!_enabled) return;

            var info = new NativeMethods.COMBOBOXINFO { cbSize = Marshal.SizeOf<NativeMethods.COMBOBOXINFO>() };
            if (NativeMethods.GetComboBoxInfo(_comboBox.Handle, ref info))
                ModernStyleController.SetRoundedCorners(info.hwndList, true);
        }

        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left, Top, Right, Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct COMBOBOXINFO
            {
                public int cbSize;
                public RECT rcItem;
                public RECT rcButton;
                public int stateButton;
                public IntPtr hwndCombo;
                public IntPtr hwndItem;
                public IntPtr hwndList;
            }

            [DllImport("user32.dll")]
            public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO info);
        }
    }
}
