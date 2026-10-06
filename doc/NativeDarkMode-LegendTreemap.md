# Legend and treemap contrast follow-up

The 2026-09-04 follow-up makes these two surfaces agree with the neutral application
rows used in high contrast. Previously the legend retained its RGB swatches and
hover fade, and the treemap painted cached category colors with a fixed blue
selection, regardless of the system palette.

## Behavior

In high contrast the legend becomes a Status guide with neutral, readable category
labels. A wrapped notice explains that status colors are disabled, directs users
to list columns for status details, and says tile sizes still show application
sizes. Category visibility and the user's show-legend setting are retained. The
floating legend stays opaque, including during its hover timer. When contrast ends,
the ordinary title, selected normal/colorblind swatches and fade behavior return.
The main window repositions the legend when the extra notice changes its size.

The two new strings are neutral English resources; untranslated cultures fall back
to them. Existing localized titles and category labels remain in use. Additional
translation review is still needed.

The shared TreeMap control now paints high-contrast tiles with Window, boundaries
with WindowText, selection with Highlight and a HighlightText outline. Explicit
outlines keep adjacent neutral tiles distinguishable at the observed 200% scale.
The palette is resolved at paint time. Original element colors, tile grouping,
geometry, selection and hover objects remain intact; no Populate call is needed
for a contrast transition. Category fills return on recovery. Both runtime builds
receive this behavior. Ordinary colors and value/log-scaling calculations retain
their existing meaning.

Painting uses shared system/standard brushes instead of allocating an undisposed
background brush. Population disposes the previous category brush cache, and control
disposal clears the final cache. No system-owned brushes are disposed.

## Verification and scope

Both application builds succeed with the existing NU1510/SYSLIB0057 warnings. The
external host and checkers build without warnings/errors. The final checker matrix
passes 70/72/70/70 assertions in ordinary .NET 10 default/dark/light override and
.NET 8 dark-bypass cases (282 total). Both runtimes also pass 72 assertions under
each of the two contrast palettes (288 additional; 570 across eight final runs).

New checks cover notice visibility, retained category visibility, both swatch
palettes, opacity behavior, unchanged tile geometry/selection, actual bitmap paint
colors and outlines, and disposal of replaced/final cached brushes. They exercise
the actual compiled controls rather than a copy of the rendering implementation.

Native Windows 11 build 26100 / 200% observations verify the wrapped guide, readable
neutral tiles and highlighted selection under configured High Contrast Black and
a controlled light system-color table. The latter temporarily applies the installed
hcwhite.theme colors with contrast active; it is not named-theme selection through
Windows Settings. All contrast helpers restore saved system colors and theme
metadata. Original category fills return in the already-open process.

The external review fixture uses real ListLegendWindow and TreeMap classes with
inert applications. A review-only frame makes the borderless legend targetable by
the native inspection tool; the production window remains borderless. Its original
transparency key can leave a narrow transparent margin inside that added frame.
The real content and opacity handlers are unchanged by the fixture. Native main-
window docking/repositioning and the complete refresh/filter flow remain to be
replayed together. Keyboard/screen-reader access to the treemap, tooltip coverage,
long translations, other DPI/display configurations and production startup remain
open in the [checklist](NativeDarkMode-Checklist.md).

Reviewed SHA-256 values:

- BCU: `99E196057FD262FC1F5ED7EF9CC7ED91C3E94F101F5742DC6236E626B50D95C6`
- ObjectListView: `F91F81E9C02B4A871574E919D18668CD049A7C7521E36B22C8BACEBB3A0D4377`
- SimpleTreeMap: `FECC17EAE335B7AFA96DFEACB57070D7248732A3340360E7706C12F08667EED2`

Screenshots and pinned logs are listed in the external prototype's
`artifacts/legend-treemap/REPORT.md`. No inventory scan or removal workflow ran.

The subsequent [integrated main replay](NativeDarkMode-MainReplay.md) exercises the
production borderless legend together with filtering and inventory reload, fixes
scrollbar overlap/empty-result synchronization, and records its newer DLL identity
separately. The measurements above remain the original isolated-control evidence.
