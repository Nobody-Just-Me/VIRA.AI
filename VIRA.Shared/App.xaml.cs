using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using VIRA.Shared.Services;
using VIRA.Shared.ViewModels;
using VIRA.Shared.Views;

namespace VIRA.Shared;

public partial class App : Application
{
    public static new App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        this.InitializeComponent();
        
        // Setup Dependency Injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register HttpClient
        services.AddSingleton<HttpClient>();

        // Register Services
        services.AddSingleton<IGeminiService, GeminiChatbotService>();
        
#if __ANDROID__
        services.AddSingleton<IVoiceService, Mobile.Services.AndroidVoiceService>();
#else
        services.AddSingleton<IVoiceService, DummyVoiceService>();
#endif

        // Register ViewModels
        services.AddTransient<MainChatViewModel>();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = new Window();
        var rootFrame = new Frame();
        rootFrame.Navigate(typeof(MainPage));
        window.Content = rootFrame;
        window.Activate();
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
}
