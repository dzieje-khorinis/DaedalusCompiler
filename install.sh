#!/bin/bash
# Daedalus Compiler Installation Script
# Usage: curl -fsSL https://raw.githubusercontent.com/dzieje-khorinis/DaedalusCompiler/main/install.sh | bash

set -e

# Configuration
REPO="dzieje-khorinis/DaedalusCompiler"
BINARY_NAME="gdc"
INSTALL_DIR="/usr/local/bin"
FALLBACK_DIR="$HOME/.local/bin"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Helper functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Detect operating system
detect_os() {
    case "$(uname -s)" in
        Linux*)     echo "linux";;
        Darwin*)    echo "macos";;
        CYGWIN*|MINGW*|MSYS*) echo "windows";;
        *)          echo "unknown";;
    esac
}

# Detect CPU architecture
detect_arch() {
    case "$(uname -m)" in
        x86_64|amd64)   echo "x64";;
        aarch64|arm64)  echo "arm64";;
        armv7l)         echo "arm64";; # Fallback for some ARM systems
        *)              echo "unknown";;
    esac
}

# Get the latest release version from GitHub API
get_latest_version() {
    local api_url="https://api.github.com/repos/$REPO/releases/latest"
    
    if command -v curl >/dev/null 2>&1; then
        curl -s "$api_url" | grep '"tag_name":' | sed -E 's/.*"tag_name": "([^"]+)".*/\1/'
    elif command -v wget >/dev/null 2>&1; then
        wget -qO- "$api_url" | grep '"tag_name":' | sed -E 's/.*"tag_name": "([^"]+)".*/\1/'
    else
        log_error "Neither curl nor wget is available. Please install one of them." >&2
        return 1
    fi
}

# Construct the download URL for the binary
get_download_url() {
    local os="$1"
    local arch="$2"
    local version="$3"
    local asset_name
    
    case "$os" in
        "linux")    asset_name="gdc-linux-${arch}";;
        "macos")    asset_name="gdc-macos-${arch}";;
        "windows")  asset_name="gdc-windows-${arch}.exe";;
        *)          return 1;;
    esac
    
    echo "https://github.com/$REPO/releases/download/$version/$asset_name"
}

# Download file with progress
download_file() {
    local url="$1"
    local output="$2"
    
    log_info "Downloading from: $url"
    
    if command -v curl >/dev/null 2>&1; then
        curl -fsSL --progress-bar "$url" -o "$output"
    elif command -v wget >/dev/null 2>&1; then
        wget --progress=bar:force -O "$output" "$url"
    else
        log_error "Neither curl nor wget is available. Please install one of them."
        exit 1
    fi
}

# Check if we have write permission to a directory
can_write_to() {
    local dir="$1"
    [ -d "$dir" ] && [ -w "$dir" ]
}

# Create directory if it doesn't exist
ensure_dir() {
    local dir="$1"
    if [ ! -d "$dir" ]; then
        mkdir -p "$dir"
    fi
}

# Add directory to PATH if not already present
add_to_path() {
    local dir="$1"
    local shell_rc=""
    
    # Detect shell and appropriate RC file
    case "$SHELL" in
        */bash)  shell_rc="$HOME/.bashrc";;
        */zsh)   shell_rc="$HOME/.zshrc";;
        */fish)  shell_rc="$HOME/.config/fish/config.fish";;
        *)       shell_rc="$HOME/.profile";;
    esac
    
    # Check if directory is already in PATH
    if echo "$PATH" | grep -q "$dir"; then
        log_info "Directory $dir is already in PATH"
        return 0
    fi
    
    # Add to PATH in shell RC file
    if [ -f "$shell_rc" ]; then
        echo "" >> "$shell_rc"
        echo "# Daedalus Compiler" >> "$shell_rc"
        if [[ "$shell_rc" == *"fish"* ]]; then
            echo "set -gx PATH $dir \$PATH" >> "$shell_rc"
        else
            echo "export PATH=\"$dir:\$PATH\"" >> "$shell_rc"
        fi
        log_info "Added $dir to PATH in $shell_rc"
        log_warning "Please restart your shell or run: source $shell_rc"
    else
        log_warning "Could not find shell RC file. Please manually add $dir to your PATH"
    fi
}

