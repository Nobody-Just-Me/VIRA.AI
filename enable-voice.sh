#!/bin/bash

echo "🎤 VIRA Voice Enabler"
echo "===================="
echo ""

# Check if device is connected
if ! adb devices | grep -q "device$"; then
    echo "❌ No Android device/emulator connected"
    echo "   Please start emulator or connect device first"
    exit 1
fi

echo "📱 Device connected"
echo ""

# Enable voice output toggle
echo "🔊 Enabling voice output toggle..."
adb shell "run-as com.vira.assistant sh -c 'echo \"<?xml version='1.0' encoding='utf-8' standalone='yes' ?><map><boolean name=\\\"voice_output_enabled\\\" value=\\\"true\\\" /></map>\" > /data/data/com.vira.assistant/shared_prefs/vira_settings.xml'"

if [ $? -eq 0 ]; then
    echo "✅ Voice toggle enabled"
else
    echo "⚠️  Could not enable via run-as, trying alternative method..."
    # Alternative: use am broadcast to trigger settings change
    adb shell am broadcast -a com.vira.assistant.ENABLE_VOICE
fi

echo ""
echo "📋 Current Settings:"
echo "-------------------"

# Try to read current settings
adb shell "run-as com.vira.assistant cat /data/data/com.vira.assistant/shared_prefs/vira_settings.xml" 2>/dev/null

echo ""
echo "🔄 Restarting VIRA to apply changes..."
adb shell am force-stop com.vira.assistant
sleep 1
adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity

echo ""
echo "✅ Done!"
echo ""
echo "📝 Next Steps:"
echo "   1. Open VIRA Settings (⚙️ icon)"
echo "   2. Scroll to 'ElevenLabs TTS Configuration'"
echo "   3. Enter your ElevenLabs API Key"
echo "   4. Tap 'Save Settings'"
echo "   5. Voice toggle should now be ON (🔊 green)"
echo ""
echo "🎤 Get ElevenLabs API Key (FREE):"
echo "   https://elevenlabs.io/ → Profile → API Key"
echo ""
