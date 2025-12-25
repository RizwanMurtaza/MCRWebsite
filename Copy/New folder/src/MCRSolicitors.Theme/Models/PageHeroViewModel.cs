namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for inner page hero sections
/// </summary>
public class PageHeroViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public bool ShowPattern { get; set; } = true;
}