# Main installation function
main() {
    log_info "🔥 Daedalus Compiler Installer"
    log_info "=============================="
    
    # Detect system
    local os=$(detect_os)
    local arch=$(detect_arch)
    
    log_info "Detected OS: $os"
    log_info "Detected Architecture: $arch"
    
    # Validate platform support
    if [ "$os" = "unknown" ]; then
        log_error "Unsupported operating system: $(uname -s)"
        log_error "Supported: Linux, macOS, Windows (WSL/Cygwin)"
        exit 1
    fi
    
    if [ "$arch" = "unknown" ]; then
        log_error "Unsupported architecture: $(uname -m)"
        log_error "Supported: x64, arm64"
        exit 1
    fi
    
    # Get latest version
    log_info "Fetching latest release information..."
    local version=$(get_latest_version)
    
    if [ -z "$version" ]; then
        log_error "Failed to fetch latest version from GitHub API"
        exit 1
    fi
    
    log_info "Latest version: $version"
    
    # Construct download URL
    local download_url=$(get_download_url "$os" "$arch" "$version")
    
    if [ -z "$download_url" ]; then
        log_error "Failed to construct download URL for $os-$arch"
        exit 1
    fi
    
    # Determine installation directory
    local install_dir=""
    local binary_name="$BINARY_NAME"
    
    if [ "$os" = "windows" ]; then
        binary_name="${BINARY_NAME}.exe"
    fi
    
    if can_write_to "$INSTALL_DIR"; then
        install_dir="$INSTALL_DIR"
        log_info "Installing to system directory: $install_dir"
    else
        ensure_dir "$FALLBACK_DIR"
        install_dir="$FALLBACK_DIR"
        log_info "Installing to user directory: $install_dir"
    fi
    
    local binary_path="$install_dir/$binary_name"
    
    # Check if already installed
    if [ -f "$binary_path" ]; then
        log_warning "Daedalus Compiler is already installed at: $binary_path"
        read -p "Do you want to overwrite it? [y/N]: " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            log_info "Installation cancelled."
            exit 0
        fi
    fi
    
    # Create temporary file for download
    local temp_file=$(mktemp)
    trap "rm -f $temp_file" EXIT
    
    # Download binary
    log_info "Downloading Daedalus Compiler $version for $os-$arch..."
    download_file "$download_url" "$temp_file"
    
    # Verify download
    if [ ! -f "$temp_file" ] || [ ! -s "$temp_file" ]; then
        log_error "Download failed or file is empty"
        exit 1
    fi
    
    # Install binary
    log_info "Installing to: $binary_path"
    
    if ! cp "$temp_file" "$binary_path"; then
        log_error "Failed to copy binary to $binary_path"
        log_error "You may need to run this script with sudo for system-wide installation"
        exit 1
    fi
    
    # Set executable permissions
    chmod +x "$binary_path"
    
    # Add to PATH if needed (only for user directory)
    if [ "$install_dir" = "$FALLBACK_DIR" ]; then
        add_to_path "$install_dir"
    fi
    
    # Test installation
    log_info "Testing installation..."
    if "$binary_path" --help >/dev/null 2>&1; then
        log_success "✅ Daedalus Compiler installed successfully!"
        log_success "Version: $version"
        log_success "Location: $binary_path"
        
        if command -v "$binary_name" >/dev/null 2>&1; then
            log_success "Binary is available in PATH as: $binary_name"
        else
            log_warning "Binary is not in PATH. You may need to restart your shell."
        fi
        
        echo ""
        log_info "🎉 Installation complete!"
        log_info "Try running: $binary_name --help"
        
    else
        log_error "Installation verification failed"
        log_error "The binary was installed but doesn't seem to work correctly"
        exit 1
    fi
}

# Check if script is being piped (common with curl | bash)
if [ -t 0 ]; then
    # Running interactively
    log_info "Running Daedalus Compiler installer..."
else
    # Being piped, run automatically
    log_info "Downloading and installing Daedalus Compiler..."
fi

# Run main installation
main "$@"
