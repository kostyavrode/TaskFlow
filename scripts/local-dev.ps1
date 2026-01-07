Write-Host "Starting TaskFlow local development environment..." -ForegroundColor Green

Set-Location $PSScriptRoot\..

Write-Host "Starting infrastructure (PostgreSQL, RabbitMQ)..." -ForegroundColor Yellow
docker-compose -f infrastructure/docker-compose.yml up -d

Write-Host "Waiting for services to be healthy..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "Running database migrations..." -ForegroundColor Yellow
Set-Location services/task-management/src/TaskManagement.Infrastructure
dotnet ef database update --startup-project ../TaskManagement.Api/TaskManagement.Api.csproj

Set-Location $PSScriptRoot\..

Write-Host "" -ForegroundColor Green
Write-Host "Environment is ready!" -ForegroundColor Green
Write-Host "PostgreSQL: localhost:5432" -ForegroundColor Cyan
Write-Host "RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor Cyan
Write-Host "" -ForegroundColor Green
Write-Host "To start Task Management Service, run:" -ForegroundColor Yellow
Write-Host "cd services/task-management/src/TaskManagement.Api" -ForegroundColor White
Write-Host "dotnet run" -ForegroundColor White


