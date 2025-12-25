namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for feature/why choose us section
/// </summary>
public class FeatureSectionViewModel
{
    public string SectionTitle { get; set; } = "Why Choose Us";
    public string? SectionSubtitle { get; set; }
    public string? SectionDescription { get; set; }
    public List<FeatureItem> Features { get; set; } = new();
    public FeatureLayout Layout { get; set; } = FeatureLayout.Grid;
    public string? ImageUrl { get; set; }
    public ImagePosition ImagePosition { get; set; } = ImagePosition.Right;
}

public class FeatureItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-check";
    public string? ImageUrl { get; set; }
}

public enum FeatureLayout
{
    Grid,
    List,
    Alternating,
    IconList
}

public enum ImagePosition
{
    Left,
    Right,
    Top,
    Bottom
}
