using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Settings;
using YesSql;

namespace MCRSolicitors.Theme.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    private readonly ISession _session;
    private readonly ISiteService _siteService;

    public FooterViewComponent(ISession session, ISiteService siteService)
    {
        _session = session;
        _siteService = siteService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var site = await _siteService.GetSiteSettingsAsync();

        // Query footer settings content item
        var footerSettings = await _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "FooterSettings" && x.Published)
            .FirstOrDefaultAsync();

        var model = new FooterViewModel
        {
            SiteName = site.SiteName ?? "MCR Solicitors",
            Tagline = GetFieldValue(footerSettings, "FooterSettings", "Tagline"),
            CompanyName = GetFieldValue(footerSettings, "FooterSettings", "CompanyName"),
            CompanyRegistration = GetFieldValue(footerSettings, "FooterSettings", "CompanyRegistration"),
            VatNumber = GetFieldValue(footerSettings, "FooterSettings", "VatNumber"),
            PhoneNumber = GetFieldValue(footerSettings, "FooterSettings", "PhoneNumber"),
            Email = GetFieldValue(footerSettings, "FooterSettings", "Email"),
            Address = GetFieldValue(footerSettings, "FooterSettings", "Address"),
            RegisteredAddress = GetFieldValue(footerSettings, "FooterSettings", "RegisteredAddress"),
            FacebookUrl = GetFieldValue(footerSettings, "FooterSettings", "FacebookUrl"),
            TwitterUrl = GetFieldValue(footerSettings, "FooterSettings", "TwitterUrl"),
            LinkedInUrl = GetFieldValue(footerSettings, "FooterSettings", "LinkedInUrl"),
            TikTokUrl = GetFieldValue(footerSettings, "FooterSettings", "TikTokUrl"),
            SraNumber = GetFieldValue(footerSettings, "FooterSettings", "SraNumber"),
            SraLogo = GetFieldValue(footerSettings, "FooterSettings", "SraLogo"),
            LexcelLogo = GetFieldValue(footerSettings, "FooterSettings", "LexcelLogo"),
            WeekdayHours = GetFieldValue(footerSettings, "FooterSettings", "WeekdayHours"),
            SaturdayHours = GetFieldValue(footerSettings, "FooterSettings", "SaturdayHours"),
            SundayHours = GetFieldValue(footerSettings, "FooterSettings", "SundayHours"),
            CopyrightText = GetFieldValue(footerSettings, "FooterSettings", "CopyrightText"),
            Year = DateTime.Now.Year
        };

        return View(model);
    }

    private string? GetFieldValue(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return null;

        var part = item.Content[partName];
        if (part == null) return null;

        var field = part[fieldName];
        if (field == null) return null;

        // Handle MediaField (has Paths array)
        if (field["Paths"] != null && field["Paths"].HasValues)
        {
            return "/media/" + field["Paths"][0]?.ToString();
        }

        // Handle TextField
        return field["Text"]?.ToString();
    }
}

public class FooterViewModel
{
    public string SiteName { get; set; } = "";
    public string? Tagline { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyRegistration { get; set; }
    public string? VatNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? RegisteredAddress { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? TikTokUrl { get; set; }
    public string? SraNumber { get; set; }
    public string? SraLogo { get; set; }
    public string? LexcelLogo { get; set; }
    public string? WeekdayHours { get; set; }
    public string? SaturdayHours { get; set; }
    public string? SundayHours { get; set; }
    public string? CopyrightText { get; set; }
    public int Year { get; set; }
}
