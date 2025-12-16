using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public sealed class CreateTodoRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 3)]
    public int Priority { get; set; } = 2;

    [Required]
    public DateTimeOffset DueAt { get; set; }
}

public sealed class UpdateTodoRequest
{
    // the client sends the full payload we care about.
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 3)]
    public int Priority { get; set; } = 2;

    public bool IsCompleted { get; set; }
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
