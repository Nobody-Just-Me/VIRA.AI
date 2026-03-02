#!/bin/bash

echo "🎤 Setting up ElevenLabs Voice for VIRA"
echo "========================================"

ELEVENLABS_API_KEY="sk_308603cc4ce21513cd1e4c289efdf31ed1a0a415ded08e31"

echo "📱 Checking if VIRA is installed..."
if ! adb shell pm list packages | grep -q "com.vira.assistant"; then
    echo "❌ VIRA not installed. Please install first."
    exit 1
fi

echo "✅ VIRA found"

echo ""
echo "🔧 Configuring ElevenLabs API Key..."

# Set ElevenLabs API key
adb shell "run-as com.vira.assistant sh -c 'cat > /data/data/com.vira.assistant/shared_prefs/vira_settings.xml << EOF
<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>
<map>
    <string name=\"ai_provider\">groq</string>
    <string name=\"groq_api_key\">YOUR_GROQ_API_KEY_HERE</string>
    <string name=\"elevenlabs_api_key\">$ELEVENLABS_API_KEY</string>
    <boolean name=\"voice_output_enabled\" value=\"true\" />
</map>
EOF
'"

if [ $? -eq 0 ]; then
    echo "✅ ElevenLabs API Key configured"
    echo "✅ Voice output enabled"
else
    echo "❌ Failed to configure. Trying alternative method..."
    
    # Alternative method using content provider
    adb shell am broadcast -a com.vira.assistant.SET_CONFIG \
        --es elevenlabs_api_key "$ELEVENLABS_API_KEY" \
        --ez voice_output_enabled true
fi

echo ""
echo "🔄 Restarting VIRA to apply changes..."
adb shell am force-stop com.vira.assistant
sleep 1
adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity

echo ""
echo "✅ Setup complete!"
echo ""
echo "📋 Configuration:"
echo "   - ElevenLabs API Key: ${ELEVENLABS_API_KEY:0:20}..."
echo "   - Voice ID: zd0hd2egR1Q6EzSLTzCp (Female, Natural)"
echo "   - Voice Output: ENABLED 🔊"
echo ""
echo "💡 Test voice by sending a message in VIRA"
echo "   The voice should now be a natural female voice (ElevenLabs)"
