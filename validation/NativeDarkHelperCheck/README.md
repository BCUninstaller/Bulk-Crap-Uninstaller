# File deletion and loading-dialog regression check

Build the normal .NET 10 BCU package first, including UniversalUninstaller.
Then, from the repository root (PowerShell):

```powershell
$package = 'C:\path\to\built\package'
dotnet build validation\NativeDarkHelperCheck\NativeDarkHelperCheck.csproj -c Release -p:BcuOutputPath="$package" -o "$package"
dotnet "$package\NativeDarkHelperCheck.dll" --dark
dotnet "$package\NativeDarkHelperCheck.dll"
```

Use a disposable package directory: the checker adds its own executable and a
`helper-check-fixture` directory containing two test files. It never invokes file
deletion. Both runs check actual helper-form rows, native virtual-list counts,
expansion/collapse, checked selection and handle recreation. The modal loading
factory exercises each progress bar through determinate, marquee and determinate
states; dark runs also check that native visual styles remain disabled after each
handle recreation. Dark mode is suppressed when Windows high contrast is active.

Optional `--visual` keeps the final loading state visible for 60 seconds.
Optional `--launch-helper` then launches UniversalUninstaller through BCU's actual
`RunUninstaller` path. Cancel that window within two minutes; the checker requires
the cancellation exit code. This checks theme propagation at the process boundary.
Run at matching integrity if inspecting its UI. The normal helper executable has
an administrator manifest; a controlled `RunAsInvoker` run covers ordinary
integrity only and does not validate elevation.
