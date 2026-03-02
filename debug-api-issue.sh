#!/bin/bash

echo "🔍 VIRA API Debug Script"
echo "========================"
echo ""

# Check if app is running
echo "1. Checking if VIRA is running..."
APP_PID=$(adb shell pidof com.vira.assistant)
if [ -z "$APP_PID" ]; then
    echo "   ❌ VIRA not running. Launching..."
    adb shell monkey -p com.vira.assistant -c android.intent.category.LAUNCHER 1 > /dev/null 2>&1
    sleep 3
else
    echo "   ✅ VIRA is running (PID: $APP_PID)"
fi

echo ""
echo "2. Checking API Key in SharedPreferences..."
API_KEY=$(adb shell "run-as com.vira.assistant cat /data/data/com.vira.assistant/shared_prefs/vira_settings.xml 2>/dev/null | grep gemini_api_key" 2>/dev/null)

if [ -z "$API_KEY" ]; then
    echo "   ❌ No API Key found in preferences"
    echo ""
    echo "   Please set API Key manually:"
    echo "   1. Open VIRA app"
    echo "   2. Tap Settings (⚙️)"
    echo "   3. Enter Gemini API Key"
    echo "   4. Tap 'Save Configuration'"
else
    echo "   ✅ API Key found in preferences"
    echo "   $API_KEY"
fi

echo ""
echo "3. Monitoring logs for API calls..."
echo "   (Send a message in VIRA now)"
echo ""
echo "   Press Ctrl+C to stop"
echo "   ================================"
echo ""

# Monitor logs
adb logcat -c
adb logcat | grep -E "VIRA_MainActivity|VIRA_Gemini|AndroidRuntime" --line-buffered | while read line; do
    echo "$line"
done
