using System.Text.RegularExpressions;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for weather queries
/// Pattern: "cuaca|weather|suhu|temperature|hujan"
/// </summary>
public class WeatherHandler : ICommandHandler
{
    private readonly WeatherApiService _weatherService;

    public WeatherHandler(WeatherApiService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Get default city from context or use Surabaya
        string city = context.Location ?? "Surabaya";

        // Fetch weather data
        var weather = await _weatherService.GetWeatherAsync(city);

        if (weather == null)
        {
            return new CommandResult(
                response: "Maaf, saya tidak bisa mendapatkan informasi cuaca saat ini. Pastikan API key OpenWeatherMap sudah dikonfigurasi di Settings.",
                confidence: 1.0f,
                speak: true
            );
        }

        // Build response
        string response = $"🌤️ **Cuaca di {weather.City}**\n\n" +
                         $"🌡️ Suhu: {weather.Temp}\n" +
                         $"🌈 Kondisi: {weather.Condition}\n" +
                         $"💧 Kelembaban: {weather.Humidity}\n" +
                         $"🌬️ Kecepatan Angin: {weather.WindSpeed}\n" +
                         $"📝 {weather.Description}";

        // Spoken summary
        string spokenSummary = $"Cuaca di {weather.City} saat ini {weather.Condition.ToLower()} dengan suhu {weather.Temp}.";

        return new CommandResult(
            response: response,
            action: new { Type = "weather_query", City = weather.City, Temp = weather.Temp },
            confidence: 1.0f,
            speak: true
        );
    }
}
