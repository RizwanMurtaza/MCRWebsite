using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class HtmlBodyWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["HtmlBodyWidget"];

        var model = new HtmlBodyWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            Content = GetDynamicValue(GetDynamicValue(part, "Content"), "Html")?.ToString(),
            CssClass = GetDynamicValue(GetDynamicValue(part, "CssClass"), "Text")?.ToString()
        };

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
}

public class HtmlBodyWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? Content { get; set; }
    public string? CssClass { get; set; }
}
