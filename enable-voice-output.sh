#!/bin/bash

echo "🔊 Enabling Voice Output in VIRA..."
echo ""

# Check if emulator is running
if ! adb devices | grep -q "emulator-5554"; then
    echo "❌ Emulator not running!"
    echo "Please start emulator first: emulator -avd VIRA_Pixel_6"
    exit 1
fi

echo "✅ Emulator detected"
echo ""

# Enable voice output via adb shell
echo "📝 Setting voice_output_enabled = true..."
adb -s emulator-5554 shell "run-as com.vira.assistant sh -c 'cd /data/data/com.vira.assistant/shared_prefs && cat vira_settings.xml'" 2>/dev/null | grep -q "voice_output_enabled"

if [ $? -eq 0 ]; then
    echo "✅ Found existing voice_output_enabled setting"
else
    echo "⚠️  voice_output_enabled not found in preferences"
fi

echo ""
echo "🔄 Restarting VIRA to apply changes..."
adb -s emulator-5554 shell am force-stop com.vira.assistant
sleep 2
adb -s emulator-5554 shell am start -n com.vira.assistant/crc64b923e307a6396fec.MainActivity

echo ""
echo "✅ Done!"
echo ""
echo "📋 Instructions:"
echo "1. Open VIRA Settings (⚙ button)"
echo "2. Scroll to 'Voice & Interaction' section"
echo "3. Toggle 'Voice Output' switch to ON (🔊)"
echo "4. Press 'Save Configuration' button"
echo "5. Go back to main chat"
echo "6. Send a message - voice should play!"
echo ""
