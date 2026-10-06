# WIP native dark mode: continuation guide

This is an unfinished, opt-in implementation for review and continuation. It grew
out of investigating the dark-mode problem discussed in
[PR #962](https://github.com/BCUninstaller/Bulk-Crap-Uninstaller/pull/962).
It does not implement that PR's broader core refactor or remove COM dependencies.

The branch is synchronized with upstream commit `30da609`, which includes the
v6.3 release tag (`2e53fc1`) and the following README ordering update.

## Start here

Read [build instructions and architecture](NativeDarkMode.md), then the
[regression checklist](NativeDarkMode-Checklist.md). The solution and supporting
projects target .NET 10 and use the repository's normal build/output path. Building
the complete solution requires Visual Studio 2026 full MSBuild with desktop/C++
tools and the .NET 10 SDK because three projects retain COM references. Launch
`bin/Release/AnyCPU/BCUninstaller.exe` normally, then enable **Use dark mode
(restart required)** under **Settings > Interface**. The persisted option defaults
to off. Accept the restart prompt to apply either direction.

The executable retains its real elevation and uninstall behavior. The external
development host used for inspection disconnected execution actions; those
protections are not part of the application or this PR.

## Implemented scope

- Persisted .NET 10 dark-mode opt-in under Settings > Interface, with scoped
  adapters for inspected main-window, Settings, Properties, wizard, progress and
  leftover-review controls.
- List headers/group captions, status rows, selection/link colors, monochrome
  icons, search fields/dropdowns, legend/treemap and progress lifecycle handling.
- Live high-contrast entry/recovery fixes, including main-window buttons and
  primary list checkboxes.

`source/BulkCrapUninstaller/Theming` contains the adapters. Shared-library changes
are also present in KlocTools, ObjectListView and SimpleTreeMap. The patch includes
some related layout/refresh fixes documented in the individual follow-up notes.
Tooltip rendering is now explicitly set through `ThemeManager` for ObjectListView
tooltip event handlers and the two known `ToolTip` owners in Properties/Settings
and the uninstall wizard.

## Latest validation and its limits

The current setting/restart revision was built by
[fork CI run 36248632055](https://github.com/ObnubiladO/Bulk-Crap-Uninstaller/actions/runs/36248632055)
at `0d72482`; its application code matches PR commit `867e039` and differs only in
the temporary workflow branch trigger. Both the solution build/test job and native
launcher job passed. The published portable artifact was then exercised through
the exact `BCUninstaller.exe` app host at normal integrity so the UI automation
helper could drive it. Starting in light mode, the Interface checkbox was enabled,
the restart prompt accepted, and a new process opened with the dark interface. The
reopened setting remained checked and the portable XML contained
`WindowUseDarkMode=True`. The option was then cleared; the next restart opened a
new light process, the reopened checkbox remained clear, and the XML contained
`WindowUseDarkMode=False`. This cycle also verified that settings finish saving and
the single-instance mutex is released before the replacement process starts.

The light installer now consumes `publish-AnyCPU-net10.0` and registers a checked
.NET Desktop Runtime 10.0.12 dependency instead of the old .NET 8 runtime. The
x86/x64 installers were verified against Microsoft's release-metadata SHA-512
values, and their SHA-256 values are enforced by the existing Inno download helper.
Inno Setup 6.4.3 compiled a clean installer from the untouched fork-CI artifact.
A clean-machine runtime download/install and installer-upgrade replay remain part
of the broader packaging gate.

### 2026-10-06 file deletion and scan progress repair

The reported light `Delete` window is UniversalUninstaller, a separate process.
BCU now passes its active dark theme only when launching that exact bundled helper
for a SimpleDelete entry. The helper initializes WinForms color mode before
creating its window, adapts its list/header/tooltips and reuses BCU's owned-image
and contrast adapters. No theme switch is persisted in uninstall commands; the
main application's Settings option remains the user-facing control. High contrast
suppresses the dark request.

The empty file tree in the same screenshot was a .NET compatibility defect:
ObjectListView looked up the old `virtualListSize` private field. It now recognizes
the current `_virtualListSize` name and falls back to the supported property setter
if neither exists. This restores actual native row counts, expansion and checked
selection. The progress lifecycle adapter now lives in KlocTools and attaches to
both bars in every LoadingDialog, including modal directory scans. Attachment is
idempotent, so the existing main-inventory adapter call remains safe.

Fresh BCU and UniversalUninstaller builds passed using the local .NET 10.0.400
SDK. Local compilation substituted the previous clean CI package's
`Interop.Scripting.dll` for COM-reference generation, using an ignored targets
file; this is focused managed-build evidence, not a fresh complete solution or
installer build. Existing warnings remain. On Windows 11 with Desktop Runtime
10.0.12 at 200% DPI, visual inspection confirmed dark helper rows/icons/buttons,
blue scan progress after style changes, and the normal light helper/progress
appearance. Both launches through the real `RunUninstaller` path were canceled
without deleting fixture files. The controlled app-host launch used
`RunAsInvoker`; elevated interaction and other display/Windows coverage remain open.

The included [focused regression checker](../validation/NativeDarkHelperCheck/README.md)
passed 16 checks in dark mode and 14 in light mode, including the optional helper
launch/cancellation. It checks native virtual row counts and recreation, checked
selection, and each modal progress bar's determinate/marquee/determinate cycle.
It uses disposable files and never invokes deletion. Local build/check logs and
the patched portable package are ignored under `artifacts/screenshot-fix`.

The subsequent [full fork CI run 37516481604](https://github.com/ObnubiladO/Bulk-Crap-Uninstaller/actions/runs/37516481604)
rebuilt/published the solution, passed the native launcher build, and passed the
included checker (15 dark and 13 light checks). Its commit `533c23c` differs from
application/helper source `b4805b4` only in the temporary validation workflow.
The normal Test step returned success but produced no TRX, so no full-suite count
is claimed. The clean CI portable package is published as an
[unofficial testing prerelease](https://github.com/ObnubiladO/Bulk-Crap-Uninstaller/releases/tag/native-dark-test-2026-10-06-b4805b4),
with `TESTING.txt` and a SHA-256 checksum. It supersedes the earlier CI download in
PR #1000. No installer was rebuilt for this publication.

The 2026-09-26 [v6.3 fork-validation pass](NativeDarkMode-ForkValidation.md)
interactively exercised the current portable package through its production
managed entry point on Windows 11 build 26100.9457 and .NET Desktop Runtime
10.0.12. A 503-entry dark inventory covered grouped/ungrouped rendering,
collapse/expand, sorting, search, checkbox keyboard input, filtered reload, all
Properties and Settings pages, clean
close/relaunch and valid portable settings XML. A no-switch launch returned to a
ready light inventory. The exact executable remains the authority for elevation
and single-instance behavior; a second managed-host process stayed headless in
this older pass. The later setting cycle covers exact app-host restart at normal
integrity, while a combined same-integrity elevated restart/culture campaign remains
open.
The source-side [remaining surface audit](NativeDarkMode-SurfaceAudit.md) found no
additional native-message or embedded-HTML implementation beyond the existing
message facade, the intentional light-mode fallback and `FeedbackWindow`. It also
identified the standard dialogs that still need an interactive dark/contrast
review. The current command-line environment exposes only the .NET 8.0.425 SDK,
so the documentation-only updates in this pass did not receive another local
.NET 10 compilation; the already built v6.3 package supplied the runtime evidence.

After the v6.3 synchronization on 2026-09-08, the .NET 10 application project
restored and compiled locally with the existing warnings. The updated test project
compiled and ran 56 tests: 52 passed, one was skipped, and three rating tests could
not reach `bugsklocman.ddns.net:7721` because the local application firewall blocks
that endpoint. Upstream's v6.3 CI passes the same suite. The v6.3 native launcher
also built successfully with the local MFC-equipped Visual Studio installation.
[Fork CI run 34207844097](https://github.com/ObnubiladO/Bulk-Crap-Uninstaller/actions/runs/34207844097)
then restored, built and published the complete .NET 10 solution with Visual Studio
2026, passed the full test suite, and built the MFC launcher on Windows 2022. Its
temporary source commit differs from `a4a99a7` only by enabling the workflow on the
validation branch. The equivalent PR run still requires maintainer approval.

Before the solution-wide .NET 10 migration, the focused repair pass completed fresh
.NET 8 and .NET 10 builds. Fresh external-checker runs recorded 631
assertions: .NET 10 ordinary default/dark/light override (158/160/158) and the
.NET 8 dark bypass (155). A focused STA WinForms host recorded another 155
assertions across .NET 10 dark/light-override and .NET 8 bypass runs (79/38/38).
It checked native dialog names, roles, checked state and initial focus; both known
`ToolTip` owners; ObjectListView cell/header tooltip colors; dialog mapping and
owner-bound worker-thread marshalling; and current/legacy Feedback HTML fixtures.
The dark Feedback fixture and a native tooltip window were rendered to bitmaps and
checked for dark backgrounds and contrasting text colors. These are local
observations, not CI results. An earlier eight-run contrast matrix recorded 1,313
assertions, including button and checkbox replays, but was not repeated in this
repair pass.

The environment was Windows 11 build 26100 at 200% DPI, with Desktop runtimes
10.0.11 and 8.0.30. The visual host loaded the built application assemblies and
exercised actual form classes, but did not run the production entry point.
A subsequent read-only replay populated all four Properties pages from one
installed entry in dark, light and both controlled contrast palettes; no
production defect was found.
A second replay covered nested/context menus and the actual main/progress toolbar
overflow in the same palettes. No split/drop-down button controls exist in source,
and no production defect was found in the applicable surfaces.
The light-contrast test applied the installed hcwhite.theme color table while
high contrast was active; it was not native Settings selection of that theme.

A separate network-dependent dark smoke passed 85 assertions. Because the local
application firewall intentionally blocks `NativeDarkRepairCheck.exe`, the same
compiled harness DLL was launched through the allowed .NET host. The current page
loaded, its `wsite-content` DOM was adapted, its computed background matched the
dark system color, and the rendered content remained readable. The blocked apphost
run produced a themed navigation-error document as expected; that result was not
treated as evidence about MSHTML or the remote page.

A bounded production acceptance pass staged hash-identical copies of the fresh
outputs in disposable portable directories. The exact .NET 10 executable followed
its manifest/UAC elevation path and reached a responsive dark inventory, closing
the production-launch check. Exact .NET 10 runs with no switch and with
`--dark-mode --light-mode`, plus the historical .NET 8 executable, reached ready light
inventories. The two .NET 10 light captures were byte-identical. A second exact dark
executable exited with code 0 after 3.2 seconds while the first remained responsive
and the only BCU process.

Windows integrity isolation prevented interactive automation of those elevated
windows. The remaining UI campaign ran the same managed entry point through the
medium-integrity .NET host. It covered a real 260-entry inventory, grouping, refresh,
multiple-app Properties, all seven Settings pages, live dialogs, the uninstall
wizard through its final summary, keyboard navigation, clipboard commands and
native Save dialogs. The wizard was canceled explicitly on page 5 of 5 before any
worker, cleanup or uninstall action started. Its related and running-app pages
auto-skipped; returning Back also reset the 7-Zip exclusion before it was reapplied.
Those limits leave the detailed production-executable rows open.

The managed entry point saved `es-AR`, closed normally, and relaunched into a ready
Spanish dark inventory. The English and Spanish restart confirmations exposed
localized headings, details and named Yes/No buttons without visible truncation.
English was restored and the app closed normally again; the portable settings file
parsed successfully afterward. This is useful culture and close evidence, but it
does not substitute for an elevated exact-executable `Application.Restart` cycle.

Two checked applications produced byte-identical XML exports before and after a
filter hid one of them. Both exports and the low-sensitivity clipboard command kept
both names, and a Properties value was written through the native Save dialog. The
filtered status count used the visible checked count while selected size and export
used the complete checked set, exposing a pre-existing display mismatch. Local
ignored evidence is under `artifacts/production-acceptance-20260905-025500`.

The external prototype harnesses, raw logs, inventory screenshots and local
`artifacts/` directories mentioned in the follow-up documents are not included
in this PR. Those references describe historical evidence, not files available
in this checkout. The focused repair checker is also a local ignored artifact.
The build helper and implementation are included; those historical assertion counts
cannot currently be reproduced from this checkout alone.
The new file-deletion/loading checker above is included, but does not reproduce
those broader historical campaigns.

Existing build warnings NU1510 and SYSLIB0057 remain unresolved. The documented
all-bad-confidence leftover-filtering issue is separate and unchanged.

## Bounded next investigations

1. Tooltip rendering and contrast, followed by native dialogs and HTML surfaces.
   - Completed in this handoff: ObjectListView tooltip events and both known BCU
     `ToolTip` components use theme system colors. Inspected `OK`/`OKCancel`
     `MessageBox` flows use the themed dialog in dark mode; unsupported layouts and
     unowned worker-thread calls retain the native implementation. Owner-bound calls
     marshal to the UI thread. `FeedbackWindow` handles both the current Weebly DOM
     and legacy IDs, using literal RGB foreground/background CSS in dark mode. The
     live current page loaded and rendered through a host permitted by the local
     application firewall.
2. Keyboard-only navigation, visible focus and screen-reader metadata on those
   repaired surfaces.
   - Programmatically checked in this handoff: the standard WinForms accessibility
     providers expose localized control names, roles and checkbox checked state, and
     custom dialogs focus the first visible enabled action. The Feedback browser is
     keyboard reachable and receives focus after loading. A real Narrator or other
     screen-reader announcement pass remains open.

3. Broader main-window keyboard and screen-reader checks.
   - Completed in this handoff: an all-keyboard managed-entry-point pass covered
     search focus/clearing, list movement and checking, the row context menu,
     Tab/Shift+Tab and Properties. Observable behavior passed, although UI Automation
     reported stale search focus after some transfers. A real screen-reader
     announcement pass remains open.

4. Next target: repeat the UI campaign from a same-integrity elevated production
   executable, including culture handling, then cover the wizard's related/running-
   process branches and direct Settings opening from Progress.

After these investigations, consolidate findings before extending individual
adapters. The release checklist still contains 21 open checks, many overlapping
validation areas rather than known implementation defects: real startup and
elevation, packaging/helpers, Windows and DPI coverage, localization/RTL, resource
stability, and disposable uninstall/backup/cleanup workflows.

The persisted opt-in preference is implemented. Automatic system-theme following
and default-on rollout remain design decisions. This draft is not ready to merge
as a release feature.
