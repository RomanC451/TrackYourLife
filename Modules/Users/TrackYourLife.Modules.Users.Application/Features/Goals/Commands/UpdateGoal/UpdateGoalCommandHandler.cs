using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;

public sealed class UpdateGoalCommandHandler(
    IGoalRepository userGoalRepository,
    IUsersUnitOfWork unitOfWork,
    IUserIdentifierProvider userIdentifierProvider
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

        newUserGoal.UpdateType(command.Type);
        newUserGoal.UpdatePerPeriod(command.PerPeriod);

        var result = Result.FirstFailureOrSuccess(
            newUserGoal.UpdateValue(command.Value),
            newUserGoal.UpdateStartDate(command.StartDate),
            newUserGoal.UpdateEndDate(command.EndDate ?? DateOnly.MaxValue)
        );

        if (result.IsFailure)
            return result;

        List<Goal> goals = (
            await userGoalRepository.GetOverlappingGoalsAsync(newUserGoal, cancellationToken)
        )
            .FindAll(g => g.Id != newUserGoal.Id)
            .ToList();

        if (!command.Force && goals.Count > 0)
        {
            return Result.Failure(GoalErrors.Overlapping(command.Type));
        }

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
                    return result;
                userGoalRepository.Update(goal);
            }
            else //if (goal.EndDate > newUserGoal.EndDate)
            {
                result = goal.UpdateStartDate(newUserGoal.EndDate.AddDays(1));
                if (result.IsFailure)
                    return result;
                userGoalRepository.Update(goal);
            }
            // else
            // {
            //     throw new InvalidOperationException("Goal is not fully overlapped by new goal");
            //     //!! Log this
            // }
        }

        userGoalRepository.Update(newUserGoal);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
