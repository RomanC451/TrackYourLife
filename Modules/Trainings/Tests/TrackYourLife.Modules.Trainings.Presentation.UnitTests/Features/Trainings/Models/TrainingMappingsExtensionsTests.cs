using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Trainings.Models;

public class TrainingMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithTrainingReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var trainingId = TrainingId.NewId();
        var userId = UserId.NewId();
        var exercise1 = ExerciseReadModelFaker.Generate();
        var exercise2 = ExerciseReadModelFaker.Generate();

        var trainingExercise1 = new TrainingExerciseReadModel(trainingId, exercise1.Id, 0);
        var trainingExercise2 = new TrainingExerciseReadModel(trainingId, exercise2.Id, 1);

        var training = new TrainingReadModel(
            trainingId,
            userId,
            "Test Training",
            new List<string> { "Chest", "Triceps" },
            Difficulty.Medium,
            "Test description",
            DateTime.UtcNow,
            60,
            90,
            null
        )
        {
            TrainingExercises = new List<TrainingExerciseReadModel>
            {
                trainingExercise1,
                trainingExercise2,
            },
        };

        trainingExercise1.Exercise = exercise1;
        trainingExercise2.Exercise = exercise2;

        // Act
        var dto = training.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(trainingId);
        dto.Name.Should().Be("Test Training");
        dto.MuscleGroups.Should().HaveCount(2);
        dto.MuscleGroups.Should().Contain("Chest");
        dto.MuscleGroups.Should().Contain("Triceps");
        dto.Difficulty.Should().Be(Difficulty.Medium);
        dto.Description.Should().Be("Test description");
        dto.Duration.Should().Be(60);
        dto.RestSeconds.Should().Be(90);
        dto.Exercises.Should().HaveCount(2);
        dto.Exercises.ElementAt(0).Id.Should().Be(exercise1.Id);
        dto.Exercises.ElementAt(1).Id.Should().Be(exercise2.Id);
        dto.CreatedOnUtc.Should().Be(training.CreatedOnUtc);
        dto.ModifiedOnUtc.Should().Be(training.ModifiedOnUtc);
    }
}
