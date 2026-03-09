using System.Net.Http.Json;
using System.Text.Json;

namespace VIRA.Mobile.SharedServices;

public class WeatherApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5";
    private string _apiKey = string.Empty;

    public WeatherApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<WeatherResponse?> GetWeatherAsync(string city = "Surabaya")
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
#if __ANDROID__
            Android.Util.Log.Warn("VIRA_Weather", "OpenWeatherMap API Key not set, using mock data");
#endif
            return null;
        }

        try
        {
            var url = $"{BaseUrl}/weather?q={city}&appid={_apiKey}&units=metric&lang=id";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_Weather", $"API Error: {response.StatusCode}");
#endif
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            return new WeatherResponse
            {
                City = data.GetProperty("name").GetString() ?? city,
                Temp = $"{Math.Round(data.GetProperty("main").GetProperty("temp").GetDouble())}°C",
                Condition = GetWeatherCondition(data.GetProperty("weather")[0].GetProperty("main").GetString() ?? "Clear"),
                Humidity = $"{data.GetProperty("main").GetProperty("humidity").GetInt32()}%",
                FeelsLike = $"{Math.Round(data.GetProperty("main").GetProperty("feels_like").GetDouble())}°C",
                WindSpeed = $"{data.GetProperty("wind").GetProperty("speed").GetDouble()} m/s",
                Description = data.GetProperty("weather")[0].GetProperty("description").GetString() ?? ""
            };
        }
        catch (Exception ex)
        {
#if __ANDROID__
            Android.Util.Log.Error("VIRA_Weather", $"Exception: {ex.Message}");
#endif
            return null;
        }
    }

    public async Task<ForecastResponse?> GetForecastAsync(string city = "Surabaya")
    {
        if (string.IsNullOrEmpty(_apiKey))
            return null;

        try
        {
            var url = $"{BaseUrl}/forecast?q={city}&appid={_apiKey}&units=metric&lang=id&cnt=8";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            var list = data.GetProperty("list");
            
            if (list.GetArrayLength() > 0)
            {
                var tomorrow = list[7]; // ~24 hours ahead
                return new ForecastResponse
                {
                    Temp = $"{Math.Round(tomorrow.GetProperty("main").GetProperty("temp").GetDouble())}°C",
                    Condition = GetWeatherCondition(tomorrow.GetProperty("weather")[0].GetProperty("main").GetString() ?? "Clear"),
                    Description = tomorrow.GetProperty("weather")[0].GetProperty("description").GetString() ?? ""
                };
            }

            return null;
        }
        catch (Exception ex)
        {
#if __ANDROID__
            Android.Util.Log.Error("VIRA_Weather", $"Forecast Exception: {ex.Message}");
#endif
            return null;
        }
    }

    private string GetWeatherCondition(string main)
    {
        return main switch
        {
            "Clear" => "Cerah ☀️",
            "Clouds" => "Berawan ☁️",
            "Rain" => "Hujan 🌧️",
            "Drizzle" => "Gerimis 🌦️",
            "Thunderstorm" => "Badai Petir ⛈️",
            "Snow" => "Salju ❄️",
            "Mist" or "Fog" => "Berkabut 🌫️",
            "Haze" => "Kabut Asap 🌫️",
            _ => "Cerah Berawan ⛅"
        };
    }
}

public class WeatherResponse
{
    public string City { get; set; } = string.Empty;
    public string Temp { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public string FeelsLike { get; set; } = string.Empty;
    public string WindSpeed { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ForecastResponse
{
    public string Temp { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
