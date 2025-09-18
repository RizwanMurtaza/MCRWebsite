using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using EmailService.Data;
using EmailService.Models;
using System.Text.Json;
using System.Net.Mail;
using System.Net;

namespace EmailService.Services
{
    public class EmailQueueProcessor : BackgroundService
    {
        private readonly ILogger<EmailQueueProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public EmailQueueProcessor(ILogger<EmailQueueProcessor> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Queue Processor Service started");
            await LogToDatabase("ServiceStarted", "Info", "Email Queue Processor Service has been started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingEmails();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing email queue");
                    await LogToDatabase("ProcessingError", "Error", $"Error in email processing loop: {ex.Message}",
                        exceptionType: ex.GetType().Name, stackTrace: ex.StackTrace);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Email Queue Processor Service stopping");
            await LogToDatabase("ServiceStopped", "Info", "Email Queue Processor Service has been stopped");
        }

        private async Task ProcessPendingEmails()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmailDbContext>();

            // Get pending emails ordered by priority and creation date
            var pendingEmails = await dbContext.EmailQueueItems
                .Where(e => e.Status == "Pending" || (e.Status == "Failed" && e.RetryCount < e.MaxRetries))
                .OrderByDescending(e => e.Priority == "High" ? 3 : e.Priority == "Normal" ? 2 : 1)
                .ThenBy(e => e.CreatedAt)
                .Take(10) // Process 10 emails at a time
                .ToListAsync();

            if (pendingEmails.Any())
            {
                _logger.LogInformation($"Processing {pendingEmails.Count} pending emails");
            }

            foreach (var emailItem in pendingEmails)
            {
                await ProcessSingleEmail(emailItem, dbContext);
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task ProcessSingleEmail(EmailQueueItem emailItem, EmailDbContext dbContext)
        {
            try
            {
                emailItem.Status = "Processing";
                emailItem.ProcessedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();

                await LogToDatabase("EmailProcessing", "Info", $"Processing email ID {emailItem.Id} to {emailItem.ToEmail}",
                    emailQueueItemId: emailItem.Id, appId: emailItem.AppId);

                // Get app credentials
                var credentials = await dbContext.AppCredentials
                    .FirstOrDefaultAsync(c => c.AppId == emailItem.AppId && c.IsActive);

                if (credentials == null)
                {
                    throw new InvalidOperationException($"Active credentials not found for AppId: {emailItem.AppId}");
                }

                // Send email
                await SendEmail(emailItem, credentials);

                // Mark as sent
                emailItem.Status = "Sent";
                emailItem.SentAt = DateTime.UtcNow;

                _logger.LogInformation($"Email sent successfully to {emailItem.ToEmail}");
                await LogToDatabase("EmailSent", "Info", $"Email successfully sent to {emailItem.ToEmail}",
                    emailQueueItemId: emailItem.Id, appId: emailItem.AppId);

                // Also update the original EmailLog if exists
                var emailLog = await dbContext.EmailLogs
                    .FirstOrDefaultAsync(e => e.AppId == emailItem.AppId &&
                                             e.ToEmail == emailItem.ToEmail &&
                                             e.Subject == emailItem.Subject &&
                                             e.Status == "Pending");
                if (emailLog != null)
                {
                    emailLog.Status = "Sent";
                    emailLog.SentAt = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                emailItem.RetryCount++;
                emailItem.LastError = ex.Message;

                if (emailItem.RetryCount >= emailItem.MaxRetries)
                {
                    emailItem.Status = "Failed";
                    emailItem.FailedAt = DateTime.UtcNow;

                    _logger.LogError(ex, $"Email permanently failed after {emailItem.RetryCount} attempts to {emailItem.ToEmail}");
                    await LogToDatabase("EmailFailed", "Error",
                        $"Email permanently failed after {emailItem.RetryCount} attempts to {emailItem.ToEmail}: {ex.Message}",
                        emailQueueItemId: emailItem.Id, appId: emailItem.AppId,
                        exceptionType: ex.GetType().Name, stackTrace: ex.StackTrace);
                }
                else
                {
                    emailItem.Status = "Failed";

                    _logger.LogWarning($"Email failed (attempt {emailItem.RetryCount}/{emailItem.MaxRetries}) to {emailItem.ToEmail}: {ex.Message}");
                    await LogToDatabase("RetryAttempt", "Warning",
                        $"Email failed (attempt {emailItem.RetryCount}/{emailItem.MaxRetries}) to {emailItem.ToEmail}: {ex.Message}",
                        emailQueueItemId: emailItem.Id, appId: emailItem.AppId);
                }

                // Update EmailLog if exists
                var emailLog = await dbContext.EmailLogs
                    .FirstOrDefaultAsync(e => e.AppId == emailItem.AppId &&
                                             e.ToEmail == emailItem.ToEmail &&
                                             e.Subject == emailItem.Subject &&
                                             e.Status == "Pending");
                if (emailLog != null)
                {
                    emailLog.Status = emailItem.Status;
                    emailLog.ErrorMessage = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task SendEmail(EmailQueueItem emailItem, AppCredentials credentials)
        {
            using var smtpClient = new SmtpClient(credentials.SmtpHost, credentials.SmtpPort)
            {
                Credentials = new NetworkCredential(credentials.SmtpUsername, credentials.SmtpPassword),
                EnableSsl = credentials.EnableSsl,
                Timeout = 30000 // 30 seconds timeout
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(credentials.FromEmail, credentials.FromName),
                Subject = emailItem.Subject,
                Body = emailItem.Body,
                IsBodyHtml = emailItem.IsHtml
            };

            // Add recipients
            mailMessage.To.Add(new MailAddress(emailItem.ToEmail, emailItem.ToName ?? ""));

            if (!string.IsNullOrEmpty(emailItem.CcEmail))
            {
                mailMessage.CC.Add(emailItem.CcEmail);
            }

            if (!string.IsNullOrEmpty(emailItem.BccEmail))
            {
                mailMessage.Bcc.Add(emailItem.BccEmail);
            }

            // Add attachments if any
            if (!string.IsNullOrEmpty(emailItem.AttachmentPaths))
            {
                try
                {
                    var attachmentPaths = JsonSerializer.Deserialize<List<string>>(emailItem.AttachmentPaths);
                    if (attachmentPaths != null)
                    {
                        foreach (var path in attachmentPaths)
                        {
                            if (File.Exists(path))
                            {
                                mailMessage.Attachments.Add(new Attachment(path));
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning($"Failed to parse attachment paths: {ex.Message}");
                }
            }

            await smtpClient.SendMailAsync(mailMessage);
        }

        private async Task LogToDatabase(string eventName, string logLevel, string message,
            int? emailQueueItemId = null, string appId = null,
            string exceptionType = null, string stackTrace = null)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<EmailDbContext>();

                var log = new EmailProcessLog
                {
                    EmailQueueItemId = emailQueueItemId,
                    AppId = appId,
                    LogLevel = logLevel,
                    Event = eventName,
                    Message = message,
                    ExceptionType = exceptionType,
                    StackTrace = stackTrace,
                    ProcessName = "EmailQueueProcessor",
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.EmailProcessLogs.Add(log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write log to database");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email Queue Processor Service is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}