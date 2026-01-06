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
            ShowGoogleReviews = GetBoolValue(GetDynamicValue(GetDynamicValue(part, "ShowGoogleReviews"), "Value")),
            GoogleReviewsUrl = GetDynamicValue(GetDynamicValue(part, "GoogleReviewsUrl"), "Text")?.ToString(),
            GoogleReviewCount = GetIntValue(GetDynamicValue(GetDynamicValue(part, "GoogleReviewCount"), "Value"), 0),
            GoogleAverageRating = GetDecimalValue(GetDynamicValue(GetDynamicValue(part, "GoogleAverageRating"), "Value"), 0),
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

    private decimal GetDecimalValue(dynamic? value, decimal defaultValue)
    {
        if (value == null) return defaultValue;
        try
        {
            if (value is decimal decVal) return decVal;
            if (value is double dblVal) return (decimal)dblVal;
            if (value is int intVal) return intVal;
            if (decimal.TryParse(value.ToString(), out decimal result)) return result;
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    private bool GetBoolValue(dynamic? value)
    {
        if (value == null) return false;
        try
        {
            if (value is bool boolVal) return boolVal;
            if (bool.TryParse(value.ToString(), out bool result)) return result;
            return false;
        }
        catch
        {
            return false;
        }
    }
}

public class TestimonialsSectionWidgetViewModel
{
    public string? SectionTitle { get; set; }
    public string? SectionSubtitle { get; set; }
    public string? CssClass { get; set; }
    public bool ShowGoogleReviews { get; set; }
    public string? GoogleReviewsUrl { get; set; }
    public int GoogleReviewCount { get; set; }
    public decimal GoogleAverageRating { get; set; }
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
