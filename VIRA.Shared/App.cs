using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VIRA.Shared.Services;
using VIRA.Shared.ViewModels;
using VIRA.Shared.Views;
using VIRA.Shared.Repositories;
using Windows.UI;

namespace VIRA.Shared;

public partial class App : Microsoft.UI.Xaml.Application
{
    public static new App? Current => Microsoft.UI.Xaml.Application.Current as App;
    public IServiceProvider? Services { get; private set; }

    public App()
    {
#if __ANDROID__
        Android.Util.Log.Info("VIRA_App", "App constructor called");
#endif
        
        // Setup Dependency Injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        
        // Set default theme resources to avoid null reference
        try
        {
            this.RequestedTheme = ApplicationTheme.Dark;
#if __ANDROID__
            Android.Util.Log.Info("VIRA_App", "Theme set to Dark");
#endif
        }
        catch (System.Exception ex)
        {
#if __ANDROID__
            Android.Util.Log.Error("VIRA_App", $"Error setting theme: {ex.Message}");
#endif
        }
        
        // Load Modern Theme ResourceDictionary
        try
        {
            var modernTheme = new ResourceDictionary
            {
                Source = new Uri("ms-appx:///Styles/ModernTheme.xaml")
            };
            this.Resources.MergedDictionaries.Add(modernTheme);
#if __ANDROID__
            Android.Util.Log.Info("VIRA_App", "Modern Theme ResourceDictionary loaded");
#endif
        }
        catch (System.Exception ex)
        {
#if __ANDROID__
            Android.Util.Log.Error("VIRA_App", $"Error loading Modern Theme: {ex.Message}");
#endif
        }
        
#if __ANDROID__
        Android.Util.Log.Info("VIRA_App", "Services configured");
#endif
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register HttpClient
        services.AddSingleton<HttpClient>();

        // Register Preferences Service
        services.AddSingleton<IPreferencesService, PreferencesService>();
        
        // Register Secure Storage Manager
        services.AddSingleton<ISecureStorageManager, SecureStorageManager>();
        
        // Register API Key Validator
        services.AddSingleton<APIKeyValidator>();
        
        // Register Error Recovery Service
        services.AddSingleton<ErrorRecoveryService>();
        
        // Register Repositories
        services.AddSingleton<IMessageRepository, InMemoryMessageRepository>();
        services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        services.AddSingleton<IConfigRepository, InMemoryConfigRepository>();
        
        // Register Cache Service
        services.AddSingleton<CacheService>();

        // Register Task Management Services
        services.AddSingleton<TaskManager>();
        
        // Register Quick Action Service
        services.AddSingleton<QuickActionService>();
        
        // Register Task Analytics Service
        services.AddSingleton<TaskAnalyticsService>(provider =>
        {
            var taskManager = provider.GetRequiredService<TaskManager>();
            return new TaskAnalyticsService(taskManager);
        });
        
        // Register Reminder Service
        services.AddSingleton<ReminderService>(provider =>
        {
            var taskManager = provider.GetRequiredService<TaskManager>();
            var config = new ReminderConfig();
            
            // Try to load reminder config from SharedPreferences
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                
                config.Enabled = prefs?.GetBoolean("reminder_enabled", true) ?? true;
                config.MinutesBeforeDue = prefs?.GetInt("reminder_minutes_before", 15) ?? 15;
                config.EveningSummaryEnabled = prefs?.GetBoolean("evening_summary_enabled", true) ?? true;
                
                var eveningHour = prefs?.GetInt("evening_summary_hour", 20) ?? 20;
                var eveningMinute = prefs?.GetInt("evening_summary_minute", 0) ?? 0;
                config.EveningSummaryTime = new TimeSpan(eveningHour, eveningMinute, 0);
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading reminder config: {ex.Message}");
#endif
            }
            
            return new ReminderService(taskManager, config);
        });
        
