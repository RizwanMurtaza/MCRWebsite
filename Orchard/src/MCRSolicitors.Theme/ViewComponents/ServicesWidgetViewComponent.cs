using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class ServicesWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new ServicesWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "ServicesWidget", "SectionTitle") ?? "",
            SectionSubtitle = GetTextField(widget, "ServicesWidget", "SectionSubtitle") ?? "",
            Items = new List<ServiceItemViewModel>()
        };

        // Get items from BagPart
        var bagItems = GetBagPartItems(widget, "BagPart");
        foreach (var item in bagItems)
        {
            model.Items.Add(new ServiceItemViewModel
            {
                Title = GetItemTextField(item, "ServiceItem", "Title") ?? "",
                Description = GetItemTextField(item, "ServiceItem", "Description") ?? "",
                IconClass = GetItemTextField(item, "ServiceItem", "IconClass") ?? "",
                LinkUrl = GetItemTextField(item, "ServiceItem", "LinkUrl") ?? "",
                LinkText = GetItemTextField(item, "ServiceItem", "LinkText") ?? "Learn More",
                Image = GetItemMediaField(item, "ServiceItem", "Image") ?? ""
            });
        }

        return View(model);
    }
}

public class ServicesWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string SectionSubtitle { get; set; } = "";
    public List<ServiceItemViewModel> Items { get; set; } = new();
}

public class ServiceItemViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string IconClass { get; set; } = "";
    public string LinkUrl { get; set; } = "";
    public string LinkText { get; set; } = "Learn More";
    public string Image { get; set; } = "";
}
