using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for Services sections - editable from CMS admin
/// </summary>
public class ServicesPart : ContentPart
{
    public string SectionTitle { get; set; } = "Our Services";
    public string SectionSubtitle { get; set; } = string.Empty;
    public string? SectionDescription { get; set; }
    public int ColumnsPerRow { get; set; } = 3;
    public string? CtaText { get; set; }
    public string? CtaUrl { get; set; }
}

/// <summary>
/// Individual service item - linked to ServicesPart
/// </summary>
public class ServiceItemPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-balance-scale";
    public string Url { get; set; } = "#";
    public string CtaText { get; set; } = "Learn More";
    public int DisplayOrder { get; set; } = 0;
}
