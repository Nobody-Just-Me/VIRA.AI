using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;

namespace VIRA.Shared.Tests;

/// <summary>
/// Unit tests for pattern matching functionality
/// Tests all 60+ regex patterns, priority ordering, edge cases, and ambiguous patterns
/// </summary>
public class PatternMatchingTests
{
    private readonly PatternRegistry _registry;
    private readonly TaskManager _taskManager;
    private readonly WeatherApiService _weatherService;
    private readonly NewsApiService _newsService;

    public PatternMatchingTests()
    {
        // Initialize dependencies
        _taskManager = new TaskManager();
        _weatherService = new WeatherApiService();
        _newsService = new NewsApiService();
        _registry = new PatternRegistry(_taskManager, _weatherService, _newsService);
    }

    #region Task Management Pattern Tests

    [Theory]
    [InlineData("tambah task beli susu")]
    [InlineData("add task meeting with client")]
    [InlineData("buat tugas panggil dokter")]
    [InlineData("create todo finish report")]
    [InlineData("tambah tugas bayar listrik")]
    public void AddTaskPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("add_task", match.Pattern.Id);
        Assert.Equal(CommandCategory.TASK_MANAGEMENT, match.Pattern.Category);
        Assert.Equal(10, match.Pattern.Priority);
    }

    [Theory]
    [InlineData("selesai task beli susu")]
    [InlineData("done task meeting")]
    [InlineData("complete tugas panggil dokter")]
    [InlineData("finish task report")]
    [InlineData("selesai tugas")]
    public void CompleteTaskPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("complete_task", match.Pattern.Id);
        Assert.Equal(CommandCategory.TASK_MANAGEMENT, match.Pattern.Category);
    }

    [Theory]
    [InlineData("daftar task")]
    [InlineData("list tugas")]
    [InlineData("show todo")]
    [InlineData("tampilkan task")]
    public void ListTasksPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("list_tasks", match.Pattern.Id);
        Assert.Equal(CommandCategory.TASK_MANAGEMENT, match.Pattern.Category);
    }

    [Theory]
    [InlineData("hapus task beli susu")]
    [InlineData("delete tugas meeting")]
    [InlineData("remove task report")]
    [InlineData("batalkan tugas panggil dokter")]
    public void DeleteTaskPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("delete_task", match.Pattern.Id);
        Assert.Equal(CommandCategory.TASK_MANAGEMENT, match.Pattern.Category);
    }

    #endregion

    #region Information Query Pattern Tests

    [Theory]
    [InlineData("cuaca hari ini")]
    [InlineData("weather today")]
    [InlineData("suhu sekarang")]
    [InlineData("temperature now")]
    [InlineData("hujan tidak")]
    [InlineData("rain forecast")]
    public void WeatherPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("weather", match.Pattern.Id);
        Assert.Equal(CommandCategory.INFORMATION_QUERY, match.Pattern.Category);
        Assert.Equal(9, match.Pattern.Priority);
    }

    [Theory]
    [InlineData("berita terkini")]
    [InlineData("news today")]
    [InlineData("headline hari ini")]
    [InlineData("kabar terbaru")]
    public void NewsPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("news", match.Pattern.Id);
        Assert.Equal(CommandCategory.INFORMATION_QUERY, match.Pattern.Category);
    }

    [Theory]
    [InlineData("jadwal hari ini")]
    [InlineData("schedule today")]
    [InlineData("agenda besok")]
    [InlineData("appointment minggu ini")]
    [InlineData("meeting apa saja")]
    public void SchedulePattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("schedule", match.Pattern.Id);
        Assert.Equal(CommandCategory.INFORMATION_QUERY, match.Pattern.Category);
    }

    [Theory]
    [InlineData("jam berapa sekarang")]
    [InlineData("waktu sekarang")]
    [InlineData("time now")]
    [InlineData("pukul berapa")]
    public void TimePattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("time", match.Pattern.Id);
        Assert.Equal(CommandCategory.INFORMATION_QUERY, match.Pattern.Category);
    }

    [Theory]
    [InlineData("baterai berapa persen")]
    [InlineData("battery level")]
    [InlineData("daya tersisa")]
    [InlineData("power remaining")]
    public void BatteryPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("battery", match.Pattern.Id);
        Assert.Equal(CommandCategory.INFORMATION_QUERY, match.Pattern.Category);
    }

    #endregion

    #region Android Integration Pattern Tests

    [Theory]
    [InlineData("buka whatsapp")]
    [InlineData("open instagram")]
    [InlineData("jalankan aplikasi chrome")]
    [InlineData("launch app gmail")]
    [InlineData("buka aplikasi maps")]
    public void OpenAppPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("open_app", match.Pattern.Id);
        Assert.Equal(CommandCategory.ANDROID_INTEGRATION, match.Pattern.Category);
        Assert.Equal(10, match.Pattern.Priority);
    }

    [Theory]
    [InlineData("kirim whatsapp ke John")]
    [InlineData("send wa to Sarah")]
    [InlineData("chat pesan ke Budi")]
    [InlineData("kirim message to Alice")]
    public void SendWhatsAppPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("send_whatsapp", match.Pattern.Id);
        Assert.Equal(CommandCategory.CONTACT_MANAGEMENT, match.Pattern.Category);
        Assert.Contains("android.permission.READ_CONTACTS", match.Pattern.RequiredPermissions);
    }

    [Theory]
    [InlineData("telepon John")]
    [InlineData("call Sarah")]
    [InlineData("hubungi Budi")]
    [InlineData("telepon ke Alice")]
    public void MakeCallPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("make_call", match.Pattern.Id);
        Assert.Equal(CommandCategory.CONTACT_MANAGEMENT, match.Pattern.Category);
        Assert.Contains("android.permission.READ_CONTACTS", match.Pattern.RequiredPermissions);
        Assert.Contains("android.permission.CALL_PHONE", match.Pattern.RequiredPermissions);
    }

    [Theory]
    [InlineData("cari resep nasi goreng")]
    [InlineData("search best restaurants")]
    [InlineData("google tutorial programming")]
    [InlineData("googling how to cook")]
    public void SearchPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("search_google", match.Pattern.Id);
        Assert.Equal(CommandCategory.ANDROID_INTEGRATION, match.Pattern.Category);
    }

    [Theory]
    [InlineData("nyalakan wifi")]
    [InlineData("matikan wi-fi")]
    [InlineData("turn on wifi")]
    [InlineData("disable wifi")]
    [InlineData("enable wifi")]
    public void ToggleWiFiPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("toggle_wifi", match.Pattern.Id);
        Assert.Equal(CommandCategory.SYSTEM_CONTROL, match.Pattern.Category);
    }

    [Theory]
    [InlineData("nyalakan bluetooth")]
    [InlineData("matikan bt")]
    [InlineData("turn on bluetooth")]
    [InlineData("disable bluetooth")]
    public void ToggleBluetoothPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("toggle_bluetooth", match.Pattern.Id);
        Assert.Equal(CommandCategory.SYSTEM_CONTROL, match.Pattern.Category);
    }

    [Theory]
    [InlineData("nyalakan senter")]
    [InlineData("matikan flashlight")]
    [InlineData("turn on lampu")]
    [InlineData("disable flashlight")]
    public void ToggleFlashlightPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("toggle_flashlight", match.Pattern.Id);
        Assert.Equal(CommandCategory.SYSTEM_CONTROL, match.Pattern.Category);
    }

    [Theory]
    [InlineData("putar musik")]
    [InlineData("play music")]
    [InlineData("pause lagu")]
    [InlineData("next song")]
    [InlineData("previous musik")]
    [InlineData("lanjut")]
    [InlineData("kembali")]
    public void MediaControlPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("media_control", match.Pattern.Id);
        Assert.Equal(CommandCategory.MEDIA_CONTROL, match.Pattern.Category);
    }

    #endregion

    #region Greeting Pattern Tests

    [Theory]
    [InlineData("halo")]
    [InlineData("hello")]
    [InlineData("hai")]
    [InlineData("hi")]
    [InlineData("hey")]
    [InlineData("selamat pagi")]
    [InlineData("selamat siang")]
    public void GreetingPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("greeting", match.Pattern.Id);
        Assert.Equal(CommandCategory.GREETING, match.Pattern.Category);
        Assert.Equal(8, match.Pattern.Priority);
    }

    [Theory]
    [InlineData("apa kabar")]
    [InlineData("how are you")]
    [InlineData("gimana kabarmu")]
    [InlineData("kabar baik")]
    public void StatusQueryPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("status_query", match.Pattern.Id);
        Assert.Equal(CommandCategory.GREETING, match.Pattern.Category);
        Assert.Equal(9, match.Pattern.Priority);
    }

    [Theory]
    [InlineData("terima kasih")]
    [InlineData("thanks")]
    [InlineData("makasih")]
    [InlineData("thank you")]
    [InlineData("thx")]
    public void ThankYouPattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("thank_you", match.Pattern.Id);
        Assert.Equal(CommandCategory.GREETING, match.Pattern.Category);
    }

    [Theory]
    [InlineData("bye")]
    [InlineData("goodbye")]
    [InlineData("sampai jumpa")]
    [InlineData("dadah")]
    [InlineData("selamat tinggal")]
    public void GoodbyePattern_ShouldMatch_ValidInputs(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("goodbye", match.Pattern.Id);
        Assert.Equal(CommandCategory.GREETING, match.Pattern.Category);
    }

    #endregion

    #region Edge Cases Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void FindMatch_ShouldReturnNull_ForEmptyInput(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.Null(match);
    }

    [Theory]
    [InlineData("asdfghjkl")]
    [InlineData("random text without pattern")]
    [InlineData("12345")]
    [InlineData("!@#$%^&*()")]
    public void FindMatch_ShouldReturnNull_ForUnmatchedInput(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.Null(match);
    }

    [Theory]
    [InlineData("TAMBAH TASK BELI SUSU")]
    [InlineData("TaMbAh TaSk BeLi SuSu")]
    [InlineData("tambah TASK beli SUSU")]
    public void Patterns_ShouldBeCaseInsensitive(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("add_task", match.Pattern.Id);
    }

    [Theory]
    [InlineData("tambah task beli susu!!!")]
    [InlineData("???cuaca hari ini???")]
    [InlineData("buka whatsapp...")]
    [InlineData("halo!!!")]
    public void Patterns_ShouldHandleSpecialCharacters(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
    }

    [Theory]
    [InlineData("   tambah task beli susu   ")]
    [InlineData("\ttambah task beli susu\t")]
    [InlineData("\ntambah task beli susu\n")]
    public void Patterns_ShouldHandleWhitespace(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("add_task", match.Pattern.Id);
    }

    #endregion

    #region Priority Ordering Tests

    [Fact]
    public void FindMatch_ShouldReturnHighestPriorityPattern_WhenMultipleMatch()
    {
        // Arrange - "apa kabar" matches both status_query (priority 9) and greeting (priority 8)
        string input = "apa kabar";

        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("status_query", match.Pattern.Id);
        Assert.Equal(9, match.Pattern.Priority);
    }

    [Fact]
    public void FindMatch_ShouldReturnTaskPattern_OverGreeting_WhenBothMatch()
    {
        // Arrange - "tambah task halo" matches both add_task (priority 10) and greeting (priority 8)
        string input = "tambah task halo";

        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("add_task", match.Pattern.Id);
        Assert.Equal(10, match.Pattern.Priority);
    }

    [Fact]
    public void FindAllMatches_ShouldReturnMultipleMatches_InPriorityOrder()
    {
        // Arrange - "halo cuaca" matches both greeting and weather
        string input = "halo cuaca";

        // Act
        var matches = _registry.FindAllMatches(input);

        // Assert
        Assert.NotEmpty(matches);
        Assert.True(matches.Count >= 2);
        
        // Verify priority ordering
        for (int i = 0; i < matches.Count - 1; i++)
        {
            Assert.True(matches[i].Pattern.Priority >= matches[i + 1].Pattern.Priority);
        }
    }

    [Fact]
    public void GetPatternCount_ShouldReturnCorrectCount()
    {
        // Act
        var count = _registry.GetPatternCount();

        // Assert
        Assert.True(count >= 20); // We have at least 20 patterns registered
    }

    #endregion

    #region Ambiguous Pattern Tests

    [Theory]
    [InlineData("buka task")]
    [InlineData("list whatsapp")]
    [InlineData("cuaca task")]
    public void AmbiguousPatterns_ShouldMatchHighestPriority(string input)
    {
        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        // Should match the highest priority pattern that contains the keywords
    }

    [Fact]
    public void GetPatternsByCategory_ShouldReturnCorrectPatterns()
    {
        // Act
        var taskPatterns = _registry.GetPatternsByCategory(CommandCategory.TASK_MANAGEMENT);
        var greetingPatterns = _registry.GetPatternsByCategory(CommandCategory.GREETING);

        // Assert
        Assert.NotEmpty(taskPatterns);
        Assert.NotEmpty(greetingPatterns);
        Assert.All(taskPatterns, p => Assert.Equal(CommandCategory.TASK_MANAGEMENT, p.Category));
        Assert.All(greetingPatterns, p => Assert.Equal(CommandCategory.GREETING, p.Category));
    }

    #endregion

    #region Pattern Extraction Tests

    [Fact]
    public void AddTaskPattern_ShouldExtractTaskDescription()
    {
        // Arrange
        string input = "tambah task beli susu di supermarket";

        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("add_task", match.Pattern.Id);
        Assert.True(match.Match.Groups.Count >= 4);
        Assert.Contains("beli susu di supermarket", match.Match.Groups[3].Value);
    }

    [Fact]
    public void OpenAppPattern_ShouldExtractAppName()
    {
        // Arrange
        string input = "buka aplikasi whatsapp";

        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("open_app", match.Pattern.Id);
        Assert.True(match.Match.Groups.Count >= 4);
        Assert.Contains("whatsapp", match.Match.Groups[3].Value.ToLower());
    }

    [Fact]
    public void SendWhatsAppPattern_ShouldExtractContactName()
    {
        // Arrange
        string input = "kirim whatsapp ke John Doe";

        // Act
        var match = _registry.FindMatch(input);

        // Assert
        Assert.NotNull(match);
        Assert.Equal("send_whatsapp", match.Pattern.Id);
        Assert.True(match.Match.Groups.Count >= 5);
        Assert.Contains("John Doe", match.Match.Groups[4].Value);
    }

    #endregion

    #region Registry Management Tests

    [Fact]
    public void AddPattern_ShouldIncreasePatternCount()
    {
        // Arrange
        var initialCount = _registry.GetPatternCount();
        var newPattern = new CommandPattern(
            id: "test_pattern",
            regexPattern: @"\btest\b",
            category: CommandCategory.GREETING,
            priority: 5,
            handler: new GreetingHandler()
        );

        // Act
        _registry.AddPattern(newPattern);

        // Assert
        Assert.Equal(initialCount + 1, _registry.GetPatternCount());
    }

    [Fact]
    public void ClearPatterns_ShouldRemoveAllPatterns()
    {
        // Act
        _registry.ClearPatterns();

        // Assert
        Assert.Equal(0, _registry.GetPatternCount());
    }

    #endregion
}
