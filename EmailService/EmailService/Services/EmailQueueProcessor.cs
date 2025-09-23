using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using EmailService.Data;
using EmailService.Models;
using System.Text.Json;
using MailKit.Security;
using MimeKit;

namespace EmailService.Services
{
    public class EmailQueueProcessor : BackgroundService
    {
        private readonly ILogger<EmailQueueProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        // Office 365 rate limiting (per Microsoft documentation)
        private readonly Dictionary<string, Queue<DateTime>> _office365SendHistory = new();
        private readonly int _office365RateLimit = 30; // 30 messages per minute
        private readonly TimeSpan _office365RateWindow = TimeSpan.FromMinutes(1);

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
            // Exclude emails that failed due to credential errors (don't retry those)
            var pendingEmails = await dbContext.EmailQueueItems
               .Where(e => e.Status == "Pending" ||
                          (e.Status == "Failed" && e.RetryCount < e.MaxRetries && !e.IsCredentialError))
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

                // Test connection first if this email has failed before
                if (emailItem.RetryCount > 0)
                {
                    // First test basic network connectivity
                    var networkTest = await TestNetworkConnectivity(credentials.SmtpHost, credentials.SmtpPort);
                    if (!networkTest)
                    {
                        _logger.LogError($"Network connectivity blocked to {credentials.SmtpHost}:{credentials.SmtpPort}. " +
                            "This indicates firewall/ISP restrictions. Consider using alternative SMTP provider.");

                        // Try Gmail on port 587 if we were using 465
                        if (credentials.SmtpHost.Contains("gmail.com") && credentials.SmtpPort == 465)
                        {
                            _logger.LogInformation("Attempting Gmail on port 587 as fallback...");
                            var port587Test = await TestNetworkConnectivity("smtp.gmail.com", 587);
                            if (!port587Test)
                            {
                                throw new Exception($"Both Gmail SMTP ports (465, 587) are blocked by server/firewall. Contact hosting provider or use alternative SMTP service.");
                            }
                            else
                            {
                                _logger.LogInformation("Port 587 is accessible. Update your credentials to use port 587 instead of 465.");
                                throw new Exception($"Gmail port 465 blocked but port 587 is accessible. Update SMTP configuration to use port 587.");
                            }
                        }

                        throw new Exception($"Network connectivity blocked to {credentials.SmtpHost}:{credentials.SmtpPort}");
                    }

                    var connectionTest = await TestSmtpConnection(credentials);
                    if (!connectionTest)
                    {
                        throw new Exception($"SMTP connection test failed for {credentials.SmtpHost}:{credentials.SmtpPort}");
                    }
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

                // Check if this is a credential error (authentication/authorization failures)
                var isCredentialError = IsCredentialError(ex);
                if (isCredentialError)
                {
                    emailItem.IsCredentialError = true;
                    emailItem.FailureReason = "CredentialError";
                    emailItem.Status = "Failed";
                    emailItem.FailedAt = DateTime.UtcNow;

                    _logger.LogError(ex, $"Email failed due to credential error for {emailItem.ToEmail} (AppId: {emailItem.AppId}). Will not retry.");
                    await LogToDatabase("CredentialError", "Error",
                        $"Email failed due to credential error for {emailItem.ToEmail} (AppId: {emailItem.AppId}): {ex.Message}",
                        emailQueueItemId: emailItem.Id, appId: emailItem.AppId,
                        exceptionType: ex.GetType().Name, stackTrace: ex.StackTrace);
                }
                else if (emailItem.RetryCount >= emailItem.MaxRetries)
                {
                    emailItem.Status = "Failed";
                    emailItem.FailedAt = DateTime.UtcNow;
                    emailItem.FailureReason = "MaxRetriesExceeded";

                    _logger.LogError(ex, $"Email permanently failed after {emailItem.RetryCount} attempts to {emailItem.ToEmail}");
                    await LogToDatabase("EmailFailed", "Error",
                        $"Email permanently failed after {emailItem.RetryCount} attempts to {emailItem.ToEmail}: {ex.Message}",
                        emailQueueItemId: emailItem.Id, appId: emailItem.AppId,
                        exceptionType: ex.GetType().Name, stackTrace: ex.StackTrace);
                }
                else
                {
                    emailItem.Status = "Failed";
                    emailItem.FailureReason = "TemporaryError";

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

        private Task<bool> CheckOffice365RateLimit(string smtpHost)
        {
            if (!smtpHost.Contains("office365.com") && !smtpHost.Contains("outlook.com"))
                return Task.FromResult(true); // Not Office 365, no rate limit

            lock (_office365SendHistory)
            {
                if (!_office365SendHistory.ContainsKey(smtpHost))
                {
                    _office365SendHistory[smtpHost] = new Queue<DateTime>();
                }

                var history = _office365SendHistory[smtpHost];
                var now = DateTime.UtcNow;

                // Remove old entries outside the rate window
                while (history.Count > 0 && now - history.Peek() > _office365RateWindow)
                {
                    history.Dequeue();
                }

                // Check if we're at the rate limit
                if (history.Count >= _office365RateLimit)
                {
                    var oldestSend = history.Peek();
                    var waitTime = _office365RateWindow - (now - oldestSend);
                    _logger.LogWarning($"Office 365 rate limit reached ({_office365RateLimit} messages per minute). Need to wait {waitTime.TotalSeconds:F1} seconds.");
                    return Task.FromResult(false);
                }

                // Record this send
                history.Enqueue(now);
                _logger.LogDebug($"Office 365 rate limit check: {history.Count}/{_office365RateLimit} messages in the last minute");
                return Task.FromResult(true);
            }
        }

        private async Task SendEmail(EmailQueueItem emailItem, AppCredentials credentials)
        {
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                // Validate Office 365 settings if applicable
                if (credentials.SmtpHost.Contains("office365.com") ||
                    credentials.SmtpHost.Contains("outlook.com"))
                {
                    ValidateOffice365Settings(credentials);

                    // Check rate limit for Office 365
                    if (!await CheckOffice365RateLimit(credentials.SmtpHost))
                    {
                        // Rate limit exceeded, this email will be retried later
                        emailItem.LastError = "Office 365 rate limit exceeded - will retry";
                        _logger.LogWarning($"Email to {emailItem.ToEmail} delayed due to Office 365 rate limit");
                        return; // Don't throw, just return to retry later
                    }
                }

                // Set timeouts for server environments
                smtpClient.Timeout = 60000; // 60 seconds for server environments
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true; // Accept all certificates for server compatibility
                smtpClient.CheckCertificateRevocation = false; // Disable certificate revocation check for servers

                // Use optimized connection strategy for Gmail, Microsoft/Outlook and other providers
                MailKit.Security.SecureSocketOptions secureSocketOptions;

                if (credentials.SmtpHost.Contains("gmail.com"))
                {
                    if (credentials.SmtpPort == 465)
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.SslOnConnect;
                    }
                    else if (credentials.SmtpPort == 587)
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls;
                    }
                    else
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
                    }
                }
                else if (credentials.SmtpHost.Contains("outlook.com") ||
                         credentials.SmtpHost.Contains("office365.com") ||
                         credentials.SmtpHost.Contains("hotmail.com") ||
                         credentials.SmtpHost.Contains("mcrsolicitors.co.uk"))
                {
                    // Microsoft services require STARTTLS on port 587
                    if (credentials.SmtpPort == 587)
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls;
                    }
                    else
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
                    }
                }
                else
                {
                    // Default strategy for other providers
                    secureSocketOptions = credentials.EnableSsl ?
                        (credentials.SmtpPort == 465 ? MailKit.Security.SecureSocketOptions.SslOnConnect : MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable) :
                        MailKit.Security.SecureSocketOptions.None;
                }

                await smtpClient.ConnectAsync(credentials.SmtpHost, credentials.SmtpPort, secureSocketOptions);

                // Authenticate
                try
                {
                    await smtpClient.AuthenticateAsync(credentials.SmtpUsername, credentials.SmtpPassword);
                }
                catch (MailKit.Security.AuthenticationException authEx)
                {
                    // Provide specific guidance for Microsoft authentication issues
                    if (credentials.SmtpHost.Contains("outlook.com") ||
                        credentials.SmtpHost.Contains("office365.com") ||
                        credentials.SmtpHost.Contains("hotmail.com") ||
                        credentials.SmtpHost.Contains("live.com"))
                    {
                        // Check for specific Office 365 error codes
                        if (authEx.Message.Contains("5.7.139"))
                        {
                            _logger.LogError(authEx,
                                "CRITICAL: Office 365 SMTP AUTH is DISABLED for this account!\n" +
                                "==========================================================\n" +
                                "Error 5.7.139 means SMTP authentication is blocked.\n\n" +
                                "IMMEDIATE ACTIONS REQUIRED:\n" +
                                "1. Enable SMTP AUTH using PowerShell:\n" +
                                "   Set-CASMailbox -Identity " + credentials.SmtpUsername + " -SmtpClientAuthenticationDisabled $false\n\n" +
                                "2. OR in Microsoft 365 Admin Center:\n" +
                                "   Settings → Org Settings → Security & Privacy → Modern Authentication\n" +
                                "   Enable 'Authenticated SMTP'\n\n" +
                                "3. If MFA is enabled, create an App Password:\n" +
                                "   https://mysignins.microsoft.com/security-info\n" +
                                "   Add sign-in method → App password\n\n" +
                                "4. Check if Security Defaults are blocking:\n" +
                                "   Azure AD → Properties → Manage Security Defaults\n\n" +
                                "See OFFICE365_SMTP_SETUP.md for detailed instructions.\n" +
                                "==========================================================");
                        }
                        else if (authEx.Message.Contains("5.7.57"))
                        {
                            _logger.LogError(authEx,
                                "Office 365 Authentication Error 5.7.57: Client not authenticated.\n" +
                                "Check: Username format (must be full email), Password, and SMTP AUTH status.");
                        }
                        else
                        {
                            _logger.LogError(authEx, $"Microsoft/Outlook authentication failed.\n" +
                                "Common causes:\n" +
                                "1) SMTP AUTH disabled (most common) - Enable in Admin Center\n" +
                                "2) MFA enabled - Use an App Password instead\n" +
                                "3) Security Defaults blocking - Check Azure AD settings\n" +
                                "4) Wrong username format - Use full email address\n" +
                                "5) Account locked or requires verification\n" +
                                "Error details: {authEx.Message}");
                        }
                    }
                    else
                    {
                        _logger.LogError(authEx, $"Authentication failed for {credentials.SmtpHost}");
                    }
                    throw;
                }

                // Create message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(credentials.FromName, credentials.FromEmail));
                message.To.Add(new MailboxAddress(emailItem.ToName ?? "", emailItem.ToEmail));
                message.Subject = emailItem.Subject;

                // Add CC recipients
                if (!string.IsNullOrEmpty(emailItem.CcEmail))
                {
                    var ccAddresses = emailItem.CcEmail.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var cc in ccAddresses)
                    {
                        message.Cc.Add(new MailboxAddress("", cc.Trim()));
                    }
                }

                // Add BCC recipients
                if (!string.IsNullOrEmpty(emailItem.BccEmail))
                {
                    var bccAddresses = emailItem.BccEmail.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var bcc in bccAddresses)
                    {
                        message.Bcc.Add(new MailboxAddress("", bcc.Trim()));
                    }
                }

                // Create body
                var bodyBuilder = new BodyBuilder();
                if (emailItem.IsHtml)
                {
                    bodyBuilder.HtmlBody = emailItem.Body;
                }
                else
                {
                    bodyBuilder.TextBody = emailItem.Body;
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
                                    await bodyBuilder.Attachments.AddAsync(path);
                                }
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning($"Failed to parse attachment paths: {ex.Message}");
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                // Send the message
                await smtpClient.SendAsync(message);
                _logger.LogInformation($"Email sent successfully to {emailItem.ToEmail}");
            }
            catch (System.TimeoutException ex)
            {
                _logger.LogError(ex, $"SMTP timeout when sending email to {emailItem.ToEmail} via {credentials.SmtpHost}:{credentials.SmtpPort}");
                throw new Exception($"SMTP timeout: {ex.Message}", ex);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                _logger.LogError(ex, $"SMTP command error when sending email to {emailItem.ToEmail}: Status={ex.StatusCode}, Message={ex.Message}");
                throw new Exception($"SMTP error {ex.StatusCode}: {ex.Message}", ex);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                _logger.LogError(ex, $"SMTP authentication failed for {credentials.SmtpUsername} on {credentials.SmtpHost}:{credentials.SmtpPort}");
                throw new Exception($"SMTP authentication failed: {ex.Message}", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                _logger.LogError(ex, $"Network/Socket error connecting to SMTP {credentials.SmtpHost}:{credentials.SmtpPort}");
                throw new Exception($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error sending email to {emailItem.ToEmail} via {credentials.SmtpHost}:{credentials.SmtpPort}");
                throw;
            }
            finally
            {
                try
                {
                    if (smtpClient.IsConnected)
                    {
                        await smtpClient.DisconnectAsync(true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error during SMTP disconnect");
                }
            }
        }

        private async Task<bool> TestNetworkConnectivity(string host, int port)
        {
            try
            {

                using var tcpClient = new System.Net.Sockets.TcpClient();
                var connectTask = tcpClient.ConnectAsync(host, port);
                var timeoutTask = Task.Delay(10000); // 10 second timeout

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    return false;
                }

                if (connectTask.IsFaulted)
                {
                    return false;
                }

                tcpClient.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> TestSmtpConnection(AppCredentials credentials)
        {
            using var testClient = new MailKit.Net.Smtp.SmtpClient();
            try
            {

                // Server-specific configurations
                testClient.Timeout = 60000; // 60 seconds for server environments
                testClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Enable more verbose logging
                testClient.CheckCertificateRevocation = false;

                // Try multiple connection strategies for different providers
                var connectionStrategies = new List<(MailKit.Security.SecureSocketOptions, string)>();

                if (credentials.SmtpHost.Contains("gmail.com"))
                {

                    if (credentials.SmtpPort == 465)
                    {
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.SslOnConnect, "SSL on Connect (Port 465)"));
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.Auto, "Auto-detect SSL"));
                    }
                    else if (credentials.SmtpPort == 587)
                    {
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.StartTls, "StartTLS (Port 587)"));
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable, "StartTLS when available"));
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.Auto, "Auto-detect SSL"));
                    }
                }
                else if (credentials.SmtpHost.Contains("outlook.com") ||
                         credentials.SmtpHost.Contains("office365.com") ||
                         credentials.SmtpHost.Contains("hotmail.com") ||
                         credentials.SmtpHost.Contains("mcrsolicitors.com"))
                {

                    if (credentials.SmtpPort == 587)
                    {
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.StartTls, "StartTLS (Port 587) - Microsoft Required"));
                        connectionStrategies.Add((MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable, "StartTLS when available"));
                    }
                    connectionStrategies.Add((MailKit.Security.SecureSocketOptions.Auto, "Auto-detect SSL"));
                }
                else
                {
                    // Default strategy for other providers
                    var secureSocketOptions = credentials.EnableSsl ?
                        (credentials.SmtpPort == 465 ? MailKit.Security.SecureSocketOptions.SslOnConnect : MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable) :
                        MailKit.Security.SecureSocketOptions.None;
                    connectionStrategies.Add((secureSocketOptions, $"Default SSL strategy"));
                }

                Exception lastException = null;

                foreach (var (strategy, description) in connectionStrategies)
                {
                    try
                    {
                        if (testClient.IsConnected)
                        {
                            await testClient.DisconnectAsync(false);
                        }

                        await testClient.ConnectAsync(credentials.SmtpHost, credentials.SmtpPort, strategy);

                        await testClient.AuthenticateAsync(credentials.SmtpUsername, credentials.SmtpPassword);

                        if (testClient.IsConnected)
                        {
                            await testClient.DisconnectAsync(true);
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;

                        try
                        {
                            if (testClient.IsConnected)
                            {
                                await testClient.DisconnectAsync(false);
                            }
                        }
                        catch { }
                    }
                }

                throw lastException ?? new Exception("All connection strategies failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"All SMTP connection strategies failed for {credentials.SmtpHost}:{credentials.SmtpPort}. " +
                    $"Error type: {ex.GetType().Name}. " +
                    $"This may indicate firewall blocking, ISP restrictions, or server network policies.");

                // Additional server diagnostics
                _logger.LogError($"Server diagnostics - Host: {credentials.SmtpHost}, Port: {credentials.SmtpPort}, " +
                    $"SSL Enabled: {credentials.EnableSsl}, Username: {credentials.SmtpUsername}");

                try
                {
                    if (testClient.IsConnected)
                    {
                        await testClient.DisconnectAsync(true);
                    }
                }
                catch { }

                return false;
            }
        }

        private bool IsCredentialError(Exception ex)
        {
            // Check for common credential-related exceptions and error messages
            var errorMessage = ex.Message?.ToLowerInvariant() ?? "";
            var innerMessage = ex.InnerException?.Message?.ToLowerInvariant() ?? "";

            // MailKit SMTP Authentication errors
            if (ex is MailKit.Security.AuthenticationException)
            {
                return true;
            }

            if (ex is MailKit.Net.Smtp.SmtpCommandException smtpEx)
            {
                // Check SMTP status codes for authentication failures
                // MailKit uses different status code names
                var statusCode = (int)smtpEx.StatusCode;
                return statusCode == 530 || // Authentication required
                       statusCode == 535 || // Authentication failed
                       statusCode == 534 || // Authentication mechanism is too weak
                       statusCode == 550;   // Mailbox unavailable
            }

            // Check for common authentication error messages
            var credentialErrorKeywords = new[]
            {
                "authentication failed",
                "authentication failure",
                "invalid username or password",
                "invalid credentials",
                "access denied",
                "unauthorized",
                "login failed",
                "authentication error",
                "bad username or password",
                "invalid user name or password",
                "smtp auth failed",
                "535", // SMTP auth failed status code
                "534", // Authentication mechanism is too weak
                "530", // Authentication required
                "5.7.0", // Authentication Required
                "authentication required",
                "mailbox unavailable",
                "permission denied"
            };

            return credentialErrorKeywords.Any(keyword =>
                errorMessage.Contains(keyword) || innerMessage.Contains(keyword));
        }

        private bool ValidateOffice365Settings(AppCredentials credentials)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            // Check SMTP Host (as per Microsoft documentation)
            if (!credentials.SmtpHost.Equals("smtp.office365.com", StringComparison.OrdinalIgnoreCase))
            {
                warnings.Add($"Microsoft recommends 'smtp.office365.com' but found '{credentials.SmtpHost}'");
            }

            // Check Port (587 recommended, 25 also supported per Microsoft docs)
            if (credentials.SmtpPort != 587 && credentials.SmtpPort != 25)
            {
                errors.Add($"Office 365 requires port 587 (recommended) or 25, but port {credentials.SmtpPort} is configured");
            }
            else if (credentials.SmtpPort == 25)
            {
                warnings.Add("Port 25 is supported but port 587 is recommended by Microsoft");
            }

            // Check SSL/TLS is enabled (required by Microsoft)
            if (!credentials.EnableSsl)
            {
                errors.Add("Office 365 requires TLS encryption (TLS 1.2 or higher)");
            }

            // Check username format (must be full email address)
            if (!credentials.SmtpUsername.Contains("@"))
            {
                errors.Add($"Office 365 requires full email address as username (e.g., user@domain.com), but got '{credentials.SmtpUsername}'");
            }

            // Check if username matches the from email (required for authentication)
            if (!credentials.SmtpUsername.Equals(credentials.FromEmail, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"Office 365 requires the authenticated user ({credentials.SmtpUsername}) to match the From address ({credentials.FromEmail})");
            }

            // Display warnings
            foreach (var warning in warnings)
            {
                _logger.LogWarning($"Office 365 Validation Warning: {warning}");
            }

            // Display errors
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    _logger.LogError($"Office 365 Validation Error: {error}");
                }
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateSmtpAuthentication(AppCredentials credentials)
        {
            try
            {
                // Special validation for Office 365
                if (credentials.SmtpHost.Contains("office365.com") ||
                    credentials.SmtpHost.Contains("outlook.com"))
                {
                    ValidateOffice365Settings(credentials);
                }

                using var testClient = new MailKit.Net.Smtp.SmtpClient();
                testClient.Timeout = 30000; // 30 seconds timeout
                testClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                testClient.CheckCertificateRevocation = false;

                // Determine connection strategy
                MailKit.Security.SecureSocketOptions secureSocketOptions;
                if (credentials.SmtpHost.Contains("office365.com") ||
                    credentials.SmtpHost.Contains("outlook.com"))
                {
                    secureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls;
                }
                else if (credentials.SmtpPort == 587)
                {
                    secureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls;
                }
                else if (credentials.SmtpPort == 465)
                {
                    secureSocketOptions = MailKit.Security.SecureSocketOptions.SslOnConnect;
                }
                else
                {
                    secureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
                }

                // Test connection
                await testClient.ConnectAsync(credentials.SmtpHost, credentials.SmtpPort, secureSocketOptions);

                // Check if authentication is required
                if (testClient.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Authentication))
                {
                    await testClient.AuthenticateAsync(credentials.SmtpUsername, credentials.SmtpPassword);
                }
                else
                {
                    _logger.LogWarning("Server does not require authentication - this is unusual for Office 365");
                }

                await testClient.DisconnectAsync(true);
                return true;
            }
            catch (MailKit.Security.AuthenticationException authEx)
            {
                _logger.LogError(authEx, $"Authentication validation failed: {authEx.Message}");

                if (credentials.SmtpHost.Contains("office365.com"))
                {
                    _logger.LogError("Office 365 Authentication Failed - Troubleshooting Guide:\n" +
                        "1. Verify SMTP AUTH is enabled in Exchange Online (tenant-level setting)\n" +
                        "2. If using MFA: Create an app password in Microsoft account security settings\n" +
                        "3. Ensure the mailbox has a valid Exchange Online license (not just Microsoft 365)\n" +
                        "4. Check account is not compromised or locked\n" +
                        "5. Confirm TLS 1.2 or 1.3 is being used\n" +
                        "6. Verify SPF/DKIM/DMARC records are configured\n" +
                        "7. Check if IP is not blacklisted\n" +
                        "8. Consider using OAuth 2.0 for modern authentication\n" +
                        "Reference: https://learn.microsoft.com/exchange/mail-flow-best-practices/how-to-set-up-a-multifunction-device-or-application-to-send-email-using-microsoft-365-or-office-365");
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SMTP validation error: {ex.Message}");
                return false;
            }
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