namespace MCRSolicitors.Theme.ViewModels;

/// <summary>
/// View model for editing StatItem content part in CMS admin
/// </summary>
public class StatItemPartViewModel
{
    public string Value { get; set; } = "0";
    public string? Suffix { get; set; }
    public string? Prefix { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
