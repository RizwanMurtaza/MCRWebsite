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

        // Use default menu items if none exist in CMS
        if (topLevelItems.Count == 0)
        {
            topLevelItems = GetDefaultMenuItems();
        }

        var model = new NavigationViewModel
        {
            SiteName = site.SiteName ?? "MCR Solicitors",
            LogoUrl = GetFieldValue(navSettings, "NavigationSettings", "Logo"),
            PhoneNumber = GetFieldValue(navSettings, "NavigationSettings", "PhoneNumber") ?? "0161 552 2617",
            CtaButtonText = GetFieldValue(navSettings, "NavigationSettings", "CtaButtonText") ?? "Free Consultation",
            CtaButtonUrl = GetFieldValue(navSettings, "NavigationSettings", "CtaButtonUrl") ?? "/contact-us",
            MenuItems = topLevelItems
        };

        return View(model);
    }

    private List<MenuItemViewModel> GetDefaultMenuItems()
    {
        return new List<MenuItemViewModel>
        {
            new MenuItemViewModel { Id = "home", Title = "Home", Url = "/", DisplayOrder = 1 },
            new MenuItemViewModel
            {
                Id = "immigration",
                Title = "Immigration",
                Url = "/immigration",
                DisplayOrder = 2,
                Children = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Id = "visa-uk", Title = "UK Visa Applications", Url = "/immigration/uk-visa", DisplayOrder = 1 },
                    new MenuItemViewModel { Id = "spouse-visa", Title = "Spouse Visa", Url = "/immigration/spouse-visa", DisplayOrder = 2 },
                    new MenuItemViewModel { Id = "work-visa", Title = "Work Visa", Url = "/immigration/work-visa", DisplayOrder = 3 },
                    new MenuItemViewModel { Id = "asylum", Title = "Asylum & Refugees", Url = "/immigration/asylum", DisplayOrder = 4 },
                    new MenuItemViewModel { Id = "appeals", Title = "Immigration Appeals", Url = "/immigration/appeals", DisplayOrder = 5 }
                }
            },
            new MenuItemViewModel
            {
                Id = "family-law",
                Title = "Family Law",
                Url = "/family-law",
                DisplayOrder = 3,
                Children = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Id = "divorce", Title = "Divorce", Url = "/family-law/divorce", DisplayOrder = 1 },
                    new MenuItemViewModel { Id = "child-custody", Title = "Child Custody", Url = "/family-law/child-custody", DisplayOrder = 2 },
                    new MenuItemViewModel { Id = "prenuptial", Title = "Prenuptial Agreements", Url = "/family-law/prenuptial", DisplayOrder = 3 }
                }
            },
            new MenuItemViewModel
            {
                Id = "personal-injury",
                Title = "Personal Injury",
                Url = "/personal-injury",
                DisplayOrder = 4,
                Children = new List<MenuItemViewModel>
                {
                    new MenuItemViewModel { Id = "accident-claims", Title = "Accident Claims", Url = "/personal-injury/accident-claims", DisplayOrder = 1 },
                    new MenuItemViewModel { Id = "medical-negligence", Title = "Medical Negligence", Url = "/personal-injury/medical-negligence", DisplayOrder = 2 },
                    new MenuItemViewModel { Id = "workplace-injury", Title = "Workplace Injury", Url = "/personal-injury/workplace-injury", DisplayOrder = 3 }
                }
            },
            new MenuItemViewModel { Id = "about", Title = "About Us", Url = "/about-us", DisplayOrder = 5 },
            new MenuItemViewModel { Id = "contact", Title = "Contact", Url = "/contact-us", DisplayOrder = 6 }
        };
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
