#!/bin/bash

# Build VIRA terbaru dan jalankan di emulator

set -e

GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${GREEN}🚀 Build & Run VIRA (Latest Version)${NC}"
echo "======================================"
echo ""

# Step 1: Set Android SDK
if [ -z "$ANDROID_HOME" ]; then
    if [ -d "$HOME/Android/Sdk" ]; then
        export ANDROID_HOME="$HOME/Android/Sdk"
        echo -e "${BLUE}Setting ANDROID_HOME: $ANDROID_HOME${NC}"
    else
        echo -e "${RED}❌ Android SDK not found${NC}"
        echo "Please install Android SDK first: ./install-android-emulator.sh"
        exit 1
    fi
fi

# Step 2: Clean and restore
echo -e "${BLUE}Step 1: Cleaning previous builds...${NC}"
dotnet clean VIRA.Mobile/VIRA.Mobile.csproj -c Release > /dev/null 2>&1 || true

echo -e "${BLUE}Step 2: Restoring dependencies...${NC}"
dotnet restore

# Step 3: Build APK
echo -e "${BLUE}Step 3: Building APK (this may take 2-3 minutes)...${NC}"
dotnet publish VIRA.Mobile/VIRA.Mobile.csproj \
    -f net8.0-android \
    -c Release \
    -p:AndroidPackageFormat=apk \
    -p:AndroidSdkDirectory=$ANDROID_HOME

# Step 4: Find and copy APK
echo -e "${BLUE}Step 4: Locating APK...${NC}"
APK_FILE=$(find VIRA.Mobile/bin/Release/net8.0-android -name "*.apk" -type f | head -n 1)

if [ -n "$APK_FILE" ]; then
    cp "$APK_FILE" "./VIRA-Release.apk"
    APK_SIZE=$(du -h "./VIRA-Release.apk" | cut -f1)
    echo -e "${GREEN}✅ APK built successfully: $APK_SIZE${NC}"
else
    echo -e "${RED}❌ APK not found${NC}"
    exit 1
fi

# Step 5: Check if emulator is running
echo ""
echo -e "${BLUE}Step 5: Checking emulator...${NC}"

export PATH="$PATH:$ANDROID_HOME/platform-tools"
export PATH="$PATH:$ANDROID_HOME/emulator"

# Check if adb can find device
if ! $ANDROID_HOME/platform-tools/adb devices | grep -q "emulator"; then
    echo -e "${YELLOW}⚠️  No emulator running${NC}"
    echo ""
    echo "Starting emulator..."
    
    # Start emulator in background
    $ANDROID_HOME/emulator/emulator -avd VIRA_Pixel_6 -gpu auto -no-snapshot-load &
    EMULATOR_PID=$!
    
    echo "Waiting for emulator to boot (this may take 1-2 minutes)..."
    $ANDROID_HOME/platform-tools/adb wait-for-device
    sleep 15
    echo -e "${GREEN}✅ Emulator ready${NC}"
else
    echo -e "${GREEN}✅ Emulator already running${NC}"
fi

# Step 6: Install APK
echo ""
echo -e "${BLUE}Step 6: Installing VIRA...${NC}"
$ANDROID_HOME/platform-tools/adb install -r ./VIRA-Release.apk

# Step 7: Launch VIRA
echo -e "${BLUE}Step 7: Launching VIRA...${NC}"
sleep 2
$ANDROID_HOME/platform-tools/adb shell am start -n com.vira.assistant/.MainActivity

echo ""
echo -e "${GREEN}✅ VIRA (Latest Version) is now running!${NC}"
echo ""
echo -e "${YELLOW}Setup steps:${NC}"
echo "  1. Grant permissions in VIRA"
echo "  2. Open Settings → Add API key"
echo "  3. Test voice commands"
echo ""
echo -e "${BLUE}View logs:${NC}"
echo "  adb logcat | grep VIRA"
echo ""
