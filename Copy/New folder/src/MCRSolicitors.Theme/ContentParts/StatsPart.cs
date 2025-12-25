using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for Statistics section - editable from CMS admin
/// </summary>
public class StatsPart : ContentPart
{
    public string? Title { get; set; }
    public string Layout { get; set; } = "Horizontal"; // Horizontal or Grid
}

/// <summary>
/// Individual stat item
/// </summary>
public class StatItemPart : ContentPart
{
    public string Value { get; set; } = "0";
    public string? Suffix { get; set; }
    public string? Prefix { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
