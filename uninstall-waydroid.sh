#!/bin/bash

# Uninstall Waydroid completely
# Run with: sudo ./uninstall-waydroid.sh

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo -e "${RED}Please run as root: sudo ./uninstall-waydroid.sh${NC}"
    exit 1
fi

echo "🗑️  Uninstalling Waydroid"
echo "========================"
echo ""

# Step 1: Stop all Waydroid processes
echo -e "${BLUE}Step 1: Stopping Waydroid processes...${NC}"
waydroid session stop 2>/dev/null || true
sleep 1
waydroid container stop 2>/dev/null || true
sleep 1
pkill -9 -f waydroid 2>/dev/null || true
pkill -9 -f weston 2>/dev/null || true
pkill -9 -f cage 2>/dev/null || true
sleep 2
echo -e "${GREEN}✅ Processes stopped${NC}"
echo ""

# Step 2: Uninstall Waydroid package
echo -e "${BLUE}Step 2: Removing Waydroid package...${NC}"
apt remove --purge -y waydroid 2>/dev/null || true
apt autoremove -y 2>/dev/null || true
echo -e "${GREEN}✅ Package removed${NC}"
echo ""

# Step 3: Remove Waydroid data
echo -e "${BLUE}Step 3: Removing Waydroid data...${NC}"
rm -rf /var/lib/waydroid 2>/dev/null || true
rm -rf /home/*/. local/share/waydroid 2>/dev/null || true
rm -rf /etc/waydroid 2>/dev/null || true
rm -rf /home/*/.local/share/waydroid 2>/dev/null || true
echo -e "${GREEN}✅ Data removed${NC}"
echo ""

# Step 4: Remove Waydroid repository
echo -e "${BLUE}Step 4: Removing Waydroid repository...${NC}"
rm -f /etc/apt/sources.list.d/waydroid.list 2>/dev/null || true
apt update 2>/dev/null || true
echo -e "${GREEN}✅ Repository removed${NC}"
echo ""

echo ""
echo -e "${GREEN}✅ Waydroid completely uninstalled!${NC}"
echo ""
echo -e "${YELLOW}Next step: Install Android Emulator${NC}"
echo "  ./install-android-emulator.sh"
echo ""
