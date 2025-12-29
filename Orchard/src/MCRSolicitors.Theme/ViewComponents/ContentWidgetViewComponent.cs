using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class ContentWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);

        // Try multiple sources for HTML content:
        // 1. HtmlBodyPart.Html (standard Orchard format)
        // 2. ContentWidget.Content.Html (recipe import format)
        var htmlContent = GetHtmlBodyPart(widget);
        if (string.IsNullOrEmpty(htmlContent))
        {
            htmlContent = GetHtmlField(widget, "ContentWidget", "Content") ?? "";
        }

        var model = new ContentWidgetViewModel
        {
            SectionTitle = GetTextField(widget, "ContentWidget", "SectionTitle") ?? "",
            SectionClass = GetTextField(widget, "ContentWidget", "SectionClass") ?? "",
            ContainerClass = GetTextField(widget, "ContentWidget", "ContainerClass") ?? "",
            HtmlContent = htmlContent
        };

        return View(model);
    }
}

public class ContentWidgetViewModel
{
    public string SectionTitle { get; set; } = "";
    public string SectionClass { get; set; } = "";
    public string ContainerClass { get; set; } = "";
    public string HtmlContent { get; set; } = "";
}
