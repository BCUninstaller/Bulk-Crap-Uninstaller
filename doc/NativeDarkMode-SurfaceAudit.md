# Remaining window and native-surface audit

This source audit was refreshed on 2026-09-26 after the v6.3 validation pass. It
separates known message/HTML paths from windows that still need an interactive
dark and contrast review.

## Native messages and HTML

The live `MessageBox.Show` search has two implementation locations:

- `Functions/MessageBoxes.cs` is the central compatibility path. Its supported
  dark layouts are redirected through `ThemeManager` and the custom dialog; the
  native implementation remains the deliberate fallback for unsupported or
  unowned calls.
- `ThemeManager.ShowJunkDetails` uses the custom dialog in dark mode and the
  original native `MessageBox` in light mode.

The only embedded HTML surface is `FeedbackWindow` and its WinForms
`WebBrowser`. The focused repair checker covers current and legacy page fixtures,
and the live current page was validated through the network-permitted .NET host.
No additional HTML renderer or direct native-message implementation was found.

This closes the source audit for native messages and HTML. It does not claim that
every call-site workflow was executed.

## Windows still needing interactive review

The following application windows contain buttons, checkboxes, links or lists but
do not have a dedicated `ThemeManager.Apply*` call in their own constructor:

- first-run wizard, About, Feedback box, News popup and Rating popup;
- advanced clipboard copy, custom note and Debug windows;
- startup manager and ObjectListView column-selection dialogs from shared
  projects.

`ListLegendWindow`, `TargetWindow` and ObjectListView's transparent
`GlassPanelForm` are specialized overlays without the same standard-button risk.
They still need a visual check when their owning workflow is exercised.

.NET 10 application color mode supplies the baseline palette for standard
WinForms controls, but the explicit contrast adapter is currently attached only
to the surfaces listed in the handoff's implemented scope. The remaining-window
check therefore stays open until these dialogs are rendered in dark, light and
contrast modes and any actual failures receive targeted hooks.
