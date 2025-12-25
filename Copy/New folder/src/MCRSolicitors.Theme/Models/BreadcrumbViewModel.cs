namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for breadcrumb navigation
/// </summary>
public class BreadcrumbViewModel
{
    public List<BreadcrumbItem> Items { get; set; } = new();
}

public class BreadcrumbItem
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
