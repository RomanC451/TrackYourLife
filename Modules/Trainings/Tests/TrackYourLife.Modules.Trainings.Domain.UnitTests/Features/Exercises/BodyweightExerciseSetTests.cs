using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class BodyweightExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Push-ups";
        var orderIndex = 1;
        var reps = 15;
        var restTimeSeconds = 60;

        // Act
        var exerciseSet = new BodyweightExerciseSet(id, name, orderIndex, reps, restTimeSeconds);

        // Assert
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
        exerciseSet.Reps.Should().Be(reps);
        exerciseSet.RestTimeSeconds.Should().Be(restTimeSeconds);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exerciseSet = new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 1, 15);
        var newReps = 20;
        var newRestTimeSeconds = 120;

        // Act
        var result = exerciseSet.Update(newReps, newRestTimeSeconds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Reps.Should().Be(newReps);
        exerciseSet.RestTimeSeconds.Should().Be(newRestTimeSeconds);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Update_WithNegativeReps_ShouldReturnFailure(int reps)
    {
        // Arrange
        var exerciseSet = new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 1, 15);

        // Act
        var result = exerciseSet.Update(reps);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void Update_WithValidReps_ShouldReturnSuccess(int reps)
    {
        // Arrange
        var exerciseSet = new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 1, 15);

        // Act
        var result = exerciseSet.Update(reps);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Reps.Should().Be(reps);
    }

    [Fact]
    public void GetDisplayValue_ShouldReturnCorrectFormat()
    {
        // Arrange
        var exerciseSet = new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 1, 15);

        // Act
        var displayValue = exerciseSet.GetDisplayValue();

        // Assert
        displayValue.Should().Be("15 reps");
    }

    [Fact]
    public void GetUnit_ShouldReturnReps()
    {
        // Arrange
        var exerciseSet = new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 1, 15);

        // Act
        var unit = exerciseSet.GetUnit();

        // Assert
        unit.Should().Be("reps");
    }
}
