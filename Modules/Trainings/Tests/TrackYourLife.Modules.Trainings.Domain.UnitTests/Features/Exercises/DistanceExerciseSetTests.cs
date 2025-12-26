using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class DistanceExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "5K Run";
        var orderIndex = 1;
        var distance = 5.0f;
        var distanceUnit = "km";
        var restTimeSeconds = 300;

        // Act
        var exerciseSet = new DistanceExerciseSet(
            id,
            name,
            orderIndex,
            distance,
            distanceUnit,
            restTimeSeconds
        );

        // Assert
        exerciseSet.Id.Should().Be(id);
        exerciseSet.Name.Should().Be(name);
        exerciseSet.OrderIndex.Should().Be(orderIndex);
        exerciseSet.Distance.Should().Be(distance);
        exerciseSet.DistanceUnit.Should().Be(distanceUnit);
        exerciseSet.RestTimeSeconds.Should().Be(restTimeSeconds);
    }

    [Fact]
    public void Constructor_WithDefaultDistanceUnit_ShouldUseKm()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "5K Run";
        var orderIndex = 1;
        var distance = 5.0f;

        // Act
        var exerciseSet = new DistanceExerciseSet(id, name, orderIndex, distance);

        // Assert
        exerciseSet.DistanceUnit.Should().Be("km");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var exerciseSet = new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 1, 5.0f, "km");
        var newDistance = 10.0f;
        var newDistanceUnit = "miles";
        var newRestTimeSeconds = 600;

        // Act
        var result = exerciseSet.Update(newDistance, newDistanceUnit, newRestTimeSeconds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.Distance.Should().Be(newDistance);
        exerciseSet.DistanceUnit.Should().Be(newDistanceUnit);
        exerciseSet.RestTimeSeconds.Should().Be(newRestTimeSeconds);
    }

    [Fact]
    public void Update_WithNegativeDistance_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 1, 5.0f, "km");

        // Act
        var result = exerciseSet.Update(-1.0f);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithEmptyDistanceUnit_ShouldReturnFailure()
    {
        // Arrange
        var exerciseSet = new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 1, 5.0f, "km");

        // Act
        var result = exerciseSet.Update(10.0f, string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithNullDistanceUnit_ShouldKeepCurrentUnit()
    {
        // Arrange
        var exerciseSet = new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 1, 5.0f, "km");
        var originalUnit = exerciseSet.DistanceUnit;

        // Act
        var result = exerciseSet.Update(10.0f, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        exerciseSet.DistanceUnit.Should().Be(originalUnit);
    }

    [Fact]
    public void GetDisplayValue_ShouldReturnCorrectFormat()
    {
        // Arrange
        var exerciseSet = new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 1, 5.0f, "km");

        // Act
        var displayValue = exerciseSet.GetDisplayValue();

        // Assert
        displayValue.Should().Be("5.0 km");
    }

    [Fact]
    public void GetUnit_ShouldReturnDistanceUnit()
    {
        // Arrange
        var exerciseSet = new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 1, 5.0f, "miles");

        // Act
        var unit = exerciseSet.GetUnit();

        // Assert
        unit.Should().Be("miles");
    }
}
