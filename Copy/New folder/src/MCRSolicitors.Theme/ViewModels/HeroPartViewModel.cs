namespace MCRSolicitors.Theme.ViewModels;

/// <summary>
/// View model for HeroPart editing and display
/// </summary>
public class HeroPartViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PrimaryButtonText { get; set; } = "Free Consultation";
    public string PrimaryButtonUrl { get; set; } = "/contact";
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public bool ShowTrustIndicators { get; set; } = true;
}
