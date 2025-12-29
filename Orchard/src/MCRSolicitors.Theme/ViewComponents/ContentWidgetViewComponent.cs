using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class ContentWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new ContentWidgetViewModel
        {
            SectionClass = GetTextField(widget, "ContentWidget", "SectionClass") ?? "",
            ContainerClass = GetTextField(widget, "ContentWidget", "ContainerClass") ?? "",
            // HtmlBodyPart stores HTML directly in .Html property (not nested like TextField)
            HtmlContent = GetHtmlBodyPart(widget) ?? ""
        };

        return View(model);
    }
}

public class ContentWidgetViewModel
{
    public string SectionClass { get; set; } = "";
    public string ContainerClass { get; set; } = "";
    public string HtmlContent { get; set; } = "";
}
