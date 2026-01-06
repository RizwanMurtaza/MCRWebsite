using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Settings;
using YesSql;

namespace MCR.Theme.ViewComponents;

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
        // Query footer columns (these are regular content items)
        var columns = await _session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "FooterColumn" && x.Published)
            .ListAsync();

        // Get FooterSettings from Site Settings (CustomSettings are stored in Properties)
        var site = await _siteService.GetSiteSettingsAsync();
        var settingsPart = site.Properties["FooterSettings"]?["FooterSettings"]?.AsObject();

        var model = new FooterViewModel
        {
            // NO HARDCODED FALLBACKS - all from CMS Site Settings
            CompanyName = GetFieldValue(settingsPart, "CompanyName"),
            CompanyDescription = GetFieldValue(settingsPart, "CompanyDescription"),
            ShowCompanyDescription = GetBoolFieldValue(settingsPart, "ShowCompanyDescription"),
            CopyrightText = GetFieldValue(settingsPart, "CopyrightText"),
            CopyrightYear = GetIntFieldValue(settingsPart, "CopyrightYear"),
            Columns = columns.Select(c => MapColumn(c)).OrderBy(c => c.DisplayOrder).ToList()
        };

        return View(model);
    }

    private FooterColumnViewModel MapColumn(ContentItem item)
    {
        var column = new FooterColumnViewModel
        {
            Title = GetFieldValueFromItem(item, "FooterColumn", "Title"),
            DisplayOrder = GetIntFieldValueFromItem(item, "FooterColumn", "DisplayOrder"),
            Items = new List<FooterItemViewModel>()
        };

        // Get items from BagPart
        var bagPart = item.Content["BagPart"]?["ContentItems"];
        if (bagPart != null)
        {
            foreach (var bagItem in bagPart)
            {
                column.Items.Add(new FooterItemViewModel
                {
                    ItemType = bagItem["FooterItem"]?["ItemType"]?["Text"]?.ToString() ?? "text",
                    Text = bagItem["FooterItem"]?["Text"]?["Text"]?.ToString(),
                    Url = bagItem["FooterItem"]?["Url"]?["Text"]?.ToString(),
                    ImageUrl = GetMediaPathFromDynamic(bagItem["FooterItem"]?["Image"]),
                    ImageWidth = GetIntFromDynamic(bagItem["FooterItem"]?["ImageWidth"]?["Value"]),
                    ImageHeight = GetIntFromDynamic(bagItem["FooterItem"]?["ImageHeight"]?["Value"]),
                    ImageAlt = bagItem["FooterItem"]?["ImageAlt"]?["Text"]?.ToString(),
                    DisplayOrder = GetIntFromDynamic(bagItem["FooterItem"]?["DisplayOrder"]?["Value"]),
                    OpenInNewTab = GetBoolFromDynamic(bagItem["FooterItem"]?["OpenInNewTab"]?["Value"])
                });
            }
            column.Items = column.Items.OrderBy(i => i.DisplayOrder).ToList();
        }

        return column;
    }

    // Helper for Site Settings parts (JsonObject)
    private string? GetFieldValue(JsonObject? part, string fieldName)
    {
        if (part == null) return null;
        return part[fieldName]?["Text"]?.GetValue<string>();
    }

    private int GetIntFieldValue(JsonObject? part, string fieldName)
    {
        if (part == null) return 0;
        var value = part[fieldName]?["Value"]?.GetValue<decimal?>() ?? 0;
        return (int)value;
    }

    private bool GetBoolFieldValue(JsonObject? part, string fieldName)
    {
        if (part == null) return false;
        return part[fieldName]?["Value"]?.GetValue<bool>() ?? false;
    }

    // Helper for ContentItem parts
    private string? GetFieldValueFromItem(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return null;
        var part = item.Content[partName];
        if (part == null) return null;
        var field = part[fieldName];
        return field?["Text"]?.ToString();
    }

    private int GetIntFieldValueFromItem(ContentItem? item, string partName, string fieldName)
    {
        if (item == null) return 0;
        var part = item.Content[partName];
        if (part == null) return 0;
        var field = part[fieldName];
        var value = field?["Value"]?.ToObject<decimal?>() ?? 0;
        return (int)value;
    }

    private string? GetMediaPathFromDynamic(dynamic? field)
    {
        if (field == null) return null;
        var paths = field["Paths"];
        if (paths != null)
        {
            try
            {
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        return "/media/" + path.ToString();
                    }
                }
            }
            catch
            {
                // Not iterable
            }
        }
        return null;
    }

    private int GetIntFromDynamic(dynamic? value)
    {
        if (value == null) return 0;
        try
        {
            return (int)(value?.ToObject<decimal?>() ?? 0);
        }
        catch
        {
            return 0;
        }
    }

    private bool GetBoolFromDynamic(dynamic? value)
    {
        if (value == null) return false;
        try
        {
            return value?.ToObject<bool>() ?? false;
        }
        catch
        {
            return false;
        }
    }
}

public class FooterViewModel
{
    public string? CompanyName { get; set; }
    public string? CompanyDescription { get; set; }
    public bool ShowCompanyDescription { get; set; }
    public string? CopyrightText { get; set; }
    public int CopyrightYear { get; set; }
    public List<FooterColumnViewModel> Columns { get; set; } = new();
}

public class FooterColumnViewModel
{
    public string? Title { get; set; }
    public int DisplayOrder { get; set; }
    public List<FooterItemViewModel> Items { get; set; } = new();
}

public class FooterItemViewModel
{
    public string ItemType { get; set; } = "text";
    public string? Text { get; set; }
    public string? Url { get; set; }
    public string? ImageUrl { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public string? ImageAlt { get; set; }
    public int DisplayOrder { get; set; }
    public bool OpenInNewTab { get; set; }
}
