using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.TrainingExercises;

public class TrainingExerciseTests
{
    private readonly TrainingId _validTrainingId = TrainingId.Create(Guid.NewGuid());
    private readonly UserId _validUserId = UserId.Create(Guid.NewGuid());
    private readonly int _validOrderIndex = 0;

    private Exercise CreateValidExercise()
    {
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        return Exercise
            .Create(
                exerciseId,
                _validUserId,
                "Test Exercise",
                new List<string> { "Chest" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 8, 60.0f),
                },
                DateTime.UtcNow
            )
            .Value;
    }

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var exercise = CreateValidExercise();

        // Act
        var result = TrainingExercise.Create(_validTrainingId, exercise, _validOrderIndex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var trainingExercise = result.Value;
        trainingExercise.TrainingId.Should().Be(_validTrainingId);
        trainingExercise.Exercise.Should().Be(exercise);
        trainingExercise.OrderIndex.Should().Be(_validOrderIndex);
    }

    [Fact]
    public void Create_WithEmptyTrainingId_ShouldReturnFailure()
    {
        // Arrange
        var emptyTrainingId = TrainingId.Empty;
        var exercise = CreateValidExercise();

        // Act
        var result = TrainingExercise.Create(emptyTrainingId, exercise, _validOrderIndex);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNullExercise_ShouldReturnFailure()
    {
        // Act
        var result = TrainingExercise.Create(_validTrainingId, null!, _validOrderIndex);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithNegativeOrderIndex_ShouldReturnFailure(int orderIndex)
    {
        // Arrange
        var exercise = CreateValidExercise();

        // Act
        var result = TrainingExercise.Create(_validTrainingId, exercise, orderIndex);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Create_WithValidOrderIndex_ShouldReturnSuccess(int orderIndex)
    {
        // Arrange
        var exercise = CreateValidExercise();

        // Act
        var result = TrainingExercise.Create(_validTrainingId, exercise, orderIndex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var trainingExercise = result.Value;
        trainingExercise.OrderIndex.Should().Be(orderIndex);
    }

    [Fact]
    public void TrainingExercise_ShouldBeImmutable()
    {
        // Act & Assert
        var properties = typeof(TrainingExercise).GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "Exercise")
            {
                // Exercise property should be settable for navigation purposes
                property
                    .CanWrite.Should()
                    .BeTrue($"Property {property.Name} should be writable for navigation");
            }
            else
            {
                property
                    .CanWrite.Should()
                    .BeFalse($"Property {property.Name} should not be writable");
            }
        }
    }

    [Fact]
    public void TrainingExercise_ShouldHaveCorrectDefaultValues()
    {
        // Arrange
        var exercise = CreateValidExercise();

        // Act
        var trainingExercise = TrainingExercise
            .Create(_validTrainingId, exercise, _validOrderIndex)
            .Value;

        // Assert
        trainingExercise.TrainingId.Should().NotBe(TrainingId.Empty);
        trainingExercise.Exercise.Should().NotBeNull();
        trainingExercise.OrderIndex.Should().Be(_validOrderIndex);
    }

    [Fact]
    public void TrainingExercise_WithSameParameters_ShouldBeEqual()
    {
        // Arrange
        var exercise = CreateValidExercise();
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var orderIndex = 5;

        // Act
        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise, orderIndex).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise, orderIndex).Value;

        // Assert
        trainingExercise1.TrainingId.Should().Be(trainingExercise2.TrainingId);
        trainingExercise1.Exercise.Should().Be(trainingExercise2.Exercise);
        trainingExercise1.OrderIndex.Should().Be(trainingExercise2.OrderIndex);
    }

    [Fact]
    public void TrainingExercise_WithDifferentOrderIndex_ShouldBeDifferent()
    {
        // Arrange
        var exercise = CreateValidExercise();
        var trainingId = TrainingId.Create(Guid.NewGuid());

        // Act
        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise, 1).Value;

        // Assert
        trainingExercise1.TrainingId.Should().Be(trainingExercise2.TrainingId);
        trainingExercise1.Exercise.Should().Be(trainingExercise2.Exercise);
        trainingExercise1.OrderIndex.Should().NotBe(trainingExercise2.OrderIndex);
    }
}
