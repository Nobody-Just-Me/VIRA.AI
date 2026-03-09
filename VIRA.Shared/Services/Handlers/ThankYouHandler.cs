using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for thank you messages
/// Pattern: "terima kasih|thanks|makasih|thank you"
/// </summary>
public class ThankYouHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Create warm, appreciative responses
        var responses = new[]
        {
            "😊 Sama-sama! Senang bisa membantu Anda.",
            "🙏 Terima kasih kembali! Saya selalu siap membantu.",
            "✨ Dengan senang hati! Jangan ragu untuk bertanya lagi.",
            "💙 Tidak masalah! Saya di sini untuk membantu Anda.",
            "😄 Sama-sama! Ada lagi yang bisa saya bantu?",
            "🤗 Senang bisa membantu! Kapan saja Anda butuh bantuan, saya siap."
        };

        // Select a random response for variety
        var random = new Random();
        string response = responses[random.Next(responses.Length)];

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "thank_you" },
            confidence: 1.0f,
            speak: true
        ));
    }
}
