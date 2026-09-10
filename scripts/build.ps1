<#
.SYNOPSIS
    EBUninstaller Pro - Automated Build and Release Script for Windows (.NET 8)
.DESCRIPTION
    Builds the solution in Release mode (x64, ARM64, AnyCPU), runs unit/integration tests,
    generates portable ZIP packages, and builds the Inno Setup installer.
#>

param(
    [string]$Configuration = "Release",
    [string]$Platform = "Any CPU",
    [switch]$SkipTests,
    [switch]$BuildInstaller,
    [switch]$Clean,
    [switch]$RunVerify,
    [string]$InnoSetupPath = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$SolutionPath = if (Test-Path (Join-Path $RepoRoot "source\EBUninstaller.sln")) { Join-Path $RepoRoot "source\EBUninstaller.sln" } else { Join-Path $RepoRoot "source\EBUninstaller.sln" }
$OutputDir = Join-Path $RepoRoot "bin\$Configuration\AnyCPU"
$BuildDir = Join-Path $RepoRoot "build"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " Building EBUninstaller Pro ($Configuration - $Platform)        " -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

# Unblock files in repository if downloaded from web (Mark of the Web / Zone.Identifier)
Get-ChildItem -Path $RepoRoot -Recurse -ErrorAction SilentlyContinue | Unblock-File -ErrorAction SilentlyContinue

# 0. Clean prior build if requested
if ($Clean) {
    Write-Host "[Clean] Cleaning previous build outputs..." -ForegroundColor Yellow
    if (Test-Path $BuildDir) {
        Remove-Item -Path $BuildDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

New-Item -ItemType Directory -Force -Path (Join-Path $BuildDir "portable") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $BuildDir "installer") | Out-Null

# 1. Check dotnet CLI
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet CLI is not found in PATH. Please install .NET 8 SDK."
}

# 2. Build Solution
Write-Host "`n[1/4] Compiling Solution..." -ForegroundColor Yellow
dotnet build $SolutionPath -c $Configuration /p:Platform="$Platform" /p:Version="7.0.0"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
}

# 3. Run Unit and Integration Tests
if (-not $SkipTests) {
    Write-Host "`n[2/4] Executing Test Suite..." -ForegroundColor Yellow
    $TestProjectPath = if (Test-Path (Join-Path $RepoRoot "source\EBUninstallerTests\EBUninstallerTests.csproj")) { Join-Path $RepoRoot "source\EBUninstallerTests\EBUninstallerTests.csproj" } else { Join-Path $RepoRoot "source\BulkCrapUninstallerTests\BulkCrapUninstallerTests.csproj" }
    dotnet test $TestProjectPath -c $Configuration --no-build --logger "console;verbosity=normal"

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Test execution failed!"
    }
} else {
    Write-Host "`n[2/4] Skipping tests (-SkipTests requested)" -ForegroundColor DarkGray
}

# 4. Generate Portable Package
Write-Host "`n[3/4] Creating Portable Release Package..." -ForegroundColor Yellow
$PortableZip = Join-Path $BuildDir "portable\EBUninstaller_Pro_Portable.zip"

if (-not (Test-Path $OutputDir)) {
    if (Test-Path (Join-Path $RepoRoot "bin\$Configuration\$Platform")) {
        $OutputDir = Join-Path $RepoRoot "bin\$Configuration\$Platform"
    }
}

if (Test-Path $OutputDir) {
    Compress-Archive -Path "$OutputDir\*" -DestinationPath $PortableZip -Force
    Write-Host "Portable package created: $PortableZip" -ForegroundColor Green
} else {
    Write-Warning "Output directory not found at $OutputDir"
}

# 5. Build Installer if requested
if ($BuildInstaller) {
    Write-Host "`n[4/4] Compiling Inno Setup Installer..." -ForegroundColor Yellow
    $IssScript = Join-Path $RepoRoot "installer\EBUninstallSetup.iss"

    $PossibleInnoPaths = @(
        $InnoSetupPath,
        (Get-Command iscc.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source -ErrorAction SilentlyContinue),
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe",
        "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
        "C:\ProgramData\chocolatey\bin\iscc.exe",
        "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
    ) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and (Test-Path $_) }

    if ($PossibleInnoPaths.Count -gt 0) {
        $InnoCompiler = $PossibleInnoPaths[0]
        Write-Host " -> Using Inno Setup compiler: $InnoCompiler" -ForegroundColor Cyan
        & $InnoCompiler $IssScript
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Installer compilation completed successfully: build\installer\" -ForegroundColor Green
        } else {
            Write-Warning "Inno Setup compilation finished with exit code $LASTEXITCODE"
        }
    } else {
        Write-Warning "Inno Setup compiler (ISCC.exe) not found. Inno script is ready at: $IssScript"
    }
}

# 6. Static Analysis Verification if requested
if ($RunVerify) {
    Write-Host "`nRunning Architecture & Static Quality Verification..." -ForegroundColor Yellow
    if (Get-Command python -ErrorAction SilentlyContinue) {
        python (Join-Path $RepoRoot "scripts\verify_repo.py")
    } elseif (Get-Command python3 -ErrorAction SilentlyContinue) {
        python3 (Join-Path $RepoRoot "scripts\verify_repo.py")
    }
}

# 7. Generate SHA-256 Checksums for Release
Write-Host "`nGenerating SHA-256 Checksums..." -ForegroundColor Yellow
$SumsFile = Join-Path $BuildDir "SHA256SUMS.txt"
$ReleaseFiles = Get-ChildItem -Path $BuildDir -Recurse -File -Exclude "SHA256SUMS.txt"
if ($ReleaseFiles.Count -gt 0) {
    $Checksums = foreach ($file in $ReleaseFiles) {
        $hash = (Get-FileHash -Path $file.FullName -Algorithm SHA256).Hash.ToLower()
        $relPath = $file.FullName.Substring($BuildDir.Length + 1)
        "$hash  $relPath"
    }
    $Checksums | Set-Content -Path $SumsFile -Encoding UTF8
    Write-Host "Checksums written to: $SumsFile" -ForegroundColor Green
}

Write-Host "`n=================================================================" -ForegroundColor Green
Write-Host " [SUCCESS] Build process completed successfully!                 " -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green
