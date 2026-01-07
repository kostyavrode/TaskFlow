using Microsoft.Extensions.Logging;
using TaskExecution.Domain.Entities;
using TaskExecution.Domain.Services;

namespace TaskExecution.Infrastructure.Executors;

public class DataProcessingTaskHandler : ITaskTypeHandler
{
    private readonly ILogger<DataProcessingTaskHandler> _logger;

    public DataProcessingTaskHandler(ILogger<DataProcessingTaskHandler> logger)
    {
        _logger = logger;
    }

    public string TaskType => "DataProcessing";

    public async Task<ExecutionResult> HandleAsync(
        ExecutionRecord record, 
        IProgress<ExecutionProgress> progress, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing data for task {TaskId}", record.TaskId);

        var totalSteps = 10;
        for (int i = 1; i <= totalSteps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var percent = (i * 100) / totalSteps;
            progress.Report(new ExecutionProgress(percent, $"Processing batch {i}/{totalSteps}"));

            await Task.Delay(GetProcessingDelay(record.Priority), cancellationToken);
        }

        var resultLocation = $"data/{record.TaskId}/processed_{DateTime.UtcNow:yyyyMMddHHmmss}.parquet";
        
        _logger.LogInformation("Data processing completed: {ResultLocation}", resultLocation);
        return new ExecutionResult(true, resultLocation);
    }

    private static int GetProcessingDelay(string priority)
    {
        return priority.ToLowerInvariant() switch
        {
            "critical" => 100,
            "high" => 200,
            "medium" => 400,
            "low" => 600,
            _ => 400
        };
    }
}

