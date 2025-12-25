using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class FaqWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new FaqWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "FaqWidget", "SectionTitle") ?? "",
            SectionSubtitle = GetTextField(widget, "FaqWidget", "SectionSubtitle") ?? "",
            Items = new List<FaqItemViewModel>()
        };

        // Get items from BagPart
        var bagItems = GetBagPartItems(widget, "BagPart");
        int index = 0;
        foreach (var item in bagItems)
        {
            model.Items.Add(new FaqItemViewModel
            {
                ItemId = GetItemId(item) ?? $"faq-{index}",
                Question = GetItemTextField(item, "FaqItem", "Question") ?? "",
                Answer = GetItemHtmlField(item, "FaqItem", "Answer") ?? ""
            });
            index++;
        }

        return View(model);
    }
}

public class FaqWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string SectionSubtitle { get; set; } = "";
    public List<FaqItemViewModel> Items { get; set; } = new();
}

public class FaqItemViewModel
{
    public string ItemId { get; set; } = "";
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
}
