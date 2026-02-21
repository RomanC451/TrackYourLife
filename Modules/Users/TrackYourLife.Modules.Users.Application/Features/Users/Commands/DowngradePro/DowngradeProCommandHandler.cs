using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.DowngradePro;

internal sealed class DowngradeProCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DowngradeProCommand>
{
    public async Task<Result> Handle(
        DowngradeProCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        user.ClearProSubscription(command.SubscriptionStatus);
        userRepository.Update(user);

        return Result.Success();
    }
}
