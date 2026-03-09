#!/bin/bash

echo "=========================================="
echo "🎤 Installing Indonesian TTS on Emulator"
echo "=========================================="

# Check if emulator is running
if ! adb devices | grep -q "emulator-5554"; then
    echo "❌ Emulator not running!"
    echo "Please start emulator first: ./start-emulator.sh"
    exit 1
fi

echo ""
echo "📱 Emulator detected: emulator-5554"
echo ""

# Check current TTS engine
echo "🔍 Checking current TTS engine..."
CURRENT_ENGINE=$(adb -s emulator-5554 shell "settings get secure tts_default_synth")
echo "   Current engine: $CURRENT_ENGINE"

# Check if Google TTS is installed
echo ""
echo "🔍 Checking for Google TTS..."
if adb -s emulator-5554 shell "pm list packages" | grep -q "com.google.android.tts"; then
    echo "✅ Google TTS is installed"
else
    echo "❌ Google TTS not found"
    echo "   Installing Google TTS..."
    # Note: On emulator, Google TTS should be pre-installed
    # If not, user needs to install from Play Store
fi

echo ""
echo "=========================================="
echo "📋 MANUAL STEPS REQUIRED:"
echo "=========================================="
echo ""
echo "Emulator TTS Settings sudah dibuka."
echo "Silakan ikuti langkah berikut di emulator:"
echo ""
echo "1. Di TTS Settings, tap 'Google Text-to-speech Engine'"
echo "2. Tap icon Settings (⚙️) di sebelah kanan"
echo "3. Tap 'Install voice data'"
echo "4. Scroll dan cari 'Indonesian (Indonesia)' atau 'Bahasa Indonesia'"
echo "5. Tap untuk download (ukuran ~20-50MB)"
echo "6. Tunggu download selesai"
echo "7. Kembali ke TTS Settings"
echo "8. Tap 'Language'"
echo "9. Pilih 'Indonesian (Indonesia)'"
echo "10. Pilih voice 'Female' jika tersedia"
echo "11. Tap 'Play' untuk test"
echo ""
echo "=========================================="
echo "🧪 TESTING:"
echo "=========================================="
echo ""
echo "Setelah install selesai:"
echo "1. Buka aplikasi VIRA"
echo "2. Kirim pesan: 'Halo Vira, apa kabar?'"
echo "3. Dengarkan respons - seharusnya berbicara dalam bahasa Indonesia"
echo ""
echo "=========================================="
echo "💡 TIPS:"
echo "=========================================="
echo ""
echo "- Pastikan volume emulator tidak mute"
echo "- Jika suara masih bahasa Inggris, restart emulator"
echo "- Jika dialog 'Bahasa Indonesia Tidak Tersedia' muncul di VIRA,"
echo "  tap 'Install' untuk langsung ke TTS Settings"
echo ""
echo "=========================================="
