# Application rows and Properties selection in high contrast

Later correction: [copied cell backgrounds](NativeDarkMode-RowCells.md) covers
hyperlink rows in the main window, which this original fixture did not exercise.

Follow-up: [legend and treemap contrast behavior](NativeDarkMode-LegendTreemap.md)
now addresses their presentation alongside neutral rows. The measurements below
retain this earlier row/grid pass's scope and assembly identity.

The 2026-09-04 follow-up fixes two additional contrast gaps in both runtime builds.
Already-open application rows retained their RGB status tints after a system
palette change; the inherited foreground then became dark text on dark tinted
backgrounds in the controlled light contrast palette. The dark-adapted Properties
grid similarly retained WindowText on Highlight instead of HighlightText.

## Changes

`ApplicationListConstants.GetApplicationBackColor` returns SystemColors.Window in
high contrast. Status tints are temporarily suppressed in favor of the user's
contrast surface; integrity, certificate and application-type data are unchanged.
These existing columns remain the way to distinguish statuses without row color.

`ApplicationRowColors` replaces the main list's former FormatRow subscription and
also observes that list's SystemColorsChanged event. It updates existing row
backgrounds directly without rebuilding, filtering or rerunning other format
callbacks. Selection, checked objects and horizontal scroll survive recovery.
Registration is idempotent and uses no static system-event subscription or owned
graphics resources. The direct refresh is scoped to ordinary, nonvirtual lists.

Ordinary light/dark mode still uses the configured normal or colorblind status
palette. Color.Empty is assigned too, so a row whose highlighted status disappears
stops retaining its previous tint. The shared palette objects and treemap color
getter are not modified by this contrast override.

Properties uses CellFormatting to pair SelectionBackColor/SelectionForeColor with
Highlight/HighlightText only during high contrast. It changes the effective style
for that formatting operation, retaining the underlying ordinary style for
recovery and working after data-source replacement. The dark header style now
pairs Control with ControlText as well. Classic/.NET 8 startup registers the same
contrast formatting hook without enabling dark-mode control adapters.

## Verification

The earlier .NET 8 and .NET 10 comparison builds succeeded. Existing NU1510 and
SYSLIB0057 warnings remain. The external checker and visual host built without
warnings/errors, and copies matched their actual built application DLLs.

The ordinary checker matrix passed 59/61/59/59 assertions (238 total), respectively
.NET 10 default, dark, light override and .NET 8 dark bypass. Twenty-one added
checks per case cover all six status colors plus an ordinary row in both palette
settings, existing-row refresh, identity/selection/check preservation, absence of
extra format callbacks, stale-tint clearing and the actual Properties constructor
and formatting path across data-source replacement. Formatting can request a style
without colors, so the checker resolves Empty through the cell's inherited style.

Both runtimes also passed 59 assertions while each of two contrast palettes was
active (236 additional assertions). The external Windows 11 build 26100 / 200%
visual pass used the actual Properties form with an inert table and a native BCU
list with seven synthetic applications and the production color helper/adapter.
It does not constitute a full main-window inventory replay.

- Light contrast colors: cream rows with dark text; Properties uses cream selected
  text on brown. Verified and Unverified certificate labels were inspected.
- Configured High Contrast Black: dark rows with white text; Properties uses dark
  selected text on cyan.
- Recovery: original dark status tints return, including at the previous horizontal
  scroll position. No rescan or row rebuild was needed.

The light test temporarily applied the installed hcwhite.theme color table while
contrast was enabled. It is a controlled native system-color test, not a claim
that the named theme was selected through Windows Settings. All three helper
cycles restored all saved color entries, flags, configured scheme and theme path.
The first Properties screenshot was clipped by the fixture's inherited window
geometry; its pre-fix formatting log is the reliable reproduction. The host then
used explicit window sizes/positions for the post-fix visual checks.

BCU SHA-256: `934D73AEBC3ED32DDB97B36E680A7AD81C7ECB442CAC6BCBEA4B342E1002FE40`.
ObjectListView SHA-256: `F91F81E9C02B4A871574E919D18668CD049A7C7521E36B22C8BACEBB3A0D4377`.
Evidence is in the external prototype's `artifacts/row-grid-contrast/REPORT.md`.
The historical host error log did not grow. No scanner, uninstall, registry backup,
deletion, certificate verification or production startup/elevation flow ran.

Remaining review includes legend/treemap presentation when row tints are suppressed,
the complete main-window formatting/refresh flow, native Settings theme selection,
other Windows versions and the broader
[accessibility/release checklist](NativeDarkMode-Checklist.md).

A later [Properties replay](NativeDarkMode-Properties.md) populated all four pages
from one installed entry in dark, light and both controlled contrast palettes.
