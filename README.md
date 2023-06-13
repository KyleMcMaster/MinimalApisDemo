# MinimalApisDemo

A demo for Minimal APIs in .NET 7

## Initial Setup

### Creating this project 

```powershell
dotnet new web -o MinimalApisDemo
```

### Trusting the development certificate

```powershell
dotnet dev-certs https --trust
```

### Running this project

```powershell
dotnet run --launch-profile https
```

## Getting Started

### Adding some dependencies

```powershell 
dotnet add package Swashbuckle.AspNetCore
dotnet add package MinimalApis.Extensions
```

### Add some services and message body

```csharp
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
```