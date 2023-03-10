using MediatR;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Repositories;

namespace Application.WeatherForecasts.CreateWeatherForecast;

public class CreateWeatherForecastHandler : IRequestHandler<CreateWeatherForecastCommand>
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWeatherForecastHandler(
        IWeatherForecastRepository weatherForecastRepository,
        IUnitOfWork unitOfWork
    )
    {
        _weatherForecastRepository = weatherForecastRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        CreateWeatherForecastCommand request,
        CancellationToken cancellationToken
    )
    {
        WeatherForecast weatherForecast =
            new()
            {
                Date = request.Date,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

        _weatherForecastRepository.Add(weatherForecast);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
