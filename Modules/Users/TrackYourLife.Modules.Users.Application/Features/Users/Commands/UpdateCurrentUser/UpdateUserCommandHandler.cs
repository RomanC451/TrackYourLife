using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;

internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        UserId userId = userIdentifierProvider.UserId;

        User? user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        Result<Name> firstNameResult = Name.Create(command.FirstName);

        Result<Name> lastNameResult = Name.Create(command.LastName);

        var result = Result.FirstFailureOrSuccess(firstNameResult, lastNameResult);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        user.ChangeName(firstNameResult.Value, lastNameResult.Value);

        return Result.Success();
    }
}
