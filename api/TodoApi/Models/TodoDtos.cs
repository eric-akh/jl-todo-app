using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public sealed class CreateTodoRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 3, ErrorMessage = "Priority must be 1 (Low), 2 (Medium), or 3 (High).")]
    public int Priority { get; set; } = 2;

    public DateTimeOffset? DueAt { get; set; }
}

public sealed class TodoResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public int Priority { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? DueAt { get; init; }
}