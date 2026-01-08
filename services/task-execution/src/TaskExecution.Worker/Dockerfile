FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["shared/TaskFlow.Contracts/TaskFlow.Contracts.csproj", "shared/TaskFlow.Contracts/"]
COPY ["shared/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj", "shared/TaskFlow.Infrastructure/"]
COPY ["services/task-execution/src/TaskExecution.Domain/TaskExecution.Domain.csproj", "services/task-execution/src/TaskExecution.Domain/"]
COPY ["services/task-execution/src/TaskExecution.Application/TaskExecution.Application.csproj", "services/task-execution/src/TaskExecution.Application/"]
COPY ["services/task-execution/src/TaskExecution.Infrastructure/TaskExecution.Infrastructure.csproj", "services/task-execution/src/TaskExecution.Infrastructure/"]
COPY ["services/task-execution/src/TaskExecution.Worker/TaskExecution.Worker.csproj", "services/task-execution/src/TaskExecution.Worker/"]

RUN dotnet restore "services/task-execution/src/TaskExecution.Worker/TaskExecution.Worker.csproj"

COPY . .
WORKDIR "/src/services/task-execution/src/TaskExecution.Worker"
RUN dotnet build "TaskExecution.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskExecution.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskExecution.Worker.dll"]


