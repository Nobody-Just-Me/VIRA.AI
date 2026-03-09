using System.Net.Http.Json;
using System.Text.Json;
using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

public class GeminiChatbotService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private string _apiKey = string.Empty;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public GeminiChatbotService(HttpClient httpClient)
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
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"SendMessageAsync called with message: {userMessage}");
        #endif
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"📋 Current API Key: {_apiKey.Substring(0, Math.Min(10, _apiKey.Length))}... (length: {_apiKey.Length})");
        #endif
        
        if (string.IsNullOrEmpty(_apiKey))
        {
            #if __ANDROID__
            Android.Util.Log.Error("VIRA_Gemini", "API Key is empty!");
            #endif
            throw new InvalidOperationException("API Key belum diatur. Silakan masukkan API Key di Settings.");
        }

        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", "API Key is set, building request...");
        #endif
        
        var systemPrompt = GetSystemPrompt();
        var contents = BuildContents(systemPrompt, conversationHistory, userMessage);

        var requestBody = new
        {
            contents,
            generationConfig = new
            {
                temperature = 0.9,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 1024
            }
        };

        var url = $"{BaseUrl}?key={_apiKey}";
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"📡 Sending POST request to Gemini API...");
        #endif
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"   URL: {BaseUrl}?key={_apiKey.Substring(0, Math.Min(10, _apiKey.Length))}...");
        #endif
        
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"📥 Response status: {response.StatusCode}");
        #endif
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            #if __ANDROID__
            Android.Util.Log.Error("VIRA_Gemini", $"❌ Gemini API Error: {response.StatusCode}");
            #endif
            #if __ANDROID__
            Android.Util.Log.Error("VIRA_Gemini", $"❌ Error details: {error}");
            #endif
            
            // Parse error for better user message
            string userFriendlyError = "Maaf, terjadi kesalahan saat menghubungi Gemini API.";
            
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                userFriendlyError = "⚠️ API Key Anda telah mencapai batas quota.\n\n" +
                    "💡 Solusi:\n" +
                    "1. Tunggu beberapa menit (quota reset otomatis)\n" +
                    "2. Atau gunakan API Key lain di Settings\n" +
                    "3. Atau upgrade ke paid plan di https://aistudio.google.com/\n\n" +
                    $"API Key saat ini: {_apiKey.Substring(0, Math.Min(15, _apiKey.Length))}...";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                userFriendlyError = "❌ API Key tidak valid atau model tidak tersedia.\n\n" +
                    "💡 Solusi:\n" +
                    "1. Periksa API Key di Settings\n" +
                    "2. Pastikan API Key sudah di-enable untuk Gemini API\n" +
                    "3. Dapatkan API Key baru di https://aistudio.google.com/apikey\n\n" +
                    $"API Key saat ini: {_apiKey.Substring(0, Math.Min(15, _apiKey.Length))}...";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                userFriendlyError = "🔒 API Key tidak valid atau tidak memiliki akses.\n\n" +
                    "💡 Solusi:\n" +
                    "1. Periksa API Key di Settings\n" +
                    "2. Pastikan API Key benar dan aktif\n" +
                    "3. Dapatkan API Key baru di https://aistudio.google.com/apikey";
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
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "Maaf, saya tidak dapat memproses permintaan Anda.";

        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"✅ Got response from Gemini (length: {text.Length})");
        #endif
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"   First 100 chars: {text.Substring(0, Math.Min(100, text.Length))}...");
        #endif
        
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

    private List<object> BuildContents(string systemPrompt, List<ChatMessage> history, string userMessage)
    {
        var contents = new List<object>
        {
            new { role = "user", parts = new[] { new { text = systemPrompt } } },
            new { role = "model", parts = new[] { new { text = "Baik, saya siap membantu sebagai Vira!" } } }
        };

        foreach (var msg in history.TakeLast(10))
        {
            var role = msg.Role == ChatMessageRole.User ? "user" : "model";
            contents.Add(new { role, parts = new[] { new { text = msg.Content } } });
        }

        contents.Add(new { role = "user", parts = new[] { new { text = userMessage } } });

        return contents;
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
        
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"ParseResponseAsync: Type={message.Type}, keeping structured data NULL");
        #endif
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"Content will be displayed as text: {text.Substring(0, Math.Min(100, text.Length))}...");
        #endif

        return message;
    }

    private MessageType DetectMessageType(string query, string response)
    {
        var lowerQuery = query.ToLower();
        
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", $"DetectMessageType - Query: {lowerQuery}");
        #endif
        
        if (lowerQuery.Contains("cuaca") || lowerQuery.Contains("weather") || lowerQuery.Contains("suhu") || lowerQuery.Contains("temperature"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: Weather");
            #endif
            return MessageType.Weather;
        }
        
        if (lowerQuery.Contains("jadwal") || lowerQuery.Contains("schedule") || lowerQuery.Contains("agenda"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: Schedule");
            #endif
            return MessageType.Schedule;
        }
        
        if (lowerQuery.Contains("berita") || lowerQuery.Contains("news") || lowerQuery.Contains("headline"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: News");
            #endif
            return MessageType.News;
        }
        
        if (lowerQuery.Contains("lalu lintas") || lowerQuery.Contains("traffic") || lowerQuery.Contains("macet"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: Traffic");
            #endif
            return MessageType.Traffic;
        }
        
        if (lowerQuery.Contains("reminder") || lowerQuery.Contains("ingat") || lowerQuery.Contains("pengingat"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: Reminder");
            #endif
            return MessageType.Reminder;
        }
        
        if (lowerQuery.Contains("coffee") || lowerQuery.Contains("kopi") || lowerQuery.Contains("pesan kopi"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: Coffee");
            #endif
            return MessageType.Coffee;
        }
        
        if (lowerQuery.Contains("music") || lowerQuery.Contains("lagu") || lowerQuery.Contains("playlist"))
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Detected: Music");
            #endif
            return MessageType.Music;
        }

        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", "Detected: Text (default)");
        #endif
        return MessageType.Text;
    }

    private async Task<WeatherData?> ParseWeatherDataAsync(string text)
    {
        // Try to get real weather data
        var realWeather = await _weatherService.GetWeatherAsync("Surabaya");
        var forecast = await _weatherService.GetForecastAsync("Surabaya");
        
        if (realWeather != null)
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", "Using real weather data from OpenWeatherMap");
            #endif
            return new WeatherData
            {
                City = realWeather.City,
                Temp = realWeather.Temp,
                Condition = realWeather.Condition,
                Humidity = realWeather.Humidity,
                UV = "Moderate", // OpenWeatherMap free tier doesn't include UV
                Tomorrow = forecast != null ? $"{forecast.Condition} {forecast.Temp}" : "Data tidak tersedia"
            };
        }
        
        // Fallback to mock data
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", "Using mock weather data");
        #endif
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
        // Mock schedule data
        return new List<ScheduleItem>
        {
            new() { Time = "10:00", Title = "Meeting Tim", Location = "Zoom", Color = "#256AF4" },
            new() { Time = "14:00", Title = "Review Proyek", Location = "Kantor Lt. 3", Color = "#A855F7" }
        };
    }

    private async Task<List<NewsItem>?> ParseNewsDataAsync(string text)
    {
        // Try to get real news data
        var realNews = await _newsService.GetTopHeadlinesAsync("id", 3);
        
        if (realNews != null && realNews.Count > 0)
        {
            #if __ANDROID__
            Android.Util.Log.Info("VIRA_Gemini", $"Using real news data from NewsAPI ({realNews.Count} articles)");
            #endif
            return realNews.Select(article => new NewsItem
            {
                Category = article.Category,
                Title = article.Title
            }).ToList();
        }
        
        // Fallback to mock data
        #if __ANDROID__
        Android.Util.Log.Info("VIRA_Gemini", "Using mock news data");
        #endif
        return new List<NewsItem>
        {
            new() { Category = "🤖 Teknologi", Title = "AI terbaru dari Google diluncurkan" },
            new() { Category = "📈 Bisnis", Title = "Pasar saham menguat hari ini" }
        };
    }

    private List<TrafficRoute>? ParseTrafficData(string text)
    {
        // Mock traffic data
        return new List<TrafficRoute>
        {
            new() { Route = "Rumah → Kantor", ETA = "32 menit", Status = "Lancar", Color = "#22C55E" },
            new() { Route = "Via Tol", ETA = "25 menit", Status = "Direkomendasikan", Color = "#22C55E" }
        };
    }
    
    private List<ReminderItem>? ParseReminderData(string text)
    {
        // Mock reminder data
        return new List<ReminderItem>
        {
            new() { Time = "09:00", Title = "Meeting dengan Tim Marketing", IsCompleted = false },
            new() { Time = "14:00", Title = "Review Proyek Website", IsCompleted = false },
            new() { Time = "16:30", Title = "Panggilan dengan Client", IsCompleted = true }
        };
    }
    
    private CoffeeOrder? ParseCoffeeData(string text)
    {
        // Mock coffee order data
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
        // Mock music data
        return new MusicInfo
        {
            Playlist = "Focus & Productivity",
            CurrentSong = "Lofi Hip Hop Beat",
            Artist = "ChilledCow",
            TotalSongs = 42
        };
    }
}
