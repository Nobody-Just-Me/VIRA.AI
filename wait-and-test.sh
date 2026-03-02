#!/bin/bash

# Script untuk tunggu quota reset dan test lagi

API_KEY="$1"

if [ -z "$API_KEY" ]; then
    echo "Usage: ./wait-and-test.sh YOUR_API_KEY"
    exit 1
fi

echo "⏳ Menunggu quota reset..."
echo ""
echo "Quota akan reset dalam 60 detik..."

for i in {60..1}; do
    echo -ne "\r⏱️  Tunggu $i detik lagi...   "
    sleep 1
done

echo ""
echo ""
echo "✅ Waktu tunggu selesai! Testing API key..."
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
    echo "✅ API Key WORKING! Quota sudah reset!"
    echo ""
    echo "Response:"
    echo "$BODY" | jq -r '.candidates[0].content.parts[0].text' 2>/dev/null || echo "$BODY"
    echo ""
    echo "🎉 Sekarang Anda bisa menggunakan API key ini di VIRA!"
elif [ "$HTTP_STATUS" = "429" ]; then
    echo "❌ Masih quota exceeded. Tunggu lebih lama..."
    echo ""
    echo "Error details:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
else
    echo "❌ Error: HTTP $HTTP_STATUS"
    echo ""
    echo "Response:"
    echo "$BODY"
fi
