using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Settings;
using YesSql;

namespace MCRSolicitors.Theme.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
    private readonly ISession _session;
    private readonly ISiteService _siteService;

    public NavigationViewComponent(ISession session, ISiteService siteService)
    {
        _session = session;
        _siteService = siteService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var site = await _siteService.GetSiteSettingsAsync();

        // Query navigation settings content item
        var navSettings = await _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "NavigationSettings" && x.Published)
            .FirstOrDefaultAsync();

        // Query menu items
        var menuItems = await _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "MenuItem" && x.Published)
            .ListAsync();

        // Build all menu items with their properties
        var allItems = menuItems.Select(m => new MenuItemViewModel
        {
            Id = m.ContentItemId,
            Title = GetFieldValue(m, "MenuItem", "Title") ?? m.DisplayText,
            Url = GetFieldValue(m, "MenuItem", "Url") ?? "#",
            IconClass = GetFieldValue(m, "MenuItem", "IconClass"),
            DisplayOrder = GetIntFieldValue(m, "MenuItem", "DisplayOrder"),
            OpenInNewTab = GetBoolFieldValue(m, "MenuItem", "OpenInNewTab"),
            ParentId = GetFieldValue(m, "MenuItem", "ParentId"),
            IsMegaMenu = GetBoolFieldValue(m, "MenuItem", "IsMegaMenu")
        }).ToList();

        // Build hierarchy - top level items have no parent
        var topLevelItems = allItems
            .Where(m => string.IsNullOrEmpty(m.ParentId))
            .OrderBy(m => m.DisplayOrder)
            .ToList();

        // Attach children to parents
        foreach (var parent in topLevelItems)
        {
            parent.Children = allItems
                .Where(m => m.ParentId == parent.Id)
                .OrderBy(m => m.DisplayOrder)
                .ToList();
        }

        var model = new NavigationViewModel
        {
            SiteName = site.SiteName ?? "MCR Solicitors",
            LogoUrl = GetFieldValue(navSettings, "NavigationSettings", "Logo"),
            PhoneNumber = GetFieldValue(navSettings, "NavigationSettings", "PhoneNumber") ?? "0161 552 2617",
            CtaButtonText = GetFieldValue(navSettings, "NavigationSettings", "CtaButtonText") ?? "Free Consultation",
            CtaButtonUrl = GetFieldValue(navSettings, "NavigationSettings", "CtaButtonUrl") ?? "/contact-us",
            ShowCtaButton = GetBoolFieldValue(navSettings, "NavigationSettings", "ShowCtaButton"),
            FacebookUrl = GetFieldValue(navSettings, "NavigationSettings", "FacebookUrl"),
            TwitterUrl = GetFieldValue(navSettings, "NavigationSettings", "TwitterUrl"),
            LinkedInUrl = GetFieldValue(navSettings, "NavigationSettings", "LinkedInUrl"),
            InstagramUrl = GetFieldValue(navSettings, "NavigationSettings", "InstagramUrl"),
            TikTokUrl = GetFieldValue(navSettings, "NavigationSettings", "TikTokUrl"),
            YouTubeUrl = GetFieldValue(navSettings, "NavigationSettings", "YouTubeUrl"),
            ShowSocialIcons = GetBoolFieldValue(navSettings, "NavigationSettings", "ShowSocialIcons"),
            MenuItems = topLevelItems
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
                // Not an array, continue to text handling
            }
        }

        // Handle TextField
        return field["Text"]?.ToString();
    }

    private int GetIntFieldValue(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return 0;
        var part = item.Content[partName];
        if (part == null) return 0;
        var field = part[fieldName];
        // NumericField stores value in Value property
        var value = field?["Value"]?.ToObject<decimal?>() ?? 0;
        return (int)value;
    }

    private bool GetBoolFieldValue(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return false;
        var part = item.Content[partName];
        if (part == null) return false;
        var field = part[fieldName];
        return field?["Value"]?.ToObject<bool>() ?? false;
    }
}

public class NavigationViewModel
{
    public string SiteName { get; set; } = "";
    public string? LogoUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CtaButtonText { get; set; }
    public string? CtaButtonUrl { get; set; }
    public bool ShowCtaButton { get; set; } = true;
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TikTokUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public bool ShowSocialIcons { get; set; } = false;
    public List<MenuItemViewModel> MenuItems { get; set; } = new();
}

public class MenuItemViewModel
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Url { get; set; } = "#";
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool OpenInNewTab { get; set; }
    public string? ParentId { get; set; }
    public bool IsMegaMenu { get; set; }
    public List<MenuItemViewModel> Children { get; set; } = new();
    public bool HasChildren => Children.Count > 0;
}
