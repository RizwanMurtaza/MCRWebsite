using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;

namespace MCR.Theme.ViewComponents;

public class ContactFormWidgetViewComponent : ViewComponent
{
    // Support both ContentItem (from FlowPart) and dynamic widgetData (from HeroBanner BagPart)
    public IViewComponentResult Invoke(ContentItem? contentItem = null, dynamic? widgetData = null, string? cssClass = null)
    {
        dynamic? part = null;

        // Try to get the part from either source
        if (contentItem != null)
        {
            part = contentItem.Content["ContactFormWidget"];
        }
        else if (widgetData != null)
        {
            part = GetDynamicValue(widgetData, "ContactFormWidget");
        }

        var model = new ContactFormWidgetViewModel
        {
            FormTitle = GetTextField(part, "FormTitle"),
            FormSubtitle = GetTextField(part, "FormSubtitle"),
            ShowDepartment = GetBoolField(part, "ShowDepartment"),
            DepartmentRequired = GetBoolField(part, "DepartmentRequired"),
            SubmitButtonText = GetTextField(part, "SubmitButtonText"),
            SuccessMessage = GetTextField(part, "SuccessMessage"),
            RecipientEmail = GetTextField(part, "RecipientEmail"),
            WorkflowUrl = GetTextField(part, "WorkflowUrl"),
            CssClass = cssClass
        };

        // Set defaults if empty
        if (string.IsNullOrEmpty(model.SubmitButtonText))
            model.SubmitButtonText = "Send Message";
        if (string.IsNullOrEmpty(model.SuccessMessage))
            model.SuccessMessage = "Thank you for your message! We will get back to you within 24 hours.";

        return View(model);
    }

    private static dynamic? GetDynamicValue(dynamic? obj, string name)
    {
        if (obj == null) return null;
        try
        {
            return obj[name];
        }
        catch
        {
            return null;
        }
    }

    private static string? GetTextField(dynamic? part, string fieldName)
    {
        if (part == null) return null;
        try
        {
            var field = GetDynamicValue(part, fieldName);
            if (field == null) return null;
            var text = GetDynamicValue(field, "Text");
            return text?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static bool GetBoolField(dynamic? part, string fieldName)
    {
        if (part == null) return false;
        try
        {
            var field = GetDynamicValue(part, fieldName);
            if (field == null) return false;
            var value = GetDynamicValue(field, "Value");
            if (value == null) return false;

            if (value is bool boolVal) return boolVal;
            if (bool.TryParse(value.ToString(), out bool result)) return result;
            return false;
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
    public string? WorkflowUrl { get; set; }
    public string? CssClass { get; set; }
}
