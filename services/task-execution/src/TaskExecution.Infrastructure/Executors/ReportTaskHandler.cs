using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskExecution.Domain.Entities;
using TaskExecution.Domain.Enums;
using TaskExecution.Domain.Services;
using TaskExecution.Infrastructure.Persistence;

namespace TaskExecution.Infrastructure.Executors;

public class ReportTaskHandler : ITaskTypeHandler
{
    private readonly ILogger<ReportTaskHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _outputPath;

    public ReportTaskHandler(ILogger<ReportTaskHandler> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(_outputPath);
    }

    public string TaskType => "Report";

    public async Task<ExecutionResult> HandleAsync(
        ExecutionRecord record, 
        IProgress<ExecutionProgress> progress, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating report for task {TaskId}", record.TaskId);

        progress.Report(new ExecutionProgress(10, "Initializing report generator"));
        await Task.Delay(300, cancellationToken);

        progress.Report(new ExecutionProgress(30, "Fetching data from database"));
        
        var reportParams = ParseReportParameters(record.Payload);
        
        ExecutionStatistics stats;
        List<ExecutionRecord> recentTasks;
        
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskExecutionDbContext>();
            stats = await GetExecutionStatisticsAsync(dbContext, record.UserId, reportParams, cancellationToken);
            recentTasks = await GetRecentTasksAsync(dbContext, record.UserId, reportParams.RecentTasksCount, reportParams, cancellationToken);
        }
        
        await Task.Delay(300, cancellationToken);

        var taskDirectory = Path.Combine(_outputPath, "files", "reports", record.TaskId.ToString());
        Directory.CreateDirectory(taskDirectory);

        progress.Report(new ExecutionProgress(60, "Processing statistics"));
        await Task.Delay(300, cancellationToken);

        progress.Report(new ExecutionProgress(80, "Generating report file"));
        var fileName = $"report_{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
        var filePath = Path.Combine(taskDirectory, fileName);

        var reportContent = BuildReportContent(record, stats, recentTasks, reportParams);

        await File.WriteAllTextAsync(filePath, reportContent, cancellationToken);
        await Task.Delay(200, cancellationToken);

        progress.Report(new ExecutionProgress(95, "Finalizing report"));
        await Task.Delay(200, cancellationToken);

        var resultLocation = $"files/reports/{record.TaskId}/{fileName}";
        
