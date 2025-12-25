using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using Starter.Theme.Drivers;
using Starter.Theme.Models;
using Starter.Theme.Services;
using Starter.Theme.Services.Abstractions;

namespace Starter.Theme;

/// <summary>
/// Startup class for registering theme services and content parts
/// </summary>
public class Startup : OrchardCore.Modules.StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Register ThemeSettingsPart
        services.AddContentPart<ThemeSettingsPart>();
        services.AddScoped<IContentPartDisplayDriver, ThemeSettingsDisplayDriver>();

        // Register as Site Settings
        services.AddScoped<IDisplayDriver<ISite>, ThemeSettingsSiteSettingsDisplayDriver>();

        // Register services
        services.AddScoped<IThemeSettingsService, ThemeSettingsService>();

        // Register migrations
        services.AddScoped<IDataMigration, Migrations>();

        // Register permissions
        services.AddScoped<IPermissionProvider, Permissions>();

        // Register admin menu
        services.AddScoped<INavigationProvider, AdminMenu>();
    }
}
