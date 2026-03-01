#!/bin/bash

# Script untuk install Android SDK di Linux
# Ini akan download ~1-2 GB data

set -e

echo "🤖 Android SDK Installation Script"
echo "===================================="
echo ""
echo "⚠️  WARNING: This will download ~1-2 GB of data"
echo "    Make sure you have stable internet connection"
echo ""
read -p "Continue? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    exit 1
fi

# Create directory
ANDROID_HOME="$HOME/Android/Sdk"
mkdir -p "$ANDROID_HOME"

# Download command line tools
echo ""
echo "📥 Downloading Android Command Line Tools..."
cd /tmp
wget -q --show-progress https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip -O cmdline-tools.zip

echo "📦 Extracting..."
unzip -q cmdline-tools.zip
mkdir -p "$ANDROID_HOME/cmdline-tools/latest"
mv cmdline-tools/* "$ANDROID_HOME/cmdline-tools/latest/"
rm -rf cmdline-tools cmdline-tools.zip

# Set environment
export ANDROID_HOME="$HOME/Android/Sdk"
export PATH="$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools"

# Accept licenses
echo ""
echo "📜 Accepting licenses..."
yes | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --licenses

# Install required packages
echo ""
echo "📦 Installing Android SDK packages..."
echo "    This may take 10-20 minutes..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager \
    "platform-tools" \
    "platforms;android-34" \
    "build-tools;34.0.0" \
    "ndk;26.1.10909125"

# Add to bashrc
echo ""
echo "💾 Adding to ~/.bashrc..."
if ! grep -q "ANDROID_HOME" ~/.bashrc; then
    echo "" >> ~/.bashrc
    echo "# Android SDK" >> ~/.bashrc
    echo "export ANDROID_HOME=\$HOME/Android/Sdk" >> ~/.bashrc
    echo "export PATH=\$PATH:\$ANDROID_HOME/cmdline-tools/latest/bin:\$ANDROID_HOME/platform-tools" >> ~/.bashrc
fi

echo ""
echo "✅ Android SDK installed successfully!"
echo ""
echo "📍 Location: $ANDROID_HOME"
echo ""
echo "⚠️  IMPORTANT: Run this command to apply changes:"
echo "    source ~/.bashrc"
echo ""
echo "Then you can build VIRA:"
echo "    cd VIRA-UNO"
echo "    ./build.sh"
