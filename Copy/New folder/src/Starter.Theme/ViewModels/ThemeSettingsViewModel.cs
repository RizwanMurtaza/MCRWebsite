namespace Starter.Theme.ViewModels;

/// <summary>
/// View model for theme settings admin UI
/// </summary>
public class ThemeSettingsViewModel
{
    // Branding
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string SiteName { get; set; } = "My Website";
    public string? Tagline { get; set; }

    // Colors
    public string PrimaryColor { get; set; } = "#DE532A";
    public string PrimaryHoverColor { get; set; } = "#C74520";
    public string SecondaryColor { get; set; } = "#1a1a2e";

    // Typography
    public string HeadingFont { get; set; } = "'Rubik', sans-serif";
    public string BodyFont { get; set; } = "'Montserrat', sans-serif";

    // Contact Information
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? OpeningHours { get; set; }

    // Social Media
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }

    // Footer
    public string? FooterDescription { get; set; }
    public string? FooterAbout { get; set; }
    public string? FooterBadgesHtml { get; set; }
    public string? CopyrightText { get; set; }

    // Accreditations
    public string? AccreditationLogo1Url { get; set; }
    public string? AccreditationLogo1Alt { get; set; }
    public string? AccreditationLogo1Link { get; set; }
    public string? AccreditationLogo2Url { get; set; }
    public string? AccreditationLogo2Alt { get; set; }
    public string? AccreditationLogo2Link { get; set; }

    // Hero Section Defaults
    public string HeroTitle { get; set; } = "Welcome to Our Website";
    public string? HeroSubtitle { get; set; }
    public string? HeroDescription { get; set; }
    public string? HeroBackgroundUrl { get; set; }
    public string HeroButtonText { get; set; } = "Get Started";
    public string HeroButtonUrl { get; set; } = "/contact";
    public string HeroPrimaryButtonText { get; set; } = "Get Started";
    public string HeroPrimaryButtonUrl { get; set; } = "/contact";
    public string? HeroSecondaryButtonText { get; set; }
    public string? HeroSecondaryButtonUrl { get; set; }
}
