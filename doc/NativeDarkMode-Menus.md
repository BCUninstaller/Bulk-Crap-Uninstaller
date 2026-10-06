# Nested menus and toolbar overflow

The 2026-09-05 follow-up loaded the compiled application DLL in the external
asInvoker visual host and opened the actual MainWindow plus the actual
UninstallProgressWindow with its inert, never-started task fixture. It selected
and checked one main-list row only to enable menu state. The replay opened and
selected menu items without invoking click handlers or any uninstall, process,
registry, clipboard, file or web action.

The host narrowed the main split panel until eight toolbar items moved into the
native `ToolStripOverflow`. It constrained the progress toolbar until all ten
production items moved into its overflow. Overflowed items retained their original
`ToolStrip` owner and reported `ToolStripOverflow` as their current parent. View,
nested operations, the application context menu and both overflow menus rendered
with their actual enabled/disabled states, shortcuts, arrows and icons.

Three complete sequences covered ordinary dark, configured High Contrast Black
and a controlled light contrast color table. In each palette the selected main
and progress overflow icons matched the expected current foreground exactly; all
layout, hosting and image checks passed. The ordinary light comparison also passed.
Both bounded contrast cycles restored flags, scheme, all 30 system colors and
CurrentTheme with identical metadata. No production rendering change was needed.

Source inventory found no `ToolStripSplitButton` or `ToolStripDropDownButton`
controls in BCU or its in-repository libraries. The applicable production surfaces
are `ToolStripMenuItem` drop-down arrows and dynamically created overflow chevrons,
both covered by this replay.

The host build had zero warnings and errors. Its copied application DLL matched
the published build at SHA-256
`A9107D6386DE3050EC1EBDDC600A37CA052AEE14E2EC8CAA6776C2718FA4788E`.
Raw logs, screenshots and replay source remain in the external development
workspace and are not included in this PR.

This closes the scoped nested-menu and overflow investigation on Windows 11 build
26100 at 200% DPI. Keyboard-only menu traversal, tooltips, native dialogs, other
Windows/DPI configurations and native Settings selection of additional contrast
themes remain separate release checks.

