#!/bin/bash

# Script untuk uninstall Waydroid dan install Android SDK Emulator

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo "🔄 Uninstall Waydroid & Install Android Emulator"
echo "================================================"
echo ""

# Step 1: Stop Waydroid
echo -e "${BLUE}Step 1: Stopping Waydroid...${NC}"
waydroid session stop 2>/dev/null || true
sleep 1
sudo waydroid container stop 2>/dev/null || true
sleep 1
pkill -9 -f waydroid 2>/dev/null || true
pkill -9 -f weston 2>/dev/null || true
pkill -9 -f cage 2>/dev/null || true
sleep 2
echo -e "${GREEN}✅ Waydroid stopped${NC}"

# Step 2: Uninstall Waydroid
echo -e "${BLUE}Step 2: Uninstalling Waydroid...${NC}"
sudo apt remove --purge -y waydroid 2>/dev/null || true
sudo rm -rf /var/lib/waydroid 2>/dev/null || true
sudo rm -rf ~/.local/share/waydroid 2>/dev/null || true
sudo rm -rf /etc/waydroid 2>/dev/null || true
echo -e "${GREEN}✅ Waydroid uninstalled${NC}"
echo ""

# Step 3: Install dependencies
echo -e "${BLUE}Step 3: Installing dependencies...${NC}"
sudo apt update
sudo apt install -y \
    openjdk-17-jdk \
    wget \
    unzip \
    lib32z1 \
    lib32ncurses6 \
    lib32stdc++6 \
    libc6-i386 \
    qemu-kvm \
    libvirt-daemon-system \
    libvirt-clients \
    bridge-utils

echo -e "${GREEN}✅ Dependencies installed${NC}"
echo ""

# Step 4: Download Android SDK Command Line Tools
echo -e "${BLUE}Step 4: Downloading Android SDK Command Line Tools...${NC}"
ANDROID_HOME="$HOME/Android/Sdk"
mkdir -p "$ANDROID_HOME"
cd "$ANDROID_HOME"

if [ ! -f "cmdline-tools/latest/bin/sdkmanager" ]; then
    echo "Downloading SDK Command Line Tools..."
    wget -q https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip -O cmdline-tools.zip
    unzip -q cmdline-tools.zip
    mkdir -p cmdline-tools/latest
    mv cmdline-tools/* cmdline-tools/latest/ 2>/dev/null || true
    rm cmdline-tools.zip
    echo -e "${GREEN}✅ SDK Command Line Tools downloaded${NC}"
else
    echo -e "${GREEN}✅ SDK Command Line Tools already installed${NC}"
fi
echo ""

# Step 5: Setup environment variables
echo -e "${BLUE}Step 5: Setting up environment variables...${NC}"
cat >> ~/.bashrc << 'EOF'

# Android SDK
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin
export PATH=$PATH:$ANDROID_HOME/platform-tools
export PATH=$PATH:$ANDROID_HOME/emulator
EOF

# Apply environment variables
export ANDROID_HOME="$HOME/Android/Sdk"
export PATH="$PATH:$ANDROID_HOME/cmdline-tools/latest/bin"
export PATH="$PATH:$ANDROID_HOME/platform-tools"
export PATH="$PATH:$ANDROID_HOME/emulator"

echo -e "${GREEN}✅ Environment variables set${NC}"
echo ""

# Step 6: Accept licenses and install SDK packages
echo -e "${BLUE}Step 6: Installing Android SDK packages...${NC}"
yes | sdkmanager --licenses 2>/dev/null || true

echo "Installing platform-tools..."
sdkmanager "platform-tools"

echo "Installing build-tools..."
sdkmanager "build-tools;34.0.0"

echo "Installing Android 13 (API 33) platform..."
sdkmanager "platforms;android-33"

echo "Installing system images..."
sdkmanager "system-images;android-33;google_apis;x86_64"

echo "Installing emulator..."
sdkmanager "emulator"

echo -e "${GREEN}✅ SDK packages installed${NC}"
echo ""

# Step 7: Create AVD (Android Virtual Device)
echo -e "${BLUE}Step 7: Creating Android Virtual Device...${NC}"
AVD_NAME="VIRA_Pixel_6"

# Delete existing AVD if exists
avdmanager delete avd -n "$AVD_NAME" 2>/dev/null || true

# Create new AVD
echo "no" | avdmanager create avd \
    -n "$AVD_NAME" \
    -k "system-images;android-33;google_apis;x86_64" \
    -d "pixel_6" \
    --force

echo -e "${GREEN}✅ AVD created: $AVD_NAME${NC}"
echo ""

# Step 8: Configure AVD
echo -e "${BLUE}Step 8: Configuring AVD...${NC}"
AVD_CONFIG="$HOME/.android/avd/${AVD_NAME}.avd/config.ini"

if [ -f "$AVD_CONFIG" ]; then
    # Set hardware properties
    cat >> "$AVD_CONFIG" << EOF
hw.keyboard=yes
hw.ramSize=2048
hw.gpu.enabled=yes
hw.gpu.mode=auto
disk.dataPartition.size=2048M
EOF
    echo -e "${GREEN}✅ AVD configured${NC}"
fi
echo ""

# Step 9: Create launcher script
echo -e "${BLUE}Step 9: Creating launcher scripts...${NC}"

cat > "$HOME/launch-vira-emulator.sh" << 'EOF'
#!/bin/bash

# VIRA Android Emulator Launcher

GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

export ANDROID_HOME="$HOME/Android/Sdk"
export PATH="$PATH:$ANDROID_HOME/emulator"
export PATH="$PATH:$ANDROID_HOME/platform-tools"

AVD_NAME="VIRA_Pixel_6"

echo -e "${GREEN}🚀 Starting VIRA Android Emulator${NC}"
echo ""

# Start emulator
echo -e "${BLUE}Starting emulator...${NC}"
$ANDROID_HOME/emulator/emulator -avd "$AVD_NAME" \
    -gpu auto \
    -no-snapshot-load \
    -wipe-data &

EMULATOR_PID=$!
echo -e "${YELLOW}Waiting for emulator to boot...${NC}"
adb wait-for-device
sleep 10

# Install VIRA
echo -e "${BLUE}Installing VIRA...${NC}"
cd "$(dirname "$0")"
if [ -f "VIRA-Release.apk" ]; then
    adb install -r VIRA-Release.apk
    echo -e "${GREEN}✅ VIRA installed${NC}"
    
    # Launch VIRA
    echo -e "${BLUE}Launching VIRA...${NC}"
    adb shell am start -n com.vira.assistant/.MainActivity
    echo -e "${GREEN}✅ VIRA launched${NC}"
else
    echo -e "${YELLOW}⚠️  VIRA-Release.apk not found${NC}"
fi

echo ""
echo -e "${GREEN}✅ Emulator is running!${NC}"
echo ""
echo -e "${YELLOW}Setup steps:${NC}"
echo "  1. Grant permissions in VIRA"
echo "  2. Add API key in Settings"
echo "  3. Test voice commands"
echo ""
echo -e "${BLUE}To stop: Close emulator window${NC}"
echo ""

wait $EMULATOR_PID
EOF

chmod +x "$HOME/launch-vira-emulator.sh"
ln -sf "$HOME/launch-vira-emulator.sh" ./launch-emulator.sh

echo -e "${GREEN}✅ Launcher script created${NC}"
echo ""

# Summary
echo ""
echo -e "${GREEN}================================================${NC}"
echo -e "${GREEN}✅ Installation Complete!${NC}"
echo -e "${GREEN}================================================${NC}"
echo ""
echo -e "${BLUE}Android SDK installed at:${NC} $ANDROID_HOME"
echo -e "${BLUE}AVD created:${NC} $AVD_NAME"
echo ""
echo -e "${YELLOW}To start VIRA emulator:${NC}"
echo "  ./launch-emulator.sh"
echo ""
echo -e "${YELLOW}Useful commands:${NC}"
echo "  List AVDs:     avdmanager list avd"
echo "  List devices:  adb devices"
echo "  Install APK:   adb install VIRA-Release.apk"
echo "  View logs:     adb logcat"
echo ""
echo -e "${BLUE}Note:${NC} Restart terminal or run: source ~/.bashrc"
echo ""
