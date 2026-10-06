# Fork validation on BCU v6.3

This pass was run on 2026-09-26 against `integration/native-dark` at
`8d2890b`, based on upstream `30da609` and BCU v6.3. The host was Windows 11
24H2 build 26100.9457 at the existing 200% display configuration, with .NET
Desktop Runtime 10.0.12.

## Persisted setting and restart follow-up

The current revision replaces the prototype command-line activation with
**Settings > Interface > Use dark mode (restart required)**. The code at PR commit
`867e039` was built by
[fork CI run 36248632055](https://github.com/ObnubiladO/Bulk-Crap-Uninstaller/actions/runs/36248632055)
on temporary validation commit `0d72482`, which differs only by enabling the CI
workflow on that branch. Both build/test and native-launcher jobs passed; the
published artifact is named
`Bulk-Crap-Uninstaller_0d72482bfa349df967239d0e5681c011ea0488ee`.

The exact portable `BCUninstaller.exe` was exercised at normal integrity for UI
automation. A light start showed the option clear. Enabling it, closing Settings
and accepting the restart prompt produced a new process with a dark main window;
the reopened option was checked and `BCUninstaller.settings` contained
`WindowUseDarkMode=True`. Clearing it and accepting the next restart produced a
new light process; the option reopened unchecked and the XML contained
`WindowUseDarkMode=False`. This validates both persistence directions and the
repaired app-host/mutex/save ordering. The local application firewall's outbound
alert did not prevent the offline setting and restart cycle.

The older command-line pass below remains historical evidence for the surfaces it
covered. Those switches are no longer part of the current implementation.

The interactive run used an isolated copy of the portable package and invoked
the production managed entry point with `dotnet BCUninstaller.dll --dark-mode`.
Helper-backed scans were disabled in that copy because the managed process was
not elevated. Registry discovery remained enabled and produced a ready inventory
of 503 entries reporting 328.83 GB.

## Observed results

- The dark main inventory rendered readable rows, group captions, headers,
  links, checkbox columns, legend and treemap. Filtering for `7-Zip` produced two
  entries. Arrow-key movement and Space checking worked in the list.
- Both grouped and ungrouped views rendered correctly. A large `A` group
  collapsed and expanded without a palette defect; Name sorting reversed the
  group and row order and then returned to ascending. The grouped preference was
  restored before the close/relaunch check.
- Reload showed the disabled dark list and both blue progress bars. The active
  filter and its input survived the rebuild. The checked selection was cleared;
  this pass records the behavior without treating it as intentional.
- Properties for `7-Zip 26.03 (x64)` rendered all four pages: Overview,
  Uninstaller information, Registry and Certificate.
- Settings rendered all seven pages: General, Interface, Uninstallation,
  Detection, External tools, Folders and Miscellaneous.
- Closing through the dark `Send feedback` window terminated the process. The
  portable settings file remained valid XML, and a subsequent dark launch
  returned to a ready 503-entry inventory.
- A launch with no theme switch returned to the normal ready light inventory.

These observations refresh the main-window, refresh, Properties, Settings and
light-mode evidence on the v6.3 package. They do not replace the earlier fixture
coverage for wizard, progress and leftover-review states.

## Lifecycle qualification

The exact packaged executable previously passed single-instance handling: a
second executable exited with code 0 while the first remained responsive and was
the sole BCU process. In this pass a second *managed-host* launch remained as a
headless `dotnet` process for more than 15 seconds and was terminated by its exact
PID. The production executable result is the relevant release path, but the
managed-host discrepancy means the combined lifecycle gate remains open until an
exact executable close/restart/culture replay is completed.

## Coverage that still needs another environment

- Packaging and installer upgrade behavior, including all runtime and helper
  payloads.
- A normally elevated inventory with every external helper enabled.
- Windows 10 and physical 100%, 125%, 150% and 200% displays, including moves
  between monitors.
- Native selection of additional Windows contrast themes and restart recovery.
- Real screen-reader announcements, long translations and RTL layouts.
- Disposable uninstall, restore-point, process-control, backup and junk-deletion
  workflows.
- A repeatable resource-stability campaign. An attempted modal-loop measurement
  in this pass was discarded because the automation surface did not reliably
  identify the window receiving every close key.

The ignored raw summary is stored with the local validation package under
`artifacts/fork-validation-20260926-154213/validation-summary.json`; it is not a
portable test artifact and is intentionally excluded from the repository.
