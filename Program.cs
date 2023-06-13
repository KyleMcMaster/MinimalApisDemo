var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!")
.WithTags("HelloWorld");

app.MapPost("/", (World request) => $"Hello {request.Name}!")
.WithTags("HelloWorld");

app.Run();

public class World
{
    public string? Name { get; init; }
}