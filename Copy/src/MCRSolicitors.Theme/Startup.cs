using Microsoft.Extensions.DependencyInjection;
using MCRSolicitors.Theme.Services;
using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme;

/// <summary>
/// Startup class for registering theme services
/// Follows Open/Closed Principle - open for extension, closed for modification
/// </summary>
public class Startup : OrchardCore.Modules.StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Register services with dependency injection (Dependency Inversion Principle)
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<IFooterService, FooterService>();
    }
}
