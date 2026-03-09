#!/bin/bash

# VIRA Launcher using Cage (Wayland compositor)
# Works on X11 systems

set -e

echo "🚀 VIRA - Cage Launcher"
echo "======================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Step 1: Start container
echo -e "${BLUE}Step 1: Starting Waydroid container...${NC}"
sudo waydroid container start 2>/dev/null || echo "Container already running"
sleep 2

# Step 2: Start Cage with Waydroid
echo -e "${BLUE}Step 2: Starting Cage compositor with Waydroid...${NC}"
echo -e "${YELLOW}Note: Cage will open in fullscreen. Press Ctrl+C in this terminal to exit.${NC}"
echo ""
sleep 2

# Export display for Waydroid
export WAYLAND_DISPLAY=wayland-0

# Start Waydroid session in background
waydroid session start > /dev/null 2>&1 &
sleep 5

# Launch Waydroid UI inside Cage
echo -e "${GREEN}✅ Launching Waydroid in Cage...${NC}"
echo ""
cage -- waydroid show-full-ui &
CAGE_PID=$!
sleep 5

# Launch VIRA
echo -e "${GREEN}✅ Launching VIRA app...${NC}"
waydroid app launch com.vira.assistant

echo ""
echo -e "${GREEN}✅ VIRA is running!${NC}"
echo ""
echo -e "${YELLOW}Instructions:${NC}"
echo "- Waydroid UI is running in Cage window"
echo "- Press Ctrl+C here to stop everything"
echo "- Or close the Cage window"
echo ""
echo -e "${BLUE}Next steps in VIRA:${NC}"
echo "1. Grant microphone and storage permissions"
echo "2. Open Settings and add your API key"
echo "3. Test voice commands"
echo ""

# Wait for Cage to exit
wait $CAGE_PID 2>/dev/null || true

# Cleanup
echo ""
echo -e "${YELLOW}Stopping Waydroid...${NC}"
waydroid session stop 2>/dev/null || true
echo -e "${GREEN}✅ Done${NC}"
