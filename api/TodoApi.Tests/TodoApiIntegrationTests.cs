using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
    public async Task GetTodos_ReturnsOk()
    {
        var res = await _client.GetAsync("/api/todos");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostTodo_WithValidRequest_CreatesTodo()
    {
        var req = new CreateTodoRequest
        {
            Title = "Buy milk",
            Priority = 2,
            DueAt = DateTimeOffset.UtcNow.AddDays(1)
        };

        var res = await _client.PostAsJsonAsync("/api/todos", req);

        res.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await res.Content.ReadFromJsonAsync<TodoResponse>();
        body.Should().NotBeNull();
        body!.Title.Should().Be("Buy milk");
        body.Priority.Should().Be(2);
        body.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task PostTodo_WithInvalidPriority_ReturnsBadRequest()
    {
        var req = new CreateTodoRequest
        {
            Title = "Invalid priority",
            Priority = 4
        };

        var res = await _client.PostAsJsonAsync("/api/todos", req);

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteTodo_WhenNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var res = await _client.DeleteAsync($"/api/todos/{id}");
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ToggleTodo_WhenNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var res = await _client.PatchAsync($"/api/todos/{id}/toggle", content: null);
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
