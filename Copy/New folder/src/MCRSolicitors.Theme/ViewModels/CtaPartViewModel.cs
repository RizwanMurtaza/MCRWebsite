namespace MCRSolicitors.Theme.ViewModels;

/// <summary>
/// View model for editing CTA content part in CMS admin
/// </summary>
public class CtaPartViewModel
{
    public string? Subtitle { get; set; }
    public string Title { get; set; } = "Ready to Get Started?";
    public string? Description { get; set; }
    public string PrimaryButtonText { get; set; } = "Contact Us";
    public string PrimaryButtonUrl { get; set; } = "/contact";
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public string Style { get; set; } = "Banner";
    public bool ShowContactInfo { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
