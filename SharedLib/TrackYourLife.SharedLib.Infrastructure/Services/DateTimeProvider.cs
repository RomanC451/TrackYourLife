using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.SharedLib.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
