namespace MCRSolicitors.Theme.Services.Abstractions;

/// <summary>
/// Service interface for site-wide settings
/// </summary>
public interface ISiteSettingsService
{
    string GetLogoUrl();
    string GetSiteName();
    string GetPhoneNumber();
    string GetEmail();
    string GetAddress();
    string GetSraNumber();
}
