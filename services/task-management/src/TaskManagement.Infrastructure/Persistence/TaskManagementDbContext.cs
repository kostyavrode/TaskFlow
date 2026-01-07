using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Payload)
                .HasColumnName("payload")
                .HasMaxLength(10000);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.ScheduledAt)
                .HasColumnName("scheduled_at");

            entity.Property(e => e.Type)
                .HasColumnName("task_type")
                .HasConversion(
                    v => v.Name,
                    v => Domain.ValueObjects.TaskType.FromString(v))
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Priority)
                .HasColumnName("priority")
                .HasConversion(
                    v => v.Name,
                    v => Domain.ValueObjects.Priority.FromString(v))
                .HasMaxLength(50)
                .IsRequired();

            entity.Ignore(e => e.DomainEvents);

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("idx_tasks_user_id");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("idx_tasks_status");

            entity.HasIndex(e => e.ScheduledAt)
                .HasDatabaseName("idx_tasks_scheduled_at");
        });
    }
}


