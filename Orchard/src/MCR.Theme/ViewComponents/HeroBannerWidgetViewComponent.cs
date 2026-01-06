using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class HeroBannerWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["HeroBannerWidget"];
        var bagPartItems = contentItem?.Content["BagPart"]?["ContentItems"];

        // Parse slides and contact form from single BagPart
        dynamic? contactFormWidgetData = null;
        var slides = new List<dynamic>();

        if (bagPartItems != null)
        {
            foreach (var item in bagPartItems)
            {
                var contentType = GetDynamicValue(item, "ContentType")?.ToString();
                if (contentType == "HeroBannerSlide")
                {
                    slides.Add(item);
                }
                else if (contentType == "ContactFormWidget" && contactFormWidgetData == null)
                {
                    contactFormWidgetData = item;
                }
            }
        }

        var model = new HeroBannerWidgetViewModel
        {
            BackgroundImage = GetMediaPath(part?["BackgroundImage"]),
            BackgroundColor = part?["BackgroundColor"]?["Text"]?.ToString(),
            SlideInterval = GetIntValue(part?["SlideInterval"]?["Value"], 5000),
            AutoPlay = GetBoolValue(part?["AutoPlay"]?["Value"], true),
            ShowIndicators = GetBoolValue(part?["ShowIndicators"]?["Value"], true),
            ContactFormWidgetData = contactFormWidgetData,
            Slides = new List<HeroBannerSlideViewModel>()
        };

        // Parse slides
        foreach (var slideItem in slides)
        {
            var slidePart = GetDynamicValue(slideItem, "HeroBannerSlide");
            if (slidePart != null)
            {
                model.Slides.Add(new HeroBannerSlideViewModel
                {
                    Headline = GetDynamicValue(GetDynamicValue(slidePart, "Headline"), "Text")?.ToString(),
                    Subheadline = GetDynamicValue(GetDynamicValue(slidePart, "Subheadline"), "Text")?.ToString(),
                    PrimaryButtonText = GetDynamicValue(GetDynamicValue(slidePart, "PrimaryButtonText"), "Text")?.ToString(),
                    PrimaryButtonUrl = GetDynamicValue(GetDynamicValue(slidePart, "PrimaryButtonUrl"), "Text")?.ToString(),
                    SecondaryButtonText = GetDynamicValue(GetDynamicValue(slidePart, "SecondaryButtonText"), "Text")?.ToString(),
                    SecondaryButtonUrl = GetDynamicValue(GetDynamicValue(slidePart, "SecondaryButtonUrl"), "Text")?.ToString(),
                    DisplayOrder = GetIntFromDynamic(GetDynamicValue(GetDynamicValue(slidePart, "DisplayOrder"), "Value"), 1)
                });
            }
        }

        // Sort by display order
        model.Slides = model.Slides.OrderBy(s => s.DisplayOrder).ToList();

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

    private bool GetBoolValue(dynamic? value, bool defaultValue = false)
    {
        if (value == null) return defaultValue;
        try
        {
            if (value is bool boolVal) return boolVal;
            if (bool.TryParse(value.ToString(), out bool result)) return result;
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

    private int GetIntFromDynamic(dynamic? value, int defaultValue)
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

public class HeroBannerWidgetViewModel
{
    public string? BackgroundImage { get; set; }
    public string? BackgroundColor { get; set; }
    public int SlideInterval { get; set; } = 5000;
    public bool AutoPlay { get; set; } = true;
    public bool ShowIndicators { get; set; } = true;
    public dynamic? ContactFormWidgetData { get; set; }
    public List<HeroBannerSlideViewModel> Slides { get; set; } = new();
}

public class HeroBannerSlideViewModel
{
    public string? Headline { get; set; }
    public string? Subheadline { get; set; }
    public string? PrimaryButtonText { get; set; }
    public string? PrimaryButtonUrl { get; set; }
    public string? SecondaryButtonText { get; set; }
    public string? SecondaryButtonUrl { get; set; }
    public int DisplayOrder { get; set; } = 1;
}
