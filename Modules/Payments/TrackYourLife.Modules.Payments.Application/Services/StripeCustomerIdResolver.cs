using MassTransit;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Services;

internal sealed class StripeCustomerIdResolver(IStripeService stripeService, IBus bus)
    : IStripeCustomerIdResolver
{
    public async Task<Result<string>> ResolveAndPersistAsync(
        UserId userId,
        string? existingCustomerId,
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var customerId = await stripeService.GetOrCreateCustomerIdAsync(
            existingCustomerId,
            email,
            email,
            cancellationToken
        );

        if (!string.Equals(existingCustomerId, customerId, StringComparison.Ordinal))
        {
            var client = bus.CreateRequestClient<SetStripeCustomerIdRequest>();
            var response = await client.GetResponse<SetStripeCustomerIdResponse>(
                new SetStripeCustomerIdRequest(userId, customerId),
                cancellationToken
            );

            if (response.Message.Errors.Count > 0)
            {
                return Result.Failure<string>(response.Message.Errors[0]);
            }
        }

        return Result.Success(customerId);
    }
}
