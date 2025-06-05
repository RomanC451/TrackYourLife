namespace TrackYourLife.SharedLib.Application.Abstraction;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
