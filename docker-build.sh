#!/bin/bash

# Build VIRA using Docker
# This includes all dependencies in the container

echo "🐳 Building VIRA with Docker"
echo "============================="
echo ""
echo "This will:"
echo "  1. Build a Docker image with Android SDK (~2 GB)"
echo "  2. Compile VIRA APK inside container"
echo "  3. Copy APK to current directory"
echo ""
echo "⚠️  First time will take 20-30 minutes"
echo "    Subsequent builds will be faster"
echo ""

# Build Docker image
echo "📦 Building Docker image..."
docker build -t vira-builder .

if [ $? -ne 0 ]; then
    echo "❌ Docker build failed"
    exit 1
fi

# Run container and copy APK
echo ""
echo "📱 Extracting APK..."
CONTAINER_ID=$(docker create vira-builder)
docker cp $CONTAINER_ID:/app/output/. ./output/
docker rm $CONTAINER_ID

# Find APK
APK_FILE=$(find ./output -name "*.apk" -type f | head -n 1)

if [ -n "$APK_FILE" ]; then
    cp "$APK_FILE" ./VIRA-Release.apk
    echo ""
    echo "✅ Build successful!"
    echo "📱 APK: ./VIRA-Release.apk"
    echo ""
    echo "Install with:"
    echo "  adb install -r VIRA-Release.apk"
else
    echo "❌ APK not found"
    exit 1
fi
