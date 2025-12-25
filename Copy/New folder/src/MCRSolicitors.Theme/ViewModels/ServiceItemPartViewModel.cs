namespace MCRSolicitors.Theme.ViewModels;

/// <summary>
/// View model for editing ServiceItem content part in CMS admin
/// </summary>
public class ServiceItemPartViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-balance-scale";
    public string Url { get; set; } = "#";
    public string CtaText { get; set; } = "Learn More";
    public int DisplayOrder { get; set; } = 0;
}
