#!/bin/bash

# Script untuk menjalankan VIRA di emulator Android

echo "🚀 Starting VIRA on Android Emulator"
echo "====================================="
echo ""

export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/emulator:$ANDROID_HOME/platform-tools

# Check if emulator exists
if [ ! -f "$ANDROID_HOME/emulator/emulator" ]; then
    echo "❌ Android Emulator not found"
    echo "Please run: ./setup-android-emulator.sh"
    exit 1
fi

# Get AVD name
AVD_NAME=$($ANDROID_HOME/emulator/emulator -list-avds | head -n 1)

if [ -z "$AVD_NAME" ]; then
    echo "❌ No emulator found"
    echo "Please run: ./setup-android-emulator.sh"
    exit 1
fi

echo "📱 Emulator: $AVD_NAME"
echo ""

# Check if APK exists
APK_PATH="VIRA-Release.apk"
if [ ! -f "$APK_PATH" ]; then
    echo "❌ APK not found: $APK_PATH"
    echo "Building APK..."
    ./build.sh
fi

echo "🎮 Starting emulator..."
echo "   This will open in a new window"
echo "   First boot may take 2-3 minutes"
echo ""

# Start emulator in background
$ANDROID_HOME/emulator/emulator -avd "$AVD_NAME" -gpu auto &
EMULATOR_PID=$!

echo "⏳ Waiting for emulator to start..."
adb wait-for-device
echo "✅ Emulator device detected"
echo ""

echo "⏳ Waiting for Android to boot..."
while [ "$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" != "1" ]; do
    echo -n "."
    sleep 2
done
echo ""
echo "✅ Android boot complete"
echo ""

# Wait a bit more for system to stabilize
sleep 5

echo "📦 Installing VIRA APK..."
adb install -r "$APK_PATH"
echo ""

echo "🚀 Launching VIRA..."
adb shell am start -n com.vira.assistant/.MainActivity
echo ""

echo "✅ VIRA is running!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📱 Emulator is running in the background"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "💡 Useful commands:"
echo "   - View logs: adb logcat | grep 'com.vira.assistant'"
echo "   - Restart app: adb shell am start -n com.vira.assistant/.MainActivity"
echo "   - Stop emulator: adb emu kill"
echo ""
echo "Press Ctrl+C to stop monitoring (emulator will keep running)"
echo ""

# Keep script running and show logs
adb logcat | grep --line-buffered "com.vira.assistant\|AndroidRuntime"
