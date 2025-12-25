namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for Call-to-Action section
/// </summary>
public class CtaSectionViewModel
{
    public string Title { get; set; } = "Ready to Get Started?";
    public string? Subtitle { get; set; }
    public string Description { get; set; } = "Contact us today for a free consultation.";
    public string? BackgroundColor { get; set; }
    public string? BackgroundImageUrl { get; set; }

    public HeroCtaButton PrimaryButton { get; set; } = new();
    public HeroCtaButton? SecondaryButton { get; set; }

    public CtaStyle Style { get; set; } = CtaStyle.Banner;
    public bool ShowContactInfo { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public enum CtaStyle
{
    Banner,
    Card,
    FullWidth,
    Split
}
