#!/bin/bash

# Script untuk install Android Studio dan setup emulator

set -e

echo "📱 Android Studio & Emulator Installation"
echo "=========================================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check if Android Studio already installed
if [ -d "$HOME/android-studio" ] || command -v studio.sh &> /dev/null; then
    echo -e "${GREEN}✅ Android Studio already installed${NC}"
    echo ""
    read -p "Reinstall? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Skipping installation"
        exit 0
    fi
fi

echo -e "${BLUE}📋 Step 1: Installing dependencies${NC}"
echo ""

# Install dependencies
sudo apt update
sudo apt install -y \
    openjdk-17-jdk \
    libc6:i386 \
    libncurses5:i386 \
    libstdc++6:i386 \
    lib32z1 \
    libbz2-1.0:i386 \
    qemu-kvm \
    libvirt-daemon-system \
    libvirt-clients \
    bridge-utils

echo ""
echo -e "${BLUE}📋 Step 2: Downloading Android Studio${NC}"
echo ""

# Download Android Studio
cd /tmp
ANDROID_STUDIO_URL="https://redirector.gvt1.com/edgedl/android/studio/ide-zips/2024.2.1.12/android-studio-2024.2.1.12-linux.tar.gz"

echo "Downloading Android Studio..."
echo -e "${YELLOW}⏳ This may take a while (~1 GB)...${NC}"

wget -O android-studio.tar.gz "$ANDROID_STUDIO_URL" || {
    echo "Download failed. Trying alternative URL..."
    wget -O android-studio.tar.gz "https://dl.google.com/dl/android/studio/ide-zips/2024.2.1.12/android-studio-2024.2.1.12-linux.tar.gz"
}

echo ""
echo -e "${BLUE}📋 Step 3: Installing Android Studio${NC}"
echo ""

# Extract to home directory
tar -xzf android-studio.tar.gz -C "$HOME/"

echo ""
echo -e "${BLUE}📋 Step 4: Setting up environment${NC}"
echo ""

# Add to PATH
if ! grep -q "android-studio/bin" ~/.bashrc; then
    echo "" >> ~/.bashrc
    echo "# Android Studio" >> ~/.bashrc
    echo "export PATH=\$PATH:\$HOME/android-studio/bin" >> ~/.bashrc
fi

# Set ANDROID_HOME if not already set
if ! grep -q "ANDROID_HOME" ~/.bashrc; then
    echo "export ANDROID_HOME=\$HOME/Android/Sdk" >> ~/.bashrc
    echo "export PATH=\$PATH:\$ANDROID_HOME/emulator" >> ~/.bashrc
    echo "export PATH=\$PATH:\$ANDROID_HOME/platform-tools" >> ~/.bashrc
fi

# Add user to kvm group for hardware acceleration
sudo usermod -aG kvm $USER

echo ""
echo -e "${GREEN}✅ Android Studio installed successfully!${NC}"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${BLUE}📱 Next Steps:${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. Launch Android Studio:"
echo "   ~/android-studio/bin/studio.sh"
echo ""
echo "2. Complete the setup wizard:"
echo "   - Choose 'Standard' installation"
echo "   - Accept licenses"
echo "   - Wait for SDK download"
echo ""
echo "3. Create an emulator:"
echo "   - Tools → Device Manager"
echo "   - Create Device"
echo "   - Choose device (e.g., Pixel 8)"
echo "   - Choose system image (API 34 recommended)"
echo "   - Finish"
echo ""
echo "4. Or use the automated script:"
echo "   ./setup-android-emulator.sh"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo -e "${YELLOW}⚠️  Important:${NC}"
echo "- Logout and login again for group changes to take effect"
echo "- Or run: newgrp kvm"
echo ""
echo -e "${GREEN}Ready to launch Android Studio!${NC}"
echo ""

read -p "Launch Android Studio now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "Launching Android Studio..."
    ~/android-studio/bin/studio.sh &
    echo ""
    echo "Android Studio is starting..."
    echo "Complete the setup wizard and then run: ./setup-android-emulator.sh"
fi
