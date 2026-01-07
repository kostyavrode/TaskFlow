param(
    [string]$Service = ""
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$infraDir = Join-Path $projectRoot "infrastructure"

Set-Location $infraDir

if ($Service -eq "") {
    Write-Host "Showing logs for all services (Ctrl+C to exit)..." -ForegroundColor Yellow
    docker-compose logs -f
} else {
    Write-Host "Showing logs for service: $Service (Ctrl+C to exit)..." -ForegroundColor Yellow
    docker-compose logs -f $Service
}

