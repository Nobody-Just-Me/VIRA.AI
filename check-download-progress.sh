#!/bin/bash

echo "📊 Waydroid Download Progress Monitor"
echo "======================================"
echo ""

TARGET_SIZE=1190  # MB
START_TIME=$(date +%s)

while pgrep -f "waydroid init" > /dev/null; do
    # Get current size
    CURRENT_SIZE=$(du -sm /var/lib/waydroid 2>/dev/null | cut -f1)
    
    if [ -n "$CURRENT_SIZE" ]; then
        # Calculate percentage
        PERCENT=$((CURRENT_SIZE * 100 / TARGET_SIZE))
        
        # Calculate elapsed time
        CURRENT_TIME=$(date +%s)
        ELAPSED=$((CURRENT_TIME - START_TIME))
        ELAPSED_MIN=$((ELAPSED / 60))
        ELAPSED_SEC=$((ELAPSED % 60))
        
        # Calculate speed
        if [ $ELAPSED -gt 0 ]; then
            SPEED=$((CURRENT_SIZE / ELAPSED))
            REMAINING=$((TARGET_SIZE - CURRENT_SIZE))
            ETA_SEC=$((REMAINING / SPEED))
            ETA_MIN=$((ETA_SEC / 60))
            ETA_SEC=$((ETA_SEC % 60))
        else
            SPEED=0
            ETA_MIN=0
            ETA_SEC=0
        fi
        
        # Create progress bar
        BAR_LENGTH=50
        FILLED=$((PERCENT * BAR_LENGTH / 100))
        BAR=$(printf "%${FILLED}s" | tr ' ' '█')
        EMPTY=$(printf "%$((BAR_LENGTH - FILLED))s" | tr ' ' '░')
        
        # Clear screen and show progress
        clear
        echo "📊 Waydroid Download Progress Monitor"
        echo "======================================"
        echo ""
        echo "📥 Downloading Android Image..."
        echo ""
        echo "  [${BAR}${EMPTY}] ${PERCENT}%"
        echo ""
        echo "  Downloaded: ${CURRENT_SIZE} MB / ${TARGET_SIZE} MB"
        echo "  Speed: ${SPEED} MB/s"
        echo "  Elapsed: ${ELAPSED_MIN}m ${ELAPSED_SEC}s"
        echo "  ETA: ${ETA_MIN}m ${ETA_SEC}s"
        echo ""
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        echo ""
        echo "💡 This is a one-time download"
        echo "   Next time VIRA will start instantly!"
        echo ""
        echo "⏳ Please wait... (Press Ctrl+C to stop monitoring)"
        echo ""
    fi
    
    sleep 5
done

clear
echo "✅ Download Complete!"
echo ""
echo "Waydroid is now setting up..."
echo "VIRA will launch automatically in a moment..."
