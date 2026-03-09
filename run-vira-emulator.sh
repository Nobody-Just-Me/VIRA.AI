#!/bin/bash

# VIRA Emulator Runner Script
# This script starts the emulator, installs VIRA, and launches the app

echo "=========================================="
echo "VIRA Emulator Runner"
echo "=========================================="

# Check if emulator is already running
RUNNING=$(adb devices | grep emulator | wc -l)

if [ $RUNNING -eq 0 ]; then
    echo "📱 Starting Android Emulator..."
    emulator -avd VIRA_Pixel_6 -no-snapshot-load &
    
    echo "⏳ Waiting for emulator to boot..."
    adb wait-for-device
    sleep 10
    
    echo "✅ Emulator is ready!"
else
    echo "✅ Emulator already running"
fi

# Install APK
echo ""
echo "📦 Installing VIRA APK..."
adb install -r VIRA-Release-Fixed.apk

# Launch app
echo ""
echo "🚀 Launching VIRA..."
adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity

echo ""
echo "=========================================="
echo "✅ VIRA is now running in emulator!"
echo "=========================================="
echo ""
echo "Useful commands:"
echo "  - View logs: adb logcat | grep VIRA"
echo "  - Take screenshot: adb shell screencap -p /sdcard/vira.png && adb pull /sdcard/vira.png"
echo "  - Stop app: adb shell am force-stop com.vira.assistant"
echo "  - Uninstall: adb uninstall com.vira.assistant"
echo ""
