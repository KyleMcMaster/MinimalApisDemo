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

app.MapGet("/forecasts", () => "Hello World!")
.WithTags("HelloWorld");

app.MapPost("/", (World request) => $"Hello {request.Name}!")
.WithTags("HelloWorld");

app.Run();
```

## Using Ardalis.Result in Minimal APIs

### Adding some dependencies

```powershell 
dotnet add package Ardalis.Result
dotnet add package Ardalis.Result.AspNetCore
```

### New models and a service!  

```csharp
public class WeatherForecast
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Summary { get; set; } = "";
}

public class WeatherService
{
    private static List<WeatherForecast> _forecasts = new()
    {
        new() { Id = 0, Date = DateTime.Now, TemperatureC = 12, Summary = "Cool" },
        new() { Id = 1, Date = DateTime.Now.AddDays(1), TemperatureC = 21, Summary = "Warm" },
        new() { Id = 2, Date = DateTime.Now.AddDays(2), TemperatureC = 19, Summary = "Warm" },
        new() { Id = 3, Date = DateTime.Now.AddDays(3), TemperatureC = 32, Summary = "Hot" },
    };

    public Result<WeatherForecast> Create(DateTime date, int temperatureC, string summary)
    {
        if (_forecasts.Any(f => f.Date.Date == date.Date))
        {
            return Result.Conflict("A forecast already exists for that date.");
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            var validationErrors = new List<ValidationError>
            {
                new ValidationError
                {
                    Identifier = nameof(WeatherForecast.Summary),
                    ErrorMessage = "Summary is required."
                }
            };
            return Result.Invalid(validationErrors);
        }
        
        var forecast = new WeatherForecast
        {
            Id = _forecasts.Max(f => f.Id) + 1,
            Date = date,
            TemperatureC = temperatureC,
            Summary = summary
        };

        _forecasts.Add(forecast);

        return forecast; // Result.Success
    }

    public Result<IEnumerable<WeatherForecast>> List()
    {
        return _forecasts; // Always Successful
    }
}
```

### GetById
```csharp
app.MapGet("/forecasts/{id}", (int id, WeatherService weatherService) =>
    WeatherForecastEndpoints.GetById(id, weatherService))
    .WithTags("WeatherForecasts");

public Result<WeatherForecast> GetById(int id)
{
    var forecast = _forecasts.SingleOrDefault(f => f.Id == id);

    if (forecast is null)
    {
        return Result.NotFound();
    }

    return forecast;
}

public static class WeatherForecastEndpoints
{
    public static Func<WeatherService, int, Microsoft.AspNetCore.Http.IResult> GetWeatherForecastById = (weatherService, id) =>
        weatherService.GetById(id)
        .ToMinimalApiResult();
}
```

## References

https://github.com/DamianEdwards/MinimalApiPlayground/
https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-7.0&tabs=visual-studio-code