        // Register Briefing Service
        services.AddSingleton<BriefingService>(provider =>
        {
            var taskManager = provider.GetRequiredService<TaskManager>();
            var weatherService = provider.GetRequiredService<WeatherApiService>();
            var newsService = provider.GetRequiredService<NewsApiService>();
            var config = new BriefingConfig();
            
            // Try to load briefing config from SharedPreferences
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                
                config.MorningBriefingEnabled = prefs?.GetBoolean("morning_briefing_enabled", true) ?? true;
                config.EveningSummaryEnabled = prefs?.GetBoolean("evening_summary_enabled", true) ?? true;
                
                var morningHour = prefs?.GetInt("morning_briefing_hour", 8) ?? 8;
                var morningMinute = prefs?.GetInt("morning_briefing_minute", 0) ?? 0;
                config.MorningBriefingTime = new TimeSpan(morningHour, morningMinute, 0);
                
                var eveningHour = prefs?.GetInt("evening_summary_hour", 20) ?? 20;
                var eveningMinute = prefs?.GetInt("evening_summary_minute", 0) ?? 0;
                config.EveningSummaryTime = new TimeSpan(eveningHour, eveningMinute, 0);
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading briefing config: {ex.Message}");
#endif
            }
            
            return new BriefingService(taskManager, weatherService, newsService, config);
        });
        
        // Register Proactive Suggestion Service
        services.AddSingleton<ProactiveSuggestionService>(provider =>
        {
            var taskManager = provider.GetRequiredService<TaskManager>();
            var analyticsService = provider.GetRequiredService<TaskAnalyticsService>();
            var config = new ProactiveSuggestionConfig();
            
            // Try to load config from SharedPreferences
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                
                config.Enabled = prefs?.GetBoolean("proactive_suggestions_enabled", true) ?? true;
                config.MinIntervalMinutes = prefs?.GetInt("suggestion_interval_minutes", 60) ?? 60;
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading proactive suggestion config: {ex.Message}");
#endif
            }
            
            return new ProactiveSuggestionService(taskManager, analyticsService, config);
        });

        // Register API Services
        services.AddSingleton<WeatherApiService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            var weatherService = new WeatherApiService(httpClient);
            
            // Try to load API key from SharedPreferences
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("weather_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    weatherService.SetApiKey(apiKey);
                    Android.Util.Log.Info("VIRA_App", "Weather API Key loaded from preferences");
                }
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading Weather API key: {ex.Message}");
#endif
            }
            
            return weatherService;
        });

        services.AddSingleton<NewsApiService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            var newsService = new NewsApiService(httpClient);
            
            // Try to load API key from SharedPreferences
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("news_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    newsService.SetApiKey(apiKey);
                    Android.Util.Log.Info("VIRA_App", "News API Key loaded from preferences");
                }
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading News API key: {ex.Message}");
#endif
            }
            
            return newsService;
        });

        // Register PatternRegistry with all dependencies
        services.AddSingleton<PatternRegistry>(provider =>
        {
            var taskManager = provider.GetRequiredService<TaskManager>();
            var weatherService = provider.GetRequiredService<WeatherApiService>();
            var newsService = provider.GetRequiredService<NewsApiService>();
            var analyticsService = provider.GetRequiredService<TaskAnalyticsService>();
            return new PatternRegistry(taskManager, weatherService, newsService, analyticsService);
        });

        // Register RuleBasedProcessor
        services.AddSingleton<RuleBasedProcessor>(provider =>
        {
            var patternRegistry = provider.GetRequiredService<PatternRegistry>();
            return new RuleBasedProcessor(patternRegistry);
        });

        // Register Gemini Service with API key loading
        services.AddSingleton<IGeminiService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            var geminiService = new GeminiChatbotService(httpClient);
            
            // Try to load API key from SharedPreferences (Android)
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("gemini_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    geminiService.SetApiKey(apiKey);
                    Android.Util.Log.Info("VIRA_App", "Gemini API Key loaded from preferences");
                }
                else
                {
                    Android.Util.Log.Warn("VIRA_App", "No Gemini API Key found in preferences");
                }
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading Gemini API key: {ex.Message}");
#endif
            }
            
            return geminiService;
        });

        // Register Groq Service (separate instance for Groq API)
        services.AddSingleton<IGeminiService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();
            var groqService = new GroqChatbotService(httpClient);
            
            // Try to load API key from SharedPreferences (Android)
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("groq_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    groqService.SetApiKey(apiKey);
                    Android.Util.Log.Info("VIRA_App", "Groq API Key loaded from preferences");
                }
                else
                {
                    Android.Util.Log.Warn("VIRA_App", "No Groq API Key found in preferences");
                }
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading Groq API key: {ex.Message}");
#endif
            }
            
            return groqService;
        });

        // Register HybridMessageProcessor
        services.AddSingleton<IMessageProcessor, HybridMessageProcessor>(provider =>
        {
            var ruleBasedProcessor = provider.GetRequiredService<RuleBasedProcessor>();
            var geminiService = provider.GetRequiredService<IGeminiService>();
            var preferencesService = provider.GetRequiredService<IPreferencesService>();
            
            // Create separate Groq service instance
            var httpClient = provider.GetRequiredService<HttpClient>();
            var groqService = new GroqChatbotService(httpClient);
            
            // Load Groq API key
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("groq_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    groqService.SetApiKey(apiKey);
                }
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading Groq API key for HybridProcessor: {ex.Message}");
#endif
            }
            
            // Create separate OpenAI service instance
            var openaiService = new OpenAIChatbotService(httpClient);
            
            // Load OpenAI API key
            try
            {
#if __ANDROID__
                var context = Android.App.Application.Context;
                var prefs = context?.GetSharedPreferences("vira_settings", Android.Content.FileCreationMode.Private);
                var apiKey = prefs?.GetString("openai_api_key", null);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    openaiService.SetApiKey(apiKey);
                }
#endif
            }
            catch (System.Exception ex)
            {
#if __ANDROID__
                Android.Util.Log.Error("VIRA_App", $"Error loading OpenAI API key for HybridProcessor: {ex.Message}");
#endif
            }
            
            return new HybridMessageProcessor(
                ruleBasedProcessor,
                geminiService,
                groqService,
                openaiService,
                preferencesService
            );
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
