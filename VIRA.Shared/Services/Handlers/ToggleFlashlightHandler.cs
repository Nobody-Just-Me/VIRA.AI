using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for toggling flashlight on/off
/// Pattern: "nyalakan|matikan senter|flashlight"
/// </summary>
public class ToggleFlashlightHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract action from the match
        // Pattern groups: 1=action (nyalakan/matikan/turn on/turn off/enable/disable)
        string action = match.Groups.Count > 1 
            ? match.Groups[1].Value.Trim().ToLower() 
            : string.Empty;

        bool enable = action.Contains("nyalakan") || action.Contains("on") || action.Contains("enable");
        string actionText = enable ? "menyalakan" : "mematikan";
        string emoji = enable ? "🔦" : "🔌";

        // Create response with placeholder action
        // The actual Android implementation will be done in task 6.4
        string response = $"{emoji} {char.ToUpper(actionText[0]) + actionText.Substring(1)} senter... (Fitur ini akan diimplementasikan di task 6.4)";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "toggle_flashlight", Enable = enable },
            confidence: 1.0f,
            speak: true
        ));
    }
}
