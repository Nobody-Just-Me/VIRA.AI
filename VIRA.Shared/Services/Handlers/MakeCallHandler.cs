using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for making phone calls
/// Pattern: "telepon|call|hubungi [contact name]"
/// </summary>
public class MakeCallHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract contact name from the match
        // Pattern groups: 1=action (telepon/call/hubungi), 2=optional ke/to, 3=contact name
        string contactName = match.Groups.Count > 3 
            ? match.Groups[3].Value.Trim() 
            : string.Empty;

        if (string.IsNullOrWhiteSpace(contactName))
        {
            return new CommandResult(
                response: "Maaf, siapa yang ingin Anda hubungi? Coba lagi dengan format: 'telepon [nama kontak]'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Create response with placeholder action
        // The actual Android implementation will be done in task 6.3
        string response = $"📞 Menghubungi {contactName}... (Fitur ini akan diimplementasikan di task 6.3)";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "make_call", ContactName = contactName },
            confidence: 1.0f,
            speak: true
        ));
    }
}
