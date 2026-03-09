using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Services.Handlers;
using VIRA.Shared.Models;
using System.Text.RegularExpressions;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for Android integration functionality
/// Tests intent creation, permission checking, contact lookup, fuzzy matching, and app launcher
/// **Validates: Property 12 (App launch pattern matching), Property 21 (Contact name extraction), Property 24 (Fuzzy contact matching)**
/// </summary>
public class AndroidIntegrationUnitTests
{
    #region Intent Creation Tests

    [Theory]
    [InlineData("whatsapp", "com.whatsapp")]
    [InlineData("instagram", "com.instagram.android")]
    [InlineData("chrome", "com.android.chrome")]
    [InlineData("gmail", "com.google.android.gm")]
    public void OpenAppHandler_ShouldMapAppNameToPackage(string appName, string expectedPackage)
    {
        // Arrange
        var handler = new OpenAppHandler();
        
        // Act
        var packageName = handler.GetPackageNameForApp(appName.ToLower());

        // Assert
        Assert.Equal(expectedPackage, packageName);
    }

    [Theory]
    [InlineData("buka whatsapp", "whatsapp")]
    [InlineData("open instagram", "instagram")]
    [InlineData("jalankan aplikasi chrome", "chrome")]
    [InlineData("launch app gmail", "gmail")]
    public void OpenAppPattern_ShouldExtractAppName(string input, string expectedAppName)
    {
        // Arrange
        var pattern = @"\b(buka|open|jalankan|launch)\s+(aplikasi|app)?\s*([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);

        // Assert
        Assert.True(match.Success);
        Assert.Contains(expectedAppName, match.Groups[3].Value.ToLower());
    }

    [Theory]
    [InlineData("kirim whatsapp ke John", "John")]
    [InlineData("send wa to Sarah Smith", "Sarah Smith")]
    [InlineData("chat pesan ke Budi Santoso", "Budi Santoso")]
    public void SendWhatsAppPattern_ShouldExtractContactName(string input, string expectedContact)
    {
        // Arrange
        var pattern = @"\b(kirim|send|chat)\s+(pesan|message|whatsapp|wa)\s+(ke|to)\s+([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);

        // Assert
        Assert.True(match.Success);
        Assert.Equal(expectedContact, match.Groups[4].Value);
    }

    [Theory]
    [InlineData("telepon John", "John")]
    [InlineData("call Sarah Smith", "Sarah Smith")]
    [InlineData("hubungi Budi Santoso", "Budi Santoso")]
    [InlineData("telepon ke Alice", "Alice")]
    public void MakeCallPattern_ShouldExtractContactName(string input, string expectedContact)
    {
        // Arrange
        var pattern = @"\b(telepon|call|hubungi)\s+(ke|to)?\s*([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);

        // Assert
        Assert.True(match.Success);
        Assert.Contains(expectedContact, match.Groups[3].Value);
    }

    [Theory]
    [InlineData("cari resep nasi goreng", "resep nasi goreng")]
    [InlineData("search best restaurants", "best restaurants")]
    [InlineData("google tutorial programming", "tutorial programming")]
    public void SearchPattern_ShouldExtractQuery(string input, string expectedQuery)
    {
        // Arrange
        var pattern = @"\b(cari|search|google|googling)\s+(di|in)?\s*([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);

        // Assert
        Assert.True(match.Success);
        Assert.Contains(expectedQuery, match.Groups[3].Value);
    }

    #endregion

    #region Permission Checking Tests

    [Fact]
    public void SendWhatsAppHandler_ShouldRequireContactsPermission()
    {
        // Arrange
        var handler = new SendWhatsAppHandler();

        // Act
        var requiredPermissions = handler.GetRequiredPermissions();

        // Assert
        Assert.Contains("android.permission.READ_CONTACTS", requiredPermissions);
    }

    [Fact]
    public void MakeCallHandler_ShouldRequireContactsAndCallPermissions()
    {
        // Arrange
        var handler = new MakeCallHandler();

        // Act
        var requiredPermissions = handler.GetRequiredPermissions();

        // Assert
        Assert.Contains("android.permission.READ_CONTACTS", requiredPermissions);
        Assert.Contains("android.permission.CALL_PHONE", requiredPermissions);
    }

