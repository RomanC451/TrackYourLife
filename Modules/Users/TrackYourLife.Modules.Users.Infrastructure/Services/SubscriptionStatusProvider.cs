using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

internal sealed class SubscriptionStatusProvider(
    IUserIdentifierProvider userIdentifierProvider,
    IUserQuery userQuery,
    IDateTimeProvider dateTimeProvider
) : ISubscriptionStatusProvider
{
    public async Task<bool> IsProAsync(CancellationToken cancellationToken = default)
    {
        var user = await userQuery.GetByIdAsync(userIdentifierProvider.UserId, cancellationToken);

        if (user == null)
        {
            return false;
        }

        if (user.PlanType != PlanType.Pro || user.SubscriptionEndsAtUtc == null)
        {
            return false;
        }

        return user.SubscriptionEndsAtUtc.Value > dateTimeProvider.UtcNow;
    }
}
