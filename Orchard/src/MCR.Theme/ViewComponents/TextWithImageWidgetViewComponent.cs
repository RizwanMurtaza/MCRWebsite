using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class TextWithImageWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["TextWithImageWidget"];

        var model = new TextWithImageWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            Content = GetDynamicValue(GetDynamicValue(part, "Content"), "Html")?.ToString(),
            ImageUrl = GetMediaPath(GetDynamicValue(part, "Image")),
            ImageAlt = GetDynamicValue(GetDynamicValue(part, "ImageAlt"), "Text")?.ToString(),
            ImagePosition = GetDynamicValue(GetDynamicValue(part, "ImagePosition"), "Text")?.ToString() ?? "right",
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

public class TextWithImageWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public string ImagePosition { get; set; } = "right";
    public string? CssClass { get; set; }
}