        _logger.LogInformation("Report generated and saved: {FilePath}", filePath);
        return new ExecutionResult(true, resultLocation);
    }

    private ReportParameters ParseReportParameters(string? payload)
    {
        var parameters = new ReportParameters
        {
            RecentTasksCount = 10,
            DateFrom = null,
            DateTo = null,
            TaskTypesFilter = null,
            PrioritiesFilter = null
        };

        if (string.IsNullOrWhiteSpace(payload))
            return parameters;

        try
        {
            var json = JsonSerializer.Deserialize<JsonElement>(payload);
            
            if (json.TryGetProperty("recentTasksCount", out var recentTasksCountProp) && 
                recentTasksCountProp.ValueKind == JsonValueKind.Number)
            {
                parameters.RecentTasksCount = Math.Min(Math.Max(recentTasksCountProp.GetInt32(), 1), 50);
            }

            if (json.TryGetProperty("dateFrom", out var dateFromProp) && 
                dateFromProp.ValueKind == JsonValueKind.String)
            {
                if (DateTime.TryParse(dateFromProp.GetString(), out var dateFrom))
                    parameters.DateFrom = dateFrom;
            }

            if (json.TryGetProperty("dateTo", out var dateToProp) && 
                dateToProp.ValueKind == JsonValueKind.String)
            {
                if (DateTime.TryParse(dateToProp.GetString(), out var dateTo))
                    parameters.DateTo = dateTo;
            }

            if (json.TryGetProperty("taskTypes", out var taskTypesProp) && 
                taskTypesProp.ValueKind == JsonValueKind.Array)
            {
                parameters.TaskTypesFilter = taskTypesProp.EnumerateArray()
                    .Select(e => e.GetString())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Cast<string>()
                    .ToList();
            }

            if (json.TryGetProperty("priorities", out var prioritiesProp) && 
                prioritiesProp.ValueKind == JsonValueKind.Array)
            {
                parameters.PrioritiesFilter = prioritiesProp.EnumerateArray()
                    .Select(e => e.GetString())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Cast<string>()
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse report parameters from payload");
        }

        return parameters;
    }

    private async Task<ExecutionStatistics> GetExecutionStatisticsAsync(
        TaskExecutionDbContext dbContext, 
        string userId, 
        ReportParameters parameters,
        CancellationToken ct)
    {
        var query = dbContext.ExecutionRecords.Where(r => r.UserId == userId);

        if (parameters.DateFrom.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= parameters.DateFrom.Value);
        }

        if (parameters.DateTo.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= parameters.DateTo.Value);
        }

        if (parameters.TaskTypesFilter != null && parameters.TaskTypesFilter.Any())
        {
            query = query.Where(r => parameters.TaskTypesFilter.Contains(r.TaskType));
        }

        if (parameters.PrioritiesFilter != null && parameters.PrioritiesFilter.Any())
        {
            query = query.Where(r => parameters.PrioritiesFilter.Contains(r.Priority));
        }

        var allRecords = await query.ToListAsync(ct);

        return new ExecutionStatistics
        {
            TotalTasks = allRecords.Count,
            CompletedTasks = allRecords.Count(r => r.Status == ExecutionStatus.Completed),
            FailedTasks = allRecords.Count(r => r.Status == ExecutionStatus.Failed),
            RunningTasks = allRecords.Count(r => r.Status == ExecutionStatus.Running),
            QueuedTasks = allRecords.Count(r => r.Status == ExecutionStatus.Queued),
            CancelledTasks = allRecords.Count(r => r.Status == ExecutionStatus.Cancelled),
            TasksByType = allRecords.GroupBy(r => r.TaskType).ToDictionary(g => g.Key, g => g.Count()),
            TasksByPriority = allRecords.GroupBy(r => r.Priority).ToDictionary(g => g.Key, g => g.Count()),
            AverageExecutionTimeMs = allRecords
                .Where(r => r.CompletedAt.HasValue && r.StartedAt.HasValue)
                .Select(r => (r.CompletedAt!.Value - r.StartedAt!.Value).TotalMilliseconds)
                .DefaultIfEmpty(0)
                .Average()
        };
    }

    private async Task<List<ExecutionRecord>> GetRecentTasksAsync(
        TaskExecutionDbContext dbContext, 
        string userId, 
        int count,
        ReportParameters parameters,
        CancellationToken ct)
    {
        var query = dbContext.ExecutionRecords.Where(r => r.UserId == userId);

        if (parameters.DateFrom.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= parameters.DateFrom.Value);
        }

        if (parameters.DateTo.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= parameters.DateTo.Value);
        }

        if (parameters.TaskTypesFilter != null && parameters.TaskTypesFilter.Any())
        {
            query = query.Where(r => parameters.TaskTypesFilter.Contains(r.TaskType));
        }

        if (parameters.PrioritiesFilter != null && parameters.PrioritiesFilter.Any())
        {
            query = query.Where(r => parameters.PrioritiesFilter.Contains(r.Priority));
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    private string BuildReportContent(ExecutionRecord record, ExecutionStatistics stats, List<ExecutionRecord> recentTasks, ReportParameters reportParams)
    {
        var sb = new StringBuilder();
        var now = DateTime.UtcNow;

        sb.AppendLine("+===============================================================================+");
        sb.AppendLine("|                        TASKFLOW ANALYTICS REPORT                             |");
        sb.AppendLine("+===============================================================================+");
        sb.AppendLine();
        
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine("|                           REPORT INFORMATION                                 |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine($"|  Report ID:       {record.TaskId,-58} |");
        sb.AppendLine($"|  User ID:         {record.UserId,-58} |");
        sb.AppendLine($"|  Generated At:    {now:yyyy-MM-dd HH:mm:ss} UTC{new string(' ', 38)} |");
        sb.AppendLine($"|  Correlation ID:  {record.CorrelationId,-58} |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine();

        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine("|                         EXECUTION STATISTICS                                 |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine($"|  Total Tasks:          {stats.TotalTasks,-54} |");
        sb.AppendLine($"|  [OK] Completed:       {stats.CompletedTasks,-54} |");
        sb.AppendLine($"|  [XX] Failed:          {stats.FailedTasks,-54} |");
        sb.AppendLine($"|  [..] Running:         {stats.RunningTasks,-54} |");
        sb.AppendLine($"|  [--] Queued:          {stats.QueuedTasks,-54} |");
        sb.AppendLine($"|  [  ] Cancelled:       {stats.CancelledTasks,-54} |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        
        var successRate = stats.TotalTasks > 0 
            ? (stats.CompletedTasks * 100.0 / stats.TotalTasks).ToString("F1") + "%" 
            : "N/A";
        sb.AppendLine($"|  Success Rate:         {successRate,-54} |");
        sb.AppendLine($"|  Avg Execution Time:   {stats.AverageExecutionTimeMs:F0} ms{new string(' ', 48)} |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine();

        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine("|                            TASKS BY TYPE                                     |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        foreach (var kvp in stats.TasksByType.OrderByDescending(x => x.Value))
        {
            var bar = new string('#', Math.Min(kvp.Value * 2, 30));
            sb.AppendLine($"|  {kvp.Key,-18} {kvp.Value,4}  {bar,-40} |");
        }
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine();

        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine("|                          TASKS BY PRIORITY                                   |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        foreach (var kvp in stats.TasksByPriority.OrderByDescending(x => x.Value))
        {
            var bar = new string('=', Math.Min(kvp.Value * 2, 30));
            sb.AppendLine($"|  {kvp.Key,-18} {kvp.Value,4}  {bar,-40} |");
        }
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine();

        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine("|                       RECENT TASK EXECUTIONS                                 |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine("|  Task ID                               Type              Status     Created  |");
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        
        foreach (var task in recentTasks)
        {
            var statusIcon = task.Status switch
            {
                ExecutionStatus.Completed => "[OK]",
                ExecutionStatus.Failed => "[XX]",
                ExecutionStatus.Running => "[..]",
                ExecutionStatus.Cancelled => "[  ]",
                ExecutionStatus.Queued => "[--]",
                _ => "[??]"
            };
            sb.AppendLine($"|  {task.TaskId}  {task.TaskType,-16}  {statusIcon} {task.Status,-9}  {task.CreatedAt:MM-dd HH:mm} |");
        }
        sb.AppendLine("+-------------------------------------------------------------------------------+");
        sb.AppendLine();

        var hasFilters = reportParams.DateFrom.HasValue || reportParams.DateTo.HasValue ||
                         reportParams.TaskTypesFilter != null || reportParams.PrioritiesFilter != null;

        if (hasFilters)
        {
            sb.AppendLine("+-------------------------------------------------------------------------------+");
            sb.AppendLine("|                           REPORT FILTERS                                       |");
            sb.AppendLine("+-------------------------------------------------------------------------------+");
            if (reportParams.DateFrom.HasValue)
                sb.AppendLine($"|  Date From:        {reportParams.DateFrom.Value:yyyy-MM-dd HH:mm:ss} UTC{new string(' ', 40)} |");
            if (reportParams.DateTo.HasValue)
                sb.AppendLine($"|  Date To:          {reportParams.DateTo.Value:yyyy-MM-dd HH:mm:ss} UTC{new string(' ', 40)} |");
            if (reportParams.TaskTypesFilter != null && reportParams.TaskTypesFilter.Any())
                sb.AppendLine($"|  Task Types:       {string.Join(", ", reportParams.TaskTypesFilter),-57} |");
            if (reportParams.PrioritiesFilter != null && reportParams.PrioritiesFilter.Any())
                sb.AppendLine($"|  Priorities:       {string.Join(", ", reportParams.PrioritiesFilter),-57} |");
            sb.AppendLine($"|  Recent Tasks:     {reportParams.RecentTasksCount}{new string(' ', 64)} |");
            sb.AppendLine("+-------------------------------------------------------------------------------+");
            sb.AppendLine();
        }

        if (!string.IsNullOrEmpty(record.Payload))
        {
            sb.AppendLine("+-------------------------------------------------------------------------------+");
            sb.AppendLine("|                           RAW PAYLOAD DATA                                    |");
            sb.AppendLine("+-------------------------------------------------------------------------------+");
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(record.Payload);
                var formatted = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
                foreach (var line in formatted.Split('\n'))
                {
                    sb.AppendLine($"|  {line,-76} |");
                }
            }
            catch
            {
                sb.AppendLine($"|  {record.Payload,-76} |");
            }
            sb.AppendLine("+-------------------------------------------------------------------------------+");
            sb.AppendLine();
        }

        sb.AppendLine("+===============================================================================+");
        sb.AppendLine("|                 Report generated by TaskFlow System                          |");
        sb.AppendLine($"|                      {now:yyyy-MM-dd HH:mm:ss} UTC                               |");
        sb.AppendLine("+===============================================================================+");

        return sb.ToString();
    }

    private class ReportParameters
    {
        public int RecentTasksCount { get; set; } = 10;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<string>? TaskTypesFilter { get; set; }
        public List<string>? PrioritiesFilter { get; set; }
    }

    private class ExecutionStatistics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int FailedTasks { get; set; }
        public int RunningTasks { get; set; }
        public int QueuedTasks { get; set; }
        public int CancelledTasks { get; set; }
        public Dictionary<string, int> TasksByType { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        public double AverageExecutionTimeMs { get; set; }
    }
}
