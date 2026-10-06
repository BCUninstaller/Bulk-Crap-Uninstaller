# Main-window buttons and checkboxes in contrast mode

The controlled light-contrast replay left the Advanced filtering button with a
dark background and sidebar checkbox glyphs with dark fills. Clearing the native
DarkMode_Explorer association did not correct their themed rendering. WinForms'
built-in Flat renderer produced readable palette-based backgrounds, check marks
and keyboard-focus highlighting in the same replay.

ButtonContrastAdapter enrolls ordinary Standard Button and CheckBox controls
in explicitly adapted windows' initial control trees. During high contrast it uses Flat;
afterward it restores Standard. Both runtime builds use this accessibility
fallback. There is no dark-mode opt-in requirement for high contrast.

Radio buttons, uninspected forms and controls initially using custom flat styles
are excluded. The adapter does not assign colors, change check states, invoke actions,
replace click handlers or set control bounds. It uses the framework's existing
renderers. Attachments are idempotent, callbacks are coalesced on the UI thread,
recreated handles are handled, and pending callbacks tolerate disposal. A caller's
later style choice is not overwritten during recovery.

The distinction between Standard's themed rendering and Flat's managed rendering
is described in the [WinForms ButtonBase source](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Controls/Buttons/ButtonBase.cs).
This is a scoped fallback, not a replacement button implementation.

## Verification and tradeoffs

The external checker covers runtime-independent contrast behavior, scope and
custom-style exclusions, idempotence, check/action state, enabled/disabled refresh,
native handle recreation and disposal. Contrast-specific painting checks verify
the button interior and distinct unchecked/checked/indeterminate glyphs.

The actual main-window replay covers both palettes, mouse/Space checkbox toggling,
Shift+Tab button focus, disabled inventory reload in dark contrast, and recovery.
At 192 DPI, AutoSize checkbox widths shrink by five native pixels in Flat style;
labels remain legible and the original widths return. The Advanced filtering
button retains its bounds but WinForms recreates its handle when changing style.
Checkbox handles and checked states survive the transitions. Fixed-size fixture
controls do not have the same AutoSize/recreation behavior as every real control.

Exact builds, screenshots, observations and final counts are in the external
prototype's `artifacts/button-contrast/REPORT.md`. The light test uses the installed
hcwhite.theme system-color table while contrast is active, not native Settings
selection of that theme. A stale screenshot prevented the attempted light reload
click; native disabled reload was verified in dark contrast, while automated
enabled/disabled checks ran in both palettes.

Main-list checkbox painting is separate from sidebar CheckBox controls; the
[list-checkbox follow-up](NativeDarkMode-ListCheckboxes.md) covers that path.
The same adapter is now attached through the existing theme entry points for
Settings, Wizard, Properties, loading dialogs, Progress and Junk review. An actual
Settings replay covers 53 Button/CheckBox controls, input and recovery in both
contrast palettes; constructor checks cover the other entry points. Uninspected
windows, native theme selection, other Windows/DPI
configurations and the full [release checklist](NativeDarkMode-Checklist.md) remain
outside this correction.
