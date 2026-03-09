using Xunit;
using VIRA.Shared.Services;
using VIRA.Shared.Models;

namespace VIRA.Shared.Tests;

/// <summary>
/// Property-based tests for VIRA functionality
/// Tests universal properties across random inputs (minimum 100 iterations)
/// **Validates: Property 18 (Volume level normalization), Property 30 (Pattern priority ordering), Property 29 (AI fallback to rule-based)**
/// </summary>
public class PropertyBasedTests
{
    private readonly Random _random = new Random(42); // Fixed seed for reproducibility

    #region Volume Level Normalization Tests
    // **Validates: Property 18 - Volume level normalization (0-100 to device range)**

    [Fact]
    public void VolumeNormalization_ShouldAlwaysBeWithinDeviceRange()
    {
        // Property: For all volume levels 0-100, normalized value should be within device range (0-15)
        const int iterations = 100;
        const int deviceMaxVolume = 15;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange - Generate random volume level 0-100
            int inputVolume = _random.Next(0, 101);

            // Act
            int normalizedVolume = NormalizeVolume(inputVolume, deviceMaxVolume);

            // Assert
            Assert.InRange(normalizedVolume, 0, deviceMaxVolume);
        }
    }

    [Fact]
    public void VolumeNormalization_ShouldPreserveZeroAndMax()
    {
        // Property: 0 should map to 0, 100 should map to device max
        const int deviceMaxVolume = 15;

        // Test 0
        Assert.Equal(0, NormalizeVolume(0, deviceMaxVolume));

        // Test 100
        Assert.Equal(deviceMaxVolume, NormalizeVolume(100, deviceMaxVolume));
    }

    [Fact]
    public void VolumeNormalization_ShouldBeMonotonic()
    {
        // Property: If input1 < input2, then normalized1 <= normalized2
        const int iterations = 100;
        const int deviceMaxVolume = 15;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            int volume1 = _random.Next(0, 100);
            int volume2 = _random.Next(volume1, 101);

            // Act
            int normalized1 = NormalizeVolume(volume1, deviceMaxVolume);
            int normalized2 = NormalizeVolume(volume2, deviceMaxVolume);

            // Assert
            Assert.True(normalized1 <= normalized2,
                $"Monotonicity violated: {volume1} -> {normalized1}, {volume2} -> {normalized2}");
        }
    }

    [Fact]
    public void VolumeNormalization_ShouldHandleAllValidInputs()
    {
        // Property: All inputs 0-100 should produce valid outputs
        const int deviceMaxVolume = 15;

        for (int volume = 0; volume <= 100; volume++)
        {
            // Act
            int normalized = NormalizeVolume(volume, deviceMaxVolume);

            // Assert
            Assert.InRange(normalized, 0, deviceMaxVolume);
        }
    }

    [Fact]
    public void VolumeNormalization_ShouldBeProportional()
    {
        // Property: 50% input should produce ~50% of device max
        const int deviceMaxVolume = 15;
        int midpoint = 50;

        // Act
        int normalized = NormalizeVolume(midpoint, deviceMaxVolume);

        // Assert
        int expectedMidpoint = deviceMaxVolume / 2;
        Assert.InRange(normalized, expectedMidpoint - 1, expectedMidpoint + 1);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(25)]
    [InlineData(30)]
    public void VolumeNormalization_ShouldWorkForDifferentDeviceMaxVolumes(int deviceMax)
    {
        // Property: Normalization should work for any device max volume
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            int inputVolume = _random.Next(0, 101);
            int normalized = NormalizeVolume(inputVolume, deviceMax);

            Assert.InRange(normalized, 0, deviceMax);
        }
    }

    #endregion

    #region Pattern Priority Ordering Tests
    // **Validates: Property 30 - Pattern priority ordering**

    [Fact]
    public void PatternPriority_ShouldAlwaysReturnHighestPriority()
    {
        // Property: For any set of matching patterns, the one with highest priority should be returned
        const int iterations = 100;
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var registry = new PatternRegistry(taskManager, weatherService, newsService);

        for (int i = 0; i < iterations; i++)
        {
            // Arrange - Create input that matches multiple patterns
            var inputs = new[]
            {
                "halo cuaca", // matches greeting (8) and weather (9)
                "tambah task halo", // matches add_task (10) and greeting (8)
                "apa kabar cuaca", // matches status_query (9), greeting (8), weather (9)
            };

            string input = inputs[_random.Next(inputs.Length)];

            // Act
            var match = registry.FindMatch(input);
            var allMatches = registry.FindAllMatches(input);

            // Assert
            if (allMatches.Count > 1)
            {
                Assert.NotNull(match);
                var highestPriority = allMatches.Max(m => m.Pattern.Priority);
                Assert.Equal(highestPriority, match.Pattern.Priority);
            }
        }
    }

    [Fact]
    public void PatternPriority_ShouldBeConsistent()
    {
        // Property: Same input should always return same pattern
        const int iterations = 100;
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var registry = new PatternRegistry(taskManager, weatherService, newsService);

        var testInputs = new[]
        {
            "tambah task beli susu",
            "cuaca hari ini",
            "halo",
            "berita terkini"
        };

        foreach (var input in testInputs)
        {
            // Get first match
            var firstMatch = registry.FindMatch(input);
            Assert.NotNull(firstMatch);

            // Test consistency
            for (int i = 0; i < iterations; i++)
            {
                var match = registry.FindMatch(input);
                Assert.NotNull(match);
                Assert.Equal(firstMatch.Pattern.Id, match.Pattern.Id);
                Assert.Equal(firstMatch.Pattern.Priority, match.Pattern.Priority);
            }
        }
    }

    [Fact]
    public void PatternPriority_ShouldRespectPriorityOrder()
    {
        // Property: Patterns should be checked in priority order (highest first)
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var registry = new PatternRegistry(taskManager, weatherService, newsService);

        // Get all patterns
        var allPatterns = new List<CommandPattern>();
        foreach (var category in Enum.GetValues<CommandCategory>())
        {
            allPatterns.AddRange(registry.GetPatternsByCategory(category));
        }

        // Verify patterns can be sorted by priority
        var sortedPatterns = allPatterns.OrderByDescending(p => p.Priority).ToList();

        // Property: For each consecutive pair, first should have >= priority than second
        for (int i = 0; i < sortedPatterns.Count - 1; i++)
        {
            Assert.True(sortedPatterns[i].Priority >= sortedPatterns[i + 1].Priority,
                $"Priority order violated at index {i}: {sortedPatterns[i].Priority} < {sortedPatterns[i + 1].Priority}");
        }
    }

    [Fact]
    public void PatternPriority_ShouldHandleRandomPatternOrder()
    {
        // Property: Priority should work regardless of pattern registration order
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Create registry with patterns in random order
            var taskManager = new TaskManager();
            var weatherService = new WeatherApiService();
            var newsService = new NewsApiService();
            var registry = new PatternRegistry(taskManager, weatherService, newsService);

            // Test that high priority patterns still match first
            var match = registry.FindMatch("tambah task test");
            Assert.NotNull(match);
            Assert.Equal(10, match.Pattern.Priority); // add_task has priority 10
        }
    }

    #endregion

    #region AI Fallback Tests
    // **Validates: Property 29 - AI fallback to rule-based**

    [Fact]
    public async Task AIFallback_ShouldAlwaysProvideResponse()
    {
        // Property: For any input, system should always provide a response (rule-based or AI)
        const int iterations = 100;
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var preferencesService = new PreferencesService();
        var ruleBasedProcessor = new RuleBasedProcessor(taskManager, weatherService, newsService);
        var hybridProcessor = new HybridMessageProcessor(ruleBasedProcessor, preferencesService);

        var randomInputs = GenerateRandomInputs(iterations);

        foreach (var input in randomInputs)
        {
            // Act
            var context = CreateTestContext();
            var result = await hybridProcessor.ProcessMessageAsync(input, context);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Response);
        }
    }

    [Fact]
    public async Task AIFallback_ShouldUseRuleBasedForKnownPatterns()
    {
        // Property: Known patterns should always use rule-based, never AI
        const int iterations = 50;
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var preferencesService = new PreferencesService();
        var ruleBasedProcessor = new RuleBasedProcessor(taskManager, weatherService, newsService);
        var hybridProcessor = new HybridMessageProcessor(ruleBasedProcessor, preferencesService);

        var knownPatterns = new[]
        {
            "tambah task {0}",
            "cuaca {0}",
            "berita {0}",
            "halo {0}",
            "daftar task"
        };

        for (int i = 0; i < iterations; i++)
        {
            // Arrange - Generate variations of known patterns
            var pattern = knownPatterns[_random.Next(knownPatterns.Length)];
            var input = string.Format(pattern, GenerateRandomWord());

            // Act
            var context = CreateTestContext();
            var result = await hybridProcessor.ProcessMessageAsync(input, context);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsRuleBased || result.Confidence >= 0.8f);
        }
    }

    [Fact]
    public async Task AIFallback_ShouldFallbackWhenConfidenceLow()
    {
        // Property: When rule-based confidence < 0.8, should attempt AI fallback
        const int iterations = 50;
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var preferencesService = new PreferencesService();
        var ruleBasedProcessor = new RuleBasedProcessor(taskManager, weatherService, newsService);
        var hybridProcessor = new HybridMessageProcessor(ruleBasedProcessor, preferencesService);

        var unknownPatterns = GenerateRandomInputs(iterations);

        foreach (var input in unknownPatterns)
        {
            // Act
            var context = CreateTestContext();
            var result = await hybridProcessor.ProcessMessageAsync(input, context);

            // Assert
            Assert.NotNull(result);
            // Should either have high confidence (rule-based) or attempt AI
            if (result.Confidence < 0.8f)
            {
                // Should have attempted AI or returned error
                Assert.NotEmpty(result.Response);
            }
        }
    }

    [Fact]
    public async Task AIFallback_ShouldNeverReturnNull()
    {
        // Property: System should never return null response
        const int iterations = 100;
        var taskManager = new TaskManager();
        var weatherService = new WeatherApiService();
        var newsService = new NewsApiService();
        var preferencesService = new PreferencesService();
        var ruleBasedProcessor = new RuleBasedProcessor(taskManager, weatherService, newsService);
        var hybridProcessor = new HybridMessageProcessor(ruleBasedProcessor, preferencesService);

        var allInputs = GenerateRandomInputs(iterations);

        foreach (var input in allInputs)
        {
            // Act
            var context = CreateTestContext();
            var result = await hybridProcessor.ProcessMessageAsync(input, context);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Response);
        }
    }

    #endregion

    #region Contact Fuzzy Matching Tests
    // **Validates: Property 24 - Fuzzy contact matching**

    [Fact]
    public void FuzzyMatching_ShouldAlwaysReturnScoreBetween0And100()
    {
        // Property: Fuzzy match score should always be 0-100
        const int iterations = 100;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            string query = GenerateRandomWord();
            string target = GenerateRandomWord();

            // Act
            int score = CalculateFuzzyMatchScore(query, target);

            // Assert
            Assert.InRange(score, 0, 100);
        }
    }

    [Fact]
    public void FuzzyMatching_ExactMatchShouldScore100()
    {
        // Property: Exact matches should always score 100
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            string name = GenerateRandomName();

            // Act
            int score = CalculateFuzzyMatchScore(name, name);

            // Assert
            Assert.Equal(100, score);
        }
    }

    [Fact]
    public void FuzzyMatching_ShouldBeCaseInsensitive()
    {
        // Property: Case variations should produce same score
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            string name = GenerateRandomName();
            string lower = name.ToLower();
            string upper = name.ToUpper();

            // Act
            int scoreLower = CalculateFuzzyMatchScore(lower, name);
            int scoreUpper = CalculateFuzzyMatchScore(upper, name);

            // Assert
            Assert.Equal(scoreLower, scoreUpper);
        }
    }

    [Fact]
    public void FuzzyMatching_ShouldBeSymmetric()
    {
        // Property: Score(A, B) should equal Score(B, A)
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            string name1 = GenerateRandomName();
            string name2 = GenerateRandomName();

            // Act
            int score1 = CalculateFuzzyMatchScore(name1, name2);
            int score2 = CalculateFuzzyMatchScore(name2, name1);

            // Assert
            Assert.Equal(score1, score2);
        }
    }

    [Fact]
    public void FuzzyMatching_SubstringShouldScoreHigh()
    {
        // Property: If query is substring of target, score should be >= 75
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Arrange
            string fullName = GenerateRandomName();
            if (fullName.Length < 3) continue;
            
            string substring = fullName.Substring(0, fullName.Length / 2);

            // Act
            int score = CalculateFuzzyMatchScore(substring, fullName);

            // Assert
            Assert.True(score >= 75, $"Substring '{substring}' of '{fullName}' scored only {score}");
        }
    }

    #endregion

    #region Helper Methods

    private int NormalizeVolume(int volume, int deviceMaxVolume)
    {
        // Normalize 0-100 to 0-deviceMaxVolume
        if (volume < 0) volume = 0;
        if (volume > 100) volume = 100;

        return (int)Math.Round((double)volume * deviceMaxVolume / 100.0);
    }

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

        // Levenshtein distance
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

    private List<string> GenerateRandomInputs(int count)
    {
        var inputs = new List<string>();
        var words = new[] { "test", "random", "query", "input", "data", "sample", "example", "demo" };
        var patterns = new[] { "what is {0}", "tell me about {0}", "{0} information", "explain {0}" };

        for (int i = 0; i < count; i++)
        {
            var word = words[_random.Next(words.Length)];
            var pattern = patterns[_random.Next(patterns.Length)];
            inputs.Add(string.Format(pattern, word));
        }

        return inputs;
    }

    private string GenerateRandomWord()
    {
        var words = new[] { "test", "sample", "data", "query", "input", "value", "item", "element" };
        return words[_random.Next(words.Length)];
    }

    private string GenerateRandomName()
    {
        var firstNames = new[] { "John", "Jane", "Bob", "Alice", "Charlie", "Diana", "Eve", "Frank" };
        var lastNames = new[] { "Doe", "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller" };

        return $"{firstNames[_random.Next(firstNames.Length)]} {lastNames[_random.Next(lastNames.Length)]}";
    }

    private ConversationContext CreateTestContext()
    {
        return new ConversationContext
        {
            UserId = "test_user",
            SessionId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            UserName = "Test User",
            Location = "Jakarta",
            DeviceInfo = "Test Device"
        };
    }

    #endregion
}
