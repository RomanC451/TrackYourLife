using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public sealed class WorkoutPlan : AggregateRoot<WorkoutPlanId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public ICollection<WorkoutPlanTraining> WorkoutPlanTrainings { get; private set; } = [];
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; }

    private WorkoutPlan()
        : base() { }

    private WorkoutPlan(
        WorkoutPlanId id,
        UserId userId,
        string name,
        bool isActive,
        ICollection<WorkoutPlanTraining> workoutPlanTrainings,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        IsActive = isActive;
        WorkoutPlanTrainings = workoutPlanTrainings;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<WorkoutPlan> Create(
        WorkoutPlanId id,
        UserId userId,
        string name,
        bool isActive,
        IReadOnlyList<TrainingId> orderedTrainingIds,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(userId))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(name))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(createdOnUtc))
            ),
            Ensure.NotEmptyList(
                orderedTrainingIds,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(orderedTrainingIds))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<WorkoutPlan>(result.Error);
        }

        if (orderedTrainingIds.Count != orderedTrainingIds.Distinct().Count())
        {
            return Result.Failure<WorkoutPlan>(WorkoutPlanErrors.DuplicatedTrainingIds);
        }

        var workoutPlanTrainingsResults = orderedTrainingIds.Select(
            (trainingId, index) => WorkoutPlanTraining.Create(id, trainingId, index)
        );

        var workoutPlanTrainingsResult = Result.FirstFailureOrSuccess([.. workoutPlanTrainingsResults]);
        if (workoutPlanTrainingsResult.IsFailure)
        {
            return Result.Failure<WorkoutPlan>(workoutPlanTrainingsResult.Error);
        }

        return Result.Success(
            new WorkoutPlan(
                id,
                userId,
                name,
                isActive,
                workoutPlanTrainingsResults.Select(r => r.Value).ToList(),
                createdOnUtc
            )
        );
    }

    public Result UpdateDetails(string name, bool isActive)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(name))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Name = name;
        IsActive = isActive;

        return Result.Success();
    }

    public Result ReplaceTrainings(IReadOnlyList<TrainingId> orderedTrainingIds)
    {
        var result = Ensure.NotEmptyList(
            orderedTrainingIds,
            DomainErrors.ArgumentError.Empty(nameof(WorkoutPlan), nameof(orderedTrainingIds))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        if (orderedTrainingIds.Count != orderedTrainingIds.Distinct().Count())
        {
            return Result.Failure(WorkoutPlanErrors.DuplicatedTrainingIds);
        }

        var workoutPlanTrainingsResults = orderedTrainingIds.Select(
            (trainingId, index) => WorkoutPlanTraining.Create(Id, trainingId, index)
        );

        var createResult = Result.FirstFailureOrSuccess([.. workoutPlanTrainingsResults]);
        if (createResult.IsFailure)
        {
            return Result.Failure(createResult.Error);
        }

        WorkoutPlanTrainings = workoutPlanTrainingsResults.Select(r => r.Value).ToList();

        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
