using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using EmailService.Data;
using EmailService.Models;
using System.Linq;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class EmailQueueController : ControllerBase
    {
        private readonly EmailDbContext _context;
        private readonly ILogger<EmailQueueController> _logger;

        public EmailQueueController(EmailDbContext context, ILogger<EmailQueueController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("queue")]
        public async Task<IActionResult> GetQueuedEmails(
            [FromQuery] string status = null,
            [FromQuery] string appId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = _context.EmailQueueItems.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(e => e.Status == status);

                if (!string.IsNullOrEmpty(appId))
                    query = query.Where(e => e.AppId == appId);

                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(e => e.Priority == "High" ? 3 : e.Priority == "Normal" ? 2 : 1)
                    .ThenByDescending(e => e.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new
                    {
                        e.Id,
                        e.AppId,
                        e.ToEmail,
                        e.Subject,
                        e.Status,
                        e.Priority,
                        e.RetryCount,
                        e.MaxRetries,
                        e.CreatedAt,
                        e.ProcessedAt,
                        e.SentAt,
                        e.FailedAt,
                        e.LastError,
                        e.EmailType,
                        e.FailureReason,
                        e.IsCredentialError
                    })
                    .ToListAsync();

                return Ok(new
                {
                    total = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    items = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email queue");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetEmailLogs(
            [FromQuery] string status = null,
            [FromQuery] string appId = null,
            [FromQuery] string emailType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = _context.EmailLogs.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(e => e.Status == status);

                if (!string.IsNullOrEmpty(appId))
                    query = query.Where(e => e.AppId == appId);

                if (!string.IsNullOrEmpty(emailType))
                    query = query.Where(e => e.EmailType == emailType);

                if (fromDate.HasValue)
                    query = query.Where(e => e.CreatedAt >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(e => e.CreatedAt <= toDate.Value);

                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new
                    {
                        e.Id,
                        e.AppId,
                        e.ToEmail,
                        e.FromEmail,
                        e.Subject,
                        e.EmailType,
                        e.Status,
                        e.ErrorMessage,
                        e.CreatedAt,
                        e.SentAt,
                        e.Metadata,
                        CanRetry = e.Status == "Failed"
                    })
                    .ToListAsync();

                return Ok(new
                {
                    total = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    items = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email logs");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("process-logs")]
        public async Task<IActionResult> GetProcessLogs(
            [FromQuery] string logLevel = null,
            [FromQuery] string appId = null,
            [FromQuery] string eventName = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            try
            {
                var query = _context.EmailProcessLogs.AsQueryable();

                if (!string.IsNullOrEmpty(logLevel))
                    query = query.Where(e => e.LogLevel == logLevel);

                if (!string.IsNullOrEmpty(appId))
                    query = query.Where(e => e.AppId == appId);

                if (!string.IsNullOrEmpty(eventName))
                    query = query.Where(e => e.Event == eventName);

                if (fromDate.HasValue)
                    query = query.Where(e => e.CreatedAt >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(e => e.CreatedAt <= toDate.Value);

                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    total = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    items = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching process logs");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] string appId = null)
        {
            try
            {
                var query = _context.EmailQueueItems.AsQueryable();
                var logsQuery = _context.EmailLogs.AsQueryable();

                if (!string.IsNullOrEmpty(appId))
                {
                    query = query.Where(e => e.AppId == appId);
                    logsQuery = logsQuery.Where(e => e.AppId == appId);
                }

                var queueStats = await query
                    .GroupBy(e => e.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                var last24Hours = DateTime.UtcNow.AddHours(-24);
                var recentStats = await logsQuery
                    .Where(e => e.CreatedAt >= last24Hours)
                    .GroupBy(e => e.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                var emailTypes = await logsQuery
                    .GroupBy(e => e.EmailType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    queue = queueStats,
                    last24Hours = recentStats,
                    emailTypes = emailTypes,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching statistics");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("queue/{id}/retry")]
        public async Task<IActionResult> RetryEmail(int id)
        {
            try
            {
                var emailItem = await _context.EmailQueueItems.FindAsync(id);
                if (emailItem == null)
                    return NotFound(new { error = "Email not found in queue" });

                // Check if it's a credential error
                if (emailItem.IsCredentialError)
                {
                    return BadRequest(new { error = "Cannot retry emails that failed due to credential errors. Please fix credentials first." });
                }

                // Reset for retry
                emailItem.Status = "Pending";
                emailItem.RetryCount = 0;
                emailItem.LastError = null;
                emailItem.ProcessedAt = null;
                emailItem.SentAt = null;
                emailItem.FailedAt = null;
                emailItem.FailureReason = null;
                emailItem.IsCredentialError = false;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email {id} queued for retry");

                return Ok(new { message = "Email queued for retry", id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrying email {id}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("queue/retry-failed")]
        public async Task<IActionResult> RetryAllFailedEmails([FromQuery] string appId = null, [FromQuery] bool includeCredentialErrors = false)
        {
            try
            {
                var query = _context.EmailQueueItems
                    .Where(e => e.Status == "Failed");

                // Filter by appId if provided
                if (!string.IsNullOrEmpty(appId))
                    query = query.Where(e => e.AppId == appId);

                // Exclude credential errors unless specifically requested
                if (!includeCredentialErrors)
                    query = query.Where(e => !e.IsCredentialError);

                var failedEmails = await query.ToListAsync();

                if (!failedEmails.Any())
                {
                    return Ok(new { message = "No failed emails found to retry", count = 0 });
                }

                // Reset all failed emails for retry
                foreach (var emailItem in failedEmails)
                {
                    emailItem.Status = "Pending";
                    emailItem.RetryCount = 0;
                    emailItem.LastError = null;
                    emailItem.ProcessedAt = null;
                    emailItem.SentAt = null;
                    emailItem.FailedAt = null;
                    emailItem.FailureReason = null;
                    if (!emailItem.IsCredentialError) // Only reset if not credential error
                        emailItem.IsCredentialError = false;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Queued {failedEmails.Count} failed emails for retry");

                return Ok(new {
                    message = $"Queued {failedEmails.Count} failed emails for retry",
                    count = failedEmails.Count,
                    ids = failedEmails.Select(e => e.Id).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying failed emails");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("queue/retry-bulk")]
        public async Task<IActionResult> RetryBulkEmails([FromBody] List<int> emailIds)
        {
            try
            {
                if (emailIds == null || !emailIds.Any())
                    return BadRequest(new { error = "No email IDs provided" });

                var emailItems = await _context.EmailQueueItems
                    .Where(e => emailIds.Contains(e.Id) && e.Status == "Failed")
                    .ToListAsync();

                if (!emailItems.Any())
                    return NotFound(new { error = "No failed emails found with provided IDs" });

                var retryableEmails = emailItems.Where(e => !e.IsCredentialError).ToList();
                var credentialErrorEmails = emailItems.Where(e => e.IsCredentialError).ToList();

                // Reset retryable emails
                foreach (var emailItem in retryableEmails)
                {
                    emailItem.Status = "Pending";
                    emailItem.RetryCount = 0;
                    emailItem.LastError = null;
                    emailItem.ProcessedAt = null;
                    emailItem.SentAt = null;
                    emailItem.FailedAt = null;
                    emailItem.FailureReason = null;
                    emailItem.IsCredentialError = false;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Queued {retryableEmails.Count} emails for retry");

                return Ok(new {
                    message = $"Queued {retryableEmails.Count} emails for retry",
                    retried = retryableEmails.Count,
                    skippedCredentialErrors = credentialErrorEmails.Count,
                    retriedIds = retryableEmails.Select(e => e.Id).ToList(),
                    skippedIds = credentialErrorEmails.Select(e => e.Id).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying bulk emails");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("logs/{id}/retry")]
        public async Task<IActionResult> RetryFromEmailLog(int id)
        {
            try
            {
                var emailLog = await _context.EmailLogs.FindAsync(id);
                if (emailLog == null)
                    return NotFound(new { error = "Email log not found" });

                if (emailLog.Status != "Failed")
                    return BadRequest(new { error = "Only failed emails can be retried" });

                // Check if credentials exist and are active
                var credentials = await _context.AppCredentials
                    .FirstOrDefaultAsync(ac => ac.AppId == emailLog.AppId && ac.IsActive);

                if (credentials == null)
                {
                    return BadRequest(new { error = "No active credentials found for this app" });
                }

                // Create a new queue item from the email log
                var emailQueueItem = new EmailQueueItem
                {
                    AppId = emailLog.AppId,
                    ToEmail = emailLog.ToEmail,
                    ToName = "",
                    CcEmail = "",
                    BccEmail = "",
                    Subject = emailLog.Subject,
                    Body = emailLog.Body,
                    IsHtml = true,
                    Status = "Pending",
                    Priority = "Normal",
                    EmailType = emailLog.EmailType,
                    CreatedAt = DateTime.UtcNow,
                    RequestId = Guid.NewGuid().ToString(),
                    AttachmentPaths = "",
                    LastError = "",
                    FailureReason = "",
                    ClientIp = "",
                    UserAgent = ""
                };

                await _context.EmailQueueItems.AddAsync(emailQueueItem);

                // Update the original log status to indicate it's being retried
                emailLog.Status = "Retrying";
                emailLog.ErrorMessage = "Email queued for retry";

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email log {id} re-queued for retry as queue item {emailQueueItem.Id}");

                return Ok(new {
                    message = "Email re-queued for retry",
                    originalLogId = id,
                    newQueueId = emailQueueItem.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrying email from log {id}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpDelete("queue/{id}")]
        public async Task<IActionResult> DeleteFromQueue(int id)
        {
            try
            {
                var emailItem = await _context.EmailQueueItems.FindAsync(id);
                if (emailItem == null)
                    return NotFound(new { error = "Email not found in queue" });

                _context.EmailQueueItems.Remove(emailItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Email {id} deleted from queue");

                return Ok(new { message = "Email deleted from queue", id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting email {id}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}