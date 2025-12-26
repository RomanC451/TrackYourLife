using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class WeightBasedExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Bench Press";
        var orderIndex = 1;
        var reps = 10;
        var weight = 50.5f;
        var restTimeSeconds = 120;

        // Act
        var exerciseSet = new WeightBasedExerciseSet(
            id,
            name,
            orderIndex,
            reps,
            weight,
            restTimeSeconds
        );

        // Assert
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
        exerciseSet.Reps.Should().Be(reps);
        exerciseSet.Weight.Should().Be(weight);
        exerciseSet.RestTimeSeconds.Should().Be(restTimeSeconds);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exerciseSet = new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 10, 50.0f);
        var newReps = 15;
        var newWeight = 60.0f;
        var newRestTimeSeconds = 180;

        // Act
        var result = exerciseSet.Update(newReps, newWeight, newRestTimeSeconds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Reps.Should().Be(newReps);
        exerciseSet.Weight.Should().Be(newWeight);
        exerciseSet.RestTimeSeconds.Should().Be(newRestTimeSeconds);
    }

    [Theory]
    [InlineData(-1, 50.0f)]
    [InlineData(10, -1.0f)]
    [InlineData(-5, -10.0f)]
    public void Update_WithNegativeValues_ShouldReturnFailure(int reps, float weight)
    {
        // Arrange
        var exerciseSet = new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 10, 50.0f);

        // Act
        var result = exerciseSet.Update(reps, weight);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void GetDisplayValue_ShouldReturnCorrectFormat()
    {
        // Arrange
        var exerciseSet = new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 10, 50.5f);

        // Act
        var displayValue = exerciseSet.GetDisplayValue();

        // Assert
        displayValue.Should().Be("10 x 50.5");
    }

    [Fact]
    public void GetUnit_ShouldReturnKg()
    {
        // Arrange
        var exerciseSet = new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 10, 50.5f);

        // Act
        var unit = exerciseSet.GetUnit();

        // Assert
        unit.Should().Be("kg");
    }
}
