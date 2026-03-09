using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for opening applications
/// Pattern: "buka|open|jalankan aplikasi [app name]"
/// </summary>
public class OpenAppHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract app name from the match
        // Pattern groups: 1=action (buka/open/etc), 2=optional "aplikasi", 3=app name
        string appName = match.Groups.Count > 3 
            ? match.Groups[3].Value.Trim() 
            : string.Empty;

        if (string.IsNullOrWhiteSpace(appName))
        {
            return new CommandResult(
                response: "Maaf, aplikasi mana yang ingin Anda buka? Coba lagi dengan format: 'buka [nama aplikasi]'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Create response with placeholder action
        // The actual Android implementation will be done in task 6.2
        string response = $"📱 Membuka aplikasi {appName}... (Fitur ini akan diimplementasikan di task 6.2)";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "open_app", AppName = appName },
            confidence: 1.0f,
            speak: true
        ));
    }
}
