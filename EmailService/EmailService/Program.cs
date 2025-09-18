using EmailService.Data;
using EmailService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure Entity Framework with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register services
builder.Services.AddScoped<IEmailService, EmailService.Services.EmailService>();
builder.Services.AddHttpContextAccessor();

// Register background service
builder.Services.AddHostedService<EmailQueueProcessor>();

// Add logging
builder.Services.AddLogging();

// Add CORS for cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Initialize database in background task
_ = Task.Run(async () =>
{
    await Task.Delay(5000); // Wait 5 seconds after startup
    using var scope = app.Services.CreateScope();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<EmailDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Attempting to create database and apply migrations...");

        // Check if database exists first
        var canConnect = await context.Database.CanConnectAsync();
        if (!canConnect)
        {
            logger.LogInformation("Database does not exist, creating with migrations...");

            // Create database and apply all migrations
            await context.Database.MigrateAsync();
            logger.LogInformation("Database created and migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database exists, checking for pending migrations...");

            // Database exists, just apply pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Found {pendingMigrations.Count()} pending migrations, applying...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Pending migrations applied successfully");
            }
            else
            {
                logger.LogInformation("No pending migrations found, database is up to date");
            }
        }

        // Final connection test
        var finalConnectionTest = await context.Database.CanConnectAsync();
        logger.LogInformation($"Final database connection test: {(finalConnectionTest ? "Success" : "Failed")}");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
});

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("AllowAll");

// Serve static files (for admin.html)
app.UseStaticFiles();

app.MapControllers();

// Redirect root URL to admin panel
app.MapGet("/", () => Results.Redirect("/admin.html"));

// Add route for emails monitoring page
app.MapGet("/emails", () => Results.Redirect("/emails.html"));

app.Run();
