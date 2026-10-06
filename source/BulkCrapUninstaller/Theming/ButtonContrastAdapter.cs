using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Theming;

// Ordinary buttons/check boxes on explicitly adapted windows use managed
// palette-aware drawing during high contrast. Custom styles are untouched.
internal static class ButtonContrastAdapter
{
    private static readonly ConditionalWeakTable<ButtonBase, Binding> Bindings = new();

    internal static void Attach(Control root)
    {
        if (root is ButtonBase button && (button is Button || button is CheckBox)
            && button.FlatStyle == FlatStyle.Standard)
            Bindings.GetValue(button, key => new Binding(key));
        foreach (Control child in root.Controls) Attach(child);
    }

    private sealed class Binding
    {
        private readonly ButtonBase _button;
        private bool _pending;
        private bool _applied;

        internal Binding(ButtonBase button)
        {
            _button = button;
            button.SystemColorsChanged += (_, _) => Schedule();
            button.HandleCreated += (_, _) => Schedule();
            Schedule();
        }

        private void Schedule()
        {
            if (_pending || _button.IsDisposed || !_button.IsHandleCreated) return;
            _pending = true;
            _button.BeginInvoke(new Action(() =>
            {
                _pending = false;
                if (_button.IsDisposed || _button.Disposing || !_button.IsHandleCreated) return;
                if (SystemInformation.HighContrast)
                {
                    if (_button.FlatStyle == FlatStyle.Standard)
                    {
                        _button.FlatStyle = FlatStyle.Flat;
                        _applied = true;
                    }
                }
                else if (_applied)
                {
                    // Do not overwrite a style subsequently chosen by the caller.
                    if (_button.FlatStyle == FlatStyle.Flat) _button.FlatStyle = FlatStyle.Standard;
                    _applied = false;
                }
            }));
        }
    }
}
