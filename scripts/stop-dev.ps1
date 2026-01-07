Write-Host "Stopping TaskFlow development environment..." -ForegroundColor Yellow

Set-Location $PSScriptRoot\..

docker-compose -f infrastructure/docker-compose.yml down

Write-Host "Environment stopped!" -ForegroundColor Green


