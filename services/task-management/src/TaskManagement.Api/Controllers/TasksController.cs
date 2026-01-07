using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Tasks.Commands.CancelTask;
using TaskManagement.Application.Tasks.Commands.CreateTask;
using TaskManagement.Application.Tasks.Queries.GetTask;
using TaskManagement.Application.Tasks.Queries.GetUserTasks;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand
        {
            UserId = request.UserId,
            TaskType = request.TaskType,
            Priority = request.Priority,
            Payload = request.Payload,
            ScheduledAt = request.ScheduledAt
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetTask), new { taskId = result.Value!.TaskId }, result.Value);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetTask([FromRoute] Guid taskId, [FromQuery] string userId, CancellationToken cancellationToken)
    {
        var query = new GetTaskQuery
        {
            TaskId = taskId,
            UserId = userId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserTasks([FromRoute] string userId, CancellationToken cancellationToken)
    {
        var query = new GetUserTasksQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{taskId:guid}/cancel")]
    public async Task<IActionResult> CancelTask([FromRoute] Guid taskId, [FromBody] CancelTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CancelTaskCommand
        {
            TaskId = taskId,
            UserId = request.UserId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}

public record CreateTaskRequest
{
    public string UserId { get; init; } = string.Empty;
    public string TaskType { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string? Payload { get; init; }
    public DateTime? ScheduledAt { get; init; }
}

public record CancelTaskRequest
{
    public string UserId { get; init; } = string.Empty;
}


