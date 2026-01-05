using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class HeroBannerWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["HeroBannerWidget"];
        var slidesBag = contentItem?.Content["Slides"]?["ContentItems"];

        var model = new HeroBannerWidgetViewModel
        {
            BackgroundImage = GetMediaPath(part?["BackgroundImage"]),
            BackgroundColor = part?["BackgroundColor"]?["Text"]?.ToString(),
            SlideInterval = GetIntValue(part?["SlideInterval"]?["Value"], 5000),
            ShowContactForm = GetBoolValue(part?["ShowContactForm"]?["Value"]),
            ContactFormTitle = part?["ContactFormTitle"]?["Text"]?.ToString(),
            ContactFormRecipientEmail = part?["ContactFormRecipientEmail"]?["Text"]?.ToString(),
            ShowDepartmentInForm = GetBoolValue(part?["ShowDepartmentInForm"]?["Value"]),
            AutoPlay = GetBoolValue(part?["AutoPlay"]?["Value"], true),
            ShowIndicators = GetBoolValue(part?["ShowIndicators"]?["Value"], true),
            Slides = new List<HeroBannerSlideViewModel>()
        };

        // Parse slides from BagPart
        if (slidesBag != null)
        {
            foreach (var slideItem in slidesBag)
            {
                var slidePart = slideItem["HeroBannerSlide"];
                if (slidePart != null)
                {
                    model.Slides.Add(new HeroBannerSlideViewModel
                    {
                        Headline = slidePart["Headline"]?["Text"]?.ToString(),
                        Subheadline = slidePart["Subheadline"]?["Text"]?.ToString(),
                        PrimaryButtonText = slidePart["PrimaryButtonText"]?["Text"]?.ToString(),
                        PrimaryButtonUrl = slidePart["PrimaryButtonUrl"]?["Text"]?.ToString(),
                        SecondaryButtonText = slidePart["SecondaryButtonText"]?["Text"]?.ToString(),
                        SecondaryButtonUrl = slidePart["SecondaryButtonUrl"]?["Text"]?.ToString(),
                        DisplayOrder = GetIntValue(slidePart["DisplayOrder"]?["Value"], 1)
                    });
                }
            }

            // Sort by display order
            model.Slides = model.Slides.OrderBy(s => s.DisplayOrder).ToList();
        }

        return View(model);
    }

    private string? GetMediaPath(dynamic? mediaField)
    {
        if (mediaField == null) return null;
        var paths = mediaField["Paths"];
        if (paths != null && paths.HasValues)
        {
            return "/media/" + paths[0]?.ToString();
        }
        return null;
    }

    private bool GetBoolValue(dynamic? value, bool defaultValue = false)
    {
        if (value == null) return defaultValue;
        try
        {
            return value?.ToObject<bool>() ?? defaultValue;
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
            var decimalValue = value?.ToObject<decimal?>();
            return decimalValue.HasValue ? (int)decimalValue.Value : defaultValue;
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
    public bool ShowContactForm { get; set; }
    public string? ContactFormTitle { get; set; }
    public string? ContactFormRecipientEmail { get; set; }
    public bool ShowDepartmentInForm { get; set; }
    public bool AutoPlay { get; set; } = true;
    public bool ShowIndicators { get; set; } = true;
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
