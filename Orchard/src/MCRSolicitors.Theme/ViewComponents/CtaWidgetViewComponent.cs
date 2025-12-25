using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class CtaWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new CtaWidgetViewModel
        {
            Subtitle = GetTextField(widget, "CtaWidget", "Subtitle") ?? "",
            Title = GetTextField(widget, "CtaWidget", "Title") ?? "",
            Description = GetTextField(widget, "CtaWidget", "Description") ?? "",
            PrimaryButtonText = GetTextField(widget, "CtaWidget", "PrimaryButtonText") ?? "",
            PrimaryButtonUrl = GetTextField(widget, "CtaWidget", "PrimaryButtonUrl") ?? "",
            SecondaryButtonText = GetTextField(widget, "CtaWidget", "SecondaryButtonText") ?? "",
            SecondaryButtonUrl = GetTextField(widget, "CtaWidget", "SecondaryButtonUrl") ?? "",
            ShowContactInfo = GetBoolField(widget, "CtaWidget", "ShowContactInfo"),
            PhoneNumber = GetTextField(widget, "CtaWidget", "PhoneNumber") ?? "",
            Email = GetTextField(widget, "CtaWidget", "Email") ?? "",
            BackgroundColor = GetTextField(widget, "CtaWidget", "BackgroundColor") ?? "",
            BackgroundImage = GetMediaField(widget, "CtaWidget", "BackgroundImage")
        };

        return View(model);
    }
}

public class CtaWidgetViewModel
{
    public string Subtitle { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string PrimaryButtonText { get; set; } = "";
    public string PrimaryButtonUrl { get; set; } = "";
    public string SecondaryButtonText { get; set; } = "";
    public string SecondaryButtonUrl { get; set; } = "";
    public bool ShowContactInfo { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string BackgroundColor { get; set; } = "";
    public string? BackgroundImage { get; set; }
}
