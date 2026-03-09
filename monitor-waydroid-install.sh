#!/bin/bash

echo "🔍 Monitoring Waydroid Installation Progress"
echo "============================================="
echo ""

# Check if waydroid init is still running
while pgrep -f "waydroid init" > /dev/null; do
    echo "⏳ Waydroid initialization still in progress..."
    echo "   Download may take 10-15 minutes depending on your internet speed"
    echo "   Current time: $(date '+%H:%M:%S')"
    echo ""
    sleep 30
done

echo "✅ Waydroid initialization complete!"
echo ""

# Check if initialization was successful
if [ -d "/var/lib/waydroid" ]; then
    echo "✅ Waydroid images downloaded successfully"
    echo ""
    
    # Start Waydroid container
    echo "🔧 Starting Waydroid container..."
    sudo waydroid container start
    sleep 3
    
    # Start Waydroid session
    echo "🔧 Starting Waydroid session..."
    waydroid session start &
    sleep 5
    
    # Install VIRA APK
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
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "✅ VIRA is now running in Waydroid!"
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
    
else
    echo "❌ Waydroid initialization failed"
    echo "Please check the error messages above"
    exit 1
fi
