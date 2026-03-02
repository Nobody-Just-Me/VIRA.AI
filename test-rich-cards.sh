#!/bin/bash

# Test Rich Cards in VIRA
# This script tests all the rich card types

echo "🧪 Testing VIRA Rich Cards"
echo "=========================="
echo ""

# Check if emulator is running
if ! adb devices | grep -q "emulator"; then
    echo "❌ Emulator not running. Please start it first:"
    echo "   ~/Android/Sdk/emulator/emulator -avd Pixel_7_API_34 &"
    exit 1
fi

echo "✅ Emulator detected"
echo ""

# Launch app
echo "📱 Launching VIRA..."
adb shell monkey -p com.vira.assistant -c android.intent.category.LAUNCHER 1 > /dev/null 2>&1
sleep 3

echo "✅ App launched"
echo ""

echo "📋 Test Instructions:"
echo "===================="
echo ""
echo "Test the following quick actions and keywords:"
echo ""
echo "1. ☀️ Weather Card:"
echo "   - Click 'Weather' quick action"
echo "   - Or type: 'cuaca hari ini'"
echo "   - Expected: Weather card with temperature, condition, humidity, UV"
echo ""
echo "2. 📅 Schedule Card:"
echo "   - Type: 'jadwal hari ini'"
echo "   - Expected: Schedule card with time, title, location"
echo ""
echo "3. 📰 News Card:"
echo "   - Click 'News' quick action"
echo "   - Or type: 'berita terkini'"
echo "   - Expected: News card with categories and headlines"
echo ""
echo "4. 🚗 Traffic Card:"
echo "   - Click 'Traffic' quick action"
echo "   - Or type: 'kondisi lalu lintas'"
echo "   - Expected: Traffic card with routes, ETA, status"
echo ""
echo "5. 🔔 Reminder Card:"
echo "   - Click 'Reminders' quick action"
echo "   - Or type: 'pengingat saya'"
echo "   - Expected: Reminder card with checkboxes"
echo ""
echo "6. ☕ Coffee Card:"
echo "   - Click 'Coffee' quick action"
echo "   - Or type: 'pesan kopi'"
echo "   - Expected: Coffee order card with type, size, location, ETA, price"
echo ""
echo "7. 🎵 Music Card:"
echo "   - Click 'Music' quick action"
echo "   - Or type: 'putar musik'"
echo "   - Expected: Music card with playlist, song, artist"
echo ""
echo "8. ⚙️ Settings:"
echo "   - Click settings icon (top right)"
echo "   - Enter Gemini API Key"
echo "   - Click 'Save Configuration'"
echo "   - Expected: API key saved and loaded"
echo ""
echo "9. 🎤 Voice:"
echo "   - Click microphone button"
echo "   - Speak a command"
echo "   - Expected: Voice recognition works, transcription sent to chat"
echo ""

echo "📊 View Logs:"
echo "============"
echo "To see detailed logs, run:"
echo "   adb logcat | grep VIRA"
echo ""

echo "🎉 Happy Testing!"
