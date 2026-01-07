FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["shared/TaskFlow.Contracts/TaskFlow.Contracts.csproj", "shared/TaskFlow.Contracts/"]
COPY ["shared/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj", "shared/TaskFlow.Infrastructure/"]
COPY ["services/notification/src/Notification.Application/Notification.Application.csproj", "services/notification/src/Notification.Application/"]
COPY ["services/notification/src/Notification.Infrastructure/Notification.Infrastructure.csproj", "services/notification/src/Notification.Infrastructure/"]
COPY ["services/notification/src/Notification.Api/Notification.Api.csproj", "services/notification/src/Notification.Api/"]

RUN dotnet restore "services/notification/src/Notification.Api/Notification.Api.csproj"

COPY . .
WORKDIR "/src/services/notification/src/Notification.Api"
RUN dotnet build "Notification.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]

