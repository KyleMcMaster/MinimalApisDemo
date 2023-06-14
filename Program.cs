using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MinimalApis.Extensions.Results;

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

app.MapGet("/", WeatherForecastViews.Index);

app.MapWeatherForecastEndpoints();

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
        new() { Id = 1, Date = DateTime.Now.AddDays(1), TemperatureC = 22, Summary = "Warm" },
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

        return Result.Success(forecast);
    }

    public Result<IEnumerable<WeatherForecast>> List()
    {
        return _forecasts; // Always Successful
    }


    public Result<WeatherForecast> GetById(int id)
    {
        var forecast = _forecasts.SingleOrDefault(f => f.Id == id);

        if (forecast is null)
        {
            return Result.NotFound();
        }

        return forecast;
    }
}

public static class WeatherForecastEndpoints
{
    private static Func<int, WeatherService, Microsoft.AspNetCore.Http.IResult> GetById = 
        (int id, WeatherService weatherService) =>
        weatherService.GetById(id)
        .ToMinimalApiResult();

    public static WebApplication MapWeatherForecastEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("forecasts")
            .WithTags("WeatherForecasts");

        group.MapGet("", (WeatherService weatherService) => weatherService.List().ToMinimalApiResult());
            
        group.MapGet("/{id}", (int id, WeatherService weatherService) => GetById(id, weatherService));

        group.MapPost("", (WeatherForecastDTO request, WeatherService weatherService) =>
            weatherService.Create(request.Date, request.TemperatureC, request.Summary)
            .ToMinimalApiResult());

        return app;
    }
}

public static class WeatherForecastViews
{
    public static Func<WeatherService, Microsoft.AspNetCore.Http.IResult> Index = (WeatherService weatherService) =>
    {
        var forecast = weatherService.List().Value.First();
        string rawHtml = File.ReadAllText("index.html");
        string today = forecast.Date.ToShortDateString();
        string celsius = forecast.TemperatureC.ToString();
        string fahrenheit = forecast.TemperatureF.ToString();
        string rendered = rawHtml.Replace("{today}", today)
            .Replace("{celsius}", celsius)
            .Replace("{fahrenheit}", fahrenheit)
            .Replace("{summary}", forecast.Summary);
        return Results.Extensions.Html(rendered);
    };
}