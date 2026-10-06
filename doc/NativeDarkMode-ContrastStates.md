# Contrast selection and link colors

Follow-up: [application rows and Properties selection](NativeDarkMode-RowGrid.md)
now addresses those two remaining color cases. This report retains its original
scope, measurements and assembly identity.

On 2026-09-04, a small external fixture using the compiled BCU control, theme
manager and real BCU menu assets reproduced three remaining contrast failures:

- Normal and visited links retained pale dark-mode RGB colors on a light surface.
- Selected rows used WindowText instead of the foreground paired with Highlight.
- Selected menu items and checked toolbar buttons used the normal icon foreground
  even when Windows painted their background with Highlight.

## Changes

In high contrast, `NativeObjectListView` pairs selected foreground/background with
HighlightText/Highlight, and unfocused selection with ControlText/Control. The
original property values remain available when high contrast ends. The changes
apply in both runtime builds, including classic startup.

Normal, visited and hovered hyperlink cells use SystemColors.HotTrack in high
contrast. Existing cells refresh on system-color notifications without rebuilding
rows, invoking formatting callbacks or allocating new fonts. Leaving high contrast
restores normal/visited/hover colors from the existing HyperlinkStyle. Underline and
link behavior remain intact. Virtual-list cached-row refresh is outside this scope.

`MenuImageRefresh` listens to the renderer's background events and refreshes owned
icon copies before image painting. It handles pointer/keyboard selection and
checked buttons without replacing the native renderer. Only items owned by its
registered strip are processed. Renderer replacement and strip disposal detach
subscriptions, including when a renderer is shared. In high contrast, highlighted
items use HighlightText; other icons retain ControlText. Colored artwork continues
to be excluded by the existing monochrome classifier.

This follows the inspected built-in renderer's highlighted backgrounds. The
[WinForms high-contrast renderer](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Controls/ToolStrips/ToolStripHighContrastRenderer.cs)
does not remap full-color image formats itself; the BCU resource copies are 32-bit.
Custom third-party renderers with different background semantics are not covered.

## Verification

Both application runtime builds succeeded. The .NET 10/default, dark, light
override and .NET 8/dark-bypass checker cases pass 38/40/38/38 assertions (154 total).
Fifteen checks added per case cover checked/selected icon colors, resource ownership,
shared-renderer isolation, renderer replacement/disposal, normal/visited/hovered
links, cached-cell refresh and preserved row identity/selection.

The checker also passed 38 assertions on each runtime while each of two contrast
palettes was active (152 additional assertions). Native visual observations used
Windows 11 build 26100, 200% DPI, Desktop 10.0.11:

| Palette | Observation |
| --- | --- |
| Configured High Contrast Black | Cyan links on dark background; dark selected text/icons on cyan highlight. |
| Controlled light system-color table | Dark teal links on cream background; cream selected text/icons on brown highlight. |
| Recovery in the already-dark process | Original pale blue/purple links and ordinary dark selection/icon appearance return. |

Checked toolbar buttons, selected rows and keyboard-selected dropdown icons were
inspected in both palettes. Link hover coloring/underline was checked through the
compiled control's style path, not a native pointer-hover screenshot.

The light pass enabled high contrast, then temporarily applied the installed
`hcwhite.theme` color table through the documented session-only
[SetSysColors API](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setsyscolors).
It does not claim that Windows selected the named light theme through its Settings
UI. Helpers saved/restored all 30 supported color entries, contrast flags, configured
scheme and theme path. Every completed cycle reported appearance and metadata
restored. One short cycle ended before the menu screenshot/checker ran; that output
is retained as restoration evidence and excluded from contrast acceptance.

Reviewed BCU SHA-256:
`6A8BAB89BCA28F8F09FEA7CCBE71087A72CC818D9B730F3004880B44C1031EDD`.
ObjectListView SHA-256:
`F91F81E9C02B4A871574E919D18668CD049A7C7521E36B22C8BACEBB3A0D4377`.
Both checkers and the visual host load copies matching their actual built DLLs.

Screenshots and pinned logs are listed in the external prototype's
`artifacts/contrast-states/REPORT.md`. The fixture has no execution actions and
blocks hyperlink navigation. The host and checker builds had zero warnings/errors;
existing application NU1510/SYSLIB0057 warnings remain. The historical host error log
did not grow. No production manifest or startup workflow was altered.

The full production matrix remains open, particularly per-application RGB row
backgrounds, Properties grid selection, custom/third-party renderers, overflow and
split-button interactions, native Settings selection of other contrast themes,
keyboard-only navigation beyond these menu states, and screen-reader verification.

A later [menu/overflow replay](NativeDarkMode-Menus.md) covers the actual nested,
context and main/progress overflow surfaces. Source inventory found no application
split/drop-down button controls. Other custom renderers and native Settings theme
selection remain open.
