#!/bin/bash

# Script untuk test VIRA di Android Emulator

echo "🧪 Testing VIRA on Android Emulator"
echo "===================================="
echo ""

export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/emulator:$ANDROID_HOME/platform-tools

# Check if APK exists
APK_PATH="VIRA-Release.apk"
if [ ! -f "$APK_PATH" ]; then
    echo "❌ APK not found: $APK_PATH"
    echo "Building APK..."
    ./build.sh
fi

echo "Checking emulator status..."

# Wait for emulator to be ready
adb wait-for-device
echo "✅ Emulator is ready"
echo ""

# Wait for boot to complete
echo "Waiting for emulator to fully boot..."
while [ "$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" != "1" ]; do
    sleep 2
done
echo "✅ Emulator boot complete"
echo ""

# Install APK
echo "Installing VIRA APK..."
adb install -r "$APK_PATH"
echo ""

# Launch VIRA
echo "Launching VIRA..."
adb shell am start -n com.vira.assistant/.MainActivity
echo ""

echo "✅ VIRA is running on emulator!"
echo ""
echo "📱 You can now test the app in the emulator window"
echo ""
echo "💡 Useful commands:"
echo "   - View logs: adb logcat | grep 'com.vira.assistant'"
echo "   - Uninstall: adb uninstall com.vira.assistant"
echo "   - Reinstall: adb install -r VIRA-Release.apk"
echo ""
