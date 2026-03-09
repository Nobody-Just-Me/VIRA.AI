using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

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

    public PatternRegistry(
        TaskManager taskManager, 
        WeatherApiService weatherService, 
        NewsApiService newsService)
    {
        _patterns = new List<CommandPattern>();
        _taskManager = taskManager;
        _weatherService = weatherService;
        _newsService = newsService;
        RegisterAllPatterns();
    }

    /// <summary>
    /// Register all command patterns
    /// </summary>
    private void RegisterAllPatterns()
    {
        // Simplified - no patterns registered for now
        // Patterns will be handled by AI fallback
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
    /// Get all registered patterns
    /// </summary>
    public IReadOnlyList<CommandPattern> GetAllPatterns() => _patterns.AsReadOnly();

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
