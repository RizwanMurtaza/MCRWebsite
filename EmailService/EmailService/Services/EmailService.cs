using EmailService.Data;
using EmailService.DTOs;
using EmailService.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace EmailService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailDbContext _context;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(EmailDbContext context, ILogger<EmailService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendContactUsEmailAsync(ContactUsRequest request)
        {
            var subject = $"Contact Us: {request.Subject}";
            var body = GenerateContactUsEmailBody(request);
            var metadata = JsonSerializer.Serialize(new { request.Phone, Type = "ContactUs" });

            return await SendEmailAsync(request.AppId, request.Email, subject, body, "ContactUs", metadata);
        }

        public async Task<bool> SendEnquiryEmailAsync(EnquiryRequest request)
        {
            var subject = $"Service Enquiry: {request.ServiceType}";
            var body = GenerateEnquiryEmailBody(request);
            var metadata = JsonSerializer.Serialize(new
            {
                request.Phone,
                request.ServiceType,
                request.PreferredContactMethod,
                request.PreferredContactTime,
                Type = "Enquiry"
            });

            return await SendEmailAsync(request.AppId, request.Email, subject, body, "Enquiry", metadata);
        }

        public async Task<bool> SendEmailAsync(string appId, string toEmail, string subject, string body, string emailType, string? metadata = null)
        {
            try
            {
                // Check if credentials exist and are active
                var credentials = await _context.AppCredentials
                    .FirstOrDefaultAsync(ac => ac.AppId == appId && ac.IsActive);

                if (credentials == null)
                {
                    _logger.LogError($"App credentials not found or inactive for appId: {appId}");

                    // Log the failure
                    var failedLog = new EmailLog
                    {
                        AppId = appId,
                        ToEmail = toEmail,
                        Subject = subject,
                        Body = body,
                        EmailType = emailType,
                        Status = "Failed",
                        ErrorMessage = "App credentials not found or inactive",
                        Metadata = metadata
                    };
                    await _context.EmailLogs.AddAsync(failedLog);
                    await _context.SaveChangesAsync();
                    return false;
                }

                // Get HTTP context info
                var httpContext = _httpContextAccessor.HttpContext;

                // Add to email queue for background processing
                var emailQueueItem = new EmailQueueItem
                {
                    AppId = appId,
                    ToEmail = toEmail,
                    ToName = "",
                    CcEmail = "",
                    BccEmail = "",
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    Status = "Pending",
                    Priority = "Normal",
                    EmailType = emailType,
                    CreatedAt = DateTime.UtcNow,
                    ClientIp = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "",
                    UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "",
                    RequestId = Guid.NewGuid().ToString(),
                    AttachmentPaths = "",
                    LastError = "",
                    FailureReason = ""
                };

                await _context.EmailQueueItems.AddAsync(emailQueueItem);

                // Also create an email log entry for tracking
                var emailLog = new EmailLog
                {
                    AppId = appId,
                    ToEmail = toEmail,
                    Subject = subject,
                    Body = body,
                    EmailType = emailType,
                    Status = "Pending",
                    Metadata = metadata,
                    FromEmail = credentials.FromEmail
                };

                await _context.EmailLogs.AddAsync(emailLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email queued successfully for {toEmail} (AppId: {appId}, QueueId: {emailQueueItem.Id})");

                // Log to process log
                var processLog = new EmailProcessLog
                {
                    EmailQueueItemId = emailQueueItem.Id,
                    AppId = appId,
                    LogLevel = "Info",
                    Event = "EmailQueued",
                    Message = $"Email queued for {toEmail} with subject: {subject}",
                    ProcessName = "EmailService",
                    CreatedAt = DateTime.UtcNow
                };
                await _context.EmailProcessLogs.AddAsync(processLog);
                await _context.SaveChangesAsync();

                return true; // Return true as email is successfully queued
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to queue email for {toEmail} (AppId: {appId})");

                // Log the failure
                var failedLog = new EmailLog
                {
                    AppId = appId,
                    ToEmail = toEmail,
                    Subject = subject,
                    Body = body,
                    EmailType = emailType,
                    Status = "Failed",
                    ErrorMessage = $"Failed to queue email: {ex.Message}",
                    Metadata = metadata
                };
                await _context.EmailLogs.AddAsync(failedLog);
                await _context.SaveChangesAsync();

                return false;
            }
        }


        private string GenerateContactUsEmailBody(ContactUsRequest request)
        {
            return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Contact Us - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>

                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #DE532A, #c44622); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>New Contact Form Submission</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors - Manchester</p>
                        </div>

                        <!-- Content -->
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear MCR Team,<br><br>
                                    You have received a new contact form submission from your website. Please find the details below:
                                </p>
                            </div>

                            <!-- Client Information -->
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #DE532A;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    👤 Client Information
                                </h3>
                                <table style='width: 100%; border-collapse: collapse;'>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; width: 25%; vertical-align: top;'>Name:</td>
                                        <td style='padding: 8px 0; color: #333;'>{request.Name}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Email:</td>
                                        <td style='padding: 8px 0; color: #333;'><a href='mailto:{request.Email}' style='color: #DE532A; text-decoration: none;'>{request.Email}</a></td>
                                    </tr>
                                    {(string.IsNullOrEmpty(request.Phone) ? "" : $@"
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Phone:</td>
                                        <td style='padding: 8px 0; color: #333;'><a href='tel:{request.Phone}' style='color: #DE532A; text-decoration: none;'>{request.Phone}</a></td>
                                    </tr>")}
                                </table>
                            </div>

                            <!-- Subject -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #3498db;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    📋 Subject / Department
                                </h3>
                                <p style='margin: 0; font-size: 16px; color: #333; font-weight: 500;'>{request.Subject}</p>
                            </div>

                            <!-- Message -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #e1e5e9; border-left: 5px solid #27ae60;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    💬 Message Details
                                </h3>
                                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; border: 1px solid #e9ecef;'>
                                    <p style='margin: 0; white-space: pre-wrap; line-height: 1.6; color: #333;'>{request.Message}</p>
                                </div>
                            </div>

                            <!-- Action Required -->
                            <div style='background-color: #fff3cd; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #ffeaa7;'>
                                <h3 style='margin: 0 0 10px 0; color: #856404; font-size: 16px;'>⚡ Action Required</h3>
                                <p style='margin: 0; color: #856404; line-height: 1.5;'>
                                    Please follow up with this client within 24 hours. You can reply directly to this email or contact them using the information provided above.
                                </p>
                            </div>
                        </div>

                        <!-- Signature -->
                        <div style='background-color: #f8f9fa; padding: 25px 20px; border-top: 1px solid #e1e5e9;'>
                            <div style='text-align: left;'>
                                <p style='margin: 0 0 15px 0; font-weight: bold; color: #2c3e50; font-size: 16px;'>Kind Regards,</p>
                                <p style='margin: 0 0 5px 0; font-weight: bold; color: #DE532A; font-size: 16px;'>MCR Team</p>
                                <p style='margin: 0 0 15px 0; font-style: italic; color: #666; font-size: 14px;'>Solicitor/Partner</p>

                                <div style='border-top: 1px solid #e1e5e9; padding-top: 15px; margin-top: 15px;'>
                                    <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>📞 Direct:</strong> <a href='tel:01614661280' style='color: #DE532A; text-decoration: none;'>0161 466 1280</a></p>
                                    <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>✉️ Email:</strong> <a href='mailto:contactus@mcrsolicitors.co.uk' style='color: #DE532A; text-decoration: none;'>contactus@mcrsolicitors.co.uk</a></p>
                                    <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>🌐 Website:</strong> <a href='https://www.mcrsolicitors.co.uk' target='_blank' style='color: #DE532A; text-decoration: none;'>www.mcrsolicitors.co.uk</a></p>
                                    <p style='margin: 0 0 0 0; font-size: 14px; color: #333;'><strong>🏢 Address:</strong> 1024 Stockport Road, Manchester, M19 3WX</p>
                                </div>
                            </div>
                        </div>

                        <!-- Legal Notice -->
                        <div style='background-color: #fff3cd; padding: 15px 20px; border-top: 1px solid #ffeaa7;'>
                            <p style='margin: 0; font-size: 11px; color: #856404; line-height: 1.4;'>
                                <strong>⚖️ Confidentiality Notice:</strong> This message contains confidential information and is intended only for the individual named.
                                If you are not the named addressee you should not disseminate, distribute or copy this e-mail. Please notify the sender immediately by e-mail
                                if you have received this e-mail by mistake and delete this e-mail from your system. E-mail transmission cannot be guaranteed to be secured or
                                error-free as information could be intercepted, corrupted, lost, destroyed, arrive late or incomplete, or contain viruses. The sender therefore
                                does not accept liability for any errors or omissions in the contents of this message, which arise as a result of e-mail transmission.
                                If verification is required then please request a hard-copy version.
                            </p>
                        </div>

                        <!-- Footer -->
                        <div style='background-color: #2c3e50; color: #bdc3c7; padding: 15px 20px; text-align: center;'>
                            <p style='margin: 0; font-size: 12px;'>
                                This email was sent from your website contact form on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateEnquiryEmailBody(EnquiryRequest request)
        {
            return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Service Enquiry - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>

                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #e74c3c, #c0392b); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>New Service Enquiry</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors - Manchester</p>
                        </div>

                        <!-- Content -->
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear MCR Team,<br><br>
                                    You have received a new service enquiry from your website. This client is interested in scheduling an appointment. Please find the details below:
                                </p>
                            </div>

                            <!-- Client Information -->
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #DE532A;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    👤 Client Information
                                </h3>
                                <table style='width: 100%; border-collapse: collapse;'>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; width: 25%; vertical-align: top;'>Name:</td>
                                        <td style='padding: 8px 0; color: #333;'>{request.Name}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Email:</td>
                                        <td style='padding: 8px 0; color: #333;'><a href='mailto:{request.Email}' style='color: #DE532A; text-decoration: none;'>{request.Email}</a></td>
                                    </tr>
                                    {(string.IsNullOrEmpty(request.Phone) ? "" : $@"
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Phone:</td>
                                        <td style='padding: 8px 0; color: #333;'><a href='tel:{request.Phone}' style='color: #DE532A; text-decoration: none;'>{request.Phone}</a></td>
                                    </tr>")}
                                </table>
                            </div>

                            <!-- Service Type -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #e74c3c;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    ⚖️ Service Required
                                </h3>
                                <p style='margin: 0; font-size: 16px; color: #333; font-weight: 500; background-color: #f8f9fa; padding: 10px; border-radius: 5px;'>{request.ServiceType}</p>
                            </div>

                            <!-- Enquiry Details -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #f39c12;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    💬 Enquiry Details
                                </h3>
                                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; border: 1px solid #e9ecef;'>
                                    <p style='margin: 0; white-space: pre-wrap; line-height: 1.6; color: #333;'>{request.Enquiry}</p>
                                </div>
                            </div>

                            {(!string.IsNullOrEmpty(request.PreferredContactMethod) ? $@"
                            <!-- Contact Preferences -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #9b59b6;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    📅 Contact Preferences
                                </h3>
                                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px;'>
                                    <p style='margin: 0 0 10px 0; color: #333;'><strong>Preferred Method:</strong> {request.PreferredContactMethod}</p>
                                    {(request.PreferredContactTime.HasValue ? $"<p style='margin: 0; color: #333;'><strong>Preferred Time:</strong> {request.PreferredContactTime:dddd, dd MMMM yyyy 'at' HH:mm}</p>" : "")}
                                </div>
                            </div>" : "")}

                            <!-- Action Required -->
                            <div style='background-color: #fff3cd; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #ffeaa7;'>
                                <h3 style='margin: 0 0 10px 0; color: #856404; font-size: 16px;'>⚡ Urgent Action Required</h3>
                                <p style='margin: 0; color: #856404; line-height: 1.5;'>
                                    This is a service enquiry for <strong>{request.ServiceType}</strong>. Please follow up with this client within 24 hours to schedule their appointment.
                                    You can reply directly to this email or contact them using the information provided above.
                                </p>
                            </div>
                        </div>

                        <!-- Signature -->
                        <div style='background-color: #f8f9fa; padding: 25px 20px; border-top: 1px solid #e1e5e9;'>
                            <div style='text-align: left;'>
                                <p style='margin: 0 0 15px 0; font-weight: bold; color: #2c3e50; font-size: 16px;'>Kind Regards,</p>
                                <p style='margin: 0 0 5px 0; font-weight: bold; color: #DE532A; font-size: 16px;'>MCR Team</p>
                                <p style='margin: 0 0 15px 0; font-style: italic; color: #666; font-size: 14px;'>Solicitor/Partner</p>

                                <div style='border-top: 1px solid #e1e5e9; padding-top: 15px; margin-top: 15px;'>
                                    <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>📞 Direct:</strong> <a href='tel:01614661280' style='color: #DE532A; text-decoration: none;'>0161 466 1280</a></p>
                                    <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>✉️ Email:</strong> <a href='mailto:contactus@mcrsolicitors.co.uk' style='color: #DE532A; text-decoration: none;'>contactus@mcrsolicitors.co.uk</a></p>
                                    <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>🌐 Website:</strong> <a href='https://www.mcrsolicitors.co.uk' target='_blank' style='color: #DE532A; text-decoration: none;'>www.mcrsolicitors.co.uk</a></p>
                                    <p style='margin: 0 0 0 0; font-size: 14px; color: #333;'><strong>🏢 Address:</strong> 1024 Stockport Road, Manchester, M19 3WX</p>
                                </div>
                            </div>
                        </div>

                        <!-- Legal Notice -->
                        <div style='background-color: #fff3cd; padding: 15px 20px; border-top: 1px solid #ffeaa7;'>
                            <p style='margin: 0; font-size: 11px; color: #856404; line-height: 1.4;'>
                                <strong>⚖️ Confidentiality Notice:</strong> This message contains confidential information and is intended only for the individual named.
                                If you are not the named addressee you should not disseminate, distribute or copy this e-mail. Please notify the sender immediately by e-mail
                                if you have received this e-mail by mistake and delete this e-mail from your system. E-mail transmission cannot be guaranteed to be secured or
                                error-free as information could be intercepted, corrupted, lost, destroyed, arrive late or incomplete, or contain viruses. The sender therefore
                                does not accept liability for any errors or omissions in the contents of this message, which arise as a result of e-mail transmission.
                                If verification is required then please request a hard-copy version.
                            </p>
                        </div>

                        <!-- Footer -->
                        <div style='background-color: #2c3e50; color: #bdc3c7; padding: 15px 20px; text-align: center;'>
                            <p style='margin: 0; font-size: 12px;'>
                                This enquiry was submitted on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}