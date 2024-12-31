using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;

public sealed class RemoveCurrentUserCommandHandler(
    IUserRepository userRepository,
    IUsersUnitOfWork unitOfWork,
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
