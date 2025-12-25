using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class TestimonialsWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new TestimonialsWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "TestimonialsWidget", "SectionTitle") ?? "",
            SectionSubtitle = GetTextField(widget, "TestimonialsWidget", "SectionSubtitle") ?? "",
            Items = new List<TestimonialItemViewModel>()
        };

        // Get items from BagPart
        var bagItems = GetBagPartItems(widget, "BagPart");
        foreach (var item in bagItems)
        {
            model.Items.Add(new TestimonialItemViewModel
            {
                Quote = GetItemTextField(item, "TestimonialItem", "Quote") ?? "",
                AuthorName = GetItemTextField(item, "TestimonialItem", "AuthorName") ?? "",
                AuthorRole = GetItemTextField(item, "TestimonialItem", "AuthorRole") ?? "",
                AuthorImage = GetItemMediaField(item, "TestimonialItem", "AuthorImage") ?? "",
                Rating = GetItemTextField(item, "TestimonialItem", "Rating") ?? "",
                Source = GetItemTextField(item, "TestimonialItem", "Source") ?? ""
            });
        }

        return View(model);
    }
}

public class TestimonialsWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string SectionSubtitle { get; set; } = "";
    public List<TestimonialItemViewModel> Items { get; set; } = new();
}

public class TestimonialItemViewModel
{
    public string Quote { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public string AuthorRole { get; set; } = "";
    public string AuthorImage { get; set; } = "";
    public string Rating { get; set; } = "";
    public string Source { get; set; } = "";
}
