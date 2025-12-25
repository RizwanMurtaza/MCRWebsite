namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for team section
/// </summary>
public class TeamSectionViewModel
{
    public string SectionTitle { get; set; } = "Meet Our Team";
    public string? SectionSubtitle { get; set; }
    public string? SectionDescription { get; set; }
    public List<TeamMember> Members { get; set; } = new();
    public int ColumnsPerRow { get; set; } = 4;
    public bool ShowSocialLinks { get; set; } = true;
}

public class TeamMember
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Bio { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public List<string> Specializations { get; set; } = new();
}
