using Microsoft.EntityFrameworkCore;
using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

var workerFilesPath = Environment.GetEnvironmentVariable("REPORTS_VOLUME_PATH") 
    ?? Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "task-execution", "src", "TaskExecution.Worker", "wwwroot"));
Console.WriteLine($"Static files path: {workerFilesPath}");
Console.WriteLine($"Path exists: {Directory.Exists(workerFilesPath)}");
if (!Directory.Exists(workerFilesPath))
{
    Directory.CreateDirectory(workerFilesPath);
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(workerFilesPath),
    RequestPath = ""
});

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
