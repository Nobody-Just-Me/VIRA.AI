#!/bin/bash

# Simple VIRA launcher for Waydroid
# Works on X11 without needing Weston

set -e

echo "🚀 VIRA - Simple Launcher"
echo "========================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Check if Waydroid is installed
if ! command -v waydroid &> /dev/null; then
    echo -e "${RED}❌ Waydroid is not installed${NC}"
    echo "Run: ./quick-start-vira.sh to install"
    exit 1
fi

# Step 1: Start container (requires sudo)
echo -e "${BLUE}Step 1: Starting Waydroid container...${NC}"
sudo waydroid container start 2>/dev/null || echo "Container already running"
sleep 2

# Step 2: Start session (in background)
echo -e "${BLUE}Step 2: Starting Waydroid session...${NC}"
waydroid session start > /dev/null 2>&1 &
SESSION_PID=$!
sleep 5

# Check if session started
if waydroid status | grep -q "RUNNING"; then
    echo -e "${GREEN}✅ Waydroid session is running${NC}"
else
    echo -e "${YELLOW}⏳ Waiting for session to start...${NC}"
    sleep 5
fi

# Step 3: Install VIRA if not already installed
echo -e "${BLUE}Step 3: Checking VIRA installation...${NC}"
if ! waydroid app list 2>/dev/null | grep -q "com.vira.assistant"; then
    echo -e "${YELLOW}Installing VIRA APK...${NC}"
    waydroid app install VIRA-Release.apk
    echo -e "${GREEN}✅ VIRA installed${NC}"
else
    echo -e "${GREEN}✅ VIRA already installed${NC}"
fi

# Step 4: Show Waydroid UI
echo -e "${BLUE}Step 4: Launching Waydroid UI...${NC}"
waydroid show-full-ui > /dev/null 2>&1 &
UI_PID=$!
sleep 3

# Step 5: Launch VIRA
echo -e "${BLUE}Step 5: Launching VIRA app...${NC}"
waydroid app launch com.vira.assistant

echo ""
echo -e "${GREEN}✅ VIRA is now running!${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Configure your API key in VIRA settings"
echo "2. Grant microphone permissions when prompted"
echo "3. Try voice commands like 'Hello VIRA'"
echo ""
echo -e "${BLUE}To stop Waydroid:${NC}"
echo "  waydroid session stop"
echo ""
echo -e "${BLUE}To view logs:${NC}"
echo "  waydroid log"
echo ""

# Keep script running
echo "Press Ctrl+C to stop monitoring..."
wait $UI_PID 2>/dev/null || true
