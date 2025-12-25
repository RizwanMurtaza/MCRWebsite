namespace MCRSolicitors.Theme.ViewModels;

/// <summary>
/// View model for editing FeatureItem content part in CMS admin
/// </summary>
public class FeatureItemPartViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-check";
    public int DisplayOrder { get; set; } = 0;
}
