using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VIRA.Shared.Services;

namespace VIRA.Shared.Views;

public sealed partial class SettingsPage : Page
{
    private readonly IGeminiService _geminiService;

    public SettingsPage()
    {
        this.InitializeComponent();
        _geminiService = App.Current.Services.GetService<IGeminiService>();
        LoadSettings();
    }

    private async void LoadSettings()
    {
        // Load saved API key from secure storage
        try
        {
            var apiKey = await SecureStorage.GetAsync("GeminiApiKey");
            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiKeyBox.Password = apiKey;
            }
        }
        catch
        {
            // Ignore if secure storage not available
        }
    }

    private async void OnSaveApiKeyClick(object sender, RoutedEventArgs e)
    {
        var apiKey = ApiKeyBox.Password;
        
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            await ShowDialogAsync("Error", "API Key tidak boleh kosong");
            return;
        }

        try
        {
            // Save to secure storage
            await SecureStorage.SetAsync("GeminiApiKey", apiKey);
            
            // Set in service
            _geminiService.SetApiKey(apiKey);
            
            await ShowDialogAsync("Sukses", "API Key berhasil disimpan!");
        }
        catch (Exception ex)
        {
            await ShowDialogAsync("Error", $"Gagal menyimpan API Key: {ex.Message}");
        }
    }

    private async void OnTestConnectionClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var isConnected = await _geminiService.TestConnectionAsync();
            
            if (isConnected)
            {
                await ShowDialogAsync("Sukses", "Koneksi ke Gemini API berhasil! ✅");
            }
            else
            {
                await ShowDialogAsync("Gagal", "Tidak dapat terhubung ke Gemini API. Periksa API Key Anda.");
            }
        }
        catch (Exception ex)
        {
            await ShowDialogAsync("Error", $"Terjadi kesalahan: {ex.Message}");
        }
    }

    private void OnBackClick(object sender, RoutedEventArgs e)
    {
        Frame.GoBack();
    }

    private async Task ShowDialogAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}

// Simple SecureStorage implementation
public static class SecureStorage
{
    private static readonly Dictionary<string, string> _storage = new();

    public static Task<string?> GetAsync(string key)
    {
        _storage.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public static Task SetAsync(string key, string value)
    {
        _storage[key] = value;
        return Task.CompletedTask;
    }
}
