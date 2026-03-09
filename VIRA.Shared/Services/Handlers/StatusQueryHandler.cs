using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for status queries
/// Pattern: "apa kabar|how are you|gimana"
/// </summary>
public class StatusQueryHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Create friendly, human-like responses
        var responses = new[]
        {
            "😊 Saya baik-baik saja, terima kasih! Saya siap membantu Anda hari ini. Bagaimana dengan Anda?",
            "🤖 Saya dalam kondisi prima dan siap bekerja! Ada yang bisa saya bantu?",
            "✨ Saya sangat baik! Senang bisa membantu Anda. Apa yang bisa saya lakukan untuk Anda?",
            "💪 Saya selalu siap membantu! Bagaimana kabar Anda? Ada yang perlu dibantu?",
            "😄 Saya baik! Terima kasih sudah bertanya. Saya di sini untuk membantu Anda."
        };

        // Select a random response for variety
        var random = new Random();
        string response = responses[random.Next(responses.Length)];

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "status_query" },
            confidence: 1.0f,
            speak: true
        ));
    }
}
