using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.RemoveUserGoal;

public sealed class RemoveUserGoalCommandHandler(IUserGoalRepository userGoalRepository, IUserIdentifierProvider userIdentifierProvider) : ICommandHandler<RemoveUserGoalCommand>
{
    private readonly IUserGoalRepository _userGoalRepository = userGoalRepository;

    private readonly IUserIdentifierProvider _userIdentifierProvider = userIdentifierProvider;

    public async Task<Result> Handle(
        RemoveUserGoalCommand command,
        CancellationToken cancellationToken
    )
    {
        var userGoal = await _userGoalRepository.GetByIdAsync(command.Id, cancellationToken);

        if (userGoal is null)
        {
            return Result.Failure(UserGoalErrors.NotFound(command.Id));
        }

        if (userGoal.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(UserGoalErrors.NotOwned(command.Id));
        }

        _userGoalRepository.Remove(userGoal);

        return Result.Success();
    }
}
