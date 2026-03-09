#!/bin/bash

# VIRA Launcher - Multiple options for different setups
# Automatically detects best method

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo "🚀 VIRA Launcher"
echo "================"
echo ""

# Check if Waydroid is installed
if ! command -v waydroid &> /dev/null; then
    echo -e "${RED}❌ Waydroid is not installed${NC}"
    echo "Run: ./quick-start-vira.sh to install"
    exit 1
fi

# Detect session type
SESSION_TYPE=$(echo $XDG_SESSION_TYPE)
echo -e "${BLUE}Detected session type: ${SESSION_TYPE}${NC}"
echo ""

# Start container
echo -e "${BLUE}Starting Waydroid container...${NC}"
sudo waydroid container start 2>/dev/null || echo "Container already running"
sleep 2

if [ "$SESSION_TYPE" = "wayland" ]; then
    # Native Wayland - direct launch
    echo -e "${GREEN}✅ Running on Wayland - using direct launch${NC}"
    echo ""
    
    waydroid session start > /dev/null 2>&1 &
    sleep 5
    
    waydroid show-full-ui &
    sleep 3
    
    waydroid app launch com.vira.assistant
    
    echo ""
    echo -e "${GREEN}✅ VIRA is running!${NC}"
    echo "Press Ctrl+C to stop"
    wait
    
else
    # X11 - need Wayland compositor
    echo -e "${YELLOW}Running on X11 - need Wayland compositor${NC}"
    echo ""
    echo "Choose launch method:"
    echo "  1) Cage (fullscreen, simple)"
    echo "  2) Weston (windowed, more features)"
    echo "  3) Try direct (may not work)"
    echo ""
    read -p "Enter choice [1-3]: " choice
    
    case $choice in
        1)
            echo -e "${GREEN}Using Cage...${NC}"
            export WAYLAND_DISPLAY=wayland-0
            waydroid session start > /dev/null 2>&1 &
            sleep 5
            cage -- waydroid show-full-ui &
            COMPOSITOR_PID=$!
            sleep 5
            waydroid app launch com.vira.assistant
            echo ""
            echo -e "${GREEN}✅ VIRA running in Cage${NC}"
            echo "Press Ctrl+C to stop"
            wait $COMPOSITOR_PID 2>/dev/null || true
            ;;
            
        2)
            echo -e "${GREEN}Using Weston...${NC}"
            weston --width=400 --height=800 &
            WESTON_PID=$!
            sleep 3
            export WAYLAND_DISPLAY=wayland-1
            waydroid session start > /dev/null 2>&1 &
            sleep 5
            waydroid show-full-ui &
            sleep 3
            waydroid app launch com.vira.assistant
            echo ""
            echo -e "${GREEN}✅ VIRA running in Weston${NC}"
            echo "Close Weston window or press Ctrl+C to stop"
            wait $WESTON_PID 2>/dev/null || true
            ;;
            
        3)
            echo -e "${YELLOW}Trying direct launch...${NC}"
            waydroid session start > /dev/null 2>&1 &
            sleep 5
            waydroid show-full-ui &
            sleep 3
            waydroid app launch com.vira.assistant
            echo ""
            echo -e "${GREEN}✅ VIRA launched${NC}"
            echo "Press Ctrl+C to stop"
            wait
            ;;
            
        *)
            echo -e "${RED}Invalid choice${NC}"
            exit 1
            ;;
    esac
fi

# Cleanup
echo ""
echo -e "${YELLOW}Stopping Waydroid...${NC}"
waydroid session stop 2>/dev/null || true
echo -e "${GREEN}✅ Done${NC}"
