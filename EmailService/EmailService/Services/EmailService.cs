using EmailService.Data;
using EmailService.DTOs;
using EmailService.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Text.Json;

namespace EmailService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(EmailDbContext context, ILogger<EmailService> logger)
        {
            _context = context;
            _logger = logger;
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
            var emailLog = new EmailLog
            {
                AppId = appId,
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                EmailType = emailType,
                Status = "Pending",
                Metadata = metadata
            };

            try
            {
                // Get app credentials
                var credentials = await _context.AppCredentials
                    .FirstOrDefaultAsync(ac => ac.AppId == appId && ac.IsActive);

                if (credentials == null)
                {
                    emailLog.Status = "Failed";
                    emailLog.ErrorMessage = "App credentials not found or inactive";
                    await _context.EmailLogs.AddAsync(emailLog);
                    await _context.SaveChangesAsync();
                    return false;
                }

                // Create email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(credentials.FromName, credentials.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                // Send email
                using var client = new SmtpClient();
                await client.ConnectAsync(credentials.SmtpHost, credentials.SmtpPort, credentials.EnableSsl);
                await client.AuthenticateAsync(credentials.SmtpUsername, credentials.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                // Update log
                emailLog.Status = "Sent";
                emailLog.SentAt = DateTime.UtcNow;
                emailLog.FromEmail = credentials.FromEmail;

                _logger.LogInformation($"Email sent successfully to {toEmail} for app {appId}");
            }
            catch (Exception ex)
            {
                emailLog.Status = "Failed";
                emailLog.ErrorMessage = ex.Message;
                _logger.LogError(ex, $"Failed to send email to {toEmail} for app {appId}");
            }

            await _context.EmailLogs.AddAsync(emailLog);
            await _context.SaveChangesAsync();

            return emailLog.Status == "Sent";
        }

        private string GenerateContactUsEmailBody(ContactUsRequest request)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px;'>Contact Us Form Submission</h2>

                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Contact Details</h3>
                            <p><strong>Name:</strong> {request.Name}</p>
                            <p><strong>Email:</strong> {request.Email}</p>
                            {(string.IsNullOrEmpty(request.Phone) ? "" : $"<p><strong>Phone:</strong> {request.Phone}</p>")}
                        </div>

                        <div style='background-color: #fff; padding: 15px; border-left: 4px solid #3498db; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Subject</h3>
                            <p>{request.Subject}</p>
                        </div>

                        <div style='background-color: #fff; padding: 15px; border-left: 4px solid #27ae60; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Message</h3>
                            <p style='white-space: pre-wrap;'>{request.Message}</p>
                        </div>

                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px;'>
                            <p>This email was sent from your website contact form on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateEnquiryEmailBody(EnquiryRequest request)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #2c3e50; border-bottom: 2px solid #e74c3c; padding-bottom: 10px;'>Service Enquiry</h2>

                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Client Details</h3>
                            <p><strong>Name:</strong> {request.Name}</p>
                            <p><strong>Email:</strong> {request.Email}</p>
                            {(string.IsNullOrEmpty(request.Phone) ? "" : $"<p><strong>Phone:</strong> {request.Phone}</p>")}
                        </div>

                        <div style='background-color: #fff; padding: 15px; border-left: 4px solid #e74c3c; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Service Type</h3>
                            <p><strong>{request.ServiceType}</strong></p>
                        </div>

                        <div style='background-color: #fff; padding: 15px; border-left: 4px solid #f39c12; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Enquiry Details</h3>
                            <p style='white-space: pre-wrap;'>{request.Enquiry}</p>
                        </div>

                        {(!string.IsNullOrEmpty(request.PreferredContactMethod) ? $@"
                        <div style='background-color: #fff; padding: 15px; border-left: 4px solid #9b59b6; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c3e50;'>Contact Preferences</h3>
                            <p><strong>Preferred Method:</strong> {request.PreferredContactMethod}</p>
                            {(request.PreferredContactTime.HasValue ? $"<p><strong>Preferred Time:</strong> {request.PreferredContactTime:yyyy-MM-dd HH:mm}</p>" : "")}
                        </div>" : "")}

                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px;'>
                            <p>This enquiry was submitted on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}