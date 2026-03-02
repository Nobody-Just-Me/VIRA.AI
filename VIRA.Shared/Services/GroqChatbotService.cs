using System.Net.Http.Json;
using System.Text.Json;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

public class GroqChatbotService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private string _apiKey = string.Empty;
    private const string BaseUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string DefaultModel = "llama-3.3-70b-versatile";

    public GroqChatbotService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _weatherService = new WeatherApiService(httpClient);
        _newsService = new NewsApiService(httpClient);
    }
    
    public void SetWeatherApiKey(string apiKey)
    {
        _weatherService.SetApiKey(apiKey);
    }
    
    public void SetNewsApiKey(string apiKey)
    {
        _newsService.SetApiKey(apiKey);
    }

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<bool> TestConnectionAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
            return false;

        try
        {
            var response = await SendMessageAsync("Hello", new List<ChatMessage>());
            return !string.IsNullOrEmpty(response.Content);
        }
        catch
        {
            return false;
        }
    }

    public async Task<ChatMessage> SendMessageAsync(string userMessage, List<ChatMessage> conversationHistory)
    {
        Android.Util.Log.Info("VIRA_Groq", $"SendMessageAsync called with message: {userMessage}");
        Android.Util.Log.Info("VIRA_Groq", $"📋 Current API Key: {_apiKey.Substring(0, Math.Min(10, _apiKey.Length))}... (length: {_apiKey.Length})");
        
        if (string.IsNullOrEmpty(_apiKey))
        {
            Android.Util.Log.Error("VIRA_Groq", "API Key is empty!");
            throw new InvalidOperationException("Groq API Key belum diatur. Silakan masukkan API Key di Settings.");
        }

        Android.Util.Log.Info("VIRA_Groq", "API Key is set, building request...");
        
        var systemPrompt = GetSystemPrompt();
        var messages = BuildMessages(systemPrompt, conversationHistory, userMessage);

        var requestBody = new
        {
            model = DefaultModel,
            messages,
            temperature = 0.9,
            max_tokens = 1024,
            top_p = 0.95
        };

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        
        Android.Util.Log.Info("VIRA_Groq", $"📡 Sending POST request to Groq API...");
        Android.Util.Log.Info("VIRA_Groq", $"   Model: {DefaultModel}");
        
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, requestBody);
        
        Android.Util.Log.Info("VIRA_Groq", $"📥 Response status: {response.StatusCode}");
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Android.Util.Log.Error("VIRA_Groq", $"❌ Groq API Error: {response.StatusCode}");
            Android.Util.Log.Error("VIRA_Groq", $"❌ Error details: {error}");
            
            // Parse error for better user message
            string userFriendlyError = "Maaf, terjadi kesalahan saat menghubungi Groq API.";
            
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                userFriendlyError = "⚠️ Groq API Key Anda telah mencapai batas quota.\n\n" +
                    "💡 Solusi:\n" +
                    "1. Tunggu beberapa menit (quota reset otomatis)\n" +
                    "2. Atau gunakan API Key lain di Settings\n\n" +
                    "Groq Free Tier: 30 requests/minute, 14,400 requests/day";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                userFriendlyError = "❌ Groq API Key tidak valid atau request error.\n\n" +
                    "💡 Solusi:\n" +
                    "1. Periksa API Key di Settings\n" +
                    "2. Pastikan API Key format: gsk_...\n" +
                    "3. Dapatkan API Key baru di https://console.groq.com/";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                userFriendlyError = "🔒 Groq API Key tidak valid atau tidak memiliki akses.\n\n" +
                    "💡 Solusi:\n" +
                    "1. Periksa API Key di Settings\n" +
                    "2. Pastikan API Key benar dan aktif\n" +
                    "3. Dapatkan API Key baru di https://console.groq.com/";
            }
            
            // Return error as chat message instead of throwing
            return new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = userFriendlyError,
                Type = MessageType.Text,
                Timestamp = DateTime.Now
            };
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var text = result
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "Maaf, saya tidak dapat memproses permintaan Anda.";

        Android.Util.Log.Info("VIRA_Groq", $"✅ Got response from Groq (length: {text.Length})");
        Android.Util.Log.Info("VIRA_Groq", $"   First 100 chars: {text.Substring(0, Math.Min(100, text.Length))}...");
        
        return await ParseResponseAsync(text, userMessage);
    }

    private string GetSystemPrompt()
    {
        return @"Nama Anda adalah Vira. Anda adalah asisten pribadi AI perempuan yang cerdas, proaktif, dan elegan.

IDENTITAS:
- Anda berbicara dalam Bahasa Indonesia yang natural dan bersahabat
- Anda membantu pengguna dengan rutinitas harian mereka
- Anda memberikan jawaban yang ringkas namun informatif
- Anda proaktif dalam memberikan saran yang berguna

KEMAMPUAN:
- Menjawab pertanyaan umum
- Memberikan informasi cuaca
- Menampilkan jadwal dan reminder
- Mencari berita terkini
- Mengecek kondisi lalu lintas
- Memberikan saran dan rekomendasi

GAYA KOMUNIKASI:
- Ramah dan hangat
- Profesional namun tidak kaku
- Menggunakan emoji secara bijak untuk menambah kehangatan
- Jawaban singkat dan to-the-point

Lokasi default pengguna: Surabaya, Indonesia
Waktu saat ini: " + DateTime.Now.ToString("dd MMMM yyyy, HH:mm") + @"

Jika pengguna menanyakan cuaca, jadwal, berita, atau lalu lintas, berikan respons yang terstruktur.";
    }

    private List<object> BuildMessages(string systemPrompt, List<ChatMessage> history, string userMessage)
    {
        var messages = new List<object>
        {
            new { role = "system", content = systemPrompt }
        };

        foreach (var msg in history.TakeLast(10))
        {
            var role = msg.Role == ChatMessageRole.User ? "user" : "assistant";
            messages.Add(new { role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = userMessage });

        return messages;
    }

    private async Task<ChatMessage> ParseResponseAsync(string text, string userQuery)
    {
        var message = new ChatMessage
        {
            Role = ChatMessageRole.Assistant,
            Content = text,
            Type = DetectMessageType(userQuery, text)
        };

        // DO NOT parse structured responses - always use text from API
        // Rich cards use mock data which doesn't match API response
        // So we keep Weather, Schedule, News, etc. as NULL
        // This forces MainActivity to display message.Content instead
        
        Android.Util.Log.Info("VIRA_Groq", $"ParseResponseAsync: Type={message.Type}, keeping structured data NULL");
        Android.Util.Log.Info("VIRA_Groq", $"Content will be displayed as text: {text.Substring(0, Math.Min(100, text.Length))}...");

        return message;
    }

    private MessageType DetectMessageType(string query, string response)
    {
        var lowerQuery = query.ToLower();
        
        Android.Util.Log.Info("VIRA_Groq", $"DetectMessageType - Query: {lowerQuery}");
        
        if (lowerQuery.Contains("cuaca") || lowerQuery.Contains("weather") || lowerQuery.Contains("suhu") || lowerQuery.Contains("temperature"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: Weather");
            return MessageType.Weather;
        }
        
        if (lowerQuery.Contains("jadwal") || lowerQuery.Contains("schedule") || lowerQuery.Contains("agenda"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: Schedule");
            return MessageType.Schedule;
        }
        
        if (lowerQuery.Contains("berita") || lowerQuery.Contains("news") || lowerQuery.Contains("headline"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: News");
            return MessageType.News;
        }
        
        if (lowerQuery.Contains("lalu lintas") || lowerQuery.Contains("traffic") || lowerQuery.Contains("macet"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: Traffic");
            return MessageType.Traffic;
        }
        
        if (lowerQuery.Contains("reminder") || lowerQuery.Contains("ingat") || lowerQuery.Contains("pengingat"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: Reminder");
            return MessageType.Reminder;
        }
        
        if (lowerQuery.Contains("coffee") || lowerQuery.Contains("kopi") || lowerQuery.Contains("pesan kopi"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: Coffee");
            return MessageType.Coffee;
        }
        
        if (lowerQuery.Contains("music") || lowerQuery.Contains("lagu") || lowerQuery.Contains("playlist"))
        {
            Android.Util.Log.Info("VIRA_Groq", "Detected: Music");
            return MessageType.Music;
        }

        Android.Util.Log.Info("VIRA_Groq", "Detected: Text (default)");
        return MessageType.Text;
    }

    private async Task<WeatherData?> ParseWeatherDataAsync(string text)
    {
        var realWeather = await _weatherService.GetWeatherAsync("Surabaya");
        var forecast = await _weatherService.GetForecastAsync("Surabaya");
        
        if (realWeather != null)
        {
            Android.Util.Log.Info("VIRA_Groq", "Using real weather data from OpenWeatherMap");
            return new WeatherData
            {
                City = realWeather.City,
                Temp = realWeather.Temp,
                Condition = realWeather.Condition,
                Humidity = realWeather.Humidity,
                UV = "Moderate",
                Tomorrow = forecast != null ? $"{forecast.Condition} {forecast.Temp}" : "Data tidak tersedia"
            };
        }
        
        Android.Util.Log.Info("VIRA_Groq", "Using mock weather data");
        return new WeatherData
        {
            City = "Surabaya",
            Temp = "32°C",
            Condition = "Cerah Berawan ⛅",
            Humidity = "74%",
            UV = "7 (Tinggi)",
            Tomorrow = "Hujan diprediksi sore hari 🌧"
        };
    }

    private List<ScheduleItem>? ParseScheduleData(string text)
    {
        return new List<ScheduleItem>
        {
            new() { Time = "10:00", Title = "Meeting Tim", Location = "Zoom", Color = "#256AF4" },
            new() { Time = "14:00", Title = "Review Proyek", Location = "Kantor Lt. 3", Color = "#A855F7" }
        };
    }

    private async Task<List<NewsItem>?> ParseNewsDataAsync(string text)
    {
        var realNews = await _newsService.GetTopHeadlinesAsync("id", 3);
        
        if (realNews != null && realNews.Count > 0)
        {
            Android.Util.Log.Info("VIRA_Groq", $"Using real news data from NewsAPI ({realNews.Count} articles)");
            return realNews.Select(article => new NewsItem
            {
                Category = article.Category,
                Title = article.Title
            }).ToList();
        }
        
        Android.Util.Log.Info("VIRA_Groq", "Using mock news data");
        return new List<NewsItem>
        {
            new() { Category = "🤖 Teknologi", Title = "AI terbaru dari Google diluncurkan" },
            new() { Category = "📈 Bisnis", Title = "Pasar saham menguat hari ini" }
        };
    }

    private List<TrafficRoute>? ParseTrafficData(string text)
    {
        return new List<TrafficRoute>
        {
            new() { Route = "Rumah → Kantor", ETA = "32 menit", Status = "Lancar", Color = "#22C55E" },
            new() { Route = "Via Tol", ETA = "25 menit", Status = "Direkomendasikan", Color = "#22C55E" }
        };
    }
    
    private List<ReminderItem>? ParseReminderData(string text)
    {
        return new List<ReminderItem>
        {
            new() { Time = "09:00", Title = "Meeting dengan Tim Marketing", IsCompleted = false },
            new() { Time = "14:00", Title = "Review Proyek Website", IsCompleted = false },
            new() { Time = "16:30", Title = "Panggilan dengan Client", IsCompleted = true }
        };
    }
    
    private CoffeeOrder? ParseCoffeeData(string text)
    {
        return new CoffeeOrder
        {
            Type = "Caramel Macchiato",
            Size = "Grande",
            Location = "Starbucks Tunjungan Plaza",
            ETA = "15 menit",
            Price = "Rp 45.000"
        };
    }
    
    private MusicInfo? ParseMusicData(string text)
    {
        return new MusicInfo
        {
            Playlist = "Focus & Productivity",
            CurrentSong = "Lofi Hip Hop Beat",
            Artist = "ChilledCow",
            TotalSongs = 42
        };
    }
}
