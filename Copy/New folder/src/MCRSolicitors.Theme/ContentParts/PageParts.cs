using OrchardCore.ContentManagement;

namespace MCRSolicitors.Theme.ContentParts;

/// <summary>
/// Content Part for general page content
/// </summary>
public class PageContentPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? FeaturedImageUrl { get; set; }
}

/// <summary>
/// Content Part for page hero/banner sections
/// </summary>
public class PageHeroPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public bool ShowBreadcrumb { get; set; } = true;
}

/// <summary>
/// Content Part for Service Detail pages
/// </summary>
public class ServiceDetailPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-balance-scale";
    public string? FeaturedImageUrl { get; set; }
    public string? MetaDescription { get; set; }
    public string Category { get; set; } = "General"; // Immigration, FamilyLaw, PersonalInjury
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Content section within a service detail page
/// </summary>
public class ServiceContentSectionPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public string? ParentServiceId { get; set; }
}

/// <summary>
/// FAQ item for services or general pages
/// </summary>
public class FaqItemPart : ContentPart
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Team member for About page
/// </summary>
public class TeamMemberPart : ContentPart
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Email { get; set; }
    public string? LinkedInUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Company value/core principle
/// </summary>
public class CompanyValuePart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = "fas fa-check";
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Accreditation/certification badge
/// </summary>
public class AccreditationPart : ContentPart
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Office location for Contact page
/// </summary>
public class OfficePart : ContentPart
{
    public string Name { get; set; } = "Head Office";
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? OpeningHours { get; set; }
    public string? GoogleMapsEmbedUrl { get; set; }
    public bool IsPrimary { get; set; } = true;
}
