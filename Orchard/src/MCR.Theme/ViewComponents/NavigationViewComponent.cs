using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Settings;
using YesSql;

namespace MCR.Theme.ViewComponents;

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
        // Get NavigationSettings from Site Settings (CustomSettings are stored in Properties)
        var site = await _siteService.GetSiteSettingsAsync();
        var navSettingsPart = site.Properties["NavigationSettings"]?["NavigationSettings"]?.AsObject();

        // Query the Main Menu content item
        var menu = await _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "Menu" && x.Published)
            .FirstOrDefaultAsync();

        var model = new NavigationViewModel
        {
            // NO FALLBACK VALUES - all from CMS Site Settings
            LogoUrl = GetMediaPathFromPart(navSettingsPart, "Logo"),
            LogoAlt = GetFieldValueFromPart(navSettingsPart, "LogoAlt"),
            LogoWidth = GetIntFieldValueFromPart(navSettingsPart, "LogoWidth"),
            LogoHeight = GetIntFieldValueFromPart(navSettingsPart, "LogoHeight"),
            MobileLogoUrl = GetMediaPathFromPart(navSettingsPart, "MobileLogo"),
            SiteName = GetFieldValueFromPart(navSettingsPart, "SiteName"),
            CtaButtonText = GetFieldValueFromPart(navSettingsPart, "CtaButtonText"),
            CtaButtonUrl = GetFieldValueFromPart(navSettingsPart, "CtaButtonUrl"),
            ShowCtaButton = GetBoolFieldValueFromPart(navSettingsPart, "ShowCtaButton"),
            MenuItems = ParseMenuItems(menu)
        };

        return View(model);
    }

    private List<MenuItemViewModel> ParseMenuItems(ContentItem? menu)
    {
        var items = new List<MenuItemViewModel>();
        if (menu == null) return items;

        var menuItemsList = menu.Content["MenuItemsListPart"]?["MenuItems"];
        if (menuItemsList == null) return items;

        foreach (var menuItem in menuItemsList)
        {
            items.Add(ParseMenuItem(menuItem));
        }

        return items;
    }

    private MenuItemViewModel ParseMenuItem(dynamic menuItem)
    {
        var item = new MenuItemViewModel
        {
            Id = menuItem["ContentItemId"]?.ToString() ?? "",
            Name = menuItem["LinkMenuItemPart"]?["Name"]?.ToString() ?? "",
            Url = menuItem["LinkMenuItemPart"]?["Url"]?.ToString() ?? "#",
            OpenInNewTab = menuItem["LinkMenuItemPart"]?["Target"]?.ToString() == "_blank"
        };

        // Parse children if any
        var childMenuItems = menuItem["MenuItemsListPart"]?["MenuItems"];
        if (childMenuItems != null)
        {
            foreach (var child in childMenuItems)
            {
                item.Children.Add(ParseMenuItem(child));
            }
        }

        return item;
    }

    // Helper methods for Site Settings parts (JsonObject)
    private string? GetFieldValueFromPart(JsonObject? part, string fieldName)
    {
        if (part == null) return null;
        return part[fieldName]?["Text"]?.GetValue<string>();
    }

    private string? GetMediaPathFromPart(JsonObject? part, string fieldName)
    {
        if (part == null) return null;
        var paths = part[fieldName]?["Paths"];
        if (paths is JsonArray pathArray && pathArray.Count > 0)
        {
            var firstPath = pathArray[0]?.GetValue<string>();
            if (!string.IsNullOrEmpty(firstPath))
            {
                return "/media/" + firstPath;
            }
        }
        return null;
    }

    private int GetIntFieldValueFromPart(JsonObject? part, string fieldName)
    {
        if (part == null) return 0;
        var value = part[fieldName]?["Value"]?.GetValue<decimal?>() ?? 0;
        return (int)value;
    }

    private bool GetBoolFieldValueFromPart(JsonObject? part, string fieldName)
    {
        if (part == null) return false;
        return part[fieldName]?["Value"]?.GetValue<bool>() ?? false;
    }
}

public class NavigationViewModel
{
    public string? LogoUrl { get; set; }
    public string? LogoAlt { get; set; }
    public int LogoWidth { get; set; }
    public int LogoHeight { get; set; }
    public string? MobileLogoUrl { get; set; }
    public string? SiteName { get; set; }
    public string? CtaButtonText { get; set; }
    public string? CtaButtonUrl { get; set; }
    public bool ShowCtaButton { get; set; }
    public List<MenuItemViewModel> MenuItems { get; set; } = new();
}

public class MenuItemViewModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Url { get; set; } = "#";
    public bool OpenInNewTab { get; set; }
    public List<MenuItemViewModel> Children { get; set; } = new();
    public bool HasChildren => Children.Count > 0;
}
