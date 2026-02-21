using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpgradeToPro;

internal sealed class UpgradeToProCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpgradeToProCommand>
{
    public async Task<Result> Handle(
        UpgradeToProCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        user.SetProSubscription(
            command.StripeCustomerId,
            command.PeriodEndUtc,
            SubscriptionStatus.Active,
            command.CancelAtPeriodEnd
        );
        userRepository.Update(user);

        return Result.Success();
    }
}
