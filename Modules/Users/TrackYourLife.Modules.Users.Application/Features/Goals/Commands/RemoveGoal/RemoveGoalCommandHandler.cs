using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.RemoveGoal;

public sealed class RemoveGoalCommandHandler(
    IGoalRepository userGoalRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<RemoveGoalCommand>
{
    public async Task<Result> Handle(RemoveGoalCommand command, CancellationToken cancellationToken)
    {
        var userGoal = await userGoalRepository.GetByIdAsync(command.Id, cancellationToken);

        if (userGoal is null)
        {
            return Result.Failure(GoalErrors.NotFound(command.Id));
        }

        if (userGoal.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(GoalErrors.NotOwned(command.Id));
        }

        userGoalRepository.Remove(userGoal);

        return Result.Success();
    }
}
