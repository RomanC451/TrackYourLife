using MediatR;

namespace Application.WeatherForecasts.CreateWeatherForecast;

public sealed record CreateWeatherForecastCommand(DateOnly Date, int TemperatureC, string? Summary)
    : IRequest;
