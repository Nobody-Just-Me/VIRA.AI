#!/bin/bash

# Script untuk setup Android Emulator via command line

set -e

echo "📱 Android Emulator Setup"
echo "========================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check if Android SDK is installed
if [ ! -d "$HOME/Android/Sdk" ]; then
    echo -e "${YELLOW}⚠️  Android SDK not found${NC}"
    echo "Please run Android Studio first to install SDK"
    echo "Or set ANDROID_HOME manually"
    exit 1
fi

export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/emulator:$ANDROID_HOME/platform-tools:$ANDROID_HOME/cmdline-tools/latest/bin

echo -e "${GREEN}✅ Android SDK found: $ANDROID_HOME${NC}"
echo ""

# Check if sdkmanager exists
if [ ! -f "$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager" ]; then
    echo -e "${YELLOW}Installing command line tools...${NC}"
    mkdir -p "$ANDROID_HOME/cmdline-tools"
    cd /tmp
    wget https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip
    unzip -q commandlinetools-linux-*.zip -d "$ANDROID_HOME/cmdline-tools"
    mv "$ANDROID_HOME/cmdline-tools/cmdline-tools" "$ANDROID_HOME/cmdline-tools/latest"
fi

echo -e "${BLUE}📋 Step 1: Installing system image${NC}"
echo ""

# Accept licenses
yes | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --licenses 2>/dev/null || true

# Install system image
echo "Installing Android 14 (API 34) system image..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager "system-images;android-34;google_apis_playstore;x86_64"

echo ""
echo -e "${BLUE}📋 Step 2: Creating AVD${NC}"
echo ""

# Check if AVD already exists
AVD_NAME="Pixel_7_API_34"

if $ANDROID_HOME/emulator/emulator -list-avds | grep -q "$AVD_NAME"; then
    echo -e "${YELLOW}AVD '$AVD_NAME' already exists${NC}"
    read -p "Delete and recreate? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        $ANDROID_HOME/cmdline-tools/latest/bin/avdmanager delete avd -n "$AVD_NAME"
    else
        echo "Using existing AVD"
        AVD_EXISTS=true
    fi
fi

if [ "$AVD_EXISTS" != "true" ]; then
    # Create AVD
    echo "Creating Pixel 7 emulator..."
    echo "no" | $ANDROID_HOME/cmdline-tools/latest/bin/avdmanager create avd \
        -n "$AVD_NAME" \
        -k "system-images;android-34;google_apis_playstore;x86_64" \
        -d "pixel_7"
fi

echo ""
echo -e "${GREEN}✅ Android Emulator setup complete!${NC}"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${BLUE}📱 How to use:${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. Start emulator:"
echo "   ./start-emulator.sh"
echo ""
echo "2. Or manually:"
echo "   \$ANDROID_HOME/emulator/emulator -avd $AVD_NAME"
echo ""
echo "3. Install VIRA APK:"
echo "   adb install -r VIRA-Release.apk"
echo ""
echo "4. Or use automated script:"
echo "   ./test-vira-emulator.sh"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
