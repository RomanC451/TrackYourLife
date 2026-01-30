using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetMuscleGroupDistribution;

public class GetMuscleGroupDistributionQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
) : IQueryHandler<GetMuscleGroupDistributionQuery, IReadOnlyList<MuscleGroupDistributionDto>>
{
    public async Task<Result<IReadOnlyList<MuscleGroupDistributionDto>>> Handle(
        GetMuscleGroupDistributionQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        IEnumerable<OngoingTrainingReadModel> completedWorkouts;
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var startDate = request.StartDate.HasValue
                ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            var endDate = request.EndDate.HasValue
                ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
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

        var muscleGroupCounts = new Dictionary<string, int>();
        foreach (var workout in completedList)
        {
            var muscleGroups = workout.Training?.MuscleGroups ?? new List<string>();
            foreach (var muscleGroup in muscleGroups)
            {
                if (!muscleGroupCounts.ContainsKey(muscleGroup))
                {
                    muscleGroupCounts[muscleGroup] = 0;
                }
                muscleGroupCounts[muscleGroup]++;
            }
        }

        var totalMuscleGroupWorkouts = muscleGroupCounts.Values.Sum();
        var muscleGroupDistribution = muscleGroupCounts
            .Select(kvp => new MuscleGroupDistributionDto(
                kvp.Key,
                kvp.Value,
                totalMuscleGroupWorkouts > 0
                    ? (double)kvp.Value / totalMuscleGroupWorkouts * 100
                    : 0
            ))
            .OrderByDescending(m => m.WorkoutCount)
            .ToList();

        return Result.Success<IReadOnlyList<MuscleGroupDistributionDto>>(muscleGroupDistribution);
    }
}
