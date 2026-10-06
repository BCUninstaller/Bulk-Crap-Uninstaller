# Properties pages with installed application data

The 2026-09-05 follow-up loaded the compiled integration DLL in the external
asInvoker visual host, ran BCU's ordinary unelevated inventory scan and selected
an entry with a readable uninstall registry key, existing uninstaller executable
and extractable certificate. The entry identity and paths were excluded from the
evidence. The replay invoked no uninstall, registry write, clipboard or save action.

The actual `PropertiesWindow` displayed populated Overview, Uninstaller information,
Registry and Certificate pages with 31, 17, 14 and 15 rows respectively. At 200%
DPI, headers, tabs, rows, current selection, scrolling and long file, registry and
certificate values remained readable in opt-in dark mode. The ordinary light
comparison retained its existing appearance. No production rendering change was
needed.

The same open window was checked through configured High Contrast Black and a
controlled light contrast color table. Thirteen dark-startup page records covered
the ordinary and contrast palettes with no failed checks for page population,
column count, grid/header text or effective paint-time selection colors. Selection
contrast ranged from 5.159:1 to 7.893:1. Both contrast cycles restored flags,
scheme, all 30 system colors and CurrentTheme with identical metadata.

The host build had zero warnings and errors. Its copied application DLL matched
the published build at SHA-256
`A9107D6386DE3050EC1EBDDC600A37CA052AEE14E2EC8CAA6776C2718FA4788E`.
Raw logs, screenshots and the read-only replay source remain in the external
development workspace and are not included in this PR.

This closes the earlier specialist-page missing-data gap for one installed entry
on Windows 11 build 26100 at 200% DPI. Multiple application tabs, clipboard/save
actions, production elevation/startup, other Windows/DPI configurations and
keyboard/screen-reader navigation remain separate release checks.

