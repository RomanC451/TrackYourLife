using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;

public sealed class UpdateGoalCommandHandler(
    IGoalRepository userGoalRepository,
    IUsersUnitOfWork unitOfWork,
    IUserIdentifierProvider userIdentifierProvider
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

        List<Goal> goals = await userGoalRepository.GetOverlappingGoalsAsync(
            newUserGoal,
            cancellationToken
        );

        if (!command.Force && goals.Count > 0)
            return Result.Failure<GoalId>(GoalErrors.Overlapping(command.Type));

        Result result;

        foreach (var goal in goals)
        {
            if (goal.FullyOverlappedBy(newUserGoal))
            {
                userGoalRepository.Remove(goal);
                continue;
            }

            if (goal.StartDate < newUserGoal.StartDate)
            {
                result = goal.UpdateEndDate(newUserGoal.StartDate.AddDays(-1));
                if (result.IsFailure)
                    return Result.Failure<GoalId>(result.Error);
                userGoalRepository.Update(goal);
            }
            else
            {
                result = goal.UpdateStartDate(newUserGoal.EndDate.AddDays(1));
                if (result.IsFailure)
                    return Result.Failure<GoalId>(result.Error);
                userGoalRepository.Update(goal);
            }
            // else
            // {
            //     throw new InvalidOperationException("Goal is not fully overlapped by new goal");
            //     //!! Log this
            // }
        }

        await userGoalRepository.AddAsync(newUserGoal, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(newUserGoal.Id);
    }
}
