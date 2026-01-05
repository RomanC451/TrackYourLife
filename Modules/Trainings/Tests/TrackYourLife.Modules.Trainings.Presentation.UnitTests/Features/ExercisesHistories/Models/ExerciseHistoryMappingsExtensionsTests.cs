using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.ExercisesHistories.Models;

public class ExerciseHistoryMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithExerciseHistoryReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var exerciseHistoryId = ExerciseHistoryId.NewId();
        var ongoingTrainingId = OngoingTrainingId.NewId();
        var exerciseId = ExerciseId.NewId();
        var newExerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 60, "kg").Value,
        };
        var oldExerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50, "kg").Value,
        };

        var exerciseHistory = ExerciseHistoryReadModelFaker.Generate(
            id: exerciseHistoryId,
            ongoingTrainingId: ongoingTrainingId,
            exerciseId: exerciseId,
            newExerciseSets: newExerciseSets,
            oldExerciseSets: oldExerciseSets,
            areChangesApplied: true
        );

        // Act
        var dto = exerciseHistory.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(exerciseHistoryId);
        dto.ExerciseId.Should().Be(exerciseId);
        dto.NewExerciseSets.Should().HaveCount(1);
        dto.OldExerciseSets.Should().HaveCount(1);
        dto.AreChangesApplied.Should().BeTrue();
        dto.CreatedOnUtc.Should().Be(exerciseHistory.CreatedOnUtc);
        dto.ModifiedOnUtc.Should().Be(exerciseHistory.ModifiedOnUtc);
    }
}
