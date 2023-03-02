using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Errors;

namespace TrackYourLifeDotnet.Application.WeatherForecasts.Queries.GetWeatherForecastById;

public class GetWeatherForecastByIdQueryHandler
    : IQueryHandler<GetWeatherForecastByIdQuery, GetWeatherForecastResponse>
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public GetWeatherForecastByIdQueryHandler(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    public async Task<Result<GetWeatherForecastResponse>> Handle(
        GetWeatherForecastByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        WeatherForecast? forecast = await _weatherForecastRepository.GetForecastByIdAsync(
            request.ForecastId,
            cancellationToken
        );

        if (forecast is null)
        {
            return Result.Failure<GetWeatherForecastResponse>(
                DomainErrors.WeatherForecast.NotFound
            );
        }

        var response = new GetWeatherForecastResponse(
            forecast.Id,
            forecast.Date,
            forecast.TemperatureC,
            forecast.TemperatureF,
            forecast.Summary
        );

        return Result.Success(response);
    }
}
