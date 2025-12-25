namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for service card component
/// </summary>
public class ServiceCardViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-balance-scale";
    public string? ImageUrl { get; set; }
    public string Url { get; set; } = "#";
    public string CtaText { get; set; } = "Learn More";
    public CardStyle Style { get; set; } = CardStyle.Default;
}

public class ServicesSectionViewModel
{
    public string SectionTitle { get; set; } = "Our Services";
    public string SectionSubtitle { get; set; } = "Comprehensive Legal Solutions";
    public string? SectionDescription { get; set; }
    public List<ServiceCardViewModel> Services { get; set; } = new();
    public int ColumnsPerRow { get; set; } = 3;
    public string? CtaText { get; set; }
    public string? CtaUrl { get; set; }
}

public enum CardStyle
{
    Default,
    Elevated,
    Bordered,
    Minimal
}
