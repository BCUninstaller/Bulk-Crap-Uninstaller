using System;
using System.Runtime.CompilerServices;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions.ApplicationList;
using UninstallTools;

namespace BulkCrapUninstaller.Theming;

internal static class ApplicationRowColors
{
    private static readonly ConditionalWeakTable<ObjectListView, object> Attached = new();

    internal static void Attach(ObjectListView list)
    {
        if (Attached.TryGetValue(list, out _)) return;
        Attached.Add(list, new object());
        list.FormatRow += FormatRow;
        list.SystemColorsChanged += PaletteChanged;
    }

    private static void Apply(OLVListItem item)
    {
        if (item.RowObject is ApplicationUninstallerEntry entry)
        {
            // Assign Empty too: a row whose status/highlight changed must not keep
            // an old tint or a high-contrast background after recovery.
            var background = ApplicationListConstants.GetApplicationBackColor(entry);
            item.BackColor = background;
            // ObjectListView copies row colors into every cell before applying
            // hyperlink styles. These BCU rows share one status background, so
            // refresh the copies too; keep per-cell link foregrounds/fonts intact.
            // The item property already updates subitem zero.
            for (var column = 1; column < item.SubItems.Count; column++)
                item.SubItems[column].BackColor = background;
        }
    }

    private static void FormatRow(object sender, FormatRowEventArgs e) => Apply(e.Item);

    private static void PaletteChanged(object sender, EventArgs e)
    {
        var list = (ObjectListView)sender;
        if (list.IsDisposed || list.VirtualMode) return;
        // Recolor existing rows without rebuilding/filtering or repeating other
        // format handlers; checked objects, selection and scroll position survive.
        list.BeginUpdate();
        try
        {
            foreach (OLVListItem item in list.Items) Apply(item);
        }
        finally { list.EndUpdate(); }
        list.Invalidate();
    }
}
