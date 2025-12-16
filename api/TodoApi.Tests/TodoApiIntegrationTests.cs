using System.Net;
using System.Net.Http.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public sealed class TodoApiIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public TodoApiIntegrationTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_ShouldReturnOk()
    {
        var res = await _client.GetAsync("/api/todos");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var todos = await res.Content.ReadFromJsonAsync<List<TodoResponse>>();
        Assert.NotNull(todos);
    }

    [Fact]
    public async Task PostTodo_ThenDelete_ShouldWork()
    {
        var due = DateTimeOffset.UtcNow.AddDays(2);

        var create = new CreateTodoRequest
        {
            Title = "Integration test",
            Priority = (int)TodoPriority.Low,
            DueAt = due
        };

        var postRes = await _client.PostAsJsonAsync("/api/todos", create);
        Assert.Equal(HttpStatusCode.Created, postRes.StatusCode);

        var created = await postRes.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.Equal(due, created.DueAt);

        var delRes = await _client.DeleteAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delRes.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenNotFound()
    {
        var res = await _client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
