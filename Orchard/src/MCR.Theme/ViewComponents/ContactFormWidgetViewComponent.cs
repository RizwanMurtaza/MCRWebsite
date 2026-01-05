using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class ContactFormWidgetViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ContentItem contentItem)
    {
        var part = contentItem?.Content["ContactFormWidget"];

        var model = new ContactFormWidgetViewModel
        {
            FormTitle = part?["FormTitle"]?["Text"]?.ToString(),
            FormSubtitle = part?["FormSubtitle"]?["Text"]?.ToString(),
            ShowDepartment = GetBoolValue(part?["ShowDepartment"]?["Value"]),
            DepartmentRequired = GetBoolValue(part?["DepartmentRequired"]?["Value"]),
            SubmitButtonText = part?["SubmitButtonText"]?["Text"]?.ToString(),
            SuccessMessage = part?["SuccessMessage"]?["Text"]?.ToString(),
            RecipientEmail = part?["RecipientEmail"]?["Text"]?.ToString()
        };

        // Set defaults if empty
        if (string.IsNullOrEmpty(model.SubmitButtonText))
            model.SubmitButtonText = "Send Message";
        if (string.IsNullOrEmpty(model.SuccessMessage))
            model.SuccessMessage = "Thank you for your message! We will get back to you within 24 hours.";

        return View(model);
    }

    private bool GetBoolValue(dynamic? value)
    {
        if (value == null) return false;
        try
        {
            return value?.ToObject<bool>() ?? false;
        }
        catch
        {
            return false;
        }
    }
}

public class ContactFormWidgetViewModel
{
    public string? FormTitle { get; set; }
    public string? FormSubtitle { get; set; }
    public bool ShowDepartment { get; set; }
    public bool DepartmentRequired { get; set; }
    public string SubmitButtonText { get; set; } = "Send Message";
    public string SuccessMessage { get; set; } = "Thank you for your message!";
    public string? RecipientEmail { get; set; }
}
