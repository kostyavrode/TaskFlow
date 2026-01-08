Write-Host "Stopping TaskFlow services..." -ForegroundColor Yellow

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$infraDir = Join-Path $projectRoot "infrastructure"

Set-Location $infraDir
docker-compose down

Write-Host ""
Write-Host "✓ All services stopped" -ForegroundColor Green
Write-Host ""


