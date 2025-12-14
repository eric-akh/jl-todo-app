using TodoApi.Models;
using TodoApi.Services;
using Xunit;

public sealed class TodoServiceTests
{
    [Fact]
    public void Add_ShouldCreateItem_WhenRequestIsValid()
    {
        var service = new TodoService();

        var req = new CreateTodoRequest
        {
            Title = "Buy milk",
            Priority = 2,
            DueAt = DateTimeOffset.UtcNow.AddDays(1)
        };

        var created = service.Add(req);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Buy milk", created.Title);
        Assert.False(created.IsCompleted);
        Assert.Equal(TodoPriority.Medium, created.Priority);
        Assert.NotNull(created.DueAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_ShouldThrow_WhenTitleIsEmpty(string title)
    {
        var service = new TodoService();

        var req = new CreateTodoRequest
        {
            Title = title,
            Priority = 2
        };

        var ex = Assert.Throws<ArgumentException>(() => service.Add(req));
        Assert.Contains("Title is required", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(999)]
    public void Add_ShouldThrow_WhenPriorityIsInvalid(int priority)
    {
        var service = new TodoService();

        var req = new CreateTodoRequest
        {
            Title = "Test",
            Priority = priority
        };

        var ex = Assert.Throws<ArgumentException>(() => service.Add(req));
        Assert.Contains("Priority must be", ex.Message);
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        var service = new TodoService();

        var ok = service.Delete(Guid.NewGuid());

        Assert.False(ok);
    }

    [Fact]
    public void ToggleComplete_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        var service = new TodoService();

        var ok = service.ToggleComplete(Guid.NewGuid());

        Assert.False(ok);
    }
}
