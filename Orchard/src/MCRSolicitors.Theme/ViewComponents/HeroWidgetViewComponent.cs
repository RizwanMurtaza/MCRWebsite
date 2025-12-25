using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class HeroWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);

        var model = new HeroWidgetViewModel
        {
            Title = GetTextField(widget, "HeroWidget", "Title") ?? "",
            Subtitle = GetTextField(widget, "HeroWidget", "Subtitle") ?? "",
            BackgroundImage = GetMediaField(widget, "HeroWidget", "BackgroundImage"),
            PrimaryButtonText = GetTextField(widget, "HeroWidget", "PrimaryButtonText") ?? "",
            PrimaryButtonUrl = GetTextField(widget, "HeroWidget", "PrimaryButtonUrl") ?? "",
            SecondaryButtonText = GetTextField(widget, "HeroWidget", "SecondaryButtonText") ?? "",
            SecondaryButtonUrl = GetTextField(widget, "HeroWidget", "SecondaryButtonUrl") ?? "",
            ShowContactForm = GetBoolField(widget, "HeroWidget", "ShowContactForm"),
            FormTitle = GetTextField(widget, "HeroWidget", "FormTitle") ?? "Get in Touch"
        };

        return View(model);
    }
}

public class HeroWidgetViewModel
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string? BackgroundImage { get; set; }
    public string PrimaryButtonText { get; set; } = "";
    public string PrimaryButtonUrl { get; set; } = "";
    public string SecondaryButtonText { get; set; } = "";
    public string SecondaryButtonUrl { get; set; } = "";
    public bool ShowContactForm { get; set; }
    public string FormTitle { get; set; } = "Get in Touch";
}
