#!/bin/bash

echo "🔍 VIRA Voice Debug Tool"
echo "========================"
echo ""

# Check if device is connected
if ! adb devices | grep -q "device$"; then
    echo "❌ No Android device connected!"
    echo "   Please connect your device via USB"
    exit 1
fi

echo "✅ Device connected"
echo ""

echo "📊 Checking VIRA installation..."
if adb shell pm list packages | grep -q "com.vira.assistant"; then
    echo "✅ VIRA is installed"
else
    echo "❌ VIRA is not installed!"
    echo "   Run: bash install-apk.sh"
    exit 1
fi

echo ""
echo "🔑 Checking API Keys in SharedPreferences..."
echo "============================================"

# Get SharedPreferences
adb shell "run-as com.vira.assistant cat /data/data/com.vira.assistant/shared_prefs/vira_settings.xml" 2>/dev/null | grep -E "ai_provider|api_key|voice_output"

echo ""
echo "📱 Starting VIRA..."
adb shell am start -n com.vira.assistant/.MainActivity

echo ""
echo "📋 Monitoring logs (Ctrl+C to stop)..."
echo "======================================="
echo ""

# Monitor relevant logs
adb logcat -c  # Clear logs
adb logcat | grep -E "VIRA_MainActivity|VIRA_ElevenLabs|VIRA_Voice|VIRA_Settings" --color=always

