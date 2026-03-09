using VIRA.Shared.Models;
using VIRA.Shared.Services.Handlers;

namespace VIRA.Shared.Services;

/// <summary>
/// Registry for storing and matching command patterns
/// Implements priority-based pattern selection
/// </summary>
public class PatternRegistry
{
    private readonly List<CommandPattern> _patterns;
    private readonly TaskManager _taskManager;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;
    private readonly TaskAnalyticsService? _analyticsService;

    public PatternRegistry(
        TaskManager taskManager, 
        WeatherApiService weatherService, 
        NewsApiService newsService,
        TaskAnalyticsService? analyticsService = null)
    {
        _patterns = new List<CommandPattern>();
        _taskManager = taskManager;
        _weatherService = weatherService;
        _newsService = newsService;
        _analyticsService = analyticsService;
        RegisterAllPatterns();
    }

    /// <summary>
    /// Register all command patterns
    /// </summary>
    private void RegisterAllPatterns()
    {
        RegisterTaskPatterns();
        RegisterInfoPatterns();
        RegisterAndroidPatterns();
        RegisterGreetingPatterns();
    }

    /// <summary>
    /// Register task management patterns
    /// </summary>
    private void RegisterTaskPatterns()
    {
        // Add task pattern: "tambah|add|buat|create task [description]"
        _patterns.Add(new CommandPattern(
            id: "add_task",
            regexPattern: @"\b(tambah|add|buat|create)\s+(task|tugas|todo)\s+(.+)",
            category: CommandCategory.TASK_MANAGEMENT,
            priority: 10,
            handler: new AddTaskHandler(_taskManager, _analyticsService)
        ));

        // Complete task pattern: "selesai|done|complete task [optional: description]"
        _patterns.Add(new CommandPattern(
            id: "complete_task",
            regexPattern: @"\b(selesai|done|complete|finish)\s+(task|tugas)\s*(.*)",
            category: CommandCategory.TASK_MANAGEMENT,
            priority: 10,
            handler: new CompleteTaskHandler(_taskManager)
        ));

        // List tasks pattern: "daftar|list|show task"
        _patterns.Add(new CommandPattern(
            id: "list_tasks",
            regexPattern: @"\b(daftar|list|show|tampilkan)\s+(task|tugas|todo)",
            category: CommandCategory.TASK_MANAGEMENT,
            priority: 10,
            handler: new ListTasksHandler(_taskManager)
        ));

        // Delete task pattern: "hapus|delete|remove task [description]"
        _patterns.Add(new CommandPattern(
            id: "delete_task",
            regexPattern: @"\b(hapus|delete|remove|batalkan)\s+(task|tugas)\s+(.+)",
            category: CommandCategory.TASK_MANAGEMENT,
            priority: 10,
            handler: new DeleteTaskHandler(_taskManager)
        ));
    }

    /// <summary>
    /// Register information query patterns
    /// </summary>
    private void RegisterInfoPatterns()
    {
        // Weather pattern: "cuaca|weather|suhu|temperature|hujan"
        _patterns.Add(new CommandPattern(
            id: "weather",
            regexPattern: @"\b(cuaca|weather|suhu|temperature|hujan|rain)\b",
            category: CommandCategory.INFORMATION_QUERY,
            priority: 9,
            handler: new WeatherHandler(_weatherService)
        ));

        // News pattern: "berita|news|headline|kabar"
        _patterns.Add(new CommandPattern(
            id: "news",
            regexPattern: @"\b(berita|news|headline|kabar)\b",
            category: CommandCategory.INFORMATION_QUERY,
            priority: 9,
            handler: new NewsHandler(_newsService)
        ));

        // Schedule pattern: "jadwal|schedule|agenda|appointment"
        _patterns.Add(new CommandPattern(
            id: "schedule",
            regexPattern: @"\b(jadwal|schedule|agenda|appointment|meeting)\b",
            category: CommandCategory.INFORMATION_QUERY,
            priority: 9,
            handler: new ScheduleHandler(_taskManager)
        ));

        // Time pattern: "jam|waktu|time|pukul"
        _patterns.Add(new CommandPattern(
            id: "time",
            regexPattern: @"\b(jam|waktu|time|pukul)\b",
            category: CommandCategory.INFORMATION_QUERY,
            priority: 9,
            handler: new TimeHandler()
        ));

        // Battery pattern: "baterai|battery|daya"
        _patterns.Add(new CommandPattern(
            id: "battery",
            regexPattern: @"\b(baterai|battery|daya|power)\b",
            category: CommandCategory.INFORMATION_QUERY,
            priority: 9,
            handler: new BatteryHandler()
        ));
    }

