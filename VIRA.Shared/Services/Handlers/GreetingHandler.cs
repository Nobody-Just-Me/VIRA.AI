using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for greeting messages
/// Pattern: "halo|hello|hai|hi|hey|selamat"
/// </summary>
public class GreetingHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Get time-based greeting
        var hour = DateTime.Now.Hour;
        string timeGreeting;
        string emoji;

        if (hour >= 5 && hour < 11)
        {
            timeGreeting = "Selamat pagi";
            emoji = "🌅";
        }
        else if (hour >= 11 && hour < 15)
        {
            timeGreeting = "Selamat siang";
            emoji = "☀️";
        }
        else if (hour >= 15 && hour < 18)
        {
            timeGreeting = "Selamat sore";
            emoji = "🌤️";
        }
        else
        {
            timeGreeting = "Selamat malam";
            emoji = "🌙";
        }

        // Create friendly, conversational response
        var responses = new[]
        {
            $"{emoji} {timeGreeting}! Saya VIRA, asisten pribadi Anda. Ada yang bisa saya bantu hari ini?",
            $"{emoji} {timeGreeting}! Senang bertemu dengan Anda. Bagaimana saya bisa membantu?",
            $"{emoji} Halo! {timeGreeting}. Saya siap membantu Anda hari ini.",
            $"{emoji} {timeGreeting}! Saya VIRA, siap membantu aktivitas Anda. Ada yang perlu dibantu?"
        };

        // Select a random response for variety
        var random = new Random();
        string response = responses[random.Next(responses.Length)];

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "greeting", TimeOfDay = timeGreeting },
            confidence: 1.0f,
            speak: true
        ));
    }
}
