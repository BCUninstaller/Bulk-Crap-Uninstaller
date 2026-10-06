# Search-filter dropdown contrast follow-up

The native main-window replay found that the selected search-filter text and popup
items could become unreadable after entering a light contrast palette from an
already-dark .NET 10 process. The selected objects, text and item counts remained
intact; the native background and text rendering disagreed about the active palette.

## Cause and scoped correction

WinForms applies `DarkMode_CFD` to the ComboBox and `DarkMode_Explorer` to its native
popup during handle creation. Native theme associations persist across visual-style
changes. The control also caches a background brush. Refreshing colors alone did
not fix the closed field; removing the native theme alone left the popup brush dark.
The combined native-theme and color refresh corrected both observed surfaces.
Sources: [WinForms ComboBox](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Controls/ComboBox/ComboBox.cs),
[Control background brush](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Control.cs),
[SetWindowTheme contract](https://learn.microsoft.com/en-us/windows/win32/api/uxtheme/nf-uxtheme-setwindowtheme).

`ComboBoxContrastAdapter` attaches once to normal DropDownList controls directly
inside FilterEditor, only when the .NET 10 dark opt-in is active. In the historical .NET 8
build, explicit light override, contrast-at-startup path, unrelated dropdowns,
editable combos and owner-drawn controls do not acquire this adapter.

On a system-color or native-handle change, a coalesced UI callback supplies current
Window/WindowText RGB values to discard stale brush state. During contrast it opts
the ComboBox and popup out of visual styles, retaining native Win32 drawing with
system colors. Recovery restores original color properties (including ambient
defaults) and the same dark theme names WinForms uses. It neither recreates handles
nor changes data, selection, text, draw mode or item ownership. Opening a popup
does not trigger a theme reset; an early test caught that doing so closes it.

## Verification and limitations

The external assembly checker covers attachment gates, idempotence/state retention,
unrelated-control preservation, the actual brush returned by WM_CTLCOLORLISTBOX,
handle recreation and disposal with pending callbacks. Native captures and JSONL
use the compiled main window with no final host-side color/theme override.
Exact build hashes, runs, screenshots and restoration records are in the workspace
artifact `prototype/artifacts/dropdown-contrast/REPORT.md`.

The light test uses the installed hcwhite.theme color table while Windows contrast
is active. It is a controlled palette stress test, not native Settings selection of
that theme. The configured dark contrast palette and ordinary dark recovery are
checked separately. Native theme selection, editable/owner-drawn dropdowns, other
DPI configurations, screen readers and all-window contrast acceptance remain open.

Existing inventory rows and other controls are outside this dropdown correction.
The subsequent [cell-background correction](NativeDarkMode-RowCells.md) addresses
the stale subitem colors seen in that replay.
The external host performs an unelevated inventory scan; no removal or registry
editing action is invoked. Production elevation and helper packaging remain gates.
