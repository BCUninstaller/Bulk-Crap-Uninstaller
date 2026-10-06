using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions.ApplicationList;
using Klocman.Controls;
using Klocman.Forms;

namespace BulkCrapUninstaller.Theming;

/// <summary>Startup-only, explicit opt-in for the experimentally supported dark surfaces.</summary>
internal static class ThemeManager
{
    private static bool _initialized;
#if NET10_0_OR_GREATER
    private static bool _requested;
#endif
    private static readonly ConditionalWeakTable<Control, object> StyledLists = new();
    private static readonly ConditionalWeakTable<ProgressBar, object> StyledProgress = new();
    private static readonly ConditionalWeakTable<Component, ThemeImageBinding> Images = new();
    private static readonly ConditionalWeakTable<ToolStrip, MenuImageRefresh> MenuRefreshers = new();
    private static readonly ConditionalWeakTable<DataGridView, object> StyledGrids = new();
    private static readonly ConditionalWeakTable<ComboBox, ComboBoxContrastAdapter> StyledFilterDropdowns = new();

    // High contrast is an accessibility override in both runtime builds, including
    // classic/light startup. It does not enable the optional dark control adapters.
    private static Color? IconForeground() => SystemInformation.HighContrast || IsEnabled
        ? SystemColors.ControlText : null;

    private static Color? MenuIconForeground(ToolStripItem item)
    {
        if (!SystemInformation.HighContrast) return IconForeground();
        // Match the built-in high-contrast renderer's highlighted backgrounds.
        var highlighted = item.Enabled && (item.Selected || item.Pressed
            || item is ToolStripButton { Checked: true }
            || item is ToolStripMenuItem { Checked: true, IsOnDropDown: false });
        return highlighted ? SystemColors.HighlightText : SystemColors.ControlText;
    }

    internal static bool IsEnabled
    {
        get
        {
#if NET10_0_OR_GREATER
            return _requested && Application.IsDarkModeEnabled && !SystemInformation.HighContrast;
#else
            return false;
#endif
        }
    }

    internal static void Initialize(bool useDarkMode)
    {
        if (_initialized) return;
        _initialized = true;
#if NET10_0_OR_GREATER
        _requested = ShouldEnableDarkMode(useDarkMode, SystemInformation.HighContrast);
        Application.SetColorMode(_requested ? SystemColorMode.Dark : SystemColorMode.Classic);
        if (!IsEnabled) return;
        CustomMessageBox.DefaultHeadingColor = SystemColors.WindowText;
        ApplicationListColors Adapt(ApplicationListColors c) => new(Tint(c.VerifiedColor), Tint(c.UnverifiedColor),
            Tint(c.InvalidColor), Tint(c.UnregisteredColor), Tint(c.WindowsFeatureColor), Tint(c.WindowsStoreAppColor));
        ApplicationListColors.Normal = Adapt(ApplicationListColors.Normal);
        ApplicationListColors.ColorBlind = Adapt(ApplicationListColors.ColorBlind);
#endif
    }

    internal static bool ShouldEnableDarkMode(bool useDarkMode, bool highContrast)
        => useDarkMode && !highContrast;

    private static Color Tint(Color color)
    {
        var bg = SystemColors.Window;
        return Color.FromArgb((bg.R * 4 + color.R) / 5, (bg.G * 4 + color.G) / 5, (bg.B * 4 + color.B) / 5);
    }

    internal static void ApplyList(ObjectListView list)
    {
        if (!IsEnabled || StyledLists.TryGetValue(list, out _)) return;
        StyledLists.Add(list, new object());
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
        list.HyperlinkStyle = new HyperlinkStyle();
        list.HyperlinkStyle.Normal.ForeColor = Color.FromArgb(139, 194, 255);
        list.HyperlinkStyle.Visited.ForeColor = Color.FromArgb(206, 177, 255);
        list.HyperlinkStyle.Over.ForeColor = Color.FromArgb(186, 219, 255);
        list.CellToolTipShowing += OnListToolTipShowing;
        list.HeaderToolTipShowing += OnListToolTipShowing;
    }

    private static void OnListToolTipShowing(object sender, ToolTipShowingEventArgs args)
    {
        args.BackColor = SystemColors.Window;
        args.ForeColor = SystemColors.WindowText;
    }

