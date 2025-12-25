using MCRSolicitors.Theme.Services.Abstractions;

namespace MCRSolicitors.Theme.Services;

/// <summary>
/// Default implementation of site settings service
/// Can be extended to pull from Orchard CMS site settings
/// </summary>
public class SiteSettingsService : ISiteSettingsService
{
    public string GetLogoUrl() => "/MCRSolicitors.Theme/images/mcr_logo-01.png";

    public string GetSiteName() => "MCR Solicitors";

    public string GetPhoneNumber() => "0161 804 7777";

    public string GetEmail() => "info@mcrsolicitors.co.uk";

    public string GetAddress() => "123 Deansgate, Manchester, M3 2BW";

    public string GetSraNumber() => "648878";
}
