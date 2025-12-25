namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for the hero section component
/// </summary>
public class HeroViewModel
{
    public string Title { get; set; } = "Expert Legal Services";
    public string Subtitle { get; set; } = "Your Trusted Legal Partner in Manchester";
    public string Description { get; set; } = "We provide comprehensive legal solutions tailored to your needs.";
    public string? BackgroundImageUrl { get; set; }
    public string BackgroundColor { get; set; } = "#FFF6F3";

    public HeroCtaButton PrimaryButton { get; set; } = new();
    public HeroCtaButton? SecondaryButton { get; set; }

    public HeroStyle Style { get; set; } = HeroStyle.Default;
    public bool ShowTrustIndicators { get; set; } = true;
    public List<TrustIndicator> TrustIndicators { get; set; } = new();
}

public class HeroCtaButton
{
    public string Text { get; set; } = "Get Started";
    public string Url { get; set; } = "/contact";
    public string? IconClass { get; set; }
    public ButtonStyle Style { get; set; } = ButtonStyle.Primary;
}

public class TrustIndicator
{
    public string Icon { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public enum HeroStyle
{
    Default,
    WithImage,
    FullWidth,
    Centered
}

public enum ButtonStyle
{
    Primary,
    Secondary,
    Outline,
    Ghost
}