    internal static void ApplyControls(Form owner, params ToolStrip[] additionalMenus)
    {
        ButtonContrastAdapter.Attach(owner);
        void Visit(Control control)
        {
            if (IsEnabled && control is ComboBox { DropDownStyle: ComboBoxStyle.DropDownList, DrawMode: DrawMode.Normal } dropdown
                && dropdown.Parent is UninstallTools.Controls.FilterEditor)
                StyledFilterDropdowns.GetValue(dropdown, key => new ComboBoxContrastAdapter(key));
            if (IsEnabled && control is SearchBox search)
            {
                search.BackColor = SystemColors.Window;
                search.NormalSearchColor = SystemColors.WindowText;
                search.InactiveSearchColor = SystemColors.GrayText;
            }
            if (control is ToolStrip strip) ApplyMenu(owner, strip);
            if (control.ContextMenuStrip != null) ApplyMenu(owner, control.ContextMenuStrip);
            foreach (Control child in control.Controls) Visit(child);
        }
        Visit(owner);
        foreach (var menu in additionalMenus) ApplyMenu(owner, menu);
    }

    internal static void ApplyToolTip(ToolTip tooltip)
    {
        if (!IsEnabled) return;
        tooltip.BackColor = SystemColors.Window;
        tooltip.ForeColor = SystemColors.WindowText;
    }

    private static void ApplyMenu(Form owner, ToolStrip strip)
    {
        MenuRefreshers.GetValue(strip, key => new MenuImageRefresh(key, item =>
        {
            if (Images.TryGetValue(item, out var binding)) binding.Refresh();
        }));
        foreach (ToolStripItem item in strip.Items)
        {
            if (item.Image != null)
            {
                Images.GetValue(item, _ => new ThemeImageBinding(item, owner,
                    () => item.Image, image => item.Image = image, () => item.IsDisposed,
                    () => MenuIconForeground(item))).Refresh();
            }
            if (item is ToolStripDropDownItem dropdown && dropdown.HasDropDownItems)
                ApplyMenu(owner, dropdown.DropDown);
        }
    }

    internal static void ApplyProgress(ProgressBar bar)
    {
        if (!IsEnabled || StyledProgress.TryGetValue(bar, out _)) return;
        StyledProgress.Add(bar, new object());
        ProgressBarLifecycleAdapter.Attach(bar);
    }

    internal static void ApplyLoadingDialog(LoadingDialog dialog)
    {
        ButtonContrastAdapter.Attach(dialog);
        if (!IsEnabled) return;
        void Visit(Control control)
        {
            if (control is ProgressBar bar) ApplyProgress(bar);
            foreach (Control child in control.Controls) Visit(child);
        }
        Visit(dialog);
    }

    internal static void ApplyPicture(PictureBox picture)
    {
        Images.GetValue(picture, _ => new ThemeImageBinding(picture, picture,
            () => picture.Image, image => picture.Image = image, () => picture.IsDisposed, IconForeground)).Refresh();
    }

    internal static void ApplyWizard(Form wizard)
    {
        ButtonContrastAdapter.Attach(wizard);
        if (IsEnabled) wizard.BackColor = SystemColors.Control;
        void Visit(Control control)
        {
            if (control is TabPage page) ApplyTabPage(page);
            if (control is ObjectListView list) ApplyList(list);
            if (control is PictureBox picture) ApplyPicture(picture);
            foreach (Control child in control.Controls) Visit(child);
        }
        Visit(wizard);
    }

    internal static void ApplyTabPage(TabPage page)
    {
        if (!IsEnabled) return;
        page.UseVisualStyleBackColor = false;
        page.BackColor = SystemColors.Control;
    }

    internal static void ApplyProperties(Form window, DataGridView grid)
    {
        ButtonContrastAdapter.Attach(window);
        if (grid.ContextMenuStrip != null) ApplyMenu(window, grid.ContextMenuStrip);
        if (!StyledGrids.TryGetValue(grid, out _))
        {
            StyledGrids.Add(grid, new object());
            // Formatting uses the effective style for this paint, including after
            // data-source replacement, without overwriting the ordinary palette.
            grid.CellFormatting += PropertiesCellFormatting;
        }
        if (!IsEnabled) return;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
        grid.DefaultCellStyle.SelectionForeColor = SystemColors.WindowText;
    }

    private static void PropertiesCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!SystemInformation.HighContrast) return;
        e.CellStyle.SelectionBackColor = SystemColors.Highlight;
        e.CellStyle.SelectionForeColor = SystemColors.HighlightText;
    }

    internal static void ShowJunkDetails(Form owner, string text, string title)
    {
        if (IsEnabled)
            CustomMessageBox.ShowDialog(owner, new CmbBasicSettings(title, title, text,
                SystemIcons.Information, Klocman.Forms.Tools.Buttons.ButtonOk));
        else
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

}