    /// <summary>
    /// Register Android integration patterns
    /// </summary>
    private void RegisterAndroidPatterns()
    {
        // Open app pattern: "buka|open|jalankan aplikasi [app name]"
        _patterns.Add(new CommandPattern(
            id: "open_app",
            regexPattern: @"\b(buka|open|jalankan|launch)\s+(aplikasi|app)?\s*([^\s].+)",
            category: CommandCategory.ANDROID_INTEGRATION,
            priority: 10,
            handler: new OpenAppHandler()
        ));

        // Send WhatsApp pattern: "kirim|send whatsapp ke [contact name]"
        _patterns.Add(new CommandPattern(
            id: "send_whatsapp",
            regexPattern: @"\b(kirim|send|chat)\s+(pesan|message|whatsapp|wa)\s+(ke|to)\s+([^\s].+)",
            category: CommandCategory.CONTACT_MANAGEMENT,
            priority: 10,
            handler: new SendWhatsAppHandler(),
            requiredPermissions: new List<string> { "android.permission.READ_CONTACTS" }
        ));

        // Make call pattern: "telepon|call|hubungi [contact name]"
        _patterns.Add(new CommandPattern(
            id: "make_call",
            regexPattern: @"\b(telepon|call|hubungi)\s+(ke|to)?\s*([^\s].+)",
            category: CommandCategory.CONTACT_MANAGEMENT,
            priority: 10,
            handler: new MakeCallHandler(),
            requiredPermissions: new List<string> 
            { 
                "android.permission.READ_CONTACTS",
                "android.permission.CALL_PHONE"
            }
        ));

        // Search pattern: "cari|search|google [query]"
        _patterns.Add(new CommandPattern(
            id: "search_google",
            regexPattern: @"\b(cari|search|google|googling)\s+(di|in)?\s*([^\s].+)",
            category: CommandCategory.ANDROID_INTEGRATION,
            priority: 9,
            handler: new SearchHandler()
        ));

        // Toggle WiFi pattern: "nyalakan|matikan wifi"
        _patterns.Add(new CommandPattern(
            id: "toggle_wifi",
            regexPattern: @"\b(nyalakan|matikan|hidupkan|turn on|turn off|enable|disable)\s+(wifi|wi-fi)\b",
            category: CommandCategory.SYSTEM_CONTROL,
            priority: 10,
            handler: new ToggleWiFiHandler()
        ));

        // Toggle Bluetooth pattern: "nyalakan|matikan bluetooth"
        _patterns.Add(new CommandPattern(
            id: "toggle_bluetooth",
            regexPattern: @"\b(nyalakan|matikan|hidupkan|turn on|turn off|enable|disable)\s+(bluetooth|bt)\b",
            category: CommandCategory.SYSTEM_CONTROL,
            priority: 10,
            handler: new ToggleBluetoothHandler()
        ));

        // Toggle Flashlight pattern: "nyalakan|matikan senter"
        _patterns.Add(new CommandPattern(
            id: "toggle_flashlight",
            regexPattern: @"\b(nyalakan|matikan|hidupkan|turn on|turn off|enable|disable)\s+(senter|flashlight|lampu)\b",
            category: CommandCategory.SYSTEM_CONTROL,
            priority: 10,
            handler: new ToggleFlashlightHandler()
        ));

        // Media control pattern: "putar|play|pause|next musik"
        _patterns.Add(new CommandPattern(
            id: "media_control",
            regexPattern: @"\b(putar|play|pause|jeda|next|previous|lanjut|selanjutnya|sebelum|kembali)\s+(musik|music|lagu|song)?\b",
            category: CommandCategory.MEDIA_CONTROL,
            priority: 10,
            handler: new MediaControlHandler()
        ));
    }

