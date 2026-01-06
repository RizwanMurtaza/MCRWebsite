using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using YesSql;

namespace MCR.Theme.ViewComponents;

public class SiteWideSettingsViewComponent : ViewComponent
{
    private readonly ISession _session;

    public SiteWideSettingsViewComponent(ISession session)
    {
        _session = session;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? pageTitle = null, string? pageDescription = null, string? pageKeywords = null, string? pageOgImage = null)
    {
        // Query SiteWideSettings content type
        var settings = await _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "SiteWideSettings" && x.Published)
            .FirstOrDefaultAsync();

        var model = new SiteWideSettingsViewModel
        {
            // Page-specific overrides take precedence, then site-wide defaults
            // NO HARDCODED FALLBACKS - all from CMS
            MetaTitle = !string.IsNullOrEmpty(pageTitle) ? pageTitle : GetFieldValue(settings, "SiteWideSettings", "DefaultMetaTitle"),
            MetaDescription = !string.IsNullOrEmpty(pageDescription) ? pageDescription : GetFieldValue(settings, "SiteWideSettings", "DefaultMetaDescription"),
            MetaKeywords = !string.IsNullOrEmpty(pageKeywords) ? pageKeywords : GetFieldValue(settings, "SiteWideSettings", "DefaultMetaKeywords"),

            // Open Graph
            OgSiteName = GetFieldValue(settings, "SiteWideSettings", "OgSiteName"),
            OgImage = !string.IsNullOrEmpty(pageOgImage) ? pageOgImage : GetMediaPath(settings, "SiteWideSettings", "OgImage"),
            OgImageAlt = GetFieldValue(settings, "SiteWideSettings", "OgImageAlt"),
            TwitterHandle = GetFieldValue(settings, "SiteWideSettings", "TwitterHandle"),

            // Analytics
            GoogleAnalyticsId = GetFieldValue(settings, "SiteWideSettings", "GoogleAnalyticsId"),
            GoogleTagManagerId = GetFieldValue(settings, "SiteWideSettings", "GoogleTagManagerId"),

            // Favicon/Icons
            FaviconUrl = GetMediaPath(settings, "SiteWideSettings", "FaviconImage"),
            AppleTouchIconUrl = GetMediaPath(settings, "SiteWideSettings", "AppleTouchIcon"),
            ThemeColor = GetFieldValue(settings, "SiteWideSettings", "ThemeColor"),

            // JSON-LD Organization
            OrganizationName = GetFieldValue(settings, "SiteWideSettings", "OrganizationName"),
            OrganizationLogo = GetMediaPath(settings, "SiteWideSettings", "OrganizationLogo"),
            OrganizationAddress = GetFieldValue(settings, "SiteWideSettings", "OrganizationAddress"),
            OrganizationPostcode = GetFieldValue(settings, "SiteWideSettings", "OrganizationPostcode"),
            OrganizationCity = GetFieldValue(settings, "SiteWideSettings", "OrganizationCity"),
            OrganizationPhone = GetFieldValue(settings, "SiteWideSettings", "OrganizationPhone"),
            OrganizationEmail = GetFieldValue(settings, "SiteWideSettings", "OrganizationEmail")
        };

        return View(model);
    }

    private string? GetFieldValue(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return null;
        var part = item.Content[partName];
        if (part == null) return null;
        var field = part[fieldName];
        return field?["Text"]?.ToString();
    }

    private string? GetMediaPath(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return null;
        var part = item.Content[partName];
        if (part == null) return null;
        var field = part[fieldName];
        if (field == null) return null;

        var paths = field["Paths"];
        if (paths != null)
        {
            try
            {
                var pathsArray = paths as System.Collections.IEnumerable;
                if (pathsArray != null)
                {
                    foreach (var path in pathsArray)
                    {
                        if (path != null)
                        {
                            return "/media/" + path.ToString();
                        }
                    }
                }
            }
            catch
            {
                // Not an array
            }
        }
        return null;
    }
}

public class SiteWideSettingsViewModel
{
    // Meta Tags
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    // Open Graph
    public string? OgSiteName { get; set; }
    public string? OgImage { get; set; }
    public string? OgImageAlt { get; set; }
    public string? TwitterHandle { get; set; }

    // Analytics
    public string? GoogleAnalyticsId { get; set; }
    public string? GoogleTagManagerId { get; set; }

    // Favicon/Icons
    public string? FaviconUrl { get; set; }
    public string? AppleTouchIconUrl { get; set; }
    public string? ThemeColor { get; set; }

    // JSON-LD Organization
    public string? OrganizationName { get; set; }
    public string? OrganizationLogo { get; set; }
    public string? OrganizationAddress { get; set; }
    public string? OrganizationPostcode { get; set; }
    public string? OrganizationCity { get; set; }
    public string? OrganizationPhone { get; set; }
    public string? OrganizationEmail { get; set; }

    public bool HasOrganizationData => !string.IsNullOrEmpty(OrganizationName);
}
