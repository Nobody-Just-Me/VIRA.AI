#!/bin/bash

# Script untuk install dan test VIRA di HP Android via ADB

echo "📱 VIRA - Test on Android Phone"
echo "================================"
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Check if adb is available
if ! command -v adb &> /dev/null; then
    echo -e "${RED}❌ ADB not found${NC}"
    echo ""
    echo "ADB is part of Android SDK platform-tools."
    echo "It should be at: $ANDROID_HOME/platform-tools/adb"
    echo ""
    echo "Add to PATH:"
    echo "export PATH=\$PATH:\$ANDROID_HOME/platform-tools"
    exit 1
fi

echo -e "${GREEN}✅ ADB found${NC}"
echo ""

# Check if device is connected
echo "🔍 Checking for connected devices..."
DEVICES=$(adb devices | grep -v "List" | grep "device$" | wc -l)

if [ $DEVICES -eq 0 ]; then
    echo -e "${YELLOW}⚠️  No device connected${NC}"
    echo ""
    echo "Please:"
    echo "1. Enable USB Debugging on your phone:"
    echo "   Settings → About Phone → Tap 'Build Number' 7 times"
    echo "   Settings → Developer Options → Enable 'USB Debugging'"
    echo ""
    echo "2. Connect your phone via USB"
    echo ""
    echo "3. Allow USB debugging popup on your phone"
    echo ""
    echo "4. Run this script again"
    exit 1
fi

echo -e "${GREEN}✅ Device connected${NC}"
adb devices
echo ""

# Check if APK exists
APK_PATH="$HOME/Test ai/VIRA-UNO/VIRA-Release.apk"

if [ ! -f "$APK_PATH" ]; then
    echo -e "${RED}❌ APK not found at: $APK_PATH${NC}"
    echo ""
    echo "Please build the APK first:"
    echo "  cd ~/Test\\ ai/VIRA-UNO"
    echo "  ./build.sh"
    exit 1
fi

echo -e "${GREEN}✅ APK found${NC}"
echo "   Size: $(du -h "$APK_PATH" | cut -f1)"
echo ""

# Ask user what to do
echo "What would you like to do?"
echo ""
echo "1. Install APK"
echo "2. Install and Launch"
echo "3. Uninstall old version first, then install"
echo "4. View logs"
echo "5. Take screenshot"
echo ""
read -p "Choice (1-5): " choice

case $choice in
    1)
        echo ""
        echo "📦 Installing APK..."
        adb install -r "$APK_PATH"
        echo ""
        echo -e "${GREEN}✅ Installed!${NC}"
        echo "Open VIRA app on your phone to test"
        ;;
    2)
        echo ""
        echo "📦 Installing APK..."
        adb install -r "$APK_PATH"
        echo ""
        echo "🚀 Launching VIRA..."
        adb shell am start -n com.vira.assistant/.MainActivity
        echo ""
        echo -e "${GREEN}✅ VIRA launched!${NC}"
        echo "Check your phone"
        ;;
    3)
        echo ""
        echo "🗑️  Uninstalling old version..."
        adb uninstall com.vira.assistant 2>/dev/null || echo "No previous version found"
        echo ""
        echo "📦 Installing new APK..."
        adb install "$APK_PATH"
        echo ""
        echo "🚀 Launching VIRA..."
        adb shell am start -n com.vira.assistant/.MainActivity
        echo ""
        echo -e "${GREEN}✅ Done!${NC}"
        ;;
    4)
        echo ""
        echo "📋 Viewing logs (Ctrl+C to stop)..."
        echo ""
        adb logcat | grep --color=always "com.vira.assistant"
        ;;
    5)
        echo ""
        echo "📸 Taking screenshot..."
        adb shell screencap -p /sdcard/vira-screenshot.png
        adb pull /sdcard/vira-screenshot.png ~/vira-screenshot.png
        echo ""
        echo -e "${GREEN}✅ Screenshot saved to: ~/vira-screenshot.png${NC}"
        ;;
    *)
        echo "Invalid choice"
        exit 1
        ;;
esac

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "${BLUE}📱 Useful ADB Commands:${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "View logs:"
echo "  adb logcat | grep 'com.vira.assistant'"
echo ""
echo "Launch app:"
echo "  adb shell am start -n com.vira.assistant/.MainActivity"
echo ""
echo "Uninstall:"
echo "  adb uninstall com.vira.assistant"
echo ""
echo "Screenshot:"
echo "  adb shell screencap -p /sdcard/screenshot.png"
echo "  adb pull /sdcard/screenshot.png ~/"
echo ""
echo "Record video:"
echo "  adb shell screenrecord /sdcard/demo.mp4"
echo "  # Press Ctrl+C to stop"
echo "  adb pull /sdcard/demo.mp4 ~/"
echo ""