    /// <summary>
    /// Register greeting and conversation patterns
    /// </summary>
    private void RegisterGreetingPatterns()
    {
        // Greeting pattern: "halo|hello|hai|hi|hey|selamat"
        _patterns.Add(new CommandPattern(
            id: "greeting",
            regexPattern: @"\b(halo|hello|hai|hi|hey|selamat)\b",
            category: CommandCategory.GREETING,
            priority: 8,
            handler: new GreetingHandler()
        ));

        // Status query pattern: "apa kabar|how are you|gimana"
        _patterns.Add(new CommandPattern(
            id: "status_query",
            regexPattern: @"\b(apa kabar|how are you|gimana|kabar)\b",
            category: CommandCategory.GREETING,
            priority: 9,
            handler: new StatusQueryHandler()
        ));

        // Thank you pattern: "terima kasih|thanks|makasih|thank you"
        _patterns.Add(new CommandPattern(
            id: "thank_you",
            regexPattern: @"\b(terima kasih|thanks|makasih|thank you|thx)\b",
            category: CommandCategory.GREETING,
            priority: 8,
            handler: new ThankYouHandler()
        ));

        // Goodbye pattern: "bye|goodbye|sampai jumpa|dadah"
        _patterns.Add(new CommandPattern(
            id: "goodbye",
            regexPattern: @"\b(bye|goodbye|sampai jumpa|dadah|selamat tinggal)\b",
            category: CommandCategory.GREETING,
            priority: 8,
            handler: new GoodbyeHandler()
        ));
    }

    /// <summary>
    /// Add a pattern to the registry
    /// </summary>
    public void AddPattern(CommandPattern pattern)
    {
        _patterns.Add(pattern);
    }

    /// <summary>
    /// Find the first matching pattern for the given input
    /// Patterns are checked in priority order (highest first)
    /// </summary>
    /// <param name="input">User input text</param>
    /// <returns>PatternMatch if found, null otherwise</returns>
    public PatternMatch? FindMatch(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        // Sort patterns by priority (descending) and search for first match
        var sortedPatterns = _patterns.OrderByDescending(p => p.Priority);

        foreach (var pattern in sortedPatterns)
        {
            var match = pattern.Regex.Match(input);
            if (match.Success)
            {
                return new PatternMatch(pattern, match);
            }
        }

        return null;
    }

    /// <summary>
    /// Find all matching patterns for the given input
    /// Useful for debugging or showing multiple possible interpretations
    /// </summary>
    /// <param name="input">User input text</param>
    /// <returns>List of all matching patterns</returns>
    public List<PatternMatch> FindAllMatches(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new List<PatternMatch>();
        }

        var matches = new List<PatternMatch>();

        foreach (var pattern in _patterns.OrderByDescending(p => p.Priority))
        {
            var match = pattern.Regex.Match(input);
            if (match.Success)
            {
                matches.Add(new PatternMatch(pattern, match));
            }
        }

        return matches;
    }

    /// <summary>
    /// Get all patterns in a specific category
    /// </summary>
    public List<CommandPattern> GetPatternsByCategory(CommandCategory category)
    {
        return _patterns.Where(p => p.Category == category).ToList();
    }

    /// <summary>
    /// Get total number of registered patterns
    /// </summary>
    public int GetPatternCount()
    {
        return _patterns.Count;
    }

    /// <summary>
    /// Clear all patterns (useful for testing)
    /// </summary>
    public void ClearPatterns()
    {
        _patterns.Clear();
    }
}
