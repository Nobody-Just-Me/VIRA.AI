#!/bin/bash

# Script untuk debug Waydroid

echo "🔍 Waydroid Debug Tool"
echo "====================="
echo ""

echo "1. Checking Waydroid status..."
waydroid status
echo ""

echo "2. Checking container..."
sudo waydroid container start
echo ""

echo "3. Checking Wayland display..."
echo "XDG_SESSION_TYPE: $XDG_SESSION_TYPE"
echo "WAYLAND_DISPLAY: $WAYLAND_DISPLAY"
echo ""

echo "4. Checking Wayland sockets..."
ls -la /run/user/$(id -u)/ | grep wayland || echo "No Wayland sockets found"
echo ""

echo "5. Checking if Waydroid is initialized..."
if [ -d "/var/lib/waydroid" ]; then
    echo "✅ Waydroid data exists"
    ls -la /var/lib/waydroid/
else
    echo "❌ Waydroid not initialized"
    echo "Run: sudo waydroid init -s GAPPS -f"
fi
echo ""

echo "6. Checking logs..."
echo "Recent Waydroid logs:"
waydroid log | tail -20
echo ""

echo "7. Trying to start session..."
waydroid session start &
SESSION_PID=$!
sleep 5

echo ""
echo "8. Checking session status..."
waydroid status
echo ""

echo "9. Checking logcat..."
waydroid logcat | head -20 &
LOGCAT_PID=$!
sleep 3
kill $LOGCAT_PID 2>/dev/null

echo ""
echo "10. Recommendation:"
echo ""

if [ "$XDG_SESSION_TYPE" = "x11" ]; then
    echo "⚠️  You are using X11. Waydroid needs Wayland."
    echo ""
    echo "Solutions:"
    echo "1. Switch to Wayland session (logout → login with Wayland)"
    echo "2. Use: ./vira-waydroid-simple.sh (uses Cage)"
    echo "3. Test on real Android device: adb install VIRA-Release.apk"
else
    echo "✅ You are using Wayland"
    echo "Try: waydroid show-full-ui"
fi

# Cleanup
waydroid session stop 2>/dev/null
