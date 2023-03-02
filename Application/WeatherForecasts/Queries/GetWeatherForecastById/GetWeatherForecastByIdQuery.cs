using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.WeatherForecasts.Queries.GetWeatherForecastById;

public sealed record GetWeatherForecastByIdQuery(Guid ForecastId)
    : IQuery<GetWeatherForecastResponse>;
