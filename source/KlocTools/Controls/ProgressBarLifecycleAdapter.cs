using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Klocman.Controls;

public static class ProgressBarLifecycleAdapter
{
    private static readonly ConditionalWeakTable<ProgressBar, object> Bindings = new();

    // Attach once per bar, in adapted dark mode. Style changes recreate native
    // handles; the initial OnCreateControl theme setup does not survive that path.
    public static void Attach(ProgressBar bar)
    {
#if NET10_0_OR_GREATER
        if (!Application.IsDarkModeEnabled || SystemInformation.HighContrast
            || Bindings.TryGetValue(bar, out _)) return;
        Bindings.Add(bar, new object());
        bar.HandleCreated += (_, _) => Schedule();
        if (bar.IsHandleCreated) Schedule();

        void Schedule()
        {
            var handle = bar.Handle;
            // Run after the native handle's normal initialization/theme callbacks.
            bar.BeginInvoke(() =>
            {
                if (bar.IsDisposed || !bar.IsHandleCreated || bar.Handle != handle
                    || !Application.IsDarkModeEnabled || SystemInformation.HighContrast) return;
                // Same visual-style opt-out used by .NET 10 ProgressBar itself.
                // Its normal foreground/background, value and marquee logic remain intact.
                SetWindowTheme(handle, " ", " ");
                bar.Invalidate();
            });
        }
#endif
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr window, string subAppName, string subIdList);
}
