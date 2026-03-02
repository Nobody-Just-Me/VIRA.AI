#!/bin/bash

# Script untuk test Gemini API Key

echo "🔑 Gemini API Key Tester"
echo "========================"
echo ""

# Prompt for API key
read -p "Masukkan API Key Gemini Anda: " API_KEY

if [ -z "$API_KEY" ]; then
    echo "❌ API Key tidak boleh kosong!"
    exit 1
fi

echo ""
echo "📡 Testing API Key: ${API_KEY:0:10}..."
echo ""

# Test API call
RESPONSE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" \
  -H 'Content-Type: application/json' \
  -d '{
    "contents": [{
      "parts": [{
        "text": "Hello, test connection"
      }]
    }]
  }' \
  "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${API_KEY}")

# Extract HTTP status
HTTP_STATUS=$(echo "$RESPONSE" | grep "HTTP_STATUS" | cut -d: -f2)
BODY=$(echo "$RESPONSE" | sed '/HTTP_STATUS/d')

echo "📥 Response Status: $HTTP_STATUS"
echo ""

if [ "$HTTP_STATUS" = "200" ]; then
    echo "✅ API Key VALID dan WORKING!"
    echo ""
    echo "Response:"
    echo "$BODY" | jq -r '.candidates[0].content.parts[0].text' 2>/dev/null || echo "$BODY"
elif [ "$HTTP_STATUS" = "429" ]; then
    echo "❌ API Key QUOTA EXCEEDED!"
    echo ""
    echo "Error details:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
    echo ""
    echo "💡 Solusi:"
    echo "   1. Tunggu beberapa menit (quota reset setiap menit)"
    echo "   2. Atau gunakan API key lain"
    echo "   3. Atau upgrade ke paid plan di https://aistudio.google.com/"
elif [ "$HTTP_STATUS" = "400" ]; then
    echo "❌ API Key INVALID atau MODEL tidak tersedia!"
    echo ""
    echo "Error details:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
    echo ""
    echo "💡 Solusi:"
    echo "   1. Periksa API key Anda di https://aistudio.google.com/apikey"
    echo "   2. Pastikan API key sudah di-enable untuk Gemini API"
else
    echo "❌ Error: HTTP $HTTP_STATUS"
    echo ""
    echo "Response:"
    echo "$BODY"
fi

echo ""
echo "========================"
