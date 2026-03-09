#!/bin/bash

echo "========================================="
echo "🎉 VIRA Complete Settings - Test Script"
echo "========================================="
echo ""

# Check if device is connected
echo "📱 Checking for connected devices..."
DEVICES=$(adb devices | grep -v "List" | grep "device" | wc -l)

if [ $DEVICES -eq 0 ]; then
    echo "❌ No devices/emulators found!"
    echo ""
    echo "Please start emulator or connect device first:"
    echo "  emulator -avd Pixel_5_API_30 &"
    echo ""
    exit 1
fi

echo "✅ Found $DEVICES device(s)"
echo ""

# Install APK
echo "📦 Installing VIRA APK..."
adb install -r VIRA.Mobile/bin/Release/net8.0-android/com.vira.assistant-Signed.apk

if [ $? -ne 0 ]; then
    echo "❌ Installation failed!"
    exit 1
fi

echo "✅ Installation successful"
echo ""

# Launch VIRA
echo "🚀 Launching VIRA..."
adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity

sleep 2
echo "✅ VIRA launched"
echo ""

# Show instructions
echo "========================================="
echo "📋 Next Steps:"
echo "========================================="
echo ""
echo "1. Setup API Keys:"
echo "   - Tap ⚙ Settings button"
echo "   - Select Provider: Groq"
echo "   - Select Model: Llama 3.3 70B"
echo "   - Paste Groq API Key (get from: https://console.groq.com/keys)"
echo "   - (Optional) Paste ElevenLabs Key (get from: https://elevenlabs.io/)"
echo "   - Tap 'Save Configuration'"
echo ""
echo "2. Test Chat:"
echo "   - Go back to main screen"
echo "   - Type: hello"
echo "   - Press send"
echo "   - Wait for response"
echo "   - Voice will play automatically (if ElevenLabs key valid)"
echo ""
echo "3. Test Settings:"
echo "   - Tap ⚙ Settings"
echo "   - Try theme toggle (Light/Dark/System)"
echo "   - Try voice output toggle"
echo "   - Try clear history"
echo ""
echo "4. Monitor Logs:"
echo "   Run in another terminal:"
echo "   adb logcat -s VIRA_MainActivity VIRA_Groq | grep -E '(Response|Voice|TTS)'"
echo ""
echo "========================================="
echo "📚 Documentation:"
echo "========================================="
echo ""
echo "- VIRA_COMPLETE_SETTINGS_GUIDE.md - Full guide"
echo "- VOICE_OUTPUT_TROUBLESHOOTING.md - Voice troubleshooting"
echo "- VIRA_SETTINGS_COMPLETE_SUMMARY.md - Summary"
echo ""
echo "========================================="
echo "🎤 Voice Output Troubleshooting:"
echo "========================================="
echo ""
echo "If voice doesn't work:"
echo ""
echo "1. Check ElevenLabs quota:"
echo "   - Login to https://elevenlabs.io/"
echo "   - Profile → Subscription"
echo "   - Check 'Character Usage'"
echo "   - Free tier: 10,000 characters/month"
echo ""
echo "2. Check voice toggle:"
echo "   - Settings → Preferences → Voice Output"
echo "   - Make sure it's ON (blue)"
echo ""
echo "3. Check API key:"
echo "   - Settings → API Configuration"
echo "   - ElevenLabs API Key should start with 'sk_'"
echo "   - Length: ~68 characters"
echo ""
echo "4. Check logs:"
echo "   adb logcat -s VIRA_MainActivity | grep -E '(Voice|TTS|ElevenLabs)'"
echo ""
echo "5. Common issues:"
echo "   - Quota exceeded → Create new account or wait for reset"
echo "   - Voice toggle OFF → Turn it ON in Settings"
echo "   - Invalid API key → Paste correct key"
echo "   - No internet → Check WiFi/data"
echo "   - Rate limiting → Wait 1-2 seconds between messages"
echo ""
echo "========================================="
echo "✅ VIRA is ready to test!"
echo "========================================="
echo ""
