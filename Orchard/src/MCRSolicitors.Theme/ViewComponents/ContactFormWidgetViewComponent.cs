using Microsoft.AspNetCore.Mvc;

namespace MCRSolicitors.Theme.ViewComponents;

public class ContactFormWidgetViewComponent : WidgetViewComponentBase
{
    public IViewComponentResult Invoke(dynamic widgetData)
    {
        var widget = UnwrapWidgetData(widgetData);
        var model = new ContactFormWidgetViewModel
        {
            FormTitle = GetTextField(widget, "ContactFormWidget", "FormTitle") ?? "Contact Us",
            FormSubtitle = GetTextField(widget, "ContactFormWidget", "FormSubtitle") ?? "",
            RecipientEmail = GetTextField(widget, "ContactFormWidget", "RecipientEmail") ?? "info@mcrsolicitors.co.uk",
            SuccessMessage = GetTextField(widget, "ContactFormWidget", "SuccessMessage") ?? "Thank you! Your message has been sent successfully. We will get back to you soon.",
            ShowDepartment = GetBoolField(widget, "ContactFormWidget", "ShowDepartment", true),
            ShowMap = GetBoolField(widget, "ContactFormWidget", "ShowMap", true),
            MapEmbedUrl = GetTextField(widget, "ContactFormWidget", "MapEmbedUrl") ?? "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1295.8028408732268!2d-2.1920947160132207!3d53.44141823133916!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x487bb3a2d4748a47%3A0xb9f03810ea6a232!2sMCR%20Solicitors!5e0!3m2!1sen!2suk!4v1657348500786!5m2!1sen!2suk",
            SectionClass = GetTextField(widget, "ContactFormWidget", "SectionClass") ?? ""
        };

        return View(model);
    }
}

public class ContactFormWidgetViewModel
{
    public string FormTitle { get; set; } = "Contact Us";
    public string FormSubtitle { get; set; } = "";
    public string RecipientEmail { get; set; } = "info@mcrsolicitors.co.uk";
    public string SuccessMessage { get; set; } = "Thank you! Your message has been sent successfully.";
    public bool ShowDepartment { get; set; } = true;
    public bool ShowMap { get; set; } = true;
    public string MapEmbedUrl { get; set; } = "";
    public string SectionClass { get; set; } = "";
}
