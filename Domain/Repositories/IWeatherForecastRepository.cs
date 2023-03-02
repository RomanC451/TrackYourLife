using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Domain.Repositories;

public interface IWeatherForecastRepository
{
    Task<List<WeatherForecast>> GetForecastAsync(
        int entriesCount,
        CancellationToken cancellationToken
    );

    Task<WeatherForecast?> GetForecastByIdAsync(Guid id, CancellationToken cancellationToken);

    void Add(WeatherForecast weatherForecast);

    void Update(WeatherForecast weatherForecast);

    void Remove(WeatherForecast weatherForecast);
}
