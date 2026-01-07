using Microsoft.EntityFrameworkCore;
using TaskExecution.Domain.Entities;
using TaskFlow.Infrastructure.Idempotency;

namespace TaskExecution.Infrastructure.Persistence;

public class TaskExecutionDbContext : DbContext
{
    public TaskExecutionDbContext(DbContextOptions<TaskExecutionDbContext> options) : base(options)
    {
    }

    public DbSet<ExecutionRecord> ExecutionRecords => Set<ExecutionRecord>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ExecutionRecord>(entity =>
        {
            entity.ToTable("execution_records");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.TaskId)
                .HasColumnName("task_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.TaskType)
                .HasColumnName("task_type")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Priority)
                .HasColumnName("priority")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Payload)
                .HasColumnName("payload")
                .HasMaxLength(10000);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ProgressPercent)
                .HasColumnName("progress_percent")
                .HasDefaultValue(0);

            entity.Property(e => e.StatusMessage)
                .HasColumnName("status_message")
                .HasMaxLength(500);

            entity.Property(e => e.ResultLocation)
                .HasColumnName("result_location")
                .HasMaxLength(1000);

            entity.Property(e => e.ErrorMessage)
                .HasColumnName("error_message")
                .HasMaxLength(2000);

            entity.Property(e => e.RetryCount)
                .HasColumnName("retry_count")
                .HasDefaultValue(0);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.StartedAt)
                .HasColumnName("started_at");

            entity.Property(e => e.CompletedAt)
                .HasColumnName("completed_at");

            entity.Property(e => e.CorrelationId)
                .HasColumnName("correlation_id")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(e => e.TaskId)
                .HasDatabaseName("idx_execution_task_id")
                .IsUnique();

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("idx_execution_status");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("idx_execution_created_at");
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.ToTable("processed_events");

            entity.HasKey(e => new { e.EventId, e.ConsumerName });

            entity.Property(e => e.EventId)
                .HasColumnName("event_id");

            entity.Property(e => e.ConsumerName)
                .HasColumnName("consumer_name")
                .HasMaxLength(500);

            entity.Property(e => e.ProcessedAt)
                .HasColumnName("processed_at")
                .IsRequired();
        });
    }
}

