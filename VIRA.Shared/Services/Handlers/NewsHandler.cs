using System.Text.RegularExpressions;
using System.Text;
using VIRA.Shared.Models;

namespace VIRA.Shared.Services.Handlers;

/// <summary>
/// Handler for news queries
/// Pattern: "berita|news|headline|kabar"
/// </summary>
public class NewsHandler : ICommandHandler
{
    private readonly NewsApiService _newsService;

    public NewsHandler(NewsApiService newsService)
    {
        _newsService = newsService;
    }

    public async Task<CommandResult> HandleAsync(Match match, ConversationContext context)
    {
        // Fetch top headlines
        var articles = await _newsService.GetTopHeadlinesAsync("id", 5);

        if (articles == null || articles.Count == 0)
        {
            return new CommandResult(
                response: "Maaf, saya tidak bisa mendapatkan berita terkini saat ini. Pastikan API key NewsAPI sudah dikonfigurasi di Settings.",
                confidence: 1.0f,
                speak: true
            );
        }

        // Build response
        var response = new StringBuilder();
        response.AppendLine("📰 **Berita Terkini**\n");

        for (int i = 0; i < articles.Count; i++)
        {
            var article = articles[i];
            response.AppendLine($"{i + 1}. {article.Category} **{article.Title}**");
            response.AppendLine($"   📍 {article.Source}");
            response.AppendLine();
        }

        // Spoken summary
        string spokenSummary = $"Berikut {articles.Count} berita terkini untuk Anda.";

        return new CommandResult(
            response: response.ToString().Trim(),
            action: new { Type = "news_query", Count = articles.Count },
            confidence: 1.0f,
            speak: true
        );
    }
}
