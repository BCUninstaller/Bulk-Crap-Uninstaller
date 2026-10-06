# DPI and high-contrast investigation

The 2026-09-04 platform pass used the same compiled integration DLL recorded in
[the visual replay](NativeDarkMode-Visual.md). The review-host additions build with
zero warnings/errors. No application code was changed during this investigation.
The platform acceptance gate remains open: this pass found two concrete gaps.

The fixed-width DPI finding remains outside the focused dark-mode change. The
[contrast icon fix](NativeDarkMode-Icons.md) addresses the observed black-on-dark
failure. This document retains the original measurements
as the reproduction; full physical/platform validation remains open.

## Fixed column widths at high DPI

Actual leftover and progress windows were compared in a native per-monitor-aware
192-DPI process and a separate DPI-unaware process reporting 96 DPI. The latter is
bitmap-scaled by Windows on the current 200% display; it checks logical layout,
not a native 100% display or a monitor transition.

| Column | At 96 logical DPI | At native 192 DPI | Expected width if scaled proportionally |
| --- | --- | --- | --- |
| Leftovers: Item path | 412 px | 412 px | 824 px |
| Leftovers: Confidence | 103 px | 103 px | 206 px |
| Leftovers: Uninstaller Name | 163 px | 163 px | 326 px |
| Progress: Id | 25 px | 25 px | 50 px |
| Progress: Status | 105 px | 105 px | 210 px |
| Progress: Quiet | 47 px | 47 px | 94 px |

The fill column absorbs remaining width, but the fixed columns retain their pixel
sizes while fonts scale. Confidence/status labels are consequently truncated at
200%. The same 192-DPI widths and truncation reproduce with --light-mode, so this
is not caused by the dark adapters. Group labels, wrapped introductory text and
progress bars remained readable in the inspected dark windows at both control DPI
values. Fixed-width column scaling needs a separate change that also preserves
user-resized widths and handles repeated monitor transitions without double scaling.

## Actual high-contrast transition

A bounded external helper saved Windows' current high-contrast state, system colors
and theme path, enabled high contrast through SystemParametersInfo, then restored
the saved state in a finally block. No production manifest or elevation was changed.
One already-dark process remained open; another process started with --dark-mode
while high contrast was active. Both used inert review data.

| Case | Result |
| --- | --- |
| Already-dark process enters high contrast | ThemeManager.IsEnabled and Application.IsDarkModeEnabled become false. System-color-backed list foreground/background follow the contrast scheme. Previously copied icons and explicit link RGB values remain. |
| Fresh startup under high contrast | The explicit dark request is suppressed. The original black instruction and toolbar icons are nearly invisible against the actual dark contrast background. |
| Already-dark process after high contrast ends | Both dark-mode flags return true and the blue progress rendering returns. |
| High-contrast-started process after high contrast ends | Remains classic/light, as expected for the startup-only policy; a restart is needed to request dark mode again. |

The request named High Contrast White, but Windows actually activated its black
contrast theme (dark background, white text, yellow title bar). The report uses
observed colors and screenshots; it does **not** claim a white-contrast test passed.
The icon failure occurs while the dark adapters are disabled. It establishes a gap
in the fallback path, not a regression uniquely attributable to dark icon copying.
Other schemes are still needed to assess the retained RGB/link/icon state during a
live transition. A global replacement of all icons would be inappropriate because
application logos and colored assets must retain their meaning.

Restoration has an asynchronous step: the first immediate check saw the old contrast
colors and reported failure. A later read verified high contrast off, the original
white/black system colors and the original Custom.theme path. Windows initialized
the previously empty contrast-scheme name to High Contrast Black and cleared the
transient HCF_OPTION_NOTHEMECHANGE flag; the metadata is not byte-for-byte identical.
The original failure and the settled recovery snapshot are both retained. The
helper now waits for the appearance to settle, records metadata separately, and
uses the configured scheme rather than requesting a scheme by name.

## Evidence and next work

The external host's `--platform` mode records real form/control DPI, bounds, column
widths, system colors, theme flags and instruction-image brightness. Screenshots,
timestamped JSONL, contrast restoration snapshots and `platform-checks.json` are in
`artifacts/integration-visual`. Eight evidence assertions pass; they confirm the
observed transitions and reproductions, not full platform acceptance.

The most actionable next change is scaling fixed ObjectListView columns, with the
96/192-DPI measurements and light comparison as a reproduction. High-contrast icon
handling also needs a targeted design covering both light and dark contrast schemes
and already-open forms. Native 100%, 125%, 150%, cross-monitor movement, keyboard and
screen-reader checks remain open in the [checklist](NativeDarkMode-Checklist.md).

Reference: [Microsoft's SetColorMode contract](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.application.setcolormode?view=windowsdesktop-10.0)
describes high-contrast exclusion and startup color-mode selection. The
[HIGHCONTRAST structure](https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-highcontrasta)
documents the flags, including the transient no-theme-change option.
