using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.Exercises.Models;

public class ExerciseMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithExerciseReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();
        var userId = UserId.NewId();
        var exerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 10, "reps", 50, "kg").Value,
        };

        var exercise = ExerciseReadModelFaker.Generate(
            id: exerciseId,
            userId: userId,
            name: "Test Exercise",
            muscleGroups: new List<string> { "Chest", "Triceps" },
            difficulty: Difficulty.Medium,
            pictureUrl: "https://example.com/picture.jpg",
            videoUrl: "https://example.com/video.mp4",
            description: "Test description",
            equipment: "Barbell",
            exerciseSets: exerciseSets
        );

        // Act
        var dto = exercise.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(exerciseId);
        dto.Name.Should().Be("Test Exercise");
        dto.MuscleGroups.Should().HaveCount(2);
        dto.MuscleGroups.Should().Contain("Chest");
        dto.MuscleGroups.Should().Contain("Triceps");
        dto.Difficulty.Should().Be(Difficulty.Medium);
        dto.PictureUrl.Should().Be("https://example.com/picture.jpg");
        dto.VideoUrl.Should().Be("https://example.com/video.mp4");
        dto.Description.Should().Be("Test description");
        dto.Equipment.Should().Be("Barbell");
        dto.ExerciseSets.Should().HaveCount(2);
        dto.CreatedOnUtc.Should().Be(exercise.CreatedOnUtc);
        dto.ModifiedOnUtc.Should().Be(exercise.ModifiedOnUtc);
    }
}
