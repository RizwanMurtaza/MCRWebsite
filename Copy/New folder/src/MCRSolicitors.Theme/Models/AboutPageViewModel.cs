namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for the About Us page
/// </summary>
public class AboutPageViewModel
{
    public string Title { get; set; } = "About MCR Solicitors";
    public string IntroText { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<CompanyValue> Values { get; set; } = new();
    public List<TeamMember> TeamMembers { get; set; } = new();
    public CompanyStats Stats { get; set; } = new();
}

public class CompanyValue
{
    public string IconClass { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// TeamMember is defined in TeamSectionViewModel.cs

public class CompanyStats
{
    public int YearsExperience { get; set; }
    public int HappyClients { get; set; }
    public int CasesWon { get; set; }
    public int SolicitorCount { get; set; }
}
