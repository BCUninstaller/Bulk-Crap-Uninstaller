# Experimental native dark-mode integration

This patch consolidates the Windows Forms adapters investigated against BCU's real
main window, Properties, Settings, uninstall wizard, progress and leftover review.
Dark mode remains opt-in through the Interface settings, while the solution now
targets .NET 10.

For the current status, evidence limitations and next investigations, see the
[WIP continuation guide](NativeDarkMode-Handoff.md).

## Build and enable

The shared build configuration targets the application and supporting projects at
`.NET 10`. Because three projects retain COM references, building the complete
solution requires Visual Studio 2026 full MSBuild with the desktop/C++ tools and
the .NET 10 SDK. Use the repository's normal build path:

```powershell
msbuild source/BulkCrapUninstaller.sln /restore /t:Build /p:Configuration=Release /p:Platform="Any CPU"
```

Launch the executable normally, open **Settings > Interface**, and select
**Use dark mode (restart required)**. The setting is disabled by default, is saved
in BCU's existing portable settings file, and takes effect after accepting the
restart prompt. Clearing it and restarting returns to the normal light interface.
High contrast suppresses dark mode even when the preference is enabled. There is
no live light/dark switching or automatic system-theme following in this patch.
Scoped monochrome icons follow high-contrast colors, including changes while open.

The existing manifest, elevation requirements, single-instance mutex, application
settings, startup checks, scanning and uninstall/cleanup behavior remain intact.
This executable is the real application, not the safe fixture host. Do not treat
the earlier inspection harness's disconnected actions as properties of this build.

## Code organization

`Theming/ThemeManager.cs` is the entry point. Initialization runs once after the
entry point prepares application settings, text rendering and visual styles, before
dependency setup can display application windows. It reads the persisted preference,
sets the launch mode, optional shared-dialog
heading color and the two existing application-row palettes. High contrast prevents
initial opt-in; dark control adapters also check high contrast before applying.
The separate icon bindings remain active in high contrast, including classic mode.

Small constructor hooks opt individual, inspected surfaces into:

- Exposed ObjectListView header, selection, background and hyperlink styles.
- Known BCU monochrome menu/instruction icons, copied without mutating shared assets.
- Explicit Settings/wizard page backgrounds and Properties grid header/selection colors.
- The shared loading/progress handle-lifecycle workaround.
- BCU's existing custom message box for inspected `OK`/`OKCancel` message flows in
  dark mode.

`NativeObjectListView` remains an ObjectListView subclass. It uses
system contrast colors for selected rows and hyperlink cells. On .NET 10, adapted
ordinary Details lists also opt into implicit theming,
repaint simple group-caption label rectangles after native drawing, and fill the
disabled background before upstream owner-drawn rows. It does not skip native group
drawing. Virtual lists, RTL and rich group layouts are outside the workaround.

`ProgressBarLifecycleAdapter` defers the same theme opt-out used in the tested
.NET 10 control until handle initialization completes, repeating it after recreation.
It ignores disposed/stale handles. It retains native progress values and painting.

The central service avoids repeated list/menu/progress registration with weak keys.
Image bindings retain shared source bitmaps and own only their recolored copies.
System-color notifications refresh them on the UI thread; disposal detaches the
notifications and releases copies. Colored artwork is rejected as a whole, and
installed-program icons are not passed to the monochrome helper. See the
[contrast icon follow-up](NativeDarkMode-Icons.md) for scope and verification.
The subsequent [selection/link follow-up](NativeDarkMode-ContrastStates.md) covers
highlighted menu icons, paired selection colors and live hyperlink-color recovery.
The [row/grid follow-up](NativeDarkMode-RowGrid.md) suppresses main-list status tints
in high contrast, restores them on recovery, and pairs Properties selection colors.
The [legend/treemap follow-up](NativeDarkMode-LegendTreemap.md) explains that contrast
behavior in the legend and paints neutral, outlined treemap tiles with system
selection colors while retaining the original geometry and category palette.

Small support changes include optional heading colors and first-action focus for
the shared CustomMessageBox (null retains its old heading behavior), direct theme
registration for the two known BCU `ToolTip` components, and a serialization
annotation on the existing runtime-only CustomNoteDialog property required by the
.NET 10 analyzer. Standard WinForms accessibility providers retain responsibility
for localized dialog names, roles and checkbox state. The Settings cache explanation
wraps to its available width in adapted mode.

`FeedbackWindow` now navigates directly to the current HTTPS contact page. Its
document adapter supports both the current Weebly `wsite-content` structure and
the legacy `container`/`content` IDs. In dark mode it applies literal RGB system
foreground/background colors so the MSHTML host does not resolve CSS system-color
keywords to a light palette. The browser enters the tab order and receives focus
after the top-level document finishes loading.

## Evidence and limitations

The source consolidation builds on isolated real-window probes on Windows 11 build
26100, DPI 192 (200%), .NET Desktop 10.0.11. Those probes exercised the main inventory
list/refresh, Properties, seven Settings pages, wizard selection/navigation, synthetic
progress states and leftover confidence review. Fixtures never validated actual
uninstall execution, process termination, registry backup or junk deletion.

