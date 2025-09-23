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
            try
            {
                // Send acknowledgment email to client
                var clientSubject = "Thank You for Contacting MCR Solicitors - We'll Be In Touch Soon";
                var clientBody = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Thank You - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>
                        <div style='background: linear-gradient(135deg, #DE532A, #c44622); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>Thank You for Contacting Us</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors - Manchester</p>
                        </div>
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear {request.Name},<br><br>
                                    Thank you for reaching out to MCR Solicitors. We have successfully received your inquiry and wanted to confirm the details you provided:
                                </p>
                            </div>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #DE532A;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px;'>📋 Your Submission Details</h3>
                                <p style='margin: 0 0 10px 0; color: #333;'><strong>Name:</strong> {request.Name}</p>
                                <p style='margin: 0 0 10px 0; color: #333;'><strong>Email:</strong> {request.Email}</p>
                                <p style='margin: 0 0 10px 0; color: #333;'><strong>Department:</strong> {request.Subject}</p>
                                <p style='margin: 0; color: #333;'><strong>Submitted:</strong> {DateTime.UtcNow:dddd, dd MMMM yyyy 'at' HH:mm} UTC</p>
                            </div>
                            <div style='background-color: #e8f5e8; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #27ae60;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px;'>✅ What Happens Next?</h3>
                                <ul style='margin: 0; padding-left: 20px; line-height: 1.8;'>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>We will review your inquiry within <strong>24 hours</strong></li>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>A member of our team will contact you using the details provided</li>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>We'll discuss your requirements and how we can assist you</li>
                                    <li style='color: #2c3e50;'>If urgent, please call us directly on <strong>0161 466 1280</strong></li>
                                </ul>
                            </div>
                        </div>
                        <div style='background-color: #f8f9fa; padding: 25px 20px; border-top: 1px solid #e1e5e9;'>
                            <p style='margin: 0 0 15px 0; font-weight: bold; color: #2c3e50; font-size: 16px;'>Kind Regards,</p>
                            <p style='margin: 0 0 5px 0; font-weight: bold; color: #DE532A; font-size: 16px;'>MCR Solicitors Team</p>
                            <p style='margin: 0 0 15px 0; font-style: italic; color: #666; font-size: 14px;'>Professional Legal Services</p>
                        </div>
                        <div style='background-color: #2c3e50; color: #bdc3c7; padding: 15px 20px; text-align: center;'>
                            <p style='margin: 0; font-size: 12px;'>
                                This is an automated response confirming receipt of your inquiry submitted on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
                var clientMetadata = JsonSerializer.Serialize(new { request.Phone, Type = "ContactUs_ClientAcknowledgment" });

                var clientEmailSent = await SendEmailAsync(request.AppId, request.Email, clientSubject, clientBody, "ContactUs_ClientAcknowledgment", clientMetadata);

                // Send notification email to MCR team
                var teamSubject = $"🚨 New Contact Form Submission - {request.Subject} - {request.Name}";
                var teamBody = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>New Contact Form Submission - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>
                        <div style='background: linear-gradient(135deg, #DE532A, #c44622); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>🚨 New Contact Form Submission</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors Website</p>
                        </div>
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear MCR Team,<br><br>
                                    You have received a new contact form submission from your website. Please find the client details below:
                                </p>
                            </div>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #DE532A;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px;'>👤 Client Information</h3>
                                <p style='margin: 0 0 8px 0; color: #333;'><strong>Name:</strong> {request.Name}</p>
                                <p style='margin: 0 0 8px 0; color: #333;'><strong>Email:</strong> <a href='mailto:{request.Email}' style='color: #DE532A; text-decoration: none;'>{request.Email}</a></p>
                                {(string.IsNullOrEmpty(request.Phone) ? "" : $"<p style='margin: 0 0 8px 0; color: #333;'><strong>Phone:</strong> <a href='tel:{request.Phone}' style='color: #DE532A; text-decoration: none;'>{request.Phone}</a></p>")}
                                <p style='margin: 0; color: #333;'><strong>Submitted:</strong> {DateTime.UtcNow:dddd, dd MMMM yyyy 'at' HH:mm} UTC</p>
                            </div>
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #3498db;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px;'>📋 Subject / Department</h3>
                                <p style='margin: 0; font-size: 16px; color: #333; font-weight: 500;'>{request.Subject}</p>
                            </div>
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #27ae60;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px;'>💬 Message Details</h3>
                                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; border: 1px solid #e9ecef;'>
                                    <p style='margin: 0; white-space: pre-wrap; line-height: 1.6; color: #333;'>{request.Message}</p>
                                </div>
                            </div>
                            <div style='background-color: #ffebee; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #ffcdd2; border-left: 5px solid #f44336;'>
                                <h3 style='margin: 0 0 10px 0; color: #c62828; font-size: 16px;'>⚡ URGENT - Action Required</h3>
                                <p style='margin: 0; color: #c62828; line-height: 1.5;'>
                                    <strong>Please follow up with this client within 24 hours.</strong><br>
                                    • Reply directly to <a href='mailto:{request.Email}' style='color: #DE532A;'>{request.Email}</a><br>
                                    • Call them on <strong>{(string.IsNullOrEmpty(request.Phone) ? "phone not provided" : request.Phone)}</strong><br>
                                    • Client has been sent an acknowledgment email confirming receipt
                                </p>
                            </div>
                        </div>
                        <div style='background-color: #2c3e50; color: #bdc3c7; padding: 15px 20px; text-align: center;'>
                            <p style='margin: 0; font-size: 12px;'>
                                Internal notification - MCR Solicitors Contact Form System
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
                var teamMetadata = JsonSerializer.Serialize(new {
                    ClientName = request.Name,
                    ClientEmail = request.Email,
                    ClientPhone = request.Phone,
                    Type = "ContactUs_TeamNotification"
                });

                var teamEmailSent = await SendEmailAsync(request.AppId, "contactus@mcrsolicitors.co.uk", teamSubject, teamBody, "ContactUs_TeamNotification", teamMetadata);

                // Return true if at least the client email was sent successfully
                // Log if team email fails but don't fail the entire operation
                if (!teamEmailSent)
                {
                    _logger.LogWarning($"Team notification email failed for contact form submission from {request.Name} ({request.Email})");
                }

                return clientEmailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process contact form submission from {request.Name} ({request.Email})");
                return false;
            }
        }

        public async Task<bool> SendEnquiryEmailAsync(EnquiryRequest request)
        {
            try
            {
                // Send acknowledgment email to client
                var clientSubject = "Thank You for Your Service Enquiry - MCR Solicitors";
                var clientBody = GenerateEnquiryAcknowledgmentEmail(request);
                var clientMetadata = JsonSerializer.Serialize(new
                {
                    request.Phone,
                    request.ServiceType,
                    request.PreferredContactMethod,
                    request.PreferredContactTime,
                    Type = "Enquiry_ClientAcknowledgment"
                });

                var clientEmailSent = await SendEmailAsync(request.AppId, request.Email, clientSubject, clientBody, "Enquiry_ClientAcknowledgment", clientMetadata);

                // Send notification email to MCR team
                var teamSubject = $"🚨 New Service Enquiry - {request.ServiceType} - {request.Name}";
                var teamBody = GenerateEnquiryNotificationEmail(request);
                var teamMetadata = JsonSerializer.Serialize(new
                {
                    ClientName = request.Name,
                    ClientEmail = request.Email,
                    ClientPhone = request.Phone,
                    ServiceType = request.ServiceType,
                    PreferredContactMethod = request.PreferredContactMethod,
                    PreferredContactTime = request.PreferredContactTime,
                    Type = "Enquiry_TeamNotification"
                });

                var teamEmailSent = await SendEmailAsync(request.AppId, "contactus@mcrsolicitors.co.uk", teamSubject, teamBody, "Enquiry_TeamNotification", teamMetadata);

                // Return true if at least the client email was sent successfully
                if (!teamEmailSent)
                {
                    _logger.LogWarning($"Team notification email failed for service enquiry from {request.Name} ({request.Email})");
                }

                return clientEmailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process service enquiry from {request.Name} ({request.Email})");
                return false;
            }
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

                _logger.LogInformation($"Email queued successfully for {toEmail}");


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
            return GenerateClientAcknowledgmentEmailTemplate(request);
        }

        private string GenerateClientAcknowledgmentEmailTemplate(ContactUsRequest request)
        {
            return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Thank You - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>

                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #DE532A, #c44622); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>Thank You for Contacting Us</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors - Manchester</p>
                        </div>

                        <!-- Content -->
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear {request.Name},<br><br>
                                    Thank you for reaching out to MCR Solicitors. We have successfully received your inquiry and wanted to confirm the details you provided:
                                </p>
                            </div>

                            <!-- Submission Details -->
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #DE532A;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    📋 Your Submission Details
                                </h3>
                                <table style='width: 100%; border-collapse: collapse;'>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; width: 25%; vertical-align: top;'>Name:</td>
                                        <td style='padding: 8px 0; color: #333;'>{request.Name}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Email:</td>
                                        <td style='padding: 8px 0; color: #333;'>{request.Email}</td>
                                    </tr>
                                    {(string.IsNullOrEmpty(request.Phone) ? "" : $@"
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Phone:</td>
                                        <td style='padding: 8px 0; color: #333;'>{request.Phone}</td>
                                    </tr>")}
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Department:</td>
                                        <td style='padding: 8px 0; color: #333;'>{request.Subject}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Submitted:</td>
                                        <td style='padding: 8px 0; color: #333;'>{DateTime.UtcNow:dddd, dd MMMM yyyy 'at' HH:mm} UTC</td>
                                    </tr>
                                </table>
                            </div>

                            <!-- Next Steps -->
                            <div style='background-color: #e8f5e8; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #27ae60;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    ✅ What Happens Next?
                                </h3>
                                <ul style='margin: 0; padding-left: 20px; line-height: 1.8;'>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>We will review your inquiry within <strong>24 hours</strong></li>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>A member of our team will contact you using the details provided</li>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>We'll discuss your requirements and how we can assist you</li>
                                    <li style='color: #2c3e50;'>If urgent, please call us directly on <strong>0161 466 1280</strong></li>
                                </ul>
                            </div>

                            <!-- Contact Information -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #e1e5e9; border-left: 5px solid #3498db;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    📞 Contact Information
                                </h3>
                                <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>📞 Phone:</strong> <a href='tel:01614661280' style='color: #DE532A; text-decoration: none;'>0161 466 1280</a></p>
                                <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>✉️ Email:</strong> <a href='mailto:contactus@mcrsolicitors.co.uk' style='color: #DE532A; text-decoration: none;'>contactus@mcrsolicitors.co.uk</a></p>
                                <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>🌐 Website:</strong> <a href='https://www.mcrsolicitors.co.uk' target='_blank' style='color: #DE532A; text-decoration: none;'>www.mcrsolicitors.co.uk</a></p>
                                <p style='margin: 0 0 0 0; font-size: 14px; color: #333;'><strong>🏢 Address:</strong> 1024 Stockport Road, Manchester, M19 3WX</p>
                            </div>
                        </div>

                        <!-- Signature -->
                        <div style='background-color: #f8f9fa; padding: 25px 20px; border-top: 1px solid #e1e5e9;'>
                            <div style='text-align: left;'>
                                <p style='margin: 0 0 15px 0; font-weight: bold; color: #2c3e50; font-size: 16px;'>Kind Regards,</p>
                                <p style='margin: 0 0 5px 0; font-weight: bold; color: #DE532A; font-size: 16px;'>MCR Solicitors Team</p>
                                <p style='margin: 0 0 15px 0; font-style: italic; color: #666; font-size: 14px;'>Professional Legal Services</p>
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
                                This is an automated response confirming receipt of your inquiry submitted on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateContactUsNotificationEmailTemplate(ContactUsRequest request)
        {
            return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>New Contact Form Submission - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>

                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #DE532A, #c44622); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>🚨 New Contact Form Submission</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors Website</p>
                        </div>

                        <!-- Content -->
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear MCR Team,<br><br>
                                    You have received a new contact form submission from your website. Please find the client details below:
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
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #555; vertical-align: top;'>Submitted:</td>
                                        <td style='padding: 8px 0; color: #333;'>{DateTime.UtcNow:dddd, dd MMMM yyyy 'at' HH:mm} UTC</td>
                                    </tr>
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
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #27ae60;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    💬 Message Details
                                </h3>
                                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; border: 1px solid #e9ecef;'>
                                    <p style='margin: 0; white-space: pre-wrap; line-height: 1.6; color: #333;'>{request.Message}</p>
                                </div>
                            </div>

                            <!-- Action Required -->
                            <div style='background-color: #ffebee; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #ffcdd2; border-left: 5px solid #f44336;'>
                                <h3 style='margin: 0 0 10px 0; color: #c62828; font-size: 16px;'>⚡ URGENT - Action Required</h3>
                                <p style='margin: 0; color: #c62828; line-height: 1.5;'>
                                    <strong>Please follow up with this client within 24 hours.</strong><br>
                                    • Reply directly to <a href='mailto:{request.Email}' style='color: #DE532A;'>{request.Email}</a><br>
                                    • Call them on <strong>{(string.IsNullOrEmpty(request.Phone) ? "phone not provided" : request.Phone)}</strong><br>
                                    • Client has been sent an acknowledgment email confirming receipt
                                </p>
                            </div>
                        </div>

                        <!-- Footer -->
                        <div style='background-color: #2c3e50; color: #bdc3c7; padding: 15px 20px; text-align: center;'>
                            <p style='margin: 0; font-size: 12px;'>
                                Internal notification - MCR Solicitors Contact Form System
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateEnquiryAcknowledgmentEmail(EnquiryRequest request)
        {
            return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Thank You for Your Enquiry - MCR Solicitors</title>
                </head>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;'>
                    <div style='max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1); overflow: hidden;'>

                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #e74c3c, #c0392b); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>Thank You for Your Service Enquiry</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>MCR Solicitors - Manchester</p>
                        </div>

                        <!-- Content -->
                        <div style='padding: 30px 20px;'>
                            <div style='margin-bottom: 30px;'>
                                <p style='font-size: 16px; color: #2c3e50; margin: 0 0 20px 0; line-height: 1.5;'>
                                    Dear {request.Name},<br><br>
                                    Thank you for your service enquiry. We have successfully received your request and wanted to confirm the details:
                                </p>
                            </div>

                            <!-- Service Details -->
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #e74c3c;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    ⚖️ Service Requested
                                </h3>
                                <p style='margin: 0; font-size: 16px; color: #333; font-weight: 500; background-color: #fff; padding: 15px; border-radius: 5px;'>{request.ServiceType}</p>
                            </div>

                            <!-- Contact Preferences -->
                            {(!string.IsNullOrEmpty(request.PreferredContactMethod) ? $@"
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 25px; border: 1px solid #e1e5e9; border-left: 5px solid #9b59b6;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    📅 Your Contact Preferences
                                </h3>
                                <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px;'>
                                    <p style='margin: 0 0 10px 0; color: #333;'><strong>Preferred Method:</strong> {request.PreferredContactMethod}</p>
                                    {(request.PreferredContactTime.HasValue ? $"<p style='margin: 0; color: #333;'><strong>Preferred Time:</strong> {request.PreferredContactTime:dddd, dd MMMM yyyy 'at' HH:mm}</p>" : "")}
                                </div>
                            </div>" : "")}

                            <!-- Next Steps -->
                            <div style='background-color: #e8f5e8; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 5px solid #27ae60;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    ✅ What Happens Next?
                                </h3>
                                <ul style='margin: 0; padding-left: 20px; line-height: 1.8;'>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>We will review your <strong>{request.ServiceType}</strong> enquiry within <strong>24 hours</strong></li>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>A specialist solicitor will contact you to discuss your requirements</li>
                                    <li style='margin-bottom: 8px; color: #2c3e50;'>We'll arrange a consultation at your convenience</li>
                                    <li style='color: #2c3e50;'>If urgent, please call us directly on <strong>0161 466 1280</strong></li>
                                </ul>
                            </div>

                            <!-- Contact Information -->
                            <div style='background-color: #fff; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #e1e5e9; border-left: 5px solid #3498db;'>
                                <h3 style='margin: 0 0 15px 0; color: #2c3e50; font-size: 18px; display: flex; align-items: center;'>
                                    📞 Contact Information
                                </h3>
                                <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>📞 Phone:</strong> <a href='tel:01614661280' style='color: #DE532A; text-decoration: none;'>0161 466 1280</a></p>
                                <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>✉️ Email:</strong> <a href='mailto:contactus@mcrsolicitors.co.uk' style='color: #DE532A; text-decoration: none;'>contactus@mcrsolicitors.co.uk</a></p>
                                <p style='margin: 0 0 8px 0; font-size: 14px; color: #333;'><strong>🌐 Website:</strong> <a href='https://www.mcrsolicitors.co.uk' target='_blank' style='color: #DE532A; text-decoration: none;'>www.mcrsolicitors.co.uk</a></p>
                                <p style='margin: 0 0 0 0; font-size: 14px; color: #333;'><strong>🏢 Address:</strong> 1024 Stockport Road, Manchester, M19 3WX</p>
                            </div>
                        </div>

                        <!-- Signature -->
                        <div style='background-color: #f8f9fa; padding: 25px 20px; border-top: 1px solid #e1e5e9;'>
                            <div style='text-align: left;'>
                                <p style='margin: 0 0 15px 0; font-weight: bold; color: #2c3e50; font-size: 16px;'>Kind Regards,</p>
                                <p style='margin: 0 0 5px 0; font-weight: bold; color: #DE532A; font-size: 16px;'>MCR Solicitors Team</p>
                                <p style='margin: 0 0 15px 0; font-style: italic; color: #666; font-size: 14px;'>Professional Legal Services</p>
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
                                This is an automated response confirming receipt of your service enquiry submitted on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateEnquiryNotificationEmail(EnquiryRequest request)
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