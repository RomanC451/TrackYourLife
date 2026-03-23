using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class WorkoutPlanReadModelFaker
{
    private static readonly Faker f = new();

    public static WorkoutPlanReadModel Generate(
        WorkoutPlanId? id = null,
        UserId? userId = null,
        string? name = null,
        bool? isActive = null,
        DateTime? createdOnUtc = null,
        DateTime? modifiedOnUtc = null,
        IReadOnlyList<TrainingReadModel>? trainings = null
    )
    {
        var workoutPlan = new WorkoutPlanReadModel(
            id ?? WorkoutPlanId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(2),
            isActive ?? f.Random.Bool(),
            createdOnUtc ?? f.Date.Recent().ToUniversalTime(),
            modifiedOnUtc
        );

        if (trainings is not null)
        {
            workoutPlan.WorkoutPlanTrainings = trainings
                .Select((t, index) => new WorkoutPlanTrainingReadModel(workoutPlan.Id, t.Id, index))
                .ToList();

            var items = workoutPlan.WorkoutPlanTrainings.ToList();
            for (var i = 0; i < items.Count; i++)
            {
                items[i].Training = trainings[i];
            }
        }

        return workoutPlan;
    }
}
