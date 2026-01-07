# TaskFlow Local Development Setup Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   TaskFlow Local Development Setup    " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Stop any running containers
Write-Host "[1/5] Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f infrastructure/docker-compose.yml down 2>$null

# Start infrastructure
Write-Host "[2/5] Starting infrastructure (PostgreSQL, RabbitMQ)..." -ForegroundColor Yellow
docker-compose -f infrastructure/docker-compose.yml up -d

Write-Host "[3/5] Waiting for services to be healthy..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Run migrations
Write-Host "[4/5] Running database migrations..." -ForegroundColor Yellow

Write-Host "  - Task Management Service..." -ForegroundColor Gray
dotnet ef database update `
    --project services/task-management/src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj `
    --startup-project services/task-management/src/TaskManagement.Api/TaskManagement.Api.csproj 2>$null

Write-Host "  - Task Execution Service..." -ForegroundColor Gray
dotnet ef database update `
    --project services/task-execution/src/TaskExecution.Infrastructure/TaskExecution.Infrastructure.csproj `
    --startup-project services/task-execution/src/TaskExecution.Worker/TaskExecution.Worker.csproj 2>$null

Write-Host "[5/5] Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Services Ready:                     " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  PostgreSQL (Task Management): localhost:5432" -ForegroundColor White
Write-Host "  PostgreSQL (Task Execution):  localhost:5433" -ForegroundColor White
Write-Host "  RabbitMQ:                      localhost:5672" -ForegroundColor White
Write-Host "  RabbitMQ Management:           http://localhost:15672" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   To Start Services:                  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Terminal 1 - Task Management API:" -ForegroundColor Yellow
Write-Host "    cd services/task-management/src/TaskManagement.Api" -ForegroundColor Gray
Write-Host "    dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "  Terminal 2 - Task Execution Worker:" -ForegroundColor Yellow
Write-Host "    cd services/task-execution/src/TaskExecution.Worker" -ForegroundColor Gray
Write-Host "    dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "  Then open:" -ForegroundColor Yellow
Write-Host "    API Swagger: http://localhost:5000/swagger" -ForegroundColor Gray
Write-Host "    Web UI:      web/index.html" -ForegroundColor Gray
Write-Host ""
