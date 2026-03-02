#!/bin/bash

# Script untuk mengatur ukuran layar Waydroid seperti HP Android
# Pilih dari berbagai preset ukuran HP populer

set -e

echo "📱 Waydroid Screen Size Configuration"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}Pilih ukuran layar HP Android:${NC}"
echo ""
echo "1. Samsung Galaxy S23 (1080x2340) - 6.1 inch"
echo "2. Samsung Galaxy S23 Ultra (1440x3088) - 6.8 inch"
echo "3. Google Pixel 8 (1080x2400) - 6.2 inch"
echo "4. Xiaomi Redmi Note 12 (1080x2400) - 6.67 inch"
echo "5. iPhone 14 Pro (1179x2556) - 6.1 inch"
echo "6. OnePlus 11 (1440x3216) - 6.7 inch"
echo "7. Realme 11 Pro (1080x2412) - 6.7 inch"
echo "8. Custom (input manual)"
echo "9. Portrait Mode (720x1280) - Budget phone"
echo "10. Tablet Mode (1200x1920) - 10 inch tablet"
echo ""

read -p "Pilih (1-10): " choice

case $choice in
    1)
        WIDTH=1080
        HEIGHT=2340
        DEVICE="Samsung Galaxy S23"
        ;;
    2)
        WIDTH=1440
        HEIGHT=3088
        DEVICE="Samsung Galaxy S23 Ultra"
        ;;
    3)
        WIDTH=1080
        HEIGHT=2400
        DEVICE="Google Pixel 8"
        ;;
    4)
        WIDTH=1080
        HEIGHT=2400
        DEVICE="Xiaomi Redmi Note 12"
        ;;
    5)
        WIDTH=1179
        HEIGHT=2556
        DEVICE="iPhone 14 Pro"
        ;;
    6)
        WIDTH=1440
        HEIGHT=3216
        DEVICE="OnePlus 11"
        ;;
    7)
        WIDTH=1080
        HEIGHT=2412
        DEVICE="Realme 11 Pro"
        ;;
    8)
        read -p "Masukkan lebar (width): " WIDTH
        read -p "Masukkan tinggi (height): " HEIGHT
        DEVICE="Custom"
        ;;
    9)
        WIDTH=720
        HEIGHT=1280
        DEVICE="Budget Phone"
        ;;
    10)
        WIDTH=1200
        HEIGHT=1920
        DEVICE="Tablet 10 inch"
        ;;
    *)
        echo "Pilihan tidak valid"
        exit 1
        ;;
esac

echo ""
echo -e "${YELLOW}⚙️  Mengatur ukuran layar...${NC}"
echo "Device: $DEVICE"
echo "Resolution: ${WIDTH}x${HEIGHT}"
echo ""

# Set Waydroid properties
waydroid prop set persist.waydroid.width $WIDTH
waydroid prop set persist.waydroid.height $HEIGHT

# Optional: Set DPI (density)
# Hitung DPI berdasarkan resolusi
if [ $WIDTH -ge 1440 ]; then
    DPI=560  # QHD
elif [ $WIDTH -ge 1080 ]; then
    DPI=420  # FHD
else
    DPI=320  # HD
fi

waydroid prop set persist.waydroid.dpi $DPI

echo -e "${GREEN}✅ Ukuran layar berhasil diatur${NC}"
echo ""
echo -e "${YELLOW}⚠️  Restart Waydroid untuk menerapkan perubahan:${NC}"
echo ""
echo "sudo waydroid container stop"
echo "sudo waydroid container start"
echo "waydroid session start &"
echo "waydroid show-full-ui"
echo ""

read -p "Restart Waydroid sekarang? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "Restarting Waydroid..."
    
    # Stop Waydroid
    waydroid session stop 2>/dev/null || true
    sleep 2
    sudo waydroid container stop
    sleep 2
    
    # Start Waydroid
    sudo waydroid container start
    sleep 3
    waydroid session start &
    sleep 5
    
    echo -e "${GREEN}✅ Waydroid restarted${NC}"
    echo ""
    echo "Tampilkan UI dengan: waydroid show-full-ui"
    echo "Launch VIRA dengan: waydroid app launch com.vira.assistant"
fi
