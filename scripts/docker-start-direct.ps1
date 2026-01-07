# Direct docker-compose commands without script execution policy issues

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$infraDir = Join-Path $projectRoot "infrastructure"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "TaskFlow Docker Desktop Startup" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Checking Docker Desktop..." -ForegroundColor Yellow
try {
    docker --version | Out-Null
    docker-compose --version | Out-Null
    Write-Host "✓ Docker is installed" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Desktop is not running or not installed!" -ForegroundColor Red
    Write-Host "Please start Docker Desktop and try again." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Checking if Docker Desktop is running..." -ForegroundColor Yellow
try {
    docker ps | Out-Null
    Write-Host "✓ Docker Desktop is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Desktop is not running!" -ForegroundColor Red
    Write-Host "Please start Docker Desktop and wait for it to fully start, then try again." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Stopping any existing containers..." -ForegroundColor Yellow
Set-Location $infraDir
docker-compose down 2>&1 | Out-Null

Write-Host ""
Write-Host "Building and starting services..." -ForegroundColor Yellow
Write-Host "This may take a few minutes on first run..." -ForegroundColor Yellow
Write-Host ""

docker-compose up -d --build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Green
    Write-Host "Services are starting up!" -ForegroundColor Green
    Write-Host "======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
    
    Write-Host ""
    Write-Host "Service Status:" -ForegroundColor Cyan
    docker-compose ps
    
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Green
    Write-Host "TaskFlow is ready!" -ForegroundColor Green
    Write-Host "======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Services:" -ForegroundColor Cyan
    Write-Host "  📊 Task Management API: http://localhost:5000" -ForegroundColor White
    Write-Host "  📊 Swagger UI: http://localhost:5000/swagger" -ForegroundColor White
    Write-Host "  🔔 Notification API: http://localhost:5002" -ForegroundColor White
    Write-Host "  🐰 RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor White
    Write-Host ""
    Write-Host "Web UI:" -ForegroundColor Cyan
    Write-Host "  Open: web/index.html in your browser" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ Failed to start services!" -ForegroundColor Red
    Write-Host "Check the error messages above." -ForegroundColor Yellow
    exit 1
}

