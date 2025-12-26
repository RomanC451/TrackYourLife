using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class TimeBasedExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Plank";
        var orderIndex = 1;
        var durationSeconds = 60;
        var restTimeSeconds = 60;

        // Act
        var exerciseSet = new TimeBasedExerciseSet(
            id,
            name,
            orderIndex,
            durationSeconds,
            restTimeSeconds
        );

        // Assert
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
        exerciseSet.DurationSeconds.Should().Be(durationSeconds);
        exerciseSet.RestTimeSeconds.Should().Be(restTimeSeconds);
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 60);
        var newDurationSeconds = 90;
        var newRestTimeSeconds = 120;

        // Act
        var result = exerciseSet.Update(newDurationSeconds, newRestTimeSeconds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.DurationSeconds.Should().Be(newDurationSeconds);
        exerciseSet.RestTimeSeconds.Should().Be(newRestTimeSeconds);
    }

    [Fact]
    public void Update_WithZeroDuration_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 60);

        // Act
        var result = exerciseSet.Update(0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithNegativeDuration_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 60);

        // Act
        var result = exerciseSet.Update(-10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void GetDisplayValue_WithSeconds_ShouldReturnSecondsFormat()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 45);

        // Act
        var displayValue = exerciseSet.GetDisplayValue();

        // Assert
        displayValue.Should().Be("45s");
    }

    [Fact]
    public void GetDisplayValue_WithMinutes_ShouldReturnMinutesFormat()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 120);

        // Act
        var displayValue = exerciseSet.GetDisplayValue();

        // Assert
        displayValue.Should().Be("2.0m");
    }

    [Fact]
    public void GetUnit_WithSeconds_ShouldReturnSeconds()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 45);

        // Act
        var unit = exerciseSet.GetUnit();

        // Assert
        unit.Should().Be("seconds");
    }

    [Fact]
    public void GetUnit_WithMinutes_ShouldReturnMinutes()
    {
        // Arrange
        var exerciseSet = new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 1, 120);

        // Act
        var unit = exerciseSet.GetUnit();

        // Assert
        unit.Should().Be("minutes");
    }
}
