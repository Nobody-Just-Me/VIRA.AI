using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for battery status queries
/// Pattern: "baterai|battery|daya"
/// </summary>
public class BatteryHandler : ICommandHandler
{
    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Try to get battery level from device state
        int batteryLevel = 0;
        bool isCharging = false;

        if (context.DeviceState.ContainsKey("battery_level"))
        {
            batteryLevel = Convert.ToInt32(context.DeviceState["battery_level"]);
        }

        if (context.DeviceState.ContainsKey("is_charging"))
        {
            isCharging = Convert.ToBoolean(context.DeviceState["is_charging"]);
        }

        // Determine battery icon based on level
        string batteryIcon = batteryLevel switch
        {
            >= 80 => "🔋",
            >= 50 => "🔋",
            >= 20 => "🪫",
            _ => "🪫"
        };

        // Determine battery status message
        string statusMessage = (batteryLevel, isCharging) switch
        {
            (_, true) => "sedang mengisi daya ⚡",
            (>= 80, false) => "dalam kondisi baik",
            (>= 50, false) => "cukup",
            (>= 20, false) => "mulai rendah, pertimbangkan untuk mengisi daya",
            _ => "rendah! Segera isi daya"
        };

        string response = $"{batteryIcon} **Status Baterai**\n\n" +
                         $"📊 Level: {batteryLevel}%\n" +
                         $"⚡ Status: {(isCharging ? "Mengisi daya" : "Tidak mengisi daya")}\n" +
                         $"💡 Kondisi: Baterai {statusMessage}";

        string spokenSummary = $"Baterai Anda {batteryLevel} persen, {statusMessage}.";

        return await Task.FromResult(new CommandResult(
            response: response,
            action: new { Type = "battery_query", Level = batteryLevel, IsCharging = isCharging },
            confidence: 1.0f,
            speak: true
        ));
    }
}
