namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for statistics/numbers section
/// </summary>
public class StatsSectionViewModel
{
    public string? Title { get; set; }
    public string? SectionTitle { get; set; }
    public string? BackgroundColor { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public List<StatItem> Stats { get; set; } = new();
    public StatsLayout Layout { get; set; } = StatsLayout.Horizontal;
}

public class StatItem
{
    public string Value { get; set; } = "0";
    public string Suffix { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? IconClass { get; set; }
}

public enum StatsLayout
{
    Horizontal,
    Vertical,
    Grid
}
