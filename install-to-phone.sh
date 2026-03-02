#!/bin/bash

echo "📱 VIRA - Install to Android Phone"
echo "===================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if APK exists
if [ ! -f "VIRA-Release.apk" ]; then
    echo -e "${RED}❌ VIRA-Release.apk not found!${NC}"
    echo "Please run ./build.sh first"
    exit 1
fi

echo -e "${GREEN}✅ APK found: VIRA-Release.apk ($(du -h VIRA-Release.apk | cut -f1))${NC}"
echo ""

# Check ADB
if ! command -v ~/Android/Sdk/platform-tools/adb &> /dev/null; then
    echo -e "${RED}❌ ADB not found!${NC}"
    echo "Please install Android SDK first"
    exit 1
fi

echo -e "${GREEN}✅ ADB found${NC}"
echo ""

# Check device connection
echo "🔍 Checking for connected devices..."
DEVICES=$(~/Android/Sdk/platform-tools/adb devices | grep -w "device" | wc -l)

if [ "$DEVICES" -eq 0 ]; then
    echo -e "${RED}❌ No Android device connected!${NC}"
    echo ""
    echo "Please:"
    echo "1. Enable USB Debugging on your phone"
    echo "2. Connect phone to laptop via USB cable"
    echo "3. Allow USB Debugging when prompted"
    echo "4. Run this script again"
    echo ""
    echo "Or use manual install method (see INSTALL_KE_ANDROID.md)"
    exit 1
fi

echo -e "${GREEN}✅ Device connected${NC}"
~/Android/Sdk/platform-tools/adb devices
echo ""

# Ask for confirmation
read -p "Install VIRA to connected device? (y/n) " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Installation cancelled"
    exit 0
fi

# Uninstall old version (if exists)
echo ""
echo "🗑️  Checking for old version..."
if ~/Android/Sdk/platform-tools/adb shell pm list packages | grep -q "com.vira.assistant"; then
    echo -e "${YELLOW}⚠️  Old version found, uninstalling...${NC}"
    ~/Android/Sdk/platform-tools/adb uninstall com.vira.assistant
    echo -e "${GREEN}✅ Old version uninstalled${NC}"
else
    echo "No old version found"
fi

# Install APK
echo ""
echo "📦 Installing VIRA..."
if ~/Android/Sdk/platform-tools/adb install -r VIRA-Release.apk; then
    echo ""
    echo -e "${GREEN}✅ VIRA installed successfully!${NC}"
else
    echo ""
    echo -e "${RED}❌ Installation failed!${NC}"
    echo "Please check the error message above"
    exit 1
fi

# Launch app
echo ""
read -p "Launch VIRA now? (y/n) " -n 1 -r
echo ""
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🚀 Launching VIRA..."
    ~/Android/Sdk/platform-tools/adb shell monkey -p com.vira.assistant -c android.intent.category.LAUNCHER 1 > /dev/null 2>&1
    echo -e "${GREEN}✅ VIRA launched!${NC}"
    echo ""
    echo "Check your phone now!"
fi

# Show logs option
echo ""
read -p "Show live logs? (y/n) " -n 1 -r
echo ""
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "📋 Showing VIRA logs (Ctrl+C to stop)..."
    echo ""
    ~/Android/Sdk/platform-tools/adb logcat -c
    ~/Android/Sdk/platform-tools/adb logcat | grep -E "VIRA_|AndroidRuntime"
else
    echo ""
    echo "🎉 Installation complete!"
    echo ""
    echo "Next steps:"
    echo "1. Open VIRA on your phone"
    echo "2. Allow Microphone and Internet permissions"
    echo "3. Go to Settings (⚙️) and enter your Gemini API Key"
    echo "4. Test by sending: 'What's the weather like today?'"
    echo ""
    echo "For testing guide, see: CARA_TEST_RICH_CARDS.md"
fi
