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
                        e.CreatedAt,
                        e.ProcessedAt,
                        e.SentAt,
                        e.FailedAt,
                        e.LastError,
                        e.EmailType
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
                        e.Metadata
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

                // Reset for retry
                emailItem.Status = "Pending";
                emailItem.RetryCount = 0;
                emailItem.LastError = null;
                emailItem.ProcessedAt = null;
                emailItem.SentAt = null;
                emailItem.FailedAt = null;

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