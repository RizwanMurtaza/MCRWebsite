using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for Hero sections - editable from CMS admin
/// </summary>
public class HeroPart : ContentPart
{
    public string Title { get; set; } = "Expert Legal Services";
    public string Subtitle { get; set; } = "Your Trusted Legal Partner";
    public string Description { get; set; } = string.Empty;
    public string PrimaryButtonText { get; set; } = "Free Consultation";
    public string PrimaryButtonUrl { get; set; } = "/contact";
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public bool ShowTrustIndicators { get; set; } = true;
}
