using System.Net.Http.Json;
using System.Text.Json;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services;

public class GeminiChatbotService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private string _apiKey = string.Empty;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

    public GeminiChatbotService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("API Key belum diatur. Silakan masukkan API Key di Settings.");
        }

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
        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini API Error: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var text = result
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "Maaf, saya tidak dapat memproses permintaan Anda.";

        return ParseResponse(text, userMessage);
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

    private ChatMessage ParseResponse(string text, string userQuery)
    {
        var message = new ChatMessage
        {
            Role = ChatMessageRole.Assistant,
            Content = text,
            Type = DetectMessageType(userQuery, text)
        };

        // Parse structured responses based on type
        if (message.Type == MessageType.Weather)
        {
            message.Weather = ParseWeatherData(text);
        }
        else if (message.Type == MessageType.Schedule || message.Type == MessageType.Reminder)
        {
            message.Schedule = ParseScheduleData(text);
        }
        else if (message.Type == MessageType.News)
        {
            message.NewsItems = ParseNewsData(text);
        }
        else if (message.Type == MessageType.Traffic)
        {
            message.TrafficData = ParseTrafficData(text);
        }

        return message;
    }

    private MessageType DetectMessageType(string query, string response)
    {
        var lowerQuery = query.ToLower();
        
        if (lowerQuery.Contains("cuaca") || lowerQuery.Contains("weather") || lowerQuery.Contains("suhu"))
            return MessageType.Weather;
        
        if (lowerQuery.Contains("jadwal") || lowerQuery.Contains("schedule") || lowerQuery.Contains("agenda"))
            return MessageType.Schedule;
        
        if (lowerQuery.Contains("berita") || lowerQuery.Contains("news") || lowerQuery.Contains("headline"))
            return MessageType.News;
        
        if (lowerQuery.Contains("lalu lintas") || lowerQuery.Contains("traffic") || lowerQuery.Contains("macet"))
            return MessageType.Traffic;
        
        if (lowerQuery.Contains("reminder") || lowerQuery.Contains("ingat") || lowerQuery.Contains("alarm"))
            return MessageType.Reminder;

        return MessageType.Text;
    }

    private WeatherData? ParseWeatherData(string text)
    {
        // Simple mock data - in production, integrate with weather API
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

    private List<NewsItem>? ParseNewsData(string text)
    {
        // Mock news data
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
}