Before the solution-wide .NET 10 migration, the integrated application was built in
default .NET 8 and opt-in .NET 10 forms. An external, unelevated checker loaded those
assemblies and checked default-off, dark opt-in, light override, repeated application,
shared image preservation, the real leftover-review constructor hook, and progress
recreation/pending-callback disposal. It did not run the production entry point or
modify its manifest.

After the dialog/tooltip/Feedback repair, fresh checker runs retained the expected
.NET 10 default/dark/light-override counts (158/160/158) and .NET 8 bypass count
(155). A separate local STA WinForms checker passed 79 dark assertions and 38 each
for light override and .NET 8 bypass. It exercised real controls and native
accessibility providers, including dialog focus and checkbox state, both known
`ToolTip` owners, ObjectListView tooltip event colors, owner-bound worker-thread
dialog marshalling, both Feedback DOM shapes, and rendered native-tooltip and dark
Feedback bitmaps. These local checker sources are ignored artifacts and are not
part of the patch. They do not replace a real screen-reader announcement pass.

A network-dependent dark smoke passed 85 assertions. The local application
firewall intentionally blocks the generated `NativeDarkRepairCheck.exe`, so the
same compiled harness DLL was run through the allowed .NET host. The current page
then loaded in MSHTML, its `wsite-content` DOM was adapted, its computed background
matched the dark system color, and a rendered bitmap showed readable content. The
blocked apphost run's themed navigation-error page was retained only as secondary
error-surface evidence.

An external visual host subsequently replayed the compiled integration DLL's main
window, Properties, seven Settings pages, wizard, progress and leftover review.
It verified the copied assembly's SHA-256 against the published DLL. No new theme
regression was observed in this scoped Windows 11 / 200% pass. See the
[compiled-DLL visual report](NativeDarkMode-Visual.md) for exact coverage and host
limitations; this does not validate the production entry point or elevation path.

Remaining release work is tracked in [the regression checklist](NativeDarkMode-Checklist.md).
The [integrated main-window replay](NativeDarkMode-MainReplay.md) fixes stale
treemap tiles after an empty search and legend overlap with scaled scrollbars,
and stabilizes neutral tile colors during contrast-mode filtering/reload.
The [search dropdown follow-up](NativeDarkMode-Dropdowns.md) refreshes native theme
associations and cached background colors for the two filter dropdowns during live
contrast changes, with ordinary dark-mode recovery.
The [application-cell follow-up](NativeDarkMode-RowCells.md) refreshes the background
copies created by hyperlink formatting, so entire status rows follow contrast
changes and recover together.
The [search-field follow-up](NativeDarkMode-Search.md) clears its stale native
background brush during palette changes, retaining system colors and search state.
The [main-button follow-up](NativeDarkMode-Buttons.md) uses built-in managed drawing
for standard buttons/sidebar checkboxes in high contrast and restores their
ordinary style afterward.
The [primary list-checkbox follow-up](NativeDarkMode-ListCheckboxes.md) uses current
contrast colors while preserving glyph geometry and ordinary renderer behavior.
The [Properties data follow-up](NativeDarkMode-Properties.md) exercises all four
pages from one installed entry across dark, light and controlled contrast palettes.
The [menu/overflow follow-up](NativeDarkMode-Menus.md) exercises actual nested and
context menus plus the main and progress toolbar overflow paths.
A subsequent [DPI/high-contrast investigation](NativeDarkMode-Platform.md) confirmed
startup/transition theme guards, but found fixed-column truncation in both light
and dark modes and unreadable black icons in the dark high-contrast fallback.
The column-width issue is now addressed by the shared fix above. The icon follow-up
also fixes the observed black-on-dark failure and verifies live recovery; physical
monitor moves and the full native contrast-scheme matrix remain open.
A full production-executable matrix and an authorized complete-inventory/uninstall
test are still required. The .NET 10 build currently
reports NU1510 for the existing Microsoft.VisualBasic package reference and SYSLIB0057
for the existing certificate constructor. Neither warning is hidden by this patch.

Native borders and disabled text are not fully adapted. The two known BCU `ToolTip`
components and inspected ObjectListView tooltips are adapted; other tooltip owners
have not been established. Inspected `OK`/`OKCancel` MessageBox callers use the
themed dialog helper when dark mode and a safe UI context are available. Unsupported
button layouts and unowned worker-thread calls retain native behavior; one
leftover-details call path in `ThemeManager` remains intentionally light. The
separate general TabControl descendant-theming issue has no global workaround here;
the real Settings dropdowns worked in the inspected flow.

The existing all-bad-confidence leftover list issue is not changed: its constructor
checks and disables Hide bad confidence, leaving every finding filtered out. It
reproduces in the original light flow and should receive a separate behavioral fix.

No prototype bootstrap, synthetic fixtures, source-copy substitutions, telemetry
overrides, scan exclusions, hard-coded language, action interceptors or local error
logging are included in this application patch.
