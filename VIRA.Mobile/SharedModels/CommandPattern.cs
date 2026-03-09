using System.Text.RegularExpressions;
using VIRA.Mobile.SharedServices;

namespace VIRA.Mobile.SharedModels;

/// <summary>
/// Represents a command pattern with regex matching and handler
/// </summary>
public class CommandPattern
{
    /// <summary>
    /// Unique identifier for the pattern
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Regular expression for matching user input
    /// </summary>
    public Regex Regex { get; set; }
    
    /// <summary>
    /// Category of the command
    /// </summary>
    public CommandCategory Category { get; set; }
    
    /// <summary>
    /// Priority for pattern matching (higher = checked first)
    /// </summary>
    public int Priority { get; set; }
    
    /// <summary>
    /// Handler to execute when pattern matches
    /// </summary>
    public ICommandHandler Handler { get; set; }
    
    /// <summary>
    /// Required Android permissions for this command (if any)
    /// </summary>
    public List<string> RequiredPermissions { get; set; }

    public CommandPattern(
        string id,
        string regexPattern,
        CommandCategory category,
        int priority,
        ICommandHandler handler,
        List<string>? requiredPermissions = null)
    {
        Id = id;
        Regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Category = category;
        Priority = priority;
        Handler = handler;
        RequiredPermissions = requiredPermissions ?? new List<string>();
    }
}
