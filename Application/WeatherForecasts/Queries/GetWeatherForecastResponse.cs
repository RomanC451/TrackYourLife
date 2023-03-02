namespace TrackYourLifeDotnet.Application.WeatherForecasts.Queries;

public sealed record GetWeatherForecastResponse(
    Guid Id,
    DateOnly Date,
    int TemperatureC,
    int TemperatureF,
    string? Summary
);
