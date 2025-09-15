using EmailService.Data;
using EmailService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure Entity Framework with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=EmailServiceDb;User=root;Password=;";

builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register services
builder.Services.AddScoped<IEmailService, EmailService.Services.EmailService>();

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

        logger.LogInformation("Attempting to ensure database is created...");
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Database ensured successfully");

        // Test the connection
        var canConnect = await context.Database.CanConnectAsync();
        logger.LogInformation($"Database connection test: {(canConnect ? "Success" : "Failed")}");
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

app.UseCors("AllowAll");
app.UseRouting();

// Serve static files (for admin.html)
app.UseStaticFiles();

app.MapControllers();

// Redirect root URL to admin panel
app.MapGet("/", () => Results.Redirect("/admin.html"));

app.Run();
