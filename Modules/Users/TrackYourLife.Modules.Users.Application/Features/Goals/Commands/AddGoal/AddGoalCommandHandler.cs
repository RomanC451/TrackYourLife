using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;

public sealed class AddGoalCommandHandler(
    IGoalRepository userGoalRepository,
    IUserIdentifierProvider userIdentifierProvider,
    IGoalsManagerService goalsManagerService
) : ICommandHandler<AddGoalCommand, GoalId>
{
    public async Task<Result<GoalId>> Handle(
        AddGoalCommand command,
        CancellationToken cancellationToken
    )
    {
        var userGoalResult = Goal.Create(
            id: GoalId.NewId(),
            userId: userIdentifierProvider.UserId,
            type: command.Type,
            value: command.Value,
            perPeriod: command.PerPeriod,
            startDate: command.StartDate,
            endDate: command.EndDate
        );

        if (userGoalResult.IsFailure)
            return Result.Failure<GoalId>(userGoalResult.Error);

        var newUserGoal = userGoalResult.Value;

        var result = await goalsManagerService.HandleOverlappingGoalsAsync(
            newUserGoal,
            command.Force,
            cancellationToken
        );

        if (result.IsFailure)
            return Result.Failure<GoalId>(result.Error);

        await userGoalRepository.AddAsync(newUserGoal, cancellationToken);
        return Result.Success(newUserGoal.Id);
    }
}
