using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests;

public sealed class TodoServiceTests
{
    [Fact]
    public void Add_ShouldCreateTodo_WhenRequestIsValid()
    {
        var service = new TodoService();
        var due = DateTimeOffset.UtcNow.AddDays(3);

        var created = service.Add(new CreateTodoRequest
        {
            Title = "Pay bills",
            Priority = (int)TodoPriority.High,
            DueAt = due
        });

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Pay bills", created.Title);
        Assert.Equal(TodoPriority.High, created.Priority);
        Assert.Equal(due, created.DueAt);
        Assert.False(created.IsCompleted);
    }

    [Fact]
    public void Patch_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var service = new TodoService();
        var updated = new TodoItem();

        var patched = service.Update(Guid.NewGuid(), new UpdateTodoRequest { Title = "X" }, out updated);

        Assert.Null(updated);
    }
}
