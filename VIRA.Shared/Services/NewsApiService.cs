using System.Net.Http.Json;
using System.Text.Json;

namespace VIRA.Shared.Services;

public class NewsApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://newsapi.org/v2";
    private string _apiKey = string.Empty;

    public NewsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<List<NewsArticle>?> GetTopHeadlinesAsync(string country = "id", int pageSize = 5)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
#if __ANDROID__
            Android.Util.Log.Warn("VIRA_News", "NewsAPI Key not set, using mock data");
#endif
            return null;
        }

        try
        {
            var url = $"{BaseUrl}/top-headlines?country={country}&pageSize={pageSize}&apiKey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_News", $"API Error: {response.StatusCode}");
#endif
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            var articles = data.GetProperty("articles");
            var newsList = new List<NewsArticle>();

            foreach (var article in articles.EnumerateArray())
            {
                var title = article.GetProperty("title").GetString();
                if (string.IsNullOrEmpty(title) || title == "[Removed]")
                    continue;

                newsList.Add(new NewsArticle
                {
                    Title = title,
                    Category = GetCategory(article),
                    Source = article.GetProperty("source").GetProperty("name").GetString() ?? "Unknown",
                    Url = article.GetProperty("url").GetString() ?? "",
                    PublishedAt = article.GetProperty("publishedAt").GetString() ?? ""
                });

                if (newsList.Count >= pageSize)
                    break;
            }

            return newsList.Count > 0 ? newsList : null;
        }
        catch (Exception ex)
        {
#if __ANDROID__
            Android.Util.Log.Error("VIRA_News", $"Exception: {ex.Message}");
#endif
            return null;
        }
    }

    public async Task<List<NewsArticle>?> GetNewsByCategoryAsync(string category, string country = "id", int pageSize = 5)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return null;

        try
        {
            var url = $"{BaseUrl}/top-headlines?country={country}&category={category}&pageSize={pageSize}&apiKey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            var articles = data.GetProperty("articles");
            var newsList = new List<NewsArticle>();

            foreach (var article in articles.EnumerateArray())
            {
                var title = article.GetProperty("title").GetString();
                if (string.IsNullOrEmpty(title) || title == "[Removed]")
                    continue;

                newsList.Add(new NewsArticle
                {
                    Title = title,
                    Category = GetCategoryEmoji(category),
                    Source = article.GetProperty("source").GetProperty("name").GetString() ?? "Unknown",
                    Url = article.GetProperty("url").GetString() ?? "",
                    PublishedAt = article.GetProperty("publishedAt").GetString() ?? ""
                });

                if (newsList.Count >= pageSize)
                    break;
            }

            return newsList.Count > 0 ? newsList : null;
        }
        catch (Exception ex)
        {
#if __ANDROID__
            Android.Util.Log.Error("VIRA_News", $"Category Exception: {ex.Message}");
#endif
            return null;
        }
    }

    private string GetCategory(JsonElement article)
    {
        // Try to detect category from source or content
        var source = article.GetProperty("source").GetProperty("name").GetString()?.ToLower() ?? "";
        var title = article.GetProperty("title").GetString()?.ToLower() ?? "";

        if (source.Contains("tech") || title.Contains("teknologi") || title.Contains("ai") || title.Contains("digital"))
            return "🤖 Teknologi";
        if (source.Contains("business") || source.Contains("ekonomi") || title.Contains("bisnis") || title.Contains("saham"))
            return "📈 Bisnis";
        if (source.Contains("sport") || title.Contains("olahraga") || title.Contains("sepak bola"))
            return "⚽ Olahraga";
        if (source.Contains("health") || title.Contains("kesehatan"))
            return "🏥 Kesehatan";
        if (source.Contains("entertainment") || title.Contains("hiburan"))
            return "🎬 Hiburan";

        return "📰 Umum";
    }

    private string GetCategoryEmoji(string category)
    {
        return category.ToLower() switch
        {
            "technology" => "🤖 Teknologi",
            "business" => "📈 Bisnis",
            "sports" => "⚽ Olahraga",
            "health" => "🏥 Kesehatan",
            "entertainment" => "🎬 Hiburan",
            "science" => "🔬 Sains",
            _ => "📰 Umum"
        };
    }
}

public class NewsArticle
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string PublishedAt { get; set; } = string.Empty;
}
