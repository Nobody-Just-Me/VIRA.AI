#!/bin/bash

# Script untuk menjalankan Waydroid di X11 menggunakan Weston/Cage
# Waydroid membutuhkan Wayland, jadi kita jalankan Wayland compositor di dalam X11

set -e

echo "🔧 Waydroid X11 Compatibility Fix"
echo "=================================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${YELLOW}⚠️  Detected X11 session${NC}"
echo "Waydroid requires Wayland. We'll use a Wayland compositor."
echo ""

# Check if weston or cage is installed
COMPOSITOR=""

if command -v weston &> /dev/null; then
    COMPOSITOR="weston"
    echo -e "${GREEN}✅ Weston found${NC}"
elif command -v cage &> /dev/null; then
    COMPOSITOR="cage"
    echo -e "${GREEN}✅ Cage found${NC}"
else
    echo -e "${YELLOW}Installing Wayland compositor...${NC}"
    echo ""
    echo "Choose compositor:"
    echo "1. Weston (full Wayland compositor, recommended)"
    echo "2. Cage (minimal, kiosk mode)"
    echo ""
    read -p "Choice (1-2): " choice
    
    case $choice in
        1)
            echo "Installing Weston..."
            sudo apt update
            sudo apt install -y weston
            COMPOSITOR="weston"
            ;;
        2)
            echo "Installing Cage..."
            sudo apt update
            sudo apt install -y cage
            COMPOSITOR="cage"
            ;;
        *)
            echo "Invalid choice"
            exit 1
            ;;
    esac
fi

echo ""
echo -e "${BLUE}Starting Waydroid with $COMPOSITOR...${NC}"
echo ""

# Make sure Waydroid container is running
if ! sudo waydroid container status &>/dev/null; then
    echo "Starting Waydroid container..."
    sudo waydroid container start
    sleep 3
fi

# Create launch script
cat > /tmp/waydroid-launch.sh << 'EOF'
#!/bin/bash
# Start Waydroid session
waydroid session start &
sleep 5

# Show Waydroid UI
waydroid show-full-ui &

# Keep compositor running
wait
EOF

chmod +x /tmp/waydroid-launch.sh

echo ""
echo -e "${GREEN}🚀 Launching Waydroid in Wayland compositor...${NC}"
echo ""
echo -e "${YELLOW}Tips:${NC}"
echo "- Press Ctrl+Alt+Backspace to exit compositor"
echo "- Or close the window to exit"
echo ""

sleep 2

# Launch based on compositor
if [ "$COMPOSITOR" = "weston" ]; then
    # Weston
    weston --width=1080 --height=2400 &
    WESTON_PID=$!
    sleep 3
    
    # Set WAYLAND_DISPLAY for this session
    export WAYLAND_DISPLAY=wayland-1
    
    # Start Waydroid
    waydroid session start &
    sleep 5
    waydroid show-full-ui
    
elif [ "$COMPOSITOR" = "cage" ]; then
    # Cage (kiosk mode)
    cage -- /tmp/waydroid-launch.sh
fi

echo ""
echo -e "${GREEN}✅ Done${NC}"
