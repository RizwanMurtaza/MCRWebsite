namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for the main services page
/// </summary>
public class ServicesPageViewModel
{
    public string Title { get; set; } = "Our Legal Services";
    public string Description { get; set; } = "Expert legal solutions across Immigration, Family Law, and Personal Injury";
    public List<ServiceCategoryViewModel> Categories { get; set; } = new();
}

/// <summary>
/// Represents a category of services (e.g., Immigration, Family Law)
/// </summary>
public class ServiceCategoryViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<ServiceItemViewModel> Services { get; set; } = new();
}

/// <summary>
/// Individual service item within a category
/// </summary>
public class ServiceItemViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public bool IsFeatured { get; set; }
}
