#!/bin/bash

# Build VIRA Latest Version

set -e

GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${GREEN}🚀 Building VIRA Latest Version${NC}"
echo "===================================="
echo ""

# Set Android SDK
if [ -z "$ANDROID_HOME" ]; then
    if [ -d "$HOME/Android/Sdk" ]; then
        export ANDROID_HOME="$HOME/Android/Sdk"
        echo -e "${BLUE}ANDROID_HOME: $ANDROID_HOME${NC}"
    else
        echo -e "${RED}❌ Android SDK not found${NC}"
        exit 1
    fi
fi

# Step 1: Clean previous builds
echo -e "${BLUE}Step 1: Cleaning previous builds...${NC}"
dotnet clean VIRA.Mobile/VIRA.Mobile.csproj -c Release > /dev/null 2>&1 || true
rm -rf VIRA.Mobile/bin/Release VIRA.Mobile/obj/Release
rm -rf VIRA.Shared/bin/Release VIRA.Shared/obj/Release

# Step 2: Restore dependencies
echo -e "${BLUE}Step 2: Restoring dependencies...${NC}"
dotnet restore

# Step 3: Build APK
echo -e "${BLUE}Step 3: Building APK...${NC}"
dotnet publish VIRA.Mobile/VIRA.Mobile.csproj \
    -f net8.0-android \
    -c Release \
    -p:AndroidPackageFormat=apk \
    -p:AndroidSdkDirectory=$ANDROID_HOME

if [ $? -eq 0 ]; then
    # Find and copy APK
    echo -e "${BLUE}Step 4: Locating APK...${NC}"
    APK_FILE=$(find VIRA.Mobile/bin/Release/net8.0-android -name "*.apk" -type f | head -n 1)
    
    if [ -n "$APK_FILE" ]; then
        # Backup old APK
        if [ -f "VIRA-Release.apk" ]; then
            mv VIRA-Release.apk VIRA-Release.apk.old
            echo "Old APK backed up to VIRA-Release.apk.old"
        fi
        
        # Copy new APK
        cp "$APK_FILE" "./VIRA-Release.apk"
        APK_SIZE=$(du -h "./VIRA-Release.apk" | cut -f1)
        APK_DATE=$(date '+%Y-%m-%d %H:%M:%S')
        
        echo ""
        echo -e "${GREEN}✅ Build successful!${NC}"
        echo ""
        echo -e "${BLUE}APK Details:${NC}"
        echo "  File: VIRA-Release.apk"
        echo "  Size: $APK_SIZE"
        echo "  Date: $APK_DATE"
        echo ""
        
        # Create build info file
        cat > VIRA-BUILD-INFO.txt << EOF
VIRA Build Information
======================
Build Date: $APK_DATE
APK Size: $APK_SIZE
APK Path: ./VIRA-Release.apk
Source APK: $APK_FILE

Build Configuration:
- Framework: net8.0-android
- Configuration: Release
- Android SDK: $ANDROID_HOME

Features Included:
- Voice recognition and TTS
- AI chatbot (Groq/Gemini/OpenAI)
- Task management
- Android integrations (calls, apps, settings)
- Weather and news
- Proactive suggestions
- All latest updates from source code

To install:
  adb install -r VIRA-Release.apk

To run in emulator:
  ./launch-emulator.sh
EOF
        
        echo -e "${GREEN}✅ Build info saved to VIRA-BUILD-INFO.txt${NC}"
        echo ""
        echo -e "${YELLOW}Next steps:${NC}"
        echo "  1. Launch emulator: ./launch-emulator.sh"
        echo "  2. Install APK: adb install -r VIRA-Release.apk"
        echo "  3. Configure API key (see SETUP_API_KEY.md)"
        echo ""
    else
        echo -e "${RED}❌ APK not found${NC}"
        exit 1
    fi
else
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi
