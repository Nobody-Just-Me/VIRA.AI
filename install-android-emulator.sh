#!/bin/bash

# Install Android SDK Emulator (tanpa uninstall Waydroid dulu)
# Jalankan manual: sudo ./uninstall-waydroid.sh dulu jika perlu

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo "📱 Install Android SDK Emulator"
echo "================================"
echo ""

# Step 1: Install dependencies
echo -e "${BLUE}Step 1: Installing dependencies...${NC}"
echo "This will require sudo password..."
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

# Step 2: Download Android SDK Command Line Tools
echo -e "${BLUE}Step 2: Setting up Android SDK...${NC}"
ANDROID_HOME="$HOME/Android/Sdk"
mkdir -p "$ANDROID_HOME"
cd "$ANDROID_HOME"

if [ ! -f "cmdline-tools/latest/bin/sdkmanager" ]; then
    echo "Downloading SDK Command Line Tools (200MB)..."
    wget --progress=bar:force https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip -O cmdline-tools.zip
    echo "Extracting..."
    unzip -q cmdline-tools.zip
    mkdir -p cmdline-tools/latest
    mv cmdline-tools/* cmdline-tools/latest/ 2>/dev/null || true
    rm cmdline-tools.zip
    echo -e "${GREEN}✅ SDK Command Line Tools installed${NC}"
else
    echo -e "${GREEN}✅ SDK Command Line Tools already installed${NC}"
fi
echo ""

# Step 3: Setup environment variables
echo -e "${BLUE}Step 3: Setting up environment variables...${NC}"

# Remove old entries if exist
sed -i '/# Android SDK/,+4d' ~/.bashrc 2>/dev/null || true

# Add new entries
cat >> ~/.bashrc << 'EOF'

# Android SDK
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin
export PATH=$PATH:$ANDROID_HOME/platform-tools
export PATH=$PATH:$ANDROID_HOME/emulator
EOF

# Apply environment variables for current session
export ANDROID_HOME="$HOME/Android/Sdk"
export PATH="$PATH:$ANDROID_HOME/cmdline-tools/latest/bin"
export PATH="$PATH:$ANDROID_HOME/platform-tools"
export PATH="$PATH:$ANDROID_HOME/emulator"

echo -e "${GREEN}✅ Environment variables set${NC}"
echo ""

# Step 4: Accept licenses and install SDK packages
echo -e "${BLUE}Step 4: Installing Android SDK packages...${NC}"
echo "This will download ~1.5GB of data..."
echo ""

yes | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --licenses 2>/dev/null || true

echo "Installing platform-tools..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager "platform-tools"

echo "Installing build-tools..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager "build-tools;34.0.0"

echo "Installing Android 13 (API 33) platform..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager "platforms;android-33"

echo "Installing system images (this is the big download ~1GB)..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager "system-images;android-33;google_apis;x86_64"

echo "Installing emulator..."
$ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager "emulator"

echo -e "${GREEN}✅ SDK packages installed${NC}"
echo ""

# Step 5: Create AVD (Android Virtual Device)
echo -e "${BLUE}Step 5: Creating Android Virtual Device...${NC}"
AVD_NAME="VIRA_Pixel_6"

# Delete existing AVD if exists
$ANDROID_HOME/cmdline-tools/latest/bin/avdmanager delete avd -n "$AVD_NAME" 2>/dev/null || true

# Create new AVD
echo "no" | $ANDROID_HOME/cmdline-tools/latest/bin/avdmanager create avd \
    -n "$AVD_NAME" \
    -k "system-images;android-33;google_apis;x86_64" \
    -d "pixel_6" \
    --force

echo -e "${GREEN}✅ AVD created: $AVD_NAME${NC}"
echo ""

# Step 6: Configure AVD
echo -e "${BLUE}Step 6: Configuring AVD...${NC}"
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

# Step 7: Create launcher script
echo -e "${BLUE}Step 7: Creating launcher script...${NC}"

cat > "$HOME/launch-vira-emulator.sh" << 'EOFSCRIPT'
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

# Check if emulator is already running
if pgrep -f "emulator.*$AVD_NAME" > /dev/null; then
    echo -e "${YELLOW}⚠️  Emulator already running${NC}"
    echo ""
else
    # Start emulator
    echo -e "${BLUE}Starting emulator (this may take 1-2 minutes)...${NC}"
    $ANDROID_HOME/emulator/emulator -avd "$AVD_NAME" \
        -gpu auto \
        -no-snapshot-load &

    EMULATOR_PID=$!
    echo -e "${YELLOW}Waiting for emulator to boot...${NC}"
    $ANDROID_HOME/platform-tools/adb wait-for-device
    sleep 15
    echo -e "${GREEN}✅ Emulator booted${NC}"
fi

# Install VIRA
echo ""
echo -e "${BLUE}Installing VIRA...${NC}"
cd "$(dirname "$0")"
if [ -f "VIRA-Release.apk" ]; then
    $ANDROID_HOME/platform-tools/adb install -r VIRA-Release.apk 2>/dev/null || \
    $ANDROID_HOME/platform-tools/adb install VIRA-Release.apk
    echo -e "${GREEN}✅ VIRA installed${NC}"
    
    # Launch VIRA
    echo -e "${BLUE}Launching VIRA...${NC}"
    sleep 2
    $ANDROID_HOME/platform-tools/adb shell am start -n com.vira.assistant/.MainActivity
    echo -e "${GREEN}✅ VIRA launched${NC}"
else
    echo -e "${YELLOW}⚠️  VIRA-Release.apk not found in current directory${NC}"
fi

echo ""
echo -e "${GREEN}✅ Emulator is running!${NC}"
echo ""
echo -e "${YELLOW}Setup steps in VIRA:${NC}"
echo "  1. Grant permissions (microphone, storage)"
echo "  2. Open Settings → Add API key"
echo "  3. Test: 'Hello VIRA'"
echo ""
echo -e "${BLUE}Useful commands:${NC}"
echo "  View logs:     adb logcat | grep VIRA"
echo "  Restart app:   adb shell am force-stop com.vira.assistant"
echo "  Uninstall:     adb uninstall com.vira.assistant"
echo ""
echo -e "${BLUE}To stop emulator: Close the emulator window${NC}"
echo ""
EOFSCRIPT

chmod +x "$HOME/launch-vira-emulator.sh"

# Create symlink in current directory
cd ~/Data1/Test\ ai/VIRA-UNO 2>/dev/null || cd "$(dirname "$0")"
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
echo -e "${YELLOW}IMPORTANT: Restart your terminal or run:${NC}"
echo "  source ~/.bashrc"
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
