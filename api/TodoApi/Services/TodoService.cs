using System.Collections.Concurrent;
using TodoApi.Models;

namespace TodoApi.Services;

public sealed class TodoService
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    public IReadOnlyCollection<TodoItem> GetAll()
        => _items.Values
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

    public TodoItem Add(CreateTodoRequest request)
    {
        var title = (request.Title ?? string.Empty).Trim();
        if (title.Length == 0)
            throw new ArgumentException("Title is required.");
        if (title.Length > 200)
            throw new ArgumentException("Title must be 200 characters or less.");

        if (!Enum.IsDefined(typeof(TodoPriority), request.Priority))
            throw new ArgumentException("Priority must be 1, 2, or 3.");

        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsCompleted = false,
            CreatedAt = DateTimeOffset.UtcNow,
            DueAt = request.DueAt,
            Priority = (TodoPriority)request.Priority
        };

        _items[item.Id] = item;
        return item;
    }

    public bool Delete(Guid id)
        => _items.TryRemove(id, out _);

    public bool ToggleComplete(Guid id)
    {
        if (!_items.TryGetValue(id, out var existing))
            return false;

        existing.IsCompleted = !existing.IsCompleted;
        return true;
    }

    // PATCH /api/todos/{id} support (fixes your 405 MethodNotAllowed tests)
    public bool Update(Guid id, UpdateTodoRequest request, out TodoItem updated)
    {
        updated = default!;

        if (!_items.TryGetValue(id, out var existing))
            return false;

        var title = (request.Title ?? string.Empty).Trim();
        if (title.Length == 0)
            throw new ArgumentException("Title is required.");
        if (title.Length > 200)
            throw new ArgumentException("Title must be 200 characters or less.");

        if (!Enum.IsDefined(typeof(TodoPriority), request.Priority))
            throw new ArgumentException("Priority must be 1, 2, or 3.");

        // Update in place (in-memory store)
        existing.Title = title;
        existing.Priority = (TodoPriority)request.Priority;
        existing.IsCompleted = request.IsCompleted;

        updated = existing;
        return true;
    }
}
