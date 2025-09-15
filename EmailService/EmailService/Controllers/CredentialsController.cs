using EmailService.Data;
using EmailService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CredentialsController : ControllerBase
    {
        private readonly EmailDbContext _context;
        private readonly ILogger<CredentialsController> _logger;

        public CredentialsController(EmailDbContext context, ILogger<CredentialsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCredentials()
        {
            try
            {
                var credentials = await _context.AppCredentials
                    .Select(ac => new
                    {
                        ac.Id,
                        ac.AppId,
                        ac.AppName,
                        ac.SmtpHost,
                        ac.SmtpPort,
                        ac.SmtpUsername,
                        ac.FromEmail,
                        ac.FromName,
                        ac.EnableSsl,
                        ac.IsActive,
                        ac.CreatedAt,
                        ac.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting credentials");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{appId}")]
        public async Task<IActionResult> GetCredentialsByAppId(string appId)
        {
            try
            {
                var credentials = await _context.AppCredentials
                    .Where(ac => ac.AppId == appId)
                    .Select(ac => new
                    {
                        ac.Id,
                        ac.AppId,
                        ac.AppName,
                        ac.SmtpHost,
                        ac.SmtpPort,
                        ac.SmtpUsername,
                        ac.FromEmail,
                        ac.FromName,
                        ac.EnableSsl,
                        ac.IsActive,
                        ac.CreatedAt,
                        ac.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (credentials == null)
                {
                    return NotFound(new { success = false, message = "Credentials not found" });
                }

                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting credentials for app {AppId}", appId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCredentials([FromBody] AppCredentials request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if AppId already exists
                var existingCredentials = await _context.AppCredentials
                    .FirstOrDefaultAsync(ac => ac.AppId == request.AppId);

                if (existingCredentials != null)
                {
                    return Conflict(new { success = false, message = "AppId already exists" });
                }

                var credentials = new AppCredentials
                {
                    AppId = request.AppId,
                    AppName = request.AppName,
                    SmtpHost = request.SmtpHost,
                    SmtpPort = request.SmtpPort,
                    SmtpUsername = request.SmtpUsername,
                    SmtpPassword = request.SmtpPassword,
                    FromEmail = request.FromEmail,
                    FromName = request.FromName,
                    EnableSsl = request.EnableSsl,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.AppCredentials.AddAsync(credentials);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCredentialsByAppId), new { appId = credentials.AppId },
                    new { success = true, message = "Credentials created successfully", appId = credentials.AppId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating credentials");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPut("{appId}")]
        public async Task<IActionResult> UpdateCredentials(string appId, [FromBody] AppCredentials request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCredentials = await _context.AppCredentials
                    .FirstOrDefaultAsync(ac => ac.AppId == appId);

                if (existingCredentials == null)
                {
                    return NotFound(new { success = false, message = "Credentials not found" });
                }

                existingCredentials.AppName = request.AppName;
                existingCredentials.SmtpHost = request.SmtpHost;
                existingCredentials.SmtpPort = request.SmtpPort;
                existingCredentials.SmtpUsername = request.SmtpUsername;
                existingCredentials.SmtpPassword = request.SmtpPassword;
                existingCredentials.FromEmail = request.FromEmail;
                existingCredentials.FromName = request.FromName;
                existingCredentials.EnableSsl = request.EnableSsl;
                existingCredentials.IsActive = request.IsActive;
                existingCredentials.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Credentials updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating credentials for app {AppId}", appId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{appId}")]
        public async Task<IActionResult> DeleteCredentials(string appId)
        {
            try
            {
                var credentials = await _context.AppCredentials
                    .FirstOrDefaultAsync(ac => ac.AppId == appId);

                if (credentials == null)
                {
                    return NotFound(new { success = false, message = "Credentials not found" });
                }

                _context.AppCredentials.Remove(credentials);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Credentials deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting credentials for app {AppId}", appId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}