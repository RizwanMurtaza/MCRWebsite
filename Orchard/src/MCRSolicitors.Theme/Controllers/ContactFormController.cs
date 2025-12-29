using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrchardCore.Email;
using System.Text;

namespace MCRSolicitors.Theme.Controllers;

[Route("contact")]
public class ContactFormController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactFormController> _logger;

    public ContactFormController(IEmailService emailService, ILogger<ContactFormController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit([FromForm] ContactFormModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill in all required fields." });
            }

            // Validate email format
            if (!IsValidEmail(model.Email))
            {
                return Json(new { success = false, message = "Please enter a valid email address." });
            }

            // Build email body
            var emailBody = BuildEmailBody(model);

            // Send email to the firm
            var result = await _emailService.SendAsync(new MailMessage
            {
                To = model.RecipientEmail ?? "info@mcrsolicitors.co.uk",
                Subject = $"New Contact Form Submission - {model.Department ?? "General Enquiry"}",
                Body = emailBody,
                IsHtmlBody = true,
                ReplyTo = model.Email
            });

            if (result.Succeeded)
            {
                _logger.LogInformation("Contact form submitted successfully from {Email}", model.Email);

                // Send acknowledgment to the user
                await SendAcknowledgmentEmail(model);

                return Json(new { success = true, message = "Thank you! Your message has been sent successfully. We will get back to you soon." });
            }
            else
            {
                _logger.LogWarning("Failed to send contact form email");
                return Json(new { success = false, message = "Failed to send your message. Please try again or call us directly." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing contact form submission");
            return Json(new { success = false, message = "An unexpected error occurred. Please try again or call us directly." });
        }
    }

    private string BuildEmailBody(ContactFormModel model)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head><style>");
        sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; }");
        sb.AppendLine(".header { background: #DE532A; color: white; padding: 20px; }");
        sb.AppendLine(".content { padding: 20px; }");
        sb.AppendLine(".field { margin-bottom: 15px; }");
        sb.AppendLine(".label { font-weight: bold; color: #333; }");
        sb.AppendLine(".value { margin-top: 5px; }");
        sb.AppendLine(".footer { background: #f5f5f5; padding: 15px; font-size: 12px; color: #666; }");
        sb.AppendLine("</style></head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class='header'><h2>New Contact Form Submission</h2></div>");
        sb.AppendLine("<div class='content'>");

        sb.AppendLine("<div class='field'><div class='label'>Name:</div>");
        sb.AppendLine($"<div class='value'>{System.Web.HttpUtility.HtmlEncode(model.Name)}</div></div>");

        sb.AppendLine("<div class='field'><div class='label'>Email:</div>");
        sb.AppendLine($"<div class='value'><a href='mailto:{System.Web.HttpUtility.HtmlEncode(model.Email)}'>{System.Web.HttpUtility.HtmlEncode(model.Email)}</a></div></div>");

        if (!string.IsNullOrEmpty(model.Phone))
        {
            sb.AppendLine("<div class='field'><div class='label'>Phone:</div>");
            sb.AppendLine($"<div class='value'><a href='tel:{System.Web.HttpUtility.HtmlEncode(model.Phone)}'>{System.Web.HttpUtility.HtmlEncode(model.Phone)}</a></div></div>");
        }

        if (!string.IsNullOrEmpty(model.Department))
        {
            sb.AppendLine("<div class='field'><div class='label'>Department:</div>");
            sb.AppendLine($"<div class='value'>{System.Web.HttpUtility.HtmlEncode(model.Department)}</div></div>");
        }

        sb.AppendLine("<div class='field'><div class='label'>Message:</div>");
        sb.AppendLine($"<div class='value'>{System.Web.HttpUtility.HtmlEncode(model.Message)?.Replace("\n", "<br/>")}</div></div>");

        sb.AppendLine("</div>");
        sb.AppendLine("<div class='footer'>");
        sb.AppendLine($"<p>This message was sent from the MCR Solicitors website contact form on {DateTime.Now:dddd, dd MMMM yyyy 'at' HH:mm}.</p>");
        sb.AppendLine("<p><strong>URGENT - Action Required:</strong> Please respond within 24 hours.</p>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body></html>");

        return sb.ToString();
    }

    private async Task SendAcknowledgmentEmail(ContactFormModel model)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head><style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            sb.AppendLine(".header { background: #DE532A; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine(".content { padding: 30px; }");
            sb.AppendLine(".footer { background: #f5f5f5; padding: 20px; font-size: 12px; color: #666; text-align: center; }");
            sb.AppendLine("</style></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='header'><h2>Thank You for Contacting MCR Solicitors</h2></div>");
            sb.AppendLine("<div class='content'>");
            sb.AppendLine($"<p>Dear {System.Web.HttpUtility.HtmlEncode(model.Name)},</p>");
            sb.AppendLine("<p>Thank you for getting in touch with MCR Solicitors. We have received your enquiry and a member of our team will respond to you within 24 hours.</p>");
            sb.AppendLine("<p><strong>Your submission details:</strong></p>");
            sb.AppendLine("<ul>");
            if (!string.IsNullOrEmpty(model.Department))
            {
                sb.AppendLine($"<li><strong>Department:</strong> {System.Web.HttpUtility.HtmlEncode(model.Department)}</li>");
            }
            sb.AppendLine($"<li><strong>Message:</strong> {System.Web.HttpUtility.HtmlEncode(model.Message)?.Substring(0, Math.Min(model.Message?.Length ?? 0, 100))}...</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("<p>If your matter is urgent, please call us directly on <a href='tel:+441614661280'>0161 466 1280</a>.</p>");
            sb.AppendLine("<p>Kind regards,<br/>MCR Solicitors Team</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>MCR LAW ASSOCIATES LIMITED trading as MCR SOLICITORS</p>");
            sb.AppendLine("<p>First Floor, 1024 Stockport Road, Manchester, M19 3WX</p>");
            sb.AppendLine("<p>Authorised and regulated by the Solicitors Regulation Authority - SRA ID: 648878</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body></html>");

            await _emailService.SendAsync(new MailMessage
            {
                To = model.Email,
                Subject = "Thank you for contacting MCR Solicitors",
                Body = sb.ToString(),
                IsHtmlBody = true
            });
        }
        catch (Exception ex)
        {
            // Log but don't fail the main submission
            _logger.LogWarning(ex, "Failed to send acknowledgment email to {Email}", model.Email);
        }
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class ContactFormModel
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string Message { get; set; } = "";
    public string? RecipientEmail { get; set; }
}
