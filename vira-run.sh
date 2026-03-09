#!/bin/bash

# Simple VIRA Runner - Just works!

GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${GREEN}🚀 Starting VIRA...${NC}"
echo ""

# Start container
echo -e "${BLUE}[1/5]${NC} Starting Waydroid container..."
sudo waydroid container start 2>/dev/null || true
sleep 2

# Start Weston compositor
echo -e "${BLUE}[2/5]${NC} Starting Weston compositor..."
weston --width=400 --height=800 > /dev/null 2>&1 &
WESTON_PID=$!
sleep 3

# Start Waydroid session
echo -e "${BLUE}[3/5]${NC} Starting Waydroid session..."
export WAYLAND_DISPLAY=wayland-1
waydroid session start > /dev/null 2>&1 &
sleep 5

# Show Waydroid UI
echo -e "${BLUE}[4/5]${NC} Launching Waydroid UI..."
waydroid show-full-ui > /dev/null 2>&1 &
sleep 3

# Launch VIRA
echo -e "${BLUE}[5/5]${NC} Opening VIRA app..."
waydroid app launch com.vira.assistant

echo ""
echo -e "${GREEN}✅ VIRA is running!${NC}"
echo ""
echo -e "${YELLOW}Setup steps:${NC}"
echo "  1. Grant permissions (microphone, storage)"
echo "  2. Open Settings → Add API key"
echo "  3. Test: 'Hello VIRA'"
echo ""
echo -e "${BLUE}To stop:${NC} Close Weston window or press Ctrl+C"
echo ""

# Keep running
trap "echo ''; echo 'Stopping...'; waydroid session stop 2>/dev/null; kill $WESTON_PID 2>/dev/null; exit 0" INT TERM
wait $WESTON_PID 2>/dev/null || true

# Cleanup
waydroid session stop 2>/dev/null || true
echo -e "${GREEN}✅ Stopped${NC}"
