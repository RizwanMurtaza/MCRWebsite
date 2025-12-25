using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class FeaturesWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new FeaturesWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "FeaturesWidget", "SectionTitle") ?? "",
            BackgroundColor = GetTextField(widget, "FeaturesWidget", "BackgroundColor") ?? "",
            Items = new List<FeatureItemViewModel>()
        };

        // Get items from BagPart
        var bagItems = GetBagPartItems(widget, "BagPart");
        foreach (var item in bagItems)
        {
            model.Items.Add(new FeatureItemViewModel
            {
                Title = GetItemTextField(item, "FeatureItem", "Title") ?? "",
                Description = GetItemTextField(item, "FeatureItem", "Description") ?? "",
                IconClass = GetItemTextField(item, "FeatureItem", "IconClass") ?? ""
            });
        }

        return View(model);
    }
}

public class FeaturesWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string BackgroundColor { get; set; } = "";
    public List<FeatureItemViewModel> Items { get; set; } = new();
}

public class FeatureItemViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string IconClass { get; set; } = "";
}
