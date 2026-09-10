# Contributing to EBUninstaller Pro

Thank you for your interest in contributing to **EBUninstaller Pro**! We welcome contributions from developers, testers, designers, and translators worldwide. Whether you are fixing a bug, adding a new detection factory, improving system cleaning engines, enhancing UI/UX, or adding localizations, your help makes this project better for everyone.

---

## 📑 Table of Contents
1. [Code of Conduct](#-code-of-conduct)
2. [Development Environment Setup](#-development-environment-setup)
3. [Building and Debugging](#-building-and-debugging)
4. [Architecture & Modular Subsystems](#-architecture--modular-subsystems)
5. [Coding & Design Standards](#-coding--design-standards)
6. [Security & Defensive Engineering](#-security--defensive-engineering)
7. [Testing & Quality Verification](#-testing--quality-verification)
8. [Localization & RTL Support](#-localization--rtl-support)
9. [Submitting Issues & Feature Requests](#-submitting-issues--feature-requests)
10. [Pull Request Workflow](#-pull-request-workflow)

---

## 📜 Code of Conduct
This project follows the **Contributor Covenant v2.1**. By participating, you agree to maintain a respectful, welcoming, inclusive, and harassment-free community. See [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md) for full details.

---

## 🛠️ Development Environment Setup

### System Prerequisites
* **Operating System**: Windows 10 (version 1903+) or Windows 11 (x64 / ARM64).
* **.NET SDK**: .NET 8.0 SDK (targeting `net8.0-windows10.0.18362.0`).
* **IDE / Editors**:
  * **Visual Studio 2022** (v17.8 or higher) with the *.NET Desktop Development* workload.
  * **JetBrains Rider 2023+** with .NET desktop toolset.
  * **Visual Studio Code** with *C# Dev Kit* and *.NET Install Tool*.
* **Installer Compilation (Optional)**: **Inno Setup 6.2+** (for compiling `installer\EBUninstallSetup.iss`).
* **Scripting Runtime**: PowerShell 7+ or Windows PowerShell 5.1, Python 3.9+ (for verification scripts).

---

## 🏗️ Building and Debugging

### 1. Clone the Repository
```bash
git clone -b arena/01a06ec7-bulk-crap-uninstaller https://github.com/EhabYT/Bulk-Crap-Uninstaller.git
cd Bulk-Crap-Uninstaller
```

### 2. Build via .NET CLI
```powershell
# Restore NuGet dependencies
dotnet restore source\EBUninstaller.sln

# Build Release binaries for AnyCPU
dotnet build source\EBUninstaller.sln -c Release /p:Platform="AnyCPU"
```

### 3. Build via Automated Scripts
```powershell
# Compiles solution, runs test suites, builds portable ZIP, and compiles Inno Setup installer
.\scripts\build.ps1 -Configuration Release -Platform AnyCPU -BuildInstaller
```

### 4. Running the Application in Visual Studio
* Set **`BulkCrapUninstaller`** as the Startup Project for the WinForms GUI.
* Set **`BCU-console`** as the Startup Project for the Command-Line Interface.
* Press `F5` to build and debug with full symbol support.

---

## 📐 Architecture & Modular Subsystems

EBUninstaller Pro is engineered with a strict modular architecture located in `source\UninstallTools`:

| Subsystem | Folder Path | Primary Responsibility |
| :--- | :--- | :--- |
| **Core** | `UninstallTools\Core\` | `SecurityGuard`, `CryptoHasher` (SHA-256), `DigitalSignatureVerifier`, `StructuredLogger`, `UpdateManager`. |
| **Detection** | `UninstallTools\Detection\` | `AppFilterEngine`, `ConfidenceScorer`, `SoftwareHealthEngine`, `GameLauncherFactory`, `PackageManagersFactory`. |
| **Uninstaller** | `UninstallTools\Uninstaller\` | `UninstallPipeline` (10-Phase uninstallation lifecycle), `BatchQueue`, `SilentDetector`. |
| **Forced Removal** | `UninstallTools\ForcedRemoval\` | `ForcedUninstallManager`, `DeepScanner`, `MultiSignalScorer` (for broken/stubborn software). |
| **Leftovers** | `UninstallTools\Junk\` | `LeftoverScanner`, confidence rating containers, registry and file system junk finders. |
| **Monitor** | `UninstallTools\InstallationMonitor\`| `InstallationMonitorEngine`, `SnapshotDiffer`, `TraceReplayer` (user-mode tracking). |
| **Backup** | `UninstallTools\Backup\` | `BackupManager`, `BackupManifest`, `.reg` export and ZIP packaging with SHA-256 verification. |
| **Junk Cleaner** | `UninstallTools\JunkCleaner\` | `JunkCleanerEngine`, `DriverAndSystemResidualsCleaner`, `DeviceDriverResidualsCleaner`. |
| **Privacy Cleaner** | `UninstallTools\PrivacyCleaner\` | `PrivacyCleanerEngine`, browser cookies, history, and cache purgers (Chrome, Edge, Firefox, Brave, Opera). |
| **Extensions** | `UninstallTools\BrowserExtensions\`| `BrowserExtensionManager` for Chromium and Gecko add-on inspection and removal. |
| **Startup** | `UninstallTools\Startup\` | `StartupManager`, `StartupImpactAnalyzer` (boot time analyzer). |
| **System Tools** | `UninstallTools\SystemTools\` | `WindowsToolsLauncher`, `MemoryTrimmerEngine`, `AutoMaintenanceScheduler`. |
| **Hunter Mode** | `UninstallTools\HunterMode\` | `TargetModeController`, target crosshair window/process identifier. |
| **Localization** | `UninstallTools\Localization\` | `LanguageManager` (English, German, Arabic RTL). |
| **UI Controls** | `BulkCrapUninstaller\Controls\` | `ModernNavCommandBar`, `QuickFilterChipsBar`, `ModernStatsDashboard`, `AppDetailsPanel`. |

---

## 💻 Coding & Design Standards

* **C# 12 & .NET 8**: Utilize modern language features including pattern matching, records, file-scoped namespaces, collection expressions, and nullable reference types where applicable.
* **Asynchronous Execution**: Long-running I/O, file scans, and registry traversals must use `async`/`await` or background workers to prevent UI thread freezing.
* **Defensive Null Handling**: Always validate inputs, file existence, and registry subkeys against null before dereferencing.
* **Resource Disposal**: Wrap all I/O streams, registry keys, process handles, and bitmaps in `using` statements or `Dispose()` calls.
* **High-DPI & Theming**: All WinForms forms and custom user controls must support high-DPI scaling (`OnDpiChanged`) and integrate with `ThemeEngine.ApplyThemeToForm(this)`.

---

## 🔒 Security & Defensive Engineering

1. **System Path Protection (`SecurityGuard`)**:
   * Under **no circumstances** may protected Windows directories (`C:\Windows`, `C:\Windows\System32`, `WinSxS`, `Program Files\Windows Defender`) or critical registry hives (`HKLM\SAM`, `HKLM\SECURITY`, `HKLM\SYSTEM`) be marked for automated deletion.
   * Every file and registry deletion must pass through `SecurityGuard.IsProtectedPath()` and `SecurityGuard.IsProtectedRegistryKey()`.
2. **Offline-First & Zero Telemetry**:
   * EBUninstaller Pro is strictly offline-first. Never introduce third-party tracking scripts, analytic endpoints, or silent cloud communication.
3. **No Proprietary Code**:
   * Do not copy, adapt, or reference proprietary code, private algorithms, or copyrighted UI assets from commercial uninstaller software.

---

## 🧪 Testing & Quality Verification

Every contribution must pass the automated test suite and static integrity analyzer:

```powershell
# 1. Run full NUnit test suite
dotnet test source\BulkCrapUninstallerTests\BulkCrapUninstallerTests.csproj --configuration Release

# 2. Run static architecture and structural verification audit
python3 scripts\verify_repo.py
```

### Writing New Unit Tests
Add unit tests under `source\BulkCrapUninstallerTests\` for every new engine, detection factory, or bug fix. Ensure test cases cover:
* Expected happy-path execution
* Edge cases (empty lists, missing paths, locked files, corrupt strings)
* Security denylist validation

---

## 🌐 Localization & RTL Support

EBUninstaller Pro supports multi-language interfaces with native **Right-To-Left (RTL)** layout mirroring for Arabic:
* Add or update localized string keys in `source\UninstallTools\Localization\LanguageManager.cs`.
* Update language resource files (`.resx`) under `source\BulkCrapUninstaller\Properties\Localisable.*.resx` and `Forms\Windows\*.resx`.
* When adding Arabic strings, test UI layout flipping using `LanguageManager.SetLanguage(SupportedLanguage.Arabic, mainForm)`.

---

## 🐛 Submitting Issues & Feature Requests

* **Bug Reports**: Provide Windows version, system architecture (x64/ARM64), application version, detailed reproduction steps, and relevant log excerpts from `%LOCALAPPDATA%\EBUninstallerPro\Logs\`.
* **Feature Requests**: Clearly describe the proposed feature, user benefits, and technical feasibility.

---

## 🤝 Pull Request Workflow

1. Fork the repository and create a feature branch:
   ```bash
   git checkout -b feature/my-new-feature
   ```
2. Commit your changes with clear, descriptive commit messages.
3. Ensure all 16 test suites pass and `python3 scripts\verify_repo.py` reports `100% integrity`.
4. Push your branch to GitHub and open a Pull Request against `arena/01a06ec7-bulk-crap-uninstaller`.
5. Provide a detailed summary of your modifications and test results in the PR description.

---

*Thank you for helping build **EBUninstaller Pro**!*
