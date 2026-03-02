#!/bin/bash

# Script untuk install dan setup Waydroid untuk testing VIRA
# Ubuntu 24.04 LTS

set -e

echo "🚀 VIRA - Waydroid Installation Script"
echo "======================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if running on Ubuntu
if ! grep -q "Ubuntu" /etc/os-release; then
    echo -e "${RED}❌ This script is designed for Ubuntu${NC}"
    echo "For other distros, check INSTALL_WAYDROID.md"
    exit 1
fi

echo -e "${BLUE}📋 Step 1: Installing Waydroid${NC}"
echo ""

# Check if Waydroid already installed
if command -v waydroid &> /dev/null; then
    echo -e "${GREEN}✅ Waydroid already installed${NC}"
else
    echo "Installing Waydroid..."
    
    # Install dependencies
    sudo apt update
    sudo apt install -y curl ca-certificates
    
    # Add Waydroid repository
    echo "Adding Waydroid repository..."
    curl https://repo.waydro.id | sudo bash
    
    # Install Waydroid
    echo "Installing Waydroid package..."
    sudo apt install -y waydroid
    
    echo -e "${GREEN}✅ Waydroid installed${NC}"
fi

echo ""
echo -e "${BLUE}📋 Step 2: Checking kernel modules${NC}"
echo ""

# Check and load binder module
if ! lsmod | grep -q binder; then
    echo "Loading binder module..."
    sudo modprobe binder_linux devices="binder,hwbinder,vndbinder"
    
    # Make it persistent
    echo "binder_linux" | sudo tee /etc/modules-load.d/waydroid.conf
    echo -e "${GREEN}✅ Binder module loaded${NC}"
else
    echo -e "${GREEN}✅ Binder module already loaded${NC}"
fi

echo ""
echo -e "${BLUE}📋 Step 3: Initializing Waydroid${NC}"
echo ""

# Check if Waydroid is already initialized
if [ -d "/var/lib/waydroid" ] && [ -f "/var/lib/waydroid/waydroid.cfg" ]; then
    echo -e "${GREEN}✅ Waydroid already initialized${NC}"
else
    echo "Initializing Waydroid with Google Apps..."
    echo -e "${YELLOW}⏳ This will download ~1-2 GB, please wait...${NC}"
    
    sudo waydroid init -s GAPPS -f
    
    echo -e "${GREEN}✅ Waydroid initialized${NC}"
fi

echo ""
echo -e "${BLUE}📋 Step 4: Starting Waydroid${NC}"
echo ""

# Start Waydroid container
echo "Starting Waydroid container..."
sudo waydroid container start

# Wait a bit for container to start
sleep 3

# Start Waydroid session in background
echo "Starting Waydroid session..."
waydroid session start &
SESSION_PID=$!

# Wait for session to be ready
sleep 5

echo -e "${GREEN}✅ Waydroid started${NC}"

echo ""
echo -e "${BLUE}📋 Step 5: Installing VIRA APK${NC}"
echo ""

APK_PATH="$HOME/Test ai/VIRA-UNO/VIRA-Release.apk"

if [ -f "$APK_PATH" ]; then
    echo "Installing VIRA APK..."
    waydroid app install "$APK_PATH"
    echo -e "${GREEN}✅ VIRA APK installed${NC}"
else
    echo -e "${YELLOW}⚠️  APK not found at: $APK_PATH${NC}"
    echo "Please build the APK first with: ./build.sh"
fi

echo ""
echo -e "${GREEN}🎉 Installation Complete!${NC}"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${BLUE}📱 How to use Waydroid:${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. Show Waydroid UI:"
echo "   waydroid show-full-ui"
echo ""
echo "2. Launch VIRA:"
echo "   waydroid app launch com.vira.assistant"
echo ""
echo "3. View logs:"
echo "   waydroid logcat | grep 'com.vira.assistant'"
echo ""
echo "4. Stop Waydroid:"
echo "   waydroid session stop"
echo "   sudo waydroid container stop"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo -e "${YELLOW}💡 Tip: Waydroid window can be resized like any normal window${NC}"
echo ""
echo -e "${GREEN}Ready to test VIRA! 🚀${NC}"
echo ""

# Ask if user wants to launch VIRA now
read -p "Launch VIRA now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "Launching VIRA..."
    waydroid app launch com.vira.assistant
fi
