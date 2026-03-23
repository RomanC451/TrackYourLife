using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetNextWorkoutFromActivePlan;

public sealed class GetNextWorkoutFromActivePlanQueryHandler(
    IWorkoutPlansQuery workoutPlansQuery,
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetNextWorkoutFromActivePlanQuery, TrainingReadModel>
{
    public async Task<Result<TrainingReadModel>> Handle(
        GetNextWorkoutFromActivePlanQuery request,
        CancellationToken cancellationToken
    )
    {
        var activePlan = await workoutPlansQuery.GetActiveByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (activePlan is null)
        {
            return Result.Failure<TrainingReadModel>(WorkoutPlanErrors.ActivePlanNotFound);
        }

        var orderedTrainings = activePlan.GetOrderedTrainings().ToList();
        if (!orderedTrainings.Any())
        {
            return Result.Failure<TrainingReadModel>(WorkoutPlanErrors.EmptyWorkoutPlan);
        }

        var lastCompletedOngoingTraining = await ongoingTrainingsQuery.GetLastCompletedByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (lastCompletedOngoingTraining is null)
        {
            return Result.Success(orderedTrainings[0]);
        }

        var lastIndex = orderedTrainings.FindIndex(t => t.Id == lastCompletedOngoingTraining.Training.Id);
        if (lastIndex < 0)
        {
            return Result.Success(orderedTrainings[0]);
        }

        var nextIndex = (lastIndex + 1) % orderedTrainings.Count;
        return Result.Success(orderedTrainings[nextIndex]);
    }
}
