# Integrated main-window filtering and contrast replay

The 2026-09-04 replay used the actual compiled MainWindow, its ordinary unelevated
inventory scanner, filter editor, list configurator, treemap and borderless legend.
The external host enabled the legend/treemap in isolated portable settings and
disabled certificate testing, updates, ratings and statistics. It invoked no
uninstall, registry editing, backup or other execution operation.

## Findings and changes

- Searching from 103 matching rows to zero left 57 old treemap tiles behind.
  UninstallerListConfigurator used AfterSorting as its filtering completion signal,
  but ObjectListView skips sorting when empty. The application-specific list now
  exposes ListRebuilt after BuildList completes. The configurator uses that signal,
  including empty results and after selection restoration. TreeMap selection is
  synchronized from the completed list whenever its contents are refreshed.
- The legend's lower/right edges overlapped the list scrollbars by four physical
  pixels at 200% DPI. Positioning used the outer control dimensions. It now uses
  ClientSize, with the existing 30-pixel margin, and follows client-size changes
  when filtering adds/removes scrollbars. Origins clamp to zero in cramped lists.
- Code inspection found that the dark neutral tile getter returned the live
  SystemColors.Window value while treemap brushes cache RGB fills. Filtering or
  scanning during contrast could therefore cache a temporary palette in a brush.
  The dark neutral color is now captured as an ordinary RGB value at setup;
  TreeMap's paint-time contrast override continues to use live system colors.

The upstream ObjectListView sorting contract is unchanged. ListRebuilt belongs to
the application's NativeObjectListView subclass.

## Validation scope

The external checker adds five checks: notification after selection restoration,
empty-result treemap clearing, clearing the filter, scrollbar exclusion and the
legend's margin. Existing control, palette and brush-lifetime checks remain in use.

The main replay records eight checks at each settled checkpoint: list/map objects,
selection, legend anchor, legend containment, contrast opacity, cached category
colors, actual painted tile centers and neutral contrast row backgrounds. It runs
selection, a matching search, a zero-result search, filter clearing, move/resize,
bounds restoration and the real inventory reload after each settled palette.
The light contrast palette uses the installed hcwhite.theme color table while
contrast is enabled; it is not a native Settings selection of that named theme.

Exact run counts, DLL identities, restoration records, screenshots and observed
limitations are recorded in the workspace artifact
`prototype/artifacts/main-contrast/REPORT.md`. This remains an external asInvoker
host replay on Windows 11 at 200% DPI, not the production elevation/startup path.

## Remaining work

Keyboard-only focus/navigation through the main window remains open. A later
[Properties replay](NativeDarkMode-Properties.md) populated all four pages from an
installed entry without finding a new rendering defect. The light contrast native
replay also showed blank selected text in the two search-filter dropdowns; that
separate standard-control rendering
issue is the next useful investigation. Native selection of additional contrast
themes, physical mixed-DPI moves, other supported Windows versions, production
packaging and disposable-target removal workflows remain release gates.

The subsequent [dropdown correction](NativeDarkMode-Dropdowns.md) addresses the
selected-text/popup issue in the scoped .NET 10 filter controls. This report retains
the original main-replay evidence and measurements.
