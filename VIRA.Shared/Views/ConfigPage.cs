using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VIRA.Shared.Services;
using Windows.UI;
using Button = Microsoft.UI.Xaml.Controls.Button;
using PasswordBox = Microsoft.UI.Xaml.Controls.PasswordBox;

namespace VIRA.Shared.Views;

// Plain class that builds UI - does NOT inherit from Page to avoid source generator
public sealed class ConfigPage
{
    private readonly IGeminiService? _geminiService;
    private PasswordBox? _apiKeyBox;
    private Button? _saveButton;
    private Button? _testButton;
    private Button? _backButton;
    private Grid? _mainGrid;

    public ConfigPage()
    {
        _geminiService = App.Current?.Services?.GetService<IGeminiService>();
    }

    public UIElement BuildUI()
    {
        _mainGrid = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 22, 34)),
            Padding = new Thickness(16)
        };

        _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // Header
        _mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content

        // Header
        var header = CreateHeader();
        Grid.SetRow(header, 0);
        _mainGrid.Children.Add(header);

        // Content
        var content = CreateContent();
        Grid.SetRow(content, 1);
        _mainGrid.Children.Add(content);

        LoadSettings();
        
        return _mainGrid;
    }

    private Grid CreateHeader()
    {
        var header = new Grid
        {
            Padding = new Thickness(0, 8, 0, 8)
        };

        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        _backButton = new Button
        {
            Content = "←",
            FontSize = 24,
            Background = new SolidColorBrush(Colors.Transparent),
            Foreground = new SolidColorBrush(Colors.White)
        };
        // _backButton.Click += (s, e) => { /* TODO: Navigate back */ };
        Grid.SetColumn(_backButton, 0);
        header.Children.Add(_backButton);

        var title = new TextBlock
        {
            Text = "Pengaturan",
            FontSize = 24,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 1);
        header.Children.Add(title);

        return header;
    }

    private StackPanel CreateContent()
    {
        var content = new StackPanel
        {
            Spacing = 16,
            MaxWidth = 400
        };

        // API Key Label
        var apiKeyLabel = new TextBlock
        {
            Text = "Gemini API Key",
            FontSize = 16,
            Foreground = new SolidColorBrush(Colors.White),
            Margin = new Thickness(0, 16, 0, 8)
        };
        content.Children.Add(apiKeyLabel);

        // API Key Input
        _apiKeyBox = new PasswordBox
        {
            PlaceholderText = "Masukkan API Key Anda",
            Height = 50,
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 40, 60)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16, 0, 16, 0)
        };
        content.Children.Add(_apiKeyBox);

        // Info Text
        var infoText = new TextBlock
        {
            Text = "Dapatkan API Key gratis di: https://makersuite.google.com/app/apikey",
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150)),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 16)
        };
        content.Children.Add(infoText);

        // Save Button
        _saveButton = new Button
        {
            Content = "Simpan API Key",
            Height = 50,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(255, 37, 106, 244)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(8),
            FontSize = 16
        };
        _saveButton.Click += OnSaveApiKeyClick;
        content.Children.Add(_saveButton);

        // Test Button
        _testButton = new Button
        {
            Content = "Test Koneksi",
            Height = 50,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 40, 60)),
            Foreground = new SolidColorBrush(Colors.White),
            CornerRadius = new CornerRadius(8),
            FontSize = 16
        };
        _testButton.Click += OnTestConnectionClick;
        content.Children.Add(_testButton);

        return content;
    }

    private async void LoadSettings()
    {
        try
        {
            var apiKey = await SecureStorage.GetAsync("GeminiApiKey");
            if (!string.IsNullOrEmpty(apiKey))
            {
                _apiKeyBox.Password = apiKey;
            }
        }
        catch
        {
            // Ignore if secure storage not available
        }
    }

    private async void OnSaveApiKeyClick(object sender, RoutedEventArgs e)
    {
        if (_apiKeyBox == null || _geminiService == null) return;
        
        var apiKey = _apiKeyBox.Password;
        
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            await ShowDialogAsync("Error", "API Key tidak boleh kosong");
            return;
        }

        try
        {
            await SecureStorage.SetAsync("GeminiApiKey", apiKey);
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
        if (_geminiService == null) return;
        
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

    private async Task ShowDialogAsync(string title, string message)
    {
        if (_mainGrid == null) return;
        
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = _mainGrid.XamlRoot
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
