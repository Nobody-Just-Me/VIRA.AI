#!/bin/bash

echo "🧪 VIRA Testing Script"
echo "====================="
echo ""

# Check if emulator is running
if ! ~/Android/Sdk/platform-tools/adb devices | grep -q "emulator"; then
    echo "❌ No emulator detected!"
    echo "   Please start the emulator first:"
    echo "   ./start-emulator.sh"
    exit 1
fi

echo "✅ Emulator detected"
echo ""

# Build APK
echo "🔨 Building APK..."
./build.sh > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi
echo "✅ Build successful"
echo ""

# Install APK
echo "📲 Installing APK..."
~/Android/Sdk/platform-tools/adb install -r VIRA-Release.apk > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo "❌ Installation failed!"
    exit 1
fi
echo "✅ Installation successful"
echo ""

# Clear logcat
~/Android/Sdk/platform-tools/adb logcat -c

# Launch app
echo "🚀 Launching VIRA..."
~/Android/Sdk/platform-tools/adb shell am start -n com.vira.assistant/crc641dde536e025d9464.MainActivity > /dev/null 2>&1

# Wait a bit
sleep 3

# Check if app is running
if ~/Android/Sdk/platform-tools/adb shell "ps | grep vira" > /dev/null 2>&1; then
    echo "✅ VIRA is running!"
    echo ""
    echo "📱 App is now running in the emulator"
    echo ""
    echo "💡 To view logs:"
    echo "   ~/Android/Sdk/platform-tools/adb logcat | grep -i vira"
    echo ""
    echo "💡 To stop the app:"
    echo "   ~/Android/Sdk/platform-tools/adb shell am force-stop com.vira.assistant"
else
    echo "❌ App crashed!"
    echo ""
    echo "📋 Last 20 log lines:"
    ~/Android/Sdk/platform-tools/adb logcat -d | grep -i "vira\|crash\|exception" | tail -20
fi
