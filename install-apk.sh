#!/bin/bash

echo "📱 VIRA APK Installer"
echo "===================="
echo ""

# Check if APK exists
if [ ! -f "VIRA-Release.apk" ]; then
    echo "❌ Error: VIRA-Release.apk not found!"
    echo "   Please run: bash build.sh"
    exit 1
fi

echo "✅ APK found: VIRA-Release.apk (32 MB)"
echo ""

# Check if adb is available
if ! command -v adb &> /dev/null; then
    echo "❌ Error: adb not found!"
    echo "   Please install Android SDK Platform Tools"
    exit 1
fi

echo "✅ ADB found"
echo ""

# Check for connected devices
echo "🔍 Checking for connected devices..."
DEVICES=$(adb devices | grep -v "List" | grep "device$" | wc -l)

if [ "$DEVICES" -eq 0 ]; then
    echo "❌ No Android devices connected!"
    echo ""
    echo "📋 Instructions:"
    echo "   1. Connect your Android device via USB"
    echo "   2. Enable USB Debugging:"
    echo "      Settings → Developer Options → USB Debugging"
    echo "   3. Run this script again"
    echo ""
    echo "💡 Or transfer APK manually:"
    echo "   1. Copy VIRA-Release.apk to your device"
    echo "   2. Enable 'Install from Unknown Sources'"
    echo "   3. Tap the APK file to install"
    exit 1
fi

echo "✅ Found $DEVICES device(s)"
adb devices
echo ""

# Install APK
echo "📦 Installing VIRA..."
adb install -r ./VIRA-Release.apk

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Installation successful!"
    echo ""
    echo "🎉 VIRA is now installed on your device!"
    echo ""
    echo "📋 Next Steps:"
    echo "   1. Open VIRA app on your device"
    echo "   2. Tap ⚙️ (Settings) icon"
    echo "   3. Select API Provider:"
    echo "      - Groq (Recommended) ⭐"
    echo "      - or Gemini"
    echo "   4. Paste your API key"
    echo "   5. Tap 'Save Configuration'"
    echo "   6. Start chatting!"
    echo ""
    echo "🔑 Get your Groq API Key:"
    echo "   https://console.groq.com/keys"
    echo ""
    echo "💡 Get your own API key:"
    echo "   Groq:   https://console.groq.com/keys"
    echo "   Gemini: https://aistudio.google.com/apikey"
    echo ""
    echo "📊 Monitor logs (optional):"
    echo "   adb logcat | grep VIRA"
    echo ""
else
    echo ""
    echo "❌ Installation failed!"
    echo ""
    echo "💡 Try:"
    echo "   1. Uninstall old version first:"
    echo "      adb uninstall com.vira.assistant"
    echo "   2. Run this script again"
    echo ""
    echo "   Or install manually:"
    echo "   1. Transfer VIRA-Release.apk to device"
    echo "   2. Enable 'Install from Unknown Sources'"
    echo "   3. Tap APK file to install"
fi
