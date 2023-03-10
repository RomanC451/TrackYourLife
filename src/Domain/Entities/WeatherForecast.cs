using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.Entities;

public class WeatherForecast : AggregateRoot
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
