using TrackYourLifeDotnet.Application.WeatherForecasts.Queries;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.WeatherForecasts.Queries.GetWeatherForecast;

public sealed record GetWeatherForecastQuery : IQuery<IEnumerable<GetWeatherForecastResponse>>;
