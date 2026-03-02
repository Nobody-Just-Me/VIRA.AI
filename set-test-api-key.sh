#!/bin/bash

# Set test API key for VIRA
# Usage: ./set-test-api-key.sh YOUR_GEMINI_API_KEY

if [ -z "$1" ]; then
    echo "❌ Error: API Key required"
    echo ""
    echo "Usage: ./set-test-api-key.sh YOUR_GEMINI_API_KEY"
    echo ""
    echo "Get your Gemini API Key from:"
    echo "https://makersuite.google.com/app/apikey"
    exit 1
fi

API_KEY="$1"

echo "🔑 Setting Gemini API Key..."
echo ""

# Set API key via adb
adb shell "run-as com.vira.assistant sh -c 'mkdir -p /data/data/com.vira.assistant/shared_prefs'"
adb shell "run-as com.vira.assistant sh -c 'echo \"<?xml version='\"'\"'1.0'\"'\"' encoding='\"'\"'utf-8'\"'\"'?>
<map>
    <string name='\"'\"'gemini_api_key'\"'\"'>$API_KEY</string>
</map>\" > /data/data/com.vira.assistant/shared_prefs/vira_settings.xml'"

echo "✅ API Key set successfully!"
echo ""
echo "📱 Please restart the app to load the new API key:"
echo "   adb shell am force-stop com.vira.assistant"
echo "   adb shell monkey -p com.vira.assistant -c android.intent.category.LAUNCHER 1"
