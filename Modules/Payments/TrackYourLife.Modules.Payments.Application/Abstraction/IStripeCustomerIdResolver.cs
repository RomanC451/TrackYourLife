using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Abstraction;

public interface IStripeCustomerIdResolver
{
    Task<Result<string>> ResolveAndPersistAsync(
        UserId userId,
        string? existingCustomerId,
        string email,
        CancellationToken cancellationToken = default
    );
}
