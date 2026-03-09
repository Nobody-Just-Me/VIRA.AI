using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for web searches
/// Pattern: "cari|search|google [query]"
/// </summary>
public class SearchHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Extract search query from the match
        // Pattern groups: 1=action (cari/search/google), 2=optional di/in, 3=query
        string query = match.Groups.Count > 3 
            ? match.Groups[3].Value.Trim() 
            : string.Empty;

        if (string.IsNullOrWhiteSpace(query))
        {
            return new CommandResult(
                response: "Maaf, apa yang ingin Anda cari? Coba lagi dengan format: 'cari [kata kunci]'",
                confidence: 0.5f,
                speak: true
            );
        }

        // Create response with placeholder action
        // The actual Android implementation will be done in task 6.6
        string response = $"🔍 Mencari \"{query}\" di Google... (Fitur ini akan diimplementasikan di task 6.6)";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "search_google", Query = query },
            confidence: 1.0f,
            speak: true
        ));
    }
}
