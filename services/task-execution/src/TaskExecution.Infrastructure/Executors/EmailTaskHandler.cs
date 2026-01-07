using System.Text.Json;
using Microsoft.Extensions.Logging;
using TaskExecution.Domain.Entities;
using TaskExecution.Domain.Services;

namespace TaskExecution.Infrastructure.Executors;

public class EmailTaskHandler : ITaskTypeHandler
{
    private readonly ILogger<EmailTaskHandler> _logger;

    public EmailTaskHandler(ILogger<EmailTaskHandler> logger)
    {
        _logger = logger;
    }

    public string TaskType => "Email";

    public async Task<ExecutionResult> HandleAsync(
        ExecutionRecord record, 
        IProgress<ExecutionProgress> progress, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing email task {TaskId}", record.TaskId);

        progress.Report(new ExecutionProgress(20, "Parsing email data"));
        await Task.Delay(300, cancellationToken);

        string? recipient = null;
        if (!string.IsNullOrEmpty(record.Payload))
        {
            try
            {
                var payload = JsonSerializer.Deserialize<JsonElement>(record.Payload);
                if (payload.TryGetProperty("recipient", out var recipientElement))
                {
                    recipient = recipientElement.GetString();
                }
            }
            catch (JsonException)
            {
                _logger.LogWarning("Invalid payload format for email task {TaskId}", record.TaskId);
            }
        }

        progress.Report(new ExecutionProgress(50, "Preparing email content"));
        await Task.Delay(400, cancellationToken);

        progress.Report(new ExecutionProgress(80, "Sending email"));
        await Task.Delay(500, cancellationToken);

        _logger.LogInformation("Email sent to {Recipient} for task {TaskId}", recipient ?? "default", record.TaskId);
        return new ExecutionResult(true, $"email://{recipient ?? "sent"}");
    }
}

