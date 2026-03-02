using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VIRA.Shared.Services;
using VIRA.Shared.ViewModels;
using VIRA.Shared.Views;
using Windows.UI;

namespace VIRA.Shared;

public partial class App : Microsoft.UI.Xaml.Application
{
    public static new App? Current => Microsoft.UI.Xaml.Application.Current as App;
    public IServiceProvider? Services { get; private set; }

    public App()
    {
        Android.Util.Log.Info("VIRA_App", "App constructor called");
        
        // Setup Dependency Injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        
        // Set default theme resources to avoid null reference
        try
        {
            this.RequestedTheme = ApplicationTheme.Dark;
            Android.Util.Log.Info("VIRA_App", "Theme set to Dark");
        }
        catch (System.Exception ex)
        {
            Android.Util.Log.Error("VIRA_App", $"Error setting theme: {ex.Message}");
        }
        
        Android.Util.Log.Info("VIRA_App", "Services configured");
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register HttpClient
        services.AddSingleton<HttpClient>();

        // Register Services with API key loading
        services.AddSingleton<IGeminiService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            var geminiService = new GeminiChatbotService(httpClient);
            
            // Try to load API key from SharedPreferences (Android)
            try
            {
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("gemini_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    geminiService.SetApiKey(apiKey);
                    Android.Util.Log.Info("VIRA_App", "API Key loaded from preferences");
                }
                else
                {
                    Android.Util.Log.Warn("VIRA_App", "No API Key found in preferences");
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Error("VIRA_App", $"Error loading API key: {ex.Message}");
            }
            
            return geminiService;
        });
        
        // Register dummy voice service by default
        // Platform-specific projects will override this
        services.AddSingleton<IVoiceService, DummyVoiceService>();

        // Register ViewModels
        services.AddTransient<MainChatViewModel>();
        
        // Allow platform-specific configuration
        ConfigurePlatformServices(services);
    }

    // Virtual method that platform-specific projects can override
    protected virtual void ConfigurePlatformServices(IServiceCollection services)
    {
        // Override in platform-specific App class
    }
}

// Dummy implementation for non-Android platforms
public class DummyVoiceService : IVoiceService
{
    public bool IsAvailable => false;

    public Task<string> RecognizeSpeechAsync()
    {
        return Task.FromResult(string.Empty);
    }

    public Task SpeakAsync(string text)
    {
        return Task.CompletedTask;
    }
    
    public Task PlayAudioAsync(byte[] audioData)
    {
        return Task.CompletedTask;
    }
    
    public void StopAudio()
    {
        // No-op for dummy implementation
    }
}
