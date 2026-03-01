#!/bin/bash

# VIRA Build Script
# This script automates the APK build process

set -e  # Exit on error

echo "🚀 VIRA Build Script"
echo "===================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK not found. Please install .NET 8 SDK${NC}"
    echo "   Download from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

echo -e "${GREEN}✅ .NET SDK found: $(dotnet --version)${NC}"

# Check Android SDK
if [ -z "$ANDROID_HOME" ]; then
    echo -e "${YELLOW}⚠️  ANDROID_HOME not set. Trying common locations...${NC}"
    
    # Try common Android SDK locations
    if [ -d "$HOME/Android/Sdk" ]; then
        export ANDROID_HOME="$HOME/Android/Sdk"
    elif [ -d "$HOME/Library/Android/sdk" ]; then
        export ANDROID_HOME="$HOME/Library/Android/sdk"
    elif [ -d "/usr/local/android-sdk" ]; then
        export ANDROID_HOME="/usr/local/android-sdk"
    else
        echo -e "${RED}❌ Android SDK not found${NC}"
        exit 1
    fi
fi

echo -e "${GREEN}✅ Android SDK found: $ANDROID_HOME${NC}"

# Clean previous builds
echo ""
echo "🧹 Cleaning previous builds..."
dotnet clean VIRA.Mobile/VIRA.Mobile.csproj -c Release

# Restore dependencies
echo ""
echo "📦 Restoring dependencies..."
dotnet restore

# Build configuration
BUILD_CONFIG="Release"
OUTPUT_DIR="VIRA.Mobile/bin/$BUILD_CONFIG/net8.0-android"

# Build APK
echo ""
echo "🔨 Building APK..."
echo "   Configuration: $BUILD_CONFIG"
echo "   Target: net8.0-android"
echo ""

dotnet publish VIRA.Mobile/VIRA.Mobile.csproj \
    -f net8.0-android \
    -c $BUILD_CONFIG \
    -p:AndroidPackageFormat=apk \
    -p:AndroidSdkDirectory=$ANDROID_HOME

# Check if build succeeded
if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ Build successful!${NC}"
    echo ""
    echo "📱 APK Location:"
    
    # Find the APK file
    APK_FILE=$(find $OUTPUT_DIR -name "*.apk" -type f | head -n 1)
    
    if [ -n "$APK_FILE" ]; then
        echo "   $APK_FILE"
        
        # Get APK size
        APK_SIZE=$(du -h "$APK_FILE" | cut -f1)
        echo "   Size: $APK_SIZE"
        
        # Copy to easy location
        cp "$APK_FILE" "./VIRA-Release.apk"
        echo ""
        echo -e "${GREEN}✅ APK copied to: ./VIRA-Release.apk${NC}"
        
        echo ""
        echo "📲 Installation Instructions:"
        echo "   1. Transfer VIRA-Release.apk to your Android device"
        echo "   2. Enable 'Install from Unknown Sources' in Settings"
        echo "   3. Tap the APK file to install"
        echo "   4. Open VIRA and enter your Gemini API Key in Settings"
        
        echo ""
        echo "🔧 Or install via ADB:"
        echo "   adb install -r ./VIRA-Release.apk"
        
    else
        echo -e "${RED}❌ APK file not found in output directory${NC}"
        exit 1
    fi
else
    echo ""
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

echo ""
echo "🎉 Done!"
