#!/bin/bash

echo "🎤 Auto Setup ElevenLabs Voice"
echo "================================"

ELEVENLABS_KEY="sk_308603cc4ce21513cd1e4c289efdf31ed1a0a415ded08e31"

echo "📱 Opening VIRA Settings..."
# Open settings
~/Android/Sdk/platform-tools/adb shell am start -n com.vira.assistant/crc64b923e307a6396fec.SettingsActivity

sleep 3

echo "📝 Filling ElevenLabs API Key..."

# Scroll down to ElevenLabs section
~/Android/Sdk/platform-tools/adb shell input swipe 500 1500 500 500 300
sleep 1
~/Android/Sdk/platform-tools/adb shell input swipe 500 1500 500 500 300
sleep 1

# Tap on ElevenLabs API Key field (approximate coordinates)
~/Android/Sdk/platform-tools/adb shell input tap 540 1400
sleep 1

# Clear existing text
for i in {1..100}; do
    ~/Android/Sdk/platform-tools/adb shell input keyevent 67  # KEYCODE_DEL
done
sleep 1

# Type API key
~/Android/Sdk/platform-tools/adb shell input text "$ELEVENLABS_KEY"
sleep 2

# Scroll down more
~/Android/Sdk/platform-tools/adb shell input swipe 500 1500 500 500 300
sleep 1

# Tap Save button
~/Android/Sdk/platform-tools/adb shell input tap 540 2000
sleep 2

echo "✅ Settings saved!"

# Go back to main screen
~/Android/Sdk/platform-tools/adb shell input keyevent 4  # BACK
sleep 1

# Enable voice toggle (tap the voice button in header)
echo "🔊 Enabling voice output..."
~/Android/Sdk/platform-tools/adb shell input tap 950 235  # Voice toggle button
sleep 1

echo ""
echo "✅ Setup complete!"
echo "   - ElevenLabs API Key: Configured"
echo "   - Voice Output: ENABLED"
echo ""
echo "💡 Try sending a message to test the voice!"
