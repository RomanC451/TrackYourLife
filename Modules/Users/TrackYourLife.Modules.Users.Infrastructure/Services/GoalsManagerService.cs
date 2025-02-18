using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Infrastructure.Services
{
    public class GoalsManagerService(IGoalRepository userGoalRepository) : IGoalsManagerService
    {
        public async Task<Result> HandleOverlappingGoalsAsync(
            Goal newUserGoal,
            bool force,
            CancellationToken cancellationToken
        )
        {
            List<Goal> goals = (
                await userGoalRepository.GetOverlappingGoalsAsync(newUserGoal, cancellationToken)
            )
                .FindAll(g => g.Id != newUserGoal.Id)
                .ToList();

            if (!force && goals.Count > 0)
            {
                return Result.Failure(GoalErrors.Overlapping(newUserGoal.Type));
            }

            foreach (var goal in goals)
            {
                if (goal.FullyOverlappedBy(newUserGoal))
                {
                    userGoalRepository.Remove(goal);
                    continue;
                }

                Result result;
                if (goal.StartDate < newUserGoal.StartDate)
                {
                    result = goal.UpdateEndDate(newUserGoal.StartDate.AddDays(-1));
                    if (result.IsFailure)
                        return result;
                    userGoalRepository.Update(goal);
                }
                else
                {
                    result = goal.UpdateStartDate(newUserGoal.EndDate.AddDays(1));
                    if (result.IsFailure)
                        return result;
                    userGoalRepository.Update(goal);
                }
            }

            return Result.Success();
        }
    }
}
