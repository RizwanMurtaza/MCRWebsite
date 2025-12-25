namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for individual service detail pages
/// </summary>
public class ServiceDetailViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public string CategoryTitle { get; set; } = string.Empty;
    public string CategoryUrl { get; set; } = string.Empty;

    /// <summary>
    /// Main content sections for the service
    /// </summary>
    public List<ContentSection> ContentSections { get; set; } = new();

    /// <summary>
    /// Key benefits or features of this service
    /// </summary>
    public List<ServiceBenefit> Benefits { get; set; } = new();

    /// <summary>
    /// FAQ items for this service
    /// </summary>
    public List<FaqItem> Faqs { get; set; } = new();

    /// <summary>
    /// Related services in sidebar
    /// </summary>
    public List<RelatedService> RelatedServices { get; set; } = new();
}

public class ContentSection
{
    public string Title { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
}

public class ServiceBenefit
{
    public string IconClass { get; set; } = "fas fa-check";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class FaqItem
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class RelatedService
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
