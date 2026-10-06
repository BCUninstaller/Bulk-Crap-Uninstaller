# Search-field background refresh

The main search field retained its dark native background brush after a live
transition to light high contrast. Its BackColor property correctly reported
SystemColors.Window, but the brushes returned for both WM_CTLCOLOREDIT and
WM_CTLCOLORSTATIC still painted the old dark RGB value. Activation could temporarily
hide the issue; disabling the field during inventory reload exposed it again.

SearchBox now creates a private TextBox subclass that observes SystemColorsChanged.
For a system-color background it calls the normal protected OnBackColorChanged
path, which releases the cached background brush and invalidates the control.
The existing system-color property is retained. This follows the cache lifecycle
in the [WinForms Control source](https://github.com/dotnet/winforms/blob/v10.0.0/src/System.Windows.Forms/System/Windows/Forms/Control.cs),
where SystemColorsChanged alone invalidates painting without clearing BackColorBrush.

The correction is confined to SearchBox's inner edit control in KlocTools, shared
by both runtime builds. It does not change search parsing/events, placeholder and
focus logic, text selection, autocomplete, colors chosen by callers, or native
handle lifecycle. Custom RGB backgrounds keep their existing behavior. No native
theme override, reflection, new resource ownership or application-wide adapter is
added to production code.

## Verification and scope

The external checker covers enabled/disabled brush requests, retained semantic
system colors, custom colors, text/selection/handle/event preservation, placeholder
and focused-empty behavior, and handle recreation. The native host logs brush
colors from the actual main window while retaining a selected search query across
contrast changes and inventory reload. The host does not supply corrective colors.

Build identities, before/after brush measurements, final checker counts and
screenshots are recorded in the external prototype's
`artifacts/search-contrast/REPORT.md`. Light contrast uses a controlled application
of the installed hcwhite.theme system-color table; it is not native Settings
selection of that named theme.

The later [main-button correction](NativeDarkMode-Buttons.md) handles sidebar
checkboxes and the Advanced filtering button; list checkbox images remain open.
This change does not close whole-window contrast, native theme
selection, other Windows/DPI configurations or the broader
[release checklist](NativeDarkMode-Checklist.md).
