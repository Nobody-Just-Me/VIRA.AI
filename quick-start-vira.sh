#!/bin/bash

echo "🚀 VIRA Quick Start"
echo "==================="
echo ""

# Check if Waydroid is installed
if ! command -v waydroid &> /dev/null; then
    echo "📦 Waydroid not found. Installing..."
    echo ""
    
    # Quick install for Ubuntu/Debian
    if command -v apt &> /dev/null; then
        echo "Installing Waydroid..."
        curl https://repo.waydro.id 2>/dev/null | sudo bash
        sudo apt update
        sudo apt install -y waydroid
        
        # Initialize Waydroid
        echo "Initializing Waydroid..."
        sudo waydroid init -s GAPPS
    else
        echo "❌ Automatic installation not supported for your system"
        echo "Please install Waydroid manually: https://waydro.id"
        exit 1
    fi
fi

echo "✅ Waydroid installed"
echo ""

# Start Waydroid container
echo "🔧 Starting Waydroid container..."
sudo waydroid container start 2>/dev/null || true
sleep 2

# Start Waydroid session
echo "🔧 Starting Waydroid session..."
waydroid session start &
sleep 5

# Install APK
if [ -f "VIRA-Release.apk" ]; then
    echo "📦 Installing VIRA APK..."
    waydroid app install VIRA-Release.apk
    echo "✅ VIRA installed"
else
    echo "❌ VIRA-Release.apk not found!"
    exit 1
fi

echo ""
echo "🎉 Starting VIRA..."
echo ""

# Show Waydroid UI
waydroid show-full-ui &
sleep 3

# Launch VIRA
waydroid app launch com.vira.assistant

echo ""
echo "✅ VIRA is running!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📱 VIRA is now running in Waydroid"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "💡 Next steps:"
echo "   1. Tap ⚙️ (Settings) in VIRA"
echo "   2. Select API Provider (Groq recommended)"
echo "   3. Paste your API key"
echo "   4. Tap 'Save Configuration'"
echo "   5. Start chatting!"
echo ""
echo "🔑 Get API keys:"
echo "   Groq:   https://console.groq.com/keys"
echo "   Gemini: https://aistudio.google.com/apikey"
echo ""
echo "📊 View logs:"
echo "   waydroid logcat | grep VIRA"
echo ""
echo "🛑 Stop Waydroid:"
echo "   waydroid session stop"
echo "   sudo waydroid container stop"
echo ""
