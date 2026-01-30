using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsOverview;

public class GetTrainingsOverviewQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
) : IQueryHandler<GetTrainingsOverviewQuery, TrainingsOverviewDto>
{
    public async Task<Result<TrainingsOverviewDto>> Handle(
        GetTrainingsOverviewQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var allCompletedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAsync(
            userId,
            cancellationToken
        );

        var completedWorkouts = allCompletedWorkouts.Where(w => w.FinishedOnUtc.HasValue).ToList();

        if (request.StartDate.HasValue)
        {
            var startDate = DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc);
            completedWorkouts = completedWorkouts
                .Where(w => w.FinishedOnUtc!.Value >= startDate)
                .ToList();
        }

        if (request.EndDate.HasValue)
        {
            var endDate = DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc);
            completedWorkouts = completedWorkouts
                .Where(w => w.FinishedOnUtc!.Value <= endDate)
                .ToList();
        }

        var activeTraining = await ongoingTrainingsQuery.GetUnfinishedByUserIdAsync(
            userId,
            cancellationToken
        );

        var totalWorkoutsCompleted = completedWorkouts.Count;
        var totalTrainingTimeSeconds = completedWorkouts
            .Where(w => w.FinishedOnUtc.HasValue)
            .Sum(w => (long)(w.FinishedOnUtc!.Value - w.StartedOnUtc).TotalSeconds);
        var totalCaloriesBurned = completedWorkouts
            .Where(w => w.CaloriesBurned.HasValue)
            .Sum(w => w.CaloriesBurned!.Value);
        var hasActiveTraining = activeTraining != null;

        var overview = new TrainingsOverviewDto(
            TotalWorkoutsCompleted: totalWorkoutsCompleted,
            TotalTrainingTimeSeconds: totalTrainingTimeSeconds,
            TotalCaloriesBurned: totalCaloriesBurned > 0 ? totalCaloriesBurned : null,
            HasActiveTraining: hasActiveTraining
        );

        return Result.Success(overview);
    }
}
