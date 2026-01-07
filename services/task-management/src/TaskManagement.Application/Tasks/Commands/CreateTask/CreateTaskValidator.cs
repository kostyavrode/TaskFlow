using FluentValidation;

namespace TaskManagement.Application.Tasks.Commands.CreateTask;

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.TaskType)
            .NotEmpty()
            .WithMessage("TaskType is required")
            .Must(BeValidTaskType)
            .WithMessage("Invalid task type. Allowed: Report, Email, DataProcessing, Notification, Backup");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Priority is required")
            .Must(BeValidPriority)
            .WithMessage("Invalid priority. Allowed: Low, Medium, High, Critical");

        RuleFor(x => x.Payload)
            .MaximumLength(10000)
            .WithMessage("Payload is too large");

        RuleFor(x => x.ScheduledAt)
            .Must(BeInFuture)
            .When(x => x.ScheduledAt.HasValue)
            .WithMessage("ScheduledAt must be in the future");
    }

    private bool BeValidTaskType(string taskType)
    {
        var validTypes = new[] { "report", "email", "dataprocessing", "notification", "backup" };
        return validTypes.Contains(taskType?.ToLowerInvariant());
    }

    private bool BeValidPriority(string priority)
    {
        var validPriorities = new[] { "low", "medium", "high", "critical" };
        return validPriorities.Contains(priority?.ToLowerInvariant());
    }

    private bool BeInFuture(DateTime? scheduledAt)
    {
        return !scheduledAt.HasValue || scheduledAt.Value > DateTime.UtcNow;
    }
}


