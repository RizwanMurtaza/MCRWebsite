namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for the main navigation component
/// </summary>
public class NavigationViewModel
{
    public string LogoUrl { get; set; } = "/MCRSolicitors.Theme/images/mcr_logo-01.png";
    public string LogoAlt { get; set; } = "MCR Solicitors";
    public string HomeUrl { get; set; } = "/";
    public List<NavigationItemModel> MenuItems { get; set; } = new();
    public string PhoneNumber { get; set; } = "0161 804 7777";
    public string PhoneDisplay { get; set; } = "0161 804 7777";
    public bool ShowCtaButton { get; set; } = true;
    public string CtaText { get; set; } = "Free Consultation";
    public string CtaUrl { get; set; } = "/contact";
}
