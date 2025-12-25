namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for the footer component
/// </summary>
public class FooterViewModel
{
    public string LogoUrl { get; set; } = "/MCRSolicitors.Theme/images/mcr_logo-01.png";
    public string LogoAlt { get; set; } = "MCR Solicitors";
    public string CompanyDescription { get; set; } = "MCR Solicitors is a leading law firm in Manchester providing expert legal services with a personal touch.";

    public FooterContactInfo Contact { get; set; } = new();
    public List<FooterLinkSection> LinkSections { get; set; } = new();
    public List<SocialMediaLink> SocialLinks { get; set; } = new();
    public List<AccreditationBadge> Accreditations { get; set; } = new();

    public string CopyrightText { get; set; } = $"© {DateTime.Now.Year} MCR Solicitors. All Rights Reserved.";
    public string SraNumber { get; set; } = "648878";
}

public class FooterContactInfo
{
    public string Address { get; set; } = "123 Deansgate, Manchester, M3 2BW";
    public string Phone { get; set; } = "0161 804 7777";
    public string Email { get; set; } = "info@mcrsolicitors.co.uk";
    public string OpeningHours { get; set; } = "Mon - Fri: 9:00 AM - 5:30 PM";
}

public class FooterLinkSection
{
    public string Title { get; set; } = string.Empty;
    public List<FooterLink> Links { get; set; } = new();
}

public class FooterLink
{
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public bool IsExternal { get; set; }
}

public class SocialMediaLink
{
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = "#";
    public string IconClass { get; set; } = string.Empty;
}

public class AccreditationBadge
{
    public string ImageUrl { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
}
