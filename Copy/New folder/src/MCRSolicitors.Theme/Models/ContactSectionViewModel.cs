namespace MCRSolicitors.Theme.Models;

/// <summary>
/// View model for contact section
/// </summary>
public class ContactSectionViewModel
{
    public string SectionTitle { get; set; } = "Get In Touch";
    public string? SectionSubtitle { get; set; }
    public string? SectionDescription { get; set; }

    public ContactInfo ContactDetails { get; set; } = new();
    public bool ShowMap { get; set; } = true;
    public string? MapEmbedUrl { get; set; }
    public bool ShowContactForm { get; set; } = true;

    public ContactLayout Layout { get; set; } = ContactLayout.SplitForm;
}

public class ContactInfo
{
    public string Address { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = "Manchester";
    public string PostCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Dictionary<string, string> OpeningHours { get; set; } = new();
}

public enum ContactLayout
{
    SplitForm,
    FullWidthForm,
    MapWithInfo,
    CardsGrid
}
