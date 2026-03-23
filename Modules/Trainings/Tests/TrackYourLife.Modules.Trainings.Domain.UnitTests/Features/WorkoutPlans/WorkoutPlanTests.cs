using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.WorkoutPlans;

public class WorkoutPlanTests
{
    [Fact]
    public void Create_WithDuplicatedTrainingIds_ShouldReturnFailure()
    {
        var trainingId = TrainingId.NewId();

        var result = WorkoutPlan.Create(
            WorkoutPlanId.NewId(),
            UserId.NewId(),
            "Push-Pull",
            true,
            [trainingId, trainingId],
            DateTime.UtcNow
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkoutPlanErrors.DuplicatedTrainingIds);
    }

    [Fact]
    public void ReplaceTrainings_WithValidOrder_ShouldUpdateOrder()
    {
        var first = TrainingId.NewId();
        var second = TrainingId.NewId();

        var workoutPlan = WorkoutPlan
            .Create(
                WorkoutPlanId.NewId(),
                UserId.NewId(),
                "Plan",
                false,
                [first, second],
                DateTime.UtcNow
            )
            .Value;

        var result = workoutPlan.ReplaceTrainings([second, first]);

        result.IsSuccess.Should().BeTrue();
        workoutPlan.WorkoutPlanTrainings
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.TrainingId)
            .Should()
            .Equal(second, first);
    }
}
