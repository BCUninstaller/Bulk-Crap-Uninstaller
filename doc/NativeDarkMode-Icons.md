# High-contrast icon follow-up

Subsequent work: [selected menu icons and link colors](NativeDarkMode-ContrastStates.md)
now covers the selected-state and controlled light-palette gaps identified below.
This document retains the initial icon-fix evidence and assembly identity.

The 2026-09-04 change fixes the observed black BCU instruction/toolbar icons on a
dark high-contrast background. It applies to the inspected BCU UI assets in both
the earlier .NET 8 and .NET 10 comparison builds, including classic/light startup.
High contrast still suppresses the experimental dark control adapters.

## Implementation

`ThemeImageBinding` retains the original resource and owns only a recolored copy.
In high contrast it uses the current `SystemColors.ControlText`; ordinary dark
mode uses the same foreground. Classic mode outside high contrast restores the
original image. Color values are captured as ARGB integers so changes to a named
system color invalidate the cached copy.

Bindings observe system-color and user-preference notifications, coalesce refreshes
through the control's UI queue, and refresh after handle creation. Disposal removes
the static event subscription and disposes only owned copies. Explicit picture
reapplication detects replacement images, including the progress completion icon.

The monochrome classifier rejects an entire image if any visible pixel is colored,
preserving colored artwork and its black outlines. For eligible grayscale assets,
only dark ink below 80 is recolored; alpha is retained. Installed-program icons
are not submitted to this helper. This remains a scoped asset adapter, not a
general-purpose image inversion or live light/dark preference feature.

## Verification

Both GUI builds succeeded. The external compiled-assembly checker passed 94
assertions across .NET 10 default (23), dark (25), light override (23) and .NET 8
dark bypass (23), using Desktop runtimes 10.0.11 and 8.0.30 at DPI 192.
Thirteen added checks per case cover white/black/custom foregrounds, alpha,
idempotence, source preservation, colored-artwork exclusion, dynamic image
replacement, off-thread notification and disposal with a queued callback.

A real Windows 11 build 26100 / 200% cycle used the configured High Contrast Black
scheme with actual compiled progress and leftover forms in the external inert host:

| Case | Observed result |
| --- | --- |
| Already-open light process enters high contrast | Instruction and toolbar icons change from black to white. |
| Fresh --dark-mode request during high contrast | Dark mode remains suppressed; instruction and toolbar icons are white. |
| Fixture completes while high contrast is active | The replacement checkmark is white and readable. |
| High contrast ends | Both processes restore black icons; the completed process remains classic, consistent with startup-only mode selection. |

The helper verified the original theme path, colors, flags and configured scheme
were restored, with `Restored` and `MetadataIdentical` both true. No uninstall
worker, backup or deletion ran. The visual host build had zero warnings/errors;
its existing historical error log did not grow.

Reviewed published/host BCU SHA-256:
`48C1BE1DDA1AA334F8A4776D3C401675CEE3BFB7295A94DADDE57E9EB7E618B5`.
ObjectListView SHA-256:
`F91F81E9C02B4A871574E919D18668CD049A7C7521E36B22C8BACEBB3A0D4377`.
Evidence is retained in the prototype checkout under `artifacts/contrast-icons`
and the pinned platform logs named in its report.

Native light-background/custom contrast schemes, selected/hovered menu states,
explicit RGB link/row colors on already-open dark forms and the complete production
accessibility matrix remain open. Injected black/yellow palette tests do not claim
native visual coverage for those schemes. Existing NU1510 and SYSLIB0057 build
warnings remain visible. See [the checklist](NativeDarkMode-Checklist.md).

A later [menu/overflow replay](NativeDarkMode-Menus.md) covers selected nested,
context and overflow menu icons in ordinary dark plus both controlled contrast
palettes. This report retains the earlier icon investigation's original scope.
