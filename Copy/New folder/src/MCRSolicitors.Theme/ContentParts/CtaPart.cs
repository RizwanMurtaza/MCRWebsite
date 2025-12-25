using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for Call-to-Action sections - editable from CMS admin
/// </summary>
public class CtaPart : ContentPart
{
    public string? Subtitle { get; set; }
    public string Title { get; set; } = "Ready to Get Started?";
    public string? Description { get; set; }
    public string PrimaryButtonText { get; set; } = "Contact Us";
    public string PrimaryButtonUrl { get; set; } = "/contact";
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public string Style { get; set; } = "Banner"; // Banner or Inline
    public bool ShowContactInfo { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
