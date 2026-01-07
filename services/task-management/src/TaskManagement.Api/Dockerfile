FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["shared/TaskFlow.Contracts/TaskFlow.Contracts.csproj", "shared/TaskFlow.Contracts/"]
COPY ["shared/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj", "shared/TaskFlow.Infrastructure/"]
COPY ["services/task-management/src/TaskManagement.Domain/TaskManagement.Domain.csproj", "services/task-management/src/TaskManagement.Domain/"]
COPY ["services/task-management/src/TaskManagement.Application/TaskManagement.Application.csproj", "services/task-management/src/TaskManagement.Application/"]
COPY ["services/task-management/src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj", "services/task-management/src/TaskManagement.Infrastructure/"]
COPY ["services/task-management/src/TaskManagement.Api/TaskManagement.Api.csproj", "services/task-management/src/TaskManagement.Api/"]

RUN dotnet restore "services/task-management/src/TaskManagement.Api/TaskManagement.Api.csproj"

COPY . .
WORKDIR "/src/services/task-management/src/TaskManagement.Api"
RUN dotnet build "TaskManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]