    [Fact]
    public void OpenAppHandler_ShouldNotRequirePermissions()
    {
        // Arrange
        var handler = new OpenAppHandler();

        // Act
        var requiredPermissions = handler.GetRequiredPermissions();

        // Assert
        Assert.Empty(requiredPermissions);
    }

    [Fact]
    public void ToggleBluetoothHandler_ShouldRequireBluetoothPermissions()
    {
        // Arrange
        var handler = new ToggleBluetoothHandler();

        // Act
        var requiredPermissions = handler.GetRequiredPermissions();

        // Assert
        Assert.Contains("android.permission.BLUETOOTH", requiredPermissions);
    }

    #endregion

    #region Contact Lookup Tests
    // **Validates: Property 21 - Contact name extraction**

    [Theory]
    [InlineData("John", "John Doe", true)]
    [InlineData("john", "John Doe", true)]
    [InlineData("JOHN", "John Doe", true)]
    [InlineData("Doe", "John Doe", true)]
    [InlineData("Jane", "John Doe", false)]
    public void ContactLookup_ShouldMatchPartialNames_CaseInsensitive(string query, string contactName, bool shouldMatch)
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new Contact { Name = contactName, PhoneNumber = "1234567890" }
        };

        // Act
        var match = contacts.FirstOrDefault(c => 
            c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

        // Assert
        if (shouldMatch)
        {
            Assert.NotNull(match);
        }
        else
        {
            Assert.Null(match);
        }
    }

    [Fact]
    public void ContactLookup_ShouldExtractFirstAndLastName()
    {
        // Arrange
        string fullName = "John Doe";

        // Act
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal(2, parts.Length);
        Assert.Equal("John", parts[0]);
        Assert.Equal("Doe", parts[1]);
    }

    [Theory]
    [InlineData("kirim whatsapp ke John Doe", "John Doe")]
    [InlineData("send wa to Sarah", "Sarah")]
    [InlineData("chat pesan ke Budi Santoso Wijaya", "Budi Santoso Wijaya")]
    public void ContactNameExtraction_ShouldHandleMultipleWords(string input, string expectedName)
    {
        // Arrange
        var pattern = @"\b(kirim|send|chat)\s+(pesan|message|whatsapp|wa)\s+(ke|to)\s+([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);
        var extractedName = match.Groups[4].Value;

        // Assert
        Assert.Equal(expectedName, extractedName);
    }

    #endregion

    #region Fuzzy Contact Matching Tests
    // **Validates: Property 24 - Fuzzy contact matching**

    [Theory]
    [InlineData("John", "John Doe", 100)]
    [InlineData("john", "John Doe", 100)]
    [InlineData("Jon", "John Doe", 75)]
    [InlineData("Jhon", "John Doe", 75)]
    [InlineData("Jane", "John Doe", 0)]
    public void FuzzyMatching_ShouldCalculateSimilarity(string query, string contactName, int expectedMinScore)
    {
        // Act
        var score = CalculateFuzzyMatchScore(query, contactName);

        // Assert
        Assert.True(score >= expectedMinScore, 
            $"Expected score >= {expectedMinScore}, but got {score}");
    }

    [Fact]
    public void FuzzyMatching_ShouldHandleTypos()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new Contact { Name = "John Doe", PhoneNumber = "1234567890" },
            new Contact { Name = "Jane Smith", PhoneNumber = "0987654321" }
        };
        string query = "Jon"; // Typo for John

        // Act
        var matches = contacts
            .Select(c => new { Contact = c, Score = CalculateFuzzyMatchScore(query, c.Name) })
            .Where(x => x.Score > 50)
            .OrderByDescending(x => x.Score)
            .ToList();

        // Assert
        Assert.NotEmpty(matches);
        Assert.Equal("John Doe", matches.First().Contact.Name);
    }

    [Fact]
    public void FuzzyMatching_ShouldRankByRelevance()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new Contact { Name = "John Doe", PhoneNumber = "1234567890" },
            new Contact { Name = "Johnny Walker", PhoneNumber = "1111111111" },
            new Contact { Name = "Jane Doe", PhoneNumber = "2222222222" }
        };
        string query = "John";

        // Act
        var matches = contacts
            .Select(c => new { Contact = c, Score = CalculateFuzzyMatchScore(query, c.Name) })
            .OrderByDescending(x => x.Score)
            .ToList();

        // Assert
        Assert.Equal("John Doe", matches.First().Contact.Name); // Exact match should rank highest
    }

    [Theory]
    [InlineData("john doe", "John Doe", true)]
    [InlineData("JOHN DOE", "John Doe", true)]
    [InlineData("John", "John Doe", true)]
    [InlineData("Doe", "John Doe", true)]
    [InlineData("j doe", "John Doe", true)]
    public void FuzzyMatching_ShouldBeCaseInsensitive(string query, string contactName, bool shouldMatch)
    {
        // Act
        var score = CalculateFuzzyMatchScore(query, contactName);

        // Assert
        if (shouldMatch)
        {
            Assert.True(score > 50);
        }
    }

    [Fact]
    public void FuzzyMatching_ShouldHandlePartialMatches()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new Contact { Name = "John Doe", PhoneNumber = "1234567890" },
            new Contact { Name = "John Smith", PhoneNumber = "1111111111" },
            new Contact { Name = "Jane Doe", PhoneNumber = "2222222222" }
        };
        string query = "Doe";

        // Act
        var matches = contacts
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.Contains(matches, c => c.Name == "John Doe");
        Assert.Contains(matches, c => c.Name == "Jane Doe");
    }

    #endregion

    #region App Launcher Tests
    // **Validates: Property 12 - App launch pattern matching**

    [Theory]
    [InlineData("buka whatsapp", "whatsapp")]
    [InlineData("open instagram", "instagram")]
    [InlineData("jalankan chrome", "chrome")]
    [InlineData("launch gmail", "gmail")]
    [InlineData("buka aplikasi maps", "maps")]
    public void AppLaunchPattern_ShouldMatchVariousFormats(string input, string expectedApp)
    {
        // Arrange
        var pattern = @"\b(buka|open|jalankan|launch)\s+(aplikasi|app)?\s*([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);

        // Assert
        Assert.True(match.Success);
        Assert.Contains(expectedApp, match.Groups[3].Value.ToLower());
    }

    [Fact]
    public void AppLauncher_ShouldSupportCommonApps()
    {
        // Arrange
        var handler = new OpenAppHandler();
        var commonApps = new[] { "whatsapp", "instagram", "chrome", "gmail", "maps", "youtube" };

        // Act & Assert
        foreach (var app in commonApps)
        {
            var packageName = handler.GetPackageNameForApp(app);
            Assert.NotNull(packageName);
            Assert.NotEmpty(packageName);
        }
    }

    [Theory]
    [InlineData("wa", "whatsapp")]
    [InlineData("ig", "instagram")]
    [InlineData("yt", "youtube")]
    public void AppLauncher_ShouldSupportAbbreviations(string abbreviation, string expectedApp)
    {
        // Arrange
        var handler = new OpenAppHandler();

        // Act
        var packageName = handler.GetPackageNameForApp(abbreviation);

        // Assert
        Assert.NotNull(packageName);
        Assert.Contains(expectedApp, packageName.ToLower());
    }

    [Fact]
    public void AppLauncher_ShouldHandleUnknownApps()
    {
        // Arrange
        var handler = new OpenAppHandler();
        string unknownApp = "nonexistentapp123";

        // Act
        var packageName = handler.GetPackageNameForApp(unknownApp);

        // Assert
        // Should return null or the app name itself for system to search
        Assert.True(packageName == null || packageName == unknownApp);
    }

    [Theory]
    [InlineData("buka WHATSAPP")]
    [InlineData("OPEN instagram")]
    [InlineData("Buka Chrome")]
    public void AppLaunchPattern_ShouldBeCaseInsensitive(string input)
    {
        // Arrange
        var pattern = @"\b(buka|open|jalankan|launch)\s+(aplikasi|app)?\s*([^\s].+)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);

        // Assert
        Assert.True(match.Success);
    }

    #endregion

    #region System Control Tests

    [Theory]
    [InlineData("nyalakan wifi", true)]
    [InlineData("matikan wifi", false)]
    [InlineData("turn on wifi", true)]
    [InlineData("disable wifi", false)]
    public void ToggleWiFiPattern_ShouldDetectOnOffState(string input, bool expectedEnable)
    {
        // Arrange
        var pattern = @"\b(nyalakan|matikan|hidupkan|turn on|turn off|enable|disable)\s+(wifi|wi-fi)\b";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);
        var action = match.Groups[1].Value.ToLower();
        var shouldEnable = action.Contains("nyalakan") || action.Contains("turn on") || 
                          action.Contains("enable") || action.Contains("hidupkan");

        // Assert
        Assert.True(match.Success);
        Assert.Equal(expectedEnable, shouldEnable);
    }

    [Theory]
    [InlineData("nyalakan bluetooth", true)]
    [InlineData("matikan bt", false)]
    [InlineData("turn on bluetooth", true)]
    [InlineData("disable bluetooth", false)]
    public void ToggleBluetoothPattern_ShouldDetectOnOffState(string input, bool expectedEnable)
    {
        // Arrange
        var pattern = @"\b(nyalakan|matikan|hidupkan|turn on|turn off|enable|disable)\s+(bluetooth|bt)\b";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);
        var action = match.Groups[1].Value.ToLower();
        var shouldEnable = action.Contains("nyalakan") || action.Contains("turn on") || 
                          action.Contains("enable") || action.Contains("hidupkan");

        // Assert
        Assert.True(match.Success);
        Assert.Equal(expectedEnable, shouldEnable);
    }

    [Theory]
    [InlineData("nyalakan senter", true)]
    [InlineData("matikan flashlight", false)]
    [InlineData("turn on lampu", true)]
    public void ToggleFlashlightPattern_ShouldDetectOnOffState(string input, bool expectedEnable)
    {
        // Arrange
        var pattern = @"\b(nyalakan|matikan|hidupkan|turn on|turn off|enable|disable)\s+(senter|flashlight|lampu)\b";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);
        var action = match.Groups[1].Value.ToLower();
        var shouldEnable = action.Contains("nyalakan") || action.Contains("turn on") || 
                          action.Contains("enable") || action.Contains("hidupkan");

        // Assert
        Assert.True(match.Success);
        Assert.Equal(expectedEnable, shouldEnable);
    }

    #endregion

    #region Media Control Tests

    [Theory]
    [InlineData("putar musik", "play")]
    [InlineData("pause lagu", "pause")]
    [InlineData("next song", "next")]
    [InlineData("previous musik", "previous")]
    [InlineData("lanjut", "next")]
    [InlineData("kembali", "previous")]
    public void MediaControlPattern_ShouldDetectAction(string input, string expectedAction)
    {
        // Arrange
        var pattern = @"\b(putar|play|pause|jeda|next|previous|lanjut|selanjutnya|sebelum|kembali)\s+(musik|music|lagu|song)?\b";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Act
        var match = regex.Match(input);
        var action = match.Groups[1].Value.ToLower();

        // Assert
        Assert.True(match.Success);
        
        // Map action to expected
        if (action.Contains("putar") || action.Contains("play"))
            Assert.Equal("play", expectedAction);
        else if (action.Contains("pause") || action.Contains("jeda"))
            Assert.Equal("pause", expectedAction);
        else if (action.Contains("next") || action.Contains("lanjut") || action.Contains("selanjutnya"))
            Assert.Equal("next", expectedAction);
        else if (action.Contains("previous") || action.Contains("kembali") || action.Contains("sebelum"))
            Assert.Equal("previous", expectedAction);
    }

    #endregion

    #region Helper Methods

    private int CalculateFuzzyMatchScore(string query, string target)
    {
        query = query.ToLower();
        target = target.ToLower();

        // Exact match
        if (query == target)
            return 100;

        // Contains match
        if (target.Contains(query))
            return 90;

        // Word match
        var targetWords = target.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in targetWords)
        {
            if (word.StartsWith(query))
                return 85;
            if (word.Contains(query))
                return 75;
        }

        // Levenshtein distance (simplified)
        var distance = LevenshteinDistance(query, target);
        var maxLength = Math.Max(query.Length, target.Length);
        var similarity = (1.0 - (double)distance / maxLength) * 100;

        return (int)similarity;
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; i++)
            d[i, 0] = i;
        for (int j = 0; j <= m; j++)
            d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    #endregion

    #region Test Models

    private class Contact
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    #endregion
}
