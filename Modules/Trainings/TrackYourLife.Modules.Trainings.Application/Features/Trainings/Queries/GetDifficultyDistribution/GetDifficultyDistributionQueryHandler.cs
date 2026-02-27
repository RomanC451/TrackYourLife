using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;

public class GetDifficultyDistributionQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
) : IQueryHandler<GetDifficultyDistributionQuery, IReadOnlyList<DifficultyDistributionDto>>
{
    public async Task<Result<IReadOnlyList<DifficultyDistributionDto>>> Handle(
        GetDifficultyDistributionQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        IEnumerable<OngoingTrainingReadModel> completedWorkouts;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate ?? DateOnly.MinValue;
            var endDate = request.EndDate ?? DateOnly.MaxValue;
            completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAndDateRangeAsync(
                userId,
                startDate,
                endDate,
                cancellationToken
            );
        }
        else
        {
            completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAsync(
                userId,
                cancellationToken
            );
        }

        var completedList = completedWorkouts.Where(w => w.FinishedOnUtc.HasValue).ToList();

        var difficultyCounts = new Dictionary<string, int>();
        foreach (var workout in completedList)
        {
            var difficulty = workout.Training?.Difficulty.ToString() ?? Difficulty.Easy.ToString();
            if (!difficultyCounts.ContainsKey(difficulty))
            {
                difficultyCounts[difficulty] = 0;
            }
            difficultyCounts[difficulty]++;
        }

        var totalDifficultyWorkouts = difficultyCounts.Values.Sum();
        var difficultyDistribution = difficultyCounts
            .Select(kvp => new DifficultyDistributionDto(
                kvp.Key,
                kvp.Value,
                totalDifficultyWorkouts > 0 ? (double)kvp.Value / totalDifficultyWorkouts * 100 : 0
            ))
            .OrderBy(d => d.Difficulty)
            .ToList();

        return Result.Success<IReadOnlyList<DifficultyDistributionDto>>(difficultyDistribution);
    }
}
