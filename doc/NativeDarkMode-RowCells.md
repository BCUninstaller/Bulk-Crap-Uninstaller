# Copied application-cell backgrounds

The earlier [row/grid correction](NativeDarkMode-RowGrid.md) refreshed item
backgrounds, but missed cells with their own styles. The main list enables
hyperlinks: ObjectListView's `PostProcessOneRow` calls
`PropagateFormatFromRowToCells` before applying link styles. That copies the status
background into every cell and sets `UseItemStyleForSubItems=false`. Updating only
the item then leaves the other cells with old RGB tints during live high contrast.

`ApplicationRowColors.Apply` now assigns the current background to every subitem
as well as the item. This is confined to BCU application rows registered with the
adapter; BCU uses a single status background across these rows. It preserves
cell foregrounds, fonts, URLs and objects. Assigning `Color.Empty` also clears
copied tints when a highlighted status disappears. No shared ObjectListView change,
list rebuild, handle recreation or new graphics resource is needed.
Palette refreshes batch native list painting with BeginUpdate/EndUpdate.

The same correction was exercised in the earlier .NET 8 and .NET 10 comparison builds. It covers
both ordinary formatting and system-color notifications. High contrast uses
SystemColors.Window; normal and colorblind status palettes return on recovery.

## Verification

The extended checker uses actual hyperlink formatting plus a plain publisher
column, all six highlighted statuses and an ordinary application. Against the
previous DLL, it fails specifically on stale copied cell backgrounds. It also
checks cell identity, hyperlink font/foreground, checked rows, selection, absence
of extra formatting callbacks, and clearing a previous status from all cells.

Native evidence and final build/check results are recorded in the external
prototype's `artifacts/row-cell-contrast/REPORT.md`. The main-window replay uses
certificate highlighting, real inventory and the unchanged published DLL. The
controlled light palette applies the installed hcwhite.theme system-color table
while high contrast is active; it does not select a named theme through Settings.

This closes the copied-cell gap, not full contrast acceptance. The later
[search-field correction](NativeDarkMode-Search.md) addresses its native brush.
Button/checkbox and remaining sidebar painting still needs investigation.
The [release checklist](NativeDarkMode-Checklist.md) retains the
remaining platform, accessibility and production-workflow gates.
