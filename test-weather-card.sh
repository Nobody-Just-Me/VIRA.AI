#!/bin/bash

echo "🧪 Testing Weather Card..."
echo ""
echo "Please manually:"
echo "1. Open VIRA app on emulator"
echo "2. Type: 'What's the weather like today?'"
echo "3. Press Send"
echo ""
echo "Expected: Weather card should appear with:"
echo "  - Temperature (32°C)"
echo "  - Condition (Cerah Berawan)"
echo "  - Humidity (74%)"
echo "  - UV Index (7)"
echo "  - Tomorrow forecast"
echo ""
echo "Watching logs..."
~/Android/Sdk/platform-tools/adb logcat -c
~/Android/Sdk/platform-tools/adb logcat | grep -E "VIRA_Gemini|VIRA_MainActivity"
