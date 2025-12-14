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

static IResult NotFoundProblem(string detail)
{
    return Results.Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Not Found",
        detail: detail);
}

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
    return Results.Ok(service.GetAll().Select(x => new TodoResponse
    {
        Id = x.Id,
        Title = x.Title,
        IsCompleted = x.IsCompleted,
        Priority = (int)x.Priority,
        CreatedAt = x.CreatedAt,
        DueAt = x.DueAt
    }));
});

app.MapPost("/api/todos", (CreateTodoRequest request, TodoService service) =>
{
    var validation = Validate(request);
    if (validation is not Ok)
        return validation;

    try
    {
        var created = service.Add(request);

        return Results.Created($"/api/todos/{created.Id}", new TodoResponse
        {
            Id = created.Id,
            Title = created.Title,
            IsCompleted = created.IsCompleted,
            Priority = (int)created.Priority,
            CreatedAt = created.CreatedAt,
            DueAt = created.DueAt
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.MapDelete("/api/todos/{id:guid}", (TodoService service, Guid id) =>
{
    var removed = service.Delete(id);

    if (!removed)
        return NotFoundProblem($"Todo item with id '{id}' was not found.");

    return Results.NoContent();
});

app.MapPatch("/api/todos/{id:guid}/toggle", (TodoService service, Guid id) =>
{
    var ok = service.ToggleComplete(id);

    if (!ok)
        return NotFoundProblem($"Todo item with id '{id}' was not found.");

    return Results.NoContent();
});

app.Run();

public partial class Program { }