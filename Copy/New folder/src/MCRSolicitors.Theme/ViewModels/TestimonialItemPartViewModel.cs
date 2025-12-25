namespace MCRSolicitors.Theme.ViewModels;

/// <summary>
/// View model for editing Testimonial content part in CMS admin
/// </summary>
public class TestimonialItemPartViewModel
{
    public string Quote { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? CaseType { get; set; }
    public string? ClientImageUrl { get; set; }
    public int Rating { get; set; } = 5;
    public int DisplayOrder { get; set; } = 0;
}
