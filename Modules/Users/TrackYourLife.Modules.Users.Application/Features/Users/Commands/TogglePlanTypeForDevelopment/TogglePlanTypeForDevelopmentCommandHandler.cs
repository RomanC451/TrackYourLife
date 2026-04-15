using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.TogglePlanTypeForDevelopment;

internal sealed class TogglePlanTypeForDevelopmentCommandHandler(
    IUserRepository userRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<TogglePlanTypeForDevelopmentCommand>
{
    public async Task<Result> Handle(
        TogglePlanTypeForDevelopmentCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await userRepository.GetByIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure<PlanType>(UserErrors.NotFound(userIdentifierProvider.UserId));
        }

        user.TogglePlanTypeForDevelopment();
        userRepository.Update(user);

        return Result.Success();
    }
}
