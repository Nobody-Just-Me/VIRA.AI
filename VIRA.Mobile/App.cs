using Microsoft.Extensions.DependencyInjection;
using VIRA.Shared.Services;
using VIRA.Mobile.Services;

namespace VIRA.Mobile;

public class App : VIRA.Shared.App
{
    protected override void ConfigurePlatformServices(IServiceCollection services)
    {
        // Replace dummy voice service with Android implementation
        services.AddSingleton<IVoiceService, AndroidVoiceService>();
    }
}
