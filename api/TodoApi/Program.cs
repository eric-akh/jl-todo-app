using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<TodoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - To link with Angular
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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
    return Results.Ok(service.GetAll());
});

app.MapPost("/api/todos", (TodoService service, TodoItem request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest("Title is required");

    var created = service.Add(request.Title);
    return Results.Created($"/api/todos/{created.Id}", created);
});

app.MapDelete("/api/todos/{id:int}", (TodoService service, int id) =>
{
    var deleted = service.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();