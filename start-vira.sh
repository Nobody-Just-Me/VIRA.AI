#!/bin/bash

# Script untuk menjalankan VIRA di Waydroid (X11 compatible)

echo "🚀 Starting VIRA in Waydroid..."
echo ""

# Start container if not running
echo "Starting Waydroid container..."
sudo waydroid container start 2>/dev/null || true
sleep 2

# Create a script for Weston to run
cat > /tmp/waydroid-session.sh << 'INNEREOF'
#!/bin/bash
export WAYLAND_DISPLAY=wayland-0
waydroid session start &
sleep 5
waydroid show-full-ui &
sleep 2
waydroid app launch com.vira.assistant
# Keep running
tail -f /dev/null
INNEREOF

chmod +x /tmp/waydroid-session.sh

# Start Weston in standalone mode with Waydroid
echo "Starting Wayland compositor with Waydroid..."
echo ""
echo "📱 Waydroid window will open shortly..."
echo "🛑 Press Ctrl+C here to stop everything"
echo ""

# Use weston-launch or weston with XDG_RUNTIME_DIR
XDG_RUNTIME_DIR=/run/user/$(id -u) weston --backend=x11-backend.so --width=400 --height=800 &
WESTON_PID=$!
sleep 4

# Start Waydroid in the new Wayland session
export WAYLAND_DISPLAY=wayland-0
waydroid session start &
sleep 5

# Show UI and launch VIRA
waydroid show-full-ui &
sleep 2
waydroid app launch com.vira.assistant

echo ""
echo "✅ VIRA is running!"
echo ""

# Wait for Weston to close
wait $WESTON_PID

# Cleanup
echo "Stopping Waydroid..."
waydroid session stop 2>/dev/null || true
