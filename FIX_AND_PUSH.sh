#!/bin/bash

echo "🔧 VIRA - Fix Conflict and Push"
echo "================================"
echo ""
echo "Repository sudah memiliki file. Kita akan merge dulu."
echo ""

# Pull with merge
echo "📥 Pulling from remote..."
git pull origin main --allow-unrelated-histories --no-edit

if [ $? -eq 0 ]; then
    echo "✅ Pull successful!"
    echo ""
    echo "📤 Pushing to remote..."
    git push -u origin main
    
    if [ $? -eq 0 ]; then
        echo ""
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        echo ""
        echo "✅ PUSH SUCCESSFUL!"
        echo ""
        echo "🎉 NEXT STEPS:"
        echo ""
        echo "1. Monitor build:"
        echo "   https://github.com/Nobody-Just-Me/VIRA/actions"
        echo ""
        echo "2. Wait ~10-15 minutes for build to complete"
        echo ""
        echo "3. Download APK from Artifacts"
        echo ""
        echo "4. Install on your Android phone"
        echo ""
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    else
        echo ""
        echo "❌ Push failed!"
        echo "Please check your credentials and try again."
    fi
else
    echo ""
    echo "⚠️  Pull failed or has conflicts"
    echo ""
    echo "Trying alternative method..."
    echo ""
    
    # Alternative: Force push
    read -p "Do you want to FORCE PUSH (will overwrite remote)? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "📤 Force pushing..."
        git push -u origin main --force
        
        if [ $? -eq 0 ]; then
            echo ""
            echo "✅ Force push successful!"
            echo ""
            echo "Monitor build at:"
            echo "https://github.com/Nobody-Just-Me/VIRA/actions"
        else
            echo ""
            echo "❌ Force push failed!"
        fi
    else
        echo "Cancelled."
    fi
fi
