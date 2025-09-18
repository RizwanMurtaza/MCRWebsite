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

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var controllers = new[]
            {
                "CredentialsController",
                "EmailController",
                "EmailQueueController"
            };

            return Ok(new
            {
                timestamp = DateTime.UtcNow,
                version = "2.0.0",
                availableControllers = controllers,
                hasEmailQueueController = true,
                message = "EmailService is running with EmailQueue support"
            });
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetHealthStatus()
        {
            try
            {
                _logger.LogInformation("Health check requested");

                var canConnect = await _context.Database.CanConnectAsync();
                var connectionString = _context.Database.GetConnectionString();

                // Mask password in connection string for security
                var maskedConnectionString = connectionString;
                if (!string.IsNullOrEmpty(maskedConnectionString) && maskedConnectionString.Contains("Password="))
                {
                    var passwordStart = maskedConnectionString.IndexOf("Password=") + 9;
                    var passwordEnd = maskedConnectionString.IndexOf(';', passwordStart);
                    if (passwordEnd == -1) passwordEnd = maskedConnectionString.Length;
                    maskedConnectionString = maskedConnectionString.Substring(0, passwordStart) + "****" + maskedConnectionString.Substring(passwordEnd);
                }

                var healthStatus = new
                {
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    database = new
                    {
                        canConnect,
                        connectionString = maskedConnectionString,
                        provider = "MySQL"
                    },
                    status = canConnect ? "Healthy" : "Unhealthy"
                };

                if (canConnect)
                {
                    _logger.LogInformation("Health check successful - Database connected");
                    return Ok(healthStatus);
                }
                else
                {
                    _logger.LogError("Health check failed - Cannot connect to database");
                    return StatusCode(503, healthStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check error: {ErrorMessage}", ex.Message);
                return StatusCode(503, new
                {
                    timestamp = DateTime.UtcNow,
                    status = "Error",
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCredentials()
        {
            try
            {
                _logger.LogInformation("Attempting to get all credentials from database");

                // Test database connection first
                if (!await _context.Database.CanConnectAsync())
                {
                    _logger.LogError("Cannot connect to database");
                    return StatusCode(500, new {
                        success = false,
                        message = "Database connection failed",
                        details = "Unable to connect to the database server"
                    });
                }

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

                _logger.LogInformation($"Successfully retrieved {credentials.Count} credentials");
                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting credentials: {ErrorMessage}", ex.Message);

                // Return more detailed error information
                object errorResponse;

                // In production, don't expose internal details unless in Debug mode
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    errorResponse = new
                    {
                        success = false,
                        message = "Failed to retrieve credentials",
                        error = ex.Message,
                        type = ex.GetType().Name,
                        stackTrace = ex.StackTrace
                    };
                }
                else
                {
                    errorResponse = new
                    {
                        success = false,
                        message = "Failed to retrieve credentials",
                        error = ex.Message,
                        type = ex.GetType().Name
                    };
                }

                return StatusCode(500, errorResponse);
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