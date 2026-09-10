# Build Instructions

This document describes how to build EBUninstaller Pro from source.

---

## 1. Prerequisites

* **Operating System**: Windows 10 / 11 (x64 or ARM64) or Linux/macOS for static verification.
* **SDK**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
* **IDE**: Visual Studio 2022 (v17.8+) or Visual Studio Code with C# Dev Kit.
* **Installer Compiler (Optional)**: [Inno Setup 6.x](https://jrsoftware.org/isdl.php) (for building the Windows installer).

---

## 2. Building via Command Line (.NET CLI)

### Build Debug Configuration
```bash
dotnet build source/EBUninstaller.sln -c Debug
```

### Build Release Configuration
```bash
dotnet build source/EBUninstaller.sln -c Release /p:Platform=AnyCPU
```

### Build for Specific Architectures
```bash
# x64 Release Build
dotnet build source/EBUninstaller.sln -c Release /p:Platform=x64

# ARM64 Release Build
dotnet build source/EBUninstaller.sln -c Release /p:Platform=ARM64
```

---

## 3. Running Unit and Integration Tests

```bash
dotnet test source/BulkCrapUninstallerTests/BulkCrapUninstallerTests.csproj -c Release
```

---

## 4. Automated Build Script (PowerShell)

To run the complete automated build, execute tests, create the portable ZIP package, and compile the installer:

```powershell
.\scripts\build.ps1 -Configuration Release -Platform AnyCPU -BuildInstaller
```

Output artifacts will be generated in:
* `bin\Release\AnyCPU\` (Compiled binaries)
* `build\portable\EBUninstaller_Pro_Portable.zip` (Portable release)
* `build\installer\EBUninstaller_Pro_v7.0.0_Setup.exe` (Inno Setup installer)

---

## 5. Repository Verification & Analysis Script

To run structural syntax, XML, C# consistency, and architecture verification:

```bash
python3 scripts/verify_repo.py
```
