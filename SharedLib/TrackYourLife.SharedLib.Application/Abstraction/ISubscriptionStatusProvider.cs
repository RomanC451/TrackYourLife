namespace TrackYourLife.SharedLib.Application.Abstraction;

public interface ISubscriptionStatusProvider
{
    Task<bool> IsProAsync(CancellationToken cancellationToken = default);
}
