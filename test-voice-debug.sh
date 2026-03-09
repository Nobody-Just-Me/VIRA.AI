#!/bin/bash

echo "🔍 VIRA Voice Output Debug"
echo "=========================="
echo ""

# Clear logs
adb -s emulator-5554 logcat -c

echo "📋 Monitoring voice output logs..."
echo "Silakan kirim pesan di VIRA sekarang (misalnya: 'hello')"
echo ""
echo "Tekan Ctrl+C untuk stop monitoring"
echo ""

# Monitor logs with filters
adb -s emulator-5554 logcat | grep -E "(VIRA_MainActivity.*Voice|VIRA_MainActivity.*TTS|VIRA_MainActivity.*SynthesizeAndPlaySpeech|VIRA_ElevenLabs|AndroidVoiceService)" --line-buffered
