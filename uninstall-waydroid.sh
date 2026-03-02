#!/bin/bash

# Script untuk uninstall Waydroid

echo "🗑️  Uninstalling Waydroid"
echo "========================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${YELLOW}⚠️  This will remove Waydroid and all its data${NC}"
read -p "Continue? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Cancelled"
    exit 0
fi

echo ""
echo "Stopping Waydroid..."

# Stop Waydroid services
waydroid session stop 2>/dev/null || true
sudo waydroid container stop 2>/dev/null || true

echo "Removing Waydroid package..."

# Remove Waydroid
sudo apt remove --purge waydroid -y
sudo apt autoremove -y

echo "Removing Waydroid data..."

# Remove Waydroid data
sudo rm -rf /var/lib/waydroid
rm -rf ~/.local/share/waydroid

echo "Removing Waydroid repository..."

# Remove repository
sudo rm -f /etc/apt/sources.list.d/waydroid.list
sudo apt update

echo "Removing kernel modules..."

# Remove binder module config
sudo rm -f /etc/modules-load.d/waydroid.conf

echo ""
echo -e "${GREEN}✅ Waydroid uninstalled successfully${NC}"
echo ""
