using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class StatsWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new StatsWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "StatsWidget", "SectionTitle") ?? "",
            BackgroundColor = GetTextField(widget, "StatsWidget", "BackgroundColor") ?? "",
            Items = new List<StatItemViewModel>()
        };

        // Get items from BagPart
        var bagItems = GetBagPartItems(widget, "BagPart");
        foreach (var item in bagItems)
        {
            model.Items.Add(new StatItemViewModel
            {
                Value = GetItemTextField(item, "StatItem", "Value") ?? "",
                Prefix = GetItemTextField(item, "StatItem", "Prefix") ?? "",
                Suffix = GetItemTextField(item, "StatItem", "Suffix") ?? "",
                Label = GetItemTextField(item, "StatItem", "Label") ?? "",
                IconClass = GetItemTextField(item, "StatItem", "IconClass") ?? ""
            });
        }

        return View(model);
    }
}

public class StatsWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string BackgroundColor { get; set; } = "";
    public List<StatItemViewModel> Items { get; set; } = new();
}

public class StatItemViewModel
{
    public string Value { get; set; } = "";
    public string Prefix { get; set; } = "";
    public string Suffix { get; set; } = "";
    public string Label { get; set; } = "";
    public string IconClass { get; set; } = "";
}
