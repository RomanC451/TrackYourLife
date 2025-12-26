using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class WeightBasedExerciseSetChangeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var weightChange = 5.0f;
        var repsChange = 2;

        // Act
        var change = new WeightBasedExerciseSetChange(setId, weightChange, repsChange);

        // Assert
        change.SetId.Should().Be(setId);
        change.WeightChange.Should().Be(weightChange);
        change.RepsChange.Should().Be(repsChange);
    }
}

public class TimeBasedExerciseSetChangeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var durationChangeSeconds = 30;

        // Act
        var change = new TimeBasedExerciseSetChange(setId, durationChangeSeconds);

        // Assert
        change.SetId.Should().Be(setId);
        change.DurationChangeSeconds.Should().Be(durationChangeSeconds);
    }
}

public class BodyweightExerciseSetChangeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var repsChange = 3;

        // Act
        var change = new BodyweightExerciseSetChange(setId, repsChange);

        // Assert
        change.SetId.Should().Be(setId);
        change.RepsChange.Should().Be(repsChange);
    }
}

public class DistanceExerciseSetChangeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var distanceChange = 1.5f;

        // Act
        var change = new DistanceExerciseSetChange(setId, distanceChange);

        // Assert
        change.SetId.Should().Be(setId);
        change.DistanceChange.Should().Be(distanceChange);
    }
}

public class CustomExerciseSetChangeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var valueChange = "2";

        // Act
        var change = new CustomExerciseSetChange(setId, valueChange);

        // Assert
        change.SetId.Should().Be(setId);
        change.ValueChange.Should().Be(valueChange);
    }
}

public class ExerciseSetChangeJsonSerializerTests
{
    [Fact]
    public void Serialize_WithEmptyList_ShouldReturnEmptyArray()
    {
        // Arrange
        var changes = new List<ExerciseSetChange>();

        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(changes);

        // Assert
        result.Should().Be("[]");
    }

    [Fact]
    public void Serialize_WithNullList_ShouldReturnEmptyArray()
    {
        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(null!);

        // Assert
        result.Should().Be("[]");
    }

    [Fact]
    public void Deserialize_WithEmptyJson_ShouldReturnEmptyList()
    {
        // Arrange
        var json = "[]";

        // Act
        var result = ExerciseSetChangeJsonSerializer.Deserialize(json);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Deserialize_WithNullJson_ShouldReturnEmptyList()
    {
        // Act
        var result = ExerciseSetChangeJsonSerializer.Deserialize(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Serialize_WithWeightBasedChange_ShouldIncludeTypeDiscriminator()
    {
        // Arrange
        var change = new WeightBasedExerciseSetChange(Guid.NewGuid(), 5.0f, 2);
        var changes = new List<ExerciseSetChange> { change };

        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(changes);

        // Assert
        result.Should().Contain("\"type\":\"weight\"");
        result.Should().Contain("\"weightChange\":5");
        result.Should().Contain("\"repsChange\":2");
    }

    [Fact]
    public void Serialize_WithTimeBasedChange_ShouldIncludeTypeDiscriminator()
    {
        // Arrange
        var change = new TimeBasedExerciseSetChange(Guid.NewGuid(), 30);
        var changes = new List<ExerciseSetChange> { change };

        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(changes);

        // Assert
        result.Should().Contain("\"type\":\"time\"");
        result.Should().Contain("\"durationChangeSeconds\":30");
    }

    [Fact]
    public void Serialize_WithBodyweightChange_ShouldIncludeTypeDiscriminator()
    {
        // Arrange
        var change = new BodyweightExerciseSetChange(Guid.NewGuid(), 3);
        var changes = new List<ExerciseSetChange> { change };

        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(changes);

        // Assert
        result.Should().Contain("\"type\":\"bodyweight\"");
        result.Should().Contain("\"repsChange\":3");
    }

    [Fact]
    public void Serialize_WithDistanceChange_ShouldIncludeTypeDiscriminator()
    {
        // Arrange
        var change = new DistanceExerciseSetChange(Guid.NewGuid(), 1.5f);
        var changes = new List<ExerciseSetChange> { change };

        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(changes);

        // Assert
        result.Should().Contain("\"type\":\"distance\"");
        result.Should().Contain("\"distanceChange\":1.5");
    }

    [Fact]
    public void Serialize_WithCustomChange_ShouldIncludeTypeDiscriminator()
    {
        // Arrange
        var change = new CustomExerciseSetChange(Guid.NewGuid(), "2");
        var changes = new List<ExerciseSetChange> { change };

        // Act
        var result = ExerciseSetChangeJsonSerializer.Serialize(changes);

        // Assert
        result.Should().Contain("\"type\":\"custom\"");
        result.Should().Contain("\"valueChange\":\"2\"");
    }

    [Fact]
    public void SerializeAndDeserialize_WithMixedChanges_ShouldRoundTripCorrectly()
    {
        // Arrange
        var originalChanges = new List<ExerciseSetChange>
        {
            new WeightBasedExerciseSetChange(Guid.NewGuid(), 5.0f, 2),
            new TimeBasedExerciseSetChange(Guid.NewGuid(), 30),
            new BodyweightExerciseSetChange(Guid.NewGuid(), 3),
            new DistanceExerciseSetChange(Guid.NewGuid(), 1.5f),
            new CustomExerciseSetChange(Guid.NewGuid(), "2"),
        };

        // Act
        var json = ExerciseSetChangeJsonSerializer.Serialize(originalChanges);
        var deserializedChanges = ExerciseSetChangeJsonSerializer.Deserialize(json);

        // Assert
        deserializedChanges.Should().HaveCount(5);
        deserializedChanges[0].Should().BeOfType<WeightBasedExerciseSetChange>();
        deserializedChanges[1].Should().BeOfType<TimeBasedExerciseSetChange>();
        deserializedChanges[2].Should().BeOfType<BodyweightExerciseSetChange>();
        deserializedChanges[3].Should().BeOfType<DistanceExerciseSetChange>();
        deserializedChanges[4].Should().BeOfType<CustomExerciseSetChange>();

        // Verify properties are preserved
        var weightChange = (WeightBasedExerciseSetChange)deserializedChanges[0];
        weightChange.WeightChange.Should().Be(5.0f);
        weightChange.RepsChange.Should().Be(2);

        var timeChange = (TimeBasedExerciseSetChange)deserializedChanges[1];
        timeChange.DurationChangeSeconds.Should().Be(30);

        var bodyweightChange = (BodyweightExerciseSetChange)deserializedChanges[2];
        bodyweightChange.RepsChange.Should().Be(3);

        var distanceChange = (DistanceExerciseSetChange)deserializedChanges[3];
        distanceChange.DistanceChange.Should().Be(1.5f);

        var customChange = (CustomExerciseSetChange)deserializedChanges[4];
        customChange.ValueChange.Should().Be("2");
    }
}
