using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;
using TodoApi.Models;
using TodoApi.Services;

static IResult Validate<T>(T model)
{
    var context = new ValidationContext(model!);
    var results = new List<ValidationResult>();

    if (Validator.TryValidateObject(model!, context, results, validateAllProperties: true))
        return Results.Ok();

    var errors = results
        .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
        .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage ?? "Invalid").ToArray());

    return Results.ValidationProblem(errors);
}

static TodoResponse ToResponse(TodoItem x) => new()
{
    Id = x.Id,
    Title = x.Title,
    IsCompleted = x.IsCompleted,
    Priority = (int)x.Priority,
    CreatedAt = x.CreatedAt,
    DueAt = x.DueAt
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TodoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApi", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/todos", (TodoService service) =>
{
    return Results.Ok(service.GetAll().Select(ToResponse));
});

app.MapPost("/api/todos", (CreateTodoRequest request, TodoService service) =>
{
    // DataAnnotations validation (includes DueAt required)
    var validation = Validate(request);
    if (validation is not Ok)
        return validation;

    // Extra guardrails (defense in depth)
    if (!Enum.IsDefined(typeof(TodoPriority), request.Priority))
        return Results.BadRequest(new { message = "Priority must be 1, 2, or 3." });

    try
    {
        var created = service.Add(request);
        return Results.Created($"/api/todos/{created.Id}", ToResponse(created));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.MapDelete("/api/todos/{id:guid}", (TodoService service, Guid id) =>
{
    // IMPORTANT: If not found, return 404 (fixes your failing tests)
    var removed = service.Delete(id);
    return removed ? Results.NoContent() : Results.NotFound(new { message = "Todo not found." });
});

// PATCH update (this fixes your MethodNotAllowed integration tests)
app.MapPatch("/api/todos/{id:guid}", (TodoService service, Guid id, UpdateTodoRequest request) =>
{
    var validation = Validate(request);
    if (validation is not Ok)
        return validation;

    if (!Enum.IsDefined(typeof(TodoPriority), request.Priority))
        return Results.BadRequest(new { message = "Priority must be 1, 2, or 3." });

    try
    {
        var ok = service.Update(id, request, out var updated);
        return ok ? Results.Ok(ToResponse(updated)) : Results.NotFound(new { message = "Todo not found." });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

// Keep toggle endpoint (handy for UI)
app.MapPatch("/api/todos/{id:guid}/toggle", (TodoService service, Guid id) =>
{
    var ok = service.ToggleComplete(id);
    return ok ? Results.NoContent() : Results.NotFound(new { message = "Todo not found." });
});

app.Run();
