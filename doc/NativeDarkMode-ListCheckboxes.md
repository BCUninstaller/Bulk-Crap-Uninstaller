# List checkboxes in high contrast

ObjectListView's primary checkbox paint path calls CheckBoxRenderer directly.
Its StateImageList is used for printing or explicitly enabled custom images, so
regenerating that image list would not fix the ordinary owner-drawn main list.
The controlled light-contrast replay exposed dark themed checkbox fills here.

NativeObjectListView now starts with ContrastListRenderer, a HighlightTextRenderer
subclass. Only its primary checkbox painting changes, only in Details view and
high contrast. It reads Window/WindowText, GrayText for disabled lists or rows,
and HotTrack for a hovered border on every paint. Check marks use WinForms'
recolorable DrawMenuGlyph primitive; mixed state uses an inset square. The outline
scales with graphics DPI. Glyph size, alignment and returned text offset remain
ObjectListView's, preserving checkbox hit testing and text layout.

The renderer keeps filter highlighting and leaves custom column/default renderers
in control. Normal light/dark drawing, printing and custom checkbox images use the
upstream path. This accessibility fallback works in both runtime builds without a
dark-mode opt-in. No handles, colored image caches, input handlers or check-state
callbacks are introduced. Other checkbox columns use their existing renderers.

The external checker compares normal painting against upstream pixel for pixel,
checks all three states at 96/192 graphics DPI, contrast palette pixels, painting
bounds, disabled rows/lists, hover, custom/printing fallbacks and state preservation.
The native replay uses the actual main window at 192 DPI. See the external
prototype's `artifacts/list-checkbox-contrast/REPORT.md` for final runs and evidence.
The light test applies the installed hcwhite.theme color table with high contrast
active; native Settings theme selection and the wider release matrix remain open.

The uninstall confirmation and related-application controls also opt their three
checkbox columns into ContrastCheckStateRenderer. That subclass preserves the
separate CheckStateRenderer layout, hit testing, printing and aspect putter path,
but uses the same current-color painter during high contrast. Other checkbox
columns are not changed automatically.

Shared ObjectListView code is unchanged by this correction. Screen-reader
announcements and other Windows/DPI configurations still need the
[release checklist](NativeDarkMode-Checklist.md).
