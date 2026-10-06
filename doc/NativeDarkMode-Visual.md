# Compiled-DLL visual replay

On 2026-09-04, the integrated .NET 10 application DLL was loaded into a separate
unelevated Windows Forms review host. No new theme regression was observed in the
cases below. This establishes that the consolidated hooks reproduce the inspected
prototype behavior; it is not production-entry-point or release acceptance.

## Build identity and environment

- Base revision: `608321de98e92297377b1eb69029af55c25504a1`, with the uncommitted native-dark integration patch.
- Published `BCUninstaller.dll` SHA-256: `949F80D7B316FD59A45BCC829989CAFAA5A2700A216D833436BCCA85C801582D`.
- The host verifies that its DLL copy has the same hash as the published DLL and records the loaded hash at startup.
- Windows 11 build 26100; .NET Desktop 10.0.11; DPI 192 (200%); English; high contrast off.
- Review-host build: zero warnings and zero errors. Application build and assembly smoke-check evidence are recorded separately.

The host is external to this patch. It references the compiled application and
dependencies, with no copied application source or replacement theme adapters.
It initializes the theme manager and isolated portable settings, supplies the
dialog owner normally established by MainWindow, and opens the real form classes.
Its own manifest is asInvoker; the production manifest is unchanged.

A later [Properties follow-up](NativeDarkMode-Properties.md) replaces the synthetic
missing-data limitation for one installed entry. This report retains the earlier
compiled-DLL replay as originally scoped.

## Observed coverage

| Surface | Observed result | Scope limit |
| --- | --- | --- |
| Main inventory | 468 visible entries after the initial scan. Dark row palettes, legend and treemap agree; group captions and links are readable. Search for 7-Zip yields one row. Checkbox selection survives switching to an ungrouped list. | Unelevated scan with external scan sources disabled in host settings. Narrow existing columns remain truncated. |
| Refresh | Filtered row remains on a dark background while disabled; both loading bars are blue. Completion restores input and the one-row filter. | Initial unfiltered load and one filtered reload; no endurance test. |
| Properties | Overview grid/header, four pages, application tabs, and copy/save menu icons remain readable. | Synthetic applications; uninstaller/registry/certificate pages show their missing-data states. No clipboard or file write. |
| Settings | All seven pages render in dark mode; language dropdown is readable and the cache explanation wraps. | English at 200%; no live language change or new behavior-setting exercise. |
| Wizard | Related plugin selection flows into three checked confirmation rows, then options and summary. The real process check finds no fixture processes and skips its empty page. Finish returns OK. | No execution caller, actual process target, backward-navigation replay, or nonzero size totals. |
| Progress | All eight statuses display; grouped and ungrouped lists remain readable. Marquee-to-determinate recreation settles to blue. Completion reaches 8/8 and replaces the instructional icon with a white checkmark. | Synthetic task never started. Execution actions detached in host; no termination, retries, sleep or walk-away behavior tested. |
| Leftovers | Mixed-confidence selection, group text, context menu icons and actual custom confidence-details dialog render in dark mode. Checked fixture results are returned. | Accept event replaced by inspection-only close; no backup, process checks or deletion. |
| Light comparison | The same DLL with --light-mode retains white leftover/progress lists, green native progress and black instruction/completion icons. | Two representative dialogs; default/no-switch and .NET 8 bypass covered by separate assembly checks, not a full light visual replay. |

The first wizard attempt exposed an omitted host prerequisite: MessageBoxes.DefaultOwner
was null. The host now supplies it before constructing each form, as the production
main window normally does. The original exception log is retained. No application
source change was needed, and no additional unhandled exception was logged through
the subsequent wizard, progress, inventory and light comparison runs.

## Evidence and remaining work

The external development workspace contains `prototypes/IntegrationVisual` and
`artifacts/integration-visual`: source/launcher, assembly identity, timestamped
JSONL events and named screenshots. Dark and light progress-close events report
`workerStarted=false`, `value=8`, `maximum=8`. Inventory events record transitions
from 468 visible rows to one, then disabled and enabled during refresh.

The host uses separate settings and fixture data. Review actions do not establish
real uninstall, process termination, registry backup or cleanup correctness. Its
main-window branch is the actual application UI and was used only for browsing
and refresh; its execution commands are not disconnected.

Normal elevated startup, complete inventory/helper packaging, other Windows/DPI
configurations, high contrast, localization/accessibility and disposable-target
execution remain open in the [regression checklist](NativeDarkMode-Checklist.md).
Existing bright borders, dim disabled text, truncated columns and unadapted native
MessageBox/HTML surfaces remain follow-up work. The original all-bad-confidence
leftover filtering issue remains separate from the theme patch.
