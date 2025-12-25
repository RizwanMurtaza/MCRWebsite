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
            HtmlContent = GetHtmlField(widget, "HtmlBodyPart", "Html") ?? ""
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
