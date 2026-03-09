using System.Text.RegularExpressions;
using System.Globalization;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for time queries
/// Pattern: "jam|waktu|time|pukul"
/// </summary>
public class TimeHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        var now = DateTime.Now;
        
        // Format time in Indonesian style
        string timeStr = now.ToString("HH:mm");
        string dateStr = now.ToString("dddd, dd MMMM yyyy", new CultureInfo("id-ID"));
        
        // Get time of day greeting
        string timeOfDay = now.Hour switch
        {
            >= 5 and < 11 => "pagi",
            >= 11 and < 15 => "siang",
            >= 15 and < 18 => "sore",
            _ => "malam"
        };

        string response = $"🕐 **Waktu Saat Ini**\n\n" +
                         $"⏰ Pukul: {timeStr} WIB\n" +
                         $"📅 Tanggal: {dateStr}\n" +
                         $"🌅 Waktu: {timeOfDay}";

        string spokenSummary = $"Sekarang pukul {timeStr} {timeOfDay}.";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "time_query", Time = timeStr, Date = dateStr },
            confidence: 1.0f,
            speak: true
        ));
    }
}
