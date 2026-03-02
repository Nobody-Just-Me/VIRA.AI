#!/bin/bash

# Script untuk start Android Emulator

echo "🚀 Starting Android Emulator"
echo "============================"
echo ""

export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/emulator:$ANDROID_HOME/platform-tools

# Check if emulator exists
if [ ! -f "$ANDROID_HOME/emulator/emulator" ]; then
    echo "❌ Android Emulator not found"
    echo "Please run: ./install-android-studio.sh"
    exit 1
fi

# List available AVDs
echo "Available emulators:"
$ANDROID_HOME/emulator/emulator -list-avds
echo ""

# Get first AVD or use default
AVD_NAME=$($ANDROID_HOME/emulator/emulator -list-avds | head -n 1)

if [ -z "$AVD_NAME" ]; then
    echo "❌ No emulator found"
    echo "Please run: ./setup-android-emulator.sh"
    exit 1
fi

echo "Starting emulator: $AVD_NAME"
echo ""
echo "💡 Tips:"
echo "   - Emulator will open in a new window"
echo "   - First boot may take 2-3 minutes"
echo "   - Press Ctrl+C here to stop emulator"
echo ""

# Start emulator
$ANDROID_HOME/emulator/emulator -avd "$AVD_NAME" \
    -gpu auto \
    -no-snapshot-load
