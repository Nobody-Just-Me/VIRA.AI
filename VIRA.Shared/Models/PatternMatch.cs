using System.Text.RegularExpressions;

namespace VIRA.Shared.Models;

/// <summary>
/// Represents a successful pattern match with the matched pattern and regex result
/// </summary>
public class PatternMatch
{
    /// <summary>
    /// The command pattern that matched
    /// </summary>
    public CommandPattern Pattern { get; set; }
    
    /// <summary>
    /// The regex match result
    /// </summary>
    public Match Match { get; set; }

    public PatternMatch(CommandPattern pattern, Match match)
    {
        Pattern = pattern;
        Match = match;
    }
}
