using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;

internal sealed class RemoveCurrentUserCommandHandler(
    IUserRepository userRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<RemoveCurrentUserCommand>
{
    public async Task<Result> Handle(
        RemoveCurrentUserCommand _,
        CancellationToken cancellationToken
    )
    {
        User? user = await userRepository.GetByIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userIdentifierProvider.UserId));
        }

        userRepository.Remove(user);

        return Result.Success();
    }
}
