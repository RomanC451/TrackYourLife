using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;

internal sealed class UpdateProSubscriptionPeriodEndCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateProSubscriptionPeriodEndCommand>
{
    public async Task<Result> Handle(
        UpdateProSubscriptionPeriodEndCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        var result = user.UpdateProSubscriptionPeriodEnd(
            command.PeriodEndUtc,
            command.SubscriptionStatus,
            command.CancelAtPeriodEnd
        );
        if (result.IsFailure)
        {
            return result;
        }

        userRepository.Update(user);
        return Result.Success();
    }
}
