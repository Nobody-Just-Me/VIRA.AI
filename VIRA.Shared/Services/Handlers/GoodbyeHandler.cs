using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for goodbye messages
/// Pattern: "bye|goodbye|sampai jumpa|dadah"
/// </summary>
public class GoodbyeHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Get time-based farewell
        var hour = DateTime.Now.Hour;
        string timeFarewell;

        if (hour >= 5 && hour < 11)
        {
            timeFarewell = "Semoga hari Anda menyenangkan!";
        }
        else if (hour >= 11 && hour < 18)
        {
            timeFarewell = "Semoga sisa hari Anda produktif!";
        }
        else
        {
            timeFarewell = "Selamat beristirahat!";
        }

        // Create warm farewell responses
        var responses = new[]
        {
            $"👋 Sampai jumpa! {timeFarewell}",
            $"😊 Bye! Senang bisa membantu Anda. {timeFarewell}",
            $"🙋 Sampai bertemu lagi! Jangan ragu untuk kembali jika butuh bantuan.",
            $"✨ Goodbye! {timeFarewell} Saya akan selalu siap membantu.",
            $"💙 Dadah! Terima kasih sudah menggunakan VIRA. {timeFarewell}"
        };

        // Select a random response for variety
        var random = new Random();
        string response = responses[random.Next(responses.Length)];

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "goodbye", TimeOfDay = hour },
            confidence: 1.0f,
            speak: true
        ));
    }
}
