using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class PageHeroWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new PageHeroWidgetViewModel
        {
            Title = GetTextField(widget, "PageHeroWidget", "Title") ?? "",
            Subtitle = GetTextField(widget, "PageHeroWidget", "Subtitle") ?? "",
            BackgroundImage = GetMediaField(widget, "PageHeroWidget", "BackgroundImage"),
            ShowBreadcrumb = GetBoolField(widget, "PageHeroWidget", "ShowBreadcrumb", true)
        };

        return View(model);
    }
}

public class PageHeroWidgetViewModel
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string? BackgroundImage { get; set; }
    public bool ShowBreadcrumb { get; set; } = true;
}
