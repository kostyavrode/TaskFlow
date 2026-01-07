using Notification.Application;
using Notification.Application.Hubs;
using Notification.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors("AllowAll");

app.MapHub<TaskNotificationHub>("/hubs/tasks");

app.MapHealthChecks("/health");

app.MapGet("/", () => "Notification Service is running. Connect to /hubs/tasks for real-time updates.");

app.Run();
