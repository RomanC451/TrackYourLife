using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class ExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Set";
        var reps = 10;
        var weight = 50.5f;
        var orderIndex = 1;

        // Act
        var exerciseSet = new ExerciseSet(id, name, reps, weight, orderIndex);

        // Assert
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.Reps.Should().Be(reps);
        exerciseSet.Weight.Should().Be(weight);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), "Test Set", 10, 50.0f, 1);
        var newReps = 15;
        var newWeight = 60.0f;

        // Act
        var result = exerciseSet.Update(newReps, newWeight);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Reps.Should().Be(newReps);
        exerciseSet.Weight.Should().Be(newWeight);
    }

    [Theory]
    [InlineData(-1, 50.0f)]
    [InlineData(10, -1.0f)]
    [InlineData(-5, -10.0f)]
    public void Update_WithNegativeValues_ShouldReturnFailure(int reps, float weight)
    {
        // Arrange
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), "Test Set", 10, 50.0f, 1);

        // Act
        var result = exerciseSet.Update(reps, weight);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 0.0f)]
    [InlineData(1, 0.1f)]
    [InlineData(100, 200.5f)]
    public void Update_WithValidValues_ShouldReturnSuccess(int reps, float weight)
    {
        // Arrange
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), "Test Set", 10, 50.0f, 1);

        // Act
        var result = exerciseSet.Update(reps, weight);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Reps.Should().Be(reps);
        exerciseSet.Weight.Should().Be(weight);
    }

    [Fact]
    public void Update_WithSameValues_ShouldReturnSuccess()
    {
        // Arrange
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), "Test Set", 10, 50.0f, 1);
        var originalReps = exerciseSet.Reps;
        var originalWeight = exerciseSet.Weight;

        // Act
        var result = exerciseSet.Update(originalReps, originalWeight);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Reps.Should().Be(originalReps);
        exerciseSet.Weight.Should().Be(originalWeight);
    }
}
