namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for testimonials section
/// </summary>
public class TestimonialSectionViewModel
{
    public string SectionTitle { get; set; } = "What Our Clients Say";
    public string? SectionSubtitle { get; set; }
    public List<TestimonialItem> Testimonials { get; set; } = new();
    public TestimonialLayout Layout { get; set; } = TestimonialLayout.Carousel;
    public bool ShowRating { get; set; } = true;
}

public class TestimonialItem
{
    public string Quote { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ClientRole { get; set; }
    public string? ClientImage { get; set; }
    public int Rating { get; set; } = 5;
    public string? CaseType { get; set; }
}

public enum TestimonialLayout
{
    Carousel,
    Grid,
    Featured,
    Masonry
}
