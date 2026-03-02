using System.Net.Http.Json;
using System.Text.Json;

namespace VIRA.Shared.Services;

public class ElevenLabsTTSService : ITTSService
{
    private readonly HttpClient _httpClient;
    private string _apiKey = string.Empty;
    
    // Voice ID dari ElevenLabs Pre-made Voices (Free Tier Compatible)
    // Rachel - Female, calm, natural voice (FREE)
    private const string VoiceId = "21m00Tcm4TlvDq8ikWAM";
    private const string BaseUrl = "https://api.elevenlabs.io/v1";
    
    public ElevenLabsTTSService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
        Android.Util.Log.Info("VIRA_ElevenLabs", $"API Key set (length: {apiKey.Length})");
    }
    
    public async Task<bool> TestConnectionAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Android.Util.Log.Error("VIRA_ElevenLabs", "API Key is empty");
            return false;
        }
        
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("xi-api-key", _apiKey);
            
            var response = await _httpClient.GetAsync($"{BaseUrl}/user");
            
            if (response.IsSuccessStatusCode)
            {
                Android.Util.Log.Info("VIRA_ElevenLabs", "✅ Connection test successful");
                return true;
            }
            else
            {
                Android.Util.Log.Error("VIRA_ElevenLabs", $"❌ Connection test failed: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("VIRA_ElevenLabs", $"❌ Connection test error: {ex.Message}");
            return false;
        }
    }
    
    public async Task<byte[]> SynthesizeSpeechAsync(string text)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Android.Util.Log.Error("VIRA_ElevenLabs", "API Key is empty!");
            throw new InvalidOperationException("ElevenLabs API Key belum diatur. Silakan masukkan API Key di Settings.");
        }
        
        Android.Util.Log.Info("VIRA_ElevenLabs", $"🎤 Synthesizing speech for text: {text.Substring(0, Math.Min(50, text.Length))}...");
        
        try
        {
            var requestBody = new
            {
                text = text,
                model_id = "eleven_multilingual_v2",
                voice_settings = new
                {
                    stability = 0.5,
                    similarity_boost = 0.75,
                    style = 0.0,
                    use_speaker_boost = true
                }
            };
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("xi-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "audio/mpeg");
            
            var url = $"{BaseUrl}/text-to-speech/{VoiceId}";
            Android.Util.Log.Info("VIRA_ElevenLabs", $"📡 Sending request to: {url}");
            
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            
            Android.Util.Log.Info("VIRA_ElevenLabs", $"📥 Response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Android.Util.Log.Error("VIRA_ElevenLabs", $"❌ ElevenLabs API Error: {response.StatusCode}");
                Android.Util.Log.Error("VIRA_ElevenLabs", $"❌ Error details: {error}");
                
                string userFriendlyError = "Maaf, terjadi kesalahan saat menghasilkan suara.";
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    userFriendlyError = "❌ ElevenLabs API Key tidak valid.\n\n" +
                        "💡 Solusi:\n" +
                        "1. Periksa API Key di Settings\n" +
                        "2. Dapatkan API Key baru di https://elevenlabs.io/";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    userFriendlyError = "⚠️ ElevenLabs API quota exceeded.\n\n" +
                        "💡 Solusi:\n" +
                        "1. Tunggu beberapa saat\n" +
                        "2. Atau upgrade plan di https://elevenlabs.io/\n\n" +
                        "Free tier: 10,000 karakter/bulan";
                }
                
                throw new Exception(userFriendlyError);
            }
            
            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            Android.Util.Log.Info("VIRA_ElevenLabs", $"✅ Got audio data (size: {audioBytes.Length} bytes)");
            
            return audioBytes;
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("VIRA_ElevenLabs", $"❌ Error synthesizing speech: {ex.Message}");
            throw;
        }
    }
}
