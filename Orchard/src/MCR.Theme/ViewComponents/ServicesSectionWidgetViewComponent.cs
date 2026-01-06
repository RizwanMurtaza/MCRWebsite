using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class ServicesSectionWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["ServicesSectionWidget"];
        var serviceCardsBag = contentItem?.Content["BagPart"]?["ContentItems"];

        var model = new ServicesSectionWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            SectionSubtitle = GetDynamicValue(GetDynamicValue(part, "SectionSubtitle"), "Text")?.ToString(),
            CssClass = GetDynamicValue(GetDynamicValue(part, "CssClass"), "Text")?.ToString(),
            ServiceCards = new List<ServiceCardViewModel>()
        };

        if (serviceCardsBag != null)
        {
            foreach (var cardItem in serviceCardsBag)
            {
                var cardPart = GetDynamicValue(cardItem, "ServiceCard");
                if (cardPart != null)
                {
                    model.ServiceCards.Add(new ServiceCardViewModel
                    {
                        Title = GetDynamicValue(GetDynamicValue(cardPart, "Title"), "Text")?.ToString(),
                        Description = GetDynamicValue(GetDynamicValue(cardPart, "Description"), "Text")?.ToString(),
                        IconClass = GetDynamicValue(GetDynamicValue(cardPart, "IconClass"), "Text")?.ToString(),
                        Image = GetMediaPath(GetDynamicValue(cardPart, "Image")),
                        LinkUrl = GetDynamicValue(GetDynamicValue(cardPart, "LinkUrl"), "Text")?.ToString(),
                        LinkText = GetDynamicValue(GetDynamicValue(cardPart, "LinkText"), "Text")?.ToString(),
                        DisplayOrder = GetIntValue(GetDynamicValue(GetDynamicValue(cardPart, "DisplayOrder"), "Value"), 1)
                    });
                }
            }
            model.ServiceCards = model.ServiceCards.OrderBy(s => s.DisplayOrder).ToList();
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

public class ServicesSectionWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionSubtitle { get; set; }
    public string? CssClass { get; set; }
    public List<ServiceCardViewModel> ServiceCards { get; set; } = new();
}

public class ServiceCardViewModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? Image { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public int DisplayOrder { get; set; } = 1;
}
