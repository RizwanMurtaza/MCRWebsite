using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for Testimonials section - editable from CMS admin
/// </summary>
public class TestimonialsPart : ContentPart
{
    public string SectionTitle { get; set; } = "What Our Clients Say";
    public string? SectionSubtitle { get; set; }
    public string? SectionDescription { get; set; }
    public string Layout { get; set; } = "Grid"; // Grid or Carousel
}

/// <summary>
/// Individual testimonial item
/// </summary>
public class TestimonialItemPart : ContentPart
{
    public string Quote { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? CaseType { get; set; }
    public string? ClientImageUrl { get; set; }
    public int Rating { get; set; } = 5;
    public int DisplayOrder { get; set; } = 0;
}
