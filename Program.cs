using Ardalis.Result;
using Ardalis.Result.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", (HttpResponse httpResponse) => httpResponse.Redirect("/swagger"));

app.MapGet("/forecasts", (WeatherService weatherService) => weatherService.List().ToMinimalApiResult());
//.WithTags("WeatherForecasts");

app.MapPost("/forecasts", (WeatherForecastDTO request, WeatherService weatherService) => 
    weatherService.Create(request.Date, request.TemperatureC, request.Summary)
    .ToMinimalApiResult());

app.Run();

public class WeatherForecast
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public int TemperatureC { get; init; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Summary { get; init; } = "";
}

public class WeatherForecastDTO
{
    public DateTime Date { get; init; }
    public int TemperatureC { get; init; }
    public string Summary { get; init; } = "";
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

