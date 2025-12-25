namespace MCRSolicitors.Theme.Models;

/// <summary>
/// Represents a single navigation item in the menu
/// </summary>
public class NavigationItemModel
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public bool IsActive { get; set; }
    public bool IsExternal { get; set; }
    public string? IconClass { get; set; }
    public List<NavigationItemModel> Children { get; set; } = new();
    public bool HasChildren => Children.Count > 0;
}
