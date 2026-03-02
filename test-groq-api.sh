#!/bin/bash

# Script untuk test Groq API Key

echo "🚀 Groq API Key Tester"
echo "========================"
echo ""

# Prompt for API key
read -p "Masukkan Groq API Key Anda (gsk_...): " API_KEY

if [ -z "$API_KEY" ]; then
    echo "❌ API Key tidak boleh kosong!"
    exit 1
fi

echo ""
echo "📡 Testing Groq API Key: ${API_KEY:0:10}..."
echo ""

# Test API call
RESPONSE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer ${API_KEY}" \
  -d '{
    "model": "llama-3.3-70b-versatile",
    "messages": [
      {
        "role": "user",
        "content": "Hello, test connection"
      }
    ]
  }' \
  "https://api.groq.com/openai/v1/chat/completions")

# Extract HTTP status
HTTP_STATUS=$(echo "$RESPONSE" | grep "HTTP_STATUS" | cut -d: -f2)
BODY=$(echo "$RESPONSE" | sed '/HTTP_STATUS/d')

echo "📥 Response Status: $HTTP_STATUS"
echo ""

if [ "$HTTP_STATUS" = "200" ]; then
    echo "✅ Groq API Key VALID dan WORKING!"
    echo ""
    echo "Response:"
    echo "$BODY" | jq -r '.choices[0].message.content' 2>/dev/null || echo "$BODY"
    echo ""
    echo "🎉 Sekarang Anda bisa menggunakan Groq API key ini di VIRA!"
    echo ""
    echo "💡 Keunggulan Groq:"
    echo "   - 30 requests per minute (vs Gemini: 15 RPM)"
    echo "   - 14,400 requests per day (vs Gemini: 1,500 RPD)"
    echo "   - Sangat cepat (inference < 1 detik)"
    echo "   - Model: Llama 3.1 70B"
elif [ "$HTTP_STATUS" = "429" ]; then
    echo "❌ Groq API Key QUOTA EXCEEDED!"
    echo ""
    echo "Error details:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
    echo ""
    echo "💡 Solusi:"
    echo "   1. Tunggu beberapa menit (quota reset setiap menit)"
    echo "   2. Atau gunakan API key lain"
elif [ "$HTTP_STATUS" = "401" ]; then
    echo "❌ Groq API Key INVALID atau UNAUTHORIZED!"
    echo ""
    echo "Error details:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
    echo ""
    echo "💡 Solusi:"
    echo "   1. Periksa API key Anda di https://console.groq.com/"
    echo "   2. Pastikan API key format: gsk_..."
    echo "   3. Buat API key baru jika perlu"
elif [ "$HTTP_STATUS" = "400" ]; then
    echo "❌ Bad Request - API Key atau Request Error!"
    echo ""
    echo "Error details:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
    echo ""
    echo "💡 Solusi:"
    echo "   1. Periksa format API key (harus: gsk_...)"
    echo "   2. Pastikan API key valid"
else
    echo "❌ Error: HTTP $HTTP_STATUS"
    echo ""
    echo "Response:"
    echo "$BODY"
fi

echo ""
echo "========================"
echo ""
echo "📚 Cara Mendapatkan Groq API Key:"
echo "   1. Buka https://console.groq.com/"
echo "   2. Sign up dengan Google/GitHub (GRATIS)"
echo "   3. Pergi ke 'API Keys'"
echo "   4. Klik 'Create API Key'"
echo "   5. Copy API key (format: gsk_...)"
echo ""
echo "🆓 Groq Free Tier:"
echo "   - 30 requests per minute"
echo "   - 14,400 requests per day"
echo "   - 7,000 tokens per minute"
echo "   - GRATIS selamanya!"
