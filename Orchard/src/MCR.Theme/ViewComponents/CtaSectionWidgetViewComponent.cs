using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class CtaSectionWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["CtaSectionWidget"];

        var model = new CtaSectionWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            SectionText = GetDynamicValue(GetDynamicValue(part, "SectionText"), "Text")?.ToString(),
            ButtonText = GetDynamicValue(GetDynamicValue(part, "ButtonText"), "Text")?.ToString(),
            ButtonUrl = GetDynamicValue(GetDynamicValue(part, "ButtonUrl"), "Text")?.ToString(),
            BackgroundImage = GetMediaPath(GetDynamicValue(part, "BackgroundImage")),
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

    private string? GetMediaPath(dynamic? mediaField)
    {
        if (mediaField == null) return null;
        try
        {
            var paths = GetDynamicValue(mediaField, "Paths");
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        return "/media/" + path.ToString();
                    }
                }
            }
        }
        catch { }
        return null;
    }
}

public class CtaSectionWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionText { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public string? BackgroundImage { get; set; }
    public string? CssClass { get; set; }
}
