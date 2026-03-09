#!/bin/bash

echo "========================================="
echo "🚀 VIRA Emulator Launcher"
echo "========================================="
echo ""

# Check if emulator is running
echo "📱 Checking for running emulator..."
DEVICE=$(adb devices | grep -w "device" | head -1 | awk '{print $1}')

if [ -z "$DEVICE" ]; then
    echo "❌ No emulator detected!"
    echo ""
    echo "Please start Android emulator first:"
    echo "  1. Open Android Studio"
    echo "  2. Go to Tools > Device Manager"
    echo "  3. Start an emulator"
    echo ""
    echo "Or run from command line:"
    echo "  \$ANDROID_HOME/emulator/emulator -avd <avd_name> &"
    echo ""
    exit 1
fi

echo "✅ Emulator found: $DEVICE"
echo ""

# Install APK
echo "📦 Installing VIRA APK..."
adb -s $DEVICE install -r VIRA-Release-Fixed.apk

if [ $? -eq 0 ]; then
    echo "✅ APK installed successfully!"
    echo ""
    
    # Launch app
    echo "🚀 Launching VIRA..."
    adb -s $DEVICE shell am start -n com.vira.assistant/com.vira.assistant.MainActivity
    
    echo ""
    echo "========================================="
    echo "✅ VIRA is now running!"
    echo "========================================="
    echo ""
    echo "📋 Useful commands:"
    echo "  - View logs: adb logcat | grep VIRA"
    echo "  - Uninstall: adb uninstall com.vira.assistant"
    echo "  - Restart: adb shell am force-stop com.vira.assistant && adb shell am start -n com.vira.assistant/com.vira.assistant.MainActivity"
    echo ""
else
    echo "❌ Failed to install APK"
    exit 1
fi
