using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class TestimonialsSectionWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["TestimonialsSectionWidget"];
        var testimonialsBag = contentItem?.Content["BagPart"]?["ContentItems"];

        var model = new TestimonialsSectionWidgetViewModel
        {
            SectionTitle = GetDynamicValue(GetDynamicValue(part, "SectionTitle"), "Text")?.ToString(),
            SectionSubtitle = GetDynamicValue(GetDynamicValue(part, "SectionSubtitle"), "Text")?.ToString(),
            CssClass = GetDynamicValue(GetDynamicValue(part, "CssClass"), "Text")?.ToString(),
            Testimonials = new List<TestimonialViewModel>()
        };

        if (testimonialsBag != null)
        {
            foreach (var testimonialItem in testimonialsBag)
            {
                var testimonialPart = GetDynamicValue(testimonialItem, "Testimonial");
                if (testimonialPart != null)
                {
                    model.Testimonials.Add(new TestimonialViewModel
                    {
                        Quote = GetDynamicValue(GetDynamicValue(testimonialPart, "Quote"), "Text")?.ToString(),
                        AuthorName = GetDynamicValue(GetDynamicValue(testimonialPart, "AuthorName"), "Text")?.ToString(),
                        AuthorTitle = GetDynamicValue(GetDynamicValue(testimonialPart, "AuthorTitle"), "Text")?.ToString(),
                        AuthorImage = GetMediaPath(GetDynamicValue(testimonialPart, "AuthorImage")),
                        Rating = GetIntValue(GetDynamicValue(GetDynamicValue(testimonialPart, "Rating"), "Value"), 5),
                        DisplayOrder = GetIntValue(GetDynamicValue(GetDynamicValue(testimonialPart, "DisplayOrder"), "Value"), 1)
                    });
                }
            }
            model.Testimonials = model.Testimonials.OrderBy(t => t.DisplayOrder).ToList();
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

public class TestimonialsSectionWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionSubtitle { get; set; }
    public string? CssClass { get; set; }
    public List<TestimonialViewModel> Testimonials { get; set; } = new();
}

public class TestimonialViewModel
{
    public string? Quote { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorTitle { get; set; }
    public string? AuthorImage { get; set; }
    public int Rating { get; set; } = 5;
    public int DisplayOrder { get; set; } = 1;
}
