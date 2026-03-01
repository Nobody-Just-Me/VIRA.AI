namespace VIRA.Shared.Models;

public enum DailyAction
{
    SearchWeb,
    GetWeather,
    SetReminder,
    SummarizeText,
    GetNews,
    CheckTraffic
}

public class DailyCommandIntent
{
    public DailyAction Command { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}
