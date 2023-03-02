using TrackYourLifeDotnet.Application.Abstractions.Messaging;

using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.WeatherForecasts.Queries.GetWeatherForecast;

internal sealed class GetWeatherForecastQueryHandler
    : IQueryHandler<GetWeatherForecastQuery, IEnumerable<GetWeatherForecastResponse>>
{
    private readonly IWeatherForecastRepository _repository;

    public GetWeatherForecastQueryHandler(IWeatherForecastRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<GetWeatherForecastResponse>>> Handle(
        GetWeatherForecastQuery request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<WeatherForecast>? forecast = await _repository.GetForecastAsync(
            5,
            cancellationToken
        );

        if (forecast is null)
        {
            return Result.Failure<IEnumerable<GetWeatherForecastResponse>>(
                DomainErrors.WeatherForecast.Empty
            );
        }

        IEnumerable<GetWeatherForecastResponse> response = forecast.Select(
            x =>
                new GetWeatherForecastResponse(
                    x.Id,
                    x.Date,
                    x.TemperatureC,
                    x.TemperatureF,
                    x.Summary
                )
        );

        return Result.Success(response);
    }
}
