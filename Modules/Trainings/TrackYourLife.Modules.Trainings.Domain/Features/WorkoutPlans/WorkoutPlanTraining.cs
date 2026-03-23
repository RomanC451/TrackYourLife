using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public sealed class WorkoutPlanTraining
{
    public WorkoutPlanId WorkoutPlanId { get; } = WorkoutPlanId.Empty;
    public TrainingId TrainingId { get; } = TrainingId.Empty;
    public int OrderIndex { get; }

    private WorkoutPlanTraining() { }

    private WorkoutPlanTraining(WorkoutPlanId workoutPlanId, TrainingId trainingId, int orderIndex)
    {
        WorkoutPlanId = workoutPlanId;
        TrainingId = trainingId;
        OrderIndex = orderIndex;
    }

    public static Result<WorkoutPlanTraining> Create(
        WorkoutPlanId workoutPlanId,
        TrainingId trainingId,
        int orderIndex
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                workoutPlanId,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(workoutPlanId))
            ),
            Ensure.NotEmptyId(
                trainingId,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(trainingId))
            ),
            Ensure.NotNegative(
                orderIndex,
                DomainErrors.ArgumentError.Invalid(nameof(WorkoutPlan), nameof(orderIndex))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<WorkoutPlanTraining>(result.Error);
        }

        return Result.Success(new WorkoutPlanTraining(workoutPlanId, trainingId, orderIndex));
    }
}
