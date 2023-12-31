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

## Talk outline 

### Getting Started 

- Show basic endpoints and layout of program.cs
- Add packages and enable swagger

### Ardalis.Result Mapping to Microsoft.AspNetCore.Http.Result

- Review Result Pattern and problem it solves
- Add Ardalis.Result package
- Show switch mapping to Microsoft.AspNetCore.Http.Result
- Demo two endpoints, very simple extensions, duplicate swagger attributes
- Add getById endpoint, show static delegate (method group), show variable local function

### Experiments with grouping, extensions, html, and other fun

- Show extension for registering endpoints, grouping
- What really is going on? string and delegate is all that is needed
- Producing other content types
- Bootstrap example

### Takeaways 

- How might we use this in our projects?
- What are the downsides?
- Still feels like new terrain to explore, similar patterns but different structure
- What are the implications for testing?
- Any questions?