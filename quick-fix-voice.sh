#!/bin/bash

echo "🔧 VIRA Voice Quick Fix"
echo "======================="
echo ""

# 1. Uninstall old version
echo "1️⃣ Uninstalling old version..."
adb uninstall com.vira.assistant 2>/dev/null
echo "   ✅ Done"
echo ""

# 2. Build new APK
echo "2️⃣ Building new APK with voice features..."
bash build.sh | tail -20
echo ""

# 3. Install new APK
echo "3️⃣ Installing new APK..."
adb install -r ./VIRA-Release.apk
echo ""

# 4. Launch VIRA
echo "4️⃣ Launching VIRA..."
adb shell am start -n com.vira.assistant/.MainActivity
echo ""

echo "✅ Installation Complete!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📋 NEXT STEPS - IMPORTANT!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "🔑 Step 1: Get ElevenLabs API Key (if you don't have one)"
echo "   Visit: https://elevenlabs.io/"
echo "   Sign up (FREE - no credit card needed)"
echo "   Go to: Settings → API Keys"
echo "   Create API Key"
echo "   Copy the key (format: sk_...)"
echo ""
echo "⚙️ Step 2: Configure VIRA"
echo "   1. Open VIRA app on your device"
echo "   2. Tap ⚙️ (Settings) icon at top right"
echo "   3. Scroll down to '🎤 Voice Output (ElevenLabs TTS)'"
echo "   4. Paste your ElevenLabs API key"
echo "   5. Tap 'Save Configuration'"
echo "   6. Go back to chat (← button)"
echo ""
echo "🧪 Step 3: Test Voice Toggle"
echo "   1. Look at header - find toggle button next to 'Vira AI'"
echo "   2. Button should be 🔊 (green) = Voice ON"
echo "   3. Tap to toggle OFF → becomes 🔇 (red)"
echo "   4. Tap again to toggle ON → becomes 🔊 (green)"
echo ""
echo "💬 Step 4: Test Voice Output"
echo "   1. Make sure toggle is ON (🔊 green)"
echo "   2. Send message: 'Hello'"
echo "   3. Wait for response"
echo "   4. You should hear natural female voice"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "🐛 Troubleshooting:"
echo "   - If no voice: Check ElevenLabs API key in Settings"
echo "   - If toggle doesn't work: Restart VIRA app"
echo "   - If voice is robotic: ElevenLabs not configured (using Android TTS)"
echo "   - Monitor logs: adb logcat | grep VIRA"
echo ""
echo "📚 Full guide: TROUBLESHOOTING_VOICE.md"
echo ""
