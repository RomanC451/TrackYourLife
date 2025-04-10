using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;

internal sealed class UpdateGoalCommandHandler(
    IGoalRepository userGoalRepository,
    IUserIdentifierProvider userIdentifierProvider,
    IGoalsManagerService goalsManagerService
) : ICommandHandler<UpdateGoalCommand>
{
    public async Task<Result> Handle(UpdateGoalCommand command, CancellationToken cancellationToken)
    {
        Goal? newUserGoal = await userGoalRepository.GetByIdAsync(command.Id, cancellationToken);

        if (newUserGoal is null)
        {
            return Result.Failure(GoalErrors.NotFound(command.Id));
        }

        if (newUserGoal.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(GoalErrors.NotOwned(command.Id));
        }

        var result = Result.FirstFailureOrSuccess(
            newUserGoal.UpdateType(command.Type),
            newUserGoal.UpdatePerPeriod(command.PerPeriod),
            newUserGoal.UpdateValue(command.Value),
            newUserGoal.UpdateStartDate(command.StartDate),
            newUserGoal.UpdateEndDate(command.EndDate ?? DateOnly.MaxValue)
        );

        if (result.IsFailure)
            return result;

        result = await goalsManagerService.HandleOverlappingGoalsAsync(
            newUserGoal,
            command.Force,
            cancellationToken
        );
        if (result.IsFailure)
            return result;

        userGoalRepository.Update(newUserGoal);
        return Result.Success();
    }
}
