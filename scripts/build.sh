#!/usr/bin/env bash
# ==============================================================================
# EBUninstaller Pro - Cross-Platform Build & Verification Runner
# ==============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
BUILD_DIR="$REPO_ROOT/build"
PORTABLE_DIR="$BUILD_DIR/portable"
INSTALLER_DIR="$BUILD_DIR/installer"

CONFIG="Release"
PLATFORM="Any CPU"
SKIP_TESTS=false
CLEAN=false
VERBOSE=false

show_help() {
    cat << EOF
Usage: ./scripts/build.sh [OPTIONS]

Options:
  -c, --configuration <Config>   Build configuration (Release, Debug). Default: Release
  -p, --platform <Platform>      Build platform (AnyCPU, x64, ARM64, x86). Default: Any CPU
  -s, --skip-tests               Skip running unit and integration tests
  --clean                        Clean build and output directories prior to compilation
  -v, --verbose                  Enable verbose logging
  -h, --help                     Show this help message
EOF
    exit 0
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        -c|--configuration)
            CONFIG="$2"
            shift 2
            ;;
        -p|--platform)
            PLATFORM="$2"
            shift 2
            ;;
        -s|--skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        -h|--help)
            show_help
            ;;
        *)
            echo "Unknown argument: $1"
            show_help
            ;;
    esac
done

echo "================================================================="
echo " EBUninstaller Pro - Build & Quality Verification Runner         "
echo " Configuration: $CONFIG | Platform: $PLATFORM"
echo "================================================================="

if [ "$CLEAN" = true ]; then
    echo "[Clean] Removing previous build output..."
    rm -rf "$BUILD_DIR"
fi

mkdir -p "$PORTABLE_DIR" "$INSTALLER_DIR"

# 1. Compile Solution if dotnet is available
if command -v dotnet &> /dev/null; then
    echo "[1/4] Compiling EBUninstaller Pro solution with dotnet CLI..."
    SLN_FILE="$REPO_ROOT/source/EBUninstaller.sln"
    if [ ! -f "$SLN_FILE" ]; then
        SLN_FILE="$REPO_ROOT/source/EBUninstaller.sln"
    fi
    dotnet build "$SLN_FILE" \
        --configuration "$CONFIG" \
        -p:Platform="$PLATFORM" \
        -p:Version="7.0.0"

    # 2. Execute Tests
    if [ "$SKIP_TESTS" = false ]; then
        echo "[2/4] Executing Unit & Integration Test Suite..."
        TEST_PROJ="$REPO_ROOT/source/EBUninstallerTests/EBUninstallerTests.csproj"
        if [ ! -f "$TEST_PROJ" ]; then
            TEST_PROJ="$REPO_ROOT/source/BulkCrapUninstallerTests/BulkCrapUninstallerTests.csproj"
        fi
        dotnet test "$TEST_PROJ" \
            --configuration "$CONFIG" \
            --no-build \
            --logger "console;verbosity=normal"
    else
        echo "[2/4] Skipping tests (--skip-tests requested)."
    fi

    # 3. Create Portable Release Archive
    BIN_DIR="$REPO_ROOT/bin/$CONFIG/AnyCPU"
    if [ ! -d "$BIN_DIR" ] && [ -d "$REPO_ROOT/bin/$CONFIG/$PLATFORM" ]; then
        BIN_DIR="$REPO_ROOT/bin/$CONFIG/$PLATFORM"
    fi

    if [ -d "$BIN_DIR" ]; then
        echo "[3/4] Creating Portable Package..."
        (cd "$BIN_DIR" && zip -r -q "$PORTABLE_DIR/EBUninstaller_Pro_Portable.zip" ./*)
        echo " -> Portable ZIP created: build/portable/EBUninstaller_Pro_Portable.zip"
    fi
else
    echo "[INFO] dotnet CLI not found in current environment."
    echo "       Skipping native binary compilation; proceeding to repository analysis."
fi

# 4. Run Static Analysis & Verification Suite
echo "[4/4] Executing static analysis and repository architecture verification..."
VERIFY_ARGS=()
if [ "$VERBOSE" = true ]; then
    VERIFY_ARGS+=("-v")
fi
python3 "$REPO_ROOT/scripts/verify_repo.py" "${VERIFY_ARGS[@]}"

# 5. Generate Checksums if artifacts exist in build/
if [ -d "$BUILD_DIR" ] && [ "$(ls -A "$BUILD_DIR/portable" "$BUILD_DIR/installer" 2>/dev/null)" ]; then
    echo "Generating SHA-256 digests for release artifacts..."
    (
        cd "$BUILD_DIR"
        find . -type f ! -name "SHA256SUMS.txt" -exec sha256sum {} + > SHA256SUMS.txt
    )
    echo " -> SHA-256 digests written to: build/SHA256SUMS.txt"
fi

echo ""
echo "================================================================="
echo " [SUCCESS] Build & Verification runner finished successfully!    "
echo "================================================================="
