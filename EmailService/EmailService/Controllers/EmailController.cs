using EmailService.DTOs;
using EmailService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("contact-us")]
        public async Task<IActionResult> SendContactUsEmail([FromBody] ContactUsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _emailService.SendContactUsEmailAsync(request);

                if (result)
                {
                    return Ok(new { success = true, message = "Contact form submitted and queued for delivery" });
                }

                // More specific error message - check if it's credentials or something else
                return StatusCode(500, new { success = false, message = "Failed to queue email - check credentials and database connection" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact us email");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("enquiry")]
        public async Task<IActionResult> SendEnquiryEmail([FromBody] EnquiryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _emailService.SendEnquiryEmailAsync(request);

                if (result)
                {
                    return Ok(new { success = true, message = "Enquiry submitted and queued for delivery" });
                }

                // More specific error message - check if it's credentials or something else
                return StatusCode(500, new { success = false, message = "Failed to queue email - check credentials and database connection" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending enquiry email");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendCustomEmail([FromBody] CustomEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _emailService.SendEmailAsync(
                    request.AppId,
                    request.ToEmail,
                    request.Subject,
                    request.Body,
                    request.EmailType,
                    request.Metadata
                );

                if (result)
                {
                    return Ok(new { success = true, message = "Email queued for delivery successfully" });
                }

                return StatusCode(500, new { success = false, message = "Failed to queue email for delivery" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom email");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }

    public class CustomEmailRequest
    {
        public string AppId { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string EmailType { get; set; } = string.Empty;
        public string? Metadata { get; set; }
    }
}