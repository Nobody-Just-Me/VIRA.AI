#!/bin/bash

# Script sederhana untuk menjalankan Waydroid di X11
# Menggunakan Weston sebagai Wayland compositor

set -e

echo "🚀 VIRA - Waydroid Launcher"
echo "==========================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Check if weston is installed
if ! command -v weston &> /dev/null; then
    echo -e "${YELLOW}Installing Weston (Wayland compositor)...${NC}"
    sudo apt update
    sudo apt install -y weston
fi

# Check if Waydroid container is running
echo "Checking Waydroid container..."
if ! sudo waydroid container status &>/dev/null; then
    echo "Starting Waydroid container..."
    sudo waydroid container start
    sleep 3
fi

echo ""
echo -e "${GREEN}✅ Starting Waydroid${NC}"
echo ""
echo -e "${BLUE}Instructions:${NC}"
echo "1. Weston window will open (Wayland compositor)"
echo "2. Waydroid will start inside Weston"
echo "3. To exit: Close Weston window or press Ctrl+C here"
echo ""
echo -e "${YELLOW}Press Enter to continue...${NC}"
read

# Start Weston in background
echo "Starting Weston..."
weston --width=400 --height=800 &
WESTON_PID=$!
sleep 3

# Set Wayland display
export WAYLAND_DISPLAY=wayland-1

# Start Waydroid session
echo "Starting Waydroid session..."
waydroid session start &
sleep 5

# Show Waydroid UI
echo "Launching Waydroid UI..."
waydroid show-full-ui &

# Launch VIRA
sleep 3
echo ""
echo -e "${GREEN}Launching VIRA...${NC}"
waydroid app launch com.vira.assistant

echo ""
echo -e "${GREEN}✅ Waydroid is running${NC}"
echo ""
echo "Press Ctrl+C to stop Waydroid"
echo ""

# Wait for user to stop
wait $WESTON_PID

# Cleanup
echo ""
echo "Stopping Waydroid..."
waydroid session stop
