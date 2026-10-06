using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BulkCrapUninstaller.Theming;

/// <summary>Owns palette-specific copies while retaining the shared source image.</summary>
internal sealed class ThemeImageBinding : IDisposable
{
    private readonly Component _target;
    private readonly Control _dispatcher;
    private readonly Func<Image> _getImage;
    private readonly Action<Image> _setImage;
    private readonly Func<bool> _targetDisposed;
    private readonly Func<Color?> _foreground;
    private Image _source;
    private Image _owned;
    private int? _appliedArgb;
    private bool _hasPalette;
    private int _pending;
    private volatile bool _disposed;

    internal ThemeImageBinding(Component target, Control dispatcher, Func<Image> getImage,
        Action<Image> setImage, Func<bool> targetDisposed, Func<Color?> foreground)
    {
        _target = target;
        _dispatcher = dispatcher;
        _getImage = getImage;
        _setImage = setImage;
        _targetDisposed = targetDisposed;
        _foreground = foreground;
        _source = getImage();
        target.Disposed += TargetDisposed;
        dispatcher.Disposed += TargetDisposed;
        dispatcher.HandleCreated += PaletteChanged;
        dispatcher.HandleDestroyed += HandleDestroyed;
        dispatcher.SystemColorsChanged += PaletteChanged;
        SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
    }

    internal void Refresh()
    {
        if (_disposed || _targetDisposed()) return;
        var current = _getImage();
        if (!ReferenceEquals(current, _owned ?? _source))
        {
            // For example, progress replaces its instruction image on completion.
            _source = current;
            _owned?.Dispose();
            _owned = null;
            _hasPalette = false;
        }
        var argb = _foreground()?.ToArgb();
        if (_hasPalette && _appliedArgb == argb) return;
        var replacement = argb.HasValue && _source != null
            ? MonochromeIcons.CreateForForeground(_source, Color.FromArgb(argb.Value)) : null;
        var previous = _owned;
        _setImage(replacement ?? _source);
        _owned = replacement;
        _appliedArgb = argb;
        _hasPalette = true;
        previous?.Dispose();
    }

    private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) => Schedule();
    private void PaletteChanged(object sender, EventArgs e) => Schedule();
    private void HandleDestroyed(object sender, EventArgs e) => Interlocked.Exchange(ref _pending, 0);

    private void Schedule()
    {
        // SystemEvents can arrive on its own thread. Resolve the actual colors
        // on the UI thread after WinForms has processed the system notification.
        if (_disposed || _dispatcher.IsDisposed || !_dispatcher.IsHandleCreated) return;
        if (Interlocked.Exchange(ref _pending, 1) != 0) return;
        try
        {
            _dispatcher.BeginInvoke((Action)(() =>
            {
                Interlocked.Exchange(ref _pending, 0);
                Refresh();
            }));
        }
        catch (InvalidOperationException) when (_disposed || _dispatcher.IsDisposed || !_dispatcher.IsHandleCreated)
        {
            Interlocked.Exchange(ref _pending, 0);
            // A disappearing/recreated handle will refresh on its next creation.
        }
    }

    private void TargetDisposed(object sender, EventArgs e) => Dispose();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        _target.Disposed -= TargetDisposed;
        _dispatcher.Disposed -= TargetDisposed;
        _dispatcher.HandleCreated -= PaletteChanged;
        _dispatcher.HandleDestroyed -= HandleDestroyed;
        _dispatcher.SystemColorsChanged -= PaletteChanged;
        if (_owned != null && !_targetDisposed() && ReferenceEquals(_getImage(), _owned))
            _setImage(_source);
        _owned?.Dispose();
        _owned = null;
    }
}
