using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Theming;

namespace UniversalUninstaller;

internal static class HelperTheme
{
    private static bool IsDark => Application.IsDarkModeEnabled && !SystemInformation.HighContrast;
    private static Color? Foreground() => IsDark || SystemInformation.HighContrast ? SystemColors.ControlText : null;

    internal static void Apply(Form window)
    {
        ButtonContrastAdapter.Attach(window);
        Visit(window);

        void Visit(Control control)
        {
            if (IsDark && control is ObjectListView list)
            {
                list.BackColor = SystemColors.Window;
                list.ForeColor = SystemColors.WindowText;
                list.GridLines = false;
                list.HeaderUsesThemes = false;
                var header = new HeaderFormatStyle();
                header.SetBackColor(SystemColors.Control);
                header.SetForeColor(SystemColors.ControlText);
                list.HeaderFormatStyle = header;
                list.SelectedBackColor = SystemColors.Highlight;
                list.SelectedForeColor = SystemColors.WindowText;
                list.UnfocusedSelectedBackColor = SystemColors.ControlDark;
                list.UnfocusedSelectedForeColor = SystemColors.ControlText;
                list.CellToolTipShowing += ToolTipShowing;
                list.HeaderToolTipShowing += ToolTipShowing;
            }
            if (control is PictureBox picture)
                new ThemeImageBinding(picture, picture, () => picture.Image,
                    image => picture.Image = image, () => picture.IsDisposed, Foreground).Refresh();
            if (control is ToolStrip strip)
            {
                var bindings = new Dictionary<ToolStripItem, ThemeImageBinding>();
                foreach (ToolStripItem item in strip.Items)
                {
                    if (item.Image == null) continue;
                    var binding = new ThemeImageBinding(item, window, () => item.Image,
                        image => item.Image = image, () => item.IsDisposed, () =>
                            SystemInformation.HighContrast && item.Enabled && (item.Selected || item.Pressed)
                                ? SystemColors.HighlightText : Foreground());
                    binding.Refresh();
                    bindings.Add(item, binding);
                }
                // Refresh before drawing selected high-contrast toolbar items.
                _ = new MenuImageRefresh(strip, target =>
                {
                    if (bindings.TryGetValue(target, out var binding)) binding.Refresh();
                });
            }
            foreach (Control child in control.Controls) Visit(child);
        }
    }

    private static void ToolTipShowing(object sender, ToolTipShowingEventArgs e)
    {
        e.BackColor = SystemColors.Window;
        e.ForeColor = SystemColors.WindowText;
    }
}
