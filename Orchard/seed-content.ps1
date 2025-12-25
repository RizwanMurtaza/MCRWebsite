# MCR Solicitors Content Seeding Script
# This script seeds all website content into the Orchard Core CMS database

param(
    [string]$Tenant = "Default",
    [string]$BaseUrl = "http://localhost:5000"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MCR Solicitors Content Seeder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if the application is running
Write-Host "Checking if application is running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $BaseUrl -UseBasicParsing -TimeoutSec 5 -ErrorAction Stop
    Write-Host "Application is running at $BaseUrl" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Application is not running at $BaseUrl" -ForegroundColor Red
    Write-Host "Please start the application first with: dotnet run --project src\MCRSolicitors.Web" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "To seed content, follow these steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "OPTION 1: Via Admin Panel (Recommended)" -ForegroundColor Green
Write-Host "----------------------------------------"
Write-Host "1. Navigate to: $BaseUrl/admin" -ForegroundColor White
Write-Host "2. Login with your admin credentials" -ForegroundColor White
Write-Host "3. Go to: Configuration > Recipes" -ForegroundColor White
Write-Host "4. Find 'MCR Solicitors Content' recipe" -ForegroundColor White
Write-Host "5. Click 'Execute' to seed all content" -ForegroundColor White
Write-Host ""
Write-Host "OPTION 2: Via Command Line" -ForegroundColor Green
Write-Host "----------------------------------------"
Write-Host "Run this command from the Orchard folder:" -ForegroundColor White
Write-Host ""
Write-Host "  dotnet orchard run mcr-content --tenant $Tenant" -ForegroundColor Yellow
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Optional: Attempt to execute recipe via API if setup
$executeRecipe = Read-Host "Would you like to open the admin panel now? (y/n)"
if ($executeRecipe -eq "y" -or $executeRecipe -eq "Y") {
    Start-Process "$BaseUrl/admin/Recipes"
}
