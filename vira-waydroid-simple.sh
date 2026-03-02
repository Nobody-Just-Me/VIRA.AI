#!/bin/bash

# Solusi paling sederhana: Gunakan Cage (Wayland kiosk compositor)

echo "🚀 VIRA - Waydroid Launcher (Simple)"
echo "====================================="
echo ""

# Check if cage is installed
if ! command -v cage &> /dev/null; then
    echo "📦 Installing Cage (Wayland compositor)..."
    sudo apt update
    sudo apt install -y cage
    echo "✅ Cage installed"
    echo ""
fi

# Start Waydroid container
echo "Starting Waydroid container..."
sudo waydroid container start 2>/dev/null || true
sleep 2

# Create launcher script
cat > /tmp/vira-launcher.sh << 'EOF'
#!/bin/bash
# Wait a bit for Wayland to be ready
sleep 2

# Start Waydroid session
waydroid session start &
sleep 5

# Show Waydroid UI
waydroid show-full-ui &
sleep 3

# Launch VIRA
waydroid app launch com.vira.assistant

# Keep running
while true; do
    sleep 1
done
EOF

chmod +x /tmp/vira-launcher.sh

echo ""
echo "🎮 Launching Waydroid with Cage..."
echo ""
echo "📱 Controls:"
echo "   - Alt+Shift+Esc : Exit Cage"
echo "   - Or press Ctrl+C in this terminal"
echo ""
sleep 2

# Launch Cage with Waydroid
cage -d -- /tmp/vira-launcher.sh

# Cleanup
echo ""
echo "Stopping Waydroid..."
waydroid session stop 2>/dev/null || true

echo "✅ Done"
