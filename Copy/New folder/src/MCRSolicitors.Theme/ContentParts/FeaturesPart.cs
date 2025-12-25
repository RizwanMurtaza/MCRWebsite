using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for Features/Why Choose Us section - editable from CMS admin
/// </summary>
public class FeaturesPart : ContentPart
{
    public string SectionTitle { get; set; } = "Why Choose Us";
    public string? SectionSubtitle { get; set; }
    public string? SectionDescription { get; set; }
    public string Layout { get; set; } = "Grid"; // Grid or List
    public string? ImageUrl { get; set; }
    public string ImagePosition { get; set; } = "Right"; // Left or Right
}

/// <summary>
/// Individual feature item
/// </summary>
public class FeatureItemPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-check";
    public int DisplayOrder { get; set; } = 0;
}
