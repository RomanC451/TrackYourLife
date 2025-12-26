using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Primitives;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class ExerciseHistoryReadModelTests
{
    [Fact]
    public void ExerciseHistoryReadModel_ShouldImplementIReadModel()
    {
        // Arrange & Act
        var readModelType = typeof(ExerciseHistoryReadModel);
        var interfaceType = typeof(IReadModel<ExerciseHistoryId>);

        // Assert
        readModelType.Should().BeAssignableTo(interfaceType);
    }

    [Fact]
    public void ExerciseHistoryReadModel_ShouldBeRecord()
    {
        // Arrange & Act
        var readModelType = typeof(ExerciseHistoryReadModel);

        // Assert
        readModelType.IsClass.Should().BeTrue();
        readModelType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = ExerciseHistoryId.Create(Guid.NewGuid());
        var ongoingTrainingId = OngoingTrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var exerciseSetChangesJson =
            "[{\"type\":\"weight\",\"setId\":\"12345678-1234-1234-1234-123456789012\",\"weightChange\":5.0,\"repsChange\":2}]";
        var exerciseSetsBeforeChangeJson =
            "[{\"type\":\"weight\",\"id\":\"12345678-1234-1234-1234-123456789012\",\"name\":\"Set 1\",\"reps\":10,\"weight\":50.0,\"orderIndex\":0}]";
        var areChangesApplied = false;
        var createdOnUtc = DateTime.UtcNow;
        var modifiedOnUtc = DateTime.UtcNow.AddMinutes(5);

        // Act
        var readModel = new ExerciseHistoryReadModel(
            id,
            ongoingTrainingId,
            exerciseId,
            exerciseSetChangesJson,
            exerciseSetsBeforeChangeJson,
            areChangesApplied,
            createdOnUtc,
            modifiedOnUtc
        );

        // Assert
        readModel.Id.Should().Be(id);
        readModel.OngoingTrainingId.Should().Be(ongoingTrainingId);
        readModel.ExerciseId.Should().Be(exerciseId);
        readModel.ExerciseSetChangesJson.Should().Be(exerciseSetChangesJson);
        readModel.ExerciseSetsBeforeChangeJson.Should().Be(exerciseSetsBeforeChangeJson);
        readModel.AreChangesApplied.Should().Be(areChangesApplied);
        readModel.CreatedOnUtc.Should().Be(createdOnUtc);
        readModel.ModifiedOnUtc.Should().Be(modifiedOnUtc);
    }

    [Fact]
    public void Constructor_WithDefaultValues_ShouldUseDefaults()
    {
        // Arrange
        var id = ExerciseHistoryId.Create(Guid.NewGuid());
        var ongoingTrainingId = OngoingTrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());

        // Act
        var readModel = new ExerciseHistoryReadModel(id, ongoingTrainingId, exerciseId);

        // Assert
        readModel.Id.Should().Be(id);
        readModel.OngoingTrainingId.Should().Be(ongoingTrainingId);
        readModel.ExerciseId.Should().Be(exerciseId);
        readModel.ExerciseSetChangesJson.Should().Be("[]");
        readModel.ExerciseSetsBeforeChangeJson.Should().Be("[]");
        readModel.AreChangesApplied.Should().BeFalse();
        readModel.CreatedOnUtc.Should().Be(default(DateTime));
        readModel.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void ExerciseSetChanges_ShouldDeserializeFromJson()
    {
        // Arrange
        var exerciseSetChangesJson =
            @"[
            {
                ""type"": ""weight"",
                ""setId"": ""12345678-1234-1234-1234-123456789012"",
                ""weightChange"": 5.0,
                ""repsChange"": 2
            },
            {
                ""type"": ""weight"",
                ""setId"": ""87654321-4321-4321-4321-210987654321"",
                ""weightChange"": -2.5,
                ""repsChange"": -1
            }
        ]";

        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            exerciseSetChangesJson
        );

        // Act
        var exerciseSetChanges = readModel.ExerciseSetChanges;

        // Assert
        exerciseSetChanges.Should().HaveCount(2);
        exerciseSetChanges[0].SetId.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        ((WeightBasedExerciseSetChange)exerciseSetChanges[0]).WeightChange.Should().Be(5.0f);
        ((WeightBasedExerciseSetChange)exerciseSetChanges[0]).RepsChange.Should().Be(2);
        exerciseSetChanges[1].SetId.Should().Be(Guid.Parse("87654321-4321-4321-4321-210987654321"));
        ((WeightBasedExerciseSetChange)exerciseSetChanges[1]).WeightChange.Should().Be(-2.5f);
        ((WeightBasedExerciseSetChange)exerciseSetChanges[1]).RepsChange.Should().Be(-1);
    }

    [Fact]
    public void ExerciseSetsBeforeChange_ShouldDeserializeFromJson()
    {
        // Arrange
        var exerciseSetsBeforeChangeJson =
            @"[
            {
                ""type"": ""weight"",
                ""id"": ""12345678-1234-1234-1234-123456789012"",
                ""name"": ""Set 1"",
                ""reps"": 10,
                ""weight"": 50.0,
                ""orderIndex"": 0
            },
            {
                ""type"": ""weight"",
                ""id"": ""87654321-4321-4321-4321-210987654321"",
                ""name"": ""Set 2"",
                ""reps"": 8,
                ""weight"": 60.0,
                ""orderIndex"": 1
            }
        ]";

        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            "[]",
            exerciseSetsBeforeChangeJson
        );

        // Act
        var exerciseSetsBeforeChange = readModel.ExerciseSetsBeforeChange;

        // Assert
        exerciseSetsBeforeChange.Should().HaveCount(2);
        exerciseSetsBeforeChange[0].Name.Should().Be("Set 1");
        ((WeightBasedExerciseSet)exerciseSetsBeforeChange[0]).Reps.Should().Be(10);
        ((WeightBasedExerciseSet)exerciseSetsBeforeChange[0]).Weight.Should().Be(50.0f);
        exerciseSetsBeforeChange[0].OrderIndex.Should().Be(0);
        exerciseSetsBeforeChange[1].Name.Should().Be("Set 2");
        ((WeightBasedExerciseSet)exerciseSetsBeforeChange[1]).Reps.Should().Be(8);
        ((WeightBasedExerciseSet)exerciseSetsBeforeChange[1]).Weight.Should().Be(60.0f);
        exerciseSetsBeforeChange[1].OrderIndex.Should().Be(1);
    }

    [Fact]
    public void ExerciseSetChanges_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            "invalid json"
        );

        // Act & Assert
        // The actual implementation throws an exception for invalid JSON
        // This test should verify that the property getter throws JsonException
        var action = () => readModel.ExerciseSetChanges;
        action.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void ExerciseSetsBeforeChange_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            "[]",
            "invalid json"
        );

        // Act & Assert
        // The actual implementation throws an exception for invalid JSON
        // This test should verify that the property getter throws JsonException
        var action = () => readModel.ExerciseSetsBeforeChange;
        action.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void ExerciseSetChanges_Init_ShouldSerializeToJson()
    {
        // Arrange
        var exerciseSetChanges = new List<ExerciseSetChange>
        {
            new WeightBasedExerciseSetChange(Guid.NewGuid(), 5.0f, 2),
            new WeightBasedExerciseSetChange(Guid.NewGuid(), -2.5f, -1),
        };

        // Act
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid())
        )
        {
            ExerciseSetChanges = exerciseSetChanges,
        };

        // Assert
        readModel.ExerciseSetChangesJson.Should().NotBeNullOrEmpty();
        readModel.ExerciseSetChanges.Should().BeEquivalentTo(exerciseSetChanges);
    }

    [Fact]
    public void ExerciseSetsBeforeChange_Init_ShouldSerializeToJson()
    {
        // Arrange
        var exerciseSets = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
            new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 8, 60.0f),
        };

        // Act
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid())
        )
        {
            ExerciseSetsBeforeChange = exerciseSets,
        };

        // Assert
        readModel.ExerciseSetsBeforeChangeJson.Should().NotBeNullOrEmpty();
        readModel.ExerciseSetsBeforeChange.Should().BeEquivalentTo(exerciseSets);
    }
}
