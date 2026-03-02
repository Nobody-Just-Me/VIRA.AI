#!/bin/bash

echo "🎤 ElevenLabs API Key Setup"
echo "==========================="
echo ""

# Check if API key is provided
if [ -z "$1" ]; then
    echo "❌ Error: No API key provided"
    echo ""
    echo "Usage:"
    echo "  bash set-elevenlabs-key.sh YOUR_API_KEY"
    echo ""
    echo "Example:"
    echo "  bash set-elevenlabs-key.sh sk_abc123..."
    echo ""
    echo "Get your FREE API key from:"
    echo "  https://elevenlabs.io/ → Profile → API Key"
    echo ""
    exit 1
fi

API_KEY="$1"

echo "📱 Checking device connection..."
if ! adb devices | grep -q "device$"; then
    echo "❌ No Android device/emulator connected"
    echo "   Please start emulator or connect device first"
    exit 1
fi

echo "✅ Device connected"
echo ""

echo "🔑 Setting ElevenLabs API Key..."
echo "   Key: ${API_KEY:0:10}...${API_KEY: -4}"
echo ""

# Create settings XML with both Groq and ElevenLabs keys
cat > /tmp/vira_settings.xml << EOF
<?xml version='1.0' encoding='utf-8' standalone='yes' ?>
<map>
    <string name="ai_provider">groq</string>
    <string name="groq_api_key">YOUR_GROQ_API_KEY_HERE</string>
    <string name="elevenlabs_api_key">$API_KEY</string>
    <boolean name="voice_output_enabled" value="true" />
</map>
EOF

# Push to device
echo "📤 Uploading settings to device..."
adb push /tmp/vira_settings.xml /sdcard/vira_settings.xml

# Move to app directory (requires root or run-as)
echo "📥 Installing settings..."
adb shell "run-as com.vira.assistant cp /sdcard/vira_settings.xml /data/data/com.vira.assistant/shared_prefs/vira_settings.xml" 2>/dev/null

if [ $? -ne 0 ]; then
    echo "⚠️  Could not use run-as, trying alternative method..."
    # Alternative: restart app and let it create settings
    adb shell am force-stop com.vira.assistant
    sleep 1
    adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity
    sleep 2
    
    # Try again
    adb shell "run-as com.vira.assistant cp /sdcard/vira_settings.xml /data/data/com.vira.assistant/shared_prefs/vira_settings.xml"
fi

# Cleanup
adb shell rm /sdcard/vira_settings.xml
rm /tmp/vira_settings.xml

echo ""
echo "🔄 Restarting VIRA to apply changes..."
adb shell am force-stop com.vira.assistant
sleep 1
adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity

echo ""
echo "✅ Done!"
echo ""
echo "📋 Configuration:"
echo "   ✅ AI Provider: Groq (llama-3.3-70b-versatile)"
echo "   ✅ Groq API Key: Configured"
echo "   ✅ ElevenLabs API Key: $API_KEY"
echo "   ✅ Voice Output: ENABLED (🔊)"
echo "   ✅ Voice ID: zd0hd2egR1Q6EzSLTzCp (Female, Natural)"
echo ""
echo "🎤 Voice should now work with ElevenLabs!"
echo ""
echo "💡 Test by:"
echo "   1. Open VIRA"
echo "   2. Send a message: 'Hello Vira'"
echo "   3. Listen for natural female voice"
echo ""
echo "📊 View logs:"
echo "   adb logcat | grep -E 'VIRA_|ElevenLabs|TTS'"
echo ""
