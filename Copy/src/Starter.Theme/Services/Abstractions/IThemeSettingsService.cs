using Starter.Theme.Models;

namespace Starter.Theme.Services.Abstractions;

/// <summary>
/// Service interface for retrieving theme settings
/// </summary>
public interface IThemeSettingsService
{
    Task<ThemeSettingsPart> GetSettingsAsync();

    // Convenience methods
    Task<string> GetLogoUrlAsync();
    Task<string> GetSiteNameAsync();
    Task<string> GetPrimaryColorAsync();
    Task<string> GetPhoneAsync();
    Task<string> GetEmailAsync();
}
