using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Theming;

// Scoped to the standard search-filter dropdowns in an opted-in dark process.
internal sealed class ComboBoxContrastAdapter
{
#if NET10_0_OR_GREATER
    private readonly ComboBox _combo;
    private readonly Color _originalForeground;
    private readonly Color _originalBackground;
    private bool _contrastApplied;
    private bool _pending;
#endif

    internal ComboBoxContrastAdapter(ComboBox combo)
    {
#if NET10_0_OR_GREATER
        _combo = combo;
        var properties = TypeDescriptor.GetProperties(combo);
        _originalForeground = properties[nameof(combo.ForeColor)].ShouldSerializeValue(combo) ? combo.ForeColor : Color.Empty;
        _originalBackground = properties[nameof(combo.BackColor)].ShouldSerializeValue(combo) ? combo.BackColor : Color.Empty;
        combo.SystemColorsChanged += (_, _) => Schedule();
        combo.HandleCreated += (_, _) => Schedule();
        Schedule();
#endif
    }

#if NET10_0_OR_GREATER
    private void Schedule()
    {
        if (_pending || _combo.IsDisposed || !_combo.IsHandleCreated) return;
        _pending = true;
        _combo.BeginInvoke(() =>
        {
            _pending = false;
            if (_combo.IsDisposed || _combo.Disposing || !_combo.IsHandleCreated) return;
            Refresh();
        });
    }

    private void Refresh()
    {
        var contrast = SystemInformation.HighContrast;
        if (contrast)
        {
            // RGB assignments also discard a background brush cached in dark mode.
            _combo.ForeColor = Color.FromArgb(SystemColors.WindowText.ToArgb());
            _combo.BackColor = Color.FromArgb(SystemColors.Window.ToArgb());
            _contrastApplied = true;
        }
        else if (_contrastApplied)
        {
            _combo.ForeColor = _originalForeground;
            _combo.BackColor = _originalBackground;
            _contrastApplied = false;
        }

        // .NET 10 applies these native associations only during handle creation.
        // Contrast uses classic native painting so all colors follow the palette;
        // recovery restores the same dark associations as WinForms itself.
        var dark = Application.IsDarkModeEnabled && !contrast;
        SetWindowTheme(_combo.Handle, contrast ? " " : dark ? "DarkMode_CFD" : null, contrast ? " " : null);
        var info = new ComboInfo { Size = Marshal.SizeOf<ComboInfo>() };
        if (GetComboBoxInfo(_combo.Handle, ref info) && info.List != IntPtr.Zero)
            SetWindowTheme(info.List, contrast ? " " : dark ? "DarkMode_Explorer" : null, contrast ? " " : null);
        _combo.Invalidate();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)]
    private struct ComboInfo
    {
        public int Size;
        public NativeRect Item, Button;
        public uint State;
        public IntPtr Combo, Edit, List;
    }
    [DllImport("user32.dll")]
    private static extern bool GetComboBoxInfo(IntPtr window, ref ComboInfo info);
    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr window, string app, string id);
#endif
}
