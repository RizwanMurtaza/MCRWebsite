using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class AccordionWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["AccordionWidget"];
        var itemsBag = contentItem?.Content["BagPart"]?["ContentItems"];

        var model = new AccordionWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            SectionSubtitle = GetDynamicValue(GetDynamicValue(part, "SectionSubtitle"), "Text")?.ToString(),
            AllowMultipleOpen = GetBoolValue(GetDynamicValue(GetDynamicValue(part, "AllowMultipleOpen"), "Value"), false),
            CssClass = GetDynamicValue(GetDynamicValue(part, "CssClass"), "Text")?.ToString(),
            Items = new List<AccordionItemViewModel>()
        };

        if (itemsBag != null)
        {
            foreach (var item in itemsBag)
            {
                var itemPart = GetDynamicValue(item, "AccordionItem");
                if (itemPart != null)
                {
                    model.Items.Add(new AccordionItemViewModel
                    {
                        Title = GetDynamicValue(GetDynamicValue(itemPart, "Title"), "Text")?.ToString(),
                        Content = GetDynamicValue(GetDynamicValue(itemPart, "Content"), "Html")?.ToString(),
                        IsOpenByDefault = GetBoolValue(GetDynamicValue(GetDynamicValue(itemPart, "IsOpenByDefault"), "Value"), false),
                        DisplayOrder = GetIntValue(GetDynamicValue(GetDynamicValue(itemPart, "DisplayOrder"), "Value"), 1)
                    });
                }
            }
            model.Items = model.Items.OrderBy(i => i.DisplayOrder).ToList();
        }

        return View(model);
    }

    private static dynamic? GetDynamicValue(dynamic? obj, string name)
    {
        if (obj == null) return null;
        try
        {
            return obj[name];
        }
        catch
        {
            return null;
        }
    }

    private bool GetBoolValue(dynamic? value, bool defaultValue)
    {
        if (value == null) return defaultValue;
        try
        {
            if (value is bool boolVal) return boolVal;
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    private int GetIntValue(dynamic? value, int defaultValue)
    {
        if (value == null) return defaultValue;
        try
        {
            if (value is int intVal) return intVal;
            if (value is decimal decVal) return (int)decVal;
            if (value is double dblVal) return (int)dblVal;
            if (int.TryParse(value.ToString(), out int result)) return result;
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}

public class AccordionWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionSubtitle { get; set; }
    public bool AllowMultipleOpen { get; set; }
    public string? CssClass { get; set; }
    public List<AccordionItemViewModel> Items { get; set; } = new();
}

public class AccordionItemViewModel
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool IsOpenByDefault { get; set; }
    public int DisplayOrder { get; set; } = 1;
}
