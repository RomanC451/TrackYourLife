using TrackYourLifeDotnet.Persistence;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Repositories;

namespace Persistence.Repositories;

internal sealed class WeatherForecastRepository : IWeatherForecastRepository
{
    public readonly ApplicationDbContext _context;

    public WeatherForecastRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WeatherForecast>> GetForecastAsync(
        int entriesCount,
        CancellationToken cancellationToken
    )
    {
        IQueryable<WeatherForecast>? query = _context.WeatherForecasts.Take(entriesCount);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<WeatherForecast?> GetForecastByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    ) => _context.WeatherForecasts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Add(WeatherForecast weatherForecast)
    {
        _context.WeatherForecasts.Add(weatherForecast);
    }

    public void Update(WeatherForecast weatherForecast)
    {
        _context.WeatherForecasts.Update(weatherForecast);
    }

    public void Remove(WeatherForecast weatherForecast)
    {
        _context.WeatherForecasts.Remove(weatherForecast);
    }
}
