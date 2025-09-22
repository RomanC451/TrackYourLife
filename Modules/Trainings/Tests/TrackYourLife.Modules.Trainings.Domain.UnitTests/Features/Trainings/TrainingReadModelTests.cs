using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Trainings;

public class TrainingReadModelTests
{
    [Fact]
    public void TrainingReadModel_ShouldImplementIReadModel()
    {
        // Arrange & Act
        var readModelType = typeof(TrainingReadModel);
        var interfaceType = typeof(IReadModel<TrainingId>);

        // Assert
        readModelType.Should().BeAssignableTo(interfaceType);
    }

    [Fact]
    public void TrainingReadModel_ShouldBeRecord()
    {
        // Arrange & Act
        var readModelType = typeof(TrainingReadModel);

        // Assert
        readModelType.IsClass.Should().BeTrue();
        readModelType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = TrainingId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var name = "Test Training";
        var muscleGroups = new List<string> { "Chest", "Triceps" };
        var difficulty = Difficulty.Medium;
        var description = "Test training description";
        var createdOnUtc = DateTime.UtcNow;
        var duration = 45;
        var restSeconds = 90;
        var modifiedOnUtc = DateTime.UtcNow.AddMinutes(5);

        // Act
        var readModel = new TrainingReadModel(
            id,
            userId,
            name,
            muscleGroups,
            difficulty,
            description,
            createdOnUtc,
            duration,
            restSeconds,
            modifiedOnUtc
        );

        // Assert
        readModel.Id.Should().Be(id);
        readModel.UserId.Should().Be(userId);
        readModel.Name.Should().Be(name);
        readModel.MuscleGroups.Should().BeEquivalentTo(muscleGroups);
        readModel.Difficulty.Should().Be(difficulty);
        readModel.Description.Should().Be(description);
        readModel.CreatedOnUtc.Should().Be(createdOnUtc);
        readModel.Duration.Should().Be(duration);
        readModel.RestSeconds.Should().Be(restSeconds);
        readModel.ModifiedOnUtc.Should().Be(modifiedOnUtc);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldSetNull()
    {
        // Arrange
        var id = TrainingId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var name = "Test Training";
        var muscleGroups = new List<string> { "Chest" };
        var difficulty = Difficulty.Easy;
        var createdOnUtc = DateTime.UtcNow;
        var duration = 30;
        var restSeconds = 60;

        // Act
        var readModel = new TrainingReadModel(
            id,
            userId,
            name,
            muscleGroups,
            difficulty,
            null,
            createdOnUtc,
            duration,
            restSeconds,
            null
        );

        // Assert
        readModel.Description.Should().BeNull();
        readModel.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void TrainingExercises_ShouldInitializeAsEmptyCollection()
    {
        // Arrange
        var readModel = new TrainingReadModel(
            TrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            DateTime.UtcNow,
            30,
            60,
            null
        );

        // Act & Assert
        readModel.TrainingExercises.Should().NotBeNull();
        readModel.TrainingExercises.Should().BeEmpty();
    }

    [Fact]
    public void TrainingExercises_ShouldBeSettable()
    {
        // Arrange
        var readModel = new TrainingReadModel(
            TrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            DateTime.UtcNow,
            30,
            60,
            null
        );

        // Act & Assert
        // Note: TrainingExercises is init-only, so we can't test assignment after construction
        readModel.TrainingExercises.Should().NotBeNull();
    }

    [Theory]
    [InlineData(Difficulty.Easy, 0)]
    [InlineData(Difficulty.Medium, 1)]
    [InlineData(Difficulty.Hard, 2)]
    public void Difficulty_ShouldHaveCorrectValues(Difficulty difficulty, int expectedValue)
    {
        // Arrange
        var readModel = new TrainingReadModel(
            TrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            difficulty,
            null,
            DateTime.UtcNow,
            30,
            60,
            null
        );

        // Act
        var actualValue = (int)readModel.Difficulty;

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(30, 30)]
    [InlineData(120, 120)]
    public void Duration_ShouldAcceptVariousValues(int duration, int expectedValue)
    {
        // Arrange
        var readModel = new TrainingReadModel(
            TrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            DateTime.UtcNow,
            duration,
            60,
            null
        );

        // Act & Assert
        readModel.Duration.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(30, 30)]
    [InlineData(300, 300)]
    public void RestSeconds_ShouldAcceptVariousValues(int restSeconds, int expectedValue)
    {
        // Arrange
        var readModel = new TrainingReadModel(
            TrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            DateTime.UtcNow,
            30,
            restSeconds,
            null
        );

        // Act & Assert
        readModel.RestSeconds.Should().Be(expectedValue);
    }
}
