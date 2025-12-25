using OrchardCore.Entities;
using OrchardCore.Settings;
using Starter.Theme.Models;
using Starter.Theme.Services.Abstractions;

namespace Starter.Theme.Services;

/// <summary>
/// Service to retrieve theme settings from Orchard Core's Site Settings
/// </summary>
public class ThemeSettingsService : IThemeSettingsService
{
    private readonly ISiteService _siteService;

    public ThemeSettingsService(ISiteService siteService)
    {
        _siteService = siteService;
    }

    public async Task<ThemeSettingsPart> GetSettingsAsync()
    {
        var site = await _siteService.GetSiteSettingsAsync();
        return site.As<ThemeSettingsPart>() ?? new ThemeSettingsPart();
    }

    public async Task<string> GetLogoUrlAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.LogoUrl ?? "/Starter.Theme/images/logo-placeholder.svg";
    }

    public async Task<string> GetSiteNameAsync()
    {
        var settings = await GetSettingsAsync();
        if (!string.IsNullOrEmpty(settings.SiteName))
        {
            return settings.SiteName;
        }

        var site = await _siteService.GetSiteSettingsAsync();
        return site.SiteName ?? "My Website";
    }

    public async Task<string> GetPrimaryColorAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.PrimaryColor ?? "#DE532A";
    }

    public async Task<string> GetPhoneAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Phone ?? "";
    }

    public async Task<string> GetEmailAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Email ?? "";
    }
}
