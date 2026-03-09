using System.Text.RegularExpressions;
using VIRA.Mobile.SharedModels;

namespace VIRA.Mobile.SharedServices;

/// <summary>
/// Interface for handling matched command patterns
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Handle a matched command pattern
    /// </summary>
    /// <param name="match">The regex match result</param>
    /// <param name="context">The conversation context</param>
    /// <returns>Command result with response and optional action</returns>
    Task<CommandResult> HandleAsync(Match match, ConversationContext context);
}
