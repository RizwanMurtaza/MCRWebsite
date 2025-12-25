using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for global site settings - editable from CMS admin
/// Attached to the Site Settings content item
/// </summary>
public class SiteSettingsPart : ContentPart
{
    // Branding
    public string LogoUrl { get; set; } = "/MCRSolicitors.Theme/images/mcr_logo-01.png";
    public string SiteName { get; set; } = "MCR Solicitors";
    public string Tagline { get; set; } = "Expert Legal Services";

    // Contact Information
    public string PhoneNumber { get; set; } = "0161 804 7777";
    public string Email { get; set; } = "info@mcrsolicitors.co.uk";
    public string Address { get; set; } = "123 Deansgate, Manchester, M3 2BW";

    // Social Media
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }

    // Regulatory
    public string SraNumber { get; set; } = "648878";

    // Opening Hours
    public string WeekdayHours { get; set; } = "9:00 AM - 5:30 PM";
    public string SaturdayHours { get; set; } = "10:00 AM - 2:00 PM";
    public string SundayHours { get; set; } = "Closed";
}
