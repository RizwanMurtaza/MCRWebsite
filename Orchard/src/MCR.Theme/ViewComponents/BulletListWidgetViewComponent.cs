using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class BulletListWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["BulletListWidget"];
        var itemsBag = contentItem?.Content["BagPart"]?["ContentItems"];

        var model = new BulletListWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            SectionSubtitle = GetDynamicValue(GetDynamicValue(part, "SectionSubtitle"), "Text")?.ToString(),
            ListStyle = GetDynamicValue(GetDynamicValue(part, "ListStyle"), "Text")?.ToString() ?? "bullet",
            Columns = GetDynamicValue(GetDynamicValue(part, "Columns"), "Text")?.ToString() ?? "1",
            CssClass = GetDynamicValue(GetDynamicValue(part, "CssClass"), "Text")?.ToString(),
            Items = new List<BulletListItemViewModel>()
        };

        if (itemsBag != null)
        {
            foreach (var item in itemsBag)
            {
                var itemPart = GetDynamicValue(item, "BulletListItem");
                if (itemPart != null)
                {
                    model.Items.Add(new BulletListItemViewModel
                    {
                        Text = GetDynamicValue(GetDynamicValue(itemPart, "Text"), "Text")?.ToString(),
                        Url = GetDynamicValue(GetDynamicValue(itemPart, "Url"), "Text")?.ToString(),
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

public class BulletListWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionSubtitle { get; set; }
    public string ListStyle { get; set; } = "bullet";
    public string Columns { get; set; } = "1";
    public string? CssClass { get; set; }
    public List<BulletListItemViewModel> Items { get; set; } = new();
}

public class BulletListItemViewModel
{
    public string? Text { get; set; }
    public string? Url { get; set; }
    public int DisplayOrder { get; set; } = 1;
}
