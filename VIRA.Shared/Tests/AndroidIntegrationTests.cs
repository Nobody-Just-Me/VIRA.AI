using VIRA.Shared.Models;
using VIRA.Shared.Services;

namespace VIRA.Shared.Tests;

/// <summary>
/// Tests for Android integration patterns
/// Validates AC-11.1, AC-11.2, AC-11.3, AC-11.4, AC-11.5, AC-11.6
/// </summary>
public class AndroidIntegrationTests
{
    private readonly PatternRegistry _registry;
    private readonly ConversationContext _context;

    public AndroidIntegrationTests()
    {
        var taskManager = new TaskManager();
        var httpClient = new HttpClient();
        var weatherService = new WeatherApiService(httpClient);
        var newsService = new NewsApiService(httpClient);
        _registry = new PatternRegistry(taskManager, weatherService, newsService);
        
        _context = new ConversationContext
        {
            UserId = "test-user",
            SessionId = "test-session"
        };
    }

    /// <summary>
    /// Test AC-11.1: User can open applications with voice commands
    /// </summary>
    public async Task TestOpenAppIndonesian()
    {
        var input = "buka whatsapp";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "open_app")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.ANDROID_INTEGRATION)
        {
            throw new Exception("Wrong category for open_app pattern");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("whatsapp"))
        {
            throw new Exception("Response doesn't contain app name");
        }

        if (result.Confidence != 1.0f)
        {
            throw new Exception("Confidence should be 1.0");
        }
    }

    public async Task TestOpenAppEnglish()
    {
        var input = "open chrome";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "open_app")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("chrome"))
        {
            throw new Exception("Response doesn't contain app name");
        }
    }

    /// <summary>
    /// Test AC-11.2: User can send WhatsApp messages to contacts
    /// </summary>
    public async Task TestSendWhatsAppIndonesian()
    {
        var input = "kirim whatsapp ke budi";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "send_whatsapp")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.CONTACT_MANAGEMENT)
        {
            throw new Exception("Wrong category for send_whatsapp pattern");
        }

        if (!match.Pattern.RequiredPermissions.Contains("android.permission.READ_CONTACTS"))
        {
            throw new Exception("Missing required permission");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("budi"))
        {
            throw new Exception("Response doesn't contain contact name");
        }
    }

    /// <summary>
    /// Test AC-11.3: User can make phone calls
    /// </summary>
    public async Task TestMakeCallIndonesian()
    {
        var input = "telepon mama";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "make_call")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.CONTACT_MANAGEMENT)
        {
            throw new Exception("Wrong category for make_call pattern");
        }

        if (!match.Pattern.RequiredPermissions.Contains("android.permission.READ_CONTACTS") ||
            !match.Pattern.RequiredPermissions.Contains("android.permission.CALL_PHONE"))
        {
            throw new Exception("Missing required permissions");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("mama"))
        {
            throw new Exception("Response doesn't contain contact name");
        }
    }

    /// <summary>
    /// Test AC-11.4: User can search on Google
    /// </summary>
    public async Task TestSearchIndonesian()
    {
        var input = "cari resep nasi goreng";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "search_google")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.ANDROID_INTEGRATION)
        {
            throw new Exception("Wrong category for search_google pattern");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("resep nasi goreng"))
        {
            throw new Exception("Response doesn't contain search query");
        }
    }

    /// <summary>
    /// Test AC-11.5: User can control system settings (WiFi, Bluetooth, Flashlight)
    /// </summary>
    public async Task TestToggleWiFiOn()
    {
        var input = "nyalakan wifi";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "toggle_wifi")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.SYSTEM_CONTROL)
        {
            throw new Exception("Wrong category for toggle_wifi pattern");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("wifi"))
        {
            throw new Exception("Response doesn't mention WiFi");
        }
    }

    public async Task TestToggleBluetoothEnglish()
    {
        var input = "turn on bluetooth";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "toggle_bluetooth")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.SYSTEM_CONTROL)
        {
            throw new Exception("Wrong category for toggle_bluetooth pattern");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("bluetooth"))
        {
            throw new Exception("Response doesn't mention Bluetooth");
        }
    }

    public async Task TestToggleFlashlightIndonesian()
    {
        var input = "nyalakan senter";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "toggle_flashlight")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.SYSTEM_CONTROL)
        {
            throw new Exception("Wrong category for toggle_flashlight pattern");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("senter"))
        {
            throw new Exception("Response doesn't mention flashlight");
        }
    }

    /// <summary>
    /// Test AC-11.6: User can control media playback
    /// </summary>
    public async Task TestMediaControlPlay()
    {
        var input = "putar musik";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "media_control")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        if (match.Pattern.Category != CommandCategory.MEDIA_CONTROL)
        {
            throw new Exception("Wrong category for media_control pattern");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("musik"))
        {
            throw new Exception("Response doesn't mention music");
        }
    }

    public async Task TestMediaControlNext()
    {
        var input = "next lagu";
        var match = _registry.FindMatch(input);
        
        if (match == null || match.Pattern.Id != "media_control")
        {
            throw new Exception($"Pattern not matched correctly for: {input}");
        }

        var result = await match.Pattern.Handler.HandleAsync(match.Match, _context);
        
        if (!result.Response.ToLower().Contains("lagu"))
        {
            throw new Exception("Response doesn't mention song");
        }
    }

    /// <summary>
    /// Test that all Android integration patterns are registered
    /// </summary>
    public void TestAllAndroidPatternsRegistered()
    {
        var totalPatterns = _registry.GetPatternCount();
        var androidPatterns = _registry.GetPatternsByCategory(CommandCategory.ANDROID_INTEGRATION);
        var systemPatterns = _registry.GetPatternsByCategory(CommandCategory.SYSTEM_CONTROL);
        var mediaPatterns = _registry.GetPatternsByCategory(CommandCategory.MEDIA_CONTROL);
        var contactPatterns = _registry.GetPatternsByCategory(CommandCategory.CONTACT_MANAGEMENT);

        if (totalPatterns < 17)
        {
            throw new Exception($"Expected at least 17 patterns, found {totalPatterns}");
        }

        if (androidPatterns.Count < 2)
        {
            throw new Exception($"Expected at least 2 Android integration patterns, found {androidPatterns.Count}");
        }

        if (systemPatterns.Count < 3)
        {
            throw new Exception($"Expected at least 3 system control patterns, found {systemPatterns.Count}");
        }

        if (mediaPatterns.Count < 1)
        {
            throw new Exception($"Expected at least 1 media control pattern, found {mediaPatterns.Count}");
        }

        if (contactPatterns.Count < 2)
        {
            throw new Exception($"Expected at least 2 contact management patterns, found {contactPatterns.Count}");
        }
    }
}